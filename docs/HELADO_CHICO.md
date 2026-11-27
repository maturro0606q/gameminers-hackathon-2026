# Helado chico

Enemigo visual de la primera oleada de Mundo Helados. Diseño original de **Cami**. FBX publicado con su autorización y crédito. Arte con derechos reservados. El permiso de publicación de esta pieza no concede una licencia general de reutilización a terceros. La licencia MIT del código se aplica al código del proyecto.

## Recurso para Unity

- Modelo: `Assets/Art/Characters/HeladoChico/HeladoChico.fbx`. Arrástralo como hijo visual del prefab del helado chico.
- Malla con pesos, 15 huesos, 11 materiales URP y cinco clips esqueléticos a 30 fps. La altura del modelo base es aproximadamente 1,66 m. Ajusta el hijo visual según la escala que tengan el jugador, el enemigo y su colisionador.
- El frente del modelo apunta hacia **−Z** en Unity. Si el controlador usa `transform.forward` (+Z), gira el hijo visual **180° en Y**.
- Usa el avatar **Generic** y asigna un Animator Controller del prefab de gameplay. Desactiva **Apply Root Motion**. El movimiento, las colisiones, el daño, el conteo de bajas y la eliminación del enemigo quedan a cargo de gameplay.

| Clip | Duración aproximada | Reproducción | Uso |
| --- | ---: | --- | --- |
| `Idle` | 2,00 s | Bucle | Respiración y parpadeo |
| `Walk` | 1,00 s | Bucle | Pasos alternados con balanceo |
| `Attack` | 1,27 s | Una vez | Anticipación, embestida corta y recuperación |
| `Hit` | 0,67 s | Una vez | Reacción al recibir daño |
| `Death` | 1,70 s | Una vez | El cuerpo se derrite hasta quedar en el suelo |

El golpe visual de `Attack` alcanza su punto de contacto alrededor de **0,63 s** (tiempo normalizado **0,50**). Úsalo para sincronizar la ventana del daño cuando definas el comportamiento. El clip no incluye eventos ni parámetros del Animator.

## Validación

- Blender 5.2.2. Reimportación del FBX con una malla de 9.846 vértices, 15 huesos, pesos, 11 materiales y las cinco pistas. Raíz fija y poses de inicio y fin coincidentes en los dos bucles.
- Unity **6000.4.11f1**, sobre una copia aislada de la base Universal 3D del PR #2. Importación con avatar Generic, 11 materiales URP remapeados y los cinco clips. Al muestrearlos, se verificó la deformación de la malla, los bucles, la raíz fija, el parpadeo y la pose final de `Death`.
- Al montarlo en gameplay, verifica la orientación y el tamaño en la escena de Mundo Helados, el contacto del ataque, el conteo de bajas y el rendimiento con la oleada de 23 helados chicos.
