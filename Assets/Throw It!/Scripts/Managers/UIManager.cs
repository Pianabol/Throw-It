using UnityEngine;

public class UIManager : MonoBehaviour, IGameStateListener
{
    [Header(" Panels ")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject retryPanel;
    [SerializeField] private GameObject gameFinishedPanel; 

    public void GameStateChangedCallBack(EGameState gameState)
    {
        // Gelen state'e göre sadece ilgili paneli aktif et, diğerlerini kapat
        if(menuPanel) menuPanel.SetActive(gameState == EGameState.MENU);
        if(gamePanel) gamePanel.SetActive(gameState == EGameState.GAME);
        if(levelCompletePanel) levelCompletePanel.SetActive(gameState == EGameState.LEVELCOMPLETE);
        if(retryPanel) retryPanel.SetActive(gameState == EGameState.GAMEOVER);
        if(gameFinishedPanel) gameFinishedPanel.SetActive(gameState == EGameState.GAMEFINISHED);
    }
}