using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    [Header("Ink JSON")]
    [SerializeField]
    private TextAsset m_inkJSON;

    public override void Interact()
    {
        GameManager.GetInstance().EnterDialogue(m_inkJSON);
    }
}
