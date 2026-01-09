using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Script para asegurar que los Tilemaps no tengan Rigidbody2D dinámico.
/// Debe ejecutarse una vez al inicio o puede usarse como editor script.
/// </summary>
public class TilemapFixer : MonoBehaviour
{
    [ContextMenu("Arreglar Tilemaps")]
    public void ArreglarTilemaps()
    {
        // Buscar todos los tilemaps en la escena
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        
        foreach (Tilemap tilemap in tilemaps)
        {
            Rigidbody2D rb = tilemap.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                // Si tiene Rigidbody2D, cambiarlo a Kinematic o eliminarlo
                if (rb.bodyType == RigidbodyType2D.Dynamic)
                {
                    Debug.Log($"Cambiando Rigidbody2D de {tilemap.name} a Kinematic");
                    rb.bodyType = RigidbodyType2D.Kinematic;
                }
                else if (rb.bodyType == RigidbodyType2D.Kinematic)
                {
                    // Ya está bien configurado
                    Debug.Log($"{tilemap.name} ya tiene Rigidbody2D Kinematic");
                }
            }
            else
            {
                // No tiene Rigidbody2D, está bien
                Debug.Log($"{tilemap.name} no tiene Rigidbody2D (correcto)");
            }
        }
        
        Debug.Log("Verificación de Tilemaps completada.");
    }
    
    void Start()
    {
        // Ejecutar automáticamente al inicio
        ArreglarTilemaps();
    }
}