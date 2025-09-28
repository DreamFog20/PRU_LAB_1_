using UnityEngine;
using UnityEngine.UI;

public class CheatUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text cheatStatusText;
    public GameObject cheatIndicator;
    
    private CheatManager cheatManager;
    
    void Start()
    {
        // Tìm CheatManager
        cheatManager = FindFirstObjectByType<CheatManager>();
        
        // Nếu không tìm thấy CheatManager, tạo một GameObject mới với CheatManager
        if (cheatManager == null)
        {
            GameObject cheatManagerObject = new GameObject("CheatManager");
            cheatManager = cheatManagerObject.AddComponent<CheatManager>();
        }
        
        // Gán UI references cho CheatManager
        if (cheatManager != null)
        {
            cheatManager.cheatStatusText = cheatStatusText;
            cheatManager.cheatIndicator = cheatIndicator;
        }
        
        // Tạo UI text nếu chưa có
        if (cheatStatusText == null)
        {
            CreateCheatStatusText();
        }
    }
    
    void CreateCheatStatusText()
    {
        // Tạo Canvas nếu chưa có
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }
        
        // Tạo Text object
        GameObject textObject = new GameObject("CheatStatusText");
        textObject.transform.SetParent(canvas.transform);
        
        cheatStatusText = textObject.AddComponent<Text>();
        cheatStatusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        cheatStatusText.fontSize = 24;
        cheatStatusText.color = Color.white;
        cheatStatusText.text = "CHEAT MODE: TẮT";
        
        // Đặt vị trí ở góc trên bên trái
        RectTransform rectTransform = cheatStatusText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(150, -30);
        rectTransform.sizeDelta = new Vector2(300, 50);
        
        // Gán lại cho CheatManager
        if (cheatManager != null)
        {
            cheatManager.cheatStatusText = cheatStatusText;
        }
    }
    
    void Update()
    {
        // Cập nhật UI nếu cần
        if (cheatManager != null && cheatStatusText != null)
        {
            bool isCheatActive = cheatManager.IsCheatModeActive();
            cheatStatusText.text = isCheatActive ? "CHEAT MODE: BẬT" : "CHEAT MODE: TẮT";
            cheatStatusText.color = isCheatActive ? Color.red : Color.white;
        }
    }
}
