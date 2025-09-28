using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Thêm thư viện Input System

public class GameManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject menuPanel; // Đổi tên từ gameOverPanel để dùng chung
    public GameObject gameOverMenuPrefab;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuPrefab; // Kéo prefab PauseMenu_Prefab vào đây
    AudioManager audioManager;
    // Biến trạng thái
    private bool isGameOver = false;
    private bool isPaused = false;

    // Tham chiếu đến menu đang được hiển thị
    private GameObject currentMenuInstance;

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
            Debug.LogWarning("No GameObject with 'Audio' tag found. AudioManager will be null.");
        }
    }

    // Hàm Update để lắng nghe input
    void Update()
    {
        // Chỉ cho phép pause khi game chưa kết thúc
        if (!isGameOver && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // --- PAUSE/RESUME LOGIC ---
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Dừng thời gian của game

        menuPanel.SetActive(true);
        currentMenuInstance = Instantiate(pauseMenuPrefab, menuPanel.transform);

        // Gán chức năng cho các nút trong Pause Menu
        Button[] buttons = currentMenuInstance.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Resume")) // Nút tiếp tục
            {
                btn.onClick.AddListener(ResumeGame);
            }
            else if (btn.name.Contains("Quit")) // Nút về menu chính
            {
                btn.onClick.AddListener(GoToMainMenu);
            }
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Cho thời gian chạy lại bình thường

        menuPanel.SetActive(false);
        if (currentMenuInstance != null)
        {
            Destroy(currentMenuInstance);
        }
    }

    // --- GAME OVER LOGIC ---
    public void EndGame()
    {
        if (isGameOver) return;
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.death);

        isGameOver = true;
        Time.timeScale = 0f;

        menuPanel.SetActive(true);
        currentMenuInstance = Instantiate(gameOverMenuPrefab, menuPanel.transform);

        Button[] buttons = currentMenuInstance.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Restart"))
            {
                btn.onClick.AddListener(RestartGame);
            }
            else if (btn.name.Contains("Quit"))
            {
                btn.onClick.AddListener(GoToMainMenu);
            }
        }
    }

<<<<<<< HEAD
    // --- WIN GAME LOGIC ---
    public void WinGame()
    {
        if (isGameOver) return;
        if (audioManager != null)
            audioManager.PlaySFX(audioManager.finish);

        isGameOver = true;
        Time.timeScale = 0f; // Pause everything

      

        menuPanel.SetActive(true);
        currentMenuInstance = Instantiate(gameOverMenuPrefab, menuPanel.transform);

        Button[] buttons = currentMenuInstance.GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            if (btn.name.Contains("Restart"))
            {
                btn.onClick.AddListener(RestartGame);
            }
            else if (btn.name.Contains("Quit"))
            {
                btn.onClick.AddListener(GoToMainMenu);
            }
        }
    }

=======
>>>>>>> parent of 898bf5d ( endgame thì pause hết mọi thứ)
    // --- BUTTON FUNCTIONS ---
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        // Luôn nhớ set lại timeScale trước khi chuyển scene
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}