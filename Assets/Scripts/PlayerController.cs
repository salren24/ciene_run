using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Carrera (estilo Mario)")]
    public float walkSpeed = 5.5f;
    public float runSpeed = 9f;
    public float acceleration = 40f;
    public float deceleration = 50f;
    public float airControl = 0.65f;

    [Header("Salto")]
    public float jumpForce = 15f;
    public float jumpCutMultiplier = 0.45f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.12f;
    public float fallGravityMultiplier = 1.7f;
    public float maxFallSpeed = 22f;

    [Header("Detección de suelo")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Reaparición tras morir")]
    public float respawnInvulnerabilityDuration = 1.6f;
    public float blinkInterval = 0.08f;

    [Header("Power-ups")]
    public float speedBoostMultiplier = 1.5f;
    public Color shieldColor = new Color(0.3f, 0.65f, 1f, 0.55f);

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;
    SpriteRenderer shieldVisual;

    float moveInput;
    bool runHeld;
    bool jumpPressed;
    bool jumpReleased;
    bool isGrounded;

    float touchMoveInput;
    bool touchRunHeld;
    bool touchLookDownHeld;

    float coyoteCounter;
    float jumpBufferCounter;
    float baseGravity;
    bool jumpConsumed;

    Vector3 lastGroundedPosition;
    float invulnerabilityTimer;
    float blinkTimer;

    bool airJumpAvailable;
    float doubleJumpTimer;
    float speedBoostTimer;
    bool hasShield;

    public bool IsGrounded => isGrounded;
    public float FacingSign => sr != null && sr.flipX ? -1f : 1f;
    public bool IsLookDownHeld { get; private set; }
    public Vector3 LastGroundedPosition => lastGroundedPosition;
    public bool IsInvulnerable => invulnerabilityTimer > 0f;
    public bool DoubleJumpActive => doubleJumpTimer > 0f;
    public bool SpeedBoostActive => speedBoostTimer > 0f;
    public bool HasShield => hasShield;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        baseGravity = rb.gravityScale;
        lastGroundedPosition = transform.position;

        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");

        BuildShieldVisual();
    }

    void BuildShieldVisual()
    {
        GameObject go = new GameObject("ShieldVisual");
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * 1.8f;
        shieldVisual = go.AddComponent<SpriteRenderer>();
        shieldVisual.sprite = PlaceholderSprite.Ring();
        shieldVisual.color = shieldColor;
        shieldVisual.sortingOrder = 11;
        shieldVisual.enabled = false;
    }

    void Update()
    {
        float keyboardMove = Input.GetAxisRaw("Horizontal");
        moveInput = Mathf.Approximately(keyboardMove, 0f) ? touchMoveInput : keyboardMove;

        bool keyboardRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetButton("Fire3");
        runHeld = keyboardRun || touchRunHeld;

        bool keyboardLookDown = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        IsLookDownHeld = keyboardLookDown || touchLookDownHeld;

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        if (Input.GetButtonUp("Jump"))
            jumpReleased = true;

        if (sr != null)
        {
            if (moveInput > 0.01f) sr.flipX = false;
            else if (moveInput < -0.01f) sr.flipX = true;
        }

        UpdateGrounded();
        UpdateTimers();
        TryBufferedJump();
        UpdateAnimator();
        UpdateInvulnerabilityBlink();
        UpdatePowerUpTimers();
    }

    void UpdatePowerUpTimers()
    {
        if (doubleJumpTimer > 0f)
            doubleJumpTimer -= Time.deltaTime;
        if (speedBoostTimer > 0f)
            speedBoostTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyJumpPhysics();
        ClampFallSpeed();
        jumpPressed = false;
        jumpReleased = false;
    }

    void UpdateGrounded()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        Collider2D groundHit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundHit != null;
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpConsumed = false;
            airJumpAvailable = true;

            // No guardar como "posicion segura" si el suelo es una plataforma movil:
            // podria haberse alejado para cuando el jugador muera y se use este punto para reaparecer.
            if (groundHit.GetComponent<ElevatorPlatform>() == null)
                lastGroundedPosition = transform.position;
        }
    }

    public bool HasStaticGroundBelow(Vector3 position, float maxDistance = 2f)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, maxDistance, groundLayer);
        return hit.collider != null && hit.collider.GetComponent<ElevatorPlatform>() == null;
    }

    void UpdateTimers()
    {
        if (!isGrounded)
            coyoteCounter -= Time.deltaTime;

        if (jumpPressed)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    void TryBufferedJump()
    {
        if (jumpBufferCounter > 0f && coyoteCounter > 0f && !jumpConsumed)
        {
            Jump();
            jumpBufferCounter = 0f;
            coyoteCounter = 0f;
            jumpConsumed = true;
            return;
        }

        if (jumpBufferCounter > 0f && !isGrounded && DoubleJumpActive && airJumpAvailable)
        {
            Jump();
            jumpBufferCounter = 0f;
            airJumpAvailable = false;
        }
    }

    void ApplyHorizontalMovement()
    {
        float speedMultiplier = SpeedBoostActive ? speedBoostMultiplier : 1f;
        float targetSpeed = moveInput * (runHeld ? runSpeed : walkSpeed) * speedMultiplier;
        float accel = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        if (!isGrounded)
            accel *= airControl;

        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    void ApplyJumpPhysics()
    {
        if (jumpReleased && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);

        if (rb.linearVelocity.y < 0f)
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        else
            rb.gravityScale = baseGravity;
    }

    void ClampFallSpeed()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void Bounce(float force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
    }

    public void RespawnAt(Vector3 point)
    {
        transform.position = point;
        rb.linearVelocity = Vector2.zero;
        lastGroundedPosition = point;
        invulnerabilityTimer = respawnInvulnerabilityDuration;
        blinkTimer = 0f;
        if (sr != null)
            sr.enabled = true;
    }

    void UpdateInvulnerabilityBlink()
    {
        if (invulnerabilityTimer <= 0f)
            return;

        invulnerabilityTimer -= Time.deltaTime;

        if (invulnerabilityTimer <= 0f)
        {
            invulnerabilityTimer = 0f;
            if (sr != null)
                sr.enabled = true;
            return;
        }

        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            blinkTimer = blinkInterval;
            if (sr != null)
                sr.enabled = !sr.enabled;
        }
    }

    void UpdateAnimator()
    {
        if (animator == null)
            return;

        bool moving = Mathf.Abs(rb.linearVelocity.x) > 0.15f || Mathf.Abs(moveInput) > 0.01f;
        animator.SetBool("IsMoving", moving && isGrounded);
    }

    public void SetMoveInput(float value)
    {
        touchMoveInput = value;
    }

    public void SetRunHeld(bool held)
    {
        touchRunHeld = held;
    }

    public void SetLookDown(bool held)
    {
        touchLookDownHeld = held;
    }

    public void ActivateDoubleJump(float duration)
    {
        doubleJumpTimer = Mathf.Max(doubleJumpTimer, duration);
    }

    public void ActivateSpeedBoost(float duration)
    {
        speedBoostTimer = Mathf.Max(speedBoostTimer, duration);
    }

    public void ActivateShield()
    {
        hasShield = true;
        if (shieldVisual != null)
            shieldVisual.enabled = true;
    }

    public void TakeHit()
    {
        if (IsInvulnerable)
            return;

        if (hasShield)
        {
            hasShield = false;
            if (shieldVisual != null)
                shieldVisual.enabled = false;
            invulnerabilityTimer = respawnInvulnerabilityDuration;
            blinkTimer = 0f;
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.LoseLife();
    }

    public void TryJump()
    {
        jumpPressed = true;
        UpdateGrounded();
        TryBufferedJump();
    }

    public void TryJumpRelease()
    {
        jumpReleased = true;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
