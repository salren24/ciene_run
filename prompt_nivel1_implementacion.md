# Implementar diseño de Nivel 1 — Ciudad Limpia

Ya definimos el layout de este nivel. Impleméntalo con estos ajustes incluidos (no me lo vuelvas a proponer, ya está aprobado — implementa directo):

## Diagnóstico base (contexto, ya confirmado)

- El suelo actual es una franja plana sin huecos (x=-8 a x=45), sin variación de altura.
- El tileset (`groundTop`/`groundFill`) ya es arte real, pero genérico y compartido entre los 5 niveles — para este nivel necesita ser temático.
- Las monedas están en zigzag simple, no en patrones (arcos/diagonales).
- Falta frame de Jump en `PlayerMale` y `PlayerFemale` (5 y 4 frames respectivamente, sin salto dedicado).
- No hay parallax montado; existen sprites de nubes (`nube.png` a `nube4.png`) sin usar.
- `actIEndX=14` / `actIIEndX=28` son solo gizmos organizativos, no afectan el layout real todavía.

## Layout a implementar

**`goalX = 110`** (antes 42 — extender para que el nivel no se sienta corto, consistente con Level4=100 y Level5=124). Ajusta la distribución de los 3 actos abajo proporcionalmente a esta nueva longitud, no los dejes comprimidos en el rango original de 42.

**Acto I (introducción)**
- Suelo llano corto al inicio para que el jugador se ubique.
- Hueco de salto simple obligatorio, con fila de monedas en arco guiando el salto.
- Escalón subiendo (+1 altura), se mantiene hasta el checkpoint.

**Acto II (variación)**
- Plataforma flotante corta a +1.5 de altura, con línea diagonal de monedas llevando hacia ella.
- Segundo hueco más ancho justo después del checkpoint (reemplaza uno de los obstáculos estáticos actuales por el hueco mismo).
- Enemigo (smog) patrullando sobre plataforma.

**Acto III (cierre hacia la meta)**
- Escalón descendente en dos niveles hacia la meta.
- Arco final de monedas frente a la meta.

## Requisito obligatorio antes de implementar los huecos

Antes de colocar cualquier hueco, verifica los valores reales de velocidad y fuerza de salto en `PlayerController` (o el script equivalente) y confirma que cada hueco propuesto es saltable con esos valores. Ajusta el ancho de los huecos si no lo son — no coloques huecos "a ojo".

## Assets a generar (con tu aprobación de diseño antes de generarlos, como ya veníamos haciendo)

1. Tileset temático "Ciudad Limpia" (`groundTop`/`groundFill`) — paleta urbana gris/verde, contorno definido, reemplaza el tileset genérico solo en este nivel.
2. Capas de parallax: cielo con gradiente + silueta de edificios lejanos. Reutiliza `nube.png`–`nube4.png` como capa frontal, no hace falta generarlas de nuevo.
3. Frame de Jump para `PlayerMale` y `PlayerFemale` — hazlo ahora, ya que afecta a los 5 niveles, no solo a este.

## Entregable

Implementa el layout completo con estos ajustes. Muéstrame el resultado final (no una nueva propuesta) para revisarlo en Play antes de pasar a Nivel 2.
