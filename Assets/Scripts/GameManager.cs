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
    [Header("Referencias UI")]
    [SerializeField] private GameObject panelGamePlay;
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
        // ASEGURAR QUE EL JUEGO NO ESTÁ PAUSADO
        Time.timeScale = 1f;

        Debug.Log("<color=green>GameManager: START ejecutándose. Buscando componentes...</color>");

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        
        if (metaNivel == null)
        {
            GameObject metaObj = GameObject.Find("MetaNivel");
            if (metaObj != null)
                metaNivel = metaObj.transform;
        }

        // 1. Configurar Panel GamePlay (activarlo al inicio)
        if (panelGamePlay == null)
        {
            panelGamePlay = GameObject.Find("panel_GamePlay");
            if (panelGamePlay == null) panelGamePlay = GameObject.Find("Panel_GamePlay");
        }

        if (panelGamePlay != null)
        {
            panelGamePlay.SetActive(true);
        }
        else
        {
            Debug.LogWarning("GameManager: No se encontró 'panel_GamePlay'. Asegúrate de que existe en el Canvas.");
        }
        
        // Buscar GameOverManager si no está asignado
        if (gameOverManager == null)
        {
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            // ... (omitimos creación automática para no ensuciar, mejor que avise si falta)
        }
        
        // Buscar WinManager si no está asignado
        if (winManager == null)
        {
            winManager = FindFirstObjectByType<WinManager>();
        }
        
        // Contar objetivos del nivel
        ContarObjetivosDelNivel();
        
        ResetearContadores();
    }
    
    void Update()
    {
        // Optimización: No buscar cada frame, solo si es nulo y cada cierto tiempo (opcional)
        // Por ahora lo dejamos simple para asegurar que lo encuentra, pero cuidado con el rendimiento.
        if (player == null && Time.frameCount % 60 == 0) // Buscar solo una vez por segundo (aprox)
            player = GameObject.FindGameObjectWithTag("Player");
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
        // Contar enemigos
        if (enemigosAEliminar <= 0)
        {
            EnemyController[] enemigos = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
            totalEnemigosEnNivel = enemigos.Length;
        }
        else
        {
            totalEnemigosEnNivel = enemigosAEliminar;
        }
        
        // RECUENTO DE MONEDAS EN EL INSPECTOR AL INICIO (Petición usuario: Por Tag "Moneda")
        try
        {
            GameObject[] monedasPorTag = GameObject.FindGameObjectsWithTag("Moneda");
            totalMonedasEnNivel = monedasPorTag.Length;
            Debug.Log($"<color=yellow>---> RECUENTO INICIAL: Se han detectado {totalMonedasEnNivel} objetos con el tag 'Moneda' en la escena. <---</color>");
        }
        catch (UnityException)
        {
            Debug.LogError("GameManager: Error al buscar por Tag 'Moneda'. Asegúrate de que el Tag existe en Project Settings.");
            // Fallback: Buscar por tipo Coin
            Coin[] monedasComponente = FindObjectsByType<Coin>(FindObjectsSortMode.None);
            totalMonedasEnNivel = monedasComponente.Length;
        }
    }

    public void RegistrarTotalMonedas(int cantidad)
    {
        // Mantener por si se generan monedas dinámicamente después del Start
        totalMonedasEnNivel += cantidad;
        Debug.Log($"Moneda extra registrada. Total monedas a recoger: {totalMonedasEnNivel}");
    }
    
    void VerificarVictoria()
    {
        if (nivelCompletado) return;
        
        // Verificar si se han cumplido TODOS los objetivos
        bool todasLasMonedasRecogidas = (monedasRecogidas >= totalMonedasEnNivel);
        
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
        
        // DESACTIVAR HUD
        if (panelGamePlay != null) panelGamePlay.SetActive(false);

        if (winManager != null)
        {
            winManager.ShowWin();
        }
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        
        // DESACTIVAR HUD
        if (panelGamePlay != null) panelGamePlay.SetActive(false);
        
        if (gameOverManager == null)
            gameOverManager = FindFirstObjectByType<GameOverManager>();
            
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            // Fallback
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