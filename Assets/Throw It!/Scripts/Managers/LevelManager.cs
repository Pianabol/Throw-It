using UnityEngine;

public class LevelManager : MonoBehaviour, IGameStateListener
{
    public static LevelManager Instance { get; private set; }

    [Header(" Level Settings ")]
    [Tooltip("Sırasıyla oynanacak level prefabları")]
    [SerializeField] private GameObject[] levelPrefabs;
    
    [Tooltip("Levelların doğacağı Anchor/Pivot noktası")]
    [SerializeField] private Transform levelSpawnPoint;

    private const string levelKey = "LevelReached";
    private int levelIndex = 0;
    private GameObject currentLevelInstance;

    // UIManager'ın ekrana yazdırabilmesi için mevcut level numarası
    public int CurrentLevelNum => levelIndex + 1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadData();
    }

    private void LoadData()
    {
        levelIndex = PlayerPrefs.GetInt(levelKey, 0);
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(levelKey, levelIndex);
        PlayerPrefs.Save();
    }

    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (gameState == EGameState.GAME)
        {
            SpawnLevel();
        }
        else if (gameState == EGameState.LEVELCOMPLETE)
        {
            levelIndex++;
            SaveData();
        }
        else if (gameState == EGameState.MENU)
        {
            ClearLevel();
        }
    }

    private void SpawnLevel()
    {
        ClearLevel();

        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("LevelManager: Prefab listesi boş!");
            return;
        }

        int safeIndex = Mathf.Clamp(levelIndex, 0, levelPrefabs.Length - 1);

        Vector3 spawnPos = levelSpawnPoint != null ? levelSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = levelSpawnPoint != null ? levelSpawnPoint.rotation : Quaternion.identity;

        // 1. Yeni leveli oluştur
        currentLevelInstance = Instantiate(levelPrefabs[safeIndex], spawnPos, spawnRot);

        // 2. SADECE bu yeni levelin altındaki hedefleri say (Eski ölü şişeler asla karışamaz!)
        CanItem[] targets = currentLevelInstance.GetComponentsInChildren<CanItem>();

        // 3. GoalManager'a kesin hedef sayısını teslim et
        if (GoalManager.Instance != null)
        {
            GoalManager.Instance.SetGoals(targets.Length);
        }
    }

    private void ClearLevel()
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
            currentLevelInstance = null;
        }
    }

    public bool IsGameFinished()
    {
        return levelIndex >= levelPrefabs.Length - 1;
    }
}