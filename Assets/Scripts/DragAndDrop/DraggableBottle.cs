using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableBottle : DraggableObject
{
    private FluitGenerator m_generator;

    private void Awake()
    {
        m_generator = GetComponentInChildren<FluitGenerator>();
    }

    protected override void Rotate()
    {
        base.Rotate();

        if (isRotating && transform.eulerAngles.z > 45 && transform.eulerAngles.z < 315)
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
