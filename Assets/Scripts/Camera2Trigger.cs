using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera2Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.GetInstance().EnableCamera2();
        }
    }

}
