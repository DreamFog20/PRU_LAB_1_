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

    [Header("Knockback")]
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    
    // Tham chiếu đến CheatManager
    private CheatManager cheatManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Find AudioManager with null check
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            audioManager = audioObject.GetComponent<AudioManager>();
        }
        else
        {
            UnityEngine.Debug.LogWarning("No GameObject with 'Audio' tag found. AudioManager will be null.");
        }

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
		
		// Tìm CheatManager
		if (cheatManager == null)
		{
			cheatManager = FindFirstObjectByType<CheatManager>();
		}
    }

    void Update()
    {
        if (rb == null) return;

        // Handle knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
            }
        }

        // Apply horizontal movement only if not knocked back
        if (!isKnockedBack)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }

        // Flip sprite based on movement direction
        if (horizontalMovement > 0)
            spriteRenderer.flipX = false;
        else if (horizontalMovement < 0)
            spriteRenderer.flipX = true;

        // Update animator parameters
        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        
        // Kiểm tra nếu cheat mode tắt thì bật lại collision với tất cả chướng ngại vật
        if (cheatManager != null && !cheatManager.IsCheatModeActive())
        {
            ResetAllCollisions();
        }
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
            if (audioManager != null)
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

    public void ApplyKnockback(Vector2 direction, float force, float duration)
    {
        // Kiểm tra cheat mode - nếu bật thì không bị knockback
        if (cheatManager != null && cheatManager.IsCheatModeActive())
        {
            UnityEngine.Debug.Log("Cheat mode bật - không bị knockback!");
            return;
        }
        
        isKnockedBack = true;
        knockbackTimer = duration;
        
        // Apply knockback force
        rb.linearVelocity = new Vector2(direction.x * force, direction.y * force);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Banh Mi"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.coin);
        }
    }
    
    // Xử lý collision với chướng ngại vật
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu đang trong cheat mode và va chạm với chướng ngại vật
        if (cheatManager != null && cheatManager.IsCheatModeActive())
        {
            // Kiểm tra nếu đối tượng có component ObjDamage (chướng ngại vật gây sát thương)
            if (collision.gameObject.GetComponent<ObjDamage>() != null)
            {
                // Đi xuyên qua chướng ngại vật bằng cách tắt collision
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider, true);
                UnityEngine.Debug.Log("Cheat mode: Đi xuyên qua chướng ngại vật!");
            }
        }
    }
    
    // Bật lại collision với tất cả chướng ngại vật
    void ResetAllCollisions()
    {
        // Tìm tất cả chướng ngại vật có component ObjDamage
        ObjDamage[] obstacles = FindObjectsOfType<ObjDamage>();
        Collider2D playerCollider = GetComponent<Collider2D>();
        
        foreach (ObjDamage obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                // Bật lại collision
                Physics2D.IgnoreCollision(playerCollider, obstacleCollider, false);
            }
        }
    }
    
    // Method để CheatManager gọi để tắt/bật collision với tất cả chướng ngại vật
    public void SetIgnoreAllObstacles(bool ignore)
    {
        // Tìm tất cả chướng ngại vật có component ObjDamage
        ObjDamage[] obstacles = FindObjectsOfType<ObjDamage>();
        Collider2D playerCollider = GetComponent<Collider2D>();
        
        foreach (ObjDamage obstacle in obstacles)
        {
            Collider2D obstacleCollider = obstacle.GetComponent<Collider2D>();
            if (obstacleCollider != null)
            {
                // Tắt hoặc bật collision
                Physics2D.IgnoreCollision(playerCollider, obstacleCollider, ignore);
            }
        }
        
        if (ignore)
        {
            UnityEngine.Debug.Log("Cheat mode: Tắt collision với tất cả chướng ngại vật!");
        }
        else
        {
            UnityEngine.Debug.Log("Cheat mode: Bật lại collision với tất cả chướng ngại vật!");
        }
    }
}