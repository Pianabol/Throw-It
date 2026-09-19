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

    // Trafik Polisi (GameManager) durum değiştirdiğinde burası otomatik tetiklenir
    public void GameStateChangedCallBack(EGameState gameState)
    {
        if (gameState == EGameState.GAME)
        {
            // Oyun başladığında leveli sahneye fırlat
            SpawnLevel();
        }
        else if (gameState == EGameState.LEVELCOMPLETE)
        {
            // Level başarıyla bittiyse index'i artır ve cihaz hafızasına kaydet
            levelIndex++;
            SaveData();
        }
    }

    private void SpawnLevel()
    {
        // Önceki seviyeden kalan masayı ve hedefleri temizle
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        if (levelPrefabs == null || levelPrefabs.Length == 0)
        {
            Debug.LogError("LevelManager: Prefab listesi boş!");
            return;
        }

        // Güvenli indeks (Son levele ulaşıldığında index'i sınırla)
        int safeIndex = Mathf.Clamp(levelIndex, 0, levelPrefabs.Length - 1);

        Vector3 spawnPos = levelSpawnPoint != null ? levelSpawnPoint.position : Vector3.zero;
        Quaternion spawnRot = levelSpawnPoint != null ? levelSpawnPoint.rotation : Quaternion.identity;

        // Leveli tam belirlenen noktada sahneye yarat
        currentLevelInstance = Instantiate(levelPrefabs[safeIndex], spawnPos, spawnRot);

        // Hedefleri GoalManager'a otomatik bildir
        if (GoalManager.Instance != null)
        {
            GoalManager.Instance.ResetAndCountGoals();
        }
    }

    /// <summary>
    /// GameManager, LEVELCOMPLETE sinyali geldiğinde bu metodu sorar.
    /// Eğer true dönerse sinyali GAMEFINISHED olarak değiştirir.
    /// </summary>
    public bool IsGameFinished()
    {
        return levelIndex >= levelPrefabs.Length - 1;
    }
}