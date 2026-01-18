using UnityEngine;

public class PlayerControllerBe : MonoBehaviour
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
    [SerializeField] private Collider2D playerCollider;

    // Estado interno
    private float velocidadActual;
    private bool haLlegadoAMeta = false;
    private string tilemapActual = "Suelo";
    private Vector3 posicionAnterior;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (playerCollider == null)
            playerCollider = GetComponent<Collider2D>();

        // Configuración Rigidbody2D - IMPORTANTE para colisiones con tilemaps
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
       
        // PARA COLISIONES CON TILEMAPS: Usar Dynamic o Kinematic con Continuous detection
        rb.bodyType = RigidbodyType2D.Dynamic; // Dynamic permite colisiones automáticas
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Para mejor detección
       
        // Asegurar que el collider esté configurado correctamente para colisiones
        if (playerCollider != null)
        {
            playerCollider.isTrigger = false; // Debe ser NO trigger para colisiones físicas
        }
       
        // Crear un collider adicional como trigger SOLO para detectar enemigos
        Collider2D[] colliders = GetComponents<Collider2D>();
        bool tieneTriggerParaEnemigos = false;
        foreach (Collider2D col in colliders)
        {
            if (col.isTrigger && col != playerCollider)
            {
                tieneTriggerParaEnemigos = true;
                break;
            }
        }
       
        if (!tieneTriggerParaEnemigos)
        {
            CircleCollider2D enemyTrigger = gameObject.AddComponent<CircleCollider2D>();
            enemyTrigger.isTrigger = true;
            enemyTrigger.radius = 0.6f;
        }

        velocidadActual = velocidadInicial;
        posicionAnterior = transform.position;
    }

    void Update()
    {
        if (haLlegadoAMeta)
        {
            ActualizarAnimaciones(0f);
            return;
        }

        // Guardar posición anterior antes de calcular movimiento
        posicionAnterior = transform.position;

        // Incremento progresivo de velocidad
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
        float velocidadHorizontalFinal = velocidadActual * multiplicadorHorizontal * inputHorizontal;

        // Aplicar movimiento usando velocity (mejor para colisiones con Dynamic)
        if (rb != null)
        {
            // Usar velocity para que Unity maneje las colisiones automáticamente
            rb.linearVelocity = new Vector2(velocidadHorizontalFinal, velocidadVerticalFinal);
        }

        ActualizarAnimaciones(inputHorizontal);
    }

    // =======================
    // TILEMAP DETECTOR
    // =======================
    public void SetTilemapActual(string nombreTilemap)
    {
        tilemapActual = nombreTilemap;
    }

    // =======================
    // META
    // =======================
    public void LlegarAMeta()
    {
        if (haLlegadoAMeta) return;
       
        haLlegadoAMeta = true;
        velocidadActual = 0f;
       
        // Detener completamente el movimiento
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        // Detener animaciones
        if (animator != null)
        {
            if (TieneParametro("run"))
                animator.SetBool("run", false);
            if (TieneParametro("left"))
                animator.SetBool("left", false);
            if (TieneParametro("right"))
                animator.SetBool("right", false);
        }
    }

    // =======================
    // ANIMACIONES
    // =======================
    void ActualizarAnimaciones(float inputHorizontal)
    {
        if (animator == null) return;

        if (TieneParametro("run"))
            animator.SetBool("run", !haLlegadoAMeta);

        // Actualizar animaciones left y right independientemente
        if (TieneParametro("left"))
        {
            animator.SetBool("left", inputHorizontal < -0.1f);
        }
       
        if (TieneParametro("right"))
        {
            animator.SetBool("right", inputHorizontal > 0.1f);
        }
       
        // Si ambos parámetros existen, asegurar que solo uno esté activo
        if (TieneParametro("left") && TieneParametro("right"))
        {
            bool left = inputHorizontal < -0.1f;
            bool right = inputHorizontal > 0.1f;
            animator.SetBool("left", left);
            animator.SetBool("right", right);
        }
    }

    bool TieneParametro(string nombre)
    {
        if (animator == null) return false;
       
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == nombre)
                return true;
        }
        return false;
    }

    // =======================
    // COLISIONES CON TILEMAP
    // =======================
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Para detectar colisiones con tilemaps (no triggers)
        if (collision.gameObject.CompareTag("TilemapBordes") ||
            collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            // El jugador choca con un tilemap sólido
            Debug.Log("Colisión con tilemap detectada");
        }
    }

    // =======================
    // COLISIONES CON ENEMIGOS (Triggers)
    // =======================
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance?.GameOver();
        }
    }
   
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance?.GameOver();
        }
    }
}
