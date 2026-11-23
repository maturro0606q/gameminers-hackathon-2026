# GameMiners — Plan por responsable e integración del juego de helados
> V0.1 | Propuesta a aprobar antes de programar | Leer junto a ICE_GAME_GDD.md

## 1. Arquitectura de escenas propuesta
- `Assets/Gameplay/Scenes/00_Menu.unity`: menú inicial; Nacho controla composición, Matías aporta UI prefabs.
- `Assets/Gameplay/Scenes/01_Shop.unity`: mostrador, clientes simples, puerta bodega, refrigerador y captura del Jety. Bodega dentro de esta escena, NO crear escena extra sin necesidad.
- `Assets/Gameplay/Scenes/02_IceWorld.unity`: las 3 fases (23 pequeños, 10 grandes, jefe helado) en **una escena**, no tres niveles separados.
- `Assets/Gameplay/Scenes/03_JetyRoom.unity`: pelea del Jety y secuencia final del trono en la misma escena.

La escena de menú se puede sustituir por bootstrap/panel si el proyecto actual lo hace mejor. No cambiar escena/versión/plantilla sin revisión. `AGENTS.md` conserva prioridad para políticas generales.

### Arquitectura global de cámaras y controles (acuerdo de equipo)
- `01_Shop`: rig y controlador **primera persona (POV)**. Mouse = mirar; WASD mover; E interactuar.
- Refrigerador: interacción + captura + transición por fundido/carga. Al llegar a `02_IceWorld`, activar otro modo de control/cámara.
- `02_IceWorld` y `03_JetyRoom`: **tercera persona 3D lateral elevada / tres cuartos, estilo visual *Fairytale Fights* (2009)**. Protagonista visible; cámara persigue con encuadre estable, sin free-look del mouse. WASD mueve relativo a ejes de cámara en suelo 3D; orientación/ataque hacia dirección del personaje. No asumir movimiento restringido a solo X hasta probarlo.
- Usar un **controlador de tienda** y un **controlador de combate** independientes; reutilizar sistema mínimo de estado y transiciones. El rig de combate se mantiene entre ambos niveles. No introducir cambio de cámara complejo en tiempo real dentro del combate.
- Nacho posee rigs, cámaras, controladores y escenas en `Assets/Gameplay/**`. Matías adapta texto de controles, indicaciones y HUD según contexto sin editar escenas ajenas. Cami entrega protagonista con silueta y rig legibles desde vista externa lateral/3-4.
- Detalles de implementación, pruebas y decisiones abiertas: [ICE_GAME_CAMERA_ARCHITECTURE.md](ICE_GAME_CAMERA_ARCHITECTURE.md).

## 2. Propiedad de archivos
| Persona | Dueño / alcance | Evitar |
| --- | --- | --- |
| Matías | `Assets/UI/**`, pantallas, prefabs HUD/menú/pausa/final, `Assets/Shared/Contracts/**` tras acuerdo, documentación, integración/revisión | Editar escenas gameplay a la vez que Nacho |
| Nacho | `Assets/Gameplay/**`, control jugador, fases, enemigos, golpes, armas, cofres, oleadas, composición de todas las escenas | Editar prefabs UI o fuente Blender |
| Cami | `Assets/Art/**`, modelos, rigs y texturas en `Source/`, `Models/`, `Materials/`; prefabs visuales solo si lo acuerdan | Editar lógica, escenas o prefabs de otra área |

Ramas previstas (ya documentadas en el repo): `feature/ui`, `feature/gameplay`, `feature/art`. **Una persona por prefab/escena.** Integrar mediante PR a `main`, nunca copiar archivos manualmente a Unity.

## 2.1 Elementos de identidad obligatorios (trabajo transversal)

Ver [HACKATHON_ELEMENTS.md](HACKATHON_ELEMENTS.md) para especificación, responsables, derechos y criterios de aceptación. El equipo ha decidido incluir: «¡Queda una pura bola de helado!» (cuando queda 1 bola en tienda; después servir la última y habilitar bodega); conejo guía con línea exacta «Oh, eres el heladero número 23 en venir aquí» (primera llegada a mundo helado); logo de Codex como letrero de heladería (sujeto a guía de marca); conejo espectador con completo italiano; conejo guía original con lenguaje visual de títeres inspirado conceptualmente en *31 Minutos*; y conejos protestantes en el fondo con «No más conos» y «No más humanos».

- **Nacho** incorpora triggers de diálogo y distribución de props/NPC decorativos dentro de sus escenas, sin sistemas complejos.
- **Matías** integra dos líneas de texto y créditos/indicaciones desde sus prefabs UI, usando el contrato existente sin tocar gameplay.
- **Cami** diseña conejo guía, conejo ambiente (reutilizable), completo, pancartas y letrero; modelado original con sus créditos y archivos autorizados.

**Primero placeholders + mensajes. Después assets.** Ninguna de estas referencias bloquea M1 si su arte aún está pendiente. El logo oficial de Codex se incorpora solo tras comprobar términos de marca aplicables, sin atribuir patrocinio.

