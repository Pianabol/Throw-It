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

    [Header("PowerUp Patlama Ayarları")]
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private float explosionForce = 35f;

    // --- YENİ EFEKT DEĞİŞKENLERİ ---
    private GameObject currentImpactVfx;
    private bool isPowerUpBall = false;

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
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }
    }

    public void SetPool(ObjectPool<Ball> pool)
    {
        referencePool = pool;
    }

    // --- YENİ: SHOOTER TOPA HANGİ EFEKTİ KULLANACAĞINI SÖYLER ---
    public void SetImpactEffect(GameObject impactVfx, bool isPowerUp)
    {
        currentImpactVfx = impactVfx;
        isPowerUpBall = isPowerUp;
    }

    public void PrepareForSpawn(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        CacheRigidbody();

        CancelInvoke();

        isReturningToPool = false;
        returnScheduled = false;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.position = spawnPosition;
        rb.rotation = spawnRotation;
        
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);

        rb.Sleep();
    }

    public void Launch(Vector3 targetPoint)
    {
        CacheRigidbody();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.WakeUp();

        Vector3 direction = (targetPoint - transform.position).normalized;
        direction.y += 0.15f;
        direction.Normalize();

        rb.AddForce(direction * launchSpeed, ForceMode.VelocityChange);

        Vector3 randomSpin = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        rb.AddTorque(randomSpin * spinForce, ForceMode.Impulse);

        Invoke(nameof(ReturnToPool), 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isReturningToPool) return;

        // Uçuruma veya yok etme duvarına çarptıysa sessizce havuza dön
        if (collision.gameObject.CompareTag("DespawnWall"))
        {
            ReturnToPool();
            return;
        }

        // --- SES TETİKLEYİCİSİ: TOP BİR YERE ÇARPTIĞINDA PATLAMA SESİ ÇAL ---
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayExplosion();
        }

        // --- YENİ: ÇARPMA ANINDA EFEKTİ PATLAT ---
        if (currentImpactVfx != null)
        {
            ContactPoint contact = collision.contacts[0];
            Instantiate(currentImpactVfx, contact.point, Quaternion.LookRotation(contact.normal));
        }

        // --- YENİ: EĞER GÜÇLENDİRİCİLİ TOPSA ETRAFI DAĞIT ---
        if (isPowerUpBall)
        {
            TriggerExplosion();
        }

        if (returnScheduled) return;

        returnScheduled = true;
        CancelInvoke(nameof(ReturnToPool));
        Invoke(nameof(ReturnToPool), 1.5f);
    }

    // --- YENİ: ALAN PATLAMASI (POWERUP) ---
    private void TriggerExplosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in colliders)
        {
            // Tenekelerin uyku fiziğini aç
            if (hit.TryGetComponent<CanItem>(out var can))
            {
                can.WakeUpPhysics();
            }

            // Şok dalgasını uygula
            if (hit.attachedRigidbody != null)
            {
                hit.attachedRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
            }
        }
    }

    private void ReturnToPool()
    {
        if (isReturningToPool) return;

        if (referencePool == null)
        {
            Debug.LogWarning($"{name}: Ball pool reference bulunamadı.");
            return;
        }

        isReturningToPool = true;
        returnScheduled = false;

        CancelInvoke();
        CacheRigidbody();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.Sleep();

        referencePool.Release(this);
    }
}