using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Seguimiento")]
    public Vector3 offset = new Vector3(0f, 3.1f, -10f);
    public float smoothSpeed = 8f;
    public bool findPlayerOnStart = true;

    [Header("Encuadre vertical (estilo Super Mario World: el alto define la escala de camara, " +
        "el ancho queda libre segun el aspect del dispositivo)")]
    public float targetViewHeight = 15f;

    [Header("Camara anclada al suelo (no persigue el salto, solo la ultima altura pisada)")]
    public float verticalChaseMargin = 3f;

    [Header("Ventana muerta horizontal (el jugador se mueve libre adentro sin mover la camara)")]
    public float deadZoneWidth = 3f;

    [Header("Adelanto en la direccion de marcha")]
    public float lookAheadDistance = 2.5f;
    public float lookAheadSmooth = 3f;

    [Header("Limites de nivel (X se autocalcula desde LevelMapBuilder.startX/endX)")]
    public bool useBounds = true;
    public float minX = -5f;
    public float maxX = 40f;
    public float minY = -10f;
    public float maxY = 20f;

    [Header("Vista al caer (para ver donde se aterriza)")]
    public float maxFallLookAhead = 3.5f;
    public float fallLookAheadPerVelocity = 0.15f;
    public float fallLookAheadSmoothSpeed = 4f;

    [Header("Mirar hacia abajo manualmente (mantener presionado)")]
    public float maxManualLookDown = 5f;
    public float manualLookDownSmoothSpeed = 6f;

    Camera cam;
    Rigidbody2D targetRb;
    PlayerController targetController;
    float fallLookAheadCurrent;
    float manualLookDownCurrent;
    float lookAheadCurrent;
    float lastGroundedY;
    int lastScreenWidth;
    int lastScreenHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
        ApplyFraming();
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
            lastGroundedY = target.position.y;
        }

        RecalculateBoundsFromLevel();
    }

    void LateUpdate()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
            ApplyFraming();

        if (target == null)
            return;

        // La camara solo actualiza su referencia de altura cuando el jugador esta parado en
        // el suelo - saltar o caer NO mueve esta referencia, por eso la camara no persigue el salto.
        if (targetController != null && targetController.IsGrounded)
            lastGroundedY = target.position.y;

        float fallVelocity = targetRb != null ? targetRb.linearVelocity.y : 0f;
        float fallLookAheadTarget = fallVelocity < 0f
            ? Mathf.Clamp(-fallVelocity * fallLookAheadPerVelocity, 0f, maxFallLookAhead)
            : 0f;
        fallLookAheadCurrent = Mathf.Lerp(fallLookAheadCurrent, fallLookAheadTarget, fallLookAheadSmoothSpeed * Time.deltaTime);

        bool lookDownHeld = targetController != null && targetController.IsLookDownHeld;
        float manualLookDownTarget = lookDownHeld ? maxManualLookDown : 0f;
        manualLookDownCurrent = Mathf.Lerp(manualLookDownCurrent, manualLookDownTarget, manualLookDownSmoothSpeed * Time.deltaTime);

        float desiredY = lastGroundedY + offset.y - Mathf.Max(fallLookAheadCurrent, manualLookDownCurrent);

        // Adelanto horizontal en la direccion de marcha; al frenar (velocidad ~0) vuelve al centro.
        float velocityX = targetRb != null ? targetRb.linearVelocity.x : 0f;
        float lookAheadTarget = Mathf.Abs(velocityX) > 0.1f ? Mathf.Sign(velocityX) * lookAheadDistance : 0f;
        lookAheadCurrent = Mathf.Lerp(lookAheadCurrent, lookAheadTarget, lookAheadSmooth * Time.deltaTime);

        // Ventana muerta horizontal: el objetivo (jugador + adelanto) solo empuja la camara
        // cuando se sale de la franja central; adentro de la franja la camara no se mueve en X.
        float halfDeadZone = deadZoneWidth * 0.5f;
        float refX = target.position.x + lookAheadCurrent;
        float camX = transform.position.x;
        float desiredX;
        if (refX > camX + halfDeadZone)
            desiredX = refX - halfDeadZone;
        else if (refX < camX - halfDeadZone)
            desiredX = refX + halfDeadZone;
        else
            desiredX = camX;

        Vector3 desired = new Vector3(desiredX, desiredY, offset.z);
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Si el jugador se sale del viewport (menos el margen) por arriba o por abajo -saltos muy
        // altos o caidas largas- la camara lo persigue directo en vez de quedarse anclada al suelo.
        float halfHeight = cam != null ? cam.orthographicSize : targetViewHeight / 2f;
        float minCamY = target.position.y - halfHeight + verticalChaseMargin;
        float maxCamY = target.position.y + halfHeight - verticalChaseMargin;
        smoothed.y = Mathf.Clamp(smoothed.y, minCamY, maxCamY);

        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
            smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);
        }

        transform.position = smoothed;
    }

    void ApplyFraming()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        if (cam == null)
            cam = GetComponent<Camera>();
        if (cam == null)
            return;

        cam.orthographicSize = targetViewHeight / 2f;

        RecalculateBoundsFromLevel();
    }

    // minX/maxX se calculan desde el nivel para que la camara nunca muestre vacio fuera de el;
    // minY/maxY quedan a mano (LevelMapBuilder no tiene un rango vertical equivalente a startX/endX).
    void RecalculateBoundsFromLevel()
    {
        if (cam == null)
            return;

        LevelMapBuilder level = FindAnyObjectByType<LevelMapBuilder>();
        if (level == null)
            return;

        useBounds = true;
        float halfViewportWidth = cam.orthographicSize * cam.aspect;
        minX = level.startX + halfViewportWidth;
        maxX = level.endX - halfViewportWidth;
    }
}
