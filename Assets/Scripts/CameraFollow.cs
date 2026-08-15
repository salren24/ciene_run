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

    Rigidbody2D targetRb;
    PlayerController targetController;
    float fallLookAheadCurrent;
    float manualLookDownCurrent;

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
}
