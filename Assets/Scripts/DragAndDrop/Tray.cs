using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Tray : MonoBehaviour
{
    private List<GameObject> m_trayDrinks;

    // Start is called before the first frame update
    void Start()
    {
        m_trayDrinks = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Cup") //This tag its only in the cup fluid trigger
        {
            m_trayDrinks.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Cup")
        {
            m_trayDrinks.Remove(collision.gameObject);
        }
    }

    [Button]
    private void DebugList()
    {
        Debug.Log("-------------");
        Debug.Log("Count: " + m_trayDrinks.Count);
        
        foreach (GameObject a in m_trayDrinks)
        {
            Debug.Log(a.name + " " + a);
        }
   
    }

    [Button]
    private void TestDiccionary()
    {
        Dictionary<drinkType, float> drinkLlist = GetDrinks();

        Debug.Log("-------------");
        Debug.Log("Count: " + drinkLlist.Count);

        int i = 0;
        foreach (KeyValuePair<drinkType, float> cup in drinkLlist)
        {
            i++;
            Debug.Log("Drink "+ cup.Key + ": " + cup.Value);
        }
    }

    //Return a diccionary with all the drinks on the Tray, return null if empty
    public Dictionary<drinkType, float> GetDrinks()
    {
        if (m_trayDrinks.Count == 0)
            return null;

        Dictionary<drinkType, float> drinkLlist = new Dictionary<drinkType, float>();
        foreach (GameObject cup in m_trayDrinks)
        {
            drinkType cupDrinkType = cup.transform.parent.parent.GetComponent<DraggableCup>().drinkType;
            //Add a drink to the Dictionary list
            drinkLlist[cupDrinkType] = drinkLlist.ContainsKey(cupDrinkType) ? drinkLlist[cupDrinkType] + 1 : 1;
        }

        return drinkLlist;
    }
}
