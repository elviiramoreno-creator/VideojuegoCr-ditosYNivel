using UnityEngine;

/// <summary>
/// Script para el collider de detección del enemigo.
/// Este script debe estar en un GameObject hijo del enemigo con un Collider2D como Trigger.
/// Detecta cuando el player entra en su rango y notifica al EnemyController.
/// </summary>
public class EnemyDetectionZone : MonoBehaviour
{
    private EnemyController enemyController;
    
    void Start()
    {
        // Buscar el EnemyController en el padre
        enemyController = GetComponentInParent<EnemyController>();
        
        if (enemyController == null)
        {
            Debug.LogWarning($"EnemyDetectionZone en {gameObject.name} no encontró EnemyController en el padre.");
        }
        
        // Asegurar que este collider sea trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
            Debug.LogWarning($"El collider de detección en {gameObject.name} debe ser Trigger. Se configuró automáticamente.");
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando el player entra en el collider de detección.
    /// </summary>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enemyController != null)
        {
            enemyController.PlayerEntroEnRango();
        }
    }
    
    /// <summary>
    /// Se ejecuta cuando el player sale del collider de detección.
    /// </summary>
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enemyController != null)
        {
            enemyController.PlayerSalioDelRango();
        }
    }
}

