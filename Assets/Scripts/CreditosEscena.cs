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
            Debug.Log("Volviendo al menú principal...");
            SceneManager.LoadScene("Menu");
        }
    }
}
