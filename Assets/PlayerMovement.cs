using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private int jumpCount;
    AudioManager audioManager;

    [Header("Components")]
    public Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int extraJumps = 1;

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.3f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    public CoinManager cm;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Ensure groundCheckPos is assigned to avoid null reference exceptions
        if (groundCheckPos == null)
		{
			Transform existingGroundCheck = transform.Find("GroundCheck");
			if (existingGroundCheck != null)
			{
				groundCheckPos = existingGroundCheck;
			}
			else
			{
				GameObject groundCheckObject = new GameObject("GroundCheck");
				groundCheckObject.transform.SetParent(transform);
				groundCheckObject.transform.localPosition = new Vector3(0f, -0.6f, 0f);
				groundCheckPos = groundCheckObject.transform;
			}
		}
    }

    void Update()
    {
        if (rb == null) return;

        // Apply horizontal movement
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        // Flip sprite based on movement direction
        if (horizontalMovement > 0)
            spriteRenderer.flipX = false;
        else if (horizontalMovement < 0)
            spriteRenderer.flipX = true;

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
            audioManager.PlaySFX(audioManager.jump);
        }

        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawCube(groundCheckPos.position, groundCheckSize);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Banh Mi"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
            audioManager.PlaySFX(audioManager.coin);
        }
    }
}