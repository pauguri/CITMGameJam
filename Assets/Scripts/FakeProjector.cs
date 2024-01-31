using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class FakeProjector : MonoBehaviour
{
    public bool canMatch = true;
    [Space]
    [SerializeField] private float focusWalkSpeed = 2f;
    [SerializeField] private float positionFocusTolerance = 0.5f;
    [SerializeField] private float positionMatchTolerance = 0.2f;
    [SerializeField] private float rotationTolerance = 20f;
    [Space]
    [SerializeField] private UnityEvent onMatch;
    [SerializeField] private CanvasGroup confirmCanvas;

    [Header("Sound")]
    [SerializeField] private AudioSource humAudioSource;
    [SerializeField] private AudioSource figmentMatchAudioSource;
    [SerializeField] private float humVolume = 1f;
    [SerializeField] private float humRange = 4f;

    private bool isMatched = false;
    private bool canConfirm = false;
    private bool fadedInConfirmCanvas = false;
    private bool focused = false;
    private float originalWalkSpeed;

    private PlayerController player;
    private Camera cam;
    private Tween confirmFadeIn;
    private Tween confirmFadeOut;

    void Start()
    {
        if (PlayerController.instance != null)
        {
            player = PlayerController.instance;
            originalWalkSpeed = player.walkSpeed;
            player.OnConfirmClick += TryConfirmFigment;
        }
    }

    void Update()
    {
        if (isMatched || !canMatch || player == null)
        {
            return;
        }

        cam = player.cameraComponent;
        if (cam == null)
        {
            return;
        }

        // vertical distance check
        if (Mathf.Abs(cam.transform.position.y - transform.position.y) > 2f)
        {
            return;
        }

        float distance = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
        float angle = Vector3.Angle(cam.transform.forward, transform.forward);

        if (humAudioSource.isPlaying)
        {
            float remappedDistance = distance.Remap(0f, humRange, 0.7f, 0f) * angle.Remap(0f, rotationTolerance, 1.4f, 1f);
            humAudioSource.volume = Mathf.Lerp(0f, humVolume, remappedDistance);
            humAudioSource.pitch = Mathf.Lerp(0.5f, 1f, remappedDistance);
        }

        bool positionFocus = distance < positionFocusTolerance;
        bool rotationFocus = angle < rotationTolerance;

        if (positionFocus && rotationFocus)
        {
            player.walkSpeed = focusWalkSpeed;
            focused = true;

            bool positionMatch = Vector3.Distance(new Vector3(player.transform.position.x, 0, player.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < positionMatchTolerance;

            if (positionMatch)
            {
                if (confirmCanvas != null && (confirmFadeIn == null || !confirmFadeIn.active) && confirmCanvas.alpha < 1f)
                {
                    Debug.Log("confirm fade in " + gameObject.name);
                    confirmFadeIn = confirmCanvas.DOFade(1f, 0.5f);
                    fadedInConfirmCanvas = true;
                }
                canConfirm = true;
                return;
            }
        }
        else if (originalWalkSpeed != 0f && focused)
        {
            focused = false;
            player.walkSpeed = originalWalkSpeed;
        }

        if (confirmCanvas != null && (confirmFadeOut == null || !confirmFadeOut.active) && confirmCanvas.alpha > 0f && fadedInConfirmCanvas)
        {
            Debug.Log("confirm fade out " + gameObject.name);
            fadedInConfirmCanvas = false;
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

        figmentMatchAudioSource.Play();

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
        camFovSequence.Play();

        if (confirmCanvas != null)
        {
            confirmCanvas.DOFade(0f, 0.5f);
        }

        humAudioSource.Stop();

        player.walkSpeed = originalWalkSpeed;
        onMatch.Invoke();
    }

    public void SetCanMatch(bool value)
    {
        canMatch = value;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawFrustum(Vector3.zero, 60, 0.1f, 10f, 1);
    }
}
