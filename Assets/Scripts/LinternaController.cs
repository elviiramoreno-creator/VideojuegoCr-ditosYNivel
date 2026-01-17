using UnityEngine;

/// <summary>
/// Script para controlar la linterna que sigue el ratón y actúa como arma.
/// La linterna debe tener un Light2D (Spot Light) y un Collider2D configurado como Trigger.
/// </summary>
public class LinternaController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform player; // Referencia al player
    [SerializeField] private Camera mainCamera; // Cámara para convertir posición del mouse a mundo

    [Header("Configuración")]
    [SerializeField] private float velocidadRotacion = 10f; // Velocidad de rotación suave
    [SerializeField] private Vector3 offsetDesdePlayer = Vector3.zero; // Offset de posición relativo al player

    private Vector3 posicionMouse;
    private Vector3 direccionMouse;
    private float angulo;

    [Header("Ajustes de Rotación")]
    [Tooltip("Ajuste de ángulo si el sprite no apunta a la derecha por defecto. Si el sprite 'mira' hacia arriba, usa -90. Si mira a la derecha, usa 0.")]
    [SerializeField] private float rotationOffset = -90f;

    [Header("Configuración de Arma")]
    [Tooltip("Si es true, este objeto hace daño a los enemigos. Desactívalo para luces decorativas como 'alumbrado personaje'.")]
    [SerializeField] private bool esArmaLetal = true;

    void Start()
    {
        // Auto-detectar si NO es la linterna (por ejemplo, "alumbrado personaje")
        // Si el nombre contiene "alumbrado", desactivamos el daño automáticamente
        if (gameObject.name.ToLower().Contains("alumbrado"))
        {
            esArmaLetal = false;
        }

        // Buscar player si no está asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Si la linterna es hijo del player, usar la posición del parent
        if (player != null && transform.parent == player)
        {
            // La posición se mantiene relativa al parent, solo rotamos
        }

        // Buscar cámara principal si no está asignada
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();
        }
    }

    void LateUpdate()
    {
        if (player == null || mainCamera == null) return;

        // Si la linterna NO es hijo del player, posicionarla sobre el player
        if (transform.parent != player)
        {
            transform.position = player.position + offsetDesdePlayer;
        }

        // Obtener posición del mouse en el mundo
        posicionMouse = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        posicionMouse.z = 0f; // Mantener Z = 0 para 2D

        // Calcular dirección desde el player hacia el mouse
        direccionMouse = (posicionMouse - player.position).normalized;

        // Calcular ángulo en grados
        angulo = Mathf.Atan2(direccionMouse.y, direccionMouse.x) * Mathf.Rad2Deg;

        // Rotar la linterna suavemente hacia el mouse (SOLO rotación, sin mover posición)
        // Aplicamos el offset para corregir la orientación del sprite
        Quaternion rotacionDeseada = Quaternion.AngleAxis(angulo + rotationOffset, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
    }

    [Header("Sistema de Daño")]
    [Tooltip("Intervalo más pequeño = daño más frecuente. 1.0f = daño cada segundo.")]
    [SerializeField] private float intervaloDanos = 1.0f; // Intervalo entre cada aplicación de daño
    
    private float tiempoUltimoDano = 0f;
    private EnemyController enemigoActual = null;
    
    // =======================
    // DETECCIÓN DE ENEMIGOS (ARMA)
    // =======================
    /// <summary>
    /// Se ejecuta cuando un enemigo entra en el collider de la linterna.
    /// La linterna debe tener un Collider2D configurado como Trigger.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Si no es un arma letal (ej. alumbrado decorativo), no hacemos nada
        if (!esArmaLetal) return;
        
        // IMPORTANTE: Ignorar colliders que sean triggers (como el área de visión del enemigo)
        // Solo queremos dañar si tocamos el CUERPO FÍSICO del enemigo.
        if (other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            // El collider de la linterna está en contacto con un enemigo
            // Buscar EnemyController en el GameObject o en el padre (por si el collider está en un hijo)
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyController>();
            }
            
            if (enemy != null)
            {
                enemigoActual = enemy;
                tiempoUltimoDano = Time.time;
                
                // Hacer que el enemigo huya de la luz
                enemy.EmpezarHuir(transform.position);

                // Aplicar daño del golpe
                int dano = ObtenerFuerzaPlayer();
                enemy.RecibirDano(dano);
                // Debug.Log($"Linterna iluminó enemigo. Enemigo perdió {dano} de vida.");
            }
        }
    }

    /// <summary>
    /// Se ejecuta mientras un enemigo está dentro del collider de la linterna.
    /// Aplica daño continuo (por intervalos) mientras el enemigo está iluminado.
    /// </summary>
    void OnTriggerStay2D(Collider2D other)
    {
        // Si no es un arma letal (ej. alumbrado decorativo), no hacemos nada
        if (!esArmaLetal) return;
        
        // IMPORTANTE: Ignorar colliders que sean triggers
        if (other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            // Buscar EnemyController en el GameObject o en el padre
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyController>();
            }
            
            if (enemy != null)
            {
                // Actualizar la posición de la luz para que huya
                enemy.ActualizarPosicionLuz(transform.position);

                // Si cambiamos de enemigo o es el mismo
                if (enemy != enemigoActual)
                {
                   enemigoActual = enemy;
                   tiempoUltimoDano = Time.time;
                   
                   // Al cambiar, aseguramos que huya
                   enemy.EmpezarHuir(transform.position);

                   // Aplicar primer daño al cambiar de enemigo
                   int dano = ObtenerFuerzaPlayer();
                   enemy.RecibirDano(dano);
                }
                else
                {
                    // Es el mismo enemigo, comprobar intervalo
                    if (Time.time - tiempoUltimoDano >= intervaloDanos)
                    {
                        int dano = ObtenerFuerzaPlayer();
                        enemy.RecibirDano(dano);
                        tiempoUltimoDano = Time.time;
                        // Debug.Log($"Linterna continua iluminando enemigo. Enemigo perdió {dano} de vida.");
                    }
                }
            }
        }
    }

    int ObtenerFuerzaPlayer()
    {
        if (player != null)
        {
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
                return pc.fuerzaGolpe;
        }
        return 15; // Valor por defecto si no se encuentra
    }
    
    /// <summary>
    /// Se ejecuta cuando un enemigo sale del collider de la linterna.
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        // Si no es un arma letal, no hacemos nada
        if (!esArmaLetal) return;
        
        // IMPORTANTE: Ignorar colliders que sean triggers
        if (other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            // Buscar EnemyController en el GameObject o en el padre
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyController>();
            }
            
            // Si el enemigo sale, dejar de huir
            if (enemy != null)
            {
                // Solo dejamos de huir si salimos con el cuerpo
                enemy.DejarDeHuir();
            }

            if (enemy != null && enemy == enemigoActual)
            {
                enemigoActual = null;
                // Debug.Log("Enemigo salió del rango de la linterna.");
            }
        }
    }
}

