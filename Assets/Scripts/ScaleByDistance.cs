using UnityEngine;

public class ScaleByDistance : MonoBehaviour
{
    [SerializeField] private Transform reference;
    [SerializeField] private Transform target;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private float maxDistance = 4f;
    [SerializeField] private Vector3 targetScaleMultiplier = Vector3.one;
    private Vector3 initialScale;
    private Vector3 targetScale;
    [HideInInspector] public bool enableScaling = false;

    void Start()
    {
        initialScale = target.localScale;
        targetScale = new Vector3(initialScale.x * targetScaleMultiplier.x, initialScale.y * targetScaleMultiplier.y, initialScale.z * targetScaleMultiplier.z);
    }

    void Update()
    {
        if (!enableScaling)
        {
            transform.localScale = initialScale;
            return;
        }

        float distance = Vector3.Distance(reference.position, target.position);
        float remappedDistance = distance.Remap(maxDistance, minDistance, 0f, 1f);
        transform.localScale = Vector3.Lerp(initialScale, targetScale, remappedDistance);
    }


}
