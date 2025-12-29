using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Ayarlar")]
    public float yurumeHizi = 3.0f;
    public float kosmaHizi = 6.0f;
    public float ziplamaGucu = 1.5f;
    public float yerCekimi = -15.0f; // Daha tok düşüş için artırdık

    [Header("Kamera ve Kafa Ayarları")]
    public Camera playerCamera;
    public Transform kafaKemigi; // BURASI YENİ: Mixamo:Head kemiğini buraya atacağız
    public Vector3 kameraOffset = new Vector3(0, 0.1f, 0.2f); // İnce ayar için
    
    public float fareHassasiyeti = 2.0f;
    public float bakisSiniri = 85.0f;

    [Header("Animasyon")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Fizik ve Hareket İşlemleri
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool hareketEdiyor = new Vector3(x, 0, z).sqrMagnitude > 0.01f;
        
        // Yön hesapla (Kamera yönüne göre değil, vücut yönüne göre hareket)
        Vector3 move = transform.right * x + transform.forward * z;
        
        bool kosuyorMu = Input.GetKey(KeyCode.LeftShift) && z > 0;
        float currentSpeed = kosuyorMu ? kosmaHizi : yurumeHizi;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Zıplama
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(ziplamaGucu * -2f * yerCekimi);
            animator.SetTrigger("Jump");
        }

        velocity.y += yerCekimi * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Animasyon
        UpdateAnimations(x, z, kosuyorMu, hareketEdiyor);
    }

    // Tüm Kamera ve Kafa Takip İşlemleri Burada (Titremeyi önlemek için LateUpdate)
    void LateUpdate()
    {
        // 1. Mouse ile Etrafa Bakma (Rotasyon)
        float mouseX = Input.GetAxis("Mouse X") * fareHassasiyeti;
        float mouseY = Input.GetAxis("Mouse Y") * fareHassasiyeti;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -bakisSiniri, bakisSiniri);

        // Kamerayı döndür
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Vücudu döndür
        transform.Rotate(Vector3.up * mouseX);

        // 2. KAFA KEMİĞİNİ TAKİP ETME (Pozisyon Kilitleme)
        if (kafaKemigi != null)
        {
            // Kameranın pozisyonunu, kafa kemiğinin olduğu yere ışınla + Offset ekle
            // TransformDirection kullanıyoruz ki karakter dönerse offset de dönsün
            playerCamera.transform.position = kafaKemigi.position + transform.TransformDirection(kameraOffset);
        }
    }

    void UpdateAnimations(float x, float z, bool isRunning, bool isMoving)
    {
        float targetAnimValue = isMoving ? (isRunning ? 1.0f : 0.5f) : 0f;
        animator.SetFloat("Speed", targetAnimValue, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
    }
}