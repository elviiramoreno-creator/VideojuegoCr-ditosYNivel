using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("UI GameOver")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private string menuSceneName = "Menu";
    
    private bool gameOverActive = false;
    
    void Start()
    {
        // Initial search
        if (gameOverPanel == null)
        {
            // 1. Try direct find (active only)
            gameOverPanel = GameObject.Find("panel_gameOver");
            if (gameOverPanel == null) gameOverPanel = GameObject.Find("Panel_GameOver");
            if (gameOverPanel == null) gameOverPanel = GameObject.Find("GameOverPanel");
            
            // 2. Try inactive find
            if (gameOverPanel == null)
            {
                gameOverPanel = FindInactiveGameObjectByName("panel_gameOver");
                if (gameOverPanel == null) gameOverPanel = FindInactiveGameObjectByName("Panel_GameOver");
                if (gameOverPanel == null) gameOverPanel = FindInactiveGameObjectByName("GameOverPanel");
            }
        }
        
        // Log status
        if (gameOverPanel != null)
        {
            Debug.Log($"GameOverManager: Panel encontrado correctamente: {gameOverPanel.name}");
            gameOverPanel.SetActive(false); // Hide on start
            
            // Find buttons inside this specific panel
            SetupButtons(gameOverPanel);
        }
        else
        {
            Debug.LogError("GameOverManager: ¡CRÍTICO! No se encuentra 'panel_gameOver'. Asegúrate de que existe en el Canvas.");
        }
    }
    
    void SetupButtons(GameObject panel)
    {
         // Restart Button
         if (restartButton == null)
         {
             Transform t = FindRecursive(panel.transform, "Restart");
             if (t == null) t = FindRecursive(panel.transform, "RestartButton");
             if (t == null) t = FindRecursive(panel.transform, "BotonRestart");
             if (t != null) restartButton = t.GetComponent<Button>();
         }
         
         // Exit Button
         if (exitButton == null)
         {
             Transform t = FindRecursive(panel.transform, "Exit");
             if (t == null) t = FindRecursive(panel.transform, "ExitButton");
             if (t == null) t = FindRecursive(panel.transform, "BotonExit");
             if (t == null) t = FindRecursive(panel.transform, "Quit");
             if (t != null) exitButton = t.GetComponent<Button>();
         }

         // Bind listeners
         if (restartButton != null)
         {
             restartButton.onClick.RemoveAllListeners();
             restartButton.onClick.AddListener(RestartGame);
         }
         if (exitButton != null)
         {
             exitButton.onClick.RemoveAllListeners();
             exitButton.onClick.AddListener(ExitToMenu);
         }
    }

    Transform FindRecursive(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase) || child.name.Contains(name))
                return child;
            
            Transform result = FindRecursive(child, name);
            if (result != null) return result;
        }
        return null;
    }

    public void ShowGameOver()
    {
        if (gameOverActive) return;
        
        Debug.Log("GameOverManager: ShowGameOver() llamado.");
        gameOverActive = true;
        Time.timeScale = 0f; 
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling(); // Ensure it's on top
        }
        else
        {
            Debug.LogError("GameOverManager: No hay panel para mostrar. Intentando buscar de emergencia...");
            gameOverPanel = FindInactiveGameObjectByName("panel_gameOver");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                gameOverPanel.transform.SetAsLastSibling();
            }
        }
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
    public void RestartGame()
    {
        Time.timeScale = 1f; // Restaurar tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ExitToMenu()
    {
        Time.timeScale = 1f; // Restaurar tiempo
        
        // Buscar escena de menú
        if (string.IsNullOrEmpty(menuSceneName))
        {
            menuSceneName = "Menu";
        }
        
        try
        {
            SceneManager.LoadScene(menuSceneName);
        }
        catch
        {
            Debug.LogWarning($"No se pudo cargar la escena '{menuSceneName}'. Verifica que existe en Build Settings.");
            // Intentar cargar la primera escena (menú típicamente)
            if (SceneManager.sceneCountInBuildSettings > 0)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se restaure
        Time.timeScale = 1f;
    }
}