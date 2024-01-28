using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private CanvasGroup startMenuCanvasGroup;
    [SerializeField] private Animator eyelids;
    private bool enableStartInput = false;

    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(logoCanvasGroup.DOFade(1f, 1f));
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() =>
        {
            eyelids.SetTrigger("TriggerClose");
        });

        sequence.Play();
    }

    public void ShowStartMenu()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(startMenuCanvasGroup.DOFade(1f, 1f));
        sequence.AppendCallback(() =>
        {
            enableStartInput = true;
        });

        sequence.Play();
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (enableStartInput && context.started)
        {
            startMenuCanvasGroup.DOKill();
            TransitionManager.instance.TransitionToScene("MainScene");
        }
    }
}
