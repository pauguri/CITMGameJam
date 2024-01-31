using UnityEngine;

public class FallTrigger : MonoBehaviour, ITriggerable
{
    [SerializeField] private Vector3 teleportTo = Vector3.zero;

    public void EnterTrigger()
    {
        if (PlayerController.instance != null)
        {
            Debug.Log("boom");
            PlayerController.instance.enabled = false;
            PlayerController.instance.GetComponent<CharacterController>().Move(teleportTo);
            PlayerController.instance.enabled = true;
        }
    }
}
