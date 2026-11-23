# Cuchara de la protagonista

Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

Ambos prefabs se encuentran en `Assets/Art/Props/Cuchara/` y comparten el FBX y los materiales de acero URP.

| Prefab | Largo a escala 1 | Uso |
| --- | ---: | --- |
| `CucharaHeladeria.prefab` | 0,18 m | Servir helado con `ServeIceCream` |
| `CucharaCombate.prefab` | 0,90 m | Mundo de Hielo, golpe con `LadleSwing` |

La protagonista mide aproximadamente 1,80 m en Unity. La cuchara de combate mide el **50 %** de su altura. Los dos prefabs se instancian con escala local **1**. La escala del mesh ya está resuelta dentro de cada prefab.

## Montaje en la mano

El origen está en el mango, a un quinto del largo desde su extremo inferior. En cada prefab, la copa apunta a **+Z** local y abre hacia **+Y**. El hijo `Punta` sigue el extremo de la copa.

Haz que la cuchara sea hija de `Grip.R` de la protagonista, con posición local cero. Para orientar ambos tamaños, apunta +Z desde `Grip.R` hacia `GripTip.R`. En el rig actual, la siguiente orientación local se validó con `ServeIceCream` y `LadleSwing`:

```csharp
Vector3 forward = grip.InverseTransformDirection(gripTip.position - grip.position);
spoon.SetParent(grip, false);
spoon.SetLocalPositionAndRotation(Vector3.zero, Quaternion.LookRotation(forward, Vector3.forward));
```

Equipa la variante que corresponda al cambiar de mundo. Para sincronizar el combate, el contacto visual de `LadleSwing` está cerca de **0,50 s**. El controlador de la jugadora define el cambio de variante, la ventana de impacto y el daño.

## Verificación

En Unity **6000.4.11f1** se comprobaron los largos de 0,18 y 0,90 m, la relación con la altura de la protagonista, los cuatro materiales URP y el seguimiento de `Grip.R` en las dos animaciones. Queda la prueba del cambio de variante y del golpe dentro del juego.
