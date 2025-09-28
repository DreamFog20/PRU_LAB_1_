using UnityEngine;

public class ObjDamage : MonoBehaviour
{
    public int damage;
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.3f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
