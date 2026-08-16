using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Seguimiento")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);
    public float smoothSpeed = 8f;
    public bool findPlayerOnStart = true;

    [Header("Límites (opcional)")]
    public bool useBounds;
    public float minX = -5f;
    public float maxX = 40f;
    public float minY = -2f;
    public float maxY = 12f;

    [Header("Vista al caer (para ver donde se aterriza)")]
    public float maxFallLookAhead = 3.5f;
    public float fallLookAheadPerVelocity = 0.15f;
    public float fallLookAheadSmoothSpeed = 4f;

    [Header("Mirar hacia abajo manualmente (mantener presionado)")]
    public float maxManualLookDown = 5f;
    public float manualLookDownSmoothSpeed = 6f;

    [Header("Encuadre segun aspect ratio (celular vs. editor)")]
    public float targetViewWidth = 24f;
    public float minOrthoSize = 5f;
    public float maxOrthoSize = 9f;

    Camera cam;
    Rigidbody2D targetRb;
    PlayerController targetController;
    float fallLookAheadCurrent;
    float manualLookDownCurrent;
    int lastScreenWidth;
    int lastScreenHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyAspectFraming();
    }

    void Start()
    {
        if (findPlayerOnStart && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerController controller = FindAnyObjectByType<PlayerController>();
                if (controller != null)
                    player = controller.gameObject;
            }

            if (player != null)
                target = player.transform;
        }

        if (target != null)
        {
            targetRb = target.GetComponent<Rigidbody2D>();
            targetController = target.GetComponent<PlayerController>();
        }
    }

    void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            ApplyAspectFraming();

        if (target == null)
            return;

        float fallVelocity = targetRb != null ? targetRb.linearVelocity.y : 0f;
        float fallLookAheadTarget = fallVelocity < 0f
            ? Mathf.Clamp(-fallVelocity * fallLookAheadPerVelocity, 0f, maxFallLookAhead)
            : 0f;
        fallLookAheadCurrent = Mathf.Lerp(fallLookAheadCurrent, fallLookAheadTarget, fallLookAheadSmoothSpeed * Time.deltaTime);

        bool lookDownHeld = targetController != null && targetController.IsLookDownHeld;
        float manualLookDownTarget = lookDownHeld ? maxManualLookDown : 0f;
        manualLookDownCurrent = Mathf.Lerp(manualLookDownCurrent, manualLookDownTarget, manualLookDownSmoothSpeed * Time.deltaTime);

        Vector3 desired = target.position + offset;
        desired.y -= Mathf.Max(fallLookAheadCurrent, manualLookDownCurrent);
        desired.z = offset.z;

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }

    void ApplyAspectFraming()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        if (cam == null)
            cam = GetComponent<Camera>();
        if (cam == null || lastScreenHeight <= 0)
            return;

        float aspect = (float)lastScreenWidth / lastScreenHeight;
        float desiredOrthoSize = targetViewWidth / aspect / 2f;
        cam.orthographicSize = Mathf.Clamp(desiredOrthoSize, minOrthoSize, maxOrthoSize);
    }
}
