# Arquitectura y propiedad

## Decisión transversal del juego definitivo — cámaras (23-11-2026)

La prueba técnica antigua de 3 coleccionables descrita más abajo **no define** los controladores del juego definitivo. El acuerdo de equipo para `La heladería maldita` es:

- **Tienda y bodega (escena 01_Shop):** cámara subjetiva en **primera persona (POV)** con control del mouse.
- **Después de que el Jety captura al jugador en el refrigerador:** transición a escena 02_IceWorld con cámara en **tercera persona lateral elevada / tres cuartos, 3D**, semejante a la presentación de *Fairytale Fights* (2009), con personaje visible y seguimiento sin control libre de cámara. Esta cámara se mantiene en la sala del Jety (03_JetyRoom).
- **No confundir con un juego de plataformas 2D plano:** el movimiento conserva profundidad 3D relativa a la cámara, por validar con un prototipo.
- **Final:** puede cambiar a un plano cinematográfico cuando la protagonista se sienta en el trono.
- **Implementación:** Nacho controla cámaras, movimientos y escenas dentro de Gameplay; Matías solo UI, instrucciones contextuales y contratos acordados; Cami debe entregar protagonista y enemigos legibles desde vista lateral/3-4.
- **No reutilizar a la fuerza el mismo FPS controller para ambos modos**; compartir el estado y cambiar de rig/controlador al cargar el nivel. No alterar contratos compartidos sin acuerdo.

Referencias: [GDD del juego](ICE_GAME_GDD.md), [plan](ICE_GAME_IMPLEMENTATION.md), [decisión detallada de cámaras](ICE_GAME_CAMERA_ARCHITECTURE.md).

---

## Proyecto base

- **Editor exacto:** por confirmar con los tres computadores. Registrar la versión completa, incluido el sufijo, y verificar `ProjectSettings/ProjectVersion.txt`.
- **Plantilla:** por confirmar. Candidata inicial: Universal 3D si está disponible y es viable en los tres equipos.
- **Ubicación:** raíz del repo. Matías publica la copia creada por Unity Hub con `Assets/`, `Packages/`, `ProjectSettings/` y configuraciones necesarias. Versionar `Packages/manifest.json` y `Packages/packages-lock.json` si Unity lo genera. Configurar serialización `Force Text` y usar `Visible Meta Files` si el editor ofrece el ajuste.
- **Build de prueba:** destino por acordar. Evaluar Windows o Web según los módulos instalados.

## Rutas propuestas para acordar antes de implementar

| Responsable | Rutas | Trabajo |
| --- | --- | --- |
| Nacho | `Assets/Gameplay/Scenes/Prototype.unity`, `Assets/Gameplay/Scripts/`, `Assets/Gameplay/Prefabs/` | Escena principal, jugador, colisiones, coleccionables y estado de la partida |
| Matías | `Assets/UI/Prefabs/`, `Assets/UI/Scripts/` | Menú, HUD y panel de victoria/reinicio |
| Cami | `Assets/Art/Models/`, `Assets/Art/Materials/` | Modelo, materiales y prefab visual si se acuerda |
| Matías | `Packages/`, `ProjectSettings/` | Coordina cambios compartidos con el integrante afectado |

La integración en `Prototype.unity` la realiza Nacho, coordinado con Matías y Cami. Mueve o renombra assets desde el editor, o preserva su `.meta` al hacerlo fuera de Unity.

## Contratos propuestos

| Cruce | Acuerdo por confirmar |
| --- | --- |
| Gameplay → UI | Gameplay proporciona el estado inicial, actualizaciones con cantidad recogida y total, y el aviso de victoria. La UI lee el estado al montarse y lo representa |
| UI → gameplay | La UI emite solicitudes de iniciar y reiniciar. Gameplay aplica la transición y restablece posición, coleccionables y contador al reiniciar |
| Arte → gameplay | Cami entrega un recurso visual con escala, orientación y materiales acordados. Nacho lo referencia en su prefab y conserva control sobre colisiones y comportamiento |

Los nombres de APIs, las firmas de eventos y el montaje se acuerdan con los tres integrantes antes de iniciar Fase 2.
