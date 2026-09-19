using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private ObjectPool<Ball> referencePool;
    private Rigidbody rb;

    [Header("Fırlatma Ayarları")]
    [Tooltip("Topun gidiş hızı")]
    [SerializeField] private float launchSpeed = 20f;
    [Tooltip("Topa verilecek ağırlık (Kutuları tokatlaması için)")]
    [SerializeField] private float ballMass = 2f;
    [Tooltip("Havada dönebilmesi için spin gücü")]
    [SerializeField] private float spinForce = 15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // Çok hızlı giden topun hedeflerin içinden geçip gitmesini (Tünelleme - Ghosting) engeller
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        
        // Topa tok bir ağırlık ver ki kutulara çarpınca onları fiziksel olarak savurabilsin
        rb.mass = ballMass;
    }

    public void SetPool(ObjectPool<Ball> pool)
    {
        referencePool = pool;
    }

    public void Launch(Vector3 targetPoint)
    {
        CancelInvoke();

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 direction = (targetPoint - transform.position).normalized;
        
        // Top hedefe giderken yerçekimine yenilip erken düşmesin diye hafif bir kavis (yukarı itme) ekliyoruz
        direction.y += 0.15f; 
        direction = direction.normalized;

        // Hedefe roketle
        rb.AddForce(direction * launchSpeed, ForceMode.VelocityChange);
        
        // Topa rastgele bir eksende dönme (spin) efekti ver ki kutulara çarptığında gerçekçi bir darbe dağıtsın
        rb.AddTorque(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * spinForce, ForceMode.Impulse);

        Invoke(nameof(ReturnToPool), 4f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Hedefe çarptığında anında kaybolmasın, fiziksel devrilmeyi izletsin
        Invoke(nameof(ReturnToPool), 1.5f);
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf && referencePool != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            referencePool.Release(this);
        }
    }
}