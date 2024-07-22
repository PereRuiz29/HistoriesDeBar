using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;
using VInspector;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Draggable Object")]
    [SerializeField] private bool m_canBeDropInSlot;
    [SerializeField] private bool m_canBeDropInBigSlot;
    [SerializeField] private bool m_canRotate;


    [SerializeField] private Vector3 m_positionOffSet;

    private CanvasGroup m_canvasGroup;
    private RectTransform m_CanvasTransform;
    private RectTransform m_ObjectTransform;

    private Vector3 m_initialRotationPosition;

    private TargetJoint2D m_TargetJoint;
    private Slot m_slot;

    private bool m_isDragging;
    protected bool isRotating;

    //The half of the height of the object
    private float m_heightOffset;

    public bool canBeDropInSlot => m_canBeDropInSlot;
    public bool canBeDropInBigSlot => m_canBeDropInBigSlot;
    public float heightOffset => m_heightOffset;


    protected virtual void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_TargetJoint = GetComponent<TargetJoint2D>();

        m_ObjectTransform = GetComponent<RectTransform>();
        m_CanvasTransform = CoffeMinigameManager.instance.coffeCanvas;

        m_slot = null;
        m_isDragging = false;
        isRotating = false;

        m_heightOffset = m_ObjectTransform.rect.height * m_ObjectTransform.localScale.y / 2;
    }



    #region Drag

    //When the object start dragging
    public void OnBeginDrag(PointerEventData eventData)
    {
        //Free the Slot its currently placed in
        if (m_slot != null)
            m_slot.EmptySlot();
        m_slot = null;

        m_isDragging = true;
        GameManager.GetInstance().SetDraggingState(true);


        //Change Cursor sprite
        GameManager.GetInstance().SetDraggingCursor();

        //To be able to be dropped in slot
        m_canvasGroup.blocksRaycasts = false;
    }

    //When its dragging, its called each time the object move
    public void OnDrag(PointerEventData eventData)
    {
        //The target joint follow the cursor a bit of smooth
        if (!isRotating)
            m_TargetJoint.target = Input.mousePosition;
        else
            Rotate();
    }

    //When release the object
    public void OnEndDrag(PointerEventData eventData)
    {
        m_canvasGroup.blocksRaycasts = true;

        m_isDragging = false;
        isRotating = false;

        GameManager.GetInstance().SetDraggingState(false);
        //Change Cursor sprite
        GameManager.GetInstance().SetBaseCursor();

        //If the object is in a slot, don't move it to the bar 
        if (m_slot != null)
            return;

        //Move the object to the bar
        float barHeight = CoffeMinigameManager.instance.barHeight;
        transform.DOLocalMoveY(barHeight + m_heightOffset, 0.35f).SetEase(Ease.InOutCubic);
        transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic);


        float width = m_CanvasTransform.rect.width;
        //If move the object outside the canvas move it backs in
        if (transform.localPosition.x > width / 2.2f)
            transform.DOLocalMoveX(width / 2.2f, 0.35f).SetEase(Ease.OutCubic);
        else if (transform.localPosition.x < -width / 2.2)
            transform.DOLocalMoveX(-width / 2.2f, 0.35f).SetEase(Ease.OutCubic);

        EndRotate();
    }

    #endregion

    #region Rotation

    //Input call the object start rotating
    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!m_canRotate || !m_isDragging)
            return;

        if (context.started)
        {
            m_initialRotationPosition = Input.mousePosition;
            isRotating = true;

        }
        else if (context.canceled)
        {
            isRotating = false;
            transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic);
            m_TargetJoint.target = Input.mousePosition;

            EndRotate();
        }
    }

    //When rotare, its called each time the object move when rotating
    protected virtual void Rotate()
    {
        Vector3 a = Input.mousePosition - m_initialRotationPosition - m_positionOffSet;
        float rotation = Vector3.SignedAngle(Vector3.up, a, Vector3.forward);

        transform.DOLocalRotate(new Vector3(0, 0, rotation), 0.2f);
    }

    //Called when rotation or dragging stops
    protected virtual void EndRotate()
    {

    }

    #endregion

    #region Slot

    //Save the slot reference
    public void EnterSlot(Slot slot)
    {
        m_slot = slot;
    }

    #endregion

    #region Change Pointer
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.GetInstance().SetDragCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.GetInstance().SetBaseCursor();
    }
    #endregion
}
