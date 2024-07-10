using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VInspector;


public class FluidGenerator : MonoBehaviour
{
    [SerializeField] private fluidType m_fluidType;
    private GameObject m_fluidParticle;
    private GameObject m_particlesContainer;

    [Tooltip("Particles per second")]
    [SerializeField] private float m_rate = 50;

    [Header("Initial Velocity")]
    [Tooltip("It gives an initial speed depending on the rotation it has, Ex: As more tilted a bottle is, the faster the fluid comes out")]
    [SerializeField] private bool m_applyVelocity;
    
    [ShowIf("m_applyVelocity")]
    [Range(0.5f, 2.5f)]
    [SerializeField] private float m_velocityMultiplier;
    [EndIf]

    [Header("Randomness")]
    [Tooltip("It modify the initial position of the fluid particle randomly between 0 and randomValue")]
    [SerializeField] private bool m_randomPosition;

    [ShowIf("m_randomPosition")]
    [Tooltip("Shift position a random value from 0 to the given position")]
    [SerializeField] private float m_randomValue; 
    [EndIf]

    private DraggableCup cupReference;
    private float spillColor;

    bool generatingFluid = false;


    private void Start()
    {
        m_particlesContainer = CoffeMinigameController.instance.particleContainer;
        m_fluidParticle = CoffeMinigameController.instance.GetParticle(m_fluidType);
    }


    public void StartFluid(DraggableCup currentFluid = null, float spillColorValue = -1)
    {
        if (generatingFluid)
            return;

        cupReference = currentFluid;
        generatingFluid = true;
        spillColor = spillColorValue;
        InvokeRepeating("GenerateFluid", 0f, 1/m_rate);
    }

    //Stop Generating fluid partiques
    public void StopFluid()
    {
        if (!generatingFluid)
            return;

        generatingFluid = false;
        CancelInvoke("GenerateFluid");
    }

    //Intance a fluid particle
    private void GenerateFluid()
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
            float velocity = transform.rotation.z * m_velocityMultiplier;
            //Turn the angle of rotation of the bottle into a vector2
            Vector2 direction = DegreeToVector2(transform.eulerAngles.z + 90);             
            particle.GetComponent<Rigidbody2D>().AddForce(direction * Mathf.Abs(velocity));
        }

        if (spillColor != -1)
            particle.GetComponent<SpriteRenderer>().material.SetFloat("_ColorValue", spillColor);

        if (cupReference != null && cupReference.currentParticles > 0)
            cupReference.Empty();
        
    }

    //Return a Vector2 of an angle
    private Vector2 DegreeToVector2(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    #region Debug
    //Start Generating, for testing purpouses
    [Button]
    private void StartGenerating()
    {
        StartFluid();
    }

    //Stop Generating, for testing purpouses
    [Button]
    private void StopGenerating()
    {
        StopFluid();
    }

    //Draw a line of the direction of the direction
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Vector2 direction = DegreeToVector2(transform.eulerAngles.z + 90);

        //Debug.Log("2A: " + transform.position);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(direction.x, direction.y, 0) * 50);
    }
    #endregion
}
