# Diagnóstico: estado real de Level1 (Ciudad Limpia)

No generes ni modifiques nada todavía — primero necesito un reporte exacto del estado actual, porque en Play se ve un cuadrado morado (placeholder de `Enemy`) que no debería seguir ahí si ya se asignaron los sprites.

## Verificar y reportar

- [ ] En la escena `Level1` (tema Ciudad Limpia), en el GameObject `LevelSystems_*` con el componente `LevelBootstrap`: reportar qué `Sprite` está asignado actualmente en cada uno de estos campos (o si está en `None`):
  - `coinSprite`
  - `obstacleSprite`
  - `enemySprite`
  - `goalBeforeSprite`
  - `goalAfterSprite`
- [ ] Si `enemySprite` muestra asignado `nivel1_enemySprite.png` pero en Play sigue viéndose el cuadrado morado: revisar si el `SpriteRenderer` del enemigo realmente está leyendo ese campo al instanciarse en `SpawnEnemies()`, o si quedó un bug donde sigue usando `PlaceholderSprite.Square()` sin condicional. Reportar la causa exacta antes de arreglarlo.
- [ ] Reportar el valor actual de `LevelMapBuilder.endX` y el contenido del array `platforms` en `Level1`, comparado con los valores que se usaron en `Level4`/`Level5` (según el reporte previo: Level4 endX=100, Level5 endX=124). Confirmar si `Level1` quedó con un valor bajo (nivel corto) o si ya se había extendido.
- [ ] Confirmar si `Level1` tiene implementado el sistema de "3 actos por rango de X" que se pidió en el primer prompt, o si ese nivel se quedó con la estructura original sin actos.
- [ ] Confirmar si el checkpoint a mitad de nivel (`Checkpoint.cs`) está presente en `Level1`.

## No hacer todavía

- [ ] No reasignar sprites.
- [ ] No modificar `LevelMapBuilder` ni extender el nivel.
- [ ] No tocar tileset de suelo ni sprite del jugador (fuera de alcance, tareas separadas).

## Entregable esperado

Una tabla o lista con el estado real de cada punto de arriba, para decidir juntos el siguiente paso según lo que esté realmente mal (¿bug de asignación? ¿nivel no actualizado? ¿ambos?).
