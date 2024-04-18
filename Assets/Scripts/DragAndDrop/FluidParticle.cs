using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fluidType
{
    water,
    coffe,
    wisky
}

public class FluidParticle : MonoBehaviour
{
    [SerializeField] private fluidType m_fluidType;

    public fluidType fluidType => m_fluidType;
}
