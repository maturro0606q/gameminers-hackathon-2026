# GameMiners — La heladería maldita
> Diseño v0.1 | 23-11-2026 | **PROPUESTA PARA REVISIÓN DEL EQUIPO** | Nombre provisional.

## 1. Visión
Juego de acción 3D estilizado con **dos modos de cámara** (POV en heladería y tercera persona lateral elevada / tres cuartos en el mundo del Jety), con humor absurdo y giro de fantasía oscura. Una vendedora atiende una heladería, agota existencias, busca reposición en la bodega y abre un refrigerador del que sale un **Jety** que la secuestra hacia un mundo de monstruos de comida. Supera el mundo de helados y la sala del Jety. Cuando vence, en vez de escapar, se sienta en su trono y exhibe su cabeza en una lanza de forma estilizada y sin gore. El recorrido la transformó. **El final es parte obligatoria de la primera versión jugable**.

## 2. Principios de diseño
1. **Primero flujo entero**: menú → tienda → bodega/refrigerador → mundo de helados → Jety → trono.
2. Probar un recorrido de extremo a extremo con proxies antes de arte, números de oleadas o armas complejas.
3. Una sola persona controla la **composición de las escenas** (Nacho); UI y arte se integran como prefabs y assets independientes.
4. **Dos modos de juego diferenciados:** tienda + bodega en primera persona (POV); tras la captura en el refrigerador, mundo de helados y sala del Jety en **tercera persona, con cámara lateral elevada / semilateral 3D**, siguiendo como referencia visual *Fairytale Fights* (2009). No confundir con un plataformas 2D estrictamente plano. La secuencia final puede usar cámara cinematográfica.
5. Evitar sistemas no esenciales: multijugador, mundo abierto, guardado, árboles de habilidades, economía, diálogos, animaciones complejas, procedural y enemigos muy sofisticados.
6. Mantener compilable, probada y jugable la rama main después de cada integración.

### 2.1. Perspectiva y transición obligatorias
- **Heladería/bodega (01_Shop):** primera persona real, manos/herramientas o retícula si son necesarias. El jugador mira con el mouse y usa E para interactuar.
- **Captura en el refrigerador:** fundido o cinemática breve; la **cámara cambia al cargar 02_IceWorld**, no mediante un giro continuo mientras se combate.
- **Mundo de helados (02_IceWorld) y sala del Jety (03_JetyRoom):** protagonista visible en pantalla, cámara 3D **lateral elevada / en tres cuartos**, seguimiento suave y encuadre legible de enemigos (referencia: *Fairytale Fights*, no cámara FPS ni cámara libre con mouse). Movimiento sobre suelo 3D con dirección relativa a la cámara; ejes, restricciones de profundidad y ángulo exactos se validan con un prototipo visual.
- **Final del trono:** se admite cámara cinematográfica al terminar el combate.
- **Alcance:** en M1 usar un controlador POV para la tienda y otro de combate en tercera persona para ambos niveles hostiles, con configuración de estado/transiciones compartida; no intentar convertir un único controlador FPS para ambos modos.
- Ver [decisión técnica de cámaras](ICE_GAME_CAMERA_ARCHITECTURE.md).

## 2.2. Referencias obligatorias de identidad del equipo

Las cinco referencias acordadas por GameMiners son parte **obligatoria** del flujo del juego y deben colocarse antes del pulido visual: (1) «¡Queda una pura bola de helado!» al quedar una sola bola de helado en la tienda, antes del último servicio; (2) un conejo guía al llegar a IceWorld dice «Oh, eres el heladero número 23 en venir aquí»; (3) letrero de la heladería con logo Codex oficial si está permitido su uso; (4) un conejo espectador sostiene un completo italiano en el fondo de la pelea; (5) el conejo guía tiene diseño ORIGINAL de títere expresivo, inspirado conceptualmente en la estética de *31 Minutos*, sin copiar personajes. Además se ambienta el campo de batalla con **conejos de público/protesta** que portan letreros «No más conos» y «No más humanos».

