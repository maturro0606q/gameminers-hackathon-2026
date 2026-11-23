# Conejo de público

Conejo rosado que hace barra durante la batalla contra los helados. Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

## Unity

Añade `Assets/Art/Characters/ConejoPublico/ConejoPublico.prefab` en los bordes de la arena y oriéntalo hacia la pelea. Su frente es +Z y el origen está a nivel del suelo. En Play Mode, el prefab hace barra automáticamente.

| Clip | Duración | Uso |
|---|---:|---|
| Cheer | 2,37 s | Bucle con los dos brazos levantados en pulsos alternados, boca abierta y movimiento de orejas |
| Idle | 2,37 s | Reposo en bucle, con parpadeo |

El Animator está en el hijo visual y expone el bool `Cheering`, activado por defecto. Desactívalo para pasar a `Idle`. Si el montaje requiere varios conejos desfasados, inicia `Cheer` en un tiempo normalizado distinto por cada instancia.

## Verificación

Unity 6000.4.11f1 validó el avatar Generic, cinco materiales URP, orientación del prefab y las transiciones Cheer → Idle → Cheer. También se comprobó la raíz fija, la continuidad del bucle y que ambas patas suban más de 0,30 m desde su posición inicial. Queda colocar el público en la escena definitiva.
