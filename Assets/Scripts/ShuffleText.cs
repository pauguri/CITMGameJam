using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShuffleText : MonoBehaviour
{
    [SerializeField] private float shuffleDelay = 0.1f;
    private TextMeshProUGUI textComponent;
    private List<int> swappedChars = new List<int>();
    private List<int> unswappedChars = new List<int>();
    private int textLength;
    private string originalText;

    private Coroutine shuffleCR;
    private Coroutine unshuffleCR;

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

    IEnumerator Shuffle()
    {
        while (swappedChars.Count < textLength)
        {
            int indexToSwap = Random.Range(0, unswappedChars.Count);
            char swappedWithChar = originalText[Random.Range(0, textLength)];
            char[] textArr = textComponent.text.ToCharArray();
            textArr[unswappedChars[indexToSwap]] = swappedWithChar;
            textComponent.text = new string(textArr);

            swappedChars.Add(unswappedChars[indexToSwap]);
            unswappedChars.RemoveAt(indexToSwap);

            yield return new WaitForSeconds(shuffleDelay);
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

            yield return new WaitForSeconds(shuffleDelay);
        }
        textComponent.text = originalText;
    }
}
