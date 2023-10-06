using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;


    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private PlayerInput m_dialogueInput;
    [SerializeField] private PlayerInput m_pauseInput; //not inplemented

    public enum state
    {
        player,
        dialogue
    }

    private state m_CurrentState;


    // Start is called before the first frame update
    void Awake()
    {

        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manger in the scene!");
        }
        instance = this;

        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }


    public void EnterDialogue()
    {
        m_playerInput.actions.FindActionMap("Player").Disable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Enable();
    }

    public void ExitDialogue()
    {
        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
    }

    public void PauseGame()
    {
        if(m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Disable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Player").Disable();

        m_pauseInput.actions.FindActionMap("Player").Enable();
    }

    public void ResumeGame()
    {
        m_pauseInput.actions.FindActionMap("Player").Disable();

        if (m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Enable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Player").Enable();

    }

    //public void SwitchActionMap(state newActionMap)
    //{
    //    m_LastState = m_CurrentState;
    //    m_CurrentState = newActionMap;


    //}


}
