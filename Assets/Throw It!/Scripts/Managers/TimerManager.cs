using TMPro;
using System;
using UnityEngine;

public class TimerManager : MonoBehaviour, IGameStateListener
{
    public static TimerManager Instance { get; private set; }

    [Header(" Timer Settings ")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    private int currentTimer;
    private bool isTimerRunning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (gameState == EGameState.GAME)
        {
            // Level prefabı sahneye doğduktan hemen sonra süresini çekmek için ufak bir bekleme
            Invoke("InitializeTimer", 0.1f);
        }
        else if (gameState == EGameState.LEVELCOMPLETE || gameState == EGameState.GAMEOVER || gameState == EGameState.MENU)
        {
            StopTimer();
        }
    }

    private void InitializeTimer()
    {
        // Sahneye az önce doğan Level objesini bul
        Level currentLevel = FindFirstObjectByType<Level>();
        
        if (currentLevel != null)
        {
            currentTimer = currentLevel.Duration;
        }
        else
        {
            Debug.LogWarning("Sahnedeki level prefabında 'Level.cs' bulunamadı! Varsayılan süre: 90 saniye.");
            currentTimer = 90;
        }

        timerText.text = SecondsToString(currentTimer);
        StartTimer();
    }

    private void StartTimer()
    {
        if (isTimerRunning) return;
        isTimerRunning = true;
        InvokeRepeating("UpdateTimer", 1f, 1f);
    }

    private void UpdateTimer()
    {
        currentTimer--;
        timerText.text = SecondsToString(currentTimer);    

        if (currentTimer <= 0)
        {
            TimerFinished();
        }
    }

    private void TimerFinished()
    {
        StopTimer();
        
        // Süre bitince GameManager'a GAMEOVER sinyali yolla
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetGameState(EGameState.GAMEOVER);
        }
    }
    
    private void StopTimer()
    {
        isTimerRunning = false;
        CancelInvoke("UpdateTimer");
        CancelInvoke("InitializeTimer");
    }
    
    private string SecondsToString(int seconds)
    {
        // Sadece dakika ve saniye (00:00) formatında gösterir
        return TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
    }
}