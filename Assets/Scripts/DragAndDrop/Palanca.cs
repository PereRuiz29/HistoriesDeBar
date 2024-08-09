using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Palanca : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] private FluidGenerator m_fluidGenerator;

    [Tooltip("The duration of the lever when realese at max distance")]
    [SerializeField] private float m_duration;

    [SerializeField] private SpriteRenderer m_visual;

    private TargetJoint2D m_TargetJoint;
    private SliderJoint2D m_SliderJoint;

    private float m_MaxHeight;
    private float m_MinHeight;

    private float m_visualMaxSize;

    private bool m_canDrag;

    void Start()
    {
        m_TargetJoint = GetComponent<TargetJoint2D>();
        m_SliderJoint = GetComponent<SliderJoint2D>();

        m_MaxHeight = m_SliderJoint.connectedAnchor.y + m_SliderJoint.limits.max;
        m_MinHeight = m_SliderJoint.connectedAnchor.y + m_SliderJoint.limits.min;

        m_visualMaxSize = m_visual.transform.localScale.y;

        m_canDrag = true;
    }

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_canDrag)
            return;

        GameManager.GetInstance().SetDraggingState(true);
        //Change Cursor sprite
        GameManager.GetInstance().SetDraggingCursor();

        m_SliderJoint.useMotor = false;
        m_TargetJoint.target = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_TargetJoint.target = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameManager.GetInstance().SetDraggingState(false);
        //Change Cursor sprite
        GameManager.GetInstance().SetBaseCursor();

        m_SliderJoint.useMotor = true;
    }

    #endregion

    private void Update()
    {
        if (!m_canDrag && gameObject.transform.position.y >= m_MaxHeight)
        {
            m_canDrag = true;
            m_fluidGenerator.StopFluid();
        }

        else if (m_canDrag && gameObject.transform.position.y <= m_MinHeight)
        {
            OnEndDrag(null);
            m_canDrag = false;
            m_fluidGenerator.StartFluid();

            gameObject.transform.DOMoveY(m_MaxHeight, m_duration);
        }

        m_visual.transform.localScale = new Vector3(m_visualMaxSize, m_visualMaxSize * ((transform.position.y  - m_MinHeight) / (m_MaxHeight - m_MinHeight)), 0);
    }


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