# Helado mediano

Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

## Unity

Usa `Assets/Art/Characters/HeladoMediano/HeladoMediano.prefab`. A escala 1 mide aproximadamente 2,45 m y apunta hacia +Z. El hijo visual contiene el Animator con avatar Generic y Apply Root Motion apagado. Asígnale el Animator Controller del enemigo.

| Clip | Duración | Uso |
|---|---:|---|
| Walk | 1,20 s | Bucle. La base derretida alterna sus apoyos y la bola de menta se balancea. |
| Bite | 1,63 s | Abre ambas bocas, se lanza y cierra la mandíbula rosa a 0,80 s. La de menta cierra hacia 0,87 s. |

La raíz queda fija. El marcador animado `BitePoint` sigue el frente de la boca rosa y se adelanta unos 0,56 m durante el mordisco. Confirma el impacto alrededor de **0,80 s** (tiempo normalizado aproximado **0,49**). La segunda mandíbula remata el mismo ataque. La lógica de gameplay detecta a la protagonista y aplica el daño.

## Verificación

Blender reimportó el FBX con ambos clips, pesos y 13 huesos. En Unity 6000.4.11f1 se validaron el avatar Generic, los 14 materiales URP, la orientación del prefab, la raíz fija, el bucle de caminata y el cierre escalonado de las mandíbulas. Queda probar el impacto contra la protagonista en la escena del juego.
