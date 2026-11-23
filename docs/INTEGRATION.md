# Integración con Unity y GitHub

## Preparar la primera copia

1. Confirmen versión exacta y plantilla. Matías registra el acuerdo en [ARCHITECTURE.md](ARCHITECTURE.md) y confirma el módulo previsto para la compilación.
2. Matías clona `main` con GitHub Desktop. Desde Unity Hub crea el proyecto en una carpeta temporal con la versión y la plantilla acordadas. Guarda los ajustes del editor, cierra Unity y copia las carpetas `Assets/`, `Packages/` y `ProjectSettings/`, con sus `.meta` y la configuración necesaria, a la raíz del clon. Conserva la configuración Git ya publicada.
3. Matías abre la copia del clon en Unity Hub, comprueba la importación y revisa los cambios en GitHub Desktop. Publica el commit de inicialización de Unity, asegurándose de que entre el manifiesto de paquetes y queden fuera los archivos generados.
4. Matías invita a Nacho y Cami desde GitHub en **Settings → Collaborators → Add people**. Tras aceptar la invitación, cada colaborador clona el mismo repositorio, abre la raíz del clon con Unity Hub e instala la versión anunciada si hace falta.
5. Tras publicar la base, Matías comprueba que las ramas `feature/ui`, `feature/gameplay` y `feature/art` parten de ese commit. Si ya existen y tienen trabajo, actualízalas preservando sus commits. Cada integrante comprueba su rama antes de editar.

## Revisar cambios

- Actualiza desde `origin` y abre un pull request desde tu rama hacia `main`. Incluye tarea, archivos afectados, pasos y resultados de prueba.
- Antes de integrar, revisa escenas, prefabs, archivos `.meta`, referencias, cambios de paquetes y ajustes. Si dos ramas afectan al mismo YAML serializado, pide a su responsable que resuelva el conflicto, abre el recurso en Unity y comprueba las referencias.
- Matías integra los PR revisados y aprobados. Una vez integrado, en GitHub Desktop cambia a `main`, ejecuta **Fetch origin** y **Pull origin** si hay cambios remotos. Cada integrante sincroniza su rama con la base antes de continuar.
- Para validar el commit integrado, registra la prueba en Unity y el resultado del build en [TESTING.md](TESTING.md).

## Binarios grandes

Acuerda con los tres equipos los formatos o archivos que necesitan Git LFS antes de incluirlos. Quien vaya a preparar un binario grande puede usar `git lfs track --filename "Assets/Art/ruta/al/archivo.fbx"` y versionar `.gitattributes` junto al primer commit del binario. Antes de abrir en Unity un clon con assets LFS, comprueba que Git LFS esté operativo y que el archivo se haya descargado completo. Los assets de prueba pequeños pueden viajar por Git normal.
