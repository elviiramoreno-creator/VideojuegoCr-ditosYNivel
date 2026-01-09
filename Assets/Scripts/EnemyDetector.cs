using UnityEngine;
using System.Collections.Generic;

public class EnemyDetector : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float distanciaDeteccion = 3f;
    [SerializeField] private float distanciaMinima = 1.5f; // Distancia mínima para considerar que se esquivó
    
    private Transform player;
    private Dictionary<GameObject, bool> enemigosDetectados = new Dictionary<GameObject, bool>();
    private List<GameObject> enemigosProximos = new List<GameObject>();
    
    void Start()
    {
        player = transform;
    }
    
    void Update()
    {
        DetectarEnemigos();
    }
    
    void DetectarEnemigos()
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");
        
        foreach (GameObject enemigo in enemigos)
        {
            if (enemigo == null) continue;
            
            // Inicializar en el diccionario si no existe
            if (!enemigosDetectados.ContainsKey(enemigo))
            {
                enemigosDetectados[enemigo] = false;
            }
            
            // Si ya fue detectado, continuar
            if (enemigosDetectados[enemigo]) continue;
            
            float distancia = Vector3.Distance(player.position, enemigo.transform.position);
            
            // Si el enemigo está dentro del rango de detección
            if (distancia <= distanciaDeteccion)
            {
                // Verificar si el jugador está detrás del enemigo (lo ha esquivado)
                Vector3 direccionJugadorEnemigo = (enemigo.transform.position - player.position).normalized;
                float dotProduct = Vector3.Dot(direccionJugadorEnemigo, Vector3.up);
                
                // Si el jugador está detrás del enemigo (más arriba) y a una distancia segura
                if (player.position.y > enemigo.transform.position.y && distancia >= distanciaMinima)
                {
                    enemigosDetectados[enemigo] = true;
                    GameManager.Instance?.EsquivarEnemigo();
                }
            }
        }
    }
    
    void OnDestroy()
    {
        enemigosDetectados.Clear();
        enemigosProximos.Clear();
    }
}