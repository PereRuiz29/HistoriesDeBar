using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera1Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.GetInstance().EnableCamera1();
        }
    }
}
