using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject m_content;

    [SerializeField] private Image m_background;
    [SerializeField] private TextMeshProUGUI m_title;

    [SerializeField] private Button[] m_buttons;
    [SerializeField] private TextMeshProUGUI m_credits;

    [SerializeField] private float m_cameraMovementTime = 8;


    private void Start()
    {
        m_content.SetActive(false);
    }

    [Button]
    private void OpenMenu()
    {
        ShowMenu();
    }

    private void ShowMenu()
    {
        m_content.SetActive(true);

        m_title.alpha = 0;
        m_background.DOFade(0, 0);

        //Hide Buttons
        foreach (Button button in m_buttons)
        {
            button.image.fillAmount = 0;
            button.GetComponentInChildren<TextMeshProUGUI>().alpha = 0;
        }

        //Hide credits
        m_credits.alpha = 0;

        m_title.DOFade(1, 0.3f);

        Sequence sequence = DOTween.Sequence();
        sequence
            .Insert(0, m_buttons[0].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc))
            .Insert(0.1f, m_buttons[1].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc));
        //.Insert(0.2f, m_buttons[2].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc));

        sequence
            .Insert(0.3f, m_buttons[0].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f))
            .Insert(0.4f, m_buttons[1].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f));
        //.Insert(0.5f, m_buttons[2].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f));

        sequence
            .Insert(0.3f, m_credits.DOFade(1, 0.2f))
            .Insert(0.1f, m_background.DOFade(0.3f, 0.2f));


        sequence.AppendInterval(0.1f);
        sequence.SetUpdate(true);


        //return sequence.Duration();
    }
}
