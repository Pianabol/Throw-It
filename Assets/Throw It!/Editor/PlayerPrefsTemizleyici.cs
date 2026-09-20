using UnityEngine;
using UnityEditor; // Bu kütüphane sayesinde Tools menüsüne erişebiliyoruz

public class PlayerPrefsTemizleyici : MonoBehaviour
{
    // Bu kod, Unity'nin en üstündeki "Tools" menüsüne yeni bir buton ekler.
    [MenuItem("Tools/🧹 Hafızayı Temizle (PlayerPrefs)")]
    public static void HafizayiTemizle()
    {
        // Cihazdaki tüm PlayerPrefs kayıtlarını (Level, ses ayarları vb.) siler
        PlayerPrefs.DeleteAll();
        
        // Değişikliğin anında algılanması için kaydeder
        PlayerPrefs.Save();
        
        // Konsola temizlendiğine dair bir geri bildirim mesajı atar
        Debug.Log("<color=green>✅ TÜM HAFIZA (PLAYER PREFS) TERTEMİZ EDİLDİ!</color> Oyun artık Level 1'den başlayacak.");
    }
}