using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Prefabs")]
    [SerializeField] private GameObject[] levelPrefabs;

    [Header("Spawn Settings")]
    [Tooltip("Masanın doğacağı tam koordinat/anchor")]
    [SerializeField] private Transform levelSpawnPoint;

    private GameObject currentLevelInstance;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentLevelIndex = PlayerPrefs.GetInt("CurrentLevel", 0);
        SpawnLevel();
    }

    public void SpawnLevel()
    {
        // 1. Önceki seviyeden kalan masayı ve hedefleri temizle
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("LevelManager: Prefab listesi boş!");
            return;
        }

        // 2. Güvenli indeks (Son level geçilirse başa sar)
        int safeIndex = currentLevelIndex % levelPrefabs.Length;

        // 3. Level'ı tam belirlenen Anchor noktasında sahneye yarat
        Vector3 spawnPos = levelSpawnPoint != null ? levelSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = levelSpawnPoint != null ? levelSpawnPoint.rotation : Quaternion.identity;

        currentLevelInstance = Instantiate(levelPrefabs[safeIndex], spawnPos, spawnRot);

        // 4. Hedefleri GoalManager'a otomatik bildir/tetikle
        InitGoalsForNewLevel();
    }

    private void InitGoalsForNewLevel()
    {
        // GoalManager yeni doğan hedefleri hemen bulsun
        if (GoalManager.Instance != null)
        {
            // GoalManager'ın yeni doğan hedefleri sayması için metodunu tetikle
            GoalManager.Instance.ResetAndCountGoals();
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevelIndex);
        PlayerPrefs.Save();
        SpawnLevel();
    }

    public void RestartLevel()
    {
        SpawnLevel();
    }
}