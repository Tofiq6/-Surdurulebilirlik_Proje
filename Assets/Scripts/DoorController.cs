using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float acilmaAcisi = 90f;
    public float acilmaHizi = 5f;

    private bool acikMi = false;
    private Quaternion hedefRotasyon;
    private Quaternion kapaliRotasyon;

    void Start()
    {
        // Başlangıç pozisyonunu (Kapalı hali) kaydet
        kapaliRotasyon = transform.rotation;
        hedefRotasyon = kapaliRotasyon;
    }

    void Update()
    {
        // Kapıyı hedefe doğru pürüzsüzce döndür (Slerp)
        transform.rotation = Quaternion.Slerp(transform.rotation, hedefRotasyon, Time.deltaTime * acilmaHizi);
    }

    // Bu fonksiyonu oyuncu çağıracak
    public void KapiyiTetikle(Vector3 oyuncuPozisyonu)
    {
        if (!acikMi)
        {
            // Kapıyı AÇ
            
            // Oyuncu kapının neresinde? (Vektör Matematiği)
            // Kapının yönü ile oyuncunun yönü arasındaki açıya bakar.
            Vector3 kapiyaGoreYon = (oyuncuPozisyonu - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.forward, kapiyaGoreYon);

            // Eğer oyuncu öndeyse arkaya, arkadaysa öne aç (-90 veya 90)
            float kapiYonu = (dotProduct > 0) ? -acilmaAcisi : acilmaAcisi;

            // Hedef rotasyonu ayarla (Mevcut rotasyon * Yeni açı)
            hedefRotasyon = Quaternion.Euler(0, kapaliRotasyon.eulerAngles.y + kapiYonu, 0);
            acikMi = true;
        }
        else
        {
            // Kapıyı KAPAT
            hedefRotasyon = kapaliRotasyon;
            acikMi = false;
        }
    }
}