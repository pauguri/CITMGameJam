using System.Collections;
using UnityEngine;

public class BreakFloorSequence : MonoBehaviour, ITriggerable
{
    [SerializeField] private Animator roomAnimator;
    private bool isTriggered = false;

    public void EnterTrigger()
    {
        if (isTriggered)
        {
            return;
        }
        isTriggered = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(1f);
        roomAnimator.SetBool("BreakFloor", true);
    }
}
