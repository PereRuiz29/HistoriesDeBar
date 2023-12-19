using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DialogueTween : MonoBehaviour
{
    [Header("Speaker")]
    [SerializeField] private Transform m_speakerBox;
    [SerializeField] private TextMeshProUGUI m_speakerName;
    private bool m_speakerNameIsVisible;

    [Header("Main Box")]
    [SerializeField] private GameObject m_mainBox;
    [SerializeField] private Transform m_continueIcon;
    private Tween m_continueAnimation;

    private void Start()
    {
        m_continueIcon.gameObject.SetActive(false);
    }

    #region Speaker
    //show the speaker box
    public void ShowSpeakerName()
    {
        if (m_speakerNameIsVisible)
            return;

        m_speakerBox.DOLocalMoveY(0f, 0.5f).SetEase(Ease.InOutCirc);
        m_speakerNameIsVisible = true;
    }

    //hide the speaker box, return the animation duration
    public float HideSpeakerName()
    {
        if (!m_speakerNameIsVisible)
            return 0;

        m_speakerNameIsVisible = false;
        return m_speakerBox.DOLocalMoveY(-65f, 0.5f).SetEase(Ease.InOutCirc).Duration();
    }

    //Change the name of the speaker, if the name is hidden will remain hidden
    public void ChangeSpeakerName(string name)
    {
        if (!m_speakerNameIsVisible){
            m_speakerName.text = name;
            return;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(m_speakerBox.DOLocalMoveY(-65f, 0.3f).SetEase(Ease.InOutCirc)
            .OnComplete(() => m_speakerName.text = name));
        seq.AppendInterval(0.2f);
        seq.Append(m_speakerBox.DOLocalMoveY(0f, 0.3f).SetEase(Ease.InOutCirc));
    }
    #endregion

    #region Main Box
    //open dialogue animation
    public void openDialogue()
    {
        m_mainBox.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        m_mainBox.GetComponent<CanvasGroup>().alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.Append(m_mainBox.transform.DOScale(new Vector3(1f, 1f, 1f), 0.25f));
        seq.Join(m_mainBox.GetComponent<CanvasGroup>().DOFade(1, 0.25f)
            .OnComplete(() => ShowSpeakerName()));
    }

    //close dialogue animation
    public void closeDialogue()
    {
        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(HideSpeakerName());
        seq.Append(m_mainBox.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.25f));
        seq.Join(m_mainBox.GetComponent<CanvasGroup>().DOFade(0, 0.25f));

        m_mainBox.transform.localScale = new Vector3(1f, 1f, 1f);
        m_mainBox.GetComponent<CanvasGroup>().alpha = 1;
    }

    public void ShowContinueIcon()
    {
        m_continueIcon.gameObject.SetActive(true);
        m_continueAnimation = m_continueIcon.DOLocalMoveY(2, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    public void hideContinueIcon()
    {
        m_continueAnimation.Kill();
        m_continueIcon.localPosition = Vector3.zero;
        m_continueIcon.gameObject.SetActive(false);
    }

    #endregion

    #region Choices

    public void showChoices()
    {

    }

    #endregion
}
