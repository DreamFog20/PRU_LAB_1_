using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class FInishScript : MonoBehaviour
{
    private bool canRestart = false;

    void Start()
    {
        // Optionally delay restart availability
        canRestart = true;
    }

    void Update()
    {
        if (!canRestart) return;

        // Check for any key press or left mouse click
        if (Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            RestartGame();
        }
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
