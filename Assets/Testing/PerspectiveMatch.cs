using UnityEngine;

public class PerspectiveMatch : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject goalObject;
    [SerializeField] private float tolerance = 0.1f;
    private Light spotlight;

    private void Start()
    {
        spotlight = GetComponent<Light>();
    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(player.position.x, 0, player.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < tolerance)
        {
            Debug.Log("Perspective matched!");
            spotlight.intensity = 0;
            goalObject.SetActive(true);
        }
    }
}
