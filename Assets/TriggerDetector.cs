using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ITriggerable triggerable))
        {
            triggerable.EnterTrigger();
        }
    }
}
