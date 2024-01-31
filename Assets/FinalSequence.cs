using UnityEngine;

public class FinalSequence : MonoBehaviour
{
    [SerializeField] private Camera cinematicCamera;
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private Animator roomAnimator;
    private PlayerController playerController;

    private bool sequenceRunning = false;

    void Start()
    {
        if (PlayerController.instance != null)
        {
            playerController = PlayerController.instance;
        }

        cinematicCamera.enabled = false;
    }

    public void StartSequence()
    {
        if (sequenceRunning)
        {
            return;
        }
        sequenceRunning = true;

        Cursor.lockState = CursorLockMode.Locked;
        playerController.toggleInput = false;
        playerController.enabled = false;
        cinematicCamera.enabled = true;
        cameraAnimator.SetTrigger("Start");
        roomAnimator.SetBool("BreakFloor", false);
        roomAnimator.SetBool("BreakRoom", false);

        ShowHide[] showHides = FindObjectsOfType<ShowHide>();
        foreach (ShowHide showHide in showHides)
        {
            showHide.Hide(1f);
        }
    }

    public void EndSequence()
    {
        if (!sequenceRunning)
        {
            return;
        }

        Application.Quit();
    }
}
