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

    [Header("Choices")]
    private List<Tween> m_arrows;

    private void Start()
    {
        m_continueIcon.gameObject.SetActive(false);
        //save all tween
        m_arrows = new List<Tween>();
    }

    #region Speaker
    //show the speaker box
    public void ShowSpeakerName()
    {
        if (m_speakerNameIsVisible)
            return;

        m_speakerBox.DOLocalMoveY(0f, 0.2f).SetEase(Ease.InOutCirc);
        m_speakerNameIsVisible = true;
    }

    //hide the speaker box, return the animation duration
    public float HideSpeakerName()
    {
        if (!m_speakerNameIsVisible)
            return 0;

        m_speakerNameIsVisible = false;
        return m_speakerBox.DOLocalMoveY(-65f, 0.2f).SetEase(Ease.InOutCirc).Duration();
    }

    //Change the name of the speaker, if the name is hidden will remain hidden
    public void ChangeSpeakerName(string name)
    {
        if (!m_speakerNameIsVisible){
            m_speakerName.text = name;
            return;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(m_speakerBox.DOLocalMoveY(-65f, 0.2f).SetEase(Ease.InOutCirc)
            .OnComplete(() => m_speakerName.text = name));
        seq.AppendInterval(0.1f);
        seq.Append(m_speakerBox.DOLocalMoveY(0f, 0.2f).SetEase(Ease.InOutCirc));
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

    //Show all button in order top to bottom recursively
    //Pre: List of the buttons and the idex of the last button
    //Post: The delay of the animation
    public float showChoices(List<GameObject> m_choices, int index)
    {
        if (index < 1)
            return 0;

        GameObject button = m_choices[index-1];
        button.SetActive(true);
        button.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        button.GetComponent<CanvasGroup>().alpha = 0;

        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(showChoices(m_choices, index -1));
        seq.Append(button.transform.DOScale(Vector3.one, 0.1f)).SetEase(Ease.InOutCirc);
        seq.Join(button.GetComponent<CanvasGroup>().DOFade(1, 0.1f)).SetEase(Ease.InOutCirc);
        return seq.Duration();
    }

    //Hide all the buttons bottom to top recursively
    //Pre: List of the buttons and the idex first
    //Post: The delay of the animation
    public float hideChoices(List<GameObject> m_choices, int index)
    {
        if (index >= m_choices.Count)
            return 0;

        GameObject button = m_choices[index];
        //button.SetActive(true);
        //button.transform.localScale = Vector3.one;
        //button.GetComponent<CanvasGroup>().alpha = 1;
        //button.GetComponent<ChoiceButton>().playAudio = false;


        Sequence seq = DOTween.Sequence();
        seq.PrependInterval(hideChoices(m_choices, index + 1));
        seq.Append(button.transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f)).SetEase(Ease.InOutCirc);
        seq.Join(button.GetComponent<CanvasGroup>().DOFade(0, 0.05f)).SetEase(Ease.InOutCirc);
        return seq.Duration();
    }

    //Set a loop animation to the buttons arrows
    public void showArrows(List<GameObject> m_choices)
    {
        foreach(GameObject button in m_choices)
        {
            GameObject arrow = button.transform.GetChild(1).gameObject;

            Tween arrowAnimation = arrow.transform.DOLocalMoveX(arrow.transform.localPosition.x + 5, 0.5f).SetLoops(-1, LoopType.Yoyo);
            m_arrows.Add(arrowAnimation);
        }
    }
    
    //kill the animation of all arrows
    public void hideArrows()
    {
        foreach(Tween arrowAnimation in m_arrows)
        {
            arrowAnimation.Kill();
        }
        m_arrows.Clear();
    }

    #endregion
}
