# GameMiners — Integración musical en Unity (M1)
> Plan de trabajo para Nacho (escenas) y Matías (créditos/UI). **No es prueba de compilación**.

## Principio: mantener el juego jugable
En M1 integrar las tres músicas por escena (`01_Shop`, `02_IceWorld`, `03_JetyRoom`). **La música del final del trono ya está elegida: Spooky Waltz**, pero puede añadirse justo después de validar el recorrido completo, sin bloquear M1. El audio opcional del refrigerador puede esperar.

## Procedimiento de importación y sincronización
1. Descargar desde los enlaces de `docs/AUDIO_LICENSES.md`:
   - Tienda: ZIP `ice_cream_truck_theme.zip`, extraer **la versión OGG** (comprobar nombre real).
   - Mundo de helados: **Magic Puzzle In-Game 1**, MintoDog, CC0. Descargar el archivo OGG **`magic_puzzle_in-game_1_bpm110.ogg`** del enlace registrado. Reutilizar la misma música en las oleadas de helados pequeños y grandes (sin reiniciar la pista entre fases si siguen en la misma escena).
   - Jety: **Boss Battle Music** de SubspaceAudio / Juhani Junkala, archivo original `Juhani Junkala - Epic Boss Battle [Seamlessly Looping].wav`, según ficha de OpenGameArt. Unity admite WAV; se puede convertir a OGG para reducir tamaño si se conserva atribución y trazabilidad.
   - **Trono / final confirmado:** **Spooky Waltz** de Aureolus_Omicron (CC0), pista `musicbox1_spooky_waltz` de `4_music_box_tracks_ogg.zip` en https://opengameart.org/content/4-music-box-tracks. Extraer únicamente la versión OGG de esta pista y verificar su nombre real antes de renombrarla o importarla.
2. Verificar autor, título, licencia CC0 y formato de cada archivo. No usar reinterpretaciones no autorizadas, reuploads o material protegido por licencia diferente.
3. Copiar los archivos aprobados (`.ogg` o `.wav`, según corresponda) a `Assets/Audio/Music/` **dentro del clon del repositorio Unity**. Unity creará `.meta`; incluirlos con los archivos al hacer commit.
4. **No hacer cambios en la escena principal desde la rama audio.** Preparar sonidos, créditos y documentación en una rama pequeña como `feature/audio-m1`. Crear PR y solicitar a Matías/Nacho coordinación de merge.
5. Tras integrar la carpeta de música, **Nacho**, dueño de escenas, las abre en su rama y añade el componente `AudioSource` a un GameObject `Music` en cada escena, con:
   - `AudioClip` asignado según escena.
   - `Spatial Blend = 0` (2D), `Loop = true`, `Play On Awake = true`, volumen de partida moderado (ej. 0.2–0.35 ajustable).
   - Mantener `AudioListener` activo único y evitar fuentes duplicadas.
6. En `00_Menu` se puede elegir silencio inicialmente o reutilizar música de tienda; evitar dos AudioSources que reproduzcan simultáneamente.
7. En `01_Shop`, la música debe detenerse al cambiar de escena (GameObject no persistente), con interrupción o fade breve si el sistema existente lo facilita. No introducir un `DontDestroyOnLoad` global en M1 sin control de duplicados.
8. En `03_JetyRoom`, al morir el Jety, Nacho **detiene/funde Boss Battle Music**, deja un breve silencio (1–2 segundos opcionales), y **comienza Spooky Waltz** a volumen bajo cuando inicia la secuencia de sentarse en el trono. Puede usar un segundo AudioSource en la misma escena, inicialmente deshabilitado, o un mismo AudioSource con cambio de AudioClip y Play: evitar reproducciones simultáneas. Conservar la música del trono hasta los créditos/panel final y cortarla al volver al menú. Si el recorrido M1 aún no está validado, dejar la lógica preparada y hacer este cambio en el siguiente hito.
9. Matías añade en UI de Créditos los títulos/autores y en README enlace a `docs/AUDIO_LICENSES.md`, sin modificar los controladores.
10. Hacer PR de los cambios de escena por separado, probar en Play Mode, luego generar y ejecutar un build externo.

## Lista de aceptación
- [ ] Música tienda suena en POV sin variar por distancia.
- [ ] Al ir al IceWorld suena **Magic Puzzle In-Game 1** para pequeños y grandes sin cambiar ni duplicar audio entre oleadas.
- [ ] Al entrar a JetyRoom cambia a **Boss Battle Music (SubspaceAudio / Juhani Junkala)**.
- [ ] Al morir/pausar/reiniciar no se acumulan instancias duplicadas.
- [ ] Al derrotar al Jety se corta/funde **Boss Battle Music** y empieza **Spooky Waltz** (CC0, Aureolus_Omicron) en el trono, sin duplicación de pistas.
- [ ] Los cuatro audios seleccionados (tienda, mundo helados, Jety, trono) aparecen en Git junto con sus .meta cuando se hayan incorporado; en M1 bastan las tres músicas de escena.
- [ ] Todos los autores están acreditados y la selección figura en `docs/AUDIO_LICENSES.md`.
- [ ] El build fuera de Unity reproduce música correctamente.

**No modificar `feature/gameplay` ajeno ni resolver conflictos sobre escenas sin consultar a Nacho.**
