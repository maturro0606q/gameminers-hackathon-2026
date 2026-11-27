# Jety

Jefe de la Sala del Jety. Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

El Jety mide **3,52 m con corona** frente a los **1,80 m** de la protagonista, medidos en Unity a escala 1. La corona sigue el hueso de la cabeza.

## Prefabs

| Prefab | Contenido |
| --- | --- |
| `Assets/Art/Characters/Jety/Jety.prefab` | Rig Generic de 31 huesos, materiales URP, Animator y trigger `PushHitbox` |
| `Assets/Art/Characters/Jety/Puas/PuasJety.prefab` | Nueve púas que emergen en secuencia, Animator y trigger `SpikeHitbox` |

Los dos prefabs se montan con **+Z** al frente. El Animator vive en el hijo visual. Se conserva la raíz fija y el controlador de gameplay desplaza y orienta al Jety.

## Animaciones y contacto

| Clip | Duración | Marcador de contacto o uso |
| --- | ---: | --- |
| `Idle` | 2,13 s | Bucle de reposo |
| `Walk` | 1,33 s | Bucle para aproximación |
| `PushAttack` | 1,50 s | Empujón con dos manos. Contacto a **0,60 s** |
| `GroundSlam` | 2,10 s | Anticipación visible con los puños arriba. Contacto con el suelo a **1,03 s** |
| `Hit` | 0,83 s | Reacción al daño |
| `Defeat` | 2,07 s | Caída con pose final sostenida |
| `Eruption` | 2,23 s | Púas escalonadas. Ventana de peligro inicial sugerida de **0,47 a 1,70 s** desde la aparición del prefab |

El Animator del Jety expone `Moving` (bool), `Dead` (bool) y los triggers `Push`, `Slam` y `Hit`. `Dead` lleva al estado de derrota e impide transiciones a ataques. El Animator de las púas arranca en `Eruption` al instanciar el prefab.

## Montaje en el combate

1. Para el empujón, dispara `Push` y habilita `PushHitbox` cerca del contacto. Aplica el daño y un desplazamiento de empuje en dirección al frente del Jety al confirmar el impacto. Toma **0,55 a 0,73 s** como ventana inicial de ajuste.
2. Para el golpe al suelo, dispara `Slam`. Al contacto, instancia `PuasJety` en el punto del hueso `GroundImpact`, proyectado al piso, con la orientación del Jety. La animación del efecto sucede en relación a su propio origen.
3. Habilita `SpikeHitbox` durante la ventana peligrosa y desactívalo antes de que se retraigan las púas. Registra cada objetivo una vez por activación. Los dos triggers se entregan apagados. El sistema de combate determina colisiones válidas, daño, intensidad del empuje, duración y cancelaciones.
4. Prueba con la cámara semilateral y la escala real. Verifica que la protagonista pueda esquivar al ver levantarse los puños, que reciba daño solo dentro de la ventana activa y que ambos triggers queden desactivados si se cancela el ataque.

## Verificación

Se reimportaron los dos FBX en Blender. En Unity **6000.4.11f1**, sobre la base Universal 3D del PR #2, se verificaron los siete clips, materiales URP, escala relativa, corona, raíz fija, transiciones del Animator, contacto de las manos con el trigger frontal y el suelo, y salida y retirada de las nueve púas. Queda conectar y probar el combate contra la protagonista en la escena del juego.
