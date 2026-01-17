using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EstadoEnemigo
    {
        Patrullando,
        Persiguiendo,
        Atacando,
        Huyendo // Nuevo estado
    }

    [Header("Configuración de Patrullaje")]
    [SerializeField] private float velocidadPatrullaje = 3f;
    [SerializeField] private float limiteIzquierdo = -5f;
    [SerializeField] private float limiteDerecho = 5f;
    
    [Header("Configuración de Persecución")]
    [SerializeField] private float velocidadPersecucion = 4f;
    [SerializeField] private float distanciaPersecucion = 5f; // Distancia a la que empieza a perseguir

    [Header("Configuración de Huida")]
    [SerializeField] private float velocidadHuida = 8f; // Velocidad al huir de la luz (Rápido, efecto resorte)
    
    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private GameObject barraVidaPrefab; // Prefab de la barra de vida
    [SerializeField] private Vector3 offsetBarraVida = new Vector3(0, 1.5f, 0); // Posición de la barra sobre el enemigo
    
    [Header("Colliders")]
    [Tooltip("Este collider es del tamaño del cuerpo del enemigo y se usa para recibir daño del player")]
    [SerializeField] private Collider2D colliderCuerpo; // Collider del cuerpo (recibe daño)
    
    [Tooltip("Este collider es más grande y se usa para detectar cuando el player entra en su rango")]
    [SerializeField] private Collider2D colliderDeteccion; // Collider de detección (más grande)
    
    [Header("Detección de Player")]
    [SerializeField] private float distanciaAtaque = 2.5f; // Distancia para activar animación de ataque
    
    [Header("Orientación")]
    [SerializeField] private bool invertirOrientacion = true; // Si true, invierte la lógica del flipX
    
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    // Estado interno
    private EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;
    private int direccion = 1; // 1 = derecha, -1 = izquierda
    private float posicionInicial;
    private GameObject player;
    private bool estaAtacando = false;
    private int vidaActual;
    private GameObject barraVidaObjeto;
    private HealthBarController healthBarController;

    // Referencia de quién nos ilumina para huir de él
    private Vector3 fuenteDeLuz; 
    
    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (animator == null)
            animator = GetComponent<Animator>();
        
        // Buscar player
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        // Asegurar que el Rigidbody2D sea Kinematic
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Buscar y configurar colliders si no están asignados
        ConfigurarColliders();
        
        // Inicializar vida
        vidaActual = vidaMaxima;
        
        // Crear barra de vida
        CrearBarraVida();
        
        posicionInicial = transform.position.x;
        
        // Establecer límites relativos a la posición inicial si no están configurados
        if (limiteIzquierdo == -5f && limiteDerecho == 5f)
        {
            limiteIzquierdo = posicionInicial - 3f;
            limiteDerecho = posicionInicial + 3f;
        }
        
        // Iniciar movimiento hacia la derecha
        direccion = 1;
        estadoActual = EstadoEnemigo.Patrullando;
        ActualizarOrientacion();
    }

    void CrearBarraVida()
    {
        // Crear barra de vida si existe el prefab
        if (barraVidaPrefab != null)
        {
            barraVidaObjeto = Instantiate(barraVidaPrefab, transform.position + offsetBarraVida, Quaternion.identity);
            barraVidaObjeto.transform.SetParent(transform);
            healthBarController = barraVidaObjeto.GetComponent<HealthBarController>();
            if (healthBarController == null)
            {
                healthBarController = barraVidaObjeto.AddComponent<HealthBarController>();
            }
            healthBarController.Inicializar(vidaMaxima);
        }
        else
        {
            // Si no hay prefab, crear una barra de vida simple programáticamente
            // Por ahora, solo creamos el objeto básico
            GameObject barraVida = new GameObject("BarraVida");
            barraVida.transform.SetParent(transform);
            barraVida.transform.localPosition = offsetBarraVida;
            healthBarController = barraVida.AddComponent<HealthBarController>();
            healthBarController.Inicializar(vidaMaxima);
        }
    }
    
    void Update()
    {
        // Buscar player si no está asignado
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        // Si el enemigo está muerto, no hacer nada
        if (vidaActual <= 0)
            return;
        
        // Ejecutar comportamiento según el estado
        switch (estadoActual)
        {
            case EstadoEnemigo.Patrullando:
                Patrullar();
                break;
            case EstadoEnemigo.Persiguiendo:
                PerseguirPlayer();
                break;
            case EstadoEnemigo.Huyendo: // Nueva lógica de huida
                HuirDeLuz();
                break;
            case EstadoEnemigo.Atacando:
                // Aquí puedes añadir lógica de ataque
                break;
        }

        // Verificar si el player está cerca para activar animación de ataque
        // (Solo si no estamos huyendo)
        if (estadoActual != EstadoEnemigo.Huyendo)
        {
            VerificarPlayerCerca();
        }
        
        // Actualizar barra de vida
        ActualizarBarraVida();
    }
    
    // =======================
    // NUEVA MECÁNICA: HUIR DE LA LUZ
    // =======================
    public void EmpezarHuir(Vector3 posicionLuz)
    {
        // Solo huir si estamos vivos
        if (vidaActual > 0)
        {
            estadoActual = EstadoEnemigo.Huyendo;
            fuenteDeLuz = posicionLuz;
            // Debug.Log("¡La luz me quema! Huyendo...");
        }
    }

    public void ActualizarPosicionLuz(Vector3 posicionLuz)
    {
        fuenteDeLuz = posicionLuz;
    }

    public void DejarDeHuir()
    {
        if (vidaActual > 0 && estadoActual == EstadoEnemigo.Huyendo)
        {
            // Volver a perseguir si el player está cerca, o patrullar
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= distanciaPersecucion)
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
                Debug.Log("Luz fuera. Volviendo a perseguir.");
            }
            else
            {
                estadoActual = EstadoEnemigo.Patrullando;
                Debug.Log("Luz fuera. Volviendo a patrullar.");
            }
        }
    }

    void HuirDeLuz()
    {
        // 1. Calcular dirección básica de huida (opuesta a la luz)
        Vector2 direccionHuida = (transform.position - fuenteDeLuz).normalized;

        // 2. Añadir componente tangencial para que curve/flanquee (buscar zonas oscuras)
        // Esto crea el efecto de "intentar rodear" o salir del cono lateralmente
        Vector2 tangencial = new Vector2(-direccionHuida.y, direccionHuida.x); // Perpendicular (90 grados)
        
        // Mezclamos: Mucho de huida + un poco de lateral
        Vector2 direccionFinal = (direccionHuida + tangencial * 0.8f).normalized;

        // Orientación visual (Flip X)
        if (direccionFinal.x > 0.1f) direccion = 1;
        else if (direccionFinal.x < -0.1f) direccion = -1;
        ActualizarOrientacion();

        // Moverse usando Rigidbody2D si es posible
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            Vector2 nuevaPosicion = rb.position + direccionFinal * velocidadHuida * Time.deltaTime;
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            transform.position += (Vector3)direccionFinal * velocidadHuida * Time.deltaTime;
        }
    }

    /// <summary>
    /// Método para recibir daño del player (linterna).
    /// </summary>
    public void RecibirDano(float cantidadDano)
    {
        if (vidaActual <= 0) return; // Si ya está muerto, no recibir más daño
        
        // Usamos el daño recibido (que viene de la fuerza del player)
        // Convertimos a int asegurando redondeo correcto
        int danoInt = Mathf.RoundToInt(cantidadDano);
        
        vidaActual -= danoInt;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        
        Debug.Log($"Enemigo recibió {danoInt} de daño (Input: {cantidadDano}). Vida restante: {vidaActual}/{vidaMaxima}");
        
        // Actualizar barra de vida inmediatamente
        ActualizarBarraVida();
        
        // Si la vida llega a 0, el enemigo muere
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    /// <summary>
    /// Método para actualizar la barra de vida visualmente.
    /// </summary>
    void ActualizarBarraVida()
    {
        if (healthBarController != null)
        {
            healthBarController.ActualizarVida(vidaActual);
        }
    }
    
    /// <summary>
    /// Método que se ejecuta cuando el enemigo muere.
    /// </summary>
    void Morir()
    {
        Debug.Log("Enemigo muerto.");
        // Destruir el enemigo
        Destroy(gameObject);
    }

    // El método ActualizarEstado ha sido eliminado ya que la lógica de cambio de estado
    // ahora reside completamente en los eventos de Trigger (Entrada/Salida) para mayor precisión.
    
    void VerificarPlayerCerca()
    {
        if (player == null || animator == null) return;
        
        float distancia = Vector3.Distance(transform.position, player.transform.position);
        bool playerCerca = distancia <= distanciaAtaque;
        
        // Activar animación de ataque si el player está cerca
        if (playerCerca && !estaAtacando)
        {
            estaAtacando = true;
            // Intentar activar el parámetro "attack" si existe
            if (TieneParametro("attack"))
            {
                animator.SetTrigger("attack");
            }
            else if (TieneParametro("Attack"))
            {
                animator.SetTrigger("Attack");
            }
        }
        else if (!playerCerca && estaAtacando)
        {
            estaAtacando = false;
        }
    }
    
    bool TieneParametro(string nombreParametro)
    {
        if (animator == null) return false;
        
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == nombreParametro)
                return true;
        }
        return false;
    }
    
    void Patrullar()
    {
        // Calcular nueva posición
        float nuevaPosX = transform.position.x + (direccion * velocidadPatrullaje * Time.deltaTime);
        
        // Verificar límites y cambiar dirección
        if (nuevaPosX >= limiteDerecho)
        {
            nuevaPosX = limiteDerecho;
            direccion = -1; // Cambiar a izquierda
            ActualizarOrientacion();
        }
        else if (nuevaPosX <= limiteIzquierdo)
        {
            nuevaPosX = limiteIzquierdo;
            direccion = 1; // Cambiar a derecha
            ActualizarOrientacion();
        }
        
        // Aplicar movimiento (usar Rigidbody2D si está disponible)
        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.MovePosition(new Vector2(nuevaPosX, transform.position.y));
        }
        else
        {
            transform.position = new Vector3(nuevaPosX, transform.position.y, transform.position.z);
        }
    }

    void PerseguirPlayer()
    {
        if (player == null) return;

        // Calcular dirección hacia el player
        Vector3 direccionAlPlayer = (player.transform.position - transform.position).normalized;
        
        // Determinar dirección horizontal para la orientación
        if (direccionAlPlayer.x > 0.1f)
            direccion = 1;
        else if (direccionAlPlayer.x < -0.1f)
            direccion = -1;

        ActualizarOrientacion();

        // Calcular nueva posición moviéndose hacia el player
        Vector2 nuevaPosicion = Vector2.MoveTowards(
            transform.position,
            player.transform.position,
            velocidadPersecucion * Time.deltaTime
        );

        // Aplicar movimiento
        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
        {
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            transform.position = new Vector3(nuevaPosicion.x, nuevaPosicion.y, transform.position.z);
        }
    }
    
    void ActualizarOrientacion()
    {
        if (spriteRenderer != null)
        {
            // Lógica de orientación: 
            // Si invertirOrientacion = true: voltear cuando va a la derecha (direccion > 0)
            // Si invertirOrientacion = false: voltear cuando va a la izquierda (direccion < 0)
            if (invertirOrientacion)
            {
                spriteRenderer.flipX = (direccion > 0); // Voltear cuando va a la derecha
            }
            else
            {
                spriteRenderer.flipX = (direccion < 0); // Voltear cuando va a la izquierda (comportamiento normal)
            }
        }
        
        // También actualizar animación de dirección si existe
        if (animator != null)
        {
            // Intentar actualizar parámetros de dirección si existen
            if (TieneParametro("walkLeft") || TieneParametro("walkRight"))
            {
                if (TieneParametro("walkLeft"))
                    animator.SetBool("walkLeft", direccion < 0);
                if (TieneParametro("walkRight"))
                    animator.SetBool("walkRight", direccion > 0);
            }
        }
    }
    
    /// <summary>
    /// Configura los colliders del enemigo.
    /// colliderCuerpo: recibe daño del player (linterna)
    /// colliderDeteccion: detecta cuando el player entra en su rango
    /// </summary>
    void ConfigurarColliders()
    {
        if (colliderCuerpo == null || colliderDeteccion == null)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            
            foreach (Collider2D col in colliders)
            {
                // El collider del cuerpo debe ser más pequeño y no ser trigger
                // El collider de detección debe ser más grande y ser trigger
                // Buscar collider del cuerpo: no trigger, puede estar en el mismo objeto o en un hijo
                if (colliderCuerpo == null && !col.isTrigger)
                {
                    if (col.gameObject == gameObject || (col.transform.parent != null && col.transform.parent == transform))
                    {
                        colliderCuerpo = col;
                    }
                }
                // Buscar collider de detección: sí trigger, puede estar en el mismo objeto o en un hijo
                else if (colliderDeteccion == null && col.isTrigger)
                {
                    if (col.gameObject == gameObject || (col.transform.parent != null && col.transform.parent == transform))
                    {
                        colliderDeteccion = col;
                    }
                }
            }
            
            // Si no se encontraron colliders hijos, buscar en el mismo objeto
            if (colliderCuerpo == null || colliderDeteccion == null)
            {
                Collider2D[] collidersSelf = GetComponents<Collider2D>();
                
                // Asignar: el más pequeño es el cuerpo, el más grande es la detección
                if (collidersSelf.Length >= 2)
                {
                    float tamanioCollider1 = GetTamanioCollider(collidersSelf[0]);
                    float tamanioCollider2 = GetTamanioCollider(collidersSelf[1]);
                    
                    if (tamanioCollider1 < tamanioCollider2)
                    {
                        colliderCuerpo = collidersSelf[0];
                        colliderDeteccion = collidersSelf[1];
                    }
                    else
                    {
                        colliderCuerpo = collidersSelf[1];
                        colliderDeteccion = collidersSelf[0];
                    }
                }
                else if (collidersSelf.Length == 1)
                {
                    colliderCuerpo = collidersSelf[0];
                    colliderCuerpo.isTrigger = false; // Asegurar que no sea trigger
                }
            }
            
            // Configurar colliders correctamente
            if (colliderCuerpo != null)
            {
                colliderCuerpo.isTrigger = false; // El collider del cuerpo NO es trigger (para OnCollisionEnter2D)
            }
            
            if (colliderDeteccion != null)
            {
                colliderDeteccion.isTrigger = true; // El collider de detección SÍ es trigger (para OnTriggerEnter2D)
            }
        }
    }
    
    /// <summary>
    /// Obtiene el tamaño aproximado de un collider para comparar.
    /// </summary>
    float GetTamanioCollider(Collider2D col)
    {
        if (col is BoxCollider2D box)
        {
            return box.size.x * box.size.y;
        }
        else if (col is CircleCollider2D circle)
        {
            return circle.radius * circle.radius * Mathf.PI;
        }
        else if (col is CapsuleCollider2D capsule)
        {
            return capsule.size.x * capsule.size.y;
        }
        return 1f;
    }
    
    public void SetLimites(float izquierdo, float derecho)
    {
        limiteIzquierdo = izquierdo;
        limiteDerecho = derecho;
    }
    
    /// <summary>
    /// Método público llamado por EnemyDetectionZone cuando el player entra en el rango de detección.
    /// </summary>
    public void PlayerEntroEnRango()
    {
        if (estadoActual == EstadoEnemigo.Patrullando)
        {
            estadoActual = EstadoEnemigo.Persiguiendo;
            Debug.Log("Enemigo detectó al player. Cambiando a modo Persiguiendo.");
        }
    }
    
    /// <summary>
    /// Método público llamado por EnemyDetectionZone cuando el player sale del rango de detección.
    /// </summary>
    public void PlayerSalioDelRango()
    {
        if (estadoActual == EstadoEnemigo.Persiguiendo && player != null)
        {
            // Volver a patrullar inmediatamente cuando sale del rango
            estadoActual = EstadoEnemigo.Patrullando;
            Debug.Log("Player salió del rango. Enemigo vuelve a patrullar.");
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando algo sale del Trigger de detección (Circle Collider).
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Si el player sale del trigger, dejar de perseguir
            if (estadoActual == EstadoEnemigo.Persiguiendo)
            {
                estadoActual = EstadoEnemigo.Patrullando;
                Debug.Log("DETECCIÓN: Player salió del rango de visión (Circle Collider).");
            }
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando el collider del cuerpo del enemigo (NO trigger) entra en contacto físico con el player.
    /// AQUÍ ES DONDE EL PLAYER RECIBE DAÑO (Capsule Collider del enemigo choca con Player).
    /// </summary>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // El collider del cuerpo del enemigo (Capsule) tocó al player
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // El player pierde una vida
                playerController.RecibirDano();
                Debug.Log("COLISIÓN FÍSICA: Enemy (Cuerpo) tocó al Player. Player pierde 1 vida.");
            }
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando algo entra en el Trigger de detección (Circle Collider).
    /// AQUÍ SOLO SE DETECTA AL PLAYER PARA PERSEGUIRLO, NO HACE DAÑO.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Si el player entra en el área de detección (Circle Collider que es Trigger)
        if (other.CompareTag("Player"))
        {
            // Solo cambiamos a estado de persecución
            if (estadoActual == EstadoEnemigo.Patrullando)
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
                Debug.Log("DETECCIÓN: Player entró en rango de visión (Circle Collider).");
            }
        }
        // NOTA: La linterna también usa triggers, pero su lógica está en LinternaController.cs,
        // así que no necesitamos hacer nada aquí para recibir daño de la linterna.
    }
}