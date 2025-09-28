using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public int coinCount;
    public int totalCoins;
    public TMP_Text coinText;

    void Start()
    {
        // Tìm tất cả object có tag "Banh Mi"
        totalCoins = GameObject.FindGameObjectsWithTag("Banh Mi").Length;
        UpdateUI();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        coinText.text = coinCount + " / " + totalCoins;
    }

    public bool CollectedAllCoins()
    {
        return coinCount >= totalCoins;
    }
}
