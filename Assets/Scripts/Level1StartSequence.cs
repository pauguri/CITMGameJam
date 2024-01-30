using UnityEngine;

public class Level1StartSequence : MonoBehaviour
{
    [SerializeField] private Camera cinematicCamera;
    [SerializeField] private Vector3 playerStartingPopsition;
    [SerializeField] private Vector3 playerStartingRotation;
    private PlayerController playerController;

    void Start()
    {
        if (PlayerController.instance != null)
        {
            playerController = PlayerController.instance;
        }

        playerController.toggleInput = false;

        cinematicCamera.enabled = true;
        playerController.cameraComponent.enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void EndSequence()
    {
        playerController.cameraComponent.enabled = true;

        cinematicCamera.enabled = false;
        cinematicCamera.gameObject.SetActive(false);

        playerController.toggleInput = true;
        playerController.transform.position = playerStartingPopsition;
        playerController.transform.eulerAngles = playerStartingRotation;
    }
}
