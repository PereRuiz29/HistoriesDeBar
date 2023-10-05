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
        Debug.Log("Start");

        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        m_camera = GameObject.Find("Main Camera");
    }

    void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying)
            return;

        m_CharacterMovement.Move(m_Movement * Time.deltaTime);

        //rotate sprite to look at camera, (have to implemented in another script)
        float cameraAngle = m_camera.transform.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, cameraAngle, 0);
    }

    //read input and return movement vector3
    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();

        //aply velocity and gravity
        m_Movement = new Vector3(m_InputVector.x, -m_Gravity, m_InputVector.y) * m_speed;
        //rotate the movent vector to match with the camera
        float cameraAngle = m_camera.transform.eulerAngles.y;
        m_Movement = Quaternion.AngleAxis(cameraAngle, Vector3.up) * m_Movement; 
    }
}