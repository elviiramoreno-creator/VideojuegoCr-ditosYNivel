# Guía de Configuración Paso a Paso - Unity

## 📋 Índice
1. [Configuración del Player](#configuración-del-player)
2. [Configuración de la Linterna](#configuración-de-la-linterna)
3. [Configuración del Enemy](#configuración-del-enemy)
4. [Configuración de la Cámara](#configuración-de-la-cámara)
5. [Explicación de Variables Importantes](#explicación-de-variables-importantes)

---

## 🎮 Configuración del Player

### Paso 1: Configurar el GameObject del Player
1. **Selecciona el GameObject del Player** en la escena "Nivel"
2. **Asegúrate de que tenga el Tag "Player"**:
   - En el Inspector, busca el campo "Tag"
   - Si no existe, crea uno nuevo: Tags & Layers → Add Tag → "Player"

### Paso 2: Configurar Componentes del Player
1. **Rigidbody2D**:
   - Debe estar presente en el Player
   - Configuración recomendada:
     - Body Type: **Kinematic**
     - Gravity Scale: **0**
     - Constraints: Freeze Rotation (Z)

2. **Collider2D** (CapsuleCollider2D o BoxCollider2D):
   - Debe estar presente para las colisiones
   - **NO debe ser Trigger** (debe estar desmarcado)

3. **Animator** (opcional):
   - Si tienes animaciones, asigna el Animator Controller correspondiente

4. **PlayerController Script**:
   - Debe estar adjunto al GameObject del Player
   - En el Inspector, configura:
     - **Velocidad Movimiento**: 5 (velocidad constante del player)
     - **Vidas Máximas**: 3
     - **Tiempo Invencibilidad**: 1 segundo (tiempo después de recibir daño)
     - **Multiplicador Hielo**: 1.4 (aumenta velocidad en hielo)
     - **Multiplicador Enredadera**: 0.5 (reduce velocidad en enredaderas)
     - **Animator**: Arrastra el componente Animator del Player
     - **Rb**: Arrastra el componente Rigidbody2D del Player
     - **Linterna**: Arrastra el GameObject de la linterna (ver siguiente sección)

5. **TilemapDetector Script**:
   - Debe estar adjunto al GameObject del Player
   - En el Inspector, configura:
     - **Tilemap Suelo**: Arrastra el Tilemap de suelo
     - **Tilemap Hielo**: Arrastra el Tilemap de hielo (opcional)
     - **Tilemap Enredadera**: Arrastra el Tilemap de enredaderas (opcional)
     - **Player Controller**: Arrastra el componente PlayerController del Player

### Paso 3: Verificar Movimiento WASD
- El movimiento ya está configurado en el código
- Usa las teclas **W, A, S, D** o las **flechas del teclado**
- El movimiento es fluido gracias al uso de `Input.GetAxis` y `rb.MovePosition`

---

## 🔦 Configuración de la Linterna

### Paso 1: Crear/Configurar el GameObject de la Linterna
1. **Crea un GameObject hijo del Player** llamado "Linterna" (o usa el existente)
2. **Posiciona la linterna** relativo al Player (ejemplo: offset en X=0.5, Y=0)

### Paso 2: Configurar Componentes de la Linterna
1. **Light2D (Universal Render Pipeline)**:
   - Agrega el componente **Light2D** (si usas URP)
   - Tipo: **Spot**
   - Configura el ángulo y rango según tu diseño
   - **Importante**: Asegúrate de que sea un Spot Light

2. **Collider2D** (CircleCollider2D o BoxCollider2D):
   - Agrega un **Collider2D** a la linterna
   - **DEBE ser Trigger** (marca la casilla "Is Trigger")
   - Este collider define el área donde la linterna puede dañar enemigos
   - Ajusta el tamaño para que coincida con el área de luz

3. **LinternaController Script**:
   - Agrega el script **LinternaController** a la linterna
   - En el Inspector, configura:
     - **Player**: Arrastra el Transform del Player
     - **Main Camera**: Arrastra la cámara principal (o déjalo vacío para buscar automáticamente)
     - **Distancia Máxima**: 3 (distancia desde el player a la linterna)
     - **Velocidad Rotación**: 10 (suavidad de rotación hacia el mouse)
     - **Daño Por Segundo**: 25 (daño que causa la linterna)
     - **Tiempo Entre Daños**: 1 segundo (tiempo entre cada aplicación de daño)

### Paso 3: Configurar Rotación con el Ratón
- La rotación ya está configurada en el código
- La linterna seguirá automáticamente la posición del ratón
- El movimiento es suave gracias a `Quaternion.Slerp`

---

## 👾 Configuración del Enemy

### Paso 1: Configurar el GameObject del Enemy
1. **Selecciona el GameObject del Enemy** en la escena "Nivel"
2. **Asegúrate de que tenga el Tag "Enemy"**:
   - En el Inspector, busca el campo "Tag"
   - Si no existe, créalo: Tags & Layers → Add Tag → "Enemy"

### Paso 2: Configurar Colliders del Enemy
**IMPORTANTE**: El enemigo necesita **DOS colliders**:

#### Collider 1: Collider del Cuerpo (para recibir daño del player)
1. **Agrega un Collider2D** al GameObject principal del Enemy (BoxCollider2D, CircleCollider2D, o CapsuleCollider2D)
2. **Configuración**:
   - **Tamaño**: Del tamaño del cuerpo del enemigo
   - **Is Trigger**: **Puede ser Trigger o NO Trigger**
   - **Purpose**: Este collider recibe daño cuando la linterna lo toca
   - **Importante**: Este collider debe estar en el mismo GameObject que el EnemyController, o en un GameObject hijo

#### Collider 2: Collider de Detección (para detectar al player) - RECOMENDADO
1. **Crea un GameObject hijo** del Enemy llamado "DetectionZone" (o el nombre que prefieras)
2. **Agrega un Collider2D** a este GameObject hijo
3. **Agrega el script EnemyDetectionZone** a este GameObject hijo
4. **Configuración**:
   - **Tamaño**: Más grande que el collider del cuerpo (ejemplo: 2-3x más grande)
   - **Is Trigger**: **MARCADO** (SÍ es trigger) - El script lo configurará automáticamente si se olvida
   - **Purpose**: Este collider detecta cuando el player entra en su rango (el enemigo empieza a perseguir)
   - **EnemyDetectionZone Script**: Se comunica automáticamente con el EnemyController del padre

**Alternativa (más simple)**: Si prefieres tener ambos colliders en el mismo GameObject:
- El script EnemyController intentará diferenciarlos automáticamente
- El más pequeño será el collider del cuerpo
- El más grande será el collider de detección (debe ser trigger)

### Paso 3: Configurar Componentes del Enemy
1. **Rigidbody2D**:
   - Debe estar presente
   - Configuración:
     - Body Type: **Kinematic**
     - Gravity Scale: **0**
     - Constraints: Freeze Rotation (Z)

2. **SpriteRenderer** (opcional):
   - Para la orientación visual del enemigo

3. **Animator** (opcional):
   - Si tienes animaciones del enemigo

4. **EnemyController Script**:
   - Agrega el script **EnemyController** al GameObject principal del Enemy
   - En el Inspector, configura:
     - **Velocidad Patrullaje**: 3 (velocidad al patrullar)
     - **Límite Izquierdo**: -5 (límite izquierdo de patrullaje)
     - **Límite Derecho**: 5 (límite derecho de patrullaje)
     - **Velocidad Persecución**: 4 (velocidad al perseguir al player)
     - **Distancia Persecución**: 5 (distancia para empezar a perseguir - respaldo si no hay collider de detección)
     - **Vida Máxima**: 100
     - **Barra Vida Prefab**: (opcional) Prefab de la barra de vida
     - **Offset Barra Vida**: (0, 1.5, 0) - Posición relativa de la barra
     - **Collider Cuerpo**: Arrastra el collider del cuerpo (opcional - el script lo encuentra automáticamente)
     - **Collider Detección**: Arrastra el collider de detección (opcional - se usa EnemyDetectionZone si está en un hijo)
     - **Distancia Ataque**: 2.5 (distancia para activar animación de ataque)
     - **Invertir Orientación**: true (si el sprite se voltea de forma especial)
     - **Rb**: Arrastra el Rigidbody2D del Enemy
     - **Sprite Renderer**: Arrastra el SpriteRenderer (opcional)
     - **Animator**: Arrastra el Animator (opcional)

5. **EnemyDetectionZone Script** (si usas GameObject hijo para detección):
   - Agrega el script **EnemyDetectionZone** al GameObject hijo "DetectionZone"
   - **No necesita configuración** - busca automáticamente el EnemyController en el padre
   - Asegúrate de que el collider en este GameObject esté marcado como Trigger

### Paso 4: Configurar Barra de Vida del Enemy
1. **Opción 1: Usar Prefab**:
   - Crea un prefab de barra de vida (GameObject con Canvas, Image, etc.)
   - Asigna el prefab en "Barra Vida Prefab" del EnemyController
   - El prefab debe tener el script **HealthBarController**

2. **Opción 2: Creación Automática**:
   - Si no asignas un prefab, el script creará automáticamente una barra de vida simple
   - La barra se creará al iniciar el juego

### Paso 5: Configurar Patrullaje
- El enemigo patrullará automáticamente entre los límites izquierdo y derecho
- Cambiará de dirección al llegar a los límites
- El patrullaje se detendrá cuando detecte al player

---

## 📷 Configuración de la Cámara

### Paso 1: Configurar el GameObject de la Cámara
1. **Selecciona la Cámara Principal** en la escena
2. **Agrega el script CameraFollow** a la cámara

### Paso 2: Configurar CameraFollow
En el Inspector del script CameraFollow:
- **Target Player**: Arrastra el Transform del Player
- **Main Camera**: Arrastra la cámara principal (o déjalo vacío)
- **Offset**: (0, 0, -10) - Offset de la cámara respecto al player
- **Smooth Speed**: 5 - Velocidad de suavizado del seguimiento

### Paso 3: Verificar Funcionamiento
- La cámara seguirá automáticamente al player
- Si tienes Cinemachine instalado, el script intentará usarlo automáticamente
- Si no, usará seguimiento simple con suavizado

---

## 📚 Explicación de Variables Importantes

### Multiplicadores por Tilemap (PlayerController)

Estas variables modifican la velocidad del player según el tipo de suelo que está pisando:

#### `multiplicadorHielo` (valor por defecto: 1.4)
- **Propósito**: Aumenta la velocidad cuando el player pisa hielo
- **Cómo funciona**:
  - Si el valor es **mayor que 1**, el player se moverá **más rápido**
  - Ejemplo: Si velocidadMovimiento = 5 y multiplicadorHielo = 1.4
    - Velocidad en hielo = 5 × 1.4 = **7 unidades/segundo**
- **Uso**: Simula que el hielo es resbaladizo y permite moverse más rápido

#### `multiplicadorEnredadera` (valor por defecto: 0.5)
- **Propósito**: Reduce la velocidad cuando el player pisa enredaderas
- **Cómo funciona**:
  - Si el valor es **menor que 1**, el player se moverá **más lento**
  - Ejemplo: Si velocidadMovimiento = 5 y multiplicadorEnredadera = 0.5
    - Velocidad en enredaderas = 5 × 0.5 = **2.5 unidades/segundo**
- **Uso**: Simula que las enredaderas dificultan el movimiento

#### ¿Cómo se detecta el tilemap?
- El script **TilemapDetector** detecta en qué tilemap está el player
- Usa `WorldToCell` para convertir la posición del player a coordenadas de celda
- Verifica si hay un tile en esa celda para cada tilemap (Hielo, Enredadera, Suelo)
- El **PlayerController** actualiza la velocidad según el tilemap detectado

#### Importante sobre Velocidad Constante
- El player **siempre mantiene la misma velocidad base** (`velocidadMovimiento`)
- Los multiplicadores solo modifican temporalmente la velocidad según el suelo
- El movimiento diagonal mantiene la misma velocidad gracias a la normalización del vector de dirección

---

## ✅ Checklist de Configuración

### Player
- [ ] Tag "Player" asignado
- [ ] Rigidbody2D configurado (Kinematic, Gravity Scale = 0)
- [ ] Collider2D presente (NO trigger)
- [ ] PlayerController script adjunto y configurado
- [ ] TilemapDetector script adjunto (si hay tilemaps especiales)
- [ ] Referencias asignadas (Animator, Rb, Linterna)

### Linterna
- [ ] GameObject hijo del Player
- [ ] Light2D (Spot) configurado
- [ ] Collider2D configurado como **Trigger**
- [ ] LinternaController script adjunto
- [ ] Referencias asignadas (Player, Main Camera)

### Enemy
- [ ] Tag "Enemy" asignado
- [ ] Rigidbody2D configurado (Kinematic, Gravity Scale = 0)
- [ ] **Collider del cuerpo** presente (NO trigger)
- [ ] **Collider de detección** presente (SÍ trigger, más grande)
- [ ] EnemyController script adjunto y configurado
- [ ] Referencias asignadas (Rb, SpriteRenderer, Animator, Colliders)
- [ ] Límites de patrullaje configurados

### Cámara
- [ ] CameraFollow script adjunto a la cámara
- [ ] Referencias asignadas (Target Player, Main Camera)
- [ ] Offset configurado

### GameManager
- [ ] GameManager presente en la escena (singleton)
- [ ] Referencias asignadas (Player, Meta Nivel)

---

## 🐛 Solución de Problemas

### El player no se mueve
- Verifica que el Rigidbody2D esté configurado como Kinematic
- Verifica que el tag "Player" esté asignado
- Revisa la consola de Unity para errores

### La linterna no daña a los enemigos
- Verifica que el collider de la linterna esté marcado como **Trigger**
- Verifica que los enemigos tengan el tag "Enemy"
- Verifica que los enemigos tengan el script EnemyController

### Los enemigos no persiguen al player
- Verifica que el collider de detección esté marcado como **Trigger**
- Verifica que el collider de detección sea más grande que el del cuerpo
- Verifica que el player tenga el tag "Player"

### El player no recibe daño cuando toca al enemigo
- Verifica que el collider del cuerpo del enemigo **NO** esté marcado como Trigger
- Verifica que el player tenga el script PlayerController
- Verifica que el método `RecibirDano()` esté presente en PlayerController

### La barra de vida no aparece
- Si usas prefab, verifica que tenga el script HealthBarController
- Si no usas prefab, el script creará automáticamente una barra simple
- Verifica que el offset de la barra sea visible (ejemplo: Y = 1.5)

### La cámara no sigue al player
- Verifica que el script CameraFollow esté adjunto a la cámara
- Verifica que Target Player esté asignado
- Si usas Cinemachine, verifica que esté configurado correctamente

---

## 📝 Notas Finales

- Todos los scripts ya están implementados y listos para usar
- El código maneja automáticamente la mayoría de las situaciones
- Si falta alguna referencia, los scripts intentarán encontrarla automáticamente
- Revisa la consola de Unity para mensajes de debug y errores

¡Buena suerte con tu juego! 🎮

