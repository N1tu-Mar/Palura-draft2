using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D body;

    [Header("Movement")]
    [SerializeField] float speed = 10f;

    [Header("Jumping")]
    [SerializeField] float minJumpForce = 1f;        // initial jump for tap
    [SerializeField] float maxJumpForce = 4f;       // max jump velocity
    [SerializeField] float maxJumpHoldTime = 0.25f;  // max time to hold jump
    [SerializeField] float jumpHoldAcceleration = 30f; // upward acceleration while holding

    [Header("Manual Gravity")]
    [SerializeField] float gravity = 30f;            // downward acceleration
    [SerializeField] float maxFallSpeed = -60f;      // terminal velocity

    [Header("Ground Check")]
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckDistance = 0.2f;

    [SerializeField] private Animator _animator;


    bool isGrounded;
    bool isJumping;
    float jumpTimer;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        body.gravityScale = 0f; // manual gravity

        _animator = GetComponent<Animator>();

    }

    private void Update()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Bounds bounds = box.bounds;
        Vector2 rayOrigin = new Vector2(bounds.center.x, bounds.min.y);

        // Cast ray down from bottom of collider
        isGrounded = Physics2D.Raycast(rayOrigin, Vector2.down, groundCheckDistance, groundLayer);

        // --- Horizontal movement ---
        float x = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector2(x * speed, body.linearVelocity.y);

        // --- Jump input detection ---
        bool jumpPressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool jumpJustPressed = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
        bool jumpReleased = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow);

        // --- Start jump ---
        if (jumpJustPressed && isGrounded)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, minJumpForce); // start jump
            isJumping = true;
            jumpTimer = 0f;
        }

        // --- Jump hold phase (gravity ignored while rising) ---
        if (isJumping)
        {
            jumpTimer += Time.deltaTime;

            if (jumpPressed && jumpTimer <= maxJumpHoldTime)
            {
                // Apply upward acceleration
                body.linearVelocity += Vector2.up * jumpHoldAcceleration * Time.deltaTime;

                // Clamp velocity to maxJumpForce
                if (body.linearVelocity.y > maxJumpForce)
                    body.linearVelocity = new Vector2(body.linearVelocity.x, maxJumpForce);
            }
            else
            {
                // Stop upward acceleration
                isJumping = false;
            }

            if (jumpReleased)
            {
                isJumping = false;
            }
        }

        // --- Manual gravity (applied only when not jumping upward) ---
        if (!isGrounded && !isJumping)
        {
            body.linearVelocity += Vector2.up * -gravity * Time.deltaTime;

            if (body.linearVelocity.y < maxFallSpeed)
                body.linearVelocity = new Vector2(body.linearVelocity.x, maxFallSpeed);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }


    private void HandleMovement()
    {
        // input will store a value between -1 and +1
        // GetAxisRaw() returns exactly -1, 0, or +1
        // GetAxis() returns a smooth value between -1 and +1
        // A/D, Left/Right Arrow, and joystick map to "Horizontal"

        float input = Input.GetAxis("Horizontal");
        body.linearVelocity = new Vector3(input * speed, body.linearVelocity.y);
        _animator.SetBool("isRunning", Mathf.Abs(input) > 0.1f);


        if (input != 0)
        {
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false); // ← FIXED
        }
    }



}