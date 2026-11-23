# GameMiners — Política propuesta de licencias y publicación
> 23-11-2026 · BORRADOR. Requiere aprobación de Cami y del equipo. Este documento no concede ningún permiso sobre su arte.

## Derechos separados
| Contenido | Régimen propuesto | Lugar |
| --- | --- | --- |
| Código original C# y herramientas GameMiners | MIT (archivo LICENSE existente) | GitHub público |
| Arte original de Cami: dibujos, modelos, rigs, texturas y fuentes editables | Todos los derechos reservados a la autora | Almacenamiento privado, no GitHub público sin autorización |
| Música y efectos externos | Licencia individual por recurso: preferencia CC0, alternativa CC BY con créditos | Publicable solo al verificar redistribución |
| Recursos de terceros | La licencia propia del recurso; no se vuelven MIT | Según permiso específico |

MIT de la raíz no se extiende automáticamente a obras artísticas propietarias ni a obras musicales externas. El README deberá comunicarlo inequívocamente.

## Privacidad no es lo mismo que derechos reservados
Un FBX o un archivo de Blender que se suba a una rama de un repositorio público es descargable, aunque se indique «Todos los derechos reservados». Tampoco es seguro subirlo a un Pull Request público y después borrar el commit. Por defecto, prohibir upload de cualquier dibujo o modelo original de Cami al repo público hasta que ella lo autorice expresamente.

## Implementación práctica recomendada
1. El repositorio público mantiene código, documentación y placeholders o arte redistribuible CC0/propio expresamente autorizado.
2. Cami mantiene los originales editables y exportaciones privadas en una ubicación privada con acceso controlado para los integrantes autorizados. Nunca poner secretos ni tokens en el repo.
3. Antes de distribuir un build con modelos de Cami, obtener autorización escrita limitada para incorporarlos al juego compilado, exhibirlo y distribuirlo en el marco acordado de la hackatón. No implica ceder sus derechos ni liberar los originales.
4. Las escenas y prefabs públicos deben ser funcionales usando assets placeholder públicos. Si se usan sustituciones de arte privadas para compilar, deben quedar en una copia local no versionada.
5. El build con arte propietario no es 100% código y assets abiertos: declarar correctamente que la licencia es mixta. Los recursos incluidos pueden llegar a extraerse del ejecutable; no garantizar confidencialidad.
6. Validar bases del evento: aún falta confirmar si el requisito open source abarca *solo código* o también todos los assets y la capacidad de reconstruir íntegramente el proyecto.
7. Si Cami autoriza expresamente publicar FBX propietarios bajo derechos reservados, documentar esa excepción; serían públicos/descargables, aunque no open source. No elegir esta opción sin su aprobación.

## Audio
Preferir música CC0 con fuente y autor comprobados. CC0 no exige atribución legalmente, pero recomiendo créditos por buena práctica. Para CC BY 4.0 se deben incluir autor, título, URL, licencia e indicar modificaciones. «Royalty free» o una canción tomada de YouTube no garantizan derechos para publicar y distribuir audio.
Fuente preliminar de candidatas: MUSIC_SHORTLIST.md. No descargar ni subir música sin seleccionar cada pieza y verificar la licencia específica del archivo.

## Verificaciones previas a entrega
- [ ] Código MIT y exclusión del arte explicada en README.
- [ ] Consentimiento de Cami específico para uso y distribución de su arte en el build.
- [ ] Ningún original privado aparece en GitHub público, incluidas ramas.
- [ ] El repo abierto puede probarse con placeholders.
- [ ] Bases del evento permiten distribución mixta, o se adopta alternativa compatible.
- [ ] Audio seleccionado con procedencia y créditos.
- [ ] Se prueba el build final fuera de Unity.

Fuentes: https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/licensing-a-repository , https://creativecommons.org/faq/ , https://creativecommons.org/publicdomain/zero/1.0/
