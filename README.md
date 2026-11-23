# La Heladería Maldita

Un juego de GameMiners para la Hackatón Codex Chile 2026.

## Jugar en Windows

1. Abre [GitHub Releases](https://github.com/maturro0606q/gameminers-hackathon-2026/releases) y descarga `La_Heladeria_Maldita_Creditos_Windows.zip` desde **Assets**.
2. Extrae la carpeta completa y abre `GameMiners.exe`. Mantén `GameMiners_Data` junto al ejecutable. Puedes jugar sin instalar Unity.

Usa **WASD** para moverte, **E** para interactuar y **F11** para cambiar a pantalla completa. Consulta [los controles completos y los créditos de la entrega](docs/ENTREGA_WINDOWS.md).

| Integrante | Responsabilidad | Rama |
| --- | --- | --- |
| Matías | Coordinación, arquitectura funcional, UX/UI, repositorio e integración | `feature/ui` |
| Nacho | Programación y mecánicas de gameplay | `feature/gameplay` |
| Cami | Dirección artística, dibujo, modelado 3D y recursos visuales | `feature/art` |

Tecnologías del equipo: Unity Hub y Unity Editor, C#, Git y GitHub, GitHub Desktop, Codex y Blender para recursos 3D. Git LFS se usará cuando el equipo acuerde incorporar binarios grandes.

## Abrir el proyecto en Unity

1. En Unity Hub, instala la versión indicada en [ProjectVersion.txt](ProjectSettings/ProjectVersion.txt) con soporte de compilación para Windows y agrega la raíz del clon.
2. Abre `Assets/Gameplay/Scenes/00_Menu.unity` y pulsa Play.
3. Para compilar desde el editor, usa **GameMiners → Compilar Heladería Maldita para Windows**. La carpeta de salida es `Builds/Heladeria-Maldita`.

Para contribuir, lee [AGENTS.md](AGENTS.md) y [la guía de integración](docs/INTEGRATION.md).

Para seguir el trabajo, consulta [diseño](docs/GAME_DESIGN.md), [tareas](docs/TASKS.md) y [pruebas](docs/TESTING.md).

El código original se publica bajo [licencia MIT](LICENSE). El [arte original de Cami](ART_LICENSE.md) y los [recursos de terceros](docs/THIRD_PARTY_NOTICES.md) conservan las condiciones de sus respectivas licencias.
