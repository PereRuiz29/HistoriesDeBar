using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{

    [SerializeField] virtualCamera m_camera;

    [Tooltip("The duration of the transition bewteen the current chamere and the one selected")]
    [SerializeField] float m_transitionTime = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.GetInstance().ChangeCamera(m_camera, m_transitionTime);

        }
    }
}
