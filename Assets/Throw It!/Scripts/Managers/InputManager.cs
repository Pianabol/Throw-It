using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    public static Action<Vector3> onScreenTapped;
    
    void Update()
    {
        if(GameManager.Instance != null && GameManager.Instance.IsGame())
        {
            HandleControl();
        }
    }
    
    private void HandleControl()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }   
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Debug.Log($"<color=yellow>[RAYCAST HEDEFİ]</color> Işın şuna çarptı: {hit.collider.gameObject.name} | Koordinat: {hit.point}");

            // --- SES TETİKLEYİCİSİ: ATEŞ ETME ---
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayShoot();
            }

            onScreenTapped?.Invoke(hit.point);
        }
        else 
        {
            Debug.Log("<color=red>[RAYCAST BOŞA GİTTİ]</color> Işın hiçbir cisme çarpmadı.");
        }
    }
}