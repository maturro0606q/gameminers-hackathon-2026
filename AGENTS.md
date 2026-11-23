# Reglas obligatorias para GameMiners

1. Trabaja fuera de `main`. Las excepciones son la inicialización controlada y los merges que apruebe Matías después de la revisión.
2. Usa la rama de tu integrante. Matías usa `feature/ui`, Nacho usa `feature/gameplay` y Cami usa `feature/art`. Confirma la rama antes de editar.
3. Coordina con la persona responsable antes de modificar sus archivos. No sobrescribas trabajo ajeno.
4. Nacho administra la escena principal de gameplay y realiza sus cambios de composición.
5. Matías mantiene las interfaces en prefabs y scripts independientes. Coordina con Nacho para montar la UI en la escena principal.
6. Cami administra los recursos visuales de `Assets/Art/`.
7. Versiona todos los archivos `.meta` de Unity asociados a archivos o carpetas versionados. Conserva sus GUID cuando muevas o renombres recursos.
8. Excluye `Library/`, `Temp/`, `Logs/`, cachés, compilaciones y secretos.
9. Verifica la licencia de cada asset externo antes de incorporarlo y conserva el registro de origen, permisos y atribución cuando corresponda.
10. Implementa solo tareas autorizadas. El prototipo requiere confirmación de preparación terminada y autorización explícita de Matías. El juego definitivo requiere su aprobación posterior a la validación en Unity y fuera del editor.
11. Abre un pull request hacia `main` y obtén revisión y aprobación antes de fusionar trabajo de otra persona.
12. Cada tarea incluye alcance, responsable y un criterio de finalización verificable.
13. Registra en cada cambio el resumen de archivos afectados y las pruebas realizadas cuando correspondan. Indica qué prueba sigue pendiente si dependes de otro cambio o equipo.
14. Solicita autorización antes de eliminar o regenerar el proyecto Unity para resolver errores.

Formato de commits:

- `feat(ui): crear menú inicial`
- `feat(gameplay): implementar movimiento`
- `feat(art): agregar modelo de prueba`
- `fix: corregir integración`

Consulta [la arquitectura](docs/ARCHITECTURE.md) para las rutas de trabajo y [la guía de integración](docs/INTEGRATION.md) para la revisión de cambios.

## Derechos de autor y arte original (propuesta pendiente de aprobación)
- El repositorio público distribuye código original bajo MIT. **MIT no se aplica al arte original propietario de Cami** ni cambia licencias de música/terceros.
- **Prohibido subir archivos privados de arte original a cualquier rama o PR público** (incluye .blend, .fbx, ilustraciones, texturas y sus versiones). Cami debe autorizar expresamente cualquier excepción de publicación.
- Usar placeholders redistribuibles en las escenas/prefabs que queden públicos, evitando referencias obligatorias a archivos privados. Para build con arte de Cami, obtener permiso específico de uso y distribución.
- Todo audio de terceros requiere autor, fuente, licencia y crédito cuando corresponda, antes de importarlo o subirlo. Ver docs/LICENSING_POLICY.md y docs/MUSIC_SHORTLIST.md.
