using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggableCup : DraggableObject
{
    [SerializeField] private GameObject m_fillGameObject;

    private float totalParticles;
    private float waterParticles;
    private float coffeParticles;
    private float whiskyParticles;


    protected override void Start()
    {
        base.Start();

        totalParticles = waterParticles = coffeParticles = whiskyParticles = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Fluids")
        {
            fluidType type = collision.gameObject.GetComponent<FluidParticle>().fluidType;

            if (type == fluidType.water)
                waterParticles++;
            else if (type == fluidType.coffe)
                coffeParticles++;
            else if (type == fluidType.wisky)
                whiskyParticles++;

            totalParticles++;
            Destroy(collision.gameObject);

            m_fillGameObject.GetComponent<SpriteRenderer>().material.SetFloat("_FillAmount", totalParticles - 60);
            m_fillGameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Float", coffeParticles / (coffeParticles + waterParticles));

            if (totalParticles > 120)
            {
                totalParticles = 0;
                waterParticles = 0;
                coffeParticles = 0;
                whiskyParticles = 0;
            }
        }
    }
}
