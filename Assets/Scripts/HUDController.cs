using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Configuración Contador (Oro)")]
    [Tooltip("Arrastra aquí el objeto de texto del Canvas que muestra el número.")]
    [SerializeField] private TextMeshProUGUI textoContadorMonedas;
    
    [Tooltip("Arrastra aquí la IMAGEN del marco o icono que está junto al texto (si hay una).")]
    [SerializeField] private Image imagenMarcoContador;
    
    [Tooltip("Pon aquí el Sprite (dibujo) que quieres que tenga esa imagen.")]
    [SerializeField] private Sprite spriteMarcoContador;

    [Header("Configuración Vidas")]
    [SerializeField] private GameObject[] corazones; 

    [Header("Opciones de Debug (Manual)")]
    [Tooltip("Pon un número aquí para forzar el contador a este valor inicial (ignora el conteo real). Dejar en 0 para automático.")]
    public int monedasInicialesManuales = 0;

    [Header("Referencias Automáticas")]
    [SerializeField] private PlayerController playerController;

    void Awake()
    {
        // 1. Configurar la Imagen del Marco (si el usuario ha asignado las variables)
        if (imagenMarcoContador != null && spriteMarcoContador != null)
        {
            imagenMarcoContador.sprite = spriteMarcoContador;
        }

        // 2. Auto-conexión INTELIGENTE del Texto
        if (textoContadorMonedas == null)
        {
            // Buscamos por nombre habitual
            GameObject obj = GameObject.Find("contadorMonedas");
            if (obj == null) obj = GameObject.Find("ContadorMonedas");
            if (obj == null) obj = GameObject.Find("TextoMonedas");
            if (obj == null) obj = GameObject.Find("MonedasText");

            if (obj != null)
            {
                textoContadorMonedas = obj.GetComponent<TextMeshProUGUI>();
            }
            // Si sigue sin encontrarlo, busca en los hijos de este mismo objeto (el Canvas o panel donde esté el script)
            if (textoContadorMonedas == null)
            {
                textoContadorMonedas = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // Validación final para ayudar al usuario
        if (textoContadorMonedas == null)
        {
            Debug.LogError("ERROR CRÍTICO: El HUDController no encuentra el Texto del contador. " +
                           "Por favor, arrastra manualmante el objeto 'TextMeshPro - Text (UI)' al campo 'Texto Contador Monedas'.");
        }
        else
        {
            // Poner un texto por defecto para verificar que funciona
            textoContadorMonedas.text = "-";
        }
    }

    void Start()
    {
        // Buscar al Player
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        // Inicializar vidas
        if (playerController != null)
        {
            ActualizarVidas(playerController.GetVidasActuales());
        }
    }

    void Update()
    {
        // Re-conectar player si se pierde (ej. al morir y reaparecer)
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController != null)
                ActualizarVidas(playerController.GetVidasActuales());
        }
    }

    // --------------------------------------------------------------------------
    // MÉTODOS PÚBLICOS
    // --------------------------------------------------------------------------

    public void ActualizarContadorMonedas(int monedasRestantes)
    {
        if (textoContadorMonedas != null)
        {
            textoContadorMonedas.text = monedasRestantes.ToString();
            // Fuerza a que el sistema de UI refresque el cambio inmediatamente
            textoContadorMonedas.ForceMeshUpdate(); 
        }
    }

    public void ActualizarVidas(int vidasActuales)
    {
        if (corazones == null) return;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] != null)
            {
                corazones[i].SetActive(i < vidasActuales);
            }
        }
    }
}
