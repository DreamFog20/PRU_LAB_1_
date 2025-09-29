using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CheatManager : MonoBehaviour
{
    [Header("Cheat Settings")]
    public bool isCheatMode = false;
    public KeyCode cheatToggleKey = KeyCode.Tab;
    
    [Header("UI References")]
    public Text cheatStatusText; // Text hiển thị trạng thái cheat
    public GameObject cheatIndicator; // GameObject hiển thị khi cheat mode bật
    
    [Header("Player References")]
    public Player player;
    public PlayerMovement playerMovement;
    
    // Singleton pattern để dễ truy cập từ các script khác
    public static CheatManager Instance { get; private set; }
    
    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Tìm Player và PlayerMovement nếu chưa được gán
        if (player == null)
            player = FindFirstObjectByType<Player>();
        if (playerMovement == null)
            playerMovement = FindFirstObjectByType<PlayerMovement>();
            
        UpdateCheatUI();
    }
    
    void Update()
    {
        // Kiểm tra phím Tab để bật/tắt cheat mode
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleCheatMode();
        }
        
        // Tìm PlayerMovement nếu chưa có (cho trường hợp spawn sau)
        if (playerMovement == null)
        {
            playerMovement = FindFirstObjectByType<PlayerMovement>();
        }
    }
    
    public void ToggleCheatMode()
    {
        isCheatMode = !isCheatMode;
        UpdateCheatUI();
        
        // Log để debug
        Debug.Log($"Cheat Mode: {(isCheatMode ? "BẬT" : "TẮT")}");
        
        // Nếu bật cheat mode, tắt collision với tất cả chướng ngại vật ngay lập tức
        if (isCheatMode && playerMovement != null)
        {
            playerMovement.SetIgnoreAllObstacles(true);
        }
        // Nếu tắt cheat mode, bật lại collision
        else if (!isCheatMode && playerMovement != null)
        {
            playerMovement.SetIgnoreAllObstacles(false);
        }
        
        // Có thể thêm âm thanh hoặc hiệu ứng khác ở đây
    }
    
    void UpdateCheatUI()
    {
        if (cheatStatusText != null)
        {
            cheatStatusText.text = isCheatMode ? "CHEAT MODE: BẬT" : "CHEAT MODE: TẮT";
            cheatStatusText.color = isCheatMode ? Color.red : Color.white;
        }
        
        if (cheatIndicator != null)
        {
            cheatIndicator.SetActive(isCheatMode);
        }
    }
    
    // Hàm để các script khác kiểm tra trạng thái cheat
    public bool IsCheatModeActive()
    {
        return isCheatMode;
    }
    
    // Hàm để bật cheat mode từ code khác
    public void SetCheatMode(bool active)
    {
        isCheatMode = active;
        UpdateCheatUI();
    }
}
