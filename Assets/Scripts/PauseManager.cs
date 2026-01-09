using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject menuPausa;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("pulse escape");
            DesactivarMenu();
        }
    }

    public void DesactivarMenu()
    {
        menuPausa.SetActive(!menuPausa.activeSelf);
        Time.timeScale = menuPausa.activeSelf ? 0 : 1;
    }

    public void ContinuarJuego()
    {
        DesactivarMenu();
    }

    public void VolverMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu"); 
    }
}

