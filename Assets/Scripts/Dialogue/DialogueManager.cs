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

    //Dialogue tags
    private const string SPEAKER_TAG = "speaker";
    private const string AUDIO_TAG = "audio";
    private const string RESIZE_TAG = "resize";

    [Tooltip("The panel box resize based on the text size")]
    [SerializeField] private bool m_resizePanel;
    [SerializeField] private int m_preferredWidth;
    [SerializeField] private int m_preferredHeight;
    [SerializeField] private LayoutElement m_layoutElement;


    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_speakerName;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private float m_ExitDialogueTime;

    [Range(0.1f, 0.01f)]
    [SerializeField] private float m_TextVelocity;

    [Header("Continue UI")]
    [Tooltip("UI Image to show when you can continue to the next dialogue line, or end the dialogue.")]
    [SerializeField] private GameObject m_continueIcon;

    [Header("Choices UI")]
    [SerializeField] private GameObject m_choiceButtonPrefab;
    [SerializeField] private GameObject m_buttonContainer;
    private List<GameObject> m_choices;


    //Audio
    private DialogueAudio m_DialogueAudio; 

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

        m_DialogueAudio = GetComponent<DialogueAudio>();
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
        m_continueIcon.SetActive(false);
        m_choices = new List<GameObject>();
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
                string nextLine = m_currentStory.Continue();
                HandleTags(m_currentStory.currentTags);
                m_displayTextCoroutine = StartCoroutine(DisplayText(nextLine));
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
        ResizePanel();
        m_dialogueText.maxVisibleCharacters = 0;
        m_textIsWriting = true;
        m_continueIcon.SetActive(false);

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
            else
            {
                m_DialogueAudio.PlayDialogueSound(m_dialogueText.maxVisibleCharacters, m_dialogueText.text[m_dialogueText.maxVisibleCharacters]);
                m_dialogueText.maxVisibleCharacters++;
                yield return new WaitForSeconds(m_TextVelocity);
            }
        }

        m_textIsWriting = false;
        m_continueIcon.SetActive(true);
        StartCoroutine(DisplayChoices());
    }

    //The panel box resize based on the text size
    private void ResizePanel()
    {
        if (m_resizePanel)
        {
            m_layoutElement.preferredHeight = -1;
            if (m_dialogueText.preferredWidth > m_preferredWidth)
            {
                m_layoutElement.preferredWidth = m_preferredWidth;
            }
            else
                m_layoutElement.preferredWidth = -1;

        }
        else {
            m_layoutElement.preferredHeight = m_preferredHeight;
            m_layoutElement.preferredWidth = m_preferredWidth;
        }
    }

    //cancel the DisplayText courotine and display all the line Instantanally
    private void DisplayTextImmediately()
    {
        m_textIsWriting = false;
        m_continueIcon.SetActive(true);
        StopCoroutine(m_displayTextCoroutine);

        m_dialogueText.maxVisibleCharacters = m_currentStory.currentText.Length;
        StartCoroutine(DisplayChoices());
    }

    #endregion


    #region Choices

    //display the button choices when need it
    private IEnumerator DisplayChoices()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        List<Choice> currentChoices = m_currentStory.currentChoices;

        if (currentChoices.Count == 0) {
            m_optionDisplay = false;
            yield break;
        }
        m_optionDisplay = true;

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            GameObject button = Instantiate(m_choiceButtonPrefab, m_buttonContainer.transform);
            button.gameObject.SetActive(false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[index].text;
            button.GetComponent<Button>().interactable = false;
            //the button resize base on the text on awake
            button.gameObject.SetActive(true); 

            //Assign each button the make choice action
            int i = index; //Save the index value in a different variable to avoid changing it in the next loop
            button.GetComponent<Button>().onClick.AddListener(() => MakeChoice(i));

            m_choices.Add(button);

            index++;
        }

        //set circular button navigation
        int nButtons = m_choices.Count;
        for (int i = 0; i < nButtons; i++)
        {
            var navigation = m_choices[i].GetComponent<Button>().navigation;
            navigation.mode = Navigation.Mode.Explicit;

            navigation.selectOnDown = m_choices[(i + 1) % nButtons].GetComponent<Button>();
            navigation.selectOnUp = m_choices[(i + nButtons - 1) % nButtons].GetComponent<Button>();

            m_choices[i].GetComponent<Button>().navigation = navigation;
        }

        //select the first button
        EventSystem.current.SetSelectedGameObject(m_choices[0]);

        StartCoroutine(EnableButtons());
    }

    //remove all choices
    private void RemoveChoices()
    {
        foreach (GameObject button in m_choices)
            Destroy(button);

        m_choices.Clear();
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
        foreach (string tag in currentTags)
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
                case AUDIO_TAG:
                    m_DialogueAudio.SetAudioInfo(tagValue);
                    break;
                case RESIZE_TAG:
                    ResizeBox(tagValue);
                    break;
                default:
                    Debug.LogError("Unexpecter tag: " + tagKey);
                    break;
            }
        }
    }

    //Display the name of the speaker on the top of the dialogue box
    void DisplaySpeakerName(string name)
    {
        m_speakerName.text = name;
    }

    //enable or dissable the box resizing based on the text size
    void ResizeBox(string toogle)
    {
        if (toogle == "enabled")
            m_resizePanel = true;
        else if (toogle == "disabled")
            m_resizePanel = false;
        else
            Debug.LogError("Error on Resize tag: " + toogle + " option not expected");

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

        //remove current choices and display the next line
        RemoveChoices();
        ContinueStory();
    }

    #endregion
}
