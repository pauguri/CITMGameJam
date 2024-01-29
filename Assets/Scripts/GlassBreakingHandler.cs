using UnityEngine;


public class GlassBreakingHandler : MonoBehaviour
{
    public float time;
    public GameObject ps;

    public void BreakGlass()
    {
        Instantiate(ps, transform.position, Quaternion.identity);
        Destroy(gameObject, time);
    }
}
