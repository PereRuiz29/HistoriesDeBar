using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using VInspector;

public class DraggableCup : DraggableObject
{
    [Header("Cup")]
    [SerializeField] private float m_maxParticles = 500;

    [Foldout("Fluid")]
    [SerializeField] private SpriteRenderer m_fillGameObject;
    [SerializeField] private float m_height;
    [SerializeField] private float m_width;
    [EndFoldout] 

    [Foldout("Spill")]
    [SerializeField] private float minEmptyRate = 1;
    [SerializeField] private float maxEmptyRate = 3;
    [SerializeField] private float exponencialRateBase = 50;
    [SerializeField] private FluidGenerator m_spillGeneratorLeft;
    [SerializeField] private FluidGenerator m_spillGeneratorRight;
    [EndFoldout]

    [Foldout("Debug")]
    [SerializeField] private bool m_ShowDebugStats;
    [SerializeField] private SpriteRenderer m_colorTest;
    [SerializeField] private TextMeshProUGUI m_testText;
    [SerializeField] private TextMeshProUGUI m_coffeTypeText;
    [EndFoldout]

    //Particle Counter
    private float m_percentFill;
    private float m_spillColorValue;
    private float m_totalParticles;

    Dictionary<fluidType, float> m_particleCounter;

    [SerializeField] private drinkType m_drinkType;

    public float currentParticles => m_totalParticles;
    public drinkType drinkType  => m_drinkType;

    protected override void Start()
    {
        base.Start();

        m_particleCounter = new Dictionary<fluidType, float>();
        m_particleCounter[fluidType.coffe] = 0;
        m_particleCounter[fluidType.milk] = 0;
        m_particleCounter[fluidType.water] = 0;
        m_particleCounter[fluidType.whisky] = 0;



        m_fillGameObject.material.SetFloat("_Height", m_fillGameObject.GetComponent<RectTransform>().rect.height);

        HideDebugs(m_ShowDebugStats);

        UpdateFluid();
    }