## 3. Contrato entre gameplay y UI (acordar y congelar)
Para no tocar archivos del otro, fijar un adaptador pequeño, creado una sola vez en ruta compartida:

**Datos publicados desde gameplay hacia UI**
- Fase actual: Menu, Shop, Storage, Portal, SmallWave, BigWave, IceBoss, JetyBoss, Ending, Defeat (enumeración orientativa; no sobreingeniería).
- Vida actual y máxima.
- Título de objetivo actual.
- Contador de derrotas de fase (`killed`, `required`), con 23 o 10 según corresponda.
- Arma equipada (`Scoop`, `CandySpear`, `None`).
- Cantidad de bombas.
- Vida del jefe activo, cuando haya.

**Acciones desde UI a gameplay**
- Iniciar partida, reintentar, continuar, pausar/reanudar, volver al menú.

**Reglas de implementación**
- Una sola fuente de verdad para estado de partida (Nacho); UI solo muestra estado y solicita acciones.
- Preferir eventos C# + snapshot inicial de estado. UI debe actualizarse al activarse aunque haya perdido eventos previos.
- Matías y Nacho acuerdan firmas exactas antes de programar los consumidores; no crear APIs con nombres contradictorios.
- Arte usa puntos de reemplazo de `visual prefab`, colisiones y comportamiento en prefabs controlados por Nacho.

## 4. Hitos de desarrollo: siempre jugable

### M0 — Base real e integración
**Nacho:** verifica proyecto Unity real, versión exacta y escenas. **Matías:** repo, rama UI, contrato compartido. **Cami:** carpeta arte y workflow Blender→FBX.
**Criterio:** todos clonan, Unity abre y escena base ejecuta; Git sincroniza un cambio no destructivo de cada uno. Sin suponer que basta con GitHub.

### M1 — Recorrido completo con proxies (PRIORIDAD ABSOLUTA)
**Nacho**
1. Configurar 4 escenas y navegación.
2. Dos modos: jugador POV en tienda (WASD + mouse + E) y personaje visible en cámara lateral elevada / tres cuartos en niveles de combate (WASD relativo a cámara; sin free-look).
3. Tienda: atender 2 clientes de forma simplificada, presentar «¡Queda una pura bola de helado!» cuando queda 1 bola, servirla, agotar stock, acceder bodega, abrir refrigerador. Cartel Codex en zona visible (placeholder hasta validación de marca).
4. Portal por fundido/carga: transición visible y comprobable de POV a cámara tercera persona 3D semilateral al entrar en mundo de helados.
5. Ataque principal simple, enemigos placeholders y vida.
6. Conejo guía al entrar: «Oh, eres el heladero número 23 en venir aquí». Oleada de prueba: 1 pequeño, 1 grande, 1 jefe helado; conejos decorativos con carteles «No más conos» y «No más humanos» y uno con completo italiano (proxies hasta entrega de arte).
7. Portal a sala del Jety, combate simple con boss placeholder.
8. Cinemática mínima de trono y vuelta a menú.
**Matías**: menú Jugar/Controles con instrucciones para **dos esquemas de cámara**, retícula/prompts de tienda y prompts contextuales de combate, HUD vida/progreso, boss HP, derrota, final y botones. Prefabs separados; ocultar retícula FPS en combate si no resulta útil.
**Cami**: proxies artísticos o una referencia de helado y Jety; no bloquear M1 por rigs definitivos. Preparar conejo guía de títere ORIGINAL y variantes de conejo decorativo cuando termine las prioridades del personaje y primer enemigo. Preparar modelo 3D de pequeño helado o prop según referencias. Planificar protagonista visible con rig apto para cámara de combate en tercera persona.
**Criterio:** desde Jugar hasta final, sin usar editor para avanzar de escena; se puede perder/reintentar; al menos un recurso de Cami importado. Compilar versión PC de prueba. No declarar M1 completo si el final no funciona.

### M2 — Reglas originales de oleadas
**Nacho:** ajustar contadores **23 pequeños**, luego **10 grandes**, luego jefe; spawning en grupos; desactivar futuras fases y contar una vez; barra de jefe; balance de vida/ataques.
**Matías:** HUD y señales de cambio de fase; instrucciones comprensibles sin diálogo.
**Cami:** tres diseños de helados: pequeño, grande y jefe, con escalas distinguibles y prefabs visuales exportados.
**Criterio:** 23 y 10 exactos, sin doble conteo ni spawns de etapa incorrecta; derrota jefe desbloquea sala Jety.

