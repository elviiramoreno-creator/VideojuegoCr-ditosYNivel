# Guía de Configuración de Animaciones Player (Blend Tree)

Sigue estos pasos EXACTOS para configurar tus animaciones de movimiento (WASD) e Idle.

## 1. Configuración de los Archivos de Animación
Antes de tocar el Animator, asegúrate de que tus animaciones están listas para Pixel Art.
1. Ve a la carpeta de tus animaciones.
2. Selecciona tus 4 animaciones de andar (`Walk_W`, `Walk_S`, `Walk_A`, `Walk_D`).
3. En el Inspector:
   - Marca la casilla **Loop Time**.
4. (Opcional) Si van muy rápido:
   - Abre la ventana **Animation**.
   - Selecciona cada animación y cambia el valor **Samples** a **12** (o 8 para más lento).

## 2. Parámetros del Animator
1. Abre la ventana **Animator**.
2. Ve a la pestaña **Parameters**.
3. Asegúrate de tener creados estos 3 parámetros (tipo Float):
   - `MovimientoX`
   - `MovimientoY`
   - `Velocidad`
   *(Respeta las mayúsculas)*.

## 3. Crear el Blend Tree de Movimiento
1. En el gráfico del Animator (Base Layer), haz **Clic Derecho -> Create State -> From New Blend Tree**.
2. Llámalo `Caminar` (cámbiale el nombre arriba en el Inspector).
3. Haz **Doble Clic** en la caja `Caminar` para entrar.
4. Haz clic en el nodo gris **"Blend Tree"**.
5. En el Inspector:
   - **Blend Type**: Selecciona `2D Simple Directional`.
   - **Parameters**: 
     - Primer hueco: `MovimientoX`
     - Segundo hueco: `MovimientoY`
6. En la lista **Motion**, pulsa el `+` -> **Add Motion Field** (4 veces).
7. Configura las 4 animaciones así:

| Motion (Animación) | Pos X | Pos Y | Dirección |
| :--- | :---: | :---: | :--- |
| **Walk_D** (Derecha) | 1 | 0 | Derecha (D) |
| **Walk_A** (Izquierda) | -1 | 0 | Izquierda (A) |
| **Walk_W** (Arriba) | 0 | 1 | Arriba (W) |
| **Walk_S** (Abajo) | 0 | -1 | Abajo (S) |

*(Nota: Asegúrate de que NO sean valores como 0.1 o 20. Tienen que ser 1, -1 y 0 exactos).*

## 4. Configurar las Transiciones (Flechas)
Vuelve a la **Base Layer** (haz clic en "Base Layer" arriba a la izquierda del gráfico).

### De `Idle` a `Caminar`
1. Haz Clic Derecho en **Idle** -> **Make Transition** -> Clic en **Caminar**.
2. Selecciona la flecha y configura en el Inspector:
   - **Has Exit Time**: [ ] (DESMARCADO)
   - **Settings** (Despliégalos):
     - **Transition Duration (s)**: `0`
     - **Interruption Source**: `Current State`
   - **Conditions**:
     - `Velocidad` -> `Greater` -> `0.01`

### De `Caminar` a `Idle`
1. Haz Clic Derecho en **Caminar** -> **Make Transition** -> Clic en **Idle**.
2. Selecciona la flecha y configura:
   - **Has Exit Time**: [ ] (DESMARCADO)
   - **Settings**:
     - **Transition Duration (s)**: `0`
   - **Conditions**:
     - `Velocidad` -> `Less` -> `0.01`

---
**¡Listo!** Dale a Play y debería funcionar perfecto.
Si el personaje se desliza pero no mueve las piernas, revisa el paso 1 (Loop Time).
