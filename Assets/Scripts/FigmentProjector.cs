using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FigmentProjector : MonoBehaviour
{
    public PlayerController player;
    [SerializeField] private float focusWalkSpeed = 2f;
    [SerializeField] private Texture projectionTexture = null;
    [SerializeField] private GameObject[] projectionReceivers = null;
    [SerializeField] private int projectionLayer = 1;
    [Space]
    [SerializeField] private float positionFocusTolerance = 0.5f;
    [SerializeField] private float positionMatchTolerance = 0.2f;
    [SerializeField] private float rotationTolerance = 20f;
    [Space]
    [SerializeField] private UnityEvent onMatch;
    [SerializeField] private CanvasGroup confirmCanvas;
    private bool isMatched = false;
    private bool canConfirm = false;
    private float originalWalkSpeed;

    private Camera cam;
    private Tween confirmFadeIn;
    private Tween confirmFadeOut;

    void Start()
    {
        cam = Camera.main;

        Matrix4x4 matProj = Matrix4x4.Perspective(cam.fieldOfView, 1, cam.nearClipPlane, cam.farClipPlane);
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

        originalWalkSpeed = player.walkSpeed;
        player.OnConfirmClick += TryConfirmFigment;
    }

    void Update()
    {
        if (isMatched)
        {
            return;
        }

        bool positionFocus = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < positionFocusTolerance;
        bool rotationFocus = Vector3.Angle(cam.transform.forward, transform.forward) < rotationTolerance;

        if (positionFocus && rotationFocus)
        {
            player.walkSpeed = focusWalkSpeed;

            bool positionMatch = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < positionMatchTolerance;

            if (positionMatch)
            {
                if (confirmCanvas != null && (confirmFadeIn == null || !confirmFadeIn.active))
                {
                    confirmFadeIn = confirmCanvas.DOFade(1f, 0.5f);
                }
                canConfirm = true;
                return;
            }
        }
        else
        {
            player.walkSpeed = originalWalkSpeed;
        }

        if (confirmCanvas != null && (confirmFadeOut == null || !confirmFadeOut.active))
        {
            confirmFadeOut = confirmCanvas.DOFade(0f, 0.5f);
        }
        canConfirm = false;
    }

    void TryConfirmFigment()
    {
        if (!canConfirm)
        {
            return;
        }

        canConfirm = false;
        isMatched = true;
        player.OnConfirmClick -= TryConfirmFigment;

        player.toggleInput = false;
        Vector3 targetPosition = new Vector3(transform.position.x, player.transform.position.y, transform.position.z);
        player.transform.DOMove(targetPosition, 0.5f).OnComplete(() =>
        {
            player.toggleInput = true;
            PerformMatch();
        });
    }

    void PerformMatch()
    {
        float originalFov = 60f;
        Sequence camFovSequence = DOTween.Sequence();
        camFovSequence.Append(DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, originalFov - 5f, 0.1f));
        camFovSequence.Append(DOTween.To(() => cam.fieldOfView, x => cam.fieldOfView = x, originalFov, 0.5f));

        foreach (GameObject receiver in projectionReceivers)
        {
            receiver.GetComponent<MeshRenderer>().sharedMaterial.SetFloat("_ShowProjection" + projectionLayer, 0f);
        }
        if (confirmCanvas != null)
        {
            confirmCanvas.DOFade(0f, 0.5f);
        }

        player.walkSpeed = originalWalkSpeed;
        onMatch.Invoke();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, 60, 0.1f, 10f, 1);
    }
}
