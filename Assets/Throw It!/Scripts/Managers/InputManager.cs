using UnityEngine;
using System;

public class InputManager : MonoBehaviour
{
    // Artık Item göndermiyoruz; tıkladığımız yerin 3D uzaydaki tam noktasını (Vector3) yayınlıyoruz!
    public static Action<Vector3> onScreenTapped;
    
    void Update()
    {
        // İleride GameManager'ı kurduğumuzda buradaki yorum satırlarını açarsın
        // if(GameManager.Instance != null && GameManager.Instance.IsGame())
        {
            HandleControl();
        }
    }
    
    private void HandleControl()
    {
        // Mobilde tek dokunuşu (Single Touch) sorunsuz algılar
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }   
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Lazerimiz ekrandaki herhangi bir şeye (Duvar, masa, teneke) çarptı mı? Menzili 100 yaptık ki yetişsin
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            // DEDEKTİF LOGU: Neye tıkladığımızı Konsolda kabak gibi görüyoruz
            Debug.Log($"<color=yellow>[RAYCAST HEDEFİ]</color> Işın şuna çarptı: {hit.collider.gameObject.name} | Koordinat: {hit.point}");

            // Çarptığı noktanın tam koordinatını fırlatma mekanizmasına (Shooter) haber ver!
            onScreenTapped?.Invoke(hit.point);
        }
        else 
        {
            Debug.Log("<color=red>[RAYCAST BOŞA GİTTİ]</color> Işın hiçbir cisme çarpmadı.");
        }
    }
}