La ubicación, permisos de marca, separación de tareas y checklist están en [HACKATHON_ELEMENTS.md](HACKATHON_ELEMENTS.md). No deben agregar escenas ni mecánicas nuevas para cumplir estos requisitos. En M1 se permiten proxies y texto; la sustitución por arte de Cami es posterior y no bloquea el recorrido completo.

## 3. Flujo narrativo y jugable

### 3.1 Menú principal
Pantalla con título, **Jugar**, **Controles**, **Salir** (Salir solo en build PC; en Web ocultar). Al elegir Jugar se inicia la tienda.

### 3.2 Tienda de helados (escena 01_Shop)
- Vista POV de la protagonista detrás del mostrador.
- Un cliente se acerca, aparece una indicación simple de servir y vender helado; interactuar con **E** (objetivo sugerido: atender 2 o 3 clientes; NO implementar dinero ni pedidos variados en la primera versión).
- Cuando el stock llega a **1**, aparece **«¡Queda una pura bola de helado!»**. Se sirve la última bola, stock pasa a 0, y entonces se muestra «Busca más helados en la bodega». El aviso debe ocurrir antes de ir a buscar al refrigerador.
- Se habilita la puerta de bodega; entrar, acercarse al refrigerador e interactuar con **E**.
- El Jety aparece desde el refrigerador, efecto corto (animación simple, empuje de cámara o fundido), y transporta a la protagonista al mundo hostil.
- Si la actuación del Jety demora, usar silueta/cara provisional y fundido: no bloquear la progresión por cinemática.

### 3.3 Nivel 1 — Mundo de helados (escena 02_IceWorld)
- Al llegar, un **conejo guía original** (inspiración general en títeres tipo *31 Minutos*) recibe con «Oh, eres el heladero número 23 en venir aquí». El combate empieza tras esa presentación breve. El área muestra conejos espectadores con carteles «No más conos» y «No más humanos» y uno sostiene un completo italiano. Todos son ambientación sin IA ni combate en M1.
- El jugador recibe como arma principal inicial un **cucharón de helados**, usado en ataques cuerpo a cuerpo tipo espada.
- **Oleada 1:** derrotar **23 helados pequeños**. El contador muestra «Helados pequeños: X/23». Generar en grupos acotados (p. ej. 3–5) para mantener rendimiento y legibilidad; el número exacto de simultáneos se ajusta en pruebas.
- **Oleada 2:** tras las 23 bajas, derrotar **10 helados grandes**. «Helados grandes: X/10».
- **Jefe nivel 1:** aparece un **helado gigante jefe**, criatura distinta de pequeños y grandes; tiene barra de vida. Su derrota abre el acceso al siguiente nivel.
- Cami crea 3 diseños/variantes conceptuales: pequeño, grande y jefe. Para velocidad, se pueden construir con la misma familia visual y distintos tamaños, materiales y siluetas. Para la versión final, sustituir por modelos propios.

### 3.4 Nivel 2 — Sala del Jety (escena 03_JetyRoom)
- Espacio final reconocible con trono y Jety.
- Un enfrentamiento con el Jety, al menos un ataque legible y oportunidades de contraatacar. Puede haber enemigos secundarios del nivel final si hay tiempo; **no son necesarios para completar el MVP**.
- Derrotarlo activa **la secuencia final dentro de la misma escena**: cámara muestra a la protagonista ocupando el trono; la cabeza del Jety, presentada de forma estilizada/no explícita, termina en una lanza como símbolo de su nueva identidad.
- Pantalla «Fin» / «Has ocupado el trono», con opción **Volver al menú** (y créditos accesibles desde menú o final).
- No ofrecer una salida heroica que contradiga el giro narrativo.

### 3.5 Nivel intermedio opcional
Solamente si el flujo completo está estable, se cumplieron oleadas, armas, arte y build. Agregar entre 02_IceWorld y 03_JetyRoom sin modificar el resto de la progresión; temática y monstruos se definen después.

## 4. Pantallas y HUD

