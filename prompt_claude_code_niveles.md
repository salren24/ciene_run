# Tarea para Claude Code: Sistema de niveles temáticos (juego de campaña política)

## Contexto

Proyecto Unity (URP 2D), juego de plataformas estilo Mario/pixel art para la campaña de un candidato político. Cada nivel = una propuesta de campaña, con mecánica "antes → después": el jugador recolecta ítems temáticos, esquiva obstáculos que representan el problema, y al llegar a la meta se revela la solución implementada visualmente.

## Archivos existentes (extender, no reescribir desde cero)

- `LevelBootstrap.cs` — instancia monedas/enemigos/obstáculos/meta/zona de muerte en `Awake()`. Usa `PlaceholderSprite.Square()` con tint de color como sprite temporal.
- `LevelMapBuilder.cs` — construye suelo/plataformas vía `Tilemap`. Parámetros: `startX`, `endX`, `groundY`, `groundDepth`, array `platforms` (x, y, ancho).
- `CoinPickup.cs` — en cada moneda, llama `GameManager.Instance.AddCoin()` al colisionar con tag `Player`.
- `PlaceholderSprite.cs` — genera sprite blanco 1x1 de fallback.
- Otros: `GameManager`, `MarioHUD`, `TouchControls`, `Enemy`, `Obstacle`, `GoalFlag`, `DeathZone`.

Ya existen **3 niveles/escenas creados** — revisarlos primero e identificar a qué tema de la tabla corresponden antes de modificar. Luego crear **2 niveles nuevos** con el mismo patrón.

## Tabla de temas por nivel

| # | Tema | Coleccionable | Obstáculo | Transformación final |
|---|------|---------------|-----------|----------------------|
| 1 | Ciudad Limpia | Bolsas de basura / monedas reciclaje | Tachos desbordados, charcos | Calle sucia → limpia con árboles/contenedores |
| 2 | Seguridad Ciudadana | Motos de serenazgo, cámaras | Conos "zona insegura" | Terreno baldío → central de seguridad iluminada |
| 3 | Educación | Libros, mochilas | Pupitres rotos, goteras | Aula deteriorada → colegio remodelado |
| 4 | Salud | Cruces médicas, vacunas/pastillas | Camillas vacías, filas de gente | Posta antigua → posta ampliada + ambulancia |
| 5 | Empleo/Economía | Monedas + certificados de trabajo | Carteles "cerrado", cajas rotas | Calle comercial cerrada → mercado reactivado |

## Checklist de implementación

- [ ] **Sprites parametrizables**: en `LevelBootstrap`, agregar campos públicos serializados `Sprite` (`coinSprite`, `obstacleSprite`, `enemySprite`, `goalBeforeSprite`, `goalAfterSprite`). Si el campo está vacío → usar `PlaceholderSprite.Square()` como fallback (no romper niveles sin configurar).
- [ ] **Transformación final al llegar a la meta**: en `GoalFlag`, permitir cambiar el sprite de un `SpriteRenderer` de fondo de "estado problema" a "estado solución" al activarse. Transición simple (instantánea o fade básico).
- [ ] **Nivel dividido en 3 actos por rango de X**: extender `LevelMapBuilder` o `LevelBootstrap` para soportar tramos configurables (ej. Acto 1: x 0-40 más obstáculos, Acto 2: x 40-80 transición, Acto 3: x 80-120 resultado). Parámetros expuestos en Inspector, sin necesidad de sistema complejo.
- [ ] **Recompensa por completitud**: umbral configurable (cantidad o % de ítems recolectados) que determine si la meta muestra transformación completa o parcial.
- [ ] **Checkpoint a mitad de nivel**: nuevo script `Checkpoint.cs`, trigger que actualiza el punto de respawn del jugador (inverso a `DeathZone`).
- [ ] **No romper compatibilidad**: `CoinPickup`, `Enemy`, `Obstacle`, `GoalFlag`, `DeathZone`, `GameManager`, `MarioHUD` deben seguir funcionando igual si algún sprite no está asignado.

## Fuera de alcance (no hacer en esta tarea)

- [ ] No generar ni descargar sprites (se maneja en una tarea aparte).
- [ ] No tocar configuración de cámara / Pixel Perfect.
- [ ] No modificar `TouchControls` ni el sistema de input.

## Entregable esperado

1. Los 3 niveles existentes adaptados a la estructura temática (indicar a qué tema corresponde cada uno, o preguntar si no es evidente por el contenido actual).
2. 2 niveles nuevos siguiendo el mismo patrón, con placeholders donde falten sprites.
3. Lista final de qué campos `Sprite` quedaron sin asignar en el Inspector, para saber qué assets pixel art generar después.
