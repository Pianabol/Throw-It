using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CanItem : MonoBehaviour
{
    public static event Action OnCanKnockedDown;

    [Header("Fall & Goal Settings")]
    [Tooltip("Masadan ne kadar aşağı düşerse hedef sayılacak?")]
    [SerializeField] private float fallThresholdY = -2.5f; 

    [Header("Freeze Settings")]
    [Tooltip("Darbe alana kadar havada/yerinde sabit kalsın mı?")]
    [SerializeField] private bool freezeUntilHit = true;
    [SerializeField] private float wakeUpRadius = 1.2f;

    private bool isKnockedDown = false;
    public Rigidbody rb { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (freezeUntilHit)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.Sleep(); 
        }
    }

    private void Update()
    {
        if (!isKnockedDown && transform.position.y < fallThresholdY)
        {
            KnockDown("Y Sınırı Aşımı (Uçuruma Düştü)");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isKnockedDown) return;

        if (other.CompareTag("GoalTrigger"))
        {
            KnockDown("ifIsGoal Trigger Bölgesine Temas Etti");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (freezeUntilHit && !rb.useGravity)
        {
            WakeUpPhysics();
        }
    }

    public void WakeUpPhysics()
    {
        if (rb.useGravity) return; 
        
        rb.useGravity = true; 
        rb.WakeUp(); 

        Collider[] colliders = Physics.OverlapSphere(transform.position, wakeUpRadius);
        foreach (Collider col in colliders)
        {
            if (col.TryGetComponent(out CanItem neighborCan))
            {
                if (neighborCan.rb != null && !neighborCan.rb.useGravity)
                {
                    neighborCan.WakeUpPhysics();
                }
            }
        }
    }

    private void KnockDown(string sebep)
    {
        if (isKnockedDown) return;

        isKnockedDown = true;
        
        // --- SES TETİKLEYİCİSİ: HEDEF DEVRİLME ---
        if (SoundManager.Instance != null)
        {
            // İsminde "Cylin" veya etiketinde "TinTarget" varsa teneke sesi, yoksa ahşap sesi çalar
            if (gameObject.name.Contains("Cylin") || CompareTag("TinTarget"))
            {
                SoundManager.Instance.PlayTinHit();
            }
            else
            {
                SoundManager.Instance.PlayWoodHit();
            }
        }

        Debug.Log($"<color=yellow>[HEDEF DÜŞTÜ]</color> Yere Düşen Item: <b>{gameObject.name}</b> | Neden: {sebep}");

        OnCanKnockedDown?.Invoke();
    }

    public bool IsKnockedDown() => isKnockedDown;
}