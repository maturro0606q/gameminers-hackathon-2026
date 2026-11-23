# GameMiners — Protocolo de entrega final de Nacho a GitHub
> 23/11/2026 · Instrucciones para Work/Codex del responsable de gameplay. Este documento no autoriza fusionar `main` sin revisión del equipo.

## Objetivo
Una vez que Nacho estime terminado el juego de **La heladería maldita**, asegurar que el **proyecto Unity real, completo, reproducible, compilable y probado** quede respaldado en GitHub desde su rama de gameplay, con un Pull Request verificable hacia `main`.

Repositorio: https://github.com/maturro0606q/gameminers-hackathon-2026  
Versión Unity observada en base PR #2: `6000.4.11f1`, Universal 3D (confirmar `ProjectSettings/ProjectVersion.txt` antes de actuar).

## Condición de «terminado»
No basta que existan scripts/FBX o capturas. La **misma versión** debe completar:
- [ ] Menú Jugar abre tienda POV.
- [ ] Atención simplificada y mensaje **«¡Queda una pura bola de helado!»** al quedar 1; al agotarse se permite bodega.
- [ ] Refrigerador activa aparición Jety y cambia a vista lateral 3D al mundo helado.
- [ ] Conejo guía dice **«Oh, eres el heladero número 23 en venir aquí»**; fondo con conejos pancartas y completo italiano si el asset está disponible.
- [ ] Jugadora se mueve/ataca, enemigos reciben daño y se eliminan; las 23 y 10 bajas exactas se prueban si esa versión se entrega como definitiva.
- [ ] Jefe helado y jefe Jety pueden derrotarse.
- [ ] Final del trono y menú de regreso.
- [ ] Las acciones principales de UI están conectadas (sin duplicar UI de Matías).
- [ ] Créditos de Matías, Nacho, Cami y audio seleccionado; música acorde a `docs/AUDIO_LICENSES.md`.
- [ ] Consola de Unity sin errores nuevos; escena probada en Play Mode.
- [ ] Build externo probado (Windows u objetivo acordado) y evidencia de ejecución.
- [ ] No hay referencias rotas, scripts `Missing`, audio faltante o assets LFS sin resolver.

## Fase 1 — Auditar antes de escribir
1. Comprobar conexión efectiva a Unity MCP, permisos y GitHub; no simular resultados.
2. Confirmar **qué proyecto local está abierto**, raíz Git, remoto correcto, rama activa, estado de archivos y versión Unity.
3. Listar PRs, ramas y commits existentes: pueden existir simultáneamente PRs abiertos para base Unity (#2), UI (#3), arte (#5–#13), diseño (#1) y licencias (#4). **`main` podría seguir sin Unity**.
4. Identificar qué commits realmente están incorporados en la carpeta local. Nunca asumir que un PR abierto está en `main`.
5. Si faltan dependencias de otro integrante, **no copiar/forzar archivos ni fusionar PRs sin consentimiento**; preparar una rama de integración o pedir al dueño el merge requerido.

## Fase 2 — Validar el proyecto
1. Guardar las escenas y assets desde Unity MCP, refrescar base de datos y esperar compilación.
2. Leer consola y resolver errores de compilación/missing references que estén **en tu ámbito**.
3. Revisar escenas, Build Settings, prefabs, shaders, materiales, animaciones, colisiones, cámaras, UI e Input System.
4. Probar el recorrido de extremo a extremo en Play Mode, incluyendo derrota/reintento y final.
5. Generar una compilación de entrega y **abrir el ejecutable**, verificar inicio, niveles, audio y final; anotar resultado real. Si build falla, consignarlo y no declarar «listo para entrega».

## Fase 3 — Preparar commit limpio
1. Trabajar sobre `feature/gameplay` o una rama **`integration/final-game`** acordada. Si la rama de gameplay tiene cambios sin subir, preservarlos; nunca resetear, rebasear o forzar push de forma destructiva.
2. Ejecutar `git status` y revisar `git diff`. Quitar archivos temporales innecesarios del staging, sin borrar trabajo ajeno.
3. Incluir archivos necesarios para reproducir Unity: `Assets/`, `Packages/`, `ProjectSettings/`, escenas, scripts, prefabs, animadores, audios autorizados, shaders y **todos los `.meta` correctos**.
4. Excluir `Library/`, `Temp/`, `Logs/`, `UserSettings/`, `Obj/`, cache, builds pesados y secretos; no subir `.env`, tokens, credenciales ni arte fuente privado de Cami.
5. Verificar Git LFS (si aplica) y que el clon pueda descargar binarios reales, no solo punteros. Preservar GUID al mover/renombrar.
6. Confirmar `LICENSE` MIT original, `ART_LICENSE.md`, registros de créditos y licencias externas **en la rama integrada**. El arte de Cami se usa **solo en este videojuego**, sin relicenciarlo.
7. Actualizar `README.md` con instrucciones para instalar la misma versión Unity, abrir proyecto, controles, niveles y cómo construir/ejecutar; documentar `docs/TESTING.md` con fecha, commit probado, equipo, plataforma y resultado (real, no supuesto).

## Fase 4 — Publicar y revisar
1. Crear commits pequeños o un commit final coherente: `feat(gameplay): completar recorrido jugable de La heladería maldita`.
2. Hacer `push` a rama de Nacho (sin `--force`) y **verificar** que los commits y rutas aparecen en GitHub.
3. Abrir un Pull Request hacia `main` o actualizar el PR de la misma rama si existe.
4. El PR debe incluir:
   - Resumen funcional del juego.
   - Archivos modificados y dependencias de otros PRs.
   - Versión Unity / plataforma.
   - Pasos para probar y evidencia de Play Mode y build ejecutado.
   - Checklist de juego completo y limitaciones conocidas.
   - Información de licencias/creditos, arte Cami y audio.
5. Verificar diff con `main`: si incluye miles de archivos de otras ramas, explicar cuáles son dependencias y solicitar integración secuencial; **no borrar ni sobrescribir** UI o arte de otro miembro.
6. **NO fusionar automáticamente a `main`**. Matías revisa e integra, con pruebas posteriores.
7. Confirmar al usuario URL exacta del PR, rama, SHA de commit, estado de pruebas y advertencias; si algún paso falló, decirlo claramente.

## Política de autonomía
Autorizado: guardar, refactorizar y corregir **tu gameplay**, crear prefabs/escenas, automatizar cambios vía Unity MCP, compilar, ejecutar, revisar Git, subir commits de tu rama y actualizar/abrir PR.

Requiere coordinación: sustituir archivos UI/arte de Cami, resolver colisiones de escenas con otro desarrollador, cambiar versión Unity o paquetes incompatibles, reescribir historia/licencias, aprobar merges de PRs ajenos o publicar una build a una plataforma externa.

## Mensaje breve de cierre esperado
```
Repositorio:
Rama y SHA:
PR:
Versión Unity:
Escenas funcionales:
Build externo probado (sí/no y evidencia):
Arte de Cami incorporado:
Créditos/audio verificados:
Pendientes o bloqueos:
```
