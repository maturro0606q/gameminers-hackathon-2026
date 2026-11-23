# GameMiners — Política de derechos y licencias (acuerdo de trabajo)
> Actualizado 23/11/2026. Aplicable al desarrollo de **La heladería maldita**. Registro de autorización de Cami: `docs/ART_USE_PERMISSION.md`.

## 1. Qué está autorizado y bajo qué términos

| Componente | Titularidad / condiciones | Alcance |
| --- | --- | --- |
| Código original C# de GameMiners | **MIT**, en `LICENSE` | Open source: copiar, estudiar, modificar y redistribuir código conforme a MIT |
| Arte original creado por Cami | **© 2026 Cami · Todos los derechos reservados**, en `ART_LICENSE.md` | **Autorizado únicamente para este proyecto** GameMiners. No para otros juegos, empresas o proyectos; no se convierte en MIT/CC0/CC BY |
| FBX, texturas, rigs y materiales de Cami publicados | Mismo copyright del arte | Pueden ser públicos **cuando su publicación para GameMiners haya sido autorizada** y siempre con crédito. La visibilidad no concede reutilización externa |
| Fuentes artísticas .blend, bocetos y PSD | Cami | Privados salvo permiso expreso de la autora |
| Música/SFX escogidos por terceros | CC0 cuando lo declara el recurso original y se verifica | Usables en el juego; GameMiners acredita voluntariamente a sus creadores |
| Unity URP, paquetes, fuentes, otros assets externos | Licencias de sus titulares | No se convierten automáticamente a MIT |
| Logo/nombre Codex y otras marcas | OpenAI y titulares respectivos | Usos sujetos a directrices y permisos; la licencia del proyecto no los cubre |

**Forma correcta de describir la entrega:** «Código abierto MIT, arte original de Cami con derechos reservados y permiso de uso exclusivo para este videojuego, música de terceros CC0 y recursos de terceros conforme a sus licencias». El juego completo **no** está licenciado íntegramente bajo MIT.

## 2. Autorización de Cami: resolución recibida
Matías informó al equipo el **23/11/2026**: **«Cami dijo que podían usar su arte solo para este proyecto»**. Se toma como acuerdo operativo para el desarrollo y presentación de La heladería maldita, con crédito a Cami y derechos reservados. Su consentimiento fue comunicado por un integrante; **no se afirma que exista contrato firmado ni se inventan derechos adicionales**.

**Dentro de este proyecto:** integración técnica de assets entregados para él; montaje en Unity, optimización, rigging/animación y uso en la presentación de hackatón; publicación de exportaciones concretas ya aprobadas en sus PR artísticos.

**No concedido:** reutilizar en otros proyectos, vender assets por separado, publicar source editable no autorizado, conceder una licencia abierta a terceros, secuelas ni comercialización general no discutida.

**No resuelto por la frase «solo para este proyecto»:** autorización detallada de sublicencias externas, distribución comercial futura y términos específicos exigidos por un marketplace. Pedir consentimiento explícito si surgen; esto no bloquea el desarrollo de esta hackatón.

Ver `ART_LICENSE.md`, `docs/ART_CREDITS.md` y `docs/ART_USE_PERMISSION.md`. El antiguo `docs/ART_PERMISSION_DRAFT.md` queda como material histórico y NO reemplaza la autorización comunicada.

## 3. ¿Qué puede hacer quien clona el repositorio?
- El código original está sujeto a MIT.
- Los archivos artísticos públicos **pueden verse y forkease mediante las funcionalidades de GitHub**, de acuerdo con sus términos; esto no concede al público permiso para extraer los personajes y llevarlos a otro videojuego.
- La facultad de compilar una copia y redistribuir un **juego con los assets privados de terceros** no equivale a permiso de comercializar o relicenciar el arte. Antes de invitar formalmente a terceros a publicar builds derivados, precisar esa autorización.
- Si las bases de la hackatón exigen **todo** el juego bajo licencia OSI/open source (incluidos assets), deberá aclararse con los organizadores. «Código abierto» no implica que el arte tenga que ser abierto, pero tampoco satisface automáticamente una exigencia que se refiera a **todo** el proyecto.

## 4. Documentos que deben acompañar la entrega
- `LICENSE` — MIT estándar, **mantener intacto**.
- `ART_LICENSE.md` — derechos reservados sobre arte original de Cami.
- `README.md` — tabla/resumen y links de derechos.
- `docs/ART_USE_PERMISSION.md` — alcance del permiso comunicado y PRs de arte.
- `docs/ART_CREDITS.md` — aviso artístico.
- `docs/CREDITS_IN_GAME.md` — créditos mostrados en Unity.
- `docs/AUDIO_LICENSES.md` y `docs/THIRD_PARTY_NOTICES.md` — autores, archivos, fuentes y licencias externas.
- `docs/UNITY_TEMPLATE_LICENSE.md` de PR #2 — aviso de plantilla de Unity, cuando se fusione.

## 5. Créditos acordados
**Matías:** coordinación, arquitectura funcional, interfaz UX/UI e integración. **Nacho:** gameplay, mecánicas y escenas. **Cami:** dirección artística, personajes, modelado y animaciones. Los créditos exactos deben corresponder a las contribuciones integradas, sin atribuir recursos ajenos.

**Música seleccionada**: tienda — Ice Cream Truck Theme (Cleyton Kauffman); helados pequeños y grandes — Magic Puzzle In-Game 1 (MintoDog); Jety — Boss Battle Music (Juhani Junkala / publicación SubspaceAudio, confirmar denominación exacta); trono — Spooky Waltz (Aureolus_Omicron). Todas bajo **CC0 según ficha**; verificar archivo antes de incluirlo y acreditar por cortesía.

## 6. Marcas y referencias culturales
El logo Codex que se desea mostrar en la heladería es **marca de terceros**. La política de OpenAI no autoriza automáticamente usarlo como marca propia de otra tienda; **no incorporarlo sin autorización aplicable**, aunque se pueda acreditar a Codex como herramienta usada. https://openai.com/brand/

La estética de títeres de 31 Minutos es inspiración general; no copiar personajes, logotipos, voces, sonidos ni música ajena.

## 7. Control antes de distribuir la build
- [x] Licencia MIT original en raíz.
- [x] Permiso de uso del arte **solo en GameMiners** registrado según comunicación del equipo.
- [x] Aviso `ART_LICENSE.md` y créditos por separado.
- [ ] Verificar que cada FBX incorporado corresponda a un recurso entregado para este juego y autorizado para publicación.
- [ ] Confirmar los derechos requeridos por las bases de la hackatón y cualquier distribución externa más allá de la presentación.
- [ ] Verificar el permiso de marca Codex o utilizar cartel original.
- [ ] Audio y SFX realmente importados tienen registro de autor/licencia.
- [ ] Crédito mostrado en Unity y probado.
- [ ] La versión que se entregue incluye los avisos legales y ha sido probada fuera del editor.
