using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Required when using Event data.

[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_contentField;
    [SerializeField] private LayoutElement m_layoutElement;

    [SerializeField] private GameObject m_selectIcon;

    void Start()
    {
        m_layoutElement.enabled = (m_contentField.preferredWidth > m_layoutElement.preferredWidth);
    }

    public void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            m_selectIcon.SetActive(true);
        }
        else
            m_selectIcon.SetActive(false);
    }
}
