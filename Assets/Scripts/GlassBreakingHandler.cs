using UnityEngine;


public class GlassBreakingHandler : MonoBehaviour
{
    public GameObject glassObject;
    public float time;
    public GameObject particles;
    public AudioSource audioSource;

    public void BreakGlass()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        audioSource.Play();
        Destroy(glassObject, time);
    }
}
