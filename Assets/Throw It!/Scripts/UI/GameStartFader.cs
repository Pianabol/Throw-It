using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GameStartFader : MonoBehaviour
{
    [Header(" Fade Settings ")]
    [SerializeField] private float fadeDuration = 1.5f; // Perdenin kalkma süresi (1.5 saniye çok asil durur)

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // 1. Ekran oyun başlar başlamaz simsiyah (Alpha = 1) olsun
        canvasGroup.alpha = 1f;
        
        // 2. Siyah ekran varken oyuncu arkadaki Play butonuna yanlışlıkla basamasın diye tıklamayı kapat
        canvasGroup.blocksRaycasts = true; 

        // 3. Siyah perdeyi yavaşça eritip (Alpha'yı 0'a çekip) arkadaki dünyayı ve menüyü gösteriyoruz
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => 
            {
                // Animasyon bitince tıklama engelini kaldır ve paneli tamamen kapat ki sistemi yormasın
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false); 
            });
    }
}