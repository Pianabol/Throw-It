using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
    private ObjectPool<Ball> referencePool;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetPool(ObjectPool<Ball> pool)
    {
        referencePool = pool;
    }

    public void Launch(Vector3 force)
    {
        // Havuzdan yeni çıktığı için eski fiziksel kalıntıları (hızı) sıfırla
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // Mermiyi hedefe doğru patlat
        rb.AddForce(force, ForceMode.Impulse);

        // Fail-safe: Hiçbir yere çarpmazsa 3 saniye sonra havuza dönsün
        Invoke(nameof(ReturnToPool), 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Bir yere (tenekeye, duvara, masaya) çarptığında 1.5 saniye bekle ve havuza dön
        Invoke(nameof(ReturnToPool), 1.5f); 
    }

    private void ReturnToPool()
    {
        if (gameObject.activeSelf)
        {
            referencePool.Release(this);
        }
    }
}