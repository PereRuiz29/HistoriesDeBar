using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 m_InputVector;
    private Vector3 m_Movement;
    private CharacterController m_CharacterMovement;

    [SerializeField]
    private float m_speed;
    [SerializeField]
    private float m_Gravity;

    private GameObject m_camera;

    void Start()
    {
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        m_camera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        //aplly deltatime inside simplemove metode
        m_CharacterMovement.SimpleMove(m_Movement);

        //rotate sprite to look at camer, (have to implemented in another script)
        float cameraAngle = m_camera.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, cameraAngle, 0);
    }

    //read impunt and return movement vector3
    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();

        float cameraAngle = m_camera.transform.eulerAngles.y;
        m_Movement = new Vector3(m_InputVector.x * m_speed, 0, m_InputVector.y * m_speed);
        m_Movement = Quaternion.AngleAxis(cameraAngle, Vector3.up) * m_Movement;
    }

}