### M3 — Armas y cofres
**Nacho:** (1) cucharón melee ya funcional; (2) lanza de dulce pickup, equipar sustituyendo arma, arma antigua queda en suelo; (3) lanzar la lanza, permanecer en mapa y recuperarla con E; (4) bombas de chicle consumibles con daño en área; (5) cofres prefijados que entregan lanza o reponen bombas.
**Matías:** icono/nombre de arma, bombas restantes, prompts recoger/abrir, feedback visual de hit.
**Cami:** cucharón, lanza de dulce, bomba y cofre, según prioridades; animar o riggear solo lo necesario.
**Criterio:** equipar y soltar no destruye armas ni duplica objetos; lanza lanzada no reaparece por magia; recoger funciona; bombas 0 impiden lanzar; cofre entrega recompensa una sola vez.

### M4 — Arte y experiencia
**Cami:** reemplazo de placeholders, materiales, rigs (Jety y enemigos si tendrán animación esquelética), ambiente de tienda/mundo/trono.
**Nacho:** importación, control de escalas/colliders y ajustes de IA, cámara de final.
**Matías:** cohesión visual UI, transiciones, pantalla créditos, accesibilidad controles.
**Criterio:** no quedan bloqueos por recursos, FPS aceptable en equipo objetivo, build inicia y termina.

### M5 — Entrega y ampliaciones
**Matías:** integra PR revisados, README, licencia de código vs arte, build para jurado, instrucción Jugar, copia de respaldo.
**Nacho:** corrige errores críticos y congela sistemas 1 hito antes de entrega.
**Cami:** créditos y optimización de assets importados.
**Solo si todo lo anterior está validado**: nivel intermedio como nueva escena opcional.
**Criterio:** build externo y repositorio públicos verificables, recorrido completo probado sin Unity Editor.

## 5. Protocolo diario de contribución
1. Antes de editar: GitHub Desktop **Fetch/Pull** sobre rama de trabajo; revisar PR integrados en main.
2. Seleccionar rama propia, comprobar `git status` limpio. Integrar últimos cambios de main con criterio si procede; no forzar pushes ni borrar cambios no integrados.
3. Trabajar sobre rutas asignadas. Si afecta contrato compartido o escenas de Nacho: acordar antes.
4. Probar en Unity local; si Work no puede acceder, documentar **no probado**.
5. Commit pequeño, push, PR con cambios, capturas/resultado y pruebas.
6. Matías revisa e integra de a uno; se sincroniza `main` y se prueba en una máquina tras cada merge.
7. Al cerrar hito, build y prueba externa; si falla, revertir o corregir antes de sumar otra función.

## 6. Matriz de pruebas
| Caso | Esperado |
| --- | --- |
| Menú Jugar | Inicia tienda |
| Atender clientes | Stock llega a cero y permite ir bodega |
| Queda una bola en tienda | Texto «¡Queda una pura bola de helado!»; tras última venta stock=0 y se habilita bodega |
| Cartel heladería | Logo Codex visible, sujeto a política de marca verificada |
| Refrigerador E | Jety aparece / transición desde POV tienda hacia vista externa lateral 3D |
| Bienvenida conejo | Muestra «Oh, eres el heladero número 23 en venir aquí» antes de la oleada |
| Fondo de combate | Conejo con completo italiano, conejos con «No más conos» y «No más humanos» visibles |
| Cámaras por contexto | En tienda FPS con mouse; tras captura vista lateral elevada 3D con protagonista visible; cámara mantiene seguimiento en combate y sala Jety |
| Mundo primera oleada | Cuenta exactamente 23 pequeños |
| Mundo segunda oleada | Comienza tras 23, cuenta exactamente 10 |
| Jefe helado | Aparece tras 10 y abre acceso tras morir |
| Cucharón | Golpea solo dentro de rango/cooldown |
| Lanza y drop | Solo 1 arma equipada; al lanzar queda recuperable |
| Bombas | Reduce munición, daño área, nunca saldo negativo |
| Cofre | Una recompensa por cofre |
| Vida y derrota | Pantalla y reintento funcional |
| Jety | Ataque claro, vida visible, muerte dispara final |
| Final | Trono + cabeza de Jety en lanza + volver a menú |
| Build | Corre fuera del editor en PC o navegador previsto |

## 7. Lista de pendientes y decisiones
- Confirmar nombre definitivo, cantidad inicial de clientes y esquema preciso de controles. **La separación de cámaras sí está acordada:** tienda en POV; mundo Jety con cámara tercera persona lateral elevada / tres cuartos tipo *Fairytale Fights*. Falta calibrar ángulo y movimiento en profundidad en Unity.
- Confirmar versión exacta de Unity, plantilla y destino build.
- Cami: referencia del Jety y siluetas de los tres helados; modelado no debe bloquear M1.
- Decidir si la recuperación tras muerte reinicia nivel o usa checkpoint de oleada.
- Definir licencias de arte original antes de su distribución open source.

**Sincronización con documentación anterior:** `docs/GAME_DESIGN.md` y `docs/TASKS.md` aún documentan la prueba técnica anterior (3 coleccionables). No confundirla con esta hoja de ruta del juego definitivo. Tras aprobar esta propuesta, Matías decide cuándo reemplazar/adaptar documentos anteriores.
