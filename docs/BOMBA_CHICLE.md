# Bomba de chicle

Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

## Unity

Instancia `Assets/Art/Props/BombaChicle/BombaChicle.prefab`. El frente es +Z y el pivote está centrado en el cuerpo, que mide unos 21 cm de ancho. En reposo, el Animator reproduce `Idle` en bucle. El trigger `Explode` inicia una animación de **1,53 s** que termina con la malla retraída.

| Tiempo desde el inicio de `Explode` | Acción visual |
|---|---|
| 0,00 a 0,30 s | La bola se comprime y luego se hincha. Las orejas se abren. |
| 0,33 s | Estallido de chicle. Desaparece la bola y salen nubes rosadas, burbujas, destellos y un aro expansivo. |
| 0,67 s | La onda ya ocupa unos 1,25 m de ancho visual. |
| 1,53 s | El efecto se retrae y queda listo para retirar la instancia. |

El Animator está en el hijo visual y la raíz queda fija. El controlador del juego decide la trayectoria, la colisión, el momento de disparar el trigger y la limpieza. Usa **0,33 s** como referencia para sincronizar el efecto y el daño. El ancho de la onda es decorativo. Para reutilizar una instancia, reinicia el Animator a `Idle` antes de lanzarla otra vez.

## Verificación

Unity **6000.4.11f1** comprobó la importación del rig Generic y los ocho materiales URP, el bucle de reposo, la transición por trigger, la expansión, la raíz fija y el cierre sin volver al reposo. Queda probar el lanzamiento y la aplicación de daño dentro de la escena de combate.
