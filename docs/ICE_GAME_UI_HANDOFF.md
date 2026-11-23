# UI de M1 · Integración de La heladería maldita

Matías entrega los prefabs en `Assets/UI/Prefabs/` y sus componentes en `Assets/UI/Scripts/`. Nacho compone las escenas, controla la entrada, el cursor y la progresión, y conecta los prefabs al estado real del juego.

## Montaje

Instancia los prefabs como hijos de un Canvas de escena. Usar Screen Space Overlay, un Canvas Scaler con Scale With Screen Size y resolución de referencia de 1600 × 900, un Graphic Raycaster y un EventSystem con Input System UI Input Module para los botones. Coloca los paneles de resultado después del HUD en el orden del Canvas.

| Prefab | Entrada de presentación o acción |
| --- | --- |
| `MainMenu` | Activo en menú. Enlazar `MainMenuView.PlayButton.onClick` con el inicio de la partida. Controles y Volver se resuelven dentro del prefab. Salir está disponible en Windows. |
| `PlayerHud` | En tienda y bodega, `ShowShop(objetivo)` muestra objetivo y retícula. En IceWorld y JetyRoom, `ShowCombat(objetivo, vida, vidaMáxima, actual, total)` muestra vida y progreso. El progreso se oculta si el total es 0. `Hide()` oculta el HUD durante cinemáticas y resultados. |
| `InteractionPrompt` | `Show(tecla, acción)` recibe la tecla y el texto para el objeto que gameplay considera interactuable. `Hide()` al perder el objetivo, aceptar la acción o salir del contexto. Puede usarse en tienda y combate. |
| `BossHealthBar` | `Show(nombre, vida, vidaMáxima)` representa al jefe activo. `Hide()` al terminar el encuentro o dejar la escena. Reutilizable para el helado gigante y el Jety. |
| `DefeatPanel` | `Show()` tras la derrota. Enlazar `RetryButton.onClick` con el reintento y `MenuButton.onClick` con el retorno al menú. |
| `EndingPanel` | `Show()` tras acabar la secuencia del trono. Enlazar `MenuButton.onClick` con el retorno al menú. |

Estos métodos pertenecen a las vistas. El adaptador de Nacho puede llamarlos al recibir el estado inicial y cuando cambien el contexto, el objetivo, la vida, el progreso, el candidato de interacción o el resultado de la partida. Los contadores y la vida se muestran en valores enteros. La UI delega los cambios de escena, el bloqueo de entrada, la decisión de reintento y la aparición de jefes al sistema de gameplay. Si el adaptador añade listeners en `OnEnable`, debe retirar sus propios listeners en `OnDisable`.

## Traspaso para Nacho y Cami

- **Nacho:** usa el POV para heladería y bodega, con WASD, ratón y E. Tras el refrigerador, activa la cámara 3D lateral elevada en tres cuartos para IceWorld y JetyRoom. El HUD de combate omite la retícula FPS. Conecta valores de prueba 1/1 desde gameplay y documenta el lugar de reintento y la señal de trono terminado al integrar.
- **Cami:** para M1, prioriza un helado pequeño o un objeto sencillo que pueda sustituir el modelo provisional sin cambiar scripts. En los niveles de combate la protagonista se ve de cuerpo completo desde fuera, así que revisa su silueta y puestas de cámara en tres cuartos. Nacho valida las escalas, puntos de reemplazo y colisiones en escena.
- La versión común es Unity `6000.4.11f1`, plantilla Universal 3D. La base y la decisión de cámaras están en los [PR #2](https://github.com/maturro0606q/gameminers-hackathon-2026/pull/2) y [PR #1](https://github.com/maturro0606q/gameminers-hackathon-2026/pull/1).

## Pruebas de integración

1. Jugar abre la tienda y sus indicaciones muestran la retícula y el texto que publica gameplay.
2. Al pasar por el refrigerador se oculta la retícula, aparecen vida y progreso y la cámara muestra a la protagonista.
3. La barra del jefe se actualiza durante el encuentro y se oculta al salir de él.
4. Una derrota permite reintentar o volver al menú. El panel final se abre después de ver el trono y vuelve al menú.
