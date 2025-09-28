using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckpointScript : MonoBehaviour
{
    [Header("Assign the End Game Screen UI in Inspector")]
    public GameObject endGameScreen;
    AudioManager audioManager;

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
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            if (audioManager != null)
                audioManager.PlaySFX(audioManager.finish);

            // Disable player input
            playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }

            // Show end game screen
            if (endGameScreen != null)
            {
                endGameScreen.SetActive(true);
            }
        }
    }
}
