using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private TextMeshProUGUI textoPuntosUI;
    [SerializeField] private TextMeshPro textoPuntos3D;
    [SerializeField] private int puntosPorMoneda = 10;
    [SerializeField] private int puntosPorEsquivarEnemigo = 5;
    
    private int puntosActuales = 0;
    
    void Start()
    {
        // Buscar el TextMeshPro con tag "Puntos" si no está asignado
        BuscarTextoPuntos();
        
        ActualizarUI();
        
        // 🔥 CORREGIDO: Suscribirse correctamente a los eventos
        // Estos eventos ahora existen en el GameManager corregido
    }
    
    void OnEnable()
    {
        // Suscribirse cuando el objeto se active
        if (GameManager.Instance != null)
        {
            // Usar los eventos estáticos del GameManager
            GameManager.OnMonedaRecogida += AgregarPuntosMoneda;
            GameManager.OnEnemigoEsquivado += AgregarPuntosEnemigo;
        }
    }
    
    void OnDisable()
    {
        // Desuscribirse cuando el objeto se desactive
        if (GameManager.Instance != null)
        {
            GameManager.OnMonedaRecogida -= AgregarPuntosMoneda;
            GameManager.OnEnemigoEsquivado -= AgregarPuntosEnemigo;
        }
    }
    
    void BuscarTextoPuntos()
    {
        // Buscar por tag "Puntos"
        GameObject puntosObj = GameObject.FindGameObjectWithTag("Puntos");
        
        if (puntosObj != null)
        {
            // Intentar TextMeshProUGUI primero (UI 2D)
            textoPuntosUI = puntosObj.GetComponent<TextMeshProUGUI>();
            
            // Si no tiene TextMeshProUGUI, buscar TextMeshPro (3D)
            if (textoPuntosUI == null)
            {
                textoPuntos3D = puntosObj.GetComponent<TextMeshPro>();
            }
        }
        
        // Si aún no está asignado, buscar por nombre "Puntos"
        if (textoPuntosUI == null && textoPuntos3D == null)
        {
            puntosObj = GameObject.Find("Puntos");
            if (puntosObj != null)
            {
                textoPuntosUI = puntosObj.GetComponent<TextMeshProUGUI>();
                if (textoPuntosUI == null)
                {
                    textoPuntos3D = puntosObj.GetComponent<TextMeshPro>();
                }
            }
        }
    }
    
    void AgregarPuntosMoneda()
    {
        puntosActuales += puntosPorMoneda;
        ActualizarUI();
    }
    
    void AgregarPuntosEnemigo()
    {
        puntosActuales += puntosPorEsquivarEnemigo;
        ActualizarUI();
    }
    
    void ActualizarUI()
    {
        // Si no tenemos referencias, intentar buscar de nuevo
        if (textoPuntosUI == null && textoPuntos3D == null)
        {
            BuscarTextoPuntos();
        }
        
        string textoPuntos = puntosActuales.ToString();
        
        if (textoPuntosUI != null)
        {
            textoPuntosUI.text = textoPuntos;
        }
        
        if (textoPuntos3D != null)
        {
            textoPuntos3D.text = textoPuntos;
        }
    }
    
    public void ResetearPuntos()
    {
        puntosActuales = 0;
        ActualizarUI();
    }
    
    public int GetPuntos()
    {
        return puntosActuales;
    }
}