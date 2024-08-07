using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class CoffeMinigameTrigger : Interactable
{
    public override void Interact()
    {
        GameManager.GetInstance().EnterCoffeMinigame();
    }
}
    