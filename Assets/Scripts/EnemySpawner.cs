using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic; // AÑADIR ESTA LÍNEA

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private int cantidadEnemigos = 30;
    [SerializeField] private bool detectarLimitesAutomaticamente = true;
    
    [Header("Configuración de Patrullaje")]
    [SerializeField] private float rangoPatrullaje = 3f;
    
    [Header("Referencias")]
    [SerializeField] private Tilemap tilemapSuelo;
    
    private float limiteIzquierdo;
    private float limiteDerecho;
    private BoundsInt bounds;
    
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
        
        // Detectar límites de las paredes
        if (detectarLimitesAutomaticamente)
        {
            DetectarLimitesParedes();
        }
        else
        {
            // Usar límites del tilemap si no hay detección automática
            if (tilemapSuelo != null)
            {
                tilemapSuelo.CompressBounds();
                bounds = tilemapSuelo.cellBounds;
                limiteIzquierdo = tilemapSuelo.CellToWorld(new Vector3Int(bounds.xMin, 0, 0)).x;
                limiteDerecho = tilemapSuelo.CellToWorld(new Vector3Int(bounds.xMax, 0, 0)).x;
            }
        }
        
        // Si no hay prefab asignado, buscar uno en la escena
        if (prefabEnemigo == null)
        {
            GameObject enemigoExistente = GameObject.FindGameObjectWithTag("Enemy");
            if (enemigoExistente != null)
            {
                prefabEnemigo = enemigoExistente;
            }
        }
        
        if (prefabEnemigo != null)
        {
            GenerarEnemigos();
        }
        else
        {
            Debug.LogWarning("No se encontró prefab de enemigo. Asegúrate de tener un GameObject con tag 'Enemy' en la escena.");
        }
    }
    
    void DetectarLimitesParedes()
    {
        // Buscar tilemaps de bordes
        GameObject bordeIzq = GameObject.Find("Tilemap-Bordes");
        if (bordeIzq == null)
            bordeIzq = GameObject.Find("tilemap-borde");
            
        GameObject bordeDer = GameObject.Find("Tilemap-Bordes Dcha");
        if (bordeDer == null)
            bordeDer = GameObject.Find("tilemap-borde dcha");
        
        if (bordeIzq != null)
        {
            Collider2D collider = bordeIzq.GetComponent<Collider2D>();
            if (collider != null)
            {
                limiteIzquierdo = collider.bounds.max.x + 0.5f;
            }
            else
            {
                limiteIzquierdo = bordeIzq.transform.position.x + 0.5f;
            }
        }
        else if (tilemapSuelo != null)
        {
            tilemapSuelo.CompressBounds();
            bounds = tilemapSuelo.cellBounds;
            limiteIzquierdo = tilemapSuelo.CellToWorld(new Vector3Int(bounds.xMin, 0, 0)).x;
        }
        
        if (bordeDer != null)
        {
            Collider2D collider = bordeDer.GetComponent<Collider2D>();
            if (collider != null)
            {
                limiteDerecho = collider.bounds.min.x - 0.5f;
            }
            else
            {
                limiteDerecho = bordeDer.transform.position.x - 0.5f;
            }
        }
        else if (tilemapSuelo != null)
        {
            tilemapSuelo.CompressBounds();
            bounds = tilemapSuelo.cellBounds;
            limiteDerecho = tilemapSuelo.CellToWorld(new Vector3Int(bounds.xMax, 0, 0)).x;
        }
        
        Debug.Log($"Límites de spawn detectados: Izquierdo = {limiteIzquierdo}, Derecho = {limiteDerecho}");
    }
    
    void GenerarEnemigos()
    {
        if (tilemapSuelo == null)
        {
            Debug.LogError("No se encontró Tilemap-SueloMapa. No se pueden generar enemigos.");
            return;
        }
        
        // Obtener todas las celdas con tiles del tilemap
        tilemapSuelo.CompressBounds();
        bounds = tilemapSuelo.cellBounds;
        
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
                    
                    // Verificar que esté dentro de los límites horizontales
                    if (posicionMundo.x >= limiteIzquierdo && posicionMundo.x <= limiteDerecho)
                    {
                        posicionesValidas.Add(posicionMundo);
                    }
                }
            }
        }
        
        if (posicionesValidas.Count == 0)
        {
            Debug.LogWarning("No se encontraron posiciones válidas en el tilemap. Generando en posiciones aleatorias.");
            // Fallback: generar en posiciones aleatorias
            for (int i = 0; i < cantidadEnemigos; i++)
            {
                float posX = Random.Range(limiteIzquierdo, limiteDerecho);
                float posY = Random.Range(-2f, 2f);
                CrearEnemigo(new Vector3(posX, posY, 0), i);
            }
            return;
        }
        
        // Generar enemigos en posiciones aleatorias del tilemap
        int enemigosGenerados = 0;
        for (int i = 0; i < cantidadEnemigos && enemigosGenerados < posicionesValidas.Count; i++)
        {
            int indiceAleatorio = Random.Range(0, posicionesValidas.Count);
            Vector3 posicion = posicionesValidas[indiceAleatorio];
            
            // Añadir pequeña variación aleatoria
            posicion += new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
            
            CrearEnemigo(posicion, i);
            enemigosGenerados++;
        }
        
        Debug.Log($"Se generaron {enemigosGenerados} enemigos a lo largo del Tilemap-SueloMapa.");
    }
    
    void CrearEnemigo(Vector3 posicion, int indice)
    {
        // Instanciar enemigo
        GameObject nuevoEnemigo = Instantiate(prefabEnemigo, posicion, Quaternion.identity);
        nuevoEnemigo.name = $"Enemy_{indice + 1}";
        
        // Configurar límites de patrullaje
        EnemyController enemyController = nuevoEnemigo.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            float limiteIzq = Mathf.Max(limiteIzquierdo, posicion.x - rangoPatrullaje);
            float limiteDer = Mathf.Min(limiteDerecho, posicion.x + rangoPatrullaje);
            
            if (limiteIzq >= limiteDer)
            {
                float rangoAjustado = rangoPatrullaje * 0.5f;
                limiteIzq = posicion.x - rangoAjustado;
                limiteDer = posicion.x + rangoAjustado;
            }
            
            enemyController.SetLimites(limiteIzq, limiteDer);
        }
        
        // Asegurar que tenga el tag correcto
        if (!nuevoEnemigo.CompareTag("Enemy"))
        {
            nuevoEnemigo.tag = "Enemy";
        }
    }
}