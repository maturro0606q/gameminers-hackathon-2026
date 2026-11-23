# Flujo de pantallas · La heladería maldita

La UI vive en `Assets/UI/`. Matías implementa navegación, controles, HUD, paneles, créditos y preparación para arte. Nacho compone las escenas y controla entrada, cursor, cámara, transiciones y estado de partida. Cami entrega sprites de botones y fondos y los modelos / diseños de personajes.

## Recorrido

| Momento | Pantalla que muestra la UI | Qué la hace avanzar |
| --- | --- | --- |
| **Inicio** | Menú con Jugar, Controles, Créditos y Salir en Windows | Jugar solicita iniciar la partida. Controles y Créditos vuelven al menú desde la propia UI. |
| **Heladería y bodega** | Objetivo, retícula POV, indicación contextual y stock cuando esté disponible | Gameplay detecta interacciones y actualiza objetivo, stock y refrigerador. |
| **Refrigerador** | Fundido y mensaje del portal | Gameplay actualiza la opacidad y carga el mundo del Jety. |
| **Combate** | Vida, objetivo, progreso y, cuando hay datos, arma y bombas. Barra de jefe e interacción cuando corresponden. | Gameplay actualiza los datos. El HUD de cámara lateral funciona sin retícula FPS. |
| **Pausa** | Continuar, Reiniciar nivel y Volver al menú | Gameplay decide cuándo pausar, reanudar o reiniciar. |
| **Derrota** | Reintentar y Volver al menú | Gameplay confirma el punto de reintento. |
| **Trono** | Durante la secuencia, la UI se oculta. Después se muestran el panel final, Volver al menú y Créditos. | Gameplay confirma que terminó la cinemática. |

## Montaje para Nacho

1. Instanciar `Assets/UI/Prefabs/GameUiFlow.prefab` por escena. Incluye Canvas, Canvas Scaler de referencia 1600 × 900 y Graphic Raycaster. La escena aporta un solo EventSystem con Input System UI Input Module.
2. Enviar el estado inicial y los cambios con `GameUiFlowView.Render(GameUiSnapshot)`. Se puede enviar el snapshot desde el inicio de la escena. Si todavía no se ha enviado uno, el Canvas muestra el menú por defecto. Los prefabs individuales también se pueden montar por separado dentro de otro Canvas.
3. Conectar por código `PlayRequested`, `PauseRequested`, `ResumeRequested`, `RestartLevelRequested`, `RetryRequested` y `MenuRequested` con el controlador de gameplay. Suscribirse en `OnEnable` y desuscribirse en `OnDisable` si el adaptador se activa y desactiva.
4. Al detectar Escape, llamar `RequestBack()`. En tienda o combate solicita pausa, en pausa solicita continuar y en los paneles de Controles o Créditos vuelve a la pantalla de origen. Gameplay controla el cursor y los mapas de entrada.
5. Revisar los modos de cámara acordados en el [PR #1](https://github.com/maturro0606q/gameminers-hackathon-2026/pull/1). La base Unity `6000.4.11f1`, Universal 3D, está en el [PR #2](https://github.com/maturro0606q/gameminers-hackathon-2026/pull/2).

`GameUiSnapshot` pertenece a la capa de presentación. Adaptar desde la fase real de gameplay a `Menu`, `Shop` (incluye bodega), `Portal`, `Combat` (incluye oleadas y ambos jefes), `Pause`, `Defeat`, `FinalCutscene` o `Ending`. Los estados del mundo y la decisión de reinicio permanecen en gameplay.

- En `Shop` pasar `Objective`, `InteractionBinding`, `InteractionAction` y, si existe, `StockRemaining`. Una acción vacía oculta la indicación.
- En `Combat` pasar vida y máxima, objetivo y `DefeatedCount` / `DefeatedTarget`. El mismo contador muestra 1/1 en M1 y, en M2, una meta de 23 para la primera oleada y de 10 para la segunda. Para equipamiento, `EquippedWeapon` y `BombCount` son opcionales. Para el jefe enviar nombre, vida y máxima mientras esté activo.
- En `Portal` actualizar `PortalOpacity` entre 0 y 1 y, si hace falta, `PortalMessage`. Enviar `FinalCutscene` durante la secuencia de trono y `Ending` al terminarla.

## Entrega de arte para Cami

| Pieza | Formato y punto de integración |
| --- | --- |
| Fondos de menú, controles, pausa, derrota, final y créditos | PNG 16:9, referencia 1600 × 900 o superior. Importar como Sprite (2D and UI), con área libre donde va el texto. |
| Botones | PNG con transparencia. Los textos siguen en Unity. En el tema se pueden asignar sprites para botones principales y secundarios, con estados opcionales de hover, pulsado, seleccionado y deshabilitado. Para un botón único, asignar su Sprite en el Image del prefab. |
| Personajes | Diseños y modelos en el flujo de arte acordado. Nacho conecta sus modelos a los prefabs visuales de gameplay. En combate la protagonista se ve de cuerpo completo en cámara lateral elevada. |

Matías crea el tema con **Assets → Create → GameMiners → UI → Artwork** y lo asigna al campo `Artwork` del Canvas. Cada campo admite quedar vacío para conservar el estilo provisional. Un Sprite asignado directamente a un botón tiene prioridad sobre el tema general. Los originales privados de Cami se guardan en la ubicación de arte acordada. La incorporación de exportaciones al repositorio público o al build se coordina con Cami según la licencia de cada recurso.

## Prueba de integración

1. Abrir el menú, navegar Controles y Créditos y volver con botón o Escape. Probar Jugar.
2. Recorrer heladería y bodega, agotar stock y usar el refrigerador. Comprobar fundido y cambio de HUD sin retícula en combate.
3. Entrar y salir de pausa, reiniciar nivel, actualizar vida y progreso y comprobar que hay barra solo para el jefe activo.
4. Probar derrota con reintento y retorno. En el final, comprobar que la cinemática despeja la UI, el panel final permite volver al menú y Créditos regresa al final.
5. Asignar un fondo y un Sprite de botón para comprobar los puntos de arte. Verificar el recorrido desde el editor y en el build del equipo.
