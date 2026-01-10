using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controlador de la barra de vida del enemigo.
/// Maneja la visualización de la vida actual y máxima del enemigo.
/// </summary>
public class HealthBarController : MonoBehaviour
{
    [Header("Componentes de UI")]
    [SerializeField] private Image barraVidaFill; // Imagen que se rellena según la vida
    [SerializeField] private Canvas canvasBarraVida; // Canvas de la barra de vida
    
    [Header("Referencias")]
    [SerializeField] private Camera mainCamera; // Cámara para que la barra siempre mire hacia la cámara
    
    private int vidaMaxima = 100;
    private int vidaActual = 100;
    
    void Start()
    {
        // Si no hay barraVidaFill asignada, intentar encontrarla en los hijos
        if (barraVidaFill == null)
        {
            barraVidaFill = GetComponentInChildren<Image>();
        }
        
        // Buscar canvas si no está asignado
        if (canvasBarraVida == null)
        {
            canvasBarraVida = GetComponentInChildren<Canvas>();
            if (canvasBarraVida == null)
            {
                // Crear canvas si no existe
                GameObject canvasObj = new GameObject("CanvasBarraVida");
                canvasObj.transform.SetParent(transform);
                canvasObj.transform.localPosition = Vector3.zero;
                canvasBarraVida = canvasObj.AddComponent<Canvas>();
                canvasBarraVida.renderMode = RenderMode.WorldSpace;
                canvasBarraVida.worldCamera = Camera.main;
                
                // Crear imagen de fondo (opcional)
                GameObject fondoObj = new GameObject("Fondo");
                fondoObj.transform.SetParent(canvasObj.transform);
                Image fondoImg = fondoObj.AddComponent<Image>();
                fondoImg.color = new Color(0, 0, 0, 0.5f);
                RectTransform fondoRect = fondoObj.GetComponent<RectTransform>();
                fondoRect.anchorMin = Vector2.zero;
                fondoRect.anchorMax = Vector2.one;
                fondoRect.sizeDelta = Vector2.zero;
                fondoRect.anchoredPosition = Vector2.zero;
                
                // Crear imagen de relleno
                GameObject fillObj = new GameObject("Fill");
                fillObj.transform.SetParent(fondoObj.transform);
                barraVidaFill = fillObj.AddComponent<Image>();
                barraVidaFill.color = Color.red;
                RectTransform fillRect = fillObj.GetComponent<RectTransform>();
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.sizeDelta = Vector2.zero;
                fillRect.anchoredPosition = Vector2.zero;
                fillRect.offsetMin = Vector2.zero;
                fillRect.offsetMax = Vector2.zero;
            }
        }
        
        // Configurar canvas para mundo
        if (canvasBarraVida != null)
        {
            canvasBarraVida.renderMode = RenderMode.WorldSpace;
            if (mainCamera == null)
                mainCamera = Camera.main;
            if (mainCamera != null)
                canvasBarraVida.worldCamera = mainCamera;
            
            // Configurar tamaño del canvas
            RectTransform canvasRect = canvasBarraVida.GetComponent<RectTransform>();
            if (canvasRect != null)
            {
                canvasRect.sizeDelta = new Vector2(1f, 0.2f);
            }
        }
        
        // Configurar imagen de relleno
        if (barraVidaFill != null)
        {
            barraVidaFill.type = Image.Type.Filled;
            barraVidaFill.fillMethod = Image.FillMethod.Horizontal;
        }
    }
    
    /// <summary>
    /// Inicializa la barra de vida con la vida máxima del enemigo.
    /// </summary>
    public void Inicializar(int vidaMax)
    {
        vidaMaxima = vidaMax;
        vidaActual = vidaMaxima;
        ActualizarBarra();
    }
    
    /// <summary>
    /// Actualiza la vida actual del enemigo y actualiza la visualización de la barra.
    /// </summary>
    public void ActualizarVida(int vida)
    {
        vidaActual = Mathf.Clamp(vida, 0, vidaMaxima);
        ActualizarBarra();
    }
    
    /// <summary>
    /// Actualiza la visualización de la barra de vida.
    /// </summary>
    void ActualizarBarra()
    {
        if (barraVidaFill != null && vidaMaxima > 0)
        {
            float porcentajeVida = (float)vidaActual / (float)vidaMaxima;
            barraVidaFill.fillAmount = porcentajeVida;
            
            // Cambiar color según la vida restante
            if (porcentajeVida > 0.5f)
                barraVidaFill.color = Color.green;
            else if (porcentajeVida > 0.25f)
                barraVidaFill.color = Color.yellow;
            else
                barraVidaFill.color = Color.red;
        }
    }
    
    void LateUpdate()
    {
        // Hacer que la barra de vida siempre mire hacia la cámara
        if (mainCamera != null && canvasBarraVida != null)
        {
            canvasBarraVida.transform.LookAt(canvasBarraVida.transform.position + mainCamera.transform.rotation * Vector3.forward,
                                           mainCamera.transform.rotation * Vector3.up);
        }
    }
}

