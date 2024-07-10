using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBottle : DraggableObject
{
    private FluidGenerator m_generator;


    [Header("Bottle")]
    [SerializeField] private float m_generateAngle;

    private void Awake()
    {
        m_generator = GetComponentInChildren<FluidGenerator>();
    }

    protected override void Rotate()
    {
        base.Rotate();

        if (isRotating && transform.eulerAngles.z > m_generateAngle && transform.eulerAngles.z < (360 - m_generateAngle))
            m_generator.StartFluid();
        else
            m_generator.StopFluid();
    }

    protected override void EndRotate()
    {
        base.EndRotate();

        m_generator.StopFluid();
    }
}
