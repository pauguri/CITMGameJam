using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FigmentProjector : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] private Texture projectionTexture = null;
    [SerializeField] private GameObject[] projectionReceivers = null;
    [SerializeField] private int projectionLayer = 1;
    [Space]
    [SerializeField] private float positionTolerance = 0.1f;
    [SerializeField] private float rotationTolerance = 20f;
    [SerializeField] private UnityEvent onMatch;
    [SerializeField] private CanvasGroup confirmCanvas;
    private bool isMatched = false;
    private bool canConfirm = false;

    void Start()
    {
        Camera camera = Camera.main;

        Matrix4x4 matProj = Matrix4x4.Perspective(camera.fieldOfView, 1, camera.nearClipPlane, camera.farClipPlane);
        Matrix4x4 matView = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

        float x = Vector3.Dot(transform.right, -transform.position);
        float y = Vector3.Dot(transform.up, -transform.position);
        float z = Vector3.Dot(transform.forward, -transform.position);

        matView.SetRow(3, new Vector4(x, y, z, 1));

        Matrix4x4 viewProjMatrix = matView * matProj;

        if (projectionReceivers == null || projectionReceivers.Length <= 0)
        {
            return;
        }

        foreach (GameObject receiver in projectionReceivers)
        {
            projectionTexture.wrapMode = TextureWrapMode.Clamp;
            MeshRenderer renderer = receiver.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                continue;
            }
            renderer.sharedMaterial.SetTexture("_ProjectedTex" + projectionLayer, projectionTexture);
            renderer.sharedMaterial.SetVector("_ViewDirection" + projectionLayer, transform.forward);
            renderer.sharedMaterial.SetMatrix("_ProjectionMatrix" + projectionLayer, viewProjMatrix);
            renderer.sharedMaterial.SetFloat("_ShowProjection" + projectionLayer, 1f);
        }

        player.OnConfirmClick += TryConfirmFigment;
    }

    void Update()
    {
        if (isMatched)
        {
            return;
        }

        bool positionMatch = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < positionTolerance;
        bool rotationMatch = Vector3.Angle(player.transform.forward, transform.forward) < rotationTolerance;

        Debug.Log("Position Match: " + positionMatch + " Rotation Match: " + rotationMatch);

        if (positionMatch && rotationMatch)
        {
            if (confirmCanvas != null)
            {
                confirmCanvas.DOFade(1f, 0.5f);
            }
            canConfirm = true;
        }
        else
        {
            if (confirmCanvas != null)
            {
                confirmCanvas.DOFade(0f, 0.5f);
            }
            canConfirm = false;
        }
    }

    void TryConfirmFigment()
    {
        if (!canConfirm)
        {
            return;
        }

        foreach (GameObject receiver in projectionReceivers)
        {
            receiver.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_ShowProjection" + projectionLayer, 0f);
        }
        if (confirmCanvas != null)
        {
            confirmCanvas.DOFade(0f, 0.5f);
        }
        player.OnConfirmClick -= TryConfirmFigment;
        onMatch.Invoke();
        canConfirm = false;
        isMatched = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, 60, 0.1f, 10f, 1);
    }
}
