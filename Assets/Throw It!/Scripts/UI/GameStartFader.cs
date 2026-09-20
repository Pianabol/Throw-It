using UnityEngine;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class GameStartFader : MonoBehaviour
{
    public static GameStartFader Instance { get; private set; }

    [Header(" Fade Settings ")]
    [SerializeField] private float fadeDuration = 0.5f; // Geçişleri hızlandırdım ki oyuncu sıkılmasın

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        // 1. Oyun ilk açıldığında veya sahne yenilendiğinde otomatik aydınlan (Fade In)
        canvasGroup.alpha = 1f;
        FadeIn();
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true; 

        LeanTween.alphaCanvas(canvasGroup, 0f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => 
            {
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false); 
            });
    }

    public void FadeOut(Action onComplete)
    {
        gameObject.SetActive(true);
        canvasGroup.blocksRaycasts = true; 

        LeanTween.alphaCanvas(canvasGroup, 1f, fadeDuration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => 
            {
                // Kararma bitince GameManager'ın istediği işlevi çalıştır (Örn: Sahne yükle)
                onComplete?.Invoke();
            });
    }
}