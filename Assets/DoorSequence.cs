using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LayerChangeTrigger))]
public class DoorSequence : MonoBehaviour
{
    [SerializeField] private AudioSource doorOpenAudioSource;

    private LayerChangeTrigger layerChange;
    private bool sequenceRunning = false;

    void Start()
    {
        layerChange = GetComponent<LayerChangeTrigger>();
    }

    public void StartSequence(int newLayer)
    {
        if (sequenceRunning)
        {
            return;
        }
        sequenceRunning = true;

        layerChange.ChangeLayer(newLayer);
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(2f);
    }
}
