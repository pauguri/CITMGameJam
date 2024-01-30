using DG.Tweening;
using UnityEngine;

public class ShowHide : MonoBehaviour
{
    private Vector3 startingScale;

    // Start is called before the first frame update
    void Start()
    {
        startingScale = transform.localScale;
    }

    public void Show(float time)
    {
        transform.DOScale(startingScale, time);
    }
    public void Hide(float time)
    {
        transform.DOScale(Vector3.zero, time);
    }
}
