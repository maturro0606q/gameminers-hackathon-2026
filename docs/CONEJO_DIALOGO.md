# Conejo de diálogo

Conejo que recibe a la protagonista en el Mundo de Hielo. Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

## Usar en Unity

Arrastra `Assets/Art/Characters/ConejoDialogo/ConejoDialogo.prefab` a la escena. El prefab ya tiene asignado el Animator Controller. Su frente coincide con **+Z** y la raíz queda fija en todos los clips. En el rig, `DialogueAnchor` da un punto de referencia para el globo o marcador de diálogo.

| Parámetro del Animator | Tipo | Uso |
| --- | --- | --- |
| `Greet` | Trigger | Reproduce el gesto inicial |
| `Talking` | Bool | Actívalo mientras habla el conejo |
| `Listening` | Bool | Actívalo cuando el conejo escucha a la protagonista |
| `Emphasize` | Trigger | Con `Talking` activo, inserta un gesto de énfasis y vuelve a hablar |

En el encuentro de entrada, dispara `Greet` y presenta la frase acordada **«Oh, eres el heladero número 23 en venir aquí»**. Puedes usar `Emphasize` para acompañar «número 23». Para volver a reposo al cerrar la conversación, desactiva `Talking` y `Listening`. La boca tiene movimiento gestual de habla. El texto, el audio y el disparador de proximidad se conectan en la escena.

| Clip | Duración aprox. | Reproducción |
| --- | ---: | --- |
| `Idle` | 2,37 s | Bucle de reposo con parpadeo |
| `Greet` | 1,53 s | Saludo con una pata y apertura de la boca |
| `Talk` | 2,10 s | Bucle de diálogo con gestos de cabeza y orejas |
| `Listen` | 2,60 s | Bucle de escucha, con boca en reposo |
| `TalkEmphasis` | 1,87 s | Gesto breve de énfasis con el acento visual hacia 0,73 s |

## Verificación

Se reimportó el FBX en Blender y se validó el prefab en Unity **6000.4.11f1** sobre la base Universal 3D del PR #2. Las pruebas cubrieron los cinco clips, los tres bucles, 20 huesos, nueve materiales URP, la raíz fija, el parpadeo, la articulación de la boca y las transiciones del Animator entre reposo, saludo, conversación, escucha y énfasis. Queda montar y probar el encuentro con la protagonista en la escena del juego.
