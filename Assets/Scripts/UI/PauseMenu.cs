using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    private CanvasGroup cg;
    [SerializeField] private CustomButton resumeButton;
    //[SerializeField] private CustomButton settingsButton;
    [SerializeField] private CustomButton creditsButton;
    [SerializeField] private CustomButton exitButton;
    [Space]
    [SerializeField] private CanvasGroup creditsCanvasGroup;
    [SerializeField] private CustomButton creditsBackButton;

    private bool isPaused = false;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;

        resumeButton.Disable();
        //settingsButton.Disable();
        creditsButton.Disable();
        exitButton.Disable();

        creditsCanvasGroup.alpha = 0;
        creditsCanvasGroup.interactable = false;
        creditsCanvasGroup.blocksRaycasts = false;
        creditsBackButton.Disable();
    }

    private void OnEnable()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnPauseClick += Toggle;
        }
    }

    private void OnDisable()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.OnPauseClick -= Toggle;
        }
    }

    public void Toggle()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public void Show()
    {
        if (PlayerController.instance == null)
        {
            return;
        }
        isPaused = true;

        resumeButton.Enable();
        //settingsButton.Enable();
        creditsButton.Enable();
        exitButton.Enable();

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        cg.interactable = true;
        cg.DOFade(1f, 1f).SetUpdate(true);

        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(resumeButton.gameObject);
        }
    }

    public void Hide()
    {
        if (PlayerController.instance == null)
        {
            return;
        }
        isPaused = false;

        resumeButton.Disable();
        //settingsButton.Disable();
        creditsButton.Disable();
        exitButton.Disable();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        cg.interactable = false;
        cg.DOFade(0f, 1f).SetUpdate(true).OnComplete(() =>
        {
            creditsCanvasGroup.alpha = 0;
            creditsCanvasGroup.interactable = false;
            creditsCanvasGroup.blocksRaycasts = false;
            creditsBackButton.Disable();

            if (MenuManager.instance != null)
            {
                MenuManager.instance.SetSelectedGameObject(null);
            }
        });
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

    public void ShowCredits()
    {
        creditsCanvasGroup.DOFade(1f, 1f).SetUpdate(true);
        creditsCanvasGroup.interactable = true;
        creditsCanvasGroup.blocksRaycasts = true;
        creditsBackButton.Enable();

        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(creditsBackButton.gameObject);
        }
    }

    public void HideCredits()
    {
        creditsCanvasGroup.DOFade(0f, 1f).SetUpdate(true);
        creditsCanvasGroup.interactable = false;
        creditsCanvasGroup.blocksRaycasts = false;
        creditsBackButton.Disable();

        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(creditsButton.gameObject);
        }
    }
}
