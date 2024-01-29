using System.Collections;
using UnityEngine;

public class AutoShuffleText : ShuffleText
{
    [SerializeField] private float shuffleDelayMin = 5f;
    [SerializeField] private float shuffleDelayMax = 10f;
    [SerializeField] private float unshuffleDelay = 0.5f;
    private Coroutine c;

    private void OnEnable()
    {
        c = StartCoroutine(AutoShuffle());
    }

    private void OnDisable()
    {
        StopCoroutine(c);
        c = null;
    }

    IEnumerator AutoShuffle()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.Range(shuffleDelayMin, shuffleDelayMax));
            ToggleShuffle(true);
            yield return new WaitForSecondsRealtime(unshuffleDelay);
            ToggleShuffle(false);
        }
    }
}
