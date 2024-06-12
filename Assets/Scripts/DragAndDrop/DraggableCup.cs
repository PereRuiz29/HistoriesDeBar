using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;

[CustomEditor(typeof(DraggableCup))]
public class InspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (DraggableCup)target;

        if (script.m_changeSpillRate)
        {
            script.minEmptyRate = EditorGUILayout.FloatField("Min Empty Rate", script.minEmptyRate);
            script.maxEmptyRate = EditorGUILayout.FloatField("Max Empty Rate", script.maxEmptyRate);
            script.exponencialRateBase = EditorGUILayout.FloatField("Exponential Rate Base", script.exponencialRateBase);
        }
    }
}


public class DraggableCup : DraggableObject
{



    [Header("TEST TEXT")]
    [SerializeField] private TextMeshProUGUI testText;
    [SerializeField] private TextMeshProUGUI coffeTipeText;


    [Header("Cup")]
    [SerializeField] private GameObject m_fillGameObject;

    [SerializeField] private float m_maxParticles = 500;

    [SerializeField] private float m_height;
    [SerializeField] private float m_width;

    [SerializeField] private FluidGenerator m_generatorLeft;
    [SerializeField] private FluidGenerator m_generatorRight;

    private float percentFill;

    private float totalParticles;
    private float m_currentParticles;


    private float waterParticles;
    private float coffeParticles;
    private float whiskyParticles;

    [Header("Spill")]
    [SerializeField] public bool m_changeSpillRate;

    [HideInInspector] public float minEmptyRate = 1;
    [HideInInspector] public float maxEmptyRate = 10;
    [HideInInspector] public float exponencialRateBase = 10;


    public float currentParticles => m_currentParticles;

    protected override void Start()
    {
        base.Start();

        totalParticles = waterParticles = coffeParticles = whiskyParticles = 0;
        UpdateFluid();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fluids")
        {
            fluidType type = collision.gameObject.GetComponent<FluidParticle>().fluidType;

            if (type == fluidType.water)
                waterParticles++;
            else if (type == fluidType.coffe)
                coffeParticles++;
            else if (type == fluidType.wisky)
                whiskyParticles++;

            m_currentParticles++;
            totalParticles++;
            Destroy(collision.gameObject);

            UpdateFluid();


            if (m_currentParticles > m_maxParticles)
            {
                m_currentParticles = 0;
                totalParticles = 0;
                waterParticles = 0;
                coffeParticles = 0;
                whiskyParticles = 0;
            }
        }
    }

    //Update the fluid shader
    private void UpdateFluid()
    {
        percentFill = m_currentParticles / m_maxParticles;
        m_fillGameObject.GetComponent<SpriteRenderer>().material.SetFloat("_FillAmount", percentFill);
        m_fillGameObject.GetComponent<SpriteRenderer>().material.SetFloat("_ColorValue", (coffeParticles + whiskyParticles) / currentParticles);
        ShowStats();
    }


    public void Empty()
    {
        InvokeRepeating("iEmpty", 0f, 1 / 50);
    }

    private void iEmpty()
    {
        float emptyRate = TransformValueInRange(Mathf.Abs(transform.rotation.z), minEmptyRate, maxEmptyRate);

        waterParticles -= (waterParticles / m_currentParticles) * emptyRate;
        coffeParticles -= (coffeParticles / m_currentParticles) * emptyRate;
        whiskyParticles -= (whiskyParticles / m_currentParticles) * emptyRate;

        m_currentParticles -= emptyRate;

        UpdateFluid();
        StopSpilling();
    }

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

    //Start spilling
    private void Spill()
    {
        if (m_currentParticles < 1)
            return;

        float posFluid = FluidHeight();

        if (m_generatorLeft.transform.position.y < posFluid)
        {
            m_generatorLeft.StartFluid(this);
            Empty();
        }

        if (m_generatorRight.transform.position.y < posFluid)
        {
            m_generatorRight.StartFluid(this);
            Empty();
        }
    }

    //Stop spilling
    private void StopSpilling()
    {
        float posFluid = FluidHeight();

        //Spill Left
        if (m_currentParticles < 1 || m_generatorLeft.transform.position.y > posFluid)
        {
            m_generatorLeft.StopFluid();
            CancelInvoke("iEmpty");
        }

        //Spill Right
        if (m_currentParticles < 1 || m_generatorRight.transform.position.y > posFluid)
        {
            m_generatorRight.StopFluid();
            CancelInvoke("iEmpty");
        }
    }

    //Return the height position of the cup fluid
    private float FluidHeight()
    {
        return (-m_height * (1 - percentFill) * transform.localScale.y) + (m_height / 2 * transform.localScale.y) + transform.position.y;
    }


    //Transform a given value from a range of [0,1] to a range [𝑥,𝑦], exponientally
    private float TransformValueInRange(float value, float x , float y, float baseValue = 100)
    {
        if (value <= 0 && value >= 1)
        {
            Debug.LogError("Value (" + value + ") should be between 0 and 1 inclusive.");
            return 0;
        }

        float expValue = (Mathf.Pow(baseValue, value) - 1) / (baseValue - 1);
        return x + expValue * (y - x);
    }


    private string CoffeType()
    {
        float whisky = whiskyParticles / currentParticles * 100;
        float llet = waterParticles / currentParticles * 100;
        float coffe = coffeParticles / currentParticles * 100;


        if (whisky > 95)
            return "Whisky";

        if (llet > 95)
            return "LLet";

        if (coffe > 95)
            return "Caffe";


        if (whisky < 5)
        {
            if (coffe < 40 && coffe > 20)
                return "Curt";

            if (coffe > 40 && coffe < 60)
                return "Tallat";

            if (coffe < 80 && coffe > 60)
                return "Llarg";
        }

        if (llet < 5)
        {
            if (whisky > 15 && whisky < 25)
                return "Cigalo";

            if (whisky > 25 && whisky < 40)
                return "Cigalo ben carregat";
        }

        if (coffe < 60 && coffe > 40 && whisky < 15 && whisky > 7)
            return "Trifasic";

        return "Puta merda";

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + new Vector3(0, (- m_height * (1- percentFill) * transform.localScale.y) + (m_height/2 * transform.localScale.y), 0), transform.position + new Vector3(m_width / 2 * 10, (m_height * (1 - percentFill)) * 10, 0));
    }

    private void ShowStats()
    {
        string stats = "Total: " + currentParticles / m_maxParticles * 100 + "% : " + currentParticles + ",\r\n"
                    + "Water: " + waterParticles / currentParticles * 100 + "% : " + waterParticles + ",\r\n" 
                    + "Coffe: " + coffeParticles / currentParticles * 100 + "% : " + coffeParticles + ",\r\n" 
                    + "Whisky: " + whiskyParticles / currentParticles * 100 + "% : " + whiskyParticles;

        testText.text = stats;

        coffeTipeText.text = CoffeType();
    }

}
