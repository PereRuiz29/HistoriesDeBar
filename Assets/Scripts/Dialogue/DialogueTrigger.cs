using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual clue")]
    [SerializeField]
    private GameObject m_VisualClue;

    [Header("Ink JSON")]
    [SerializeField]
    private TextAsset m_inkJSON;

    private bool m_playerInRange;

    private bool m_trigger;

    private void Awake ()
    {
        m_trigger = false;
        
        m_playerInRange = false;
        m_VisualClue.SetActive(false);
    }

    private void Update()
    {
        if (m_playerInRange & !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            m_VisualClue.SetActive(true);
            if (m_trigger)
            {
                DialogueManager.GetInstance().EnterDialogueMode(m_inkJSON);
                m_trigger = false;
            }
        }
        else
        {
            m_VisualClue.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            m_playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            m_playerInRange = false;
    }

    [ContextMenu("triggerText")]
    private void triggerText()
    {
        m_trigger = true;
    }

}
