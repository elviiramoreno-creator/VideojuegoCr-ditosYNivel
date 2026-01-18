using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_System : MonoBehaviour
{
    public void CargarEscena(string Nivel)
    {
        SceneManager.LoadScene("Nivel");
    }
    public void SalirJuego()

    {
        Application.Quit();
    }
}