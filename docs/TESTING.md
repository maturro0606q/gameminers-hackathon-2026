# Pruebas de colaboración e integración

Registra el commit probado, la versión exacta de Unity, el sistema operativo, quién realizó la prueba y el resultado. Estado inicial de todas las pruebas: pendiente.

| Prueba | Resultado esperado |
| --- | --- |
| Acceso y ramas | Cada integrante puede hacer push a su rama y abrir un PR a `main` |
| Revisión e integración | Se identifica el PR y commit de cada aporte. Las referencias y `.meta` llegan al commit integrado |
| Abrir un clon sincronizado | Unity importa el proyecto sin errores de compilación, paquetes faltantes, scripts faltantes o referencias rotas |
| Flujo en Play Mode | El menú inicia una partida. El personaje se mueve y se ven los tres objetos y el modelo de Cami |
| Progreso y victoria | El HUD muestra `0/3 → 1/3 → 2/3 → 3/3`. Cada objeto cuenta una vez y la victoria aparece al recoger el tercero |
| Reinicio | El personaje vuelve a su punto de inicio, reaparecen los tres objetos y el HUD vuelve a `0/3` |
| Compilación externa | Windows o Web, según disponibilidad. La salida se genera fuera del editor en una carpeta ignorada, como `Builds/`, y se repite la partida completa desde el ejecutable o el navegador |

Evidencia del build: plataforma, commit, ruta de salida y resultado de ejecución. En Web, prueba mediante el servidor local que sirva la compilación. Matías aprueba el paso al juego definitivo después de verificar la partida en el editor y fuera de él.
