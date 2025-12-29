using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float yurumeHizi = 3.0f;
    public float kosmaHizi = 6.0f;
    public float ziplamaGucu = 1.5f;
    public float yerCekimi = -15.0f;

    [Header("Kamera ve Kafa")]
    public Camera playerCamera;
    public Transform kafaKemigi;
    public Vector3 kameraOffset = new Vector3(0, 0.15f, 0.2f);
    public float fareHassasiyeti = 2.0f;
    public float bakisSiniri = 85.0f;

    [Header("Animasyon")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded; // Bizim kontrol ettiğimiz güvenli değişken
    private float xRotation = 0f;

    // KÖR NOKTA AYARLARI (Sorunu çözecek kısım)
    private float zeminKilitZamani = 0f; // Ne zamana kadar yeri algılamayalım?

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. ZEMİN KONTROLÜ (GELİŞMİŞ)
        bool unityDiyorKiYerdeyiz = controller.isGrounded;

        // Eğer zıpladıktan sonraki o 0.2 saniyelik "Körlük" süresi dolmadıysa...
        if (Time.time < zeminKilitZamani)
        {
            isGrounded = false; // Yere değse bile havada say! (Animasyonu korumak için)
        }
        else
        {
            // Süre dolduysa Unity'nin dediğine güvenebiliriz
            isGrounded = unityDiyorKiYerdeyiz;
        }

        // Yere tam bastığımızda dikey hızı sıfırla (Karakterin yere yapışması için)
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            // Yerdeyken Trigger takılı kalmasın diye sürekli temizle
            animator.ResetTrigger("Jump"); 
        }

        // 2. HAREKET
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool hareketEdiyor = new Vector3(x, 0, z).sqrMagnitude > 0.01f;

        Vector3 move = transform.right * x + transform.forward * z;
        bool kosuyorMu = Input.GetKey(KeyCode.LeftShift) && z > 0;
        float currentSpeed = kosuyorMu ? kosmaHizi : yurumeHizi;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // 3. ZIPLAMA KOMUTU
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Fiziksel zıplama
            velocity.y = Mathf.Sqrt(ziplamaGucu * -2f * yerCekimi);
            
            // Animasyonu tetikle
            animator.SetTrigger("Jump");

            // KRİTİK NOKTA: Zıpladığım an, 0.3 saniye boyunca zemin kontrolünü kilitle!
            // Bu süre boyunca isGrounded ASLA true olamaz.
            // Böylece animasyon yarıda kesilip başa saramaz.
            zeminKilitZamani = Time.time + 0.3f; 
        }

        // 4. YERÇEKİMİ
        velocity.y += yerCekimi * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. ANİMASYON GÜNCELLEME
        UpdateAnimations(x, z, kosuyorMu, hareketEdiyor);
    }

    void LateUpdate()
    {
        // Kamera Takibi
        float mouseX = Input.GetAxis("Mouse X") * fareHassasiyeti;
        float mouseY = Input.GetAxis("Mouse Y") * fareHassasiyeti;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -bakisSiniri, bakisSiniri);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        if (kafaKemigi != null)
        {
            playerCamera.transform.position = kafaKemigi.position + transform.TransformDirection(kameraOffset);
        }
    }

    void UpdateAnimations(float x, float z, bool isRunning, bool isMoving)
    {
        // Hız değerini ayarla
        float targetAnimValue = isMoving ? (isRunning ? 1.0f : 0.5f) : 0f;
        animator.SetFloat("Speed", targetAnimValue, 0.1f, Time.deltaTime);
        
        // Yere basma bilgisini animatöre gönder
        animator.SetBool("IsGrounded", isGrounded);
    }
}