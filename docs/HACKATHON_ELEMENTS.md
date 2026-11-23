# GameMiners — Elementos obligatorios de identidad de la hackatón
> Brief aprobado por el equipo · 23/11/2026 · Aplica a La heladería maldita · Documento operativo para Work/Codex.
>
> **Objetivo:** representar cinco elementos en el juego de forma visible durante un recorrido completo, sin desarrollar nuevas mecánicas complejas ni bloquear el MVP.

## 1. Matriz de elementos y verificación

| ID | Elemento pedido | Momento exacto | Implementación mínima | Responsable de integrar | Responsable de arte | Prueba |
|---|---|---|---|---|---|---|
| H-01 | Frase **«Queda una pura...»** | Tienda, cuando queda **1 bola** de helado y ANTES de entrar a la bodega | Mensaje obligatorio **«¡Queda una pura bola de helado!»** cuando el stock baja a 1; al servir la última bola (stock = 0), objetivo «Busca helados en la bodega». Evitar otro minijuego | Nacho activa evento al llegar a stock = 1 y desbloquea bodega tras stock = 0; Matías muestra UI | Matías UI; Cami opcional tipografía/fondo | Aparece una sola vez cuando queda 1 bola; tras servirla, se agota el stock y se habilita la bodega |
| H-02 | **Número 23** | Primer instante tras entrar a 02_IceWorld, antes de la primera oleada | Conejo guía/NPC dice exactamente **«Oh, eres el heladero número 23 en venir aquí»**, mediante globo/subtítulo visible; interacción o trigger automático breve, sin árbol de diálogo | Nacho trigger de llegada y desbloqueo tras diálogo; Matías globo/subtítulos | Cami conejo guía | La línea se muestra en el primer arribo y no se repite al reiniciar la oleada. No confundir con contador 23 enemigos |
| H-03 | **Logo de Codex** | Heladería, visible al comenzar la partida | Letrero / rótulo de la tienda con logotipo Codex **oficial y sin alteraciones**, solo si su uso está permitido por bases y guía de marca; usar placeholder gráfico mientras se confirma. No implica patrocinio | Nacho coloca cartel en 01_Shop, Matías puede preparar señalización | Cami adapta cartel sin alterar marca oficial | La cámara POV deja verlo al menos una vez de forma clara al iniciar; uso de marca comprobado |
| H-04 | **Completo italiano** | Fondo de zona de combate (02_IceWorld), durante al menos una pelea | Conejo espectador sostiene un prop de **completo italiano** (pan, vienesa, tomate, palta, mayonesa); no interactivo, sin mecánica de comida | Nacho ubica un prefab de decorado en fondo, visible a la cámara lateral elevada | Cami modelo low-poly del completo y conejo con pose | Se ve en segundo plano durante la pelea sin ocultar enemigos ni afectar rendimiento |
| H-05 | **Concepto de 31 minutos** | Diseño del **conejo que recibe a la protagonista** en mundo del hielo | Conejo ORIGINAL con estética de **títere televisivo chileno**, rasgos expresivos, material tipo tela/fieltro, costuras, ojos grandes, personalidad humorística; **no copiar personajes, nombres, logos, música ni rasgos distintivos concretos de 31 Minutos** | Nacho instancia NPC del diálogo H-02 | Cami concepto, modelo y pose/rig sencillo si necesita hablar | El diseño conserva referencia general al lenguaje de títeres, pero tiene identidad y silueta propias |

## 2. Conejos de ambientación y pancartas (también obligatorios)
- Colocar varios **prefabs reutilizables de conejo espectador/protestante** al fondo de la zona de batalla de 02_IceWorld. Preferir 2–3 variantes de color, pose o accesorios a partir de una malla base, NO modelar cada conejo desde cero.
- Letreros legibles con texto **«No más conos»** y **«No más humanos»**. Ambos deben aparecer al menos una vez durante la primera pelea. Mantenerlos orientados hacia la cámara lateral/3-4.
- Algunos conejos pueden tener variaciones de animación idle simples (balanceo/gesto), pero los NPC de fondo no deben atacar, bloquear, perseguir ni tener lógica de IA en M1.
- Reutilizar el conejo base para uno que sostiene el completo italiano; el conejo receptor de H-02 debe ser distinguible y estar en primer plano durante su línea.
- Usar materiales y geometría low-poly de fácil importación. Si Cami aún no entrega recursos, Nacho coloca placeholders con texto y objetos primitivos: **no bloquear recorrido completo**.
- Verificar que las pancartas sean legibles desde la cámara lateral elevada, sin tapar al jugador ni al jefe.

## 3. Contratos entre responsables para no pisar archivos

