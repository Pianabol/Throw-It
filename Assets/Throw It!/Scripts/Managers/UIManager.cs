using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header(" Panels ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameFinishedPanel;

    [Header(" Level Popup Elements ")]
    [Tooltip("Arka planı (Image) olan Ana Popup Objesi")]
    [SerializeField] private GameObject levelPopupContainer;
    
    [Tooltip("Sadece yazıyı değiştirmek için Text referansı")]
    [SerializeField] private TextMeshProUGUI levelPopupText;

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (menuPanel != null) menuPanel.SetActive(gameState == EGameState.MENU);
        if (gamePanel != null) gamePanel.SetActive(gameState == EGameState.GAME);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(gameState == EGameState.LEVELCOMPLETE);
        if (gameOverPanel != null) gameOverPanel.SetActive(gameState == EGameState.GAMEOVER);
        if (gameFinishedPanel != null) gameFinishedPanel.SetActive(gameState == EGameState.GAMEFINISHED);

        // Oyun başladığında animasyonu tetikle
        if (gameState == EGameState.GAME)
        {
            ShowLevelPopup();
        }
    }

    private void ShowLevelPopup()
    {
        if (levelPopupContainer == null || levelPopupText == null) return;

        // Kaçıncı levelde olduğumuzu dinamik olarak yazdır
        if (LevelManager.Instance != null)
        {
            levelPopupText.text = "LEVEL " + LevelManager.Instance.CurrentLevelNum.ToString();
        }

        // Çakışmayı önlemek için eski animasyonları iptal et
        LeanTween.cancel(levelPopupContainer);
        
        // Popup'ı aç ve boyutunu görünmez yap
        levelPopupContainer.SetActive(true);
        levelPopupContainer.transform.localScale = Vector3.zero;

        // Jöle gibi ekranda belir (0.4 saniye sürer)
        LeanTween.scale(levelPopupContainer, Vector3.one, 0.4f)
            .setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() =>
            {
                // TAM 2 SANİYE bekle, sonra jöle gibi küçülüp kaybol
                LeanTween.scale(levelPopupContainer, Vector3.zero, 0.3f)
                    .setDelay(2.0f)
                    .setEase(LeanTweenType.easeInBack)
                    .setOnComplete(() => 
                    {
                        levelPopupContainer.SetActive(false);
                    });
            });
    }
}