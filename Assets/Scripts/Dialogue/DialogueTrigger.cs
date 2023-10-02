using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Visual clue")]
    [SerializeField]
    private GameObject m_VisualClue;

    private bool m_playerInRange;

    private void Awake ()
    {
        m_playerInRange = false;
        m_VisualClue.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            m_VisualClue.SetActive(true);

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            m_VisualClue.SetActive(false);

    }


}
