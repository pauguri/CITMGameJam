using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class PauseMenu : MonoBehaviour
{
    private CanvasGroup cg;
    [SerializeField] private CustomButton resumeButton;
    [SerializeField] private CustomButton settingsButton;
    [SerializeField] private CustomButton creditsButton;
    [SerializeField] private CustomButton exitButton;

    private bool isPaused = false;

    void Start()
    {
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;

        resumeButton.Disable();
        settingsButton.Disable();
        creditsButton.Disable();
        exitButton.Disable();

        settingsButton.GetComponent<Button>().onClick.AddListener(() => { Debug.Log("EEEEEE"); });
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

    void Toggle()
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
        settingsButton.Enable();
        creditsButton.Enable();
        exitButton.Enable();

        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        cg.DOFade(1f, 1f).SetUpdate(true);

        if (MenuManager.instance != null)
        {
            MenuManager.instance.SetSelectedGameObject(settingsButton.gameObject);
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
        settingsButton.Disable();
        creditsButton.Disable();
        exitButton.Disable();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        cg.DOFade(0f, 1f);
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
