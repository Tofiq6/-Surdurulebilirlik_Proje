using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Ayarlar")]
    public float etkilesimMesafesi = 3f;
    public LayerMask etkilesimKatmani; // Sadece kapıları görsün diye filtre

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            EtkilesimeGir();
        }
    }

    void EtkilesimeGir()
    {
        // Ekranın tam ortasından bir ışın oluştur
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // Işın bir şeye çarptı mı?
        if (Physics.Raycast(ray, out hit, etkilesimMesafesi, etkilesimKatmani))
        {
            // Çarptığı şeyde "DoorController" scripti var mı?
            DoorController kapi = hit.transform.GetComponentInParent<DoorController>(); // Parent'ta arıyoruz çünkü script menteşede
            
            if (kapi != null)
            {
                // Kapıya "Ben buradayım, açıl" komutu gönder
                kapi.KapiyiTetikle(transform.position);
            }
        }
    }
    
    // Editörde ışını görelim (Debug için kırmızı çizgi)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * etkilesimMesafesi);
    }
}