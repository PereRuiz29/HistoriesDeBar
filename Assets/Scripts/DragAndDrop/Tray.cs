using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Tray : MonoBehaviour
{
    private List<GameObject> m_cupList;

    // Start is called before the first frame update
    void Start()
    {
        m_cupList = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Cup") //This tag its only in the cup fluid trigger
        {
            m_cupList.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Cup")
        {
            m_cupList.Remove(collision.gameObject);
        }
    }

    [Button]
    private void DebugList()
    {
        Debug.Log("-------------");
        Debug.Log("Count: " + m_cupList.Count);
        
        foreach (GameObject a in m_cupList)
        {
            Debug.Log(a.name + " " + a);
        }
   
    }

    [Button]
    private void TestDiccionary()
    {
        Dictionary<drinkType, float> drinkLlist = GetDrinksTypes();

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
    public Dictionary<drinkType, float> GetDrinksTypes()
    {
        Dictionary<drinkType, float> drinkLlist = new Dictionary<drinkType, float>();

        if (m_cupList.Count == 0)
            return null;

        foreach (GameObject cup in m_cupList)
        {
            drinkType cupDrinkType = cup.transform.parent.parent.GetComponent<DraggableCup>().drinkType;
            AddDrink(drinkLlist, cupDrinkType);
        }

        return drinkLlist;
    }

    //Add a drink to the Dictionary list
    private void AddDrink(Dictionary<drinkType, float> drinkLlist, drinkType type)
    {
        if (drinkLlist.ContainsKey(type))
            drinkLlist[type]++;
        else
            drinkLlist.Add(type, 1);
    }
}
