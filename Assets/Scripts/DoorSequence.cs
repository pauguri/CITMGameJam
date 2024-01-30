using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LayerChangeTrigger))]
public class DoorSequence : MonoBehaviour
{
    [SerializeField] private AudioSource doorOpenAudioSource;
    [SerializeField] private Animator doorAnimator;
    //[SerializeField] private ScaleByDistance roomScaler;
    [SerializeField] private float walkSpeed = 3f;

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
        yield return new WaitForSeconds(1f);
        doorOpenAudioSource.Play();
        doorAnimator.SetTrigger("Open");
        //roomScaler.enableScaling = true;

        if (PlayerController.instance != null)
        {
            PlayerController.instance.walkSpeed = walkSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (TransitionManager.instance != null)
            {
                TransitionManager.instance.TransitionToSceneGlitch("Level2Scene");
            }
        }
    }
}
