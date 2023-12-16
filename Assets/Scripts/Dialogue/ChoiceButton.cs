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

    void Start()
    {
        m_layoutElement.enabled = (m_contentField.preferredWidth > m_layoutElement.preferredWidth);
    }
}
