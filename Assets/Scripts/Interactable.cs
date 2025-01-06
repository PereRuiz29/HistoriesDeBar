using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public void ShowVisualClue()
    {
        VisualClue.SetActive(true);

        TextMeshProUGUI textAction = VisualClue.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        StartCoroutine(ShowTextAction(textAction));
    }

    private IEnumerator ShowTextAction(TextMeshProUGUI textAction)
    {
        textAction.maxVisibleCharacters = 0;
        
        while (textAction.maxVisibleCharacters < textAction.text.Length){ 
        textAction.maxVisibleCharacters++;
            yield return new WaitForSeconds(0.02f);
        }
    }


    public virtual void HideVisualClue()
    {
        VisualClue.SetActive(false);
    }

    public virtual void Interact() { }

}