### Nacho — gameplay y escenas
- Propietario de `Assets/Gameplay/**`: escena 01_Shop, trigger de stock agotado, colocación del letrero Codex, escena 02_IceWorld, trigger de aparición del conejo, escenas de combate y distribución de decoración.
- Agregar eventos/estados o llamadas simples para **H-01** y **H-02**; no implementar conversaciones ramificadas.
- Iniciar la primera oleada tras la presentación del conejo; durante M1 permitir omitir diálogo con tecla E o hacer espera breve, para acelerar las pruebas.
- Mantener cámara POV en tienda y cámara tercera persona lateral elevada en mundo del Jety. Probar que los conejos de fondo y pancartas estén visibles en el encuadre de combate.
- No tocar código UI ni objetos de arte fuente de Cami. Instanciar prefabs aprobados en la escena.

### Matías — UI
- Propietario de `Assets/UI/**`: mensaje de stock agotado, diálogo del conejo, instrucciones breves y, si corresponde, estilo de globos/subtítulos.
- Consumir las señales de gameplay H-01 y H-02 sin crear su propia lógica de inventario o triggers.
- Si no existe el contrato de diálogo, acordar con Nacho un mínimo `ShowMessage(text)` o campos del snapshot vigente, sin definir APIs paralelas.
- Asegurar que el texto no tape objetivo, barra de vida ni acción E; el mensaje del conejo puede aparecer en panel breve al entrar.

### Cami — arte
- Propietaria de `Assets/Art/**` en `feature/art`, siempre respetando aprobaciones/licencia.
- Prioridad artística tras protagonista y primer enemigo: conejo guía original estilo títere, conejo de público con variaciones, pancartas, completo italiano y cartel/espacio reservado para marca Codex.
- Archivos FBX y texturas que autorice publicar; `.blend` y bocetos según política de arte (privados por defecto). No subir representaciones no autorizadas de marcas.
- Para conejos de fondo no necesita rig completo; basta modelo + pose/idle simple. Rig básico de cabeza/brazos para conejo guía si la escena lo requiere.

## 4. Orden de ejecución mínimo
1. **Sin esperar modelos:** H-01 texto en tienda; H-02 subtítulo y conejo proxy al entrar; H-03 cartel placeholder; H-04 completo proxy + conejo al fondo; H-05 conejo proxy y letreros de protesta texto visible.
2. **Comprobar recorrido completo** de tienda hasta trono con cámara POV→lateral 3D.
3. Sustituir conejo guía, completo, fondo y cartel por arte propio de Cami, sin cambiar scripts.
4. Pulir subtítulos, perspectiva y lectura de carteles. Mantener NPC de fondo como decorado.

## 5. Checklist de aceptación de hackatón
- [ ] En `01_Shop` se muestra textualmente «¡Queda una pura bola de helado!» cuando queda **1 bola**; luego se sirve la última y se habilita la bodega.
- [ ] El cartel de Codex se ve al menos una vez en la heladería. Antes de usar el logotipo oficial, verificar permiso y directrices.
- [ ] Tras el refrigerador, en `02_IceWorld`, un conejo recibe a la protagonista con «Oh, eres el heladero número 23 en venir aquí».
- [ ] El conejo guía posee **diseño original con estética de títere** que remite conceptualmente a *31 Minutos* sin copiar personajes.
- [ ] Durante el combate se ve al menos un conejo de fondo que lleva un completo italiano.
- [ ] En la arena hay conejos de fondo con carteles legibles «No más conos» y «No más humanos».
- [ ] No se altera el balance/código de combate ni se crean mecánicas adicionales para estos elementos.
- [ ] Se prueba en Unity Play Mode y, antes de entregar, en build.

## 6. Uso de marcas y referencias
- **Codex:** el nombre/logo es marca ajena; emplear activos oficiales solo si la política de marca y las reglas de la hackatón permiten el caso de uso. Mantener integridad del logotipo, sin distorsión ni presentar el juego como producto/patrocinio oficial de OpenAI. Fuente: https://openai.com/brand/.
- **31 Minutos:** la referencia es a un lenguaje visual amplio (títeres, humor y expresividad); evitar cualquier reproducción de personajes preexistentes, nombres, voz, elementos protegidos o música.
- **Completo italiano:** comida común representable mediante un prop original, sin marcas comerciales.
- Estas recomendaciones no sustituyen autorización cuando corresponda.

## 7. Conexión con diseño global
Leer `docs/ICE_GAME_GDD.md`, `docs/ICE_GAME_IMPLEMENTATION.md`, `docs/ICE_GAME_WORK_PROMPTS.md` y `docs/ICE_GAME_CAMERA_ARCHITECTURE.md`. Este documento concreta los hitos de identidad visual/narrativa sin modificar el juego principal. **No reemplaza el prototipo técnico antiguo** ni autoriza inventar escenas adicionales.
