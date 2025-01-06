using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractTrigger : MonoBehaviour
{
    private GameObject m_interactable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            m_interactable = other.gameObject;
            m_interactable.GetComponent<Interactable>().ShowVisualClue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            m_interactable.GetComponent<Interactable>().HideVisualClue();
            m_interactable = null;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (m_interactable != null)
            m_interactable.GetComponent<Interactable>().Interact();
    }
}
