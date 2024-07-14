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
    private const string SPEAKER_TAG = "speaker";   // show / hide / "speakerName"
    private const string AUDIO_TAG = "audio";       // "audio Info id"
    private const string AUDIOPREDICT_TAG = "audioPredict";     // enable / disable
    private const string RESIZE_TAG = "resize";     // enable / disable
    private const string TEXTVELOCITY_TAG = "textVelocity";     // "value"
    

    [Header("Dialogue Box")]
    [Tooltip("The panel box resize based on the text size")]
    [SerializeField] private bool m_resizePanel;
    [SerializeField] private int m_preferredWidth;
    [SerializeField] private int m_preferredHeight;
    [SerializeField] private LayoutElement m_layoutElement;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject m_dialoguePanel;
    [SerializeField] private TextMeshProUGUI m_dialogueText;
    [SerializeField] private float m_ExitDialogueTime;
    [Range(0.1f, 0.01f)]
    [SerializeField] private float m_TextVelocity;
    private Story m_currentStory;
    private Coroutine m_displayTextCoroutine;


    [Header("Choices UI")]
    [SerializeField] private GameObject m_choiceButtonPrefab;
    [SerializeField] private GameObject m_buttonContainer;
    private List<GameObject> m_choices;

    //Audio
    private DialogueAudio m_DialogueAudio;
    //Animations
    private DialogueTween m_tween;

    private bool m_canEnterDialogue;
    private bool m_optionsDisplay;
    private bool m_textIsWriting;


    #region Enable

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
        m_canEnterDialogue = true;
        m_textIsWriting = false;
        m_optionsDisplay = false;
        m_choices = new List<GameObject>();

        m_DialogueAudio = GetComponent<DialogueAudio>();
        m_tween = GetComponent<DialogueTween>();
        m_dialoguePanel.GetComponent<CanvasGroup>().alpha = 0;
        m_dialoguePanel.SetActive(false);
    }

    //open the dialogue box and handle the input
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //to evoid problems if spam inputs
        if (!m_canEnterDialogue)
            return;

        //change action map
        //GameManager.GetInstance().EnterDialogue();
        m_dialoguePanel.SetActive(true);

        m_currentStory = new Story(inkJSON.text);

        m_tween.openDialogue();
        ContinueStory();
    }

    //close the dialogue box and handle the input
    private IEnumerator ExitDialogueMode()
    {
        m_canEnterDialogue = false;
        yield return new WaitForSeconds(m_ExitDialogueTime);

        //change action map
        GameManager.GetInstance().ExitDialogue();

        //exit animation
        m_tween.closeDialogue();

        yield return new WaitForSeconds(0.7f);
        m_dialogueText.text = "";
        m_canEnterDialogue = true;
        m_dialoguePanel.SetActive(false);
    }

    #endregion

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
        m_dialogueText.maxVisibleCharacters = 0;
        m_textIsWriting = true;
        m_tween.hideContinueIcon();
        ResizePanel();

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
        m_tween.ShowContinueIcon();
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
        m_tween.ShowContinueIcon();
        StopCoroutine(m_displayTextCoroutine);

        m_dialogueText.maxVisibleCharacters = m_currentStory.currentText.Length;
        StartCoroutine(DisplayChoices());
    }

    #endregion

    #region Choices

    //display the button choices when need it
    private IEnumerator DisplayChoices()
    {
        List<Choice> currentChoices = m_currentStory.currentChoices;

        if (currentChoices.Count == 0) {
            m_optionsDisplay = false;
            yield break;
        }
        m_tween.hideContinueIcon();
        m_optionsDisplay = true;

        //to avoid problems with the input
        yield return new WaitForSeconds(m_ExitDialogueTime);

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            GameObject button = Instantiate(m_choiceButtonPrefab, m_buttonContainer.transform);
            button.gameObject.SetActive(false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = currentChoices[index].text;
            button.GetComponent<Button>().interactable = false;

            //Assign each button the make choice action
            int i = index; //Save the index value in a different variable to avoid changing it in the next loop
            button.GetComponent<Button>().onClick.AddListener(() => MakeChoice(i));

            m_choices.Add(button);

            index++;
        }

        //set circular buttons navigation
        int nButtons = m_choices.Count;
        for (int i = 0; i < nButtons; i++)
        {
            var navigation = m_choices[i].GetComponent<Button>().navigation;
            navigation.mode = Navigation.Mode.Explicit;

            navigation.selectOnDown = m_choices[(i + 1) % nButtons].GetComponent<Button>();
            navigation.selectOnUp = m_choices[(i + nButtons - 1) % nButtons].GetComponent<Button>();

            m_choices[i].GetComponent<Button>().navigation = navigation;
        }

        yield return new WaitForSeconds(m_tween.showChoices(m_choices, m_choices.Count));

        //select the first button
        EventSystem.current.SetSelectedGameObject(m_choices[0]);

        EnableButtons();
    }

    //enable all displaced buttons with a little delay time to avoid problems with the input
    private void EnableButtons()
    {
        m_tween.showArrows(m_choices);

        for (int i = 0; i < m_currentStory.currentChoices.Count; i++)
        {
            m_choices[i].GetComponent<Button>().interactable = true;
        }
    }

    //remove all choices
    IEnumerator RemoveChoices(float time)
    {
        yield return new WaitForSeconds(time);
        m_tween.hideArrows();

        foreach (GameObject button in m_choices)
            Destroy(button);

        m_choices.Clear();
        ContinueStory();
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
                case AUDIOPREDICT_TAG:
                    m_DialogueAudio.SetAudioInfo(tagValue);
                    break;
                case RESIZE_TAG:
                    ResizeBox(tagValue);
                    break;
                case TEXTVELOCITY_TAG:
                    TextVeocity(tagValue);
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
        if (name == "show")
            m_tween.ShowSpeakerName();
        else if (name == "hide")
            m_tween.HideSpeakerName();
        else
            m_tween.ChangeSpeakerName(name);
    }

    //enable or dissable the box resizing based on the text size
    void ResizeBox(string toggle)
    {
        if (toggle == "enable")
            m_resizePanel = true;
        else if (toggle == "disable")
            m_resizePanel = false;
        else
            Debug.LogError("Error on Resize tag: " + toggle + " option not expected");

    }


    void TextVeocity(string value)
    {
        m_TextVelocity =  float.Parse(value);
    }


    #endregion

    #region Inputs
    public void OnContinue(InputAction.CallbackContext context)
    {
        if (context.performed & !m_optionsDisplay)
            ContinueStory();
    }

    //reads the options buttons inputs
    public void MakeChoice(int choiceIndex)
    {
        //unable buttons
        for (int i = 0; i < m_currentStory.currentChoices.Count; i++)
        {
            m_choices[i].GetComponent<Button>().interactable = false;
        }

        m_currentStory.ChooseChoiceIndex(choiceIndex);
        m_optionsDisplay = false;

        //remove current choices with an animation and display the next line
        StartCoroutine(RemoveChoices(m_tween.hideChoices(m_choices, 0)));
    }

    #endregion
}
