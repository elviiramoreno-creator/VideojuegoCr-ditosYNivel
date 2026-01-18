# 📋 INSTRUCCIONES - Configuración de Paneles Game Over y Win

## ✅ Scripts Creados:
1. **WinManager.cs** - Maneja el panel de victoria
2. **GameOverManager.cs** - Ya existía, mejorado
3. **PanelButtonConfigurator.cs** - Configura botones automáticamente
4. **GameManager.cs** - Actualizado para detectar victoria/derrota

---

## 🎮 PASO 1: Configurar Panel_GameOver

### En Unity Editor:

1. **Busca el objeto `Panel_GameOver` en tu escena**
   - Está dentro del Canvas

2. **Añade el componente `GameOverManager`:**
   - Selecciona `Panel_GameOver`
   - Click en "Add Component"
   - Busca "GameOverManager" y añádelo

3. **Configura GameOverManager:**
   - **Game Over Panel**: Arrastra `Panel_GameOver` aquí
   - **Restart Button**: Arrastra el botón de reiniciar
   - **Exit Button**: Arrastra el botón Exit/Salir
   - **Menu Scene Name**: Escribe "Menu" (nombre de tu escena de menú)

4. **OPCIONAL - Auto-configuración de botones:**
   - Añade también el componente `PanelButtonConfigurator` al `Panel_GameOver`
   - Marca el checkbox **Es Game Over Panel**
   - Este script configurará automáticamente los botones por nombre

5. **Asegúrate que el panel esté DESACTIVADO al inicio:**
   - Desmarca el checkbox al lado del nombre `Panel_GameOver` en el Inspector

---

## 🏆 PASO 2: Configurar Panel_Win

### En Unity Editor:

1. **Busca el objeto `Panel_Win` en tu escena**
   - Está dentro del Canvas

2. **Añade el componente `WinManager`:**
   - Selecciona `Panel_Win`
   - Click en "Add Component"
   - Busca "WinManager" y añádelo

3. **Configura WinManager:**
   - **Win Panel**: Arrastra `Panel_Win` aquí
   - **Exit Button**: Arrastra el botón Exit/Salir
   - **Continue Button** (opcional): Si tienes botón de continuar al siguiente nivel
   - **Menu Scene Name**: Escribe "Menu"

4. **OPCIONAL - Auto-configuración de botones:**
   - Añade también el componente `PanelButtonConfigurator` al `Panel_Win`
   - Marca el checkbox **Es Win Panel**

5. **Asegúrate que el panel esté DESACTIVADO al inicio:**
   - Desmarca el checkbox al lado del nombre `Panel_Win` en el Inspector

---

## ⚙️ PASO 3: Configurar GameManager

### En Unity Editor:

1. **Busca el objeto `GameManager` en tu jerarquía**
   - Si no existe, créalo: GameObject → Create Empty → Nómbralo "GameManager"

2. **Añade el componente `GameManager` si no lo tiene**

3. **Configura las Referencias:**
   - **Game Over Manager**: Arrastra el objeto que tiene GameOverManager
   - **Win Manager**: Arrastra el objeto que tiene WinManager

4. **Configurar Objetivos del Nivel:**
   - **Enemigos A Eliminar**: Deja en **0** para contar automáticamente
   - **Monedas A Recoger**: Deja en **0** para contar automáticamente
   
   *Si dejas en 0, el sistema contará automáticamente todos los enemigos y monedas al inicio*

---

## 🎯 CÓMO FUNCIONA:

### ❌ Game Over (cuando el jugador muere):
1. Player vida llega a 0
2. `PlayerController` llama a `GameManager.GameOver()`
3. `GameManager` muestra `Panel_GameOver`
4. El juego se PAUSA (Time.timeScale = 0)
5. Botones disponibles:
   - **Restart**: Reinicia el nivel actual
   - **Exit**: Vuelve al menú principal

### ✅ Victoria (cuando se completa el nivel):
1. Jugador recoge la ÚLTIMA moneda Y elimina el ÚLTIMO enemigo
2. `GameManager.VerificarVictoria()` detecta que se cumplieron todos los objetivos
3. `GameManager` muestra `Panel_Win`
4. El juego se PAUSA (Time.timeScale = 0)
5. Botones disponibles:
   - **Continue**: Va al siguiente nivel
   - **Exit**: Vuelve al menú principal

---

## 🔧 SOLUCIÓN DE PROBLEMAS:

**❓ El panel no aparece:**
- Asegúrate que el panel está en el Canvas
- Verifica que el panel está desactivado al inicio
- Revisa la consola para ver logs de "Mostrando Game Over" o "VICTORIA!"

**❓ Los botones no funcionan:**
- Añade el componente `PanelButtonConfigurator` al panel
- Asegúrate que los botones tengan un nombre que contenga: "exit", "restart", "continue"
- Verifica que los botones tienen el componente `Button` de Unity

**❓ La victoria no se detecta:**
- Abre la consola y mira cuántos enemigos/monedas se detectaron al inicio
- Verifica que los enemigos tengan el tag "Enemy"
- Verifica que las monedas tengan el componente `Coin`
- Chequea que `EnemyController.Morir()` se llama cuando un enemigo muere

---

## 📝 NOTAS IMPORTANTES:

- Los paneles DEBEN estar desactivados al inicio de la escena
- La escena "Menu" debe existir y estar en Build Settings
- El juego se pausa automáticamente al mostrar los paneles
- El tiempo se restaura al reiniciar o salir

---

## 🎨 PERSONALIZACIÓN:

Si quieres personalizar los mensajes o comportamientos:
- **Game Over**: Edita `GameOverManager.cs` línea 45-55
- **Victoria**: Edita `WinManager.cs` línea 43-66
- **Objetivos**: Cambia los valores en GameManager Inspector

¡Listo! Todo debería funcionar automáticamente. 🎮
