using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    [Header("Ink JSON")]
    [Tooltip("When the player talk to the NPC for the first time")] //here makes the order
    [SerializeField] private TextAsset m_firstDialogue;
    [Tooltip("When the player talk to the NPC without any drinks")]
    [SerializeField] private TextAsset m_emptyTrayDialogue;
    [Tooltip("When the player talk to the NPC with the wrong order")]
    [SerializeField] private TextAsset m_wrongOrderDialogue;
    [Tooltip("When the player talk to the NPC with the correct order")]
    [SerializeField] private TextAsset m_correctOrderDialogue;
    [Tooltip("When the order is already served")]
    [SerializeField] private TextAsset m_endDialogue;

    private bool m_IsFirstDialogue;
    private bool m_IsAlreadyServed;

    private void Start()
    {
        m_IsFirstDialogue = true;
        m_IsAlreadyServed = false;
    }

public override void Interact()
    {
        if (m_IsFirstDialogue)
        {
            GameManager.GetInstance().EnterDialogue(m_firstDialogue);       //first dialogue
            m_IsFirstDialogue = false;
        }
        else if (m_IsAlreadyServed)
            GameManager.GetInstance().EnterDialogue(m_endDialogue);       //end dialogue

        else if (GameManager.GetInstance().trayDrinks == null)
            GameManager.GetInstance().EnterDialogue(m_emptyTrayDialogue);   //empty tray

        else if (GameManager.GetInstance().CheckOrder())
        {
            GameManager.GetInstance().EnterDialogue(m_correctOrderDialogue);//correct order
            m_IsAlreadyServed = true;
        }
        else
            GameManager.GetInstance().EnterDialogue(m_wrongOrderDialogue);  //wrong order
    }
}
