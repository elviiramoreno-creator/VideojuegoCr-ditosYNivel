using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDetector : MonoBehaviour
{
    [Header("Referencias a Tilemaps")]
    [SerializeField] private Tilemap tilemapSuelo;
    [SerializeField] private Tilemap tilemapHielo;
    [SerializeField] private Tilemap tilemapEnredadera;
    
    [Header("Referencia al Player")]
    [SerializeField] private PlayerController playerController;
    
    private void Start()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();
        
        // Buscar tilemaps si no están asignados
        if (tilemapSuelo == null)
            tilemapSuelo = GameObject.Find("Tilemap-suelo")?.GetComponent<Tilemap>();
        
        if (tilemapHielo == null)
            tilemapHielo = GameObject.Find("Tilemap-Hielo")?.GetComponent<Tilemap>();
        
        if (tilemapEnredadera == null)
            tilemapEnredadera = GameObject.Find("Tilemap-Enredadera")?.GetComponent<Tilemap>();
    }
    
    private void Update()
    {
        if (playerController == null) return;
        
        Vector3Int posicionCelda = Vector3Int.zero;
        string tilemapDetectado = "suelo"; // Por defecto
        
        // Convertir posición del jugador a coordenadas de celda
        if (tilemapHielo != null)
        {
            posicionCelda = tilemapHielo.WorldToCell(transform.position);
            if (tilemapHielo.HasTile(posicionCelda))
            {
                tilemapDetectado = "Hielo";
                playerController.SetTilemapActual(tilemapDetectado);
                return;
            }
        }
        
        if (tilemapEnredadera != null)
        {
            posicionCelda = tilemapEnredadera.WorldToCell(transform.position);
            if (tilemapEnredadera.HasTile(posicionCelda))
            {
                tilemapDetectado = "Enredadera";
                playerController.SetTilemapActual(tilemapDetectado);
                return;
            }
        }
        
        if (tilemapSuelo != null)
        {
            posicionCelda = tilemapSuelo.WorldToCell(transform.position);
            if (tilemapSuelo.HasTile(posicionCelda))
            {
                tilemapDetectado = "suelo";
            }
        }
        
        playerController.SetTilemapActual(tilemapDetectado);
    }
}