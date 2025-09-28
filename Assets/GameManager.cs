using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject menuPanel;
    public GameObject gameOverMenuPrefab;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuPrefab;

    [Header("Finish UI (in-scene)")]
    public GameObject finishScreenRoot;   // <-- kéo EndGameScene vào đây
    public StarRow finishStarRow;         // <-- kéo component StarRow (con Star) của FinishScreen vào

    [Header("Refs for conditions")]
    public Player player;                 // kéo Player (để đọc HP)
    public CoinManager coinManager;       // kéo CoinManager (để đọc coin)

    AudioManager audioManager;
    private bool isGameOver = false;
    private bool isPaused = false;
    private GameObject currentMenuInstance;

    private void Awake()
    {
        var audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null) audioManager = audioObject.GetComponent<AudioManager>();
        if (player == null) player = FindFirstObjectByType<Player>();
        if (coinManager == null) coinManager = FindFirstObjectByType<CoinManager>();

        if (finishScreenRoot != null) finishScreenRoot.SetActive(false); // đảm bảo ẩn lúc start
    }

    void Update()
    {
        if (!isGameOver && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused) ResumeGame(); else PauseGame();
        }
    }

    // ===== FINISH =====
    public void FinishGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        Time.timeScale = 0f;
        if (audioManager) audioManager.PauseAudio();

        if (finishScreenRoot) finishScreenRoot.SetActive(true);

        if (finishStarRow == null && finishScreenRoot != null)
            finishStarRow = finishScreenRoot.GetComponentInChildren<StarRow>(true);

        bool fullHP = player && player.currentHealth == player.maxHealth;
        bool allCoins = coinManager && coinManager.CollectedAllCoins();

        Debug.Log($"[FinishGame] fullHP={fullHP}, allCoins={allCoins}, starRow={(finishStarRow ? "OK" : "NULL")}");

        if (finishStarRow != null)
        {
            // Nếu muốn cưỡng bức 1 sao để test, tạm dùng dòng dưới:
            // finishStarRow.Apply(false, false, true);
            finishStarRow.Apply(fullHP, allCoins, true);
        }
        else
        {
            Debug.LogError("[FinishGame] StarRow is NULL. Gán EndGameScene/Star vào Finish Star Row.");
        }
    }


    // ===== PAUSE =====
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (audioManager) audioManager.PauseAudio();

        menuPanel.SetActive(true);
        currentMenuInstance = Instantiate(pauseMenuPrefab, menuPanel.transform);

        foreach (var btn in currentMenuInstance.GetComponentsInChildren<Button>())
        {
            if (btn.name.Contains("Resume")) btn.onClick.AddListener(ResumeGame);
            else if (btn.name.Contains("Quit")) btn.onClick.AddListener(GoToMainMenu);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (audioManager) audioManager.ResumeAudio();

        menuPanel.SetActive(false);
        if (currentMenuInstance) Destroy(currentMenuInstance);
    }

    // ===== GAME OVER =====
    public void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        if (audioManager) audioManager.PlaySFX(audioManager.death);
        Time.timeScale = 0f;

        menuPanel.SetActive(true);
        currentMenuInstance = Instantiate(gameOverMenuPrefab, menuPanel.transform);

        // Bật sao cho panel GameOver (nếu prefab có StarRow bên trong)
        var starRow = currentMenuInstance.GetComponentInChildren<StarRow>(true);
        bool fullHP = false; // đã chết -> không full HP
        bool allCoins = coinManager && coinManager.CollectedAllCoins();
        if (starRow) starRow.Apply(fullHP, allCoins, false);

        foreach (var btn in currentMenuInstance.GetComponentsInChildren<Button>())
        {
            if (btn.name.Contains("Restart")) btn.onClick.AddListener(RestartGame);
            else if (btn.name.Contains("Quit")) btn.onClick.AddListener(GoToMainMenu);
        }
    }

    // ===== BUTTON =====
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

}
