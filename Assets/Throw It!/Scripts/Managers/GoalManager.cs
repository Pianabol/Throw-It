using System;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    public static event Action<int, int> OnGoalProgressChanged; // (Kalan, Toplam)
    public static event Action OnAllGoalsCompleted;

    private int totalTargets = 0;
    private int remainingTargets = 0;
    private bool isLevelCompleted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        CanItem.OnCanKnockedDown += HandleCanKnockedDown;
    }

    private void OnDisable()
    {
        CanItem.OnCanKnockedDown -= HandleCanKnockedDown;
    }

    /// <summary>
    /// Sadece LevelManager yeni leveli sahneye koyduktan sonra çağrılacak!
    /// </summary>
    public void ResetAndCountGoals()
    {
        isLevelCompleted = false;

        // Sahnedeki hedefleri bul
        CanItem[] targets = FindObjectsByType<CanItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        totalTargets = targets.Length;
        remainingTargets = totalTargets;

        Debug.Log($"<color=cyan>[GOAL MANAGER]</color> Yeni Level Başlatıldı. Toplam Hedef: {totalTargets}");

        OnGoalProgressChanged?.Invoke(remainingTargets, totalTargets);
    }

    private void HandleCanKnockedDown()
    {
        if (isLevelCompleted || totalTargets == 0) return;

        remainingTargets--;
        Debug.Log($"<color=orange>[GOAL MANAGER]</color> Hedef Devrildi! Kalan Hedef: {remainingTargets} / {totalTargets}");

        OnGoalProgressChanged?.Invoke(remainingTargets, totalTargets);

        // Ancak hedefler gerçekten sayılmışsa ve sıfıra indiyse bitir
        if (remainingTargets <= 0 && totalTargets > 0)
        {
            isLevelCompleted = true;
            Debug.Log("<color=green>🎉 TÜM HEDEFLER DEVRİLDİ! LEVEL COMPLETE!</color>");
            OnAllGoalsCompleted?.Invoke();
        }
    }

    public int GetRemainingTargets() => remainingTargets;
    public int GetTotalTargets() => totalTargets;
}