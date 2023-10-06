using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [SerializeField] private GameObject gameManager;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] m_choices;
    private TextMeshProUGUI[] ChoicesText;

    [SerializeField] private float m_ExitDialogueTime;

    private Story currentStory;
    private bool m_dialogueIsPlaying;

    //accessor
    public bool dialogueIsPlaying => m_dialogueIsPlaying;


    private bool m_continue;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manger in the scene!");
        }
        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }


    private void Start()
    {
        m_continue = false;

        m_dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        ChoicesText = new TextMeshProUGUI[m_choices.Length];
        int index = 0;
        foreach(GameObject choice in m_choices)
        {
            ChoicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }

    private void Update()
    {
        if (!m_dialogueIsPlaying)
            return;

        if (m_continue) //if submite button
            ContinueStory();
        m_continue = false;
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        GameManager.GetInstance().EnterDialogue();

        currentStory = new Story(inkJSON.text);
        m_dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);

        ContinueStory();
    }

    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(m_ExitDialogueTime);

        m_dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            dialogueText.text = currentStory.Continue();

            DisplayChoices();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
        }
    }

    [ContextMenu("Continue")]
    public void Continue()
    {
        ContinueStory();
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        //en ficarho en menu modal arreglara aixo
        if(currentChoices.Count > ChoicesText.Length)
        {
            Debug.Log("La UI es cutrilla i ara mateix no da pa mes de dues opcions. Nombre de opcions que demana: " + currentChoices.Count);
        }

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            m_choices[index].gameObject.SetActive(true);
            ChoicesText[index].text = choice.text;
            index++;
        }

        //amagar opcions no utilitzades
        for (int i = index; i < ChoicesText.Length; i++)
        {
            m_choices[i].gameObject.SetActive(false);
        }

        EventSystem.current.SetSelectedGameObject(m_choices[0]);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
    }
}