| Pantalla/estado | Elementos mínimos | Acción |
| --- | --- | --- |
| Menú principal | Logo/título, Jugar, Controles, Salir PC | Jugar inicia tienda |
| Controles (overlay) | Heladería: WASD + mouse para mirar, E interactuar. Mundo Jety: WASD relativo a la cámara lateral (sin mirar con mouse), E recoger/abrir, clic izq atacar, Q bomba (pendiente de prueba), Esc pausa | Volver |
| Tienda POV | Retícula central, instrucción «E: servir», contador de existencias o ventas simples | Atender, luego ir a bodega |
| Bodega | Retícula, «E: abrir refrigerador» | Dispara transición |
| Transición de portal | Fundido/mensaje breve | Carga mundo de helados |
| HUD combate | Vida, arma principal equipada, bombas restantes, progreso de oleada, retícula | Sin interacción UI compleja |
| Cofre / arma | Mensaje contextual «E: abrir/recoger» y nombre del arma | Interactuar |
| Jefe nivel 1 | Barra de vida de jefe, objetivo claro | Derrotar |
| Sala del Jety | Barra de vida Jety, vida propia y arma/bombas | Derrotar |
| Pausa | Continuar, Reiniciar nivel, Volver al menú | Pausa/reinicio |
| Derrota | «Has caído», Reintentar, Menú | Reiniciar nivel/oleada según decisión |
| Final | Trono, mensaje final, Volver al menú, créditos | Cierra partida |

**No crear una pantalla de inventario, tienda de armas, selección de nivel, guardado, opciones avanzadas ni tutorial complejo en MVP.** Controles puede ser un panel, no otra escena.

## 5. Combate — especificación inicial

### Jugador
- En tienda: WASD + mirar con mouse + E interactuar. En combate: WASD relativo a la cámara 3D lateral/elevada, sin free-look de mouse, clic izquierdo atacar, E interactuar/recoger. La dirección del ataque depende de orientación del personaje, no del cursor.
- Vida visible y daño con invulnerabilidad breve o temporizador de golpes para evitar múltiples colisiones instantáneas.
- Dos controladores de movimiento/cámara por contexto: POV de tienda y personaje visible de combate lateral 3D. Compartir solo el estado relevante (vida/equipamiento/progresión cuando corresponda); no forzar un controlador FPS único.
- Derrota reintenta nivel; conservar progreso de oleadas o reiniciar desde el comienzo del nivel **pendiente de acuerdo**. Sugerencia MVP: reiniciar fase actual o checkpoint de jefe sin guardado permanente.

### Arma principal: cucharón
- Equipamiento inicial al llegar al mundo de helados.
- Ataque corto tipo espada; tiempo de recarga sencillo y rango definido con hitbox/raycast.
- Debe alcanzar a enemigos cercanos sin exigir animación avanzada.

### Arma principal alternativa: lanza de dulce
- Puede encontrarse en cofre. **Reemplaza** al arma principal equipada, no ocupa un segundo slot.
- Se lanza como proyectil; una vez lanzada queda en el escenario, sin regresar automáticamente.
- Para volver a usarla, el jugador debe recuperarla acercándose y presionando E.
- Si el jugador suelta el cucharón u otra arma al equiparse la lanza, el arma previa queda físicamente en el suelo y puede recuperarse después.
- Si lanza la lanza y está desarmado, puede recoger otra arma del suelo.
- No implementar inventario de múltiples armas en MVP: exactamente **una principal equipada** y pickups persistentes en la escena hasta salir de nivel.

### Arma secundaria: bombas de chicle
- Recurso consumible, con contador en HUD.
- Lanzamiento mediante tecla acordada (sugerencia Q), trayectoria simple, explosión de área y daño a enemigos.
- Cofres pueden reponer bombas. No añadir fabricación, moneda ni upgrades en MVP.
- Si contador está en 0 no se puede lanzar.

### Cofres
- Cofres prefijados manualmente por nivel (no botín aleatorio).
- Una interacción E abre y entrega una recompensa definida: lanza o bombas.
- Sin mecánicas de rareza, economía ni sistema extensible de loot hasta validar lo fundamental.
- **Fase inicial de recorrido completo usa cucharón solamente**; luego se incorporan lanza, drops, bombas y cofres.

