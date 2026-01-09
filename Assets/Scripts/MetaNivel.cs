using UnityEngine;

/// <summary>
/// Script para la meta del nivel. Debe colocarse al final del mapa.
/// </summary>
public class MetaNivel : MonoBehaviour
{
    void Start()
    {
        // Asegurar que tenga un Collider2D como trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector2(10f, 2f); // Ajusta según el ancho de tu mapa
        }
        else
        {
            collider.isTrigger = true;
        }
        
        // Asegurar que el nombre sea correcto
        if (gameObject.name != "MetaNivel")
        {
            gameObject.name = "MetaNivel";
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Notificar al PlayerController para detener movimiento automático
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.LlegarAMeta();
            }
            
            // Notificar al GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.VerificarMetaNivel();
            }
        }
    }
    
    void OnDrawGizmos()
    {
        // Visualizar la meta en el editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(10f, 2f, 0f));
    }
}