using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluitGenerator : MonoBehaviour
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


    private Coroutine m_fluidGenerator;

    //Start Generating fluid particles
    public void StartFluid()
    {
        if (m_fluidGenerator != null)
            return;

        m_fluidGenerator = StartCoroutine(GenerateFluid());
    }

    //Stop Generating fluid partiques
    public void StopFluid()
    {
        if (m_fluidGenerator == null)
            return;
            
        StopCoroutine(m_fluidGenerator);
        m_fluidGenerator = null;
    }

    //Generate fluid particles at given rate
    private IEnumerator GenerateFluid()
    {
        while (true)
        {
            GameObject particle;
            if (m_randomPosition)
            {
                Vector3 randPosition = new Vector3(Random.Range(0, m_randomValue), Random.Range(0, m_randomValue) , 0);
                particle = Instantiate(m_fluidParticle, transform.position + randPosition, transform.rotation, m_particlesContainer.transform);
            }
            else
                particle = Instantiate(m_fluidParticle, transform.position, transform.rotation, m_particlesContainer.transform);
            
            //Apply Velocity to the fluid particle 
            if (m_applyVelocity)
            {
                //Add force to the particle
                //The force apply depend of the angle 
                float velocity = ((Mathf.Abs(transform.eulerAngles.z - 180) * -1) + 180) * 0.01f * m_velocityMultiplier;
                //Turn the angle of rotation of the bottle into a vector2
                Vector2 direction = DegreeToVector2(transform.eulerAngles.z + 90);
                particle.GetComponent<Rigidbody2D>().AddForce(direction * velocity);
            }

            yield return new WaitForSeconds(1f / m_rate);
        }
    }


    //Return a Vector2 of an angle
    private Vector2 DegreeToVector2(float angle)
    {
        angle *= Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
}
