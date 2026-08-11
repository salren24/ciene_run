# Continuación Nivel 1 — parallax seguro + terreno con altura variable

## 1. Fondo parallax (implementar vía código, no editando el YAML de la escena a mano)

Coincido en que editar el YAML a mano para agregar GameObjects nuevos es riesgoso sin poder verificar en el Editor. Usa el mismo patrón que ya usa `LevelBootstrap` para monedas/enemigos/obstáculos: **instanciar en runtime desde código** (`Awake()`), no como GameObjects estáticos guardados en la escena.

- [ ] Crear `ParallaxLayer.cs`: componente simple que, dado un `float parallaxFactor` (0 = fijo, 1 = se mueve igual que la cámara), desplaza su `transform` en proporción al movimiento de la cámara respecto a su posición inicial.
- [ ] En `LevelBootstrap` (o un nuevo método `SpawnParallaxBackground()` llamado desde `Awake()`), instanciar 2-3 capas usando las nubes existentes (`nube.png` a `nube4.png`):
  - Capa lejana: 1-2 nubes grandes, `parallaxFactor` bajo (ej. 0.1-0.2).
  - Capa cercana: 2-3 nubes más pequeñas, `parallaxFactor` medio (ej. 0.3-0.4).
- [ ] Todo generado por código en runtime, igual que las monedas — así no se toca el archivo de escena directamente y es reversible con solo cambiar el script.
- [ ] Exponer como campos públicos serializados (`Sprite[] cloudSprites`, `int cloudCount`, etc.) para poder ajustar cantidad/posición desde el Inspector sin tocar código de nuevo.

## 2. Terreno con altura variable (para el escalón descendente del Acto III)

El sistema actual de `LevelMapBuilder` solo soporta una franja base a altura fija + plataformas flotantes opcionales. Extenderlo así:

- [ ] Agregar un nuevo array serializado, ej. `public TerrainSegment[] terrainProfile`, donde cada `TerrainSegment` tiene `startX`, `endX`, `height` (altura relativa a `groundY`).
- [ ] Si `terrainProfile` está vacío: mantener el comportamiento actual exacto (franja plana a `groundY`) — no romper los niveles que no usan esto todavía.
- [ ] Si `terrainProfile` tiene datos: para cada columna x dentro del rango del nivel, usar el segmento que la contenga para determinar la altura del suelo en esa columna, en vez de `groundY` fijo.
- [ ] Con esto, implementar el escalón descendente de dos niveles en el Acto III de Nivel 1 (el que se había simplificado a tramo plano) usando 2-3 `TerrainSegment` con alturas decrecientes hacia la meta.
- [ ] Verificar que las plataformas flotantes existentes (`platforms`) sigan funcionando igual, ya que son independientes del terreno base.

## Entregable

1. Parallax funcionando en Nivel 1, generado por código (no en el YAML).
2. Escalón descendente real en el Acto III usando el nuevo `terrainProfile`.
3. Confirmación de que Level2-Level5 no se rompieron (deben seguir usando franja plana ya que no tendrán `terrainProfile` configurado todavía).
