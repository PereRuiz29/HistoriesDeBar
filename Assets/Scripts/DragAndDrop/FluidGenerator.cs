using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluidGenerator : MonoBehaviour
{
    [SerializeField] private GameObject m_fluidParticle;
    [SerializeField] private GameObject m_particlesContainer;

    [Tooltip("Particles per second")]
    [SerializeField] private float m_rate = 1;

    [Header("Initial Velocity")]
    [Tooltip("It gives an initial speed depending on the rotation it has, Ex: As more tilted a bottle is, the faster the fluid comes out")]
    [SerializeField] private bool m_applyVelocity;
    [Range(0.5f, 2.5f)]
    [SerializeField] private float m_velocityMultiplier;

    [Header("Randomess")]
    [SerializeField] private bool m_randomPosition;
    [Tooltip("Shift position a random value from 0 to the given position")]
    [SerializeField] private float m_randomValue;

    private DraggableCup cupReference;

    bool generatingFluid = false;

    public void StartFluid(DraggableCup currentFluid = null)
    {
        if (generatingFluid)
            return;

        cupReference = currentFluid;
        generatingFluid = true;
        InvokeRepeating("GenerateFluid2", 0f, 1/m_rate);
    }

    //Stop Generating fluid partiques
    public void StopFluid()
    {
        if (!generatingFluid)
            return;

        generatingFluid = false;
        CancelInvoke("GenerateFluid2");
    }

    private void GenerateFluid2()
    {
        GameObject particle;
        if (m_randomPosition)
        {
            Vector3 randPosition = new Vector3(Random.Range(0, m_randomValue), Random.Range(0, m_randomValue), 0);
            particle = Instantiate(m_fluidParticle, transform.position + randPosition, transform.rotation, m_particlesContainer.transform);
        }
        else
            particle = Instantiate(m_fluidParticle, transform.position, transform.rotation, m_particlesContainer.transform);

        //Apply Velocity to the fluid particle 
        if (m_applyVelocity)
        {
            //Add force to the particle
            //The force apply depend of the angle 
            //float velocity = ((Mathf.Abs(transform.eulerAngles.z - 180) * -1) + 180) * 0.01f * m_velocityMultiplier;
            float velocity = transform.rotation.z * m_velocityMultiplier;
            //Turn the angle of rotation of the bottle into a vector2
            Vector2 direction = DegreeToVector2(transform.eulerAngles.z + 90);             
            particle.GetComponent<Rigidbody2D>().AddForce(direction * Mathf.Abs(velocity));
        }

        if (cupReference != null && cupReference.currentParticles > 0)
            cupReference.Empty();
        
    }

    //Return a Vector2 of an angle
    private Vector2 DegreeToVector2(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 direction = DegreeToVector2(transform.eulerAngles.z + 90);

        //Debug.Log("2A: " + transform.position);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y, 0) * 50);
    }
}
