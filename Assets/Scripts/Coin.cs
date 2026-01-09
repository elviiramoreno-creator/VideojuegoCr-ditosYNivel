using UnityEngine;
public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int valorMoneda = 1;
    [SerializeField] private float velocidadRotacion = 90f;
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
    }
    
    void Update()
    {
        // Rotación visual
        transform.Rotate(0, 0, velocidadRotacion * Time.deltaTime);
        
        // Verificar distancia al player cada frame
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
            // Verificar distancia antes de recoger
            float distancia = Vector3.Distance(transform.position, other.transform.position);
            if (distancia <= distanciaMinimaRecogida)
            {
                Recoger();
            }
        }
    }
    void Recoger()
    {
        if (recogida) return;
        
        recogida = true;
        
        // Notificar al GameManager
        GameManager.Instance?.RecogerMoneda(valorMoneda);
        
        // Efecto visual/sonido (opcional)
        // Aquí podrías agregar una animación o sonido
        
        // Destruir la moneda
        Destroy(gameObject);
    }
}