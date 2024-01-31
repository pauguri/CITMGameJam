using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LayerChangeTrigger))]
public class PencilSequence : MonoBehaviour
{
    [SerializeField] private Animator pencilAnimator;
    [SerializeField] private AudioSource pencilShootAudio;
    [SerializeField] private GlassBreakingHandler glassBreakingHandler;
    [Space]
    [SerializeField] private AudioSource ambient;
    [SerializeField] private AudioSource music;

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
        Debug.Log("shoot");
        pencilAnimator.SetTrigger("Shoot");
        pencilShootAudio.Play();
    }

    public void BreakGlass()
    {
        glassBreakingHandler.BreakGlass();
        ambient.Stop();
        music.Play();
    }
}
