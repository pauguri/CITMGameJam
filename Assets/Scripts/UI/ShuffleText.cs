using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShuffleText : MonoBehaviour
{
    [SerializeField] private float letterChangeDelay = 0.1f;
    private TextMeshProUGUI textComponent;
    private List<int> swappedChars = new List<int>();
    private List<int> unswappedChars = new List<int>();
    private int textLength;
    private string originalText;

    private Coroutine shuffleCR;
    private Coroutine unshuffleCR;

    private bool burstInProgress = false;

    void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        originalText = textComponent.text;
        textLength = textComponent.text.Length;
        for (int i = 0; i < textLength; i++)
        {
            unswappedChars.Add(i);
        }

        //Sequence s = DOTween.Sequence().SetLoops(-1);
        //s.AppendInterval(2f);
        //s.AppendCallback(() => ToggleShuffle(true));
        //s.AppendInterval(2f);
        //s.AppendCallback(() => ToggleShuffle(false));
    }

    public void ToggleShuffle(bool value)
    {
        if (value)
        {
            if (unshuffleCR != null)
            {
                StopCoroutine(unshuffleCR);
                unshuffleCR = null;
            }
            shuffleCR = StartCoroutine(Shuffle());
        }
        else
        {
            if (shuffleCR != null)
            {
                StopCoroutine(shuffleCR);
                shuffleCR = null;
            }
            unshuffleCR = StartCoroutine(Unshuffle());
        }
    }

    public void BurstShuffle(float delay)
    {
        if (burstInProgress)
        {
            return;
        }
        burstInProgress = true;
        StartCoroutine(BurstShuffleCR(delay));
    }

    IEnumerator BurstShuffleCR(float delay)
    {
        ToggleShuffle(true);
        yield return new WaitForSecondsRealtime(delay);
        ToggleShuffle(false);
        burstInProgress = false;
    }

    IEnumerator Shuffle()
    {
        while (swappedChars.Count < textLength)
        {
            int indexToSwap = Random.Range(0, unswappedChars.Count);
            char[] textArr = textComponent.text.ToCharArray();
            char charToSwap = textArr[unswappedChars[indexToSwap]];

            string originalTextWithoutChar = originalText.Replace(charToSwap.ToString(), "");
            char swappedWithChar = originalTextWithoutChar[Random.Range(0, originalTextWithoutChar.Length)];
            textArr[unswappedChars[indexToSwap]] = swappedWithChar;
            textComponent.text = new string(textArr);

            swappedChars.Add(unswappedChars[indexToSwap]);
            unswappedChars.RemoveAt(indexToSwap);

            yield return new WaitForSecondsRealtime(letterChangeDelay);
        }
    }

    IEnumerator Unshuffle()
    {
        while (swappedChars.Count > 0)
        {
            int indexToSwap = Random.Range(0, swappedChars.Count);
            char swappedWithChar = originalText[swappedChars[indexToSwap]];
            char[] textArr = textComponent.text.ToCharArray();
            textArr[swappedChars[indexToSwap]] = swappedWithChar;
            textComponent.text = new string(textArr);

            unswappedChars.Add(swappedChars[indexToSwap]);
            swappedChars.RemoveAt(indexToSwap);

            yield return new WaitForSecondsRealtime(letterChangeDelay);
        }
        textComponent.text = originalText;
    }
}
