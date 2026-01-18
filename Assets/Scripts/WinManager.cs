using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    [Header("UI Win Panel")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private string menuSceneName = "Menu";
    
    private bool winActive = false;
    
    void Start()
    {
        // Buscar panel si no está asignado
        if (winPanel == null)
        {
            winPanel = GameObject.Find("Panel_Win");
            if (winPanel == null)
            {
                // Intentar buscar incluso si está desactivado
                winPanel = FindInactiveGameObjectByName("Panel_Win");
            }
            if (winPanel == null)
            {
                winPanel = GameObject.Find("WinPanel");
            }
        }
        
        // Ocultar panel al inicio
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("WinManager: No se encontró el Panel_Win. Asegúrate de que existe en la escena.");
        }
        
        // Configurar botón Exit
        if (exitButton == null && winPanel != null)
        {
            // Intentar buscar el botón en los hijos del panel
            Transform exitTrans = winPanel.transform.Find("Exit");
            if (exitTrans == null) exitTrans = winPanel.transform.Find("ExitButton");
            if (exitTrans == null) exitTrans = winPanel.transform.Find("BotonExit");
            
            if (exitTrans != null)
            {
                exitButton = exitTrans.GetComponent<Button>();
            }
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveAllListeners(); // Limpiar listeners previos para evitar duplicados
            exitButton.onClick.AddListener(ExitToMenu);
        }
        
        // Configurar botón Continue (si existe)
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueToNextLevel);
        }
    }
    
    public void ShowWin()
    {
        if (winActive) return;
        
        winActive = true;
        Time.timeScale = 0f; // Pausar el juego
        
        Debug.Log("¡VICTORIA! Mostrando panel de victoria.");
        
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
        else
        {
            // Intentar encontrar de nuevo
            winPanel = GameObject.Find("Panel_Win");
            if (winPanel == null)
            {
                winPanel = GameObject.Find("WinPanel");
            }
            
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }
            else
            {
                Debug.LogError("WinManager: No se puede mostrar el panel de victoria porque no existe.");
            }
        }
    }
    
    public void ExitToMenu()
    {
        Time.timeScale = 1f; // Restaurar tiempo
        
        // Buscar escena de menú
        if (string.IsNullOrEmpty(menuSceneName))
        {
            menuSceneName = "Menu";
        }
        
        Debug.Log($"Volviendo al menú: {menuSceneName}");
        
        try
        {
            SceneManager.LoadScene(menuSceneName);
        }
        catch
        {
            Debug.LogWarning($"No se pudo cargar la escena '{menuSceneName}'. Cargando primera escena.");
            // Intentar cargar la primera escena (menú típicamente)
            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    
    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f; // Restaurar tiempo
        
        // Cargar la siguiente escena en el Build Settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No hay más niveles. Volviendo al menú.");
            ExitToMenu();
        }
    }
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se restaure
        Time.timeScale = 1f;
    }

    // Helper para encontrar objetos desactivados
    GameObject FindInactiveGameObjectByName(string name)
    {
        GameObject[] objs = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in objs)
        {
            if (obj.name == name && obj.scene.IsValid())
            {
                return obj;
            }
        }
        return null;
    }
}
