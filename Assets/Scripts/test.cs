using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VInspector;

public enum testParticle
{
    water = 0,
    milk = 0,
    coffe = 0,
    whisky = 0,
    spill = 0   //The mixture of fluids spill from a cup
}

public class test : MonoBehaviour
{

    Dictionary<fluidType, float> counter = new Dictionary<fluidType, float>();


    


    private void Start()
    {
        counter.Add(fluidType.coffe, 0);
        counter.Add(fluidType.water, 0);
        counter.Add(fluidType.whisky, 0);
        counter.Add(fluidType.milk, 0);
    }


    [Button]
    private void Clear()
    {
        foreach (fluidType type in counter.Keys.ToList())
        {
            counter[type] = 0;
        }


       
    }

    [Button]
    private void AddWater()
    {
        counter[fluidType.water]++;
    }

    [Button]
    private void AddWhisky()
    {
        counter[fluidType.whisky]++;
    }

    [Button]
    private void AddMilk()
    {
        counter[fluidType.milk]++;
    }

    [Button]
    private void AddCoffe()
    {
        counter[fluidType.coffe]++;
    }

    [Button]
    private void ShowStats() {
        Debug.Log("Water: " + counter[fluidType.water]);
        Debug.Log("whisky: " + counter[fluidType.whisky]);
        Debug.Log("milk: " + counter[fluidType.milk]);
        Debug.Log("coffe: " + counter[fluidType.coffe]);
        Debug.Log("-----------------------");

    }



    [Button]
    private void puta()
    {
        StartCoroutine(OuPuta());
    }


    private IEnumerator OuPuta()
    {
        yield return new WaitForSeconds(5);
        //Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        yield return new WaitForSeconds(5);
        Cursor.lockState = CursorLockMode.None;
        //Cursor.;

        Cursor.visible = true;
    }


}
