using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script para configurar automáticamente los botones de los paneles de UI.
/// Añádelo al Panel_GameOver y Panel_Win para que se auto-configuren.
/// </summary>
public class PanelButtonConfigurator : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private bool esGameOverPanel = false;
    [SerializeField] private bool esWinPanel = false;
    
    void Start()
    {
        ConfigurarBotones();
    }
    
    void ConfigurarBotones()
    {
        // Buscar todos los botones en este panel
        Button[] botones = GetComponentsInChildren<Button>(true);
        
        foreach (Button boton in botones)
        {
            string nombreBoton = boton.name.ToLower();
            
            // Configurar botón Exit
            if (nombreBoton.Contains("exit") || nombreBoton.Contains("salir") || nombreBoton.Contains("menu"))
            {
                // Eliminar listeners previos
                boton.onClick.RemoveAllListeners();
                
                // Añadir listener para volver al menú
                boton.onClick.AddListener(VolverAlMenu);
                
                Debug.Log($"Botón Exit configurado en {gameObject.name}: {boton.name}");
            }
            
            // Configurar botón Restart (solo en GameOver)
            if (esGameOverPanel && (nombreBoton.Contains("restart") || nombreBoton.Contains("reiniciar") || nombreBoton.Contains("retry")))
            {
                boton.onClick.RemoveAllListeners();
                boton.onClick.AddListener(ReiniciarNivel);
                
                Debug.Log($"Botón Restart configurado en {gameObject.name}: {boton.name}");
            }
            
            // Configurar botón Continue (solo en Win)
            if (esWinPanel && (nombreBoton.Contains("continue") || nombreBoton.Contains("continuar") || nombreBoton.Contains("siguiente")))
            {
                boton.onClick.RemoveAllListeners();
                boton.onClick.AddListener(ContinuarSiguienteNivel);
                
                Debug.Log($"Botón Continue configurado en {gameObject.name}: {boton.name}");
            }
        }
    }
    
    void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú...");
        
        // Restaurar tiempo por si estaba pausado
        Time.timeScale = 1f;
        
        // Buscar GameOverManager o WinManager
        if (esGameOverPanel)
        {
            GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
            if (gameOverManager != null)
            {
                gameOverManager.ExitToMenu();
                return;
            }
        }
        
        if (esWinPanel)
        {
            WinManager winManager = FindFirstObjectByType<WinManager>();
            if (winManager != null)
            {
                winManager.ExitToMenu();
                return;
            }
        }
        
        // Fallback: cargar escena Menu directamente
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
    
    void ReiniciarNivel()
    {
        Debug.Log("Reiniciando nivel...");
        
        GameOverManager gameOverManager = FindFirstObjectByType<GameOverManager>();
        if (gameOverManager != null)
        {
            gameOverManager.RestartGame();
        }
        else
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
    
    void ContinuarSiguienteNivel()
    {
        Debug.Log("Continuando al siguiente nivel...");
        
        WinManager winManager = FindFirstObjectByType<WinManager>();
        if (winManager != null)
        {
            winManager.ContinueToNextLevel();
        }
        else
        {
            Time.timeScale = 1f;
            VolverAlMenu();
        }
    }
}
