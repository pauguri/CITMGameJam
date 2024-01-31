using DG.Tweening;
using UnityEngine;

public class FallTrigger : MonoBehaviour, ITriggerable
{
    [SerializeField] private Vector3 teleportTo = Vector3.zero;

    public void EnterTrigger()
    {
        if (PlayerController.instance != null)
        {
            PlayerController.instance.toggleInput = false;
            PlayerController.instance.transform.DOMove(teleportTo, 0f).OnComplete(() =>
                PlayerController.instance.toggleInput = true
            );
        }
    }
}
