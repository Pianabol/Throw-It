using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private Transform muzzlePoint;
    
    // Group064
    [SerializeField] private Transform cannonBarrel;

    [Header("VFX Prefabs (Cartoon FX)")]
    [Tooltip("Normal atışta namlu ucunda patlayacak efekt")]
    [SerializeField] private GameObject normalShotVfx;
    [Tooltip("PowerUp atışında namlu ucunda patlayacak efekt")]
    [SerializeField] private GameObject powerUpShotVfx;
    [Tooltip("Normal top çarpma efekti")]
    [SerializeField] private GameObject normalImpactVfx;
    [Tooltip("PowerUp top çarpma efekti (Büyük Patlama)")]
    [SerializeField] private GameObject powerUpImpactVfx;

    [Header("Recoil Animation")]
    [SerializeField] private float recoilDuration = 0.25f;
    [SerializeField] private float targetYAngle = 25f;

    [Header("Object Pool")]
    [SerializeField] private int defaultPoolSize = 10;
    [SerializeField] private int maxPoolSize = 30;

    // PowerUp (Süper Top) aktif mi?
    public bool IsNextShotPowerUp { get; set; } = false;

    private ObjectPool<Ball> ballPool;
    private Vector3 initialBarrelLocalRot;
    private bool isAnimating = false;

    private void Awake()
    {
        CreatePool();

        if (cannonBarrel != null)
        {
            initialBarrelLocalRot = cannonBarrel.localEulerAngles;
        }
    }

    private void OnEnable()
    {
        InputManager.onScreenTapped += Fire;
    }

    private void OnDisable()
    {
        InputManager.onScreenTapped -= Fire;
    }

    private void CreatePool()
    {
        ballPool = new ObjectPool<Ball>(
            createFunc: CreatePoolObj,
            actionOnGet: OnTakeObjFromPool,
            actionOnRelease: OnReturnObjToPool,
            actionOnDestroy: OnDestroyPoolObj,
            collectionCheck: true,
            defaultCapacity: defaultPoolSize,
            maxSize: maxPoolSize
        );
    }

    private void Fire(Vector3 targetPoint)
    {
        // Recoil devam ederken tekrar ateş etme
        if (isAnimating)
            return;

        if (ballPool == null)
        {
            Debug.LogError("Ball Pool oluşturulmamış.");
            return;
        }

        // Pool'dan top al (Top HALA INACTIVE)
        Ball newBall = ballPool.Get();

        Vector3 spawnPosition;
        Quaternion spawnRotation;

        if (muzzlePoint != null)
        {
            spawnPosition = muzzlePoint.position;
            spawnRotation = Quaternion.identity;
        }
        else
        {
            spawnPosition = transform.position;
            spawnRotation = Quaternion.identity;
        }

        /*
         * EN KRİTİK BÖLÜM (KUSURSUZ MİMARİN)
         * Top fizik dünyasına girmeden önce resetleniyor ve namluya taşınıyor.
         */
        newBall.PrepareForSpawn(spawnPosition, spawnRotation);

        // --- YENİ: EFEKT VE POWERUP BİLGİSİNİ TOPA AKTAR ---
        GameObject impactToUse = IsNextShotPowerUp ? powerUpImpactVfx : normalImpactVfx;
        newBall.SetImpactEffect(impactToUse, IsNextShotPowerUp);

        // Artık doğru yerde. ŞİMDİ aktive ediyoruz.
        newBall.gameObject.SetActive(true);

        // Aktif ve doğru pozisyondaki topa şimdi fizik kuvveti uygulanabilir.
        newBall.Launch(targetPoint);

        // --- YENİ: NAMLU UCUNDA ATEŞ EFEKTİNİ PATLAT ---
        PlayMuzzleFlash(spawnPosition, muzzlePoint != null ? muzzlePoint.rotation : Quaternion.identity);

        // Atış yapıldığı için PowerUp hakkı normale döner
        IsNextShotPowerUp = false;

        PlayRecoilAnimation();
    }

    private void PlayMuzzleFlash(Vector3 position, Quaternion rotation)
    {
        GameObject vfxToSpawn = IsNextShotPowerUp ? powerUpShotVfx : normalShotVfx;
        if (vfxToSpawn != null)
        {
            Instantiate(vfxToSpawn, position, rotation);
        }
    }

    private void PlayRecoilAnimation()
    {
        if (cannonBarrel == null) return;
        isAnimating = true;

        Vector3 recoilRotation = new Vector3(
            initialBarrelLocalRot.x,
            targetYAngle,
            initialBarrelLocalRot.z
        );

        LeanTween
            .rotateLocal(cannonBarrel.gameObject, recoilRotation, recoilDuration * 0.4f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() =>
            {
                LeanTween
                    .rotateLocal(cannonBarrel.gameObject, initialBarrelLocalRot, recoilDuration * 0.6f)
                    .setEase(LeanTweenType.easeOutBack)
                    .setOnComplete(() =>
                    {
                        isAnimating = false;
                    });
            });
    }

    // =========================================================
    // OBJECT POOL
    // =========================================================

    private Ball CreatePoolObj()
    {
        Ball ball = Instantiate(ballPrefab);
        ball.SetPool(ballPool);
        ball.gameObject.SetActive(false);
        return ball;
    }

    private void OnTakeObjFromPool(Ball ball) { /* Bilerek boş */ }

    private void OnReturnObjToPool(Ball ball)
    {
        if (ball == null) return;
        ball.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObj(Ball ball)
    {
        if (ball == null) return;
        Destroy(ball.gameObject);
    }
}