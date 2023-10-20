using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

//Singleton to handle the dialogues, its called by the dialogue trigger
public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [SerializeField] private GameObject m_gameManager;

    //Dialogue tags
    private const string SPEAKER_TAG = "speaker";

    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_speakerName;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private float m_ExitDialogueTime;

    [Range(0.1f, 0.01f)]
    [SerializeField] private float m_TextVelocity;

    [Header("Continue UI")]
    [Tooltip("UI Image to show when you can continue to the next dialogue line, or end the dialogue.")]
    [SerializeField] private GameObject m_continue;

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
        m_continue.SetActive(false);

        m_ChoicesText = new TextMeshProUGUI[m_choices.Length];
        int index = 0;
        foreach(GameObject choice in m_choices)
        {
            m_ChoicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    //open the dialogue box and handle the input
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //change action map
        GameManager.GetInstance().EnterDialogue();

        m_currentStory = new Story(inkJSON.text);
        m_dialogueIsPlaying = true;
        m_dialoguePanel.SetActive(true);

        ContinueStory();
    }

    //close the dialogue box and handle the input
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        //change action map
        GameManager.GetInstance().ExitDialogue();

        m_dialogueIsPlaying = false;
        m_dialoguePanel.SetActive(false);
        m_dialogueText.text = "";
    }


    #region Text

    private void ContinueStory()
    {
        if (m_textIsWriting)
            DisplayTextImmediately();

        else
        {
            //check if can display the next dialogue line, will return false if there is no more lines OR if there are some options to choose
            if (m_currentStory.canContinue)
            {
                m_displayTextCoroutine = StartCoroutine(DisplayText(m_currentStory.Continue()));

                HandleTags(m_currentStory.currentTags);
            }
            //end of the dialogue
            else
                StartCoroutine(ExitDialogueMode());
        }
    }

    //Display a dialogue line, letter by letter
    private IEnumerator DisplayText(string line)
    {
        m_dialogueText.text = line;
        m_dialogueText.maxVisibleCharacters = 0;
        m_textIsWriting = true;
        m_continue.SetActive(false);
        HideChoices();

        bool isAddingRichTextTag = false;


        foreach (char letter in line)
        {
            //loops until pass the ritch text tag without waiting
            if (letter == '<' || isAddingRichTextTag)
            {
                isAddingRichTextTag = true;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else {
                m_dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(m_TextVelocity);
            }
        }

        m_textIsWriting = false;
        m_continue.SetActive(true);
        StartCoroutine(DisplayChoices());
    }

    //cancel the DisplayText courotine and display all the line Instantanally
    private void DisplayTextImmediately()
    {
        m_textIsWriting = false;
        m_continue.SetActive(true);
        StopCoroutine(m_displayTextCoroutine);

        m_dialogueText.maxVisibleCharacters = m_currentStory.currentText.Length;
        StartCoroutine(DisplayChoices());
    }

#endregion

    #region Choices

    //display of hide the button choices when need it
    private IEnumerator DisplayChoices()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

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

    //hide all choices
    private void HideChoices()
    {
        foreach (GameObject ChoiceButoon in m_choices)
        {
            ChoiceButoon.SetActive(false);
        }
    }

    //enable all displaced buttons with a little delay time to avoid problems with the input
    private IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        for (int i = 0; i < m_currentStory.currentChoices.Count; i++)
        {
            m_choices[i].GetComponent<Button>().interactable = true;
        }
    }

    #endregion

    #region Tags

    //Handle ink diolegue tags
    private void HandleTags(List<String> currentTags)
    {
        foreach(string tag in currentTags)
        {
            string[] splitTag = tag.Split(":");
            if (splitTag.Length != 2)
                Debug.LogError("Tag could not be appropriately parsed: " + tag);

            string tagKey = splitTag[0].Trim(); 
            string tagValue = splitTag[1].Trim();

            //ready to handle differents tags(i just need one for now)
            switch (tagKey)
            {
                case SPEAKER_TAG:
                    DisplaySpeakerName(tagValue);
                    break;
            }
        }
    }

    //Display the name of the speaker on the top of the dialogue box
    void DisplaySpeakerName(string name)
    {
        m_speakerName.text = name;
    }

    #endregion

    #region Inputs
    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.performed & !m_optionDisplay)
            ContinueStory();
    }

    //reads the options buttons inputs
    public void MakeChoice(int choiceIndex)
    {
        m_currentStory.ChooseChoiceIndex(choiceIndex);
        m_optionDisplay = false;
        ContinueStory();
    }

    #endregion
}
