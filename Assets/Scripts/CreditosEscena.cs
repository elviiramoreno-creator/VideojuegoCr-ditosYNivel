using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditosEscena : MonoBehaviour
{
    public void IrACreditos()
    {
        Debug.Log("Botón Créditos presionado");
        SceneManager.LoadScene("Creditos"); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            VolverAlMenu();
        }
    }

    /// <summary>
    /// Función para asignar al botón Exit en la escena de Créditos
    /// </summary>
    public void VolverAlMenu()
    {
        Debug.Log("Volviendo al menú principal...");
        SceneManager.LoadScene("Menu");
    }
}
