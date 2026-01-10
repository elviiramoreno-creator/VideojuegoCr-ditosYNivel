using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Velocidad")]
    [SerializeField] private float velocidadMovimiento = 5f; // Velocidad constante del player

    [Header("Multiplicadores por tilemap")]
    [Tooltip("Estos multiplicadores modifican la velocidad del player según el tipo de suelo que está pisando. " +
             "multiplicadorHielo: Aumenta la velocidad cuando pisa hielo (valor > 1 = más rápido). " +
             "multiplicadorEnredadera: Reduce la velocidad cuando pisa enredaderas (valor < 1 = más lento).")]
    [SerializeField] private float multiplicadorHielo = 1.4f;
    [SerializeField] private float multiplicadorEnredadera = 0.5f;

    [Header("Sistema de Vidas")]
    [SerializeField] private int vidasMaximas = 3;
    [SerializeField] private float tiempoInvencibilidad = 1f; // Tiempo en el que el player no puede recibir daño tras ser golpeado

    [Header("Referencias")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private GameObject linterna; // Referencia al objeto de la linterna

    // Estado interno
    private bool haLlegadoAMeta = false;
    private string tilemapActual = "Suelo";
    private int vidasActuales;
    private bool esInvencible = false;
    private float tiempoInvencibilidadRestante = 0f;

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        // Buscar linterna si no está asignada
        if (linterna == null)
        {
            // Buscar por nombre
            Transform linternaTransform = transform.Find("Linterna");
            if (linternaTransform != null)
                linterna = linternaTransform.gameObject;
        }

        // Configuración Rigidbody2D
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Inicializar vidas
        vidasActuales = vidasMaximas;
    }

    void Update()
    {
        if (haLlegadoAMeta)
        {
            ActualizarAnimaciones(0f, 0f);
            return;
        }

        // Actualizar tiempo de invencibilidad
        if (esInvencible)
        {
            tiempoInvencibilidadRestante -= Time.deltaTime;
            if (tiempoInvencibilidadRestante <= 0f)
            {
                esInvencible = false;
            }
        }

        // Movimiento WASD con Input.GetAxis (suavizado) para movimiento fluido
        float inputHorizontal = Input.GetAxis("Horizontal"); // W-A-S-D o flechas
        float inputVertical = Input.GetAxis("Vertical");

        // Normalizar el vector de movimiento para que la velocidad diagonal no sea mayor
        Vector2 direccionMovimiento = new Vector2(inputHorizontal, inputVertical).normalized;

        // Multiplicador según tilemap (aplicar a ambas direcciones si es necesario)
        float multiplicadorVelocidad = 1f;
        if (tilemapActual == "Hielo")
            multiplicadorVelocidad = multiplicadorHielo;
        else if (tilemapActual == "Enredadera")
            multiplicadorVelocidad = multiplicadorEnredadera;

        // Calcular velocidad final (siempre constante)
        // La velocidad base se mantiene constante, pero se puede modificar según el tilemap
        Vector2 velocidadFinal = direccionMovimiento * velocidadMovimiento * multiplicadorVelocidad;
        
        // Asegurar que la velocidad siempre sea constante (normalizar si es necesario)
        // La normalización del direccionMovimiento ya garantiza velocidad constante en todas las direcciones

        // Calcular nueva posición
        Vector2 nuevaPosicion = rb.position + velocidadFinal * Time.deltaTime;

        // Aplicar movimiento usando Rigidbody2D para movimiento fluido
        rb.MovePosition(nuevaPosicion);

        ActualizarAnimaciones(inputHorizontal, inputVertical);
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
    void ActualizarAnimaciones(float inputHorizontal, float inputVertical)
    {
        if (animator == null) return;

        float velocidadMagnitud = Mathf.Abs(inputHorizontal) + Mathf.Abs(inputVertical);
        bool estaMoviendo = velocidadMagnitud > 0.1f;

        if (TieneParametro("run"))
            animator.SetBool("run", estaMoviendo && !haLlegadoAMeta);

        if (TieneParametro("left") && TieneParametro("right"))
        {
            animator.SetBool("left", inputHorizontal < -0.1f);
            animator.SetBool("right", inputHorizontal > 0.1f);
        }

        // Si el animator tiene parámetros de movimiento vertical
        if (TieneParametro("MovimientoX"))
            animator.SetFloat("MovimientoX", inputHorizontal);
        
        if (TieneParametro("MovimientoY"))
            animator.SetFloat("MovimientoY", inputVertical);
        
        if (TieneParametro("Velocidad"))
            animator.SetFloat("Velocidad", velocidadMagnitud);
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
    // SISTEMA DE VIDAS
    // =======================
    public void RecibirDano()
    {
        // Si el player es invencible o ya está muerto, no recibir daño
        if (esInvencible || vidasActuales <= 0)
            return;

        vidasActuales--;
        Debug.Log($"Player recibió daño! Vidas restantes: {vidasActuales}/{vidasMaximas}");

        // Activar invencibilidad temporal
        esInvencible = true;
        tiempoInvencibilidadRestante = tiempoInvencibilidad;

        // Si el player se queda sin vidas, reiniciar el nivel
        if (vidasActuales <= 0)
        {
            Debug.Log("Player muerto. Reiniciando nivel...");
            Morir();
        }
    }

    void Morir()
    {
        vidasActuales = 0;
        GameManager.Instance?.ReiniciarNivel();
    }

    public int GetVidasActuales()
    {
        return vidasActuales;
    }

    public int GetVidasMaximas()
    {
        return vidasMaximas;
    }

    public bool EsInvencible()
    {
        return esInvencible;
    }

    // =======================
    // COLISIONES
    // =======================
    /// <summary>
    /// Se ejecuta cuando el collider del player entra en contacto físico con el collider del cuerpo del enemigo (NO trigger).
    /// El player pierde 1 vida cada vez que el enemigo lo toca.
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Verificar que sea el collider del cuerpo del enemigo (no el de detección)
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // El player recibe daño cuando el enemigo lo toca
                RecibirDano();
            }
        }
    }

    /// <summary>
    /// Se ejecuta cuando el collider del player (si es trigger) entra en contacto con el collider del cuerpo del enemigo.
    /// IMPORTANTE: Solo debe activarse con el collider del CUERPO del enemigo, no con el de detección.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Verificar que sea el collider del cuerpo del enemigo (no el de detección)
            // El collider de detección tiene el script EnemyDetectionZone, así que lo filtramos
            EnemyDetectionZone detectionZone = other.GetComponent<EnemyDetectionZone>();
            if (detectionZone == null) // Si NO tiene EnemyDetectionZone, es el collider del cuerpo
            {
                EnemyController enemy = other.GetComponent<EnemyController>();
                if (enemy == null)
                {
                    enemy = other.GetComponentInParent<EnemyController>();
                }
                
                if (enemy != null)
                {
                    // El player recibe daño cuando el enemigo lo toca
                    RecibirDano();
                }
            }
        }
    }
}
