using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro;

 
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Eventos para el sistema de puntos
    public static event Action OnMonedaRecogida;
    public static event Action OnEnemigoEsquivado;
    
    [Header("Objetivos del Nivel")]
    [Tooltip("Número total de enemigos que deben ser eliminados para ganar")]
    [SerializeField] private int enemigosAEliminar = 0; // 0 = contar automáticamente al inicio
    [Tooltip("Número total de monedas que deben ser recogidas para ganar")]
    [SerializeField] private int monedasARecoger = 0; // 0 = contar automáticamente al inicio
    
    [Header("Referencias")]
    [SerializeField] private Transform metaNivel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private WinManager winManager;
    
    private int enemigosEliminados = 0;
    private int monedasRecogidas = 0;
    private bool nivelCompletado = false;
    private int totalEnemigosEnNivel = 0;
    private int totalMonedasEnNivel = 0;
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (metaNivel == null)
        {
            GameObject metaObj = GameObject.Find("MetaNivel");
            if (metaObj != null)
                metaNivel = metaObj.transform;
        }
        
        // Buscar GameOverManager si no está asignado
        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            if (gameOverManager == null)
            {
                // Crear GameOverManager automáticamente
                GameObject gameOverObj = new GameObject("GameOverManager");
                gameOverManager = gameOverObj.AddComponent<GameOverManager>();
            }
        }
        
        // Buscar WinManager si no está asignado
        if (winManager == null)
        {
            winManager = FindFirstObjectByType<WinManager>();
            if (winManager == null)
            {
                // Crear WinManager automáticamente
                GameObject winObj = new GameObject("WinManager");
                winManager = winObj.AddComponent<WinManager>();
            }
        }
        
        // Contar objetivos del nivel
        ContarObjetivosDelNivel();
        
        ResetearContadores();
        
        // Asegurar que panel_GamePlay está activo
        GameObject panelGamePlay = GameObject.Find("panel_GamePlay");
        if (panelGamePlay != null)
        {
            panelGamePlay.SetActive(true);
        }
    }
    
    void Update()
    {
        // Buscar referencias si no están asignadas
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (metaNivel == null)
        {
            GameObject metaObj = GameObject.Find("MetaNivel");
            if (metaObj != null)
                metaNivel = metaObj.transform;
        }
        
        // La verificación de meta se hace mediante trigger en MetaNivel.cs
    }
    
    public void RecogerMoneda(int valor)
    {
        monedasRecogidas += valor;
        Debug.Log($"Monedas recogidas: {monedasRecogidas}/{totalMonedasEnNivel}");
        
        // Verificar si se ganó el nivel
        VerificarVictoria();
        
        // Notificar evento para actualizar puntos
        OnMonedaRecogida?.Invoke();
    }
    
    public void EliminarEnemigo()
    {
        enemigosEliminados++;
        Debug.Log($"Enemigos eliminados: {enemigosEliminados}/{totalEnemigosEnNivel}");
        
        // Verificar si se ganó el nivel
        VerificarVictoria();
        
        // Notificar evento para actualizar puntos
        OnEnemigoEsquivado?.Invoke();
    }
    
    void ContarObjetivosDelNivel()
    {
        // Contar enemigos en la escena si no está configurado manualmente
        if (enemigosAEliminar <= 0)
        {
            EnemyController[] enemigos = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            totalEnemigosEnNivel = enemigos.Length;
            Debug.Log($"Enemigos detectados en el nivel: {totalEnemigosEnNivel}");
        }
        else
        {
            totalEnemigosEnNivel = enemigosAEliminar;
        }
        
        // El conteo de monedas ahora se hace dinámicamente cuando las monedas se registran al iniciar (Coin.Start)
        // Esto soluciona problemas con monedas generadas por Spawners
        Debug.Log("Esperando registro de monedas...");
    }

    public void RegistrarTotalMonedas(int cantidad)
    {
        totalMonedasEnNivel += cantidad;
        Debug.Log($"Moneda registrada. Total monedas a recoger: {totalMonedasEnNivel}");
    }
    
    void VerificarVictoria()
    {
        if (nivelCompletado) return;
        
        // Verificar si se han cumplido TODOS los objetivos
        bool todasLasMonedasRecogidas = (monedasRecogidas >= totalMonedasEnNivel);
        // bool todosLosEnemigosEliminados = (enemigosEliminados >= totalEnemigosEnNivel); // Desactivado por requisito del usuario
        
        if (todasLasMonedasRecogidas)
        {
            MostrarVictoria();
        }
    }
    
    public void MostrarVictoria()
    {
        if (nivelCompletado) return;
        
        nivelCompletado = true;
        Debug.Log("¡VICTORIA! Has completado todos los objetivos del nivel.");
        
        if (winManager != null)
        {
            winManager.ShowWin();
        }
        else
        {
            Debug.LogWarning("WinManager no encontrado. No se puede mostrar el panel de victoria.");
        }
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        
        if (gameOverManager == null)
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Debug.LogError("GameManager: ¡No existe GameOverManager! Reiniciando nivel como fallback.");
            // Fallback: reiniciar inmediatamente si no hay GameOverManager
            ReiniciarNivel();
        }
    }
    
    public void ReiniciarNivel()
    {
        Debug.Log("Reiniciando nivel...");
        ResetearContadores();
        
        // Resetear puntos del ScoreUI
        ScoreUI scoreUI = FindFirstObjectByType<ScoreUI>();
        if (scoreUI != null)
        {
            scoreUI.ResetearPuntos();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    void ResetearContadores()
    {
        enemigosEliminados = 0;
        monedasRecogidas = 0;
        nivelCompletado = false;
    }
    
    public int GetMonedasRecogidas()
    {
        return monedasRecogidas;
    }
    
    public int GetEnemigosEliminados()
    {
        return enemigosEliminados;
    }
    
    public int GetTotalMonedas()
    {
        return totalMonedasEnNivel;
    }
    
    public int GetTotalEnemigos()
    {
        return totalEnemigosEnNivel;
    }

    public void VerificarMetaNivel()
    {
        Debug.Log("¡JUGADOR LLEGÓ A LA META DEL NIVEL!");

        if (monedasRecogidas >= totalMonedasEnNivel)
        {
            Debug.Log($"¡VICTORIA! Recogiste {monedasRecogidas} monedas");
            MostrarVictoria();
        }
        else
        {
            Debug.Log($"Faltan {totalMonedasEnNivel - monedasRecogidas} monedas");
        }
    }
}