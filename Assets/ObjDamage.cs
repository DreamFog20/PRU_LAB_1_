using UnityEngine;

public class ObjDamage : MonoBehaviour
{
    public int damage;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.3f;
    
    // Tham chiếu đến CheatManager
    private CheatManager cheatManager;
    
    void Start()
    {
        // Tìm CheatManager
        if (cheatManager == null)
        {
            cheatManager = FindFirstObjectByType<CheatManager>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Kiểm tra cheat mode - nếu bật thì không gây damage và knockback
            if (cheatManager != null && cheatManager.IsCheatModeActive())
            {
                Debug.Log("Cheat mode bật - chướng ngại vật không gây sát thương!");
                return;
            }
            
            var player = collision.gameObject.GetComponent<Player>();
            var playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            
            if (player != null)
            {
                player.TakeDamage(damage);
            }
            
            if (playerMovement != null)
            {
                // Calculate knockback direction (from object to player)
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                // Add some upward force for better visual effect
                knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 0.3f;
                knockbackDirection = knockbackDirection.normalized;
                
                playerMovement.ApplyKnockback(knockbackDirection, knockbackForce, knockbackDuration);
            }
        }
    }
}
