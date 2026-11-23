# GameMiners — Decisión de arquitectura: cámaras y movimiento
> ADR-CAM-001 | 23-11-2026 | Acordada por Matías + Nacho | Aplica al juego definitivo La heladería maldita.

## 1. Decisión vinculante
**La tienda y la bodega se juegan en POV / primera persona. Después del secuestro en el refrigerador, todo el mundo del Jety se juega en tercera persona, con cámara lateral elevada / en tres cuartos que muestra al personaje completo.** La referencia de encuadre y sensación es *Fairytale Fights* (Playlogic, 2009): videojuego 3D de acción con presentación lateral/cuasi-isométrica. No presupone un plataformas 2D de desplazamiento horizontal exclusivamente. La cinematografía final puede usar una cámara separada.

### Alcance exacto
| Lugar/fase | Perspectiva | Movimiento / orientación | Propiedad |
| --- | --- | --- | --- |
| 00_Menu | UI; sin cámara jugable obligatoria | Ratón para botones | Matías UI; Nacho integra escena |
| 01_Shop — mostrador y bodega | POV 3D en primera persona | WASD, mirar con mouse, E para servir/puerta/frigorífico | Nacho controlador+escena; Matías instrucciones |
| Refrigerador/captura | Cámara tienda + fundido/captura breve | Bloquear entrada durante transición | Nacho |
| 02_IceWorld — pequeñas, grandes, jefe helado | Tercera persona **lateral elevada / tres cuartos**, cámara de seguimiento estable, personaje visible | WASD relativo a ejes proyectados de cámara sobre suelo 3D, ataque según orientación del personaje; sin mirar con mouse | Nacho |
| 03_JetyRoom — Jety | Misma perspectiva y controlador externo de combate | Mismas reglas del nivel anterior | Nacho |
| Final — trono | Plano cinematográfico opcional | Deshabilitar control hasta terminar escena | Nacho; Matías panel final |

## 2. Interpretación de la referencia
**Referencia artística/cinematográfica**, no implementación copiada. *Fairytale Fights* utiliza personajes y entornos 3D vistos desde cámara elevada, lateral/semi-isométrica. Se busca ver la protagonista, enemigos y suelo en pantalla durante el combate. No copiar assets, código, personajes, UI ni marcas del juego de referencia. La referencia no obliga a habilitar multijugador, saltos, física avanzada o cámaras móviles.

**Por validar en Unity con un prototipo de dos cubos**: elevación de la cámara, distancia, perspectiva/ortográfica, amplitud de seguimiento, profundidad recorrible, oclusiones y comportamiento cerca de bordes. Si el equipo decide restringir movimiento a un plano 2.5D, debe documentarlo explícitamente tras prueba visual; no asumirlo ahora.

## 3. Arquitectura mínima sugerida
- Dos **modos de entrada y cámara por escena**, no una única cámara FPS reutilizada para el combate:
  - Shop: `ShopFirstPersonController` + `ShopPovCamera` (nombres de referencia, no APIs ya existentes).
  - IceWorld/JetyRoom: `WorldThirdPersonController` + `WorldSideCameraRig` (nombres de referencia, no APIs ya existentes).
- Mantener **una sola lógica de progresión compartida** para pasar de tienda→mundo→sala final y valores de estado pertinentes. No introducir una abstracción de cámaras que complique el M1.
- Al interactuar con refrigerador: bloquear entrada → efecto corto → cargar `02_IceWorld` → spawn del personaje visible → habilitar control y cámara lateral.
- El combate debe calcular dirección de golpe/lanzamiento de lanza a partir del **facing** del personaje en el mundo, no del centro de una retícula FPS.
- El movimiento debe ser relativo a cámara en suelo 3D, con orientación del avatar hacia desplazamiento/objetivo; los ejes concretos y el control de profundidad se acuerdan tras prototipo.
- Reusar rig/controller de combate tanto en `02_IceWorld` como `03_JetyRoom`; no crear un tercer controlador.
- Nacho controla todos los prefabs de cámaras, controladores y montaje de escenas en `Assets/Gameplay/**`; Matías **no** editará esas escenas/archivos.
- Matías crea prefabs UI para dos contextos: indicaciones de tienda FPS, HUD de batalla externa y mensajes de interacción. El panel de controles debe cambiar los textos según modo.
- Cami planifica mallas, rig y animación de la protagonista para **verse completa** en combate (silueta legible en lateral/tres cuartos); puede necesitar solo manos o vista subjetiva simplificada en tienda. Separar gráficos de física/IA para que modelos se sustituyan sin recodificar.

## 4. Contrato UI–gameplay (sin inventar firmas de APIs)
**Datos mínimos**: `context` = Shop / Combat / Cinematic; `objective` / `interactionHint`; vida/oleada/boss/arma/bombas cuando exista. Nacho es fuente de verdad; Matías solo representa datos. Los nombres y firmas concretas se pactan antes de crear scripts compartidos. No usar eventos de cámara como sustitutos del estado de progresión.

**Acciones UI**: Jugar, Pausar, Reintentar, Volver al menú. No agregar botones para cambiar manualmente POV y combate; el cambio ocurre narrativamente al ser capturada.

## 5. Pruebas de aceptación (M1)
1. Se inicia en menú y tienda en **POV**; mouse mueve solo la mirada, E sirve y abre refrigerador.
2. La captura cambia la escena a `02_IceWorld` sin requerir intervención externa.
3. Se ve el cuerpo completo de la protagonista en combate con cámara lateral elevada / tres cuartos.
4. WASD y ataque funcionan con orientación coherente; el mouse no gobierna la mirada en combate.
5. La cámara acompaña sin dejar al personaje ni enemigos inmediatos fuera de plano; no atraviesa paredes de manera claramente molesta.
6. El paso a `03_JetyRoom` conserva el modo externo de cámara y el control sin regresión a FPS.
7. El jefe final derrota→secuencia de trono cambia a modo cinemático y después permite volver al menú.
8. Se prueba en Play Mode **y** con un build externo. No declarar validado sin prueba ejecutada.

## 6. Riesgos de integración y evitarlos
- **Riesgo:** Work de Nacho usa el viejo prompt “mismo controlador POV”. **Mitigación:** releer GDD + IMPLEMENTATION + WORK_PROMPTS actualizados y reemplazar esa instrucción por esta ADR.
- **Riesgo:** Matías crea retícula/controles FPS para todo. **Mitigación:** separar HUD/prompts de tienda y combate; retícula no obligatoria en vista externa.
- **Riesgo:** Cami modela solamente brazos/arma sin avatar. **Mitigación:** reservar protagonista 3D visible para mundo Jety.
- **Riesgo:** "vista lateral" se interpreta como 2D plano. **Mitigación:** prototipo técnico en 3D con profundidad y captura visual aprobada por equipo antes de congelar ejes.
- **Riesgo:** desarrollo simultáneo sobre escena principal. **Mitigación:** Nacho compone escenas; Matías y Cami entregan prefabs/assets separados por ramas.

## 7. Registro de decisión
- **Decisión original reemplazada:** cámara POV durante todo el juego.
- **Nueva decisión:** POV tienda/bodega → cámara 3D lateral elevada / tres cuartos mundo helados + Jety.
- **Autorizada por:** Matías y Nacho (comunicado en conversación).
- **Cambios pendientes:** implementación real y pruebas en Unity; este archivo **solo documenta** la decisión.
