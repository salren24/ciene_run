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

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator animator;

    float moveInput;
    bool runHeld;
    bool jumpPressed;
    bool jumpReleased;
    bool isGrounded;

    float touchMoveInput;
    bool touchRunHeld;

    float coyoteCounter;
    float jumpBufferCounter;
    float baseGravity;
    bool jumpConsumed;

    public bool IsGrounded => isGrounded;
    public float FacingSign => sr != null && sr.flipX ? -1f : 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        baseGravity = rb.gravityScale;

        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        float keyboardMove = Input.GetAxisRaw("Horizontal");
        moveInput = Mathf.Approximately(keyboardMove, 0f) ? touchMoveInput : keyboardMove;

        bool keyboardRun = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetButton("Fire3");
        runHeld = keyboardRun || touchRunHeld;

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

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpConsumed = false;
        }
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
        }
    }

    void ApplyHorizontalMovement()
    {
        float targetSpeed = moveInput * (runHeld ? runSpeed : walkSpeed);
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
