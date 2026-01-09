using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidad base")]
    [SerializeField] private float velocidadInicial = 5f;
    [SerializeField] private float incrementoVelocidadPorTiempo = 0.5f;

    [Header("Multiplicadores por tilemap")]
    [SerializeField] private float multiplicadorHielo = 1.4f;
    [SerializeField] private float multiplicadorEnredadera = 0.5f;

    [Header("Movimiento horizontal")]
    [SerializeField] private float multiplicadorHorizontal = 1f;

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    // Estado interno
    private float velocidadActual;
    private bool haLlegadoAMeta = false;
    private string tilemapActual = "Suelo";

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Configuración Rigidbody2D
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;

        velocidadActual = velocidadInicial;
    }

    void Update()
    {
        if (haLlegadoAMeta)
        {
            ActualizarAnimaciones(0f);
            return;
        }

        // 🔥 Incremento progresivo de velocidad
        velocidadActual += incrementoVelocidadPorTiempo * Time.deltaTime;

        // Multiplicador según tilemap
        float multiplicadorVertical = 1f;
        if (tilemapActual == "Hielo")
            multiplicadorVertical = multiplicadorHielo;
        else if (tilemapActual == "Enredadera")
            multiplicadorVertical = multiplicadorEnredadera;

        float velocidadVerticalFinal = velocidadActual * multiplicadorVertical;

        // Movimiento horizontal
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        float velocidadHorizontalFinal =
            velocidadActual * multiplicadorHorizontal * inputHorizontal;

        float nuevaPosX = transform.position.x + velocidadHorizontalFinal * Time.deltaTime;
        float nuevaPosY = transform.position.y + velocidadVerticalFinal * Time.deltaTime;

        // Aplicar movimiento usando Rigidbody2D
        rb.MovePosition(new Vector2(nuevaPosX, nuevaPosY));

        ActualizarAnimaciones(inputHorizontal);
    }

    // =======================
    // TILEMAP DETECTOR
    // =======================
    public void SetTilemapActual(string nombreTilemap)
    {
        // Ignorar Tilemap-Bordes
        if (nombreTilemap == "Tilemap-Bordes")
            return;

        tilemapActual = nombreTilemap;
    }

    // =======================
    // META
    // =======================
    public void LlegarAMeta()
    {
        haLlegadoAMeta = true;
        rb.linearVelocity = Vector2.zero;

        if (animator != null && TieneParametro("run"))
            animator.SetBool("run", false);
    }

    // =======================
    // ANIMACIONES
    // =======================
    void ActualizarAnimaciones(float inputHorizontal)
    {
        if (animator == null) return;

        if (TieneParametro("run"))
            animator.SetBool("run", !haLlegadoAMeta);

        if (TieneParametro("left") && TieneParametro("right"))
        {
            animator.SetBool("left", inputHorizontal < -0.1f);
            animator.SetBool("right", inputHorizontal > 0.1f);
        }
    }

    bool TieneParametro(string nombre)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == nombre)
                return true;
        }
        return false;
    }

    // =======================
    // COLISIONES
    // =======================
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
            GameManager.Instance?.ReiniciarNivel();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            GameManager.Instance?.ReiniciarNivel();
    }
}
