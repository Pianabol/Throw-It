using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private EGameState gameState;

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
        } 
        else 
        { 
            Destroy(gameObject); 
            return;
        }

        // Önceki oturumdan kalan LeanTween çöplerini sıfırlar
        LeanTween.reset();
    }

    private void Start()
    {
        SetGameState(EGameState.MENU);
    }

    public void SetGameState(EGameState newState)
    {
        // Seviye bittiğinde son seviye kontrolü
        if (newState == EGameState.LEVELCOMPLETE)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.IsGameFinished())
            {
                newState = EGameState.GAMEFINISHED;
            }
        }

        this.gameState = newState;

        // Sahnedeki tüm IGameStateListener arayüzünü dinleyenlere haber ver
        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener listener in gameStateListeners)
        {
            listener.GameStateChangedCallBack(this.gameState);
        }
    }

    // --- BUTON METOTLARI (Inspector'dan OnClick'e Bağlanacak) ---

    public void StartGame()
    {
        SetGameState(EGameState.GAME);
    }

    public void NextButtonCallBack()
    {
        // Sahneyi yeniden yükle (LevelManager kaydedilen yeni seviyeyi açacaktır)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RetryButtonCallBack()
    {
        // Aynı sahneyi yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void HomeButtonCallBack()
    {
        // Ana menüye dönerken sahneyi sıfırdan açmak en temizidir
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsGame() => gameState == EGameState.GAME;
}