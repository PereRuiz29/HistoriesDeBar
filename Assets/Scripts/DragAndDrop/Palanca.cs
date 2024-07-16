using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Palanca : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    
    [SerializeField] private FluidGenerator m_fluidGenerator;

    [Tooltip("The duration of the lever when realese at max distance")]
    [SerializeField] private float m_duration;

    private TargetJoint2D m_TargetJoint;
    private SliderJoint2D m_SliderJoint;

    private float m_MaxHeight;
    private float m_MinHeight;

    private bool m_canDrag;

    void Start()
    {
        m_TargetJoint = GetComponent<TargetJoint2D>();
        m_SliderJoint = GetComponent<SliderJoint2D>();

        m_MaxHeight = m_SliderJoint.connectedAnchor.y + m_SliderJoint.limits.max;
        m_MinHeight = m_SliderJoint.connectedAnchor.y + m_SliderJoint.limits.min;

        m_canDrag = true;
    }

    #region Drag
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_canDrag)
            return;

        m_SliderJoint.useMotor = false;
        m_TargetJoint.target = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        m_TargetJoint.target = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
    }
}