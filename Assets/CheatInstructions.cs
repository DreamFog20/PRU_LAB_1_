using UnityEngine;
using UnityEngine.UI;

public class CheatInstructions : MonoBehaviour
{
    [Header("Instructions UI")]
    public Text instructionsText;
    
    void Start()
    {
        // Tạo text hướng dẫn nếu chưa có
        if (instructionsText == null)
        {
            CreateInstructionsText();
        }
        
        // Ẩn hướng dẫn ngay lập tức
        HideInstructions();
    }
    
    void CreateInstructionsText()
    {
        // Tìm Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }
        
        // Tạo Text object cho hướng dẫn
        GameObject textObject = new GameObject("CheatInstructions");
        textObject.transform.SetParent(canvas.transform);
        
        instructionsText = textObject.AddComponent<Text>();
        instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        instructionsText.fontSize = 18;
        instructionsText.color = Color.yellow;
        instructionsText.text = "HƯỚNG DẪN CHEAT:\nBấm TAB để bật/tắt cheat mode\n- Không mất máu";
        
        // Đặt vị trí ở giữa màn hình
        RectTransform rectTransform = instructionsText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0, 100);
        rectTransform.sizeDelta = new Vector2(400, 100);
        
        // Căn giữa text
        instructionsText.alignment = TextAnchor.MiddleCenter;
    }
    
    void HideInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(false);
        }
    }
    
    // Có thể gọi từ bên ngoài để hiển thị lại hướng dẫn
    public void ShowInstructions()
    {
        if (instructionsText != null)
        {
            instructionsText.gameObject.SetActive(true);
            Invoke("HideInstructions", 5f);
        }
    }
}
