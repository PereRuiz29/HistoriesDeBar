using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;

[ExecuteInEditMode]
[RequireComponent(typeof(Button))]
public class ChoiceButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI m_contentField;
    [SerializeField] private LayoutElement m_layoutElement;

    [SerializeField] private GameObject m_selectIcon;

    void Start()
    {
        m_layoutElement.enabled = (m_contentField.preferredWidth > m_layoutElement.preferredWidth);
        //layout rebuild to force the verticalLayerGroup of the parent work as intented
        LayoutRebuilder.ForceRebuildLayoutImmediate(gameObject.GetComponent<RectTransform>());
        m_selectIcon.SetActive(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        m_selectIcon.SetActive(true);
        transform.DOLocalMoveX(20, 0.2f);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        m_selectIcon.SetActive(false);
        transform.DOLocalMoveX(0, 0.2f);
    }
}
