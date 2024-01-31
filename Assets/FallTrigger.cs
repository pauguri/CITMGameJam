using System.Collections;
using UnityEngine;

public class FallTrigger : MonoBehaviour, ITriggerable
{
    [SerializeField] private Vector3 teleportTo = Vector3.zero;

    public void EnterTrigger()
    {
        if (PlayerController.instance != null)
        {
            PlayerController pc = PlayerController.instance;
            pc.enabled = false;
            StartCoroutine(Teleport(pc));
        }
    }

    IEnumerator Teleport(PlayerController pc)
    {
        yield return null;
        pc.gameObject.transform.position = teleportTo;
        pc.enabled = true;
    }
}
