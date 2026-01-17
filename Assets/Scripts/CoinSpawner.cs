using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class CoinSpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject prefabMoneda;
    [SerializeField] private int cantidadMonedas = 20; // Duplicado de 10 a 20
    
    [Header("Referencias")]
    [SerializeField] private Tilemap tilemapSuelo;
    
    void Start()
    {
        // Buscar tilemap de suelo
        if (tilemapSuelo == null)
        {
            GameObject sueloMapa = GameObject.Find("Tilemap-SueloMapa");
            if (sueloMapa != null)
            {
                tilemapSuelo = sueloMapa.GetComponent<Tilemap>();
            }
        }
        
        // Si no hay prefab asignado, buscar uno en la escena
        if (prefabMoneda == null)
        {
            GameObject monedaExistente = GameObject.FindGameObjectWithTag("Coin");
            if (monedaExistente != null)
            {
                prefabMoneda = monedaExistente;
            }
        }
        
        if (prefabMoneda != null)
        {
            GenerarMonedas();
        }
        else
        {
            Debug.LogWarning("No se encontró prefab de moneda. Asegúrate de tener un GameObject con tag 'Coin' en la escena.");
        }
    }
    
    void GenerarMonedas()
    {
        if (tilemapSuelo == null)
        {
            Debug.LogError("No se encontró Tilemap-SueloMapa. No se pueden generar monedas.");
            return;
        }
        
        // Obtener todas las celdas con tiles del tilemap
        tilemapSuelo.CompressBounds();
        BoundsInt bounds = tilemapSuelo.cellBounds;
        
        List<Vector3> posicionesValidas = new List<Vector3>();
        
        // Recorrer todas las celdas del tilemap
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int posicionCelda = new Vector3Int(x, y, 0);
                
                // Verificar si hay un tile en esta celda
                if (tilemapSuelo.HasTile(posicionCelda))
                {
                    // Convertir posición de celda a posición del mundo
                    Vector3 posicionMundo = tilemapSuelo.CellToWorld(posicionCelda);
                    posicionMundo += tilemapSuelo.cellSize * 0.5f; // Centrar en la celda
                    posicionMundo += new Vector3(0, 0.5f, 0); // Elevar un poco para que no esté en el suelo
                    
                    posicionesValidas.Add(posicionMundo);
                }
            }
        }
        
        if (posicionesValidas.Count == 0)
        {
            Debug.LogWarning("No se encontraron posiciones válidas en el tilemap. Generando en posiciones aleatorias.");
            // Fallback: generar en posiciones aleatorias
            for (int i = 0; i < cantidadMonedas; i++)
            {
                float posX = Random.Range(-10f, 10f);
                float posY = Random.Range(-1f, 3f);
                CrearMoneda(new Vector3(posX, posY, 0), i);
            }
            return;
        }
        
        // Generar monedas en posiciones aleatorias del tilemap
        int monedasGeneradas = 0;
        for (int i = 0; i < cantidadMonedas && monedasGeneradas < posicionesValidas.Count; i++)
        {
            int indiceAleatorio = Random.Range(0, posicionesValidas.Count);
            Vector3 posicion = posicionesValidas[indiceAleatorio];
            
            // Añadir pequeña variación aleatoria
            posicion += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.2f, 0.5f), 0);
            
            CrearMoneda(posicion, i);
            monedasGeneradas++;
        }
        
        Debug.Log($"Se generaron {monedasGeneradas} monedas a lo largo del Tilemap-SueloMapa.");
    }
    
    void CrearMoneda(Vector3 posicion, int indice)
    {
        // Instanciar moneda
        GameObject nuevaMoneda = Instantiate(prefabMoneda, posicion, Quaternion.identity);
        nuevaMoneda.name = $"Coin_{indice + 1}";
        
        // Registrar en game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegistrarNuevaMoneda();
        }
        
        // Asegurar que tenga el tag correcto
        if (!nuevaMoneda.CompareTag("Coin"))
        {
            nuevaMoneda.tag = "Coin";
        }
        
        // Asegurar que tenga un Collider2D como trigger
        Collider2D collider = nuevaMoneda.GetComponent<Collider2D>();
        if (collider == null)
        {
            CircleCollider2D circleCollider = nuevaMoneda.AddComponent<CircleCollider2D>();
            circleCollider.isTrigger = true;
            circleCollider.radius = 0.5f;
        }
        else
        {
            collider.isTrigger = true;
        }
    }
}