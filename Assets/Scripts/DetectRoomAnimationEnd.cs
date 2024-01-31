using UnityEngine;
using UnityEngine.Events;

public class DetectRoomAnimationEnd : MonoBehaviour
{
    public UnityEvent onFloorBreakEnd;
    public UnityEvent onRoomFixEnd;

    public void OnFloorBreakEnd()
    {
        onFloorBreakEnd.Invoke();
    }

    public void OnRoomFixEnd()
    {
        onRoomFixEnd.Invoke();
    }
}
