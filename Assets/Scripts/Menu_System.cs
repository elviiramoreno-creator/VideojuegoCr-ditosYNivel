using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (panelElegirNivel != null) panelElegirNivel.SetActive(false);
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
    /// Cierra el juego
    /// </summary>
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}