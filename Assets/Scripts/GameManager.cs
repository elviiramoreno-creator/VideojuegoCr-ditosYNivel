using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Objetivos del Nivel")]
    [SerializeField] private int enemigosAEsquivar = 15;
    [SerializeField] private int monedasARecoger = 10;
    
    [Header("UI Reference")]
    [SerializeField] private HUDController hudController;
    
    [Header("Referencias")]
    [SerializeField] private Transform metaNivel;
    [SerializeField] private GameObject player;
    
    private int enemigosEsquivados = 0;
    private int monedasRecogidas = 0;
    private bool nivelCompletado = false;
    
    private int totalMonedasNivel = 0;

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
        
        // Resetear total al iniciar (por si se recarga la escena)
        totalMonedasNivel = 0;
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

        // Buscar HUDController dinámicamente si no está asignado
        if (hudController == null)
        {
            hudController = FindFirstObjectByType<HUDController>();
        }
        
        // Contar todas las monedas que existen en la escena al inicio
        ContarMonedasIniciales();
        
        ResetearContadores();
        
        // Inicializar texto UI con lo que se haya registrado hasta ahora
        ActualizarHUDMonedas();
    }
    
    void ContarMonedasIniciales()
    {
        // Prioridad: Configuración manual en HUDController
        if (hudController != null && hudController.monedasInicialesManuales > 0)
        {
            totalMonedasNivel = hudController.monedasInicialesManuales;
            Debug.Log($"Usando configuración manual del HUD: {totalMonedasNivel} monedas.");
        }
        else
        {
            // Fallback: Contar automáticamente
            // Busca todos los objetos activos con el componente Coin
            Coin[] monedas = FindObjectsByType<Coin>(FindObjectsSortMode.None);
            totalMonedasNivel = monedas.Length;
            Debug.Log($"Monedas detectadas automáticamente: {totalMonedasNivel}");
        }
        
        // Si usamos CoinSpawner, las monedas se generan después...
        // Pero el CoinSpawner corre en Start también. 
        // Para asegurar, CoinSpawner debería tener prioridad o llamar a recalcular.
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
        
        if (hudController == null)
            hudController = FindFirstObjectByType<HUDController>();

        // La verificación de meta se hace mediante trigger en MetaNivel.cs
    }
    
    // Método para sumar monedas generadas dinámicamente si fuera necesario
    public void RegistrarNuevaMoneda()
    {
        totalMonedasNivel++;
        ActualizarHUDMonedas();
    }
    
    public void RecogerMoneda(int valor)
    {
        monedasRecogidas += valor;
        Debug.Log($"Monedas recogidas: {monedasRecogidas}/{monedasARecoger}. Total en nivel: {totalMonedasNivel}");
        
        // Actualizar UI
        ActualizarHUDMonedas();
    }
    
    private void ActualizarHUDMonedas()
    {
        if (hudController != null)
        {
            // Mostrar monedas RESTANTES
            int restantes = totalMonedasNivel - monedasRecogidas;
            if (restantes < 0) restantes = 0;
            
            hudController.ActualizarContadorMonedas(restantes);
        }
    }
    
    public void EsquivarEnemigo()
    {
        enemigosEsquivados++;
        Debug.Log($"Enemigos esquivados: {enemigosEsquivados}/{enemigosAEsquivar}");
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
    
    public void ReiniciarNivel()
    {
        Debug.Log("Reiniciando nivel...");
        ResetearContadores();
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