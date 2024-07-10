using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum fluidType
{
    water,
    milk,
    coffe,
    whisky,
    spill   //The mixture of fluids spill from a cup
}

public class FluidParticle : MonoBehaviour
{
    [SerializeField] private fluidType m_fluidType;

    public fluidType fluidType => m_fluidType;
}
