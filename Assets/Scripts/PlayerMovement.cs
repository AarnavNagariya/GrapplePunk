    using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed;

    public float horizontalSpeed = 0;
    private float verticalSpeed = 0;

    [SerializeField] private InputActionReference moveControl;

    [SerializeField] private InputActionReference jumpControl;

    [SerializeField] private float accelerationTime;

    [SerializeField] private float decelerationTime;

    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float jumpReleaseForce;
    [SerializeField] private float jumpTime;
    [SerializeField] private float epsilon;
    [SerializeField] private float fallMultiplier;

    [SerializeField] private float maxFallSpeed;

    public bool accelerating = false;

    public bool decelerating = false;
    public bool grounded;
    [SerializeField] private Collider2D groundCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 0.2f;

    [SerializeField] private float jumpBufferTime = 0.2f;

    [SerializeField] private float airMoveForce;
    [SerializeField] private float airDrag;
    private float jumpBufferCounter;
    private float coyoteTimeCounter;
    private bool jumpHeld = false;

    private bool performJump = false;
    private bool jumpButtonHeld = false;


    private Rigidbody2D rb;

    public PlayerGrappling grapplingScript;

    private void OnEnable()
    {
        moveControl.action.Enable();
        jumpControl.action.Enable();
        jumpControl.action.performed += (_) => DoJump();
        jumpControl.action.canceled += (_) => CancelJump();
    }

    private void DoJump()
    {
        performJump = true;
        jumpButtonHeld = true;
    }

    private void CancelJump()
    {
        jumpButtonHeld = false;
        jumpHeld = false;
    }

    private void OnDisable()
    {
        moveControl.action.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grapplingScript = GetComponent<PlayerGrappling>();
    }

    int sign(float value)
    {
        return value >= 0 ? 1 : -1;
    }

    private void Accelerate(float moveInput, float dt)
    {
        horizontalSpeed += moveInput * (maxSpeed / accelerationTime) * dt;

        if (Mathf.Abs(horizontalSpeed) > maxSpeed)
            horizontalSpeed = sign(horizontalSpeed) * maxSpeed;
        accelerating = true;
        decelerating = false;
    }

    private void Decelerate(float dt)
    {
        int oldSign = sign(horizontalSpeed);
        horizontalSpeed -= sign(horizontalSpeed) * (maxSpeed / decelerationTime) * dt;
        if (sign(horizontalSpeed) != oldSign)
            horizontalSpeed = 0;
        accelerating = false;
        decelerating = true;
    }

    private void Idle()
    {
        horizontalSpeed = 0;
        accelerating = false;
        decelerating = false;
    }

    void FixedUpdate()
    {
        #region Horizontal Update

        #region Read Input

        float moveInput = moveControl.action.ReadValue<float>();

        #endregion

        if (Mathf.Abs(moveInput) < epsilon)
        {
            #region No Input

            if (Mathf.Abs(horizontalSpeed) < epsilon)
                Idle();
            else
                Decelerate(Time.fixedDeltaTime);

            #endregion
        }

        else if (sign(moveInput) == sign(horizontalSpeed) || Mathf.Abs(horizontalSpeed) < epsilon)

            #region Move Towards Current Velocity

            Accelerate(moveInput, Time.fixedDeltaTime);

        #endregion

        else

            #region Move Against Current Velocity

            Decelerate(Time.fixedDeltaTime);

        #endregion

        #endregion

        #region Vertical Update

        #region Grounded Check

        grounded = groundCollider.IsTouchingLayers(groundLayer);

        #endregion

        #region Coyote Time

        if (grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.fixedDeltaTime;

        #endregion

        #region Jump Buffering

        if (performJump)
        {
            jumpBufferCounter = jumpBufferTime;
            performJump = false;
        }
        else
            jumpBufferCounter -= Time.fixedDeltaTime;

        #endregion

        #region Velocity Update

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            #region Jumping

            verticalSpeed = 2 * maxJumpHeight / jumpTime;
            if (jumpButtonHeld)
                jumpHeld = true;

            coyoteTimeCounter = 0;
            jumpBufferCounter = 0;

            #endregion
        }
        else
        {
            #region Falling

            float acceleration = 2 * maxJumpHeight / (jumpTime * jumpTime);
            acceleration *= jumpHeld ? 1 : jumpReleaseForce;
            acceleration *= rb.velocity.y < 0 ? fallMultiplier : 1;
            verticalSpeed = Mathf.Max(rb.velocity.y - (Time.fixedDeltaTime * acceleration), maxFallSpeed);

            #endregion
        }

        #endregion

        #endregion

        if (!grounded)
        {
            rb.drag = airDrag;
            rb.AddForce(moveInput * airMoveForce * Vector2.right);
            rb.velocity = new Vector2(rb.velocity.x, verticalSpeed);
        }
        else
        {
            rb.drag = 0;
            rb.velocity = new Vector2(horizontalSpeed, verticalSpeed);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("LvlChanger"))
        {
            SceneManager.LoadScene("Thanks");
        }
    }
    
}