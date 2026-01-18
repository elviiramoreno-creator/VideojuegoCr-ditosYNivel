using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int valorMoneda = 1;
    [SerializeField] private float distanciaMinimaRecogida = 0.8f; // Distancia mínima para recoger la moneda
    
    private bool recogida = false;
    private Collider2D coinCollider;
    
    void Start()
    {
        coinCollider = GetComponent<Collider2D>();
        // Asegurar que el collider sea un trigger
        if (coinCollider != null)
        {
            coinCollider.isTrigger = true;
        }
        
        // Registro manual en GameManager ahora (por petición de usuario)
        // if (GameManager.Instance != null) { GameManager.Instance.RegistrarTotalMonedas(1); }
    }
    
    void Update()
    {
        // Solo verificar distancia al player cada frame
        if (!recogida)
        {
            VerificarDistanciaPlayer();
        }
    }
    
    void VerificarDistanciaPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distancia = Vector3.Distance(transform.position, player.transform.position);
            if (distancia <= distanciaMinimaRecogida)
            {
                Recoger();
            }
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !recogida)
        {
            Recoger();
        }
    }
    
    void Recoger()
    {
        if (recogida) return;
        
        recogida = true;
        
        // Buscar el Player y añadir moneda
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.AnadirMoneda(valorMoneda);
            }
        }
        else
        {
            // Fallback por si no encuentra al player (no debería pasar)
            GameManager.Instance?.RecogerMoneda(valorMoneda);
        }
        
        // Efecto visual/sonido (opcional)
        // Aquí podrías agregar una animación o sonido
        
        // Destruir la moneda
        Destroy(gameObject);
    }
}