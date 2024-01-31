using UnityEngine;
using UnityEngine.Events;

public class DetectRoomAnimationEnd : MonoBehaviour
{
    public UnityEvent onFloorBreakEnd;

    public void OnFloorBreakEnd()
    {
        onFloorBreakEnd.Invoke();
    }
}
