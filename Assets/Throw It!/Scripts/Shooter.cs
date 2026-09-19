using UnityEngine;
using UnityEngine.Pool;

public class Shooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Ball ballPrefab;

    [SerializeField] private Transform muzzlePoint;

    // Group064
    [SerializeField] private Transform cannonBarrel;


    [Header("Recoil Animation")]
    [SerializeField] private float recoilDuration = 0.25f;

    [SerializeField] private float targetYAngle = 25f;


    [Header("Object Pool")]
    [SerializeField] private int defaultPoolSize = 10;

    [SerializeField] private int maxPoolSize = 30;


    private ObjectPool<Ball> ballPool;

    private Vector3 initialBarrelLocalRot;

    private bool isAnimating = false;


    private void Awake()
    {
        CreatePool();

        if (cannonBarrel != null)
        {
            initialBarrelLocalRot =
                cannonBarrel.localEulerAngles;
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


        // Pool'dan top al
        //
        // ÖNEMLİ:
        // Bu noktada top HALA INACTIVE.
        Ball newBall = ballPool.Get();


        Vector3 spawnPosition;

        Quaternion spawnRotation;


        if (muzzlePoint != null)
        {
            spawnPosition = muzzlePoint.position;

            // Eski Instantiate sistemindeki davranışı koruyoruz
            spawnRotation = Quaternion.identity;
        }
        else
        {
            spawnPosition = transform.position;
            spawnRotation = Quaternion.identity;
        }


        /*
         * EN KRİTİK BÖLÜM
         *
         * Top fizik dünyasına girmeden önce:
         *
         * - eski hız temizleniyor
         * - eski dönüş temizleniyor
         * - eski timer temizleniyor
         * - namluya taşınıyor
         *
         * Top şu anda HALA inactive.
         */
        newBall.PrepareForSpawn(
            spawnPosition,
            spawnRotation
        );


        /*
         * Artık doğru yerde.
         *
         * ŞİMDİ aktive ediyoruz.
         */
        newBall.gameObject.SetActive(true);


        /*
         * Aktif ve doğru pozisyondaki topa
         * şimdi fizik kuvveti uygulanabilir.
         */
        newBall.Launch(targetPoint);


        PlayRecoilAnimation();
    }


    private void PlayRecoilAnimation()
    {
        if (cannonBarrel == null)
            return;


        isAnimating = true;


        Vector3 recoilRotation = new Vector3(
            initialBarrelLocalRot.x,
            targetYAngle,
            initialBarrelLocalRot.z
        );


        LeanTween
            .rotateLocal(
                cannonBarrel.gameObject,
                recoilRotation,
                recoilDuration * 0.4f
            )
            .setEase(LeanTweenType.easeOutQuad)

            .setOnComplete(() =>
            {
                LeanTween
                    .rotateLocal(
                        cannonBarrel.gameObject,
                        initialBarrelLocalRot,
                        recoilDuration * 0.6f
                    )
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
        /*
         * Havuz boşsa Unity yeni bir top oluşturur.
         *
         * Instantiate SADECE havuzun kapasitesi
         * yetersiz olduğunda gerçekleşir.
         */
        Ball ball = Instantiate(ballPrefab);


        // Top hangi havuza geri döneceğini bilsin
        ball.SetPool(ballPool);


        /*
         * Yeni oluşturulan top fizik dünyasında
         * boş yere beklemesin.
         */
        ball.gameObject.SetActive(false);


        return ball;
    }


    private void OnTakeObjFromPool(Ball ball)
    {
        /*
         * BURADA BİLEREK SetActive(true) YOK.
         *
         * Çok önemli.
         *
         * Önce Fire() içinde top:
         *
         * 1. resetlenecek
         * 2. namluya taşınacak
         * 3. sonra SetActive(true) olacak
         *
         * Önceki bug'ın ana kaynaklarından biri
         * burada erkenden SetActive(true) yapılmasıydı.
         */
    }


    private void OnReturnObjToPool(Ball ball)
    {
        if (ball == null)
            return;


        // Fizik dünyasından tamamen çıkar
        ball.gameObject.SetActive(false);
    }


    private void OnDestroyPoolObj(Ball ball)
    {
        if (ball == null)
            return;


        Destroy(ball.gameObject);
    }
}