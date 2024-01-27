using UnityEngine;

public class ShapeProjector : MonoBehaviour
{
    private new Camera camera;
    public Texture ProjectionTexture = null;
    public GameObject[] ProjectionReceivers = null;
    public float Angle = 0.0f;

    Vector4 Vec3ToVec4(Vector3 vec3, float w)
    {
        return new Vector4(vec3.x, vec3.y, vec3.z, w);
    }

    // Use this for initialization
    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Matrix4x4 matProj = Matrix4x4.Perspective(camera.fieldOfView, 1, camera.nearClipPlane, camera.farClipPlane);

        Matrix4x4 matView = Matrix4x4.identity;
        matView = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

        float x = Vector3.Dot(transform.right, -transform.position);
        float y = Vector3.Dot(transform.up, -transform.position);
        float z = Vector3.Dot(transform.forward, -transform.position);

        matView.SetRow(3, new Vector4(x, y, z, 1));

        Matrix4x4 LightViewProjMatrix = matView * matProj;

        if (ProjectionReceivers == null || ProjectionReceivers.Length <= 0)
        {
            return;
        }

        foreach (GameObject imageReceiver in ProjectionReceivers)
        {
            ProjectionTexture.wrapMode = TextureWrapMode.Clamp;
            MeshRenderer renderer = imageReceiver.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                continue;
            }
            renderer.sharedMaterial.SetTexture("_ShadowMap", ProjectionTexture);
            renderer.sharedMaterial.SetMatrix("_ProjectionMatrix", LightViewProjMatrix);
            renderer.sharedMaterial.SetFloat("_Angle", Angle);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        //Gizmos.DrawLine(Vector3.zero, Vector3.forward * 100.0f);
        Gizmos.DrawFrustum(Vector3.zero, 60, 0.1f, 10f, 1);
    }
}
