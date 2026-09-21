using UnityEngine;

public class SoundManager : MonoBehaviour, IGameStateListener
{
    public static SoundManager Instance { get; private set; }

    [Header(" Audio Sources ")]
    [Tooltip("Sadece arka plan müziklerini çalar (Döngülü)")]
    [SerializeField] private AudioSource musicSource;
    [Tooltip("Efektleri çalar (Üst üste binmesine izin verir)")]
    [SerializeField] private AudioSource sfxSource;

    [Header(" Music & State Clips ")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameMusic;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip loseSound;

    [Header(" Gameplay SFX ")]
    [SerializeField] private AudioClip shootSFX;
    [SerializeField] private AudioClip ballExplosionSFX;
    [SerializeField] private AudioClip tinHitSFX;
    [SerializeField] private AudioClip woodHitSFX;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Trafik Polisi (GameManager) durum değiştirdiğinde burası otomatik dinler!
    public void GameStateChangedCallBack(EGameState gameState)
    {
        switch (gameState)
        {
            case EGameState.MENU:
                PlayMusic(menuMusic);
                break;
                
            case EGameState.GAME:
                PlayMusic(gameMusic);
                break;
                
            case EGameState.LEVELCOMPLETE:
                musicSource.Stop(); // Oyundaki gergin müziği kes
                PlaySFX(winSound, 1f); // Zafer sesini (Pitch değiştirmeden) çal
                break;
                
            case EGameState.GAMEOVER:
                musicSource.Stop(); // Müziği kes
                PlaySFX(loseSound, 1f); // Hüsran sesini çal
                break;
        }
    }

    // --- MÜZİK OYNATICI ---
    private void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        
        // Eğer aynı müzik zaten çalıyorsa baştan başlatma, çalmaya devam etsin
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true; // Müzikler hep döngüde kalır
        musicSource.Play();
    }

    // --- EFEKT OYNATICI (Çekirdek Metot) ---
    private void PlaySFX(AudioClip clip, float pitch)
    {
        if (clip == null) return;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip); // PlayOneShot seslerin üst üste binmesine izin verir
    }

    // --- OYUN İÇİNDEN ÇAĞRILACAK KISA METOTLAR (PITCH RANDOM İLE) ---
    public void PlayShoot() => PlaySFX(shootSFX, Random.Range(0.9f, 1.1f));
    public void PlayExplosion() => PlaySFX(ballExplosionSFX, Random.Range(0.8f, 1.2f));
    public void PlayTinHit() => PlaySFX(tinHitSFX, Random.Range(0.85f, 1.15f));
    public void PlayWoodHit() => PlaySFX(woodHitSFX, Random.Range(0.85f, 1.15f));
}