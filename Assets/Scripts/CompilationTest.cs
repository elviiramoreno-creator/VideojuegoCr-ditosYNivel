// Este archivo es solo para verificar que todos los scripts compilen correctamente
// Puede eliminarse después

using UnityEngine;

public class CompilationTest : MonoBehaviour
{
    void Test()
    {
        // Verificar que los managers existen y se pueden referenciar
        GameManager gm = GameManager.Instance;
        
        // Verificar WinManager
        WinManager wm = FindFirstObjectByType<WinManager>();
        if (wm != null)
        {
            wm.ShowWin();
            wm.ExitToMenu();
            wm.ContinueToNextLevel();
        }
        
        // Verificar GameOverManager
        GameOverManager gom = FindFirstObjectByType<GameOverManager>();
        if (gom != null)
        {
            gom.ShowGameOver();
            gom.RestartGame();
            gom.ExitToMenu();
        }
        
        // Verificar GameManager
        if (gm != null)
        {
            gm.RecogerMoneda(1);
            gm.EliminarEnemigo();
            gm.GameOver();
            gm.MostrarVictoria();
            gm.ReiniciarNivel();
            
            // Verificar getters
            int monedas = gm.GetMonedasRecogidas();
            int enemigos = gm.GetEnemigosEliminados();
            int totalMonedas = gm.GetTotalMonedas();
            int totalEnemigos = gm.GetTotalEnemigos();
        }
        
        Debug.Log("✅ Todos los scripts compilan correctamente!");
    }
}
