# GameMiners · Hackatón 2026

Proyecto colaborativo del equipo GameMiners para la hackatón de videojuegos. La primera entrega será una prueba técnica para integrar el trabajo de tres computadores en un mismo proyecto Unity, ejecutarlo en el editor y generar una versión jugable.

| Integrante | Responsabilidad | Rama |
| --- | --- | --- |
| Matías | Coordinación, arquitectura funcional, UX/UI, repositorio e integración | `feature/ui` |
| Nacho | Programación y mecánicas de gameplay | `feature/gameplay` |
| Cami | Dirección artística, dibujo, modelado 3D y recursos visuales | `feature/art` |

Tecnologías del equipo: Unity Hub y Unity Editor, C#, Git y GitHub, GitHub Desktop, Codex y Blender para recursos 3D. Git LFS se usará cuando el equipo acuerde incorporar binarios grandes.

## Abrir el proyecto

1. Lee [AGENTS.md](AGENTS.md) y [la guía de integración](docs/INTEGRATION.md).
2. Instala **Unity 6000.4.11f1** desde Unity Hub. La base usa **Universal 3D (URP)** y la primera plataforma de prueba es **Windows x64**.
3. Clona el repositorio y agrega la raíz del clon a Unity Hub con **Projects → Add → Add project from disk**. La versión exacta está en [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt).
4. Abre `Assets/Scenes/SampleScene.unity` y entra en Play Mode para comprobar la instalación. Acepta la invitación al repositorio antes de publicar cambios y usa la rama asignada.
5. Empieza el desarrollo cuando Matías confirme la validación de la base y autorice el hito correspondiente.

Para seguir el trabajo, consulta [diseño](docs/GAME_DESIGN.md), [tareas](docs/TASKS.md) y [pruebas](docs/TESTING.md).

El código original se publica bajo [licencia MIT](LICENSE). La [plantilla de Unity](docs/UNITY_TEMPLATE_LICENSE.md) conserva su licencia de origen.
