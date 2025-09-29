using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // <-- 1. Thêm thư viện này

public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public GameManager gameManager;
    
    // Tham chiếu đến CheatManager
    private CheatManager cheatManager;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Kiểm tra healthBar trước khi sử dụng
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthBar not assigned! Player health will not be displayed.");
        }

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
        
        // Tìm CheatManager
        if (cheatManager == null)
        {
            cheatManager = FindFirstObjectByType<CheatManager>();
        }
    }

    void Update()
    {
        // 2. Thay đổi cách kiểm tra input
        // Keyboard.current lấy trạng thái bàn phím hiện tại
        // .capsLockKey là phím Caps Lock
        // .wasPressedThisFrame chỉ trả về true tại frame đầu tiên phím được nhấn
        if (Keyboard.current != null && Keyboard.current.capsLockKey.wasPressedThisFrame)
        {
            TakeDamage(20);
        }
    }

     public void TakeDamage(int damage)
    {
        // Debug để kiểm tra
        Debug.Log($"TakeDamage called: damage={damage}, cheatManager={cheatManager != null}, isCheatMode={cheatManager?.IsCheatModeActive()}");
        
        // Kiểm tra cheat mode - nếu bật thì không mất máu
        if (cheatManager != null && cheatManager.IsCheatModeActive())
        {
            Debug.Log("Cheat mode bật - không mất máu!");
            return;
        }
        
        Debug.Log($"Mất máu: {damage} điểm");
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        this.enabled = false;

        if (gameManager != null)
        {
            gameManager.EndGame();
        }
        else
        {
            Debug.LogError("GameManager not found!");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Banh Mi"))
        {
            FindFirstObjectByType<CoinManager>().AddCoin();
            Destroy(other.gameObject);
        }
    }

}