using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuración de Patrullaje")]
    [SerializeField] private float velocidadPatrullaje = 3f;
    [SerializeField] private float limiteIzquierdo = -5f;
    [SerializeField] private float limiteDerecho = 5f;
    
    [Header("Detección de Player")]
    [SerializeField] private float distanciaAtaque = 2.5f; // Distancia para activar animación de ataque
    
    [Header("Orientación")]
    [SerializeField] private bool invertirOrientacion = true; // Si true, invierte la lógica del flipX
    
    [Header("Referencias")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    private int direccion = 1; // 1 = derecha, -1 = izquierda
    private float posicionInicial;
    private GameObject player;
    private bool estaAtacando = false;
    
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
        
        posicionInicial = transform.position.x;
        
        // Establecer límites relativos a la posición inicial si no están configurados
        if (limiteIzquierdo == -5f && limiteDerecho == 5f)
        {
            limiteIzquierdo = posicionInicial - 3f;
            limiteDerecho = posicionInicial + 3f;
        }
        
        // Iniciar movimiento hacia la derecha
        direccion = 1;
        ActualizarOrientacion();
    }
    
    void Update()
    {
        // Buscar player si no está asignado
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        // Verificar si el player está cerca para activar animación de ataque
        VerificarPlayerCerca();
        
        Patrullar();
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
    
    public void SetLimites(float izquierdo, float derecho)
    {
        limiteIzquierdo = izquierdo;
        limiteDerecho = derecho;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // El PlayerController manejará la muerte
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // El PlayerController manejará la muerte
        }
    }
}