    #region Fluid

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Fluids"))
        {
            fluidType type = collision.gameObject.GetComponent<FluidParticle>().fluidType;
            m_particleCounter[type] = m_particleCounter.ContainsKey(type) ? m_particleCounter[type] + 1 : 1;

            m_totalParticles++;
           
            UpdateFluid();

            Destroy(collision.gameObject);

            if (m_totalParticles > m_maxParticles)
            {
                Spill();
                //ClearParticleCounter();
            }
        }
    }

    //Set the counter for all particles to 0
    private void ClearParticleCounter() {

        foreach (fluidType type in m_particleCounter.Keys.ToList())
        {
            m_particleCounter[type] = 0;
        }

        m_totalParticles = 0;
    }

    //Update the fluid shader
    private void UpdateFluid()
    {
        m_percentFill = m_totalParticles / m_maxParticles;
        m_spillColorValue = (m_particleCounter[fluidType.coffe] + m_particleCounter[fluidType.whisky]) / currentParticles;

        m_fillGameObject.material.SetFloat("_FillAmount", m_percentFill);
        m_fillGameObject.material.SetFloat("_ColorValue", m_spillColorValue);


        if (m_percentFill < 0.01)
            m_fillGameObject.gameObject.SetActive(false);
        else
            m_fillGameObject.gameObject.SetActive(true);

        m_drinkType = GetDrinkType();
        ShowStats();
        ShowCoffeType();
    }

    #endregion

    #region Rotate

    protected override void Rotate()
    {
        base.Rotate();
        Spill();
    }

    protected override void EndRotate()
    {
        base.EndRotate();
        StopSpilling();
    }

    #endregion

    #region Spill
    public void Empty()
    {
        InvokeRepeating("iEmpty", 0f, 1 / 50);
    }

    private void iEmpty()
    {
        float emptyRate = TransformValueInRange(Mathf.Abs(transform.rotation.z), minEmptyRate, maxEmptyRate, exponencialRateBase);

        m_particleCounter[fluidType.water] -= (m_particleCounter[fluidType.water] / m_totalParticles) * emptyRate;
        m_particleCounter[fluidType.coffe] -= (m_particleCounter[fluidType.coffe] / m_totalParticles) * emptyRate;
        m_particleCounter[fluidType.whisky] -= (m_particleCounter[fluidType.whisky] / m_totalParticles) * emptyRate;

        m_totalParticles -= emptyRate;

        UpdateFluid();
        StopSpilling();
    }


    //Start spilling
    private void Spill()
    {
        if (m_totalParticles < 1)
            return;

        float posFluid = FluidHeight();

        if (m_spillGeneratorLeft.transform.position.y < posFluid)
        {
            m_spillGeneratorLeft.StartFluid(this, m_spillColorValue);
            Empty();
        }

        if (m_spillGeneratorRight.transform.position.y < posFluid)
        {
            m_spillGeneratorRight.StartFluid(this, m_spillColorValue);
            Empty();
        }
    }

    //Stop spilling
    private void StopSpilling()
    {
        float posFluid = FluidHeight();

        //Spill Left
        if (m_totalParticles < 1 || m_spillGeneratorLeft.transform.position.y > posFluid)
        {
            m_spillGeneratorLeft.StopFluid();
            CancelInvoke("iEmpty");
        }

        //Spill Right
        if (m_totalParticles < 1 || m_spillGeneratorRight.transform.position.y > posFluid)
        {
            m_spillGeneratorRight.StopFluid();
            CancelInvoke("iEmpty");
        }
    }

    //Return the height position of the cup fluid
    private float FluidHeight()
    {
        return (-m_height * (1 - m_percentFill) * transform.localScale.y) + (m_height / 2 * transform.localScale.y) + transform.position.y;
    }


    //Transform a given value from a range of [0,1] to a range [𝑥,𝑦], exponientally
    private float TransformValueInRange(float value, float x , float y, float baseValue = 100)
    {
        if (value < 0 || value > 1)
        {
            Debug.LogError("Value (" + value + ") should be between 0 and 1 inclusive.");
            return 0;
        }

        if (value < 0.01f) //Aproximally 0
            return 0.5f;

        float expValue = (Mathf.Pow(baseValue, value) - 1) / (baseValue - 1);
        return (x + (expValue * (y - x)));
    }

    #endregion

    #region Coffe Type

    private drinkType GetDrinkType()
    {
        float whisky = m_particleCounter[fluidType.whisky] / currentParticles * 100;
        float llet = m_particleCounter[fluidType.water] / currentParticles * 100;
        float coffe = m_particleCounter[fluidType.coffe] / currentParticles * 100;


        if (whisky > 95 && m_totalParticles > 100)
            return drinkType.whisky;

        if (llet > 95 && m_totalParticles > 100)
            return drinkType.llet;

        if (coffe > 95 && m_totalParticles > 250 && m_totalParticles < 450)
            return drinkType.cafe;

        if (coffe > 95 && m_totalParticles > 150 && m_totalParticles < 250)
            return drinkType.curt;

        if (coffe > 95 && m_totalParticles > 450)
            return drinkType.llarg;


        if (whisky < 5)
        {
            if (coffe > 40 && coffe < 60 && m_totalParticles > 100 && m_totalParticles < 350)
                return drinkType.tallat;

            if (coffe > 40 && coffe < 60 && m_totalParticles > 350)
                return drinkType.cafeAmbLlet;
        }

        if (llet < 5)
        {
            if (whisky > 15 && whisky < 25 && m_totalParticles > 100 && m_totalParticles < 300)
                return drinkType.rebentat;

            if (whisky > 25 && whisky < 40 && m_totalParticles > 100 && m_totalParticles < 350)
                return drinkType.rebentatCarregat;
        }

        if (coffe < 60 && coffe > 40 && whisky < 15 && whisky > 7)
            return drinkType.trifasic;

        return drinkType.resBo;

    }

    #endregion

    #region Test

    private void ShowCoffeType()
    {
        if (GetDrinkType().ToString() != "resBo")
            m_coffeTypeText.transform.parent.gameObject.SetActive(true);
        else
            m_coffeTypeText.transform.parent.gameObject.SetActive(false);


        m_coffeTypeText.text = GetDrinkType().ToString();
    }

    private void ShowStats()
    {
        HideDebugs(m_ShowDebugStats);

        string stats = "Total: " + currentParticles / m_maxParticles * 100 + "% : " + currentParticles + ",\r\n"
                    + "Water: " + m_particleCounter[fluidType.water] / currentParticles * 100 + "% : " + m_particleCounter[fluidType.water] + ",\r\n" 
                    + "Coffe: " + m_particleCounter[fluidType.coffe] / currentParticles * 100 + "% : " + m_particleCounter[fluidType.coffe] + ",\r\n" 
                    + "Whisky: " + m_particleCounter[fluidType.whisky] / currentParticles * 100 + "% : " + m_particleCounter[fluidType.whisky] + ",\r\n"
                    + "Milk: " + m_particleCounter[fluidType.milk] / currentParticles * 100 + "% : " + m_particleCounter[fluidType.milk] + ",\r\n";

        m_testText.text = stats;

        m_colorTest.material.SetFloat("_ColorValue", m_spillColorValue);
        m_colorTest.material.SetFloat("_FillAmount", 1);

        m_coffeTypeText.text = GetDrinkType().ToString();
    }

    private void HideDebugs(bool hide)
    {
        m_colorTest.transform.parent.gameObject.SetActive(hide);
    }

    #endregion
}
