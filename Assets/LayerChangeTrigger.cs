using UnityEngine;

public class LayerChangeTrigger : MonoBehaviour
{
    public void ChangeLayer(int layer)
    {
        gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layer;
        }
    }
}
