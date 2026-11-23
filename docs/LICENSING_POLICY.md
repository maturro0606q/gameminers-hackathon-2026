# GameMiners — Licencias del código, arte público y audio
> PROPUESTA V0.2 · 23/11/2026 · Requiere visto bueno de Cami antes de aplicarse a sus obras.

## Resumen de la decisión del equipo
| Material | Política | Disponibilidad |
|---|---|---|
| Código propio (C#, herramientas, scripts) | **MIT**, en `LICENSE` | Público y reutilizable de acuerdo con MIT |
| Arte original de Cami (FBX, texturas, materiales y modelos de los personajes) | **Visible públicamente con crédito a su autora; derechos reservados** (salvo una licencia adicional expresa) | Se puede descargar desde el repo público, pero NO se concede por ello permiso para reutilizarlo en otros proyectos |
| Archivos de creación (dibujos fuente, .blend, .psd) | **Por defecto privados**, salvo que Cami elija publicarlos | No incluir sin consentimiento explícito |
| Música de OpenGameArt seleccionada | **CC0 según la ficha de cada pista** | Puede distribuirse si se verifica la fuente y el archivo correcto |
| Otros recursos de terceros | Licencia propia | Analizar cada caso |

**Importante:** «público con créditos» no equivale a «CC BY 4.0». CC BY concede reutilización, modificación y también uso comercial con atribución. El acuerdo actual es **archivos públicos, autora acreditada y sin licencia general de reutilización**. Solo Cami puede autorizar una licencia más permisiva sobre sus creaciones. Fuente: https://creativecommons.org/licenses/by/4.0/

## 1. Código
- `LICENSE` en raíz permanece como **MIT**, sin añadir excepciones dentro del texto MIT.
- Los recursos artísticos y musicales están cubiertos por sus propios avisos, no por la licencia MIT del código.
- Al describir GameMiners como proyecto, distinguir **código abierto con arte de licencia diferente**; no afirmar que todo el material sea open source.

## 2. Arte público con crédito de Cami
- Cami aprueba primero los archivos concretos que se subirán. Ningún agente interpreta este documento como consentimiento o cesión irrevocable de derechos.
- Publicar `*.fbx` y sus materiales/texturas bajo `Assets/Art/**` (en `feature/art`) cuando estén aprobados y listos para importar; conservar los `.meta` generados por Unity.
- Mantener `.blend` y dibujos originales fuera del repositorio público, salvo aprobación individual.
- Incorporar el aviso propuesto `ART_CREDITS.md` en el repositorio y los créditos dentro del juego; indicar el nombre o firma artística elegida por Cami.
- Aviso de derechos reservados no impide que terceros descarguen los archivos, solo señala que no se concede autorización de reutilización. El repositorio abierto NO es confidencial.
- Para que GameMiners pueda redistribuir los archivos y la build, Cami debe otorgar autorización expresa de uso/publicación al equipo; no presume renuncia a autoría.
- Si Cami realmente desea permitir reutilización con créditos, se puede cambiar a **CC BY 4.0**, que también permite usos comerciales. Eso exige su decisión explícita.
- **Pendiente crítico:** confirmar si el reglamento del evento exige que TODO asset usado en el proyecto completo sea open source; una política de derechos reservados podría no satisfacer esa condición.

## 3. Música
La selección queda en `docs/MUSIC_SHORTLIST.md`. Se recomienda tener 3 pistas en primera iteración: tienda, mundo helados y Jety.
- Descargas exclusivamente desde la ficha de la obra original. Verificar licencia CC0 y nombre exacto de archivo.
- Registrar `docs/AUDIO_LICENSES.md` antes de importar cada pista (autor, título, URL, licencia, nombre de archivo y fecha).
- Guardar los `.ogg` en `Assets/Audio/Music/` mediante PR separada del equipo.
- Audio en Unity: una fuente 2D por escena, `Loop=true`, `Play On Awake=true`. Evitar duplicados al cambiar escenas y cambios de escena hechos por agentes simultáneos.
- Créditos dentro del juego y en README (aunque CC0 no los exige legalmente); música CC0 no se transforma en música exclusiva de GameMiners.
- Preferir no subir archivos de audio hasta comprobar que la ficha corresponde al archivo real descargado.

## 4. Publicación y pruebas
- Código bajo MIT (root `LICENSE`).
- Arte público aprobado por Cami y acreditado, sin reclamar exclusividad o licencia CC BY.
- Música con créditos y documentos de fuente.
- Build probada con audio 2D en tienda, IceWorld y sala del Jety.
- Distribución comercial futura requerirá revisar permisos de arte y de cualquier tercero.

## Estado
**Propuesta pendiente de revisión**, no incorporación definitiva ni permiso implícito para publicar las obras de Cami.
