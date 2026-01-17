using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Configuración Contador (Oro)")]
    [Tooltip("Arrastra aquí el objeto de texto del Canvas que muestra el número.")]
    [SerializeField] private TextMeshProUGUI textoContadorMonedas;
    
    [Tooltip("Arrastra aquí el SPRITE (dibujo) que quieres ver en el marco.")]
    [SerializeField] private Sprite imagenMarcoContador;

    [Header("Configuración Vidas")]
    [SerializeField] private GameObject[] corazones; 

    [Header("Opciones de Debug (Manual)")]
    [Tooltip("Pon un número aquí para forzar el contador a este valor inicial (ignora el conteo real). Dejar en 0 para automático.")]
    public int monedasInicialesManuales = 0;

    [Header("Referencias Automáticas")]
    [SerializeField] private PlayerController playerController;

    void Awake()
    {
        // 1. Auto-conexión INTELIGENTE del Texto
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
            // Si sigue sin encontrarlo, busca en los hijos de este mismo objeto
            if (textoContadorMonedas == null)
            {
                textoContadorMonedas = GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        // 2. Configurar el Sprite del Marco (Si se ha asignado)
        if (imagenMarcoContador != null)
        {
            // Buscar dónde poner este sprite. Buscamos un objeto Image cercano o por nombre.
            Image targetImage = null;
            
            // Intentar buscar por nombres comunes
            GameObject objImg = GameObject.Find("MarcoContador");
            if (objImg == null) objImg = GameObject.Find("ImagenContador");
            if (objImg == null) objImg = GameObject.Find("IconoMoneda");
            
            if (objImg != null)
            {
                targetImage = objImg.GetComponent<Image>();
            }
            
            // Si tenemos el texto, busquemos una imagen hermana (muy común en UI)
            if (targetImage == null && textoContadorMonedas != null)
            {
                // Buscar en hermanos del texto
                Transform parent = textoContadorMonedas.transform.parent;
                if (parent != null)
                {
                    foreach (Transform child in parent)
                    {
                        // Si no es el texto mismo y tiene imagen
                        if (child != textoContadorMonedas.transform)
                        {
                            Image img = child.GetComponent<Image>();
                            if (img != null)
                            {
                                targetImage = img;
                                break; // Encontramos una imagen candidata
                            }
                        }
                    }
                }
            }

            // Aplicar el sprite si encontramos dónde
            if (targetImage != null)
            {
                targetImage.sprite = imagenMarcoContador;
            }
            else
            {
                Debug.LogWarning("HUDController: Se asignó un Sprite para el marco, pero NO se encontró ningún objeto Image en la escena (llamado 'MarcoContador', 'IconoMoneda', etc) donde ponerlo.");
            }
        }

        // Validación final
        if (textoContadorMonedas == null)
        {
            Debug.LogError("ERROR CRÍTICO: El HUDController no encuentra el Texto del contador.");
        }
        else
        {
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
