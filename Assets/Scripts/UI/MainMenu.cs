using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI m_title;

    [SerializeField] private Button[] m_buttons;
    [SerializeField] private TextMeshProUGUI m_credits;

    [SerializeField] private float m_cameraMovementTime = 8;


    private void Start()
    {
        StartCoroutine(ShowMenu());
    }

    private IEnumerator ShowMenu()
    {
        m_title.maxVisibleCharacters = 0;
        m_title.alpha = 1;

        //Hide Buttons
        foreach (Button button in m_buttons)
        {
            button.image.fillAmount = 0;
            button.GetComponentInChildren<TextMeshProUGUI>().alpha = 0;
        }

        //Hide credits
        m_credits.maxVisibleCharacters = 0;


        yield return new WaitForSecondsRealtime(1f);

        while (m_title.maxVisibleCharacters < m_title.text.Length)
        {
            m_title.maxVisibleCharacters++;
            yield return new WaitForSecondsRealtime(0.1f);
        }


        Sequence sequence = DOTween.Sequence();
        sequence
            .Insert(0, m_buttons[0].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc))
            .Insert(0.1f, m_buttons[1].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc));
            //.Insert(0.2f, m_buttons[2].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc));

        sequence
            .Insert(0.3f, m_buttons[0].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f))
            .Insert(0.4f, m_buttons[1].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f));
        //.Insert(0.5f, m_buttons[2].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f));

        sequence.AppendInterval(0.1f);
        sequence.SetUpdate(true);

        StartCoroutine(ShowCredits());
    }


    private IEnumerator ShowCredits()
    {
        while (m_credits.maxVisibleCharacters < m_credits.text.Length)
        {
            m_credits.GetComponent<TextMeshProUGUI>().maxVisibleCharacters++;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }

    #region Buttons
    public void StartGame()
    {
        GameManager.GetInstance().StartGame(m_cameraMovementTime);
    }

    public void Quit()
    {
#if UNITY_STANDALONE
        Invoke("Application.Quit()", 2f);
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}
