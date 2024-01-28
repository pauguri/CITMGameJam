using UnityEngine;
using UnityEngine.Events;

public class DetectAnimationEnd : MonoBehaviour
{
    public UnityEvent onAnimationEnd;

    public void OnAnimationEnd()
    {
        onAnimationEnd.Invoke();
    }
}
