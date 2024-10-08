using DG.Tweening;
using UnityEngine;

public class StartSequence : MonoBehaviour
{
    [SerializeField] private CanvasGroup logoCanvasGroup;
    [SerializeField] private Animator eyelids;
    [SerializeField] private StartMenu startMenu;
    [SerializeField] private AudioSource musicSource;

    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(logoCanvasGroup.DOFade(1f, 1f));
        sequence.AppendInterval(1.5f);
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
        sequence.AppendCallback(() =>
        {
            if (musicSource != null)
            {
                musicSource.Play();
            }
            startMenu.Show();
        });

        sequence.Play();
    }
}
