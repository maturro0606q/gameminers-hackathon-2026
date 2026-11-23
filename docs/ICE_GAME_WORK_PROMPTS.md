# GameMiners — Prompts iniciales por Work/Codex (heladería maldita)
> Copiar solo el bloque de tu rol. **Leer antes** `AGENTS.md`, `docs/ICE_GAME_GDD.md` y `docs/ICE_GAME_IMPLEMENTATION.md`.
> Repositorio: https://github.com/maturro0606q/gameminers-hackathon-2026
> Estos son prompts para planificar y ejecutar el **hito M1** tras autorización. Nunca afirmar que Unity/Blender funcionan sin acceso real.
> **Cambio de arquitectura acordado:** tienda/bodega = POV primera persona; mundo de helados y sala Jety = cámara 3D lateral elevada / semilateral en tercera persona (referencia: *Fairytale Fights* de 2009). Leer `docs/ICE_GAME_CAMERA_ARCHITECTURE.md`.

> **Nueva instrucción de identidad del juego:** leer también `docs/HACKATHON_ELEMENTS.md` antes de programar o modelar. Los elementos obligatorios acordados son las dos frases, el cartel Codex (respetando marca), el conejo guía inspirado a nivel conceptual en títeres chilenos, el completo italiano y los conejos de protesta con ambos letreros. No copiar personajes de *31 Minutos*.

## Matías — Product / UX / Integración
```text
Actúa como mi Work técnico para GameMiners. Lee AGENTS.md, docs/ICE_GAME_GDD.md y docs/ICE_GAME_IMPLEMENTATION.md del repo https://github.com/maturro0606q/gameminers-hackathon-2026; confirma que su diseño de heladería maldita es la propuesta actual y distingue el prototipo antiguo de 3 coleccionables. Soy Matías, responsable de arquitectura funcional, UI, coordinación e integración. Trabaja en feature/ui, sin editar las escenas/Gameplay de Nacho ni los assets/Art de Cami.

Primero inspecciona el proyecto real y propón un contrato mínimo de eventos/estado con Nacho: fase, vida, progreso 23/10, boss HP, arma y bombas, e instrucciones E. No fabriques APIs de gameplay que aún no existen. Implementa el mensaje de agotamiento «¡Queda una pura bola de helado!» y subtítulo de conejo «Oh, eres el heladero número 23 en venir aquí» solo cuando Nacho active las respectivas fases. Lee docs/HACKATHON_ELEMENTS.md. La UI debe distinguir controles, indicadores y retícula de **tienda POV** frente a **combate cámara lateral 3D**; no diseñes un HUD que asuma FPS durante toda la partida. Tras mi autorización, implementa solo tus prefabs y scripts de UI para M1: menú Jugar/Controles, prompts de interacción, retícula, vida, objetivo/oleada, pantalla de derrota, barra jefe, final y volver a menú. Usa datos simulados SOLO para previsualizar la UI, no como lógica productiva. Informa qué eventos requieren vinculación.

Después de cada bloque, prueba lo que puedas, commit/push a tu rama y prepara PR sin fusionar main. Si no hay acceso local al editor, no simules pruebas. Ayúdame a integrar por etapas en una build que va de tienda a trono. No implementes funciones extra. Antes de escribir código, devuelve plan de archivos, contrato y dependencias y espera mi instrucción «implementar M1».
```

