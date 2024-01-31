using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenSequence : MonoBehaviour
{
    [SerializeField] private Image logo;
    [SerializeField] private CanvasGroup creditsCanvasGroup;
    [SerializeField] private CustomButton exitButton;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        exitButton.Disable();
        creditsCanvasGroup.alpha = 0f;

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(logo.DOFade(1f, 1f));
        sequence.AppendInterval(3f);
        sequence.Append(creditsCanvasGroup.DOFade(1f, 1f));
        sequence.AppendCallback(() => exitButton.Enable());
    }

    public void Exit()
    {
        Application.Quit();
    }
}
