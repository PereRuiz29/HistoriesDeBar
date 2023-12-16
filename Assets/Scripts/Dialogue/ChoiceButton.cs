using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_contentField;
    [SerializeField] private LayoutElement m_layoutElement;
    [SerializeField] private GameObject m_SelectIcon;
    [SerializeField] private Button m_button;

    void Start()
    {
        m_layoutElement.enabled = (m_contentField.preferredWidth > m_layoutElement.preferredWidth);
    }
}
