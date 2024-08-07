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

    private Transform m_camera;

    private CharacterAnimator m_animator;

    void Start()
    {
        m_CharacterMovement = gameObject.GetComponent<CharacterController>();
        m_animator = GetComponent<CharacterAnimator>();
        m_camera = GameManager.GetInstance().camera;
    }

    void Update()
    {
        Movement();
       
        //rotate sprite to look at camera, (have to implemented in another script)
        float cameraAngle = m_camera.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, cameraAngle, 0);
    }

    //calculate and apply movement vector3
    public void Movement()
    {
        //aply velocity and gravity
        m_Movement = new Vector3(m_InputVector.x, -m_Gravity, m_InputVector.y) * m_speed;

        //rotate the movent vector to match with the camera
        float cameraAngle = m_camera.transform.eulerAngles.y;
        Vector3 m_CameraMovement = Quaternion.AngleAxis(cameraAngle, Vector3.up) * m_Movement;

        m_CharacterMovement.Move(m_CameraMovement * Time.deltaTime);


        //Animation
        if (m_Movement.x == 0 && m_Movement.z == 0)
            m_animator.ChangeAnimationState(characterState.character_idle);
        else if (m_Movement.x < 0)
            m_animator.ChangeAnimationState(characterState.character_walkLeft);
        else if (m_Movement.x > 0 )
            m_animator.ChangeAnimationState(characterState.character_walkRight);
        else if (m_Movement.z < 0)
            m_animator.ChangeAnimationState(characterState.character_walkFront);
        else 
            m_animator.ChangeAnimationState(characterState.character_walkBack);

    }

    //read input
    public void OnMove(InputAction.CallbackContext context)
    {
        m_InputVector = context.ReadValue<Vector2>();
    }
}