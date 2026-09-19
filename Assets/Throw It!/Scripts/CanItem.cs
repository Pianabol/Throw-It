using System;
using UnityEngine;

public class CanItem : MonoBehaviour
{
    public static event Action OnCanKnockedDown;

    [Header("Settings")]
    [Tooltip("Masadan ne kadar aşağı düşerse hedef sayılacak?")]
    [SerializeField] private float fallThresholdY = -1.0f;

    private bool isKnockedDown = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Alternatif güvenlik: Obje trigger'ı ıskalasa bile Y ekseninde belli bir sınırın altına inerse say
        if (!isKnockedDown && transform.position.y < fallThresholdY)
        {
            KnockDown();
        }
    }

    // Masanın altındaki 'IfIsGoal?' trigger bölgesine girdiğinde çalışır
    private void OnTriggerEnter(Collider other)
    {
        if (isKnockedDown) return;

        if (other.CompareTag("GoalTrigger"))
        {
            KnockDown();
        }
    }

    private void KnockDown()
    {
        isKnockedDown = true;
        OnCanKnockedDown?.Invoke();
    }

    public bool IsKnockedDown() => isKnockedDown;
}