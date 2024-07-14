using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeMinigameTrigger : Interactable
{
    [Header("Cosas Nose")]
    [SerializeField]
    private string m_order;

    public override void Interact()
    {
        GameManager.GetInstance().EnterCoffeMinigame();
    }
}
