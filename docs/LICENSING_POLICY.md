# GameMiners — Política de licencias y derechos (propuesta)
> V0.3 · 23/11/2026 · No definitiva hasta confirmación del equipo y de Cami.

## 1. Matriz oficial de referencia
| Tipo de archivo u obra | Titular y régimen | Uso permitido por terceros / destino | Documento |
| --- | --- | --- | --- |
| Scripts C# y código original GameMiners | Sus autores/contribuyentes; **MIT** | Copia, modificación, distribución, uso comercial según MIT con preservación del aviso | `/LICENSE` |
| Modelos originales, FBX, animaciones y texturas de Cami | **© 2026 Cami, derechos reservados** | Archivo visible en GitHub si ella lo autoriza; NO equivale a CC BY ni a permiso general de reutilización independiente | `docs/ART_CREDITS.md` |
| Fuentes editables (.blend, PSD, bocetos) | Autora identificada | Privados salvo aprobación expresa individual | `docs/ART_PERMISSION_DRAFT.md` |
| Música y SFX CC0 de terceros | Respectivos autores, declaración CC0 | Distribución permitida conforme a CC0 tras verificar fuente/obra concreta; crédito voluntario por política del equipo | `docs/AUDIO_LICENSES.md` y `docs/THIRD_PARTY_NOTICES.md` |
| Template URP de Unity y componentes/paquetes de terceros | Unity y autores respectivos | Sus términos específicos (p. ej. Unity Companion License); NO pasan a MIT | `docs/THIRD_PARTY_NOTICES.md` |
| Marcas/escudos/logos externos, incluido Codex | Titulares de marcas | Solo usos permitidos por guía/permiso; no asumir por estar en un repo MIT | `docs/THIRD_PARTY_NOTICES.md` |

**La fórmula exacta es «código open source bajo MIT, arte original con derechos reservados, música CC0 y recursos externos según su licencia».** No llamar «videojuego 100% open source» al conjunto si el arte permanece propietario.

## 2. Avisos: dónde aparecen
- **Raíz:** `LICENSE` MIT original, sin modificar su texto normativo.
- **Raíz:** `README.md` con un resumen de licencias mixtas y enlaces.
- **Arte:** `docs/ART_CREDITS.md` con © y nombre de Cami, además de inventario autorizado en `docs/ART_PERMISSION_DRAFT.md` cuando se complete.
- **Pantalla del juego:** `docs/CREDITS_IN_GAME.md` como guion de créditos de Matías, Nacho, Cami y autores musicales.
- **Terceros:** `docs/THIRD_PARTY_NOTICES.md` como registro por archivo, con licencia y URL original.
- **Audio:** `docs/AUDIO_LICENSES.md` como mapa exacto de pistas elegidas y créditos musicales.

## 3. Autorización artística y alcance
- Cami conserva los derechos de su arte; publicar con créditos no se entiende como licencia CC BY 4.0. CC BY requeriría que ella acepte permitir reutilización y uso comercial por terceros.
- Su aprobación debe cubrir **publicación de archivos**, **integración técnica**, **distribución de builds**, **capturas/videos** y, si aplica, **posibilidad de clonar/ejecutar el proyecto con arte incluido**. No tratar el simple upload de un FBX como acuerdo de explotación del juego.
- Mantener identificación y evidencia de consentimiento en registro privado. Para obras concretas previamente aprobadas en PR de arte, conservar el alcance de esa aprobación; no presumir permiso general para futuras obras.
- Reutilizaciones externas y una eventual venta del videojuego requieren revisar permisos y la titularidad.
- GitHub permite visualizar/forkear contenido de repositorios públicos conforme a sus términos; un aviso de derechos reservados no lo hace confidencial ni elimina tales permisos de plataforma.
- Confirmar con organizadores qué significa el requisito «open source»: código MIT solamente o juego íntegro con assets reutilizables.

## 4. Música y créditos
- Pistas elegidas: tienda Ice Cream Truck Theme; helados Magic Puzzle In-Game 1; Jety Boss Battle Music; final Spooky Waltz.
- Cada archivo: autor real según ficha, título, licencia, URL original, versión/nombre de archivo, fecha y transformaciones (p. ej. WAV→OGG). Conservar prueba de licencia al momento de descarga.
- CC0 no exige atribución como condición de copyright; **GameMiners la dará voluntariamente**. Sin embargo, CC0 no concede derechos de marca, imagen ni otras titularidades ajenas.
- No atribuir a GameMiners la propiedad de música CC0 de terceros.

## 5. Marca Codex y referencias culturales
- El nombre/logo de Codex no se licencia por MIT/CC0. Las guías de OpenAI desaconsejan integrar sus logos como marca propia. El letrero de heladería previsto debe revisarse/autorizarse con organizadores o reemplazarse con una marca original, pudiendo mencionar la herramienta en créditos descriptivos. https://openai.com/brand/
- El conejo se inspira en lenguaje general de títeres, sin copiar personajes, marcas, voces ni música de 31 Minutos.
- Ninguna imagen, personaje, canción o modelo externo se incorpora por mero hallazgo en internet.

## 6. Checklist para entregar
- [ ] La titularidad del código/contribuciones y © del LICENSE están correctamente representados.
- [ ] El equipo valida compatibilidad con las bases de la hackatón.
- [ ] Cami acepta el listado de FBX públicos y el alcance del permiso para la build/jurado.
- [ ] Se define si terceros pueden clonar/ejecutar la fuente con assets propietarios o se ofrecen placeholders.
- [ ] No hay originales .blend / bocetos privados en ramas públicas sin autorización.
- [ ] Cada música/efecto real tiene licencia y URL registradas.
- [ ] Los créditos dentro de Unity coinciden con los archivos realmente usados.
- [ ] Se comprueba el logo de Codex o se sustituye por marca original.
- [ ] La distribución final incluye LICENSE y avisos de terceros.
