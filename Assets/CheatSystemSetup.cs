using UnityEngine;

public class CheatSystemSetup : MonoBehaviour
{
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupCheatSystem();
        }
    }
    
    [ContextMenu("Setup Cheat System")]
    public void SetupCheatSystem()
    {
        Debug.Log("Đang thiết lập Cheat System...");
        
        // 1. Tạo CheatManager nếu chưa có
        CheatManager cheatManager = FindFirstObjectByType<CheatManager>();
        if (cheatManager == null)
        {
            GameObject cheatManagerObject = new GameObject("CheatManager");
            cheatManager = cheatManagerObject.AddComponent<CheatManager>();
            Debug.Log("✓ Đã tạo CheatManager");
        }
        else
        {
            Debug.Log("✓ CheatManager đã tồn tại");
        }
        
        // 2. Tạo CheatUI nếu chưa có
        CheatUI cheatUI = FindFirstObjectByType<CheatUI>();
        if (cheatUI == null)
        {
            GameObject cheatUIObject = new GameObject("CheatUI");
            cheatUI = cheatUIObject.AddComponent<CheatUI>();
            Debug.Log("✓ Đã tạo CheatUI");
        }
        else
        {
            Debug.Log("✓ CheatUI đã tồn tại");
        }
        
        // 3. Tạo CheatInstructions nếu chưa có
        CheatInstructions cheatInstructions = FindFirstObjectByType<CheatInstructions>();
        if (cheatInstructions == null)
        {
            GameObject cheatInstructionsObject = new GameObject("CheatInstructions");
            cheatInstructions = cheatInstructionsObject.AddComponent<CheatInstructions>();
            Debug.Log("✓ Đã tạo CheatInstructions");
        }
        else
        {
            Debug.Log("✓ CheatInstructions đã tồn tại");
        }
        
        Debug.Log("🎮 Cheat System đã được thiết lập hoàn tất!");
        Debug.Log("📋 Hướng dẫn sử dụng:");
        Debug.Log("   - Bấm TAB để bật/tắt cheat mode");
        Debug.Log("   - Khi cheat mode bật: không mất máu, đi xuyên qua chướng ngại vật");
        Debug.Log("   - Trạng thái cheat mode sẽ hiển thị ở góc trên bên trái màn hình");
    }
    
    // Hàm để test cheat system
    [ContextMenu("Test Cheat System")]
    public void TestCheatSystem()
    {
        CheatManager cheatManager = FindFirstObjectByType<CheatManager>();
        if (cheatManager != null)
        {
            Debug.Log("🧪 Đang test cheat system...");
            cheatManager.ToggleCheatMode();
            Debug.Log($"Cheat mode hiện tại: {(cheatManager.IsCheatModeActive() ? "BẬT" : "TẮT")}");
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy CheatManager! Hãy chạy Setup Cheat System trước.");
        }
    }
}
