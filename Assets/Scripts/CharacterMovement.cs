using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

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

        Debug.Log("Camera: " + m_camera.transform.position + " " + m_camera.transform.rotation);
    }

    void Update()
    {
        if (!m_CharacterMovement.isGrounded)
            m_Movement.y = m_Movement.y - m_Gravity;
        else
            m_Movement.y = 0;


        float cameraAngle = m_camera.transform.eulerAngles.y;
        Vector3 aaa = Quaternion.AngleAxis(cameraAngle, Vector3.up) * m_Movement;
        //m_CharacterMovement.Move(Quaternion.AngleAxis(m_camera.transform.rotation.y, Vector3.up) * m_Movement * Time.deltaTime);
        m_CharacterMovement.Move(aaa * Time.deltaTime);

        transform.LookAt(m_camera.transform);
        transform.eulerAngles = new Vector3(0, cameraAngle, 0);


        Debug.Log("Camera: " + m_camera.transform.rotation.y);
        Debug.Log("Moviment: " + m_Movement);
        Debug.Log("Rotacio: " + Quaternion.AngleAxis(45, Vector3.up) * m_Movement);
        Debug.Log("---------------");

    }

    /*
    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
        m_Movement = new Vector3(m_InputVector.x * m_speed, 0, m_InputVector.y * m_speed);
    }
    */
}