using UnityEngine;

public class LayerChangeTrigger : MonoBehaviour
{
    [SerializeField] GameObject[] children;

    public void ChangeLayer(int layer)
    {
        gameObject.layer = layer;
        foreach (GameObject child in children)
        {
            child.layer = layer;
        }
    }
}
