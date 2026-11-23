# Lanza de caramelo

Diseño original de **Cami**, publicado con su autorización y crédito. Arte con derechos reservados. La licencia MIT del repositorio cubre el código.

El recurso está en `Assets/Art/Props/LanzaCaramelo/`. Incluye el FBX, el prefab visual y los dos materiales URP Lit. El prefab mide **1,65 m** a escala 1. Su origen está en el agarre, a **0,65 m** del extremo inferior. El mango apunta hacia **+Z** local, la cara de la punta hacia **+Y**, y el hijo `Punta` marca el extremo de la pieza.

## Montaje en la protagonista

Instancia `LanzaCaramelo.prefab` como hijo de `Grip.R` del rig de la protagonista (PR #6). Usa posición local cero, escala local 1 y orienta +Z de `Grip.R` hacia `GripTip.R` como en el resto de sus accesorios.

```csharp
Vector3 forward = grip.InverseTransformDirection(gripTip.position - grip.position);
spear.SetParent(grip, false);
spear.SetLocalPositionAndRotation(Vector3.zero,
    Quaternion.LookRotation(forward, Vector3.forward));
```

En `CandySpearThrow`, la referencia visual de suelta está cerca de **0,50 s**. El gameplay define el instante de suelta, la dirección, las colisiones y la recuperación de la lanza. El prefab incluye el modelo y su anclaje de punta.

## Validación

Probado en Unity **6000.4.11f1** con la base Universal 3D del PR #2. Se verificaron el largo importado, los remapeos de los dos materiales, el marcador `Punta` y el alineamiento en la mano derecha al muestrear `CandySpearThrust` (0,40 s) y `CandySpearThrow` (0,50 s). Queda la validación del lanzamiento y la recogida en la escena de juego, cuando el gameplay conecte el prefab.
