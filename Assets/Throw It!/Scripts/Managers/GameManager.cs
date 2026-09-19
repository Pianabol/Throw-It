using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private EGameState gameState;
    
    private void Awake()
    {
        if (instance == null) { instance = this; } 
        else { Destroy(gameObject); }
    }
    
    private void Start()
    {
        SetGameState(EGameState.MENU);
    }

    public void SetGameState(EGameState newState)
    {
        // Trafik Polisi Müdahalesi: Eğer Level Bitti sinyali geldiyse, son level mi diye kontrol et
        if (newState == EGameState.LEVELCOMPLETE)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.IsGameFinished())
            {
                newState = EGameState.GAMEFINISHED; // Sinyali final ekranına çevir
            }
        }

        this.gameState = newState;

        // Sahnedeki tüm dinleyicileri (UIManager, LevelManager vb.) bul ve onlara durumu bildir
        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener dependency in gameStateListeners)
        {
            dependency.GameStateChangedCallBack(this.gameState);
        }
    }

    public void StartGame()
    {
        SetGameState(EGameState.GAME);
    }
    
    public void NextButtonCallBack()
    {
        SceneManager.LoadScene(0);
    }

    public void RetryButtonCallBack()
    {
        SceneManager.LoadScene(0);
    }
    
    public void RestartGameCallBack() 
    {
        SceneManager.LoadScene(0);
    }

    public bool IsGame() => gameState == EGameState.GAME;
}