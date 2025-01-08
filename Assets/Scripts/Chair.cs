using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class Chair : MonoBehaviour
{
    [SerializeField] private bool m_HasCharacter;

    [Foldout("Chair Sprites")]
    [SerializeField] private Sprite m_spriteFrontL;
    [SerializeField] private Sprite m_spriteFrontR;
    [SerializeField] private Sprite m_spriteBackL;
    [SerializeField] private Sprite m_spriteBackR;
    [EndFoldout]

    [Foldout("Chair Character Sprites")]
    [SerializeField] private Sprite m_spriteCharacterFrontL;
    [SerializeField] private Sprite m_spriteCharacterFrontR;
    [SerializeField] private Sprite m_spriteCharacterBackL;
    [SerializeField] private Sprite m_spriteCharacterBackR;
    [EndFoldout]

    private SpriteRenderer m_spriteRenderer;

    private Transform m_camera;
    private float m_chairAngle;


    void Start()
    {
        m_chairAngle = transform.eulerAngles.y;
        Invoke("HotFix", 0.1f);
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void HotFix()
    {
        m_camera = GameManager.GetInstance().camera;

    }


    // Update is called once per frame
    void Update()
    {

        if (m_camera == null)
            return;

        //rotate sprite to look at camera, (have to implemented in another script)
        float cameraAngle = m_camera.eulerAngles.y;
        transform.eulerAngles = new Vector3(0, cameraAngle, 0);

        float angle = Mathf.DeltaAngle(m_chairAngle, m_camera.eulerAngles.y);

        if (m_HasCharacter)
        {
            if (angle <= 0 && angle > -90)
                m_spriteRenderer.sprite = m_spriteCharacterFrontL;
            else if (angle <= 90 && angle > 0)
                m_spriteRenderer.sprite = m_spriteCharacterFrontR;
            else if (angle <= 180 && angle > 90)
                m_spriteRenderer.sprite = m_spriteCharacterBackR;
            else
                m_spriteRenderer.sprite = m_spriteCharacterBackL;
        }
        else
        {
            if (angle <= 0 && angle > -90)
                m_spriteRenderer.sprite = m_spriteFrontL;
            else if (angle <= 90 && angle > 0)
                m_spriteRenderer.sprite = m_spriteFrontR;
            else if (angle <= 180 && angle > 90)
                m_spriteRenderer.sprite = m_spriteBackR;
            else
                m_spriteRenderer.sprite = m_spriteBackL;
        }
    }
}
