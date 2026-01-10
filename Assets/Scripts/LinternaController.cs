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

    void Start()
    {
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
        Quaternion rotacionDeseada = Quaternion.AngleAxis(angulo, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadRotacion * Time.deltaTime);
    }

    [Header("Sistema de Daño")]
    [Tooltip("Con vida máxima de 100, un valor de 50 significa que el enemigo muere en 2 segundos de iluminación continua")]
    [SerializeField] private float danoPorSegundo = 50f; // Daño que causa la linterna por segundo (gradual)
    [Tooltip("Intervalo más pequeño = daño más gradual y suave")]
    [SerializeField] private float intervaloDanos = 0.2f; // Intervalo entre cada aplicación de daño (más frecuente para ser gradual)
    
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
                // Aplicar daño gradual inmediato cuando entra en contacto
                float danoInicial = danoPorSegundo * intervaloDanos;
                enemy.RecibirDano(danoInicial);
                Debug.Log($"Linterna iluminó enemigo. Enemigo perdió {danoInicial:F1} de vida.");
            }
        }
    }

    /// <summary>
    /// Se ejecuta mientras un enemigo está dentro del collider de la linterna.
    /// Aplica daño continuo mientras el enemigo está iluminado.
    /// </summary>
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Buscar EnemyController en el GameObject o en el padre
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyController>();
            }
            
            if (enemy != null && enemy == enemigoActual)
            {
                // Aplicar daño gradual cada intervaloDanos segundos mientras está iluminado
                if (Time.time - tiempoUltimoDano >= intervaloDanos)
                {
                    float danoAplicado = danoPorSegundo * intervaloDanos;
                    enemy.RecibirDano(danoAplicado);
                    tiempoUltimoDano = Time.time;
                    Debug.Log($"Linterna continua iluminando enemigo. Enemigo perdió {danoAplicado:F1} de vida.");
                }
            }
            else if (enemy != null && enemy != enemigoActual)
            {
                // Si hay un enemigo diferente en el collider
                enemigoActual = enemy;
                tiempoUltimoDano = Time.time;
                float danoAplicado = danoPorSegundo * intervaloDanos;
                enemy.RecibirDano(danoAplicado);
            }
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando un enemigo sale del collider de la linterna.
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Buscar EnemyController en el GameObject o en el padre
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy == null)
            {
                enemy = other.GetComponentInParent<EnemyController>();
            }
            
            if (enemy != null && enemy == enemigoActual)
            {
                enemigoActual = null;
                Debug.Log("Enemigo salió del rango de la linterna.");
            }
        }
    }
}

