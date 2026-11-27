# Protagonista de La heladería maldita

Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código. La autorización de publicar la protagonista no concede a terceros una licencia general para reutilizar el arte.

## Montaje en Unity

1. Usa `Assets/Art/Characters/Protagonista/Protagonista.fbx` como hijo visual del prefab de la jugadora. Conserva los materiales URP y sus `.meta`. El FBX trae una malla con pesos, 28 huesos y avatar **Generic**.
2. Asigna el Animator Controller desde el prefab de la jugadora y desactiva **Apply Root Motion**. Todos los clips mantienen la raíz fija. El modelo mira a **−Z** en Unity. Si el controlador usa +Z como frente, gira el hijo visual 180° en Y.
3. Monta el cucharón, la lanza y el chicle en `Grip.R`. Usa `Grip.L` para el cono al servir. `GripTip.R` y `GripTip.L` indican hacia dónde apunta cada agarre. Para los lanzamientos, el gameplay puede tomar posición y dirección del agarre en el instante de soltado y controlar el proyectil desde ese momento.
4. En la escena final, coloca la raíz de la jugadora de espaldas al asiento, orientada hacia la cámara del desenlace. Como punto de partida, calibra un asiento a unos **0,27 m** del suelo y **0,10 m** detrás del origen de los pies a escala 1. Reproduce `ThroneSit` → `ThroneIdle`. Ajusta el punto de inicio al trono definitivo mirando apoyo de pies, asiento y brazos.

| Clip | Duración aprox. | Uso |
| --- | ---: | --- |
| `Idle` | 2,00 s | Bucle de reposo |
| `Walk` | 1,07 s | Bucle de caminar |
| `Run` | 0,80 s | Bucle de correr |
| `LadleSwing` | 1,20 s | Anticipación, golpe de arco y recuperación. Contacto hacia 0,50 s |
| `CandySpearThrust` | 1,00 s | Estocada. Contacto hacia 0,40 s |
| `CandySpearThrow` | 1,23 s | Lanzamiento por encima del hombro. Suelta hacia 0,50 s |
| `BubblegumThrow` | 1,37 s | Lanzamiento de chicle. Suelta hacia 0,57 s |
| `ServeIceCream` | 2,17 s | Recoge la porción, la pone en el cono y la ofrece. Entrega hacia 1,67 s |
| `ThroneSit` | 1,83 s | Baja hasta el asiento. Contacto hacia 1,30 s |
| `ThroneIdle` | 2,40 s | Bucle sentado que arranca en la pose final de `ThroneSit` |
| `Hit` | 0,73 s | Reacción al daño |
| `Defeat` | 1,63 s | Caída y pose final de derrota |

Los tiempos indican referencias visuales. El Animator Controller, los eventos de gameplay, el trono y los modelos de armas se conectan en los prefabs y escenas del equipo. Para la heladería en POV, prueba el encuadre de las manos con el mostrador y el cono definitivos.

## Validación

Reimportado en Blender con los 12 clips. En Unity **6000.4.11f1**, sobre la base de Universal 3D del PR #2, pasaron la importación, los cuatro bucles, la deformación de la malla, la raíz fija, los anclajes de ambas manos, el remapeo de 15 materiales URP y la continuidad entre los dos clips del trono. Queda la prueba con las escenas y el controlador del juego.
