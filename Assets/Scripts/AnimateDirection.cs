using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDirection : MonoBehaviour
{
    public float step;
    public float timeInterval;
    private Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3 direction;
    public float totalSeconds;
    private Rigidbody Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.localPosition;
        Rigidbody = GetComponent<Rigidbody>();
        StartCoroutine(Animate());
    }

    // Update is called once per frame
    public void Update()
    {
       
    }

    IEnumerator Animate()
    {
        float elapsedTime = 0;
        while (totalSeconds > elapsedTime)
        {
            if (transform.localPosition.y < endPosition.y)
            {
                yield return new WaitForSeconds(timeInterval);
                Rigidbody.AddForce(direction * step);
            }
            if (transform.localPosition.y > startPosition.y)
            {
                yield return new WaitForSeconds(timeInterval);
                Rigidbody.AddForce(-direction * step);
            }
            elapsedTime += Time.deltaTime;
        }
            
    }
}
