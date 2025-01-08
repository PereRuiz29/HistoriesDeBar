using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using VInspector;

public enum virtualCamera
{
    camera0 = 0,
    camera1 = 1,
    camera2 = 2,
    camera3 = 3
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    DialogueManager m_dilogueManager;
    CoffeMinigameManager m_coffeMinigameManager;

    [SerializeField] private PlayerInput m_playerInput;
    [SerializeField] private PlayerInput m_dialogueInput;
    [SerializeField] private PlayerInput m_pauseInput; //not inplemented

    [SerializeField] private GameObject m_endMenu;
    [SerializeField] private GameObject m_pauseMenu;


    [Foldout("Virtual Cameras")]
    [SerializeField] private GameObject[] m_virtualCameras;
    [EndFoldout]

    [Foldout("Cursor")]
    [SerializeField] private Texture2D m_cursorBase;
    [SerializeField] private Texture2D m_cursorDrag;
    [SerializeField] private Texture2D m_cursorDragging;
    [EndFoldout]

    private int ordersReady = 4;

    private GameObject m_myEventSystem;

    private Transform m_camera;
    private bool m_IsDragging;

    public enum state
    {
        player,
        dialogue,
        dragAndDrop
    }

    private state m_CurrentState;

    private virtualCamera m_virtualCamera;

    private bool m_canOpenPauseMenu = false;

    Dictionary<drinkType, float> m_drinkOrder;
    Dictionary<drinkType, float> m_trayDrinks;

    public Dictionary<drinkType, float> drinkOrder => m_drinkOrder;
    public Dictionary<drinkType, float> trayDrinks => m_trayDrinks;
    public Transform camera => m_camera;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Manger in the scene!");
        }
        instance = this;


        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

        m_dilogueManager = DialogueManager.GetInstance();
        m_coffeMinigameManager = CoffeMinigameManager.GetInstance();
        m_camera = GameObject.Find("Main Camera").transform;
        m_myEventSystem = GameObject.Find("Event System");
        SetBaseCursor();
    }

    public static GameManager GetInstance()
    {
        return instance;
    }


    private void Start()
    {
        m_CurrentState = state.player;
        m_pauseMenu.SetActive(false);
        PauseGame();
    }

    public void OrderReady()
    {
        ordersReady--;
        if (ordersReady == 0)
            Invoke("EndGame", 1f);
    }
    
    private void EndGame()
    {
        m_endMenu.SetActive(true);
    }



    #region Dialogue
    public void EnterDialogue(TextAsset inkJSON)
    {
        m_playerInput.actions.FindActionMap("Player").Disable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Enable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

        m_dilogueManager.EnterDialogueMode(inkJSON); //Call the Dialogue Manager
    }

    public void ExitDialogue()
    {
        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

    }

    #endregion

    #region Coffe Minigame
    public void EnterCoffeMinigame()
    {
        m_playerInput.actions.FindActionMap("Player").Disable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Enable();

        m_coffeMinigameManager.OpenCoffeMinigame();
    }

    public void ExitCoffeMinigame()
    {
        m_playerInput.actions.FindActionMap("Player").Enable();
        m_dialogueInput.actions.FindActionMap("Dialogue").Disable();
        m_dialogueInput.actions.FindActionMap("dragAndDrop").Disable();

        m_trayDrinks = m_coffeMinigameManager.GetTraydrinks();
        m_coffeMinigameManager.CloseCoffeMinigame();
    }

    public void SetOrder(Dictionary<drinkType, float> order)
    {
        m_drinkOrder = order;
    }

    public bool CheckOrder()
    {
        foreach (KeyValuePair<drinkType, float> drink in m_drinkOrder)
        {
            //if the tray doesn't have the type of drink or the number of this drink is lower than the order
            if (!m_trayDrinks.ContainsKey(drink.Key) || drink.Value > m_trayDrinks[drink.Key])
                return false;
        }

        return true;
    }

    //If the player is currently draggind an object
    public void SetDraggingState(bool IsDragging)
    {
        m_IsDragging = IsDragging;
    }

    #endregion

    #region Cursor

    public void SetBaseCursor()
    {
        if (!m_IsDragging)
            Cursor.SetCursor(m_cursorBase, Vector2.zero, CursorMode.Auto);
    }

    public void SetDragCursor()
    {
        if (!m_IsDragging)
            Cursor.SetCursor(m_cursorDrag, Vector2.zero, CursorMode.Auto);
    }

    public void SetDraggingCursor()
    {
        Cursor.SetCursor(m_cursorDragging, Vector2.zero, CursorMode.Auto);
    }

    #endregion

    #region Virtual Camera
    //Change the active virtual camera newCamera
    public void ChangeCamera(virtualCamera newCamera, float transitionTime = 1)
    {
        m_virtualCameras[0].SetActive(false);
        m_virtualCameras[1].SetActive(false);
        m_virtualCameras[2].SetActive(false);
        m_virtualCameras[3].SetActive(false);


        m_camera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = transitionTime;
        m_virtualCameras[(int)newCamera].SetActive(true);
    }

    #endregion

    public void StartGame(float time)
    {
        ChangeCamera(virtualCamera.camera1, time);

        UnselectCurrentButton();
        Invoke("ResumeGame", time);
    }


    public void PauseGame()
    {
        m_canOpenPauseMenu = false;




        if (m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Disable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Dialogue").Disable();

        //m_pauseInput.actions.FindActionMap("Player").Enable();  
    }

    public void ResumeGame()
    {
        //m_pauseInput.actions.FindActionMap("Player").Disable();

        m_canOpenPauseMenu = true;
        m_pauseMenu.SetActive(false);

        if (m_CurrentState == state.player)
            m_playerInput.actions.FindActionMap("Player").Enable();
        else if (m_CurrentState == state.dialogue)
            m_dialogueInput.actions.FindActionMap("Dialogue").Enable();

    }

    public void OpenPauseMenu(InputAction.CallbackContext context)
    {
        if (!context.performed || !m_canOpenPauseMenu)
            return;

        m_pauseMenu.SetActive(true);
        PauseGame();

        Debug.Log("OpenPauseMenu");
    }

    private void UnselectCurrentButton()
    {
        m_myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }
}
