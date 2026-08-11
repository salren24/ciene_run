# Especificación de assets pendientes — Nivel 1 (y transversal)

Estos dos assets NO son tarea de Claude Code (es código, no genera arte) — van a tu flujo externo habitual, igual que los 16 sprites anteriores.

## 1. Tileset temático "Ciudad Limpia"

- **Qué es**: reemplazo de `groundTop` y `groundFill` (actualmente genéricos, compartidos por los 5 niveles) por una variante temática solo para este nivel.
- **Estilo**: pixel art, mismo nivel de detalle que el resto de sprites ya generados (obstáculos, fondos de meta).
- **Paleta**: urbana, gris/verde — pensar en vereda/asfalto con toques de vegetación urbana (no tierra/pasto genérico tipo campo).
- **Piezas necesarias**:
  - `groundTop`: tile superior con contorno definido (el que se ve pisable, con "borde" visual claro).
  - `groundFill`: tile de relleno/profundidad (lo que se ve debajo del top, repetible verticalmente).
- **Tamaño**: debe ser un tile cuadrado que calce con el grid del Tilemap del proyecto (confirmar el tamaño de celda actual del Tilemap antes de generarlo — probablemente 32x32 o 16x16, revisar en el `Grid` de la escena).
- **Formato**: PNG, fondo transparente donde no haya relleno (si aplica), tileable en el eje horizontal (que no se note la costura al repetirse).

## 2. Frame de Jump — PlayerMale y PlayerFemale

- **Qué es**: falta un frame de salto dedicado en ambos spritesheets (`PlayerMale` tiene 5 frames sin Jump, `PlayerFemale` tiene 4 sin Jump).
- **Por qué importa ahora**: es transversal a los 5 niveles, no solo Nivel 1 — sin esto, el salto se ve con el frame de Idle o Walk congelado, rompe la sensación de movimiento en todos los niveles.
- **Estilo**: debe calzar exactamente con el estilo, paleta y proporciones de los frames existentes de cada personaje (Idle/Walk) — no es un personaje nuevo, es un frame adicional del mismo spritesheet.
- **Pose sugerida**: piernas flexionadas/recogidas, brazos ligeramente elevados — pose de salto clásica de plataformero 2D.
- **Tamaño**: igual al de los frames existentes de cada spritesheet (confirmar dimensión exacta mirando los PNG actuales de `PlayerMale`/`PlayerFemale` antes de generar, para que el nuevo frame encaje sin reescalar).
- **Entrega**: agregar el frame nuevo a la misma hoja/spritesheet existente, o como PNG individual del mismo tamaño si el spritesheet se maneja por Sprite Sheet slicing en Unity — confirmar cuál es el flujo que ya usa el proyecto antes de generarlo.
