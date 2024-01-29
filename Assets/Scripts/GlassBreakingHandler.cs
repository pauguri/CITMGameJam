using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GlassBreakingHandler : MonoBehaviour
{
    public string tag;
    public float time;
    public ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void BreakGlass()
    {
        Destroy(gameObject, time);
        ps.Play();
    }
}
