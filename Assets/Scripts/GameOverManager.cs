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
        // Si no hay panel asignado, buscarlo o crearlo
        if (gameOverPanel == null)
        {
            gameOverPanel = GameObject.Find("GameOverPanel");
            if (gameOverPanel == null)
            {
                CrearGameOverUI();
            }
        }
        
        // Ocultar panel al inicio
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Configurar botones
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitToMenu);
        }
    }
    
    public void ShowGameOver()
    {
        if (gameOverActive) return;
        
        gameOverActive = true;
        Time.timeScale = 0f; // Pausar el juego
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        else
        {
            // Intentar encontrar de nuevo
            gameOverPanel = GameObject.Find("GameOverPanel");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
        }
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
    
    void CrearGameOverUI()
    {
        // Crear Canvas si no existe
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // Crear Panel de GameOver
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvas.transform, false);
        Image panelImage = gameOverPanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f); // Fondo oscuro semi-transparente
        
        RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        
        // Crear texto "Game Over"
        GameObject gameOverText = new GameObject("GameOverText");
        gameOverText.transform.SetParent(gameOverPanel.transform, false);
        Text textComponent = gameOverText.AddComponent<Text>();
        textComponent.text = "GAME OVER";
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 72;
        textComponent.alignment = TextAnchor.MiddleCenter;
        textComponent.color = Color.white;
        
        RectTransform textRect = gameOverText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.7f);
        textRect.anchorMax = new Vector2(0.5f, 0.7f);
        textRect.sizeDelta = new Vector2(400, 100);
        textRect.anchoredPosition = Vector2.zero;
        
        // Crear botón Restart
        GameObject restartObj = new GameObject("RestartButton");
        restartObj.transform.SetParent(gameOverPanel.transform, false);
        restartButton = restartObj.AddComponent<Button>();
        Image restartImage = restartObj.AddComponent<Image>();
        restartImage.color = new Color(0.2f, 0.8f, 0.2f); // Verde
        
        GameObject restartTextObj = new GameObject("Text");
        restartTextObj.transform.SetParent(restartObj.transform, false);
        Text restartText = restartTextObj.AddComponent<Text>();
        restartText.text = "RESTART";
        restartText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        restartText.fontSize = 36;
        restartText.alignment = TextAnchor.MiddleCenter;
        restartText.color = Color.white;
        
        RectTransform restartRect = restartObj.GetComponent<RectTransform>();
        restartRect.anchorMin = new Vector2(0.5f, 0.4f);
        restartRect.anchorMax = new Vector2(0.5f, 0.4f);
        restartRect.sizeDelta = new Vector2(200, 60);
        restartRect.anchoredPosition = Vector2.zero;
        
        RectTransform restartTextRect = restartTextObj.GetComponent<RectTransform>();
        restartTextRect.anchorMin = Vector2.zero;
        restartTextRect.anchorMax = Vector2.one;
        restartTextRect.sizeDelta = Vector2.zero;
        
        // Crear botón Exit
        GameObject exitObj = new GameObject("ExitButton");
        exitObj.transform.SetParent(gameOverPanel.transform, false);
        exitButton = exitObj.AddComponent<Button>();
        Image exitImage = exitObj.AddComponent<Image>();
        exitImage.color = new Color(0.8f, 0.2f, 0.2f); // Rojo
        
        GameObject exitTextObj = new GameObject("Text");
        exitTextObj.transform.SetParent(exitObj.transform, false);
        Text exitText = exitTextObj.AddComponent<Text>();
        exitText.text = "EXIT";
        exitText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        exitText.fontSize = 36;
        exitText.alignment = TextAnchor.MiddleCenter;
        exitText.color = Color.white;
        
        RectTransform exitRect = exitObj.GetComponent<RectTransform>();
        exitRect.anchorMin = new Vector2(0.5f, 0.25f);
        exitRect.anchorMax = new Vector2(0.5f, 0.25f);
        exitRect.sizeDelta = new Vector2(200, 60);
        exitRect.anchoredPosition = Vector2.zero;
        
        RectTransform exitTextRect = exitTextObj.GetComponent<RectTransform>();
        exitTextRect.anchorMin = Vector2.zero;
        exitTextRect.anchorMax = Vector2.one;
        exitTextRect.sizeDelta = Vector2.zero;
        
        // Configurar listeners
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitToMenu);
    }
    
    void OnDestroy()
    {
        // Asegurar que el tiempo se restaure
        Time.timeScale = 1f;
    }
}