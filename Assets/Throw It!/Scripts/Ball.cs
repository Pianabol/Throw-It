using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private Rigidbody rb;
    private ObjectPool<Ball> referencePool;

    [Header("Fırlatma Ayarları")]
    [SerializeField] private float launchSpeed = 20f;
    [SerializeField] private float ballMass = 2f;
    [SerializeField] private float spinForce = 15f;

    // Pool'a iki kere dönmesini engeller
    private bool isReturningToPool = false;

    // İlk çarpışmadan sonra tekrar tekrar timer kurulmasını engeller
    private bool returnScheduled = false;


    private void Awake()
    {
        CacheRigidbody();
    }


    private void CacheRigidbody()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();

            rb.mass = ballMass;

            // Hızlı topun collider'lardan geçmesini engeller
            rb.collisionDetectionMode =
                CollisionDetectionMode.ContinuousDynamic;
        }
    }


    /// <summary>
    /// Shooter oluşturduğu topa hangi havuza geri döneceğini söyler.
    /// </summary>
    public void SetPool(ObjectPool<Ball> pool)
    {
        referencePool = pool;
    }


    /// <summary>
    /// Top ACTIVE edilmeden ÖNCE çağrılmalıdır.
    /// Eski pooling state'inin tamamını temizler
    /// ve topu yeni spawn noktasına taşır.
    /// </summary>
    public void PrepareForSpawn(
        Vector3 spawnPosition,
        Quaternion spawnRotation)
    {
        CacheRigidbody();

        // Önceki kullanımından kalan timer'ları sil
        CancelInvoke();

        isReturningToPool = false;
        returnScheduled = false;

        // Önceki hızları tamamen temizle
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Rigidbody üzerinden pozisyon değiştiriyoruz
        // çünkü fizik objesi.
        rb.position = spawnPosition;
        rb.rotation = spawnRotation;

        // Transform'u da garanti olarak eşitle
        transform.SetPositionAndRotation(
            spawnPosition,
            spawnRotation
        );

        // Pool'dan çıkana kadar fizik hesaplamasın
        rb.Sleep();
    }


    /// <summary>
    /// Top aktif edildikten sonra çağrılır.
    /// </summary>
    public void Launch(Vector3 targetPoint)
    {
        CacheRigidbody();

        // Her ihtimale karşı tekrar temizle
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.WakeUp();

        // Çalışan eski fizik sistemindeki hesap AYNI
        Vector3 direction =
            (targetPoint - transform.position).normalized;

        // Yerçekimi telafisi
        direction.y += 0.15f;
        direction.Normalize();

        // Fırlatma
        rb.AddForce(
            direction * launchSpeed,
            ForceMode.VelocityChange
        );

        // Spin
        Vector3 randomSpin = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        rb.AddTorque(
            randomSpin * spinForce,
            ForceMode.Impulse
        );

        // Hiçbir şeye çarpmazsa 4 saniye sonra havuza dön
        Invoke(nameof(ReturnToPool), 4f);
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Zaten havuza dönüyorsa hiçbir şey yapma
        if (isReturningToPool)
            return;


        // DespawnWall'a çarptıysa direkt havuza gönder
        if (collision.gameObject.CompareTag("DespawnWall"))
        {
            ReturnToPool();
            return;
        }


        // İlk normal çarpışmayı zaten gördüysek
        // yeniden timer oluşturma
        if (returnScheduled)
            return;


        returnScheduled = true;

        // Launch sırasında başlayan 4 saniyelik timer'ı iptal et
        CancelInvoke(nameof(ReturnToPool));

        // Normal çarpışmadan 1.5 saniye sonra geri dön
        Invoke(nameof(ReturnToPool), 1.5f);
    }


    private void ReturnToPool()
    {
        // Çift Release koruması
        if (isReturningToPool)
            return;

        if (referencePool == null)
        {
            Debug.LogWarning(
                $"{name}: Ball pool reference bulunamadı."
            );

            return;
        }

        isReturningToPool = true;
        returnScheduled = false;

        CancelInvoke();

        CacheRigidbody();

        // Havuzda eski momentum KESİNLİKLE kalmasın
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.Sleep();

        // Shooter içindeki OnReturnObjToPool çalışacak
        referencePool.Release(this);
    }
}
