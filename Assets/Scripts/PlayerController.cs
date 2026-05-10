using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    public event EventHandler OnDied;
    public event EventHandler OnRespawned;

    public enum PlayerState { Idle, Running, Jumping, Falling, WallSliding }

    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Wall Movement")]
    [SerializeField] private float wallJumpForceX = 10f;
    [SerializeField] private float wallJumpForceY = 14f;
    [SerializeField] private float wallSlideSpeed = -3f;

    [Header("Feel Mechanics")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.10f;
    [SerializeField] private float fastFallMultiplier = 2.5f;

    [Header("Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.05f;
    [SerializeField] private float wallCheckDistance = 0.05f;

    // ── Components ────────────────────────────────────────────────────────────

    private Rigidbody2D rb;
    private BoxCollider2D col;

    // ── Input ─────────────────────────────────────────────────────────────────

    private Vector2 moveInput;

    // ── State ─────────────────────────────────────────────────────────────────

    public PlayerState CurrentPlayerState { get; private set; }

    // ── Timers ────────────────────────────────────────────────────────────────

    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    // ── Wall ──────────────────────────────────────────────────────────────────

    private bool isTouchingWall;
    private bool isPressingIntoWall;
    private int wallDirection;        // 1 = right wall, -1 = left wall

    private float wallJumpLockTimer;
    private const float WallJumpLockDuration = 0.35f;


    // ── Gravity ───────────────────────────────────────────────────────────────

    private float originalGravityScale;

    // ── Public Read-Only (consumed by PlayerAnimator) ─────────────────────────

    public float XSpeed => Mathf.Abs(rb.linearVelocity.x);
    public float YVelocity => rb.linearVelocity.y;
    public bool IsGrounded { get; private set; }
    public bool IsWallSliding => CurrentPlayerState == PlayerState.WallSliding;
    public bool IsFacingRight { get; private set; } = true;
    public bool IsDead { get; private set; }

    // =========================================================================
    // Unity Lifecycle
    // =========================================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        originalGravityScale = rb.gravityScale;
    }

    private void Start()
    {
        GameInput.Instance.OnJumpStarted += GameInput_OnJumpStarted;
    }

    private void Update()
    {
        if (IsDead) return;

        moveInput = GameInput.Instance.MoveInput;

        CheckGrounded();
        CheckWall();
        UpdateCoyoteTime();
        UpdateJumpBuffer();
        HandleJump();
        HandleWallSlide();
        HandleFastFall();
        HandleFlip();

        if (wallJumpLockTimer > 0f)
            wallJumpLockTimer -= Time.deltaTime;

        UpdateState();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        HandleMovement();
    }

    private void OnDestroy()
    {
        if (GameInput.Instance == null) return;
        GameInput.Instance.OnJumpStarted -= GameInput_OnJumpStarted;
    }

    // =========================================================================
    // State Machine — describes what's happening for the animator.
    // =========================================================================

    private void UpdateState()
    {
        // Wall slide takes priority over all airborne states
        if (isTouchingWall && isPressingIntoWall && !IsGrounded && rb.linearVelocity.y <= 1f)
        {
            CurrentPlayerState = PlayerState.WallSliding;
        }
        else if (!IsGrounded && rb.linearVelocity.y > 0f)
        {
            CurrentPlayerState = PlayerState.Jumping;
        }
        else if (!IsGrounded && rb.linearVelocity.y <= 0f)
        {
            CurrentPlayerState = PlayerState.Falling;
        }
        else if (IsGrounded && Mathf.Abs(moveInput.x) > 0.01f)
        {
            CurrentPlayerState = PlayerState.Running;
        }
        else
        {
            CurrentPlayerState = PlayerState.Idle;
        }
    }

    // =========================================================================
    // Input Callbacks
    // =========================================================================

    private void GameInput_OnJumpStarted(object sender, EventArgs e)
    {
        if (IsDead) return;
        // Just register the buffer — HandleJump() in Update will consume it.
        jumpBufferCounter = jumpBufferTime;
    }

    // =========================================================================
    // Detection
    // =========================================================================

    private void CheckGrounded()
    {
        Bounds b = col.bounds;
        RaycastHit2D hit = Physics2D.BoxCast(
            b.center,
            new Vector2(b.size.x * 0.9f, b.size.y),
            0f,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
        IsGrounded = hit.collider != null;
    }

    private void CheckWall()
    {
        Bounds b = col.bounds;
        Vector2 castSize = new Vector2(b.size.x, b.size.y * 0.9f);

        RaycastHit2D rightHit = Physics2D.BoxCast(b.center, castSize, 0f, Vector2.right, wallCheckDistance, groundLayer);
        RaycastHit2D leftHit = Physics2D.BoxCast(b.center, castSize, 0f, Vector2.left, wallCheckDistance, groundLayer);

        if (rightHit.collider != null) { isTouchingWall = true; wallDirection = 1; }
        else if (leftHit.collider != null) { isTouchingWall = true; wallDirection = -1; }
        else { isTouchingWall = false; wallDirection = 0; }

        isPressingIntoWall = isTouchingWall
            && ((wallDirection == 1 && moveInput.x > 0.01f)
            || (wallDirection == -1 && moveInput.x < -0.01f));
    }

    // =========================================================================
    // Movement
    // =========================================================================

    private void HandleMovement()
    {
        if (wallJumpLockTimer > 0f) return;
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleFlip()
    {
        if (moveInput.x > 0.01f && !IsFacingRight) Flip();
        else if (moveInput.x < -0.01f && IsFacingRight) Flip();
    }

    private void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    // =========================================================================
    // Jump
    // =========================================================================

    private void UpdateCoyoteTime()
    {
        if (IsGrounded) coyoteTimeCounter = coyoteTime;
        else coyoteTimeCounter -= Time.deltaTime;
    }

    private void UpdateJumpBuffer()
    {
        if (jumpBufferCounter > 0f)
            jumpBufferCounter -= Time.deltaTime;
    }

    private void HandleJump()
    {
        if (jumpBufferCounter <= 0f) return;

        if (isTouchingWall && !IsGrounded)
        {
            rb.linearVelocity = new Vector2(-wallDirection * wallJumpForceX, wallJumpForceY);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            wallJumpLockTimer = WallJumpLockDuration;
            return;
        }

        // Regular jump (includes coyote time)
        if (coyoteTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
        }
    }

    // =========================================================================
    // Wall Slide
    // =========================================================================

    private void HandleWallSlide()
    {
        if (!isTouchingWall || !isPressingIntoWall || IsGrounded || rb.linearVelocity.y > 1f || wallJumpLockTimer > 0f) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
    }

    // =========================================================================
    // Fast Fall
    // =========================================================================

    private void HandleFastFall()
    {
        if (moveInput.y >= -0.5f || IsGrounded || rb.linearVelocity.y >= 0f) return;

        float extraGravity = Mathf.Abs(Physics2D.gravity.y)
                             * rb.gravityScale
                             * (fastFallMultiplier - 1f);

        rb.linearVelocity += Vector2.down * extraGravity * Time.deltaTime;
    }

    // =========================================================================
    // Death and Respawn
    // =========================================================================

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        OnDied?.Invoke(this, EventArgs.Empty);
    }

    public void Respawn(Vector3 position)
    {
        IsDead = false;

        transform.position = position;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = originalGravityScale;

        isTouchingWall = false;
        isPressingIntoWall = false;
        wallDirection = 0;
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
        wallJumpLockTimer = 0f;

        CurrentPlayerState = PlayerState.Idle;

        OnRespawned?.Invoke(this, EventArgs.Empty);
    }
}