## Nacho — Gameplay e integración de escenas
```text
Actúa como mi Work para programar gameplay en Unity/Codex del mismo repo https://github.com/maturro0606q/gameminers-hackathon-2026. Lee AGENTS.md y docs/ICE_GAME_GDD.md + ICE_GAME_IMPLEMENTATION.md. Soy Nacho y trabajo solo en feature/gameplay, dueño de Assets/Gameplay y composición de escenas. No toques Assets/UI ni Assets/Art; conéctalos mediante contratos acordados.

Lee docs/HACKATHON_ELEMENTS.md. En M1 instala un letrero Codex placeholder en la tienda, activa la frase «¡Queda una pura bola de helado!» al agotarse el stock, y al llegar al mundo helado presenta un conejo guía con «Oh, eres el heladero número 23 en venir aquí». Acomoda en el fondo de combate conejos espectadores con «No más conos» y «No más humanos» y un conejo sosteniendo un completo italiano; usar prefabs primitivos hasta recibir assets. No crear IA de conejos. Comprobar permiso para usar logotipo oficial Codex. Tu prioridad es M1: una partida COMPLETA con placeholders desde menú/tienda, atender clientes, agotamiento, bodega/refrigerador con Jety, mundo de helados, 1 helado chico + 1 grande + 1 jefe, sala Jety, combate básico, final de trono y retorno al menú. Implementa **dos controladores/cámaras por contexto**: POV con mouse en tienda/bodega y tercera persona con **cámara lateral elevada en tres cuartos** que sigue al personaje visible en el mundo de helados y sala Jety, como referencia *Fairytale Fights* (2009). En combate utiliza movimiento 3D relativo a la cámara sin mouse free-look, una sola mecánica de ataque con cucharón y transiciones simples; no comiences aún la complejidad de 23+10, lanza, cofres o bombas. Esas funciones pertenecen a M2/M3.

Antes de editar, lee también `docs/ICE_GAME_CAMERA_ARCHITECTURE.md`, inspecciona Unity y el repositorio, propone nombres/rutas de 4 escenas, estructuras mínimas de scripts y contrato de eventos para Matías, y presenta dependencias visuales a Cami. Usa Unity MCP solo si está conectado; confirma pruebas reales. Por tarea: commit/push a feature/gameplay, PR para revisión, sin merge directo. Si necesitas cambiar el contrato o UI, solicita coordinación. Entrega un recorrido funcional en incrementos con controles simples. Detente después del plan y espera mi autorización «implementar M1».
```

## Cami — Arte, Blender MCP y exportación Unity
```text
Actúa como mi Work 3D para GameMiners. Lee AGENTS.md, docs/ICE_GAME_GDD.md y docs/ICE_GAME_IMPLEMENTATION.md del repo https://github.com/maturro0606q/gameminers-hackathon-2026. Soy Cami; soy dueña de Assets/Art y trabajo en feature/art. Modelarás desde las fotos de mis dibujos directamente en Blender mediante el MCP conectado, no solo me explicarás cómo hacerlo, cuando tengas acceso real.

Lee docs/HACKATHON_ELEMENTS.md. Además del arte de combate, tendremos un conejo guía ORIGINAL con lenguaje visual de títere expresivo (inspiración general en *31 Minutos*, sin copiar ningún personaje), conejos espectadores reutilizables, un conejo sosteniendo un completo italiano, pancartas «No más conos» y «No más humanos», y diseño de cartel para Codex (sin alterar su marca y previa verificación de términos). No bloquees gameplay para construirlos. El juego definitivo necesita tres variantes de helados enemigos (23 pequeños, 10 grandes, 1 jefe), un Jety jefe final, armas (cucharón, lanza de dulce, bombas de chicle), cofres y ambientación tienda/mundo/trono. Pero SOLO para el primer hito M1 debemos preparar un modelo mínimo que pruebe la integración, sin bloquear a Nacho mientras él usa proxies. No modeles todo ahora. **La protagonista es visible en tercera persona en los combates**; planifica proporciones, silueta, rig y animaciones con vista lateral elevada / tres cuartos en mente, en lugar de modelar solo brazos de un FPS.

Primero verifica acceso real a Blender MCP y repositorio; identifica versión, capacidad de guardar .blend y exportar FBX. Cuando te dé una imagen y autorice un recurso, modela low-poly fiel a la referencia, conserva cuerpo/cabello/ropa separados si es personaje, y crea rig/weight paint solo si necesita deformarse animado. Prueba poses, escalas, pivote y exportación; nunca declares válido un rig sin comprobación. Guarda fuente .blend en Assets/Art/.../Source y FBX en .../Models; sube commit/push a feature/art y prepara PR. No edites escenas ni gameplay. Confirma archivo, estado de rig, ruta y pruebas; espera mis referencias y autorización.
```

## Regla general de coordinación
- El MVP es **recorrido completo** primero, **conteos 23 + 10** segundo, **armas/cofres** tercero, **pulido y opcionales** después.
- Nadie mezcla PR sobre main sin revisión.
- Si una integración requiere editar el archivo de otro, entregar requerimiento en vez de sobrescribirlo.
- Repositorio/documentación no prueba funcionamiento: verificar que Unity importe, ejecute y compile.
