using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExitCoffeMinigameButton : MonoBehaviour
{
    public void ExitCoffeMinigame()
    {
        Invoke("Exit", 0.1f);
    }

    private void Exit()
    {
        GameManager.GetInstance().ExitCoffeMinigame();
        EventSystem.current.SetSelectedGameObject(null);
    }
}
