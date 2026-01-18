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
    [SerializeField] private int enemigosAEsquivar = 15;
    [SerializeField] private int monedasARecoger = 10;
    
    [Header("Referencias")]
    [SerializeField] private Transform metaNivel;
    [SerializeField] private GameObject player;
    [SerializeField] private GameOverManager gameOverManager;
    
    private int enemigosEsquivados = 0;
    private int monedasRecogidas = 0;
    private bool nivelCompletado = false;
    
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
        
        ResetearContadores();
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
        Debug.Log($"Monedas recogidas: {monedasRecogidas}/{monedasARecoger}");
        
        // Notificar evento para actualizar puntos
        OnMonedaRecogida?.Invoke();
    }
    
    public void EsquivarEnemigo()
    {
        enemigosEsquivados++;
        Debug.Log($"Enemigos esquivados: {enemigosEsquivados}/{enemigosAEsquivar}");
        
        // Notificar evento para actualizar puntos
        OnEnemigoEsquivado?.Invoke();
    }
    
    public void VerificarMetaNivel()
    {
        if (nivelCompletado) return;
        
        CompletarNivel();
    }
    
    void CompletarNivel()
    {
        if (nivelCompletado) return;
        
        // Verificar objetivos
        if (enemigosEsquivados >= enemigosAEsquivar && monedasRecogidas >= monedasARecoger)
        {
            nivelCompletado = true;
            Debug.Log("¡Nivel completado! Pasando al siguiente nivel...");
            // Aquí puedes cargar la siguiente escena
            // SceneManager.LoadScene("SiguienteNivel");
        }
        else
        {
            Debug.Log($"Objetivos no completados. Enemigos: {enemigosEsquivados}/{enemigosAEsquivar}, Monedas: {monedasRecogidas}/{monedasARecoger}");
        }
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over!");
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
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
        enemigosEsquivados = 0;
        monedasRecogidas = 0;
        nivelCompletado = false;
    }
    
    public int GetMonedasRecogidas()
    {
        return monedasRecogidas;
    }
    
    public int GetEnemigosEsquivados()
    {
        return enemigosEsquivados;
    }
}