using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.InputSystem;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private TargetJoint2D m_TargetJoint;

    private CanvasGroup m_canvasGroup;
    private RectTransform m_CanvasTransform;
    private RectTransform m_ObjectTransform;

    [SerializeField] private float barHeight = -500;
    [SerializeField] private bool m_canBeDropInSlot;
    [SerializeField] private bool m_canRotate;

    private Slot m_slot;

    private bool m_isDragging;
    private bool m_isRotating;


    private Vector3 m_initialRotationPosition;
    [SerializeField] private Vector3 m_positionOffSet;

    //The half of the height of the object
    private float m_heightOffset;

    public bool canBeDropInSlot => m_canBeDropInSlot;
    public float heightOffset => m_heightOffset;


    protected virtual void Start()
    {
        m_canvasGroup = GetComponent<CanvasGroup>();
        m_TargetJoint = GetComponent<TargetJoint2D>();

        m_ObjectTransform = GetComponent<RectTransform>();
        m_CanvasTransform = transform.parent.GetComponent<RectTransform>();

        m_slot = null;
        m_isDragging = false;
        m_isRotating = false;

        m_heightOffset = m_ObjectTransform.rect.height * m_ObjectTransform.localScale.y / 2;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Free the Slot its currently placed in
        if (m_slot != null)
            m_slot.EmptySlot();
        m_slot = null;

        m_isDragging = true;

        //To be able to be dropped in slot
        m_canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //The target joint follow the cursor a bit of smooth

        if (!m_isRotating)
            m_TargetJoint.target = Input.mousePosition;
        else
        {
            Vector3 a = Input.mousePosition - m_initialRotationPosition - m_positionOffSet;
            float rotation = Vector3.SignedAngle(Vector3.up, a, Vector3.forward);

            transform.DOLocalRotate(new Vector3(0,0, rotation), 0.2f);
        }


        //transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        m_canvasGroup.blocksRaycasts = true;

        m_isDragging = false;
        m_isRotating = false;

        //If the object is in a slot, don't move it to the bar 
        if (m_slot != null)
            return;

        //Move the object to the bar
        transform.DOLocalMoveY(barHeight + m_heightOffset, 0.35f).SetEase(Ease.InOutCubic);
        transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic);


        float width = m_CanvasTransform.rect.width;
        //If move the object outside the canvas move it backs in
        if (transform.localPosition.x > width / 2.2f)
            transform.DOLocalMoveX(width / 2.2f, 0.35f).SetEase(Ease.OutCubic);
        else if (transform.localPosition.x < -width / 2.2)
            transform.DOLocalMoveX(-width / 2.2f, 0.35f).SetEase(Ease.OutCubic);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (!m_canRotate || !m_isDragging)
            return;

        if (context.started)
        {
            m_initialRotationPosition = Input.mousePosition;
            m_isRotating = true;

        }
        else if (context.canceled)
        {

            m_isRotating = false;
            transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InOutCubic);
            m_TargetJoint.target = Input.mousePosition;
        }
    }

    //Save the slot reference
    public void EnterSlot(Slot slot)
    {
        m_slot = slot;
    }

}
