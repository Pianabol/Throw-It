/*
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

        LeanTween.reset();
    }

    // --- 1. SİHİRLİ DOKUNUŞ: GoalManager'ı Dinleme ---
    private void OnEnable()
    {
        GoalManager.OnAllGoalsCompleted += HandleLevelComplete;
    }

    private void OnDisable()
    {
        GoalManager.OnAllGoalsCompleted -= HandleLevelComplete;
    }

    private void HandleLevelComplete()
    {
        // Hedefler sıfırlandığı an bu metod tetiklenir ve UI'a LevelComplete ekranını basar!
        SetGameState(EGameState.LEVELCOMPLETE);
    }
    // -------------------------------------------------

    private void Start()
    {
        SetGameState(EGameState.MENU);
    }

    public void SetGameState(EGameState newState)
    {
        if (newState == EGameState.LEVELCOMPLETE)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.IsGameFinished())
            {
                newState = EGameState.GAMEFINISHED;
            }
        }

        this.gameState = newState;

        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener listener in gameStateListeners)
        {
            listener.GameStateChangedCallBack(this.gameState);
        }
    }

    // --- 2. SİHİRLİ DOKUNUŞ: Butonlarda Fader Kullanımı ---

    public void StartGame()
    {
        if (GameStartFader.Instance != null)
        {
            // Menüden oyuna geçerken sahne yenilenmez. Bu yüzden: Karart -> Oyuna Geç -> Aydınlat
            GameStartFader.Instance.FadeOut(() => 
            {
                SetGameState(EGameState.GAME);
                GameStartFader.Instance.FadeIn();
            });
        }
        else
        {
            SetGameState(EGameState.GAME);
        }
    }

    public void NextButtonCallBack()
    {
        ReloadSceneWithFade();
    }

    public void RetryButtonCallBack()
    {
        ReloadSceneWithFade();
    }

    public void HomeButtonCallBack()
    {
        ReloadSceneWithFade();
    }

    private void ReloadSceneWithFade()
    {
        if (GameStartFader.Instance != null)
        {
            // Ekranı karart, tamamen siyah olunca sahneyi baştan yükle (Açılışta otomatik FadeIn yapacak)
            GameStartFader.Instance.FadeOut(() => 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public bool IsGame() => gameState == EGameState.GAME;
}
*/

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

        LeanTween.reset();
    }

    // --- HEDEFLERİ DİNLEME KABLOLARI (Burası Sağlam) ---
    private void OnEnable()
    {
        GoalManager.OnAllGoalsCompleted += HandleLevelComplete;
    }

    private void OnDisable()
    {
        GoalManager.OnAllGoalsCompleted -= HandleLevelComplete;
    }

    private void HandleLevelComplete()
    {
        // Hedefler bittiği an UI'a LevelComplete ekranını basar!
        SetGameState(EGameState.LEVELCOMPLETE);
    }
    // -------------------------------------------------

    private void Start()
    {
        SetGameState(EGameState.MENU);
    }

    public void SetGameState(EGameState newState)
    {
        if (newState == EGameState.LEVELCOMPLETE)
        {
            if (LevelManager.Instance != null && LevelManager.Instance.IsGameFinished())
            {
                newState = EGameState.GAMEFINISHED;
            }
        }

        this.gameState = newState;

        IEnumerable<IGameStateListener> gameStateListeners = 
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IGameStateListener>();

        foreach (IGameStateListener listener in gameStateListeners)
        {
            listener.GameStateChangedCallBack(this.gameState);
        }
    }

    // --- BUTON METOTLARI (Sahne Yükleme Çöpe Atıldı, Saf Geçiş Geldi) ---

    public void StartGame()
    {
        TransitionToState(EGameState.GAME);
    }

    public void NextButtonCallBack()
    {
        // LevelManager hafızadaki leveli +1 artırdı bile. 
        // Sadece State'i GAME yapıyoruz, eski masayı yıkıp yenisini dizecek!
        TransitionToState(EGameState.GAME);
    }

    public void RetryButtonCallBack()
    {
        // Level artmadığı için aynı masayı temizleyip baştan dizecek!
        TransitionToState(EGameState.GAME);
    }

    public void HomeButtonCallBack()
    {
        TransitionToState(EGameState.MENU);
    }

    // --- SİHİRLİ GEÇİŞ METODU ---
    private void TransitionToState(EGameState targetState)
    {
        if (GameStartFader.Instance != null)
        {
            // 1. Ekranı yumuşakça karart
            GameStartFader.Instance.FadeOut(() => 
            {
                // 2. Ekran SİMSİYAH olduğunda arkaplanda State'i değiştir (LevelManager eski masayı silip yenisini dizer)
                SetGameState(targetState);
                
                // 3. Her şey dizilip hazır olduğuna göre ekranı aydınlat
                GameStartFader.Instance.FadeIn();
            });
        }
        else
        {
            // Güvenlik (Sahnede Fader yoksa şak diye geç)
            SetGameState(targetState);
        }
    }

    public bool IsGame() => gameState == EGameState.GAME;
}