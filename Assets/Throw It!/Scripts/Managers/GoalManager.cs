using System;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static GoalManager Instance { get; private set; }

    public static event Action<int, int> OnGoalProgressChanged; // (Kalan, Toplam)
    public static event Action OnAllGoalsCompleted;

    private int totalTargets;
    private int remainingTargets;

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

    private void Start()
    {
        // Sahnedeki tüm CanItem bileşenlerini otomatik bul ve say
        CanItem[] targets = FindObjectsByType<CanItem>(FindObjectsSortMode.None);
        totalTargets = targets.Length;
        remainingTargets = totalTargets;

        OnGoalProgressChanged?.Invoke(remainingTargets, totalTargets);
    }

    private void HandleCanKnockedDown()
    {
        remainingTargets--;
        OnGoalProgressChanged?.Invoke(remainingTargets, totalTargets);

        if (remainingTargets <= 0)
        {
            Debug.Log("🎉 TÜM HEDEFLER DEVRİLDİ! LEVEL COMPLETE!");
            OnAllGoalsCompleted?.Invoke();
        }
    }
}