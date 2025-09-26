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
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
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
