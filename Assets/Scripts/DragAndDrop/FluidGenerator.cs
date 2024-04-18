using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FluitGenerator : MonoBehaviour
{

    [SerializeField] private GameObject fluidParticle;
    [SerializeField] private GameObject particlesContainer;

    [SerializeField] private bool active;
    [SerializeField] private bool randomPosition;
    [Tooltip("Particles per second")]
    [SerializeField] private float rate;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            if (active & rate > 0)
            {
                if (randomPosition)
                {
                    Vector3 randPosition = new Vector3(Random.Range(0, 20), Random.Range(0, 20), 0);
                    GameObject particle = Instantiate(fluidParticle, transform.position + randPosition, transform.rotation, particlesContainer.transform);
                }
                else
                {
                    GameObject particle = Instantiate(fluidParticle, transform.position, transform.rotation, particlesContainer.transform);
                }

                yield return new WaitForSeconds(1f / rate);
            }
            else
                yield return new WaitForSeconds(1f);
        }
    }
}