## 6. Enemigos y balance (valores sujetos a prueba)

| Enemigo | Cantidad a derrotar | Comportamiento mínimo | Riesgo |
| --- | ---: | --- | --- |
| Helado pequeño | 23 | Acercarse y dar daño de contacto o golpe sencillo | Bajo individual |
| Helado grande | 10 | Más vida, movimiento más lento, golpe con aviso | Medio |
| Jefe helado gigante | 1 | Más vida, ataque reconocible, barra de jefe | Alto |
| Jety | 1 | Patrón simple con aviso, ventana vulnerable | Jefe final |
| Secundarios sala del Jety | 0 obligatorios | Solo si tiempo permite | Opcional |

No generar 23+10 enemigos juntos; la progresión se cuenta por derrotas, no por total simultáneo. Para la **primera prueba de recorrido completo**, usar 1 pequeño + 1 grande + 1 jefe helado + Jety; después ampliar a 23 y 10 sin alterar el flujo.

## 7. Reglas de fase y victoria
- La tienda termina solo al interactuar con refrigerador después de agotar existencias.
- El nivel 1 avanza en orden: **pequeños (23) → grandes (10) → jefe helado (1)**.
- Cada muerte se contabiliza exactamente una vez.
- Enemigos de oleadas posteriores NO se activan antes de la condición previa.
- Puerta/portal a sala Jety se activa solo al derrotar al jefe helado.
- Al derrotar al Jety, se bloquean controles de combate y comienza el final.
- Juego terminado solo cuando se ve la secuencia de trono y el usuario puede volver al menú.

## 8. Estilo y dirección artística
- 3D estilizado, colores de dulces/helados para nivel 1; cámara y atmósfera más ominosas hacia el Jety.
- Horror fantástico liviano y humor absurdo, sin gore explícito.
- Cami define guías de silueta, paleta, proporciones y referencias. Arte debe servir al gameplay (enemigos pequeños vs grandes distinguibles).
- Separar archivos fuente (.blend) de exportaciones (.fbx); rigs para personajes realmente animados, no para props estáticos.
- Cada enemigo tiene prefab visual propio reemplazable sin reescribir IA.

## 9. MVP / recortes obligatorios
**MVP vertical:** se puede jugar el recorrido entero con prefabs provisionales, transiciones simples, una mecánica de ataque, jefes básicos y final de trono.
**Versión objetivo:** 23+10, jefe helado, Jety, interfaz clara, arte de Cami, cofres y armas si alcanzan.
**Opcional:** nivel intermedio, secundarias enemigas en sala del Jety, efectos/animaciones avanzadas.

Orden de recorte ante retrasos: quitar nivel intermedio → enemigos extra final → cosméticos → física elaborada de cofres/bombas → lanza. **Nunca recortar el final, transición inicial, jefes o la posibilidad de completar una partida.**

## 10. Decisiones que el equipo debe cerrar antes de implementar
1. Versión exacta Unity y plantilla 3D del repositorio (no inventar).
2. **Confirmado por el equipo:** POV en tienda/bodega y cámara lateral elevada/3-4 en el mundo del Jety (referencia *Fairytale Fights*). Pendiente de validar ángulo, seguimiento y movimiento en profundidad mediante prueba corta en Unity.
3. ¿Cuántos clientes sirven al inicio? sugerencia 2.
4. Diseño mínimo de Jety y del jefe helado (Cami puede entregar referencias posteriormente).
5. ¿Reinicio desde nivel, oleada o checkpoint? sugerencia nivel/checkpoint.
6. ¿Plataforma final Windows, WebGL o ambas? según viabilidad.
7. Reglas de licencia de modelos originales de Cami (MIT del código no se extiende automáticamente al arte).

La documentación antigua de docs/GAME_DESIGN.md describe una **prueba técnica anterior de recolectar 3 objetos** y no debe interpretarse como el diseño definitivo de este juego. Este documento es propuesta del juego definitivo hasta aprobación del equipo.
