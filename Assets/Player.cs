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

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
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