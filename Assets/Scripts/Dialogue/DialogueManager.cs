using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [SerializeField] private GameObject m_gameManager;

    private const string SPEAKER_TAG = "speaker";

    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_speakerName;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private float m_ExitDialogueTime;

    [Range(0.1f, 0.01f)]
    [SerializeField] private float m_TextVelocity;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] m_choices;
    private TextMeshProUGUI[] m_ChoicesText;

    private Coroutine m_displayTextCoroutine;
    private Story m_currentStory;
    private bool m_dialogueIsPlaying;
    private bool m_optionDisplay;
    private bool m_textIsWriting;

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
        m_textIsWriting = false;
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

    [ContextMenu("Continue")]
    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.performed & !m_optionDisplay)
            ContinueStory();
    }

    private void ContinueStory()
    {
        if (m_currentStory.canContinue)
        {
            //if (m_displayTextCoroutine != null)
            //{
            //    m_textIsWriting = false;
            //    StopCoroutine(m_displayTextCoroutine);
            //}

            if (!m_textIsWriting)
                m_displayTextCoroutine = StartCoroutine(DisplayText(m_currentStory.Continue()));
            else
                DisplayTextImmediately();

            //HandleTags(m_currentStory.currentTags);

        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    private IEnumerator DisplayText(string line)
    {
        m_dialogueText.text = "";
        m_textIsWriting = true;
        HideChoices();


        foreach (char letter in line)
        {
            m_dialogueText.text += letter;
            yield return new WaitForSeconds(m_TextVelocity);
        }

        m_textIsWriting = false;
        DisplayChoices();

        m_displayTextCoroutine = null;
    }

    private void Update()
    {
        Debug.Log("m_textIsWriting: " + m_textIsWriting);
        Debug.Log("m_displayTextCoroutine: " + m_displayTextCoroutine);
    }

    private void DisplayTextImmediately()
    {
        m_textIsWriting = false;
        StopCoroutine(m_displayTextCoroutine);
        m_displayTextCoroutine = null;

        m_dialogueText.text = m_currentStory.currentText;
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
            m_choices[index].GetComponent<Button>().interactable = false;
            m_ChoicesText[index].text = choice.text;
            m_choices[index].gameObject.SetActive(true);
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

    private void HideChoices()
    {
        foreach (GameObject ChoiceButoon in m_choices)
        {
            ChoiceButoon.SetActive(false);
        }
    }

    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        for (int i = 0; i < m_currentStory.currentChoices.Count; i++)
        {
            m_choices[i].GetComponent<Button>().interactable = true;
        }
    }

    private void HandleTags(List<String> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(":");
            if (splitTag.Length != 2)
                Debug.LogError("Tag could not be appropriately parsed: " + tag);

            string tagKey = splitTag[0].Trim(); 
            string tagValue = splitTag[1].Trim();

            //preparat per controlar diferents tags(layout, imatge, etc.), ara mateix nomes en necessit una
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    DisplaySpeakerName(tagValue);
                    break;
            }
        }
    
    }

    void DisplaySpeakerName(string name)
    {
        m_speakerName.text = name;
    }

    public void MakeChoice(int choiceIndex)
    {
        m_currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
}
