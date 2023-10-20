using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//With EnterTrigger its required a RigifBody
[RequireComponent(typeof(Rigidbody))]
public class Interactable : MonoBehaviour
{
    [Header("Visual clue")]
    [SerializeField] private GameObject VisualClue;

    public void Awake()
    {
        VisualClue.SetActive(false);
    }

    //Show when you can Interact with an object
    public virtual void ShowVisualClue()
    {
        VisualClue.SetActive(true);
    }

    public virtual void HideVisualClue()
    {
        VisualClue.SetActive(false);
    }

    public virtual void Interact() { }

}
