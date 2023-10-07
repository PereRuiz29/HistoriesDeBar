using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [SerializeField] private GameObject m_gameManager;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private float m_ExitDialogueTime;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] m_choices;
    private TextMeshProUGUI[] m_ChoicesText;

    private Story m_currentStory;
    private bool m_dialogueIsPlaying;
    private bool m_optionDisplay;

    //accessor
    public bool dialogueIsPlaying => m_dialogueIsPlaying;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Dialogue Manger in the scene!");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }


    private void Start()
    {
        m_dialogueIsPlaying = false;
        m_optionDisplay = false;
        m_dialoguePanel.SetActive(false);

        m_ChoicesText = new TextMeshProUGUI[m_choices.Length];
        int index = 0;
        foreach(GameObject choice in m_choices)
        {
            m_ChoicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //change action map
        GameManager.GetInstance().EnterDialogue();

        m_currentStory = new Story(inkJSON.text);
        m_dialogueIsPlaying = true;
        m_dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        //change action map
        GameManager.GetInstance().ExitDialogue();

        m_dialogueIsPlaying = false;
        m_dialoguePanel.SetActive(false);
        m_dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (m_currentStory.canContinue)
        {
            m_dialogueText.text = m_currentStory.Continue();

            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    [ContextMenu("Continue")]
    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.performed & !m_optionDisplay)
            ContinueStory();
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = m_currentStory.currentChoices;

        if (currentChoices.Count == 0)
            m_optionDisplay = false;
        else
            m_optionDisplay = true;

        //en ficarho en menu modal arreglara aixo
        if (currentChoices.Count > m_ChoicesText.Length)
        {
            Debug.Log("La UI es cutrilla i ara mateix no da pa mes de dues opcions. Nombre de opcions que demana: " + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            m_choices[index].gameObject.SetActive(true);
            m_choices[index].GetComponent<Button>().enabled = false;
            m_ChoicesText[index].text = choice.text;
            index++;
        }

        //amagar opcions no utilitzades
        for (int i = index; i < m_ChoicesText.Length; i++)
        {
            m_choices[i].gameObject.SetActive(false);
        }

        EventSystem.current.SetSelectedGameObject(m_choices[0]);
        StartCoroutine(EnableButtons());
    }

    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        for (int i = 0; i < m_currentStory.currentChoices.Count; i++)
        {
            m_choices[i].GetComponent<Button>().enabled = true;
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        m_currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
}
