using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class StartMenu : MonoBehaviour
{
    private CanvasGroup cg;
    [SerializeField] private CustomButton startButton;
    [SerializeField] private CustomButton exitButton;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        startButton.Disable();
        exitButton.Disable();
    }

    public void Show()
    {
        cg.DOFade(1f, 1f);
        startButton.Enable();
        exitButton.Enable();
        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(startButton.gameObject);
        }
    }

    public void StartGame()
    {
        startButton.Disable();
        exitButton.Disable();
        TransitionManager.instance.TransitionToScene("Level1Scene");
    }

    public void ExitGame()
    {
        if (MenuManager.instance != null)
        {
            MenuManager.instance.ExitGame();
        }
        else
        {
            Application.Quit();
        }
    }
}
