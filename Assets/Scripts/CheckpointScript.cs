using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointScript : MonoBehaviour
{
    [Header("Assign the End Game Screen UI in Inspector")]
    public GameObject endGameScreen;
    AudioManager audioManager;
    GameManager gameManager;

    private bool triggered = false;
    private PlayerInput playerInput;

    private void Awake()
    {
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

        // Find GameManager
        gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager == null)
        {
            UnityEngine.Debug.LogWarning("No GameManager found in scene.");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            
            // Disable player input
            playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }

            // Call GameManager to handle win condition and pause everything
            if (gameManager != null)
            {
                gameManager.WinGame();
            }
            else
            {
                // Fallback: show end game screen if GameManager not found
                if (endGameScreen != null)
                {
                    endGameScreen.SetActive(true);
                }
            }
        }
    }
}
