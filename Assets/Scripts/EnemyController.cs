using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EstadoEnemigo
    {
        Patrullando,
        Persiguiendo,
        Atacando,
        Huyendo,
        Retrocediendo // Estado para cuando golpea al player
    }

    [Header("Configuración de Patrullaje")]
    [SerializeField] private float velocidadPatrullaje = 3f;
    [SerializeField] private float limiteIzquierdo = -5f;
    [SerializeField] private float limiteDerecho = 5f;
    
    [Header("Configuración de Persecución")]
    [SerializeField] private float velocidadPersecucion = 4f;
    [SerializeField] private float distanciaPersecucion = 5f; // Distancia a la que empieza a perseguir

    [Header("Configuración de Huida")]
    [SerializeField] private float velocidadHuida = 8f; // Velocidad al huir de la luz
    
    [Header("Configuración de Impacto (Retroceso)")]
    [SerializeField] private float tiempoRetroceso = 0.5f; // Tiempo que tarda en recuperarse tras golpear
    [SerializeField] private float velocidadRetroceso = 3f;
    private float tiempoRetrocesoRestante = 0f;

    [Header("Sistema de Vida")]
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private float tiempoEntreDanios = 0.5f; // Evita morir al instante por la linterna
    [SerializeField] private GameObject barraVidaPrefab; 
    [SerializeField] private Vector3 offsetBarraVida = new Vector3(0, 1.5f, 0); 
    
    [Header("Colliders")]
    [Tooltip("Este collider es del tamaño del cuerpo del enemigo y se usa para recibir daño del player")]
    [SerializeField] private Collider2D colliderCuerpo; 
    
    [Tooltip("Este collider es más grande y se usa para detectar cuando el player entra en su rango")]
    [SerializeField] private Collider2D colliderDeteccion; 
    
    [Header("Detección de Player")]
    [SerializeField] private float distanciaAtaque = 2.5f; 
    
    [Header("Orientación")]
    [SerializeField] private bool invertirOrientacion = true; 
    
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    // Estado interno
    private EstadoEnemigo estadoActual = EstadoEnemigo.Patrullando;
    private int direccion = 1; 
    private float posicionInicial;
    private GameObject player;
    private bool estaAtacando = false;
    private int vidaActual;
    private GameObject barraVidaObjeto;
    private HealthBarController healthBarController;
    private float ultimoTiempoDano = 0f; // Cooldown de daño

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
        Debug.Log($"<color=orange>---> VIDA ENEMIGO INICIAL: {vidaMaxima} <---</color>");
        
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
            case EstadoEnemigo.Retrocediendo:
                ManejarRetroceso();
                break;
            case EstadoEnemigo.Atacando:
                // Lógica de ataque
                break;
        }

        // Verificar si el player está cerca para activar animación de ataque
        // (Solo si no estamos huyendo ni retrocediendo)
        if (estadoActual != EstadoEnemigo.Huyendo && estadoActual != EstadoEnemigo.Retrocediendo)
        {
            VerificarPlayerCerca();
        }
        
        // Actualizar barra de vida
        ActualizarBarraVida();
    }
    
    // =======================
    // NUEVA MECÁNICA: HUIR DE LA LUZ POR TIEMPO LIMITADO (2 SEGUNDOS)
    // =======================
    [SerializeField] private float tiempoMaximoHuida = 2.0f; // Cuánto tiempo huye antes de volver a atacar
    private float tiempoHuyendoActual = 0f;

    public void EmpezarHuir(Vector3 posicionLuz)
    {
        // Solo huir si estamos vivos y no estamos retrocediendo por golpe
        // Y SOLO si no estamos ya huyendo (para no resetear el contador constantemente si seguimos iluminados)
        if (vidaActual > 0 && estadoActual != EstadoEnemigo.Retrocediendo && estadoActual != EstadoEnemigo.Huyendo)
        {
            estadoActual = EstadoEnemigo.Huyendo;
            fuenteDeLuz = posicionLuz;
            tiempoHuyendoActual = 0f; // Reseteamos el contador al empezar
             Debug.Log("¡Luz fuerte! Huyendo 2 segundos...");
        }
        else if (estadoActual == EstadoEnemigo.Huyendo)
        {
             // Si ya estamos huyendo, actualizamos la fuente pero NO reseteamos el tiempo
             // para que no huya infinitamente mientras le sigamos apuntando.
             fuenteDeLuz = posicionLuz;
        }
    }

    public void ActualizarPosicionLuz(Vector3 posicionLuz)
    {
        fuenteDeLuz = posicionLuz;
    }

    public void DejarDeHuir()
    {
        // Este método se llamaba cuando salía del trigger, pero ahora queremos que el tiempo mande.
        // Lo dejamos por si queremos forzar el parado, pero la lógica principal estará en HuirDeLuz()
    }

    void HuirDeLuz()
    {
        // Contar tiempo
        tiempoHuyendoActual += Time.deltaTime;

        // Si ya hemos huido suficiente tiempo (2 seg), paramos OBLIGATORIAMENTE
        if (tiempoHuyendoActual >= tiempoMaximoHuida)
        {
            Debug.Log("Ya he huido suficiente. ¡Vuelvo al ataque!");
            // Determinar si volvemos a perseguir o patrullar (según si el player está cerca)
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= distanciaPersecucion)
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
            }
            else
            {
                estadoActual = EstadoEnemigo.Patrullando;
            }
            return; // Salimos de la función
        }

        // Lógica de movimiento de huida
        // 1. Calcular dirección básica de huida (opuesta a la luz)
        Vector2 direccionHuida = (transform.position - fuenteDeLuz).normalized;

        // 2. Añadir componente tangencial para que curve/flanquee
        Vector2 tangencial = new Vector2(-direccionHuida.y, direccionHuida.x); 
        
        // Mezclamos: Mucho de huida + un poco de lateral
        Vector2 direccionFinal = (direccionHuida + tangencial * 0.8f).normalized;

        // Orientación visual (Flip X)
        if (direccionFinal.x > 0.1f) direccion = 1;
        else if (direccionFinal.x < -0.1f) direccion = -1;
        ActualizarOrientacion();

        // Moverse
        MoverEnemigo(direccionFinal, velocidadHuida);
    }

    void ManejarRetroceso()
    {
        tiempoRetrocesoRestante -= Time.deltaTime;
        
        if (player != null)
        {
            // Moverse en dirección contraria al player
            Vector3 direccionRetroceso = (transform.position - player.transform.position).normalized;
            MoverEnemigo(direccionRetroceso, velocidadRetroceso);
        }

        if (tiempoRetrocesoRestante <= 0)
        {
            // Fin del retroceso, volver a perseguir
            estadoActual = EstadoEnemigo.Persiguiendo;
        }
    }

    void MoverEnemigo(Vector2 direccion, float velocidad)
    {
        if (rb != null && rb.bodyType != RigidbodyType2D.Static)
        {
            Vector2 nuevaPosicion = rb.position + direccion * velocidad * Time.deltaTime;
            rb.MovePosition(nuevaPosicion);
        }
        else
        {
            transform.position += (Vector3)direccion * velocidad * Time.deltaTime;
        }
    }

    /// <summary>
    /// Método para recibir daño del player (linterna).
    /// </summary>
    public void RecibirDano(float cantidadDano)
    {
        if (vidaActual <= 0) return; // Si ya está muerto, no recibir más daño

        // NOTA: El cooldown lo controla el atacante (LinternaController), aquí recibimos todo el daño que llegue.
        
        int danoInt = Mathf.RoundToInt(cantidadDano);
        
        vidaActual -= danoInt;
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        
        Debug.Log($"<color=red>¡GOLPE! Enemigo recibió -{danoInt} de daño. Vida restante: {vidaActual}/{vidaMaxima}</color>");
        
        ActualizarBarraVida();
        
        if (vidaActual <= 0)
        {
            Morir();
        }
    }
    
    // ... [ActualizarBarraVida se mantiene igual] ...
    void ActualizarBarraVida()
    {
        if (healthBarController != null)
        {
            healthBarController.ActualizarVida(vidaActual);
        }
    }
    
    // ... [Morir se mantiene igual] ...
    void Morir()
    {
        Debug.Log("Enemigo muerto.");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EliminarEnemigo();
        }
        
        Destroy(gameObject);
    }

    void VerificarPlayerCerca()
    {
        if (player == null || animator == null) return;
        
        float distancia = Vector3.Distance(transform.position, player.transform.position);
        bool playerCerca = distancia <= distanciaAtaque;
        
        // Activar animación de ataque si el player está cerca
        if (playerCerca && !estaAtacando)
        {
            estaAtacando = true;
            if (TieneParametro("attack")) animator.SetTrigger("attack");
            else if (TieneParametro("Attack")) animator.SetTrigger("Attack");
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
            if (param.name == nombreParametro) return true;
        }
        return false;
    }
    
    void Patrullar()
    {
        float nuevaPosX = transform.position.x + (direccion * velocidadPatrullaje * Time.deltaTime);
        
        if (nuevaPosX >= limiteDerecho)
        {
            nuevaPosX = limiteDerecho;
            direccion = -1; 
            ActualizarOrientacion();
        }
        else if (nuevaPosX <= limiteIzquierdo)
        {
            nuevaPosX = limiteIzquierdo;
            direccion = 1; 
            ActualizarOrientacion();
        }
        
        if (rb != null && rb.bodyType == RigidbodyType2D.Kinematic)
            rb.MovePosition(new Vector2(nuevaPosX, transform.position.y));
        else
            transform.position = new Vector3(nuevaPosX, transform.position.y, transform.position.z);
    }

    void PerseguirPlayer()
    {
        if (player == null) return;

        Vector3 direccionAlPlayer = (player.transform.position - transform.position).normalized;
        
        if (direccionAlPlayer.x > 0.1f) direccion = 1;
        else if (direccionAlPlayer.x < -0.1f) direccion = -1;

        ActualizarOrientacion();

        MoverEnemigo(direccionAlPlayer, velocidadPersecucion);
    }
    
    void ActualizarOrientacion()
    {
        if (spriteRenderer != null)
        {
            if (invertirOrientacion) spriteRenderer.flipX = (direccion > 0);
            else spriteRenderer.flipX = (direccion < 0);
        }
        
        if (animator != null)
        {
            if (TieneParametro("walkLeft") || TieneParametro("walkRight"))
            {
                if (TieneParametro("walkLeft")) animator.SetBool("walkLeft", direccion < 0);
                if (TieneParametro("walkRight")) animator.SetBool("walkRight", direccion > 0);
            }
        }
    }
    
    // ... [ConfigurarColliders se mantiene igual] ...
    void ConfigurarColliders()
    {
        if (colliderCuerpo == null || colliderDeteccion == null)
        {
            Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
            
            foreach (Collider2D col in colliders)
            {
                if (colliderCuerpo == null && !col.isTrigger)
                {
                    if (col.gameObject == gameObject || (col.transform.parent != null && col.transform.parent == transform))
                        colliderCuerpo = col;
                }
                else if (colliderDeteccion == null && col.isTrigger)
                {
                    if (col.gameObject == gameObject || (col.transform.parent != null && col.transform.parent == transform))
                        colliderDeteccion = col;
                }
            }
            // Fallback lógica previa...
            if (colliderCuerpo == null || colliderDeteccion == null)
            {
                 Collider2D[] collidersSelf = GetComponents<Collider2D>();
                 if (collidersSelf.Length >= 2)
                 {
                     colliderCuerpo = collidersSelf[0];
                     colliderDeteccion = collidersSelf[1];
                     colliderCuerpo.isTrigger = false;
                     colliderDeteccion.isTrigger = true;
                 }
                 else if (collidersSelf.Length == 1)
                 {
                     colliderCuerpo = collidersSelf[0];
                     colliderCuerpo.isTrigger = false;
                 }
            }
        }
    }

    // ... [GetTamanioCollider se mantiene igual (omitido por brevedad en tool pero asumido intacto en archivo real si no se reemplaza)] ... 
    // Nota: El replace tool debe incluir todo el archivo o los trozos que cambio. Voy a incluir todo lo necesario.

    float GetTamanioCollider(Collider2D col)
    {
        if (col is BoxCollider2D box) return box.size.x * box.size.y;
        else if (col is CircleCollider2D circle) return circle.radius * circle.radius * Mathf.PI;
        else if (col is CapsuleCollider2D capsule) return capsule.size.x * capsule.size.y;
        return 1f;
    }
    
    public void SetLimites(float izquierdo, float derecho)
    {
        limiteIzquierdo = izquierdo;
        limiteDerecho = derecho;
    }
    
    public void PlayerEntroEnRango()
    {
        if (estadoActual == EstadoEnemigo.Patrullando)
        {
            estadoActual = EstadoEnemigo.Persiguiendo;
            Debug.Log("Enemigo detectó al player. Cambiando a modo Persiguiendo.");
        }
    }
    
    public void PlayerSalioDelRango()
    {
        if (estadoActual == EstadoEnemigo.Persiguiendo && player != null)
        {
            estadoActual = EstadoEnemigo.Patrullando;
            Debug.Log("Player salió del rango. Enemigo vuelve a patrullar.");
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (estadoActual == EstadoEnemigo.Persiguiendo)
            {
                estadoActual = EstadoEnemigo.Patrullando;
                Debug.Log("DETECCIÓN: Player salió del rango de visión.");
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.RecibirDano();
                Debug.Log("COLISIÓN: Enemy tocó al Player --> INICIANDO RETROCESO.");
                
                // INICIAR RETROCESO
                estadoActual = EstadoEnemigo.Retrocediendo;
                tiempoRetrocesoRestante = tiempoRetroceso;
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (estadoActual == EstadoEnemigo.Patrullando)
            {
                estadoActual = EstadoEnemigo.Persiguiendo;
                Debug.Log("DETECCIÓN: Player entró en rango de visión.");
            }
        }
    }
}