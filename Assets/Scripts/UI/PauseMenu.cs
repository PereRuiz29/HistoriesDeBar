using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI m_title;

    [SerializeField] private Button[] m_buttons;
    [SerializeField] private TextMeshProUGUI m_credits;

    [SerializeField] private float m_cameraMovementTime = 8;


    private void OnEnable()
    {
        Debug.Log("Helooooow");
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


        yield return new WaitForSecondsRealtime(0.2f);

        while (m_title.maxVisibleCharacters < m_title.text.Length)
        {
            m_title.maxVisibleCharacters++;
            yield return new WaitForSecondsRealtime(0.02f);
        }


        Sequence sequence = DOTween.Sequence();
        sequence
            .Insert(0, m_buttons[0].image.DOFillAmount(1, 0.3f).SetEase(Ease.InOutCirc))
            .Insert(0.1f, m_buttons[1].image.DOFillAmount(1, 0.3f).SetEase(Ease.InOutCirc));
            //.Insert(0.2f, m_buttons[2].image.DOFillAmount(1, 0.5f).SetEase(Ease.InOutCirc));

        sequence
            .Insert(0.3f, m_buttons[0].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.15f))
            .Insert(0.4f, m_buttons[1].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.15f));
        //.Insert(0.5f, m_buttons[2].GetComponentInChildren<TextMeshProUGUI>().DOFade(1, 0.2f));

        sequence.AppendInterval(0.07f);
        sequence.SetUpdate(true);

        StartCoroutine(ShowCredits());
    }


    private IEnumerator ShowCredits()
    {
        while (m_credits.maxVisibleCharacters < m_credits.text.Length)
        {
            m_credits.GetComponent<TextMeshProUGUI>().maxVisibleCharacters++;
            yield return new WaitForSecondsRealtime(0.03f);
        }
    }

    #region Buttons
    public void ResumeGame()
    {
        gameObject.SetActive(false);
        GameManager.GetInstance().ResumeGame();
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
