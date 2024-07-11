using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VInspector;

public enum drinkType
{
    llarg,
    curt,
    tallat,
    trifasic,
    cigalo,
    cigalocarregat,
    whisky,
    caffe,
    llet,
    putamerda
}

public class CoffeMinigameController : MonoBehaviour
{
    public static CoffeMinigameController instance;

    [SerializeField] private GameObject m_particleContainer;
    [SerializeField] private RectTransform m_CoffeCanvas;
    [SerializeField] private float m_barHeight = -500;

    [SerializeField] private TextMeshProUGUI m_order;
    [SerializeField] private Tray m_tray;

    [Header("Particle Types")]
    [SerializeField] private GameObject m_waterParticle;
    [SerializeField] private GameObject m_milkParticle;
    [SerializeField] private GameObject m_coffeParticle;
    [SerializeField] private GameObject m_whiskyParticle;
    [SerializeField] private GameObject m_spillParticle;

    private Dictionary<drinkType, float> m_drinkOrder;


    public GameObject particleContainer => m_particleContainer;
    public RectTransform coffeCanvas => m_CoffeCanvas;
    public float barHeight => m_barHeight;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one CoffeMinigameController in the scene!");
        }

        instance = this;
    }

    public void OpenCoffeMinigame()
    {
        //Open coffe Minigame
    }

    //Return the prefab for the given type of fluid particle
    public GameObject GetParticle(fluidType type)
    {
        switch (type)
        {
            case fluidType.water:
                return m_waterParticle;   
                
            case fluidType.milk:
                return m_milkParticle;

            case fluidType.coffe:
                return m_coffeParticle;

            case fluidType.whisky:
                return m_whiskyParticle;

            case fluidType.spill:
                return m_spillParticle;

            default:
                Debug.LogError("Fluid Particle type: " + type + " not available");
                return null;
        }
    }

    private void ResetOrder()
    {
        m_drinkOrder.Clear();
    }

    private void SetOrder(Dictionary<drinkType, float> order)
    {
        m_drinkOrder = order;
    }

    [Button]
    private void ShowOrder()
    {
        string order = "Order:<br>";

        Dictionary<drinkType, float> drinks = m_tray.GetDrinksTypes();
        if (drinks != null)
        {
            foreach (KeyValuePair<drinkType, float> drink in drinks)
            {
                order += " " + drink.Key;
                if (drink.Value > 1)
                    order += " x" + drink.Value;
                order += "<br>";
            }
        }

        m_order.text = order;
    }
}