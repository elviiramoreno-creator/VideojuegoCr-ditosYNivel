using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para Button

public class Menu_System : MonoBehaviour
{
    [Header("Paneles UI")]
    [Tooltip("Arrastra aquí el objeto Panel_MenuPrincipal")]
    [SerializeField] private GameObject panelMenuPrincipal;
    
    [Tooltip("Arrastra aquí el objeto Panel_ElegirNivel")]
    [SerializeField] private GameObject panelElegirNivel;

    void Start()
    {
        // Asegurarse de empezar estado correcto
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
        if (panelElegirNivel != null) 
        {
            panelElegirNivel.SetActive(false);
            
            // Intentar configurar automáticamente el botón Level 2
            ConfigurarBotonNivel2();
        }
    }

    void ConfigurarBotonNivel2()
    {
        if (panelElegirNivel == null) return;

        // Buscar el botón por nombre "Level 2" dentro del panel
        Button btnLevel2 = null;
        Transform[] hijos = panelElegirNivel.GetComponentsInChildren<Transform>(true);
        
        foreach(Transform hijo in hijos)
        {
            if (hijo.name == "Level 2" || hijo.name == "BotonNivel2")
            {
                btnLevel2 = hijo.GetComponent<Button>();
                break;
            }
        }

        if (btnLevel2 != null)
        {
            // Añadir el evento si lo encontramos
            btnLevel2.onClick.RemoveListener(CargarNivel2); // Evitar duplicados
            btnLevel2.onClick.AddListener(CargarNivel2);
            Debug.Log("Menu_System: Botón 'Level 2' configurado automáticamente.");
        }
    }

    /// <summary>
    /// Activa el panel de elegir nivel y desactiva el menú principal
    /// </summary>
    public void AbrirSelectorNiveles()
    {
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(false);
        if (panelElegirNivel != null) panelElegirNivel.SetActive(true);
    }

    /// <summary>
    /// Vuelve al menú principal desde el selector
    /// </summary>
    public void VolverMenuPrincipal()
    {
        if (panelElegirNivel != null) panelElegirNivel.SetActive(false);
        if (panelMenuPrincipal != null) panelMenuPrincipal.SetActive(true);
    }

    /// <summary>
    /// Carga la escena de créditos
    /// </summary>
    public void IrACreditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    /// <summary>
    /// Carga el Nivel 1
    /// </summary>
    public void CargarNivel1()
    {
        SceneManager.LoadScene("Nivel 1"); 
    }

    /// <summary>
    /// Carga el Nivel 2
    /// </summary>
    public void CargarNivel2()
    {
        SceneManager.LoadScene("Nivel 2"); 
    }

    /// <summary>
    /// Cierra el juego
    /// </summary>
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}