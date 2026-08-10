# Especificación de sprites pixel art — 5 niveles de campaña

## Guía de estilo (aplicar a TODOS los sprites, para consistencia visual)

- **Estilo**: pixel art 2D, mismo nivel de detalle que sprites clásicos estilo Super Mario World (NO fotorrealista, NO vectorial suave).
- **Paleta**: colores planos con 2-3 tonos de sombreado por color (dithering mínimo), contornos oscuros de 1px.
- **Tamaño base**:
  - Coleccionables (`coinSprite`): 32x32 px
  - Obstáculos (`obstacleSprite`): 32x32 px o 48x48 px (según si es objeto pequeño o mediano)
  - Enemigos (`enemySprite`): 32x48 px (formato vertical, deja espacio para "piernas")
  - Fondos de meta (`goalBeforeSprite` / `goalAfterSprite`): 480x270 px (horizontal, se ve de fondo en la zona de meta)
- **Formato de archivo**: PNG con fondo transparente, sin compresión con pérdida.
- **Import settings en Unity** (recordatorio, no es parte del encargo de arte): Filter Mode = Point (no filter), Compression = None, Pixels Per Unit = 32.

## Tabla de assets por nivel

### Nivel 1 — Ciudad Limpia

| Campo | Descripción |
|---|---|
| `coinSprite` | Bolsa de basura verde reciclable con símbolo de reciclaje, o moneda con ícono de hoja/reciclaje |
| `obstacleSprite` | Tacho de basura desbordado, bolsas cayendo, líneas de "mal olor" estilizadas |
| `enemySprite` | Nube de smog/contaminación con cara simple (estilo amenaza leve, no agresiva) |
| `goalBeforeSprite` | Calle con basura acumulada, tachos rotos, cielo gris |
| `goalAfterSprite` | Misma calle limpia, árboles nuevos, tachos ordenados, cielo despejado |

### Nivel 2 — Seguridad Ciudadana

| Campo | Descripción |
|---|---|
| `coinSprite` | Moto de serenazgo en miniatura o cámara de vigilancia estilizada |
| `obstacleSprite` | Cono de tránsito naranja con cinta de "zona insegura" |
| `enemySprite` | *(opcional — evaluar si se necesita, evitar representar delincuencia de forma directa/estigmatizante; considerar dejar sin enemigo temático y usar solo obstáculos)* |
| `goalBeforeSprite` | Terreno baldío oscuro, sin iluminación |
| `goalAfterSprite` | Centro de videovigilancia iluminado, patrulla estacionada, referencia visual a `comisaria.png` ya entregado |

### Nivel 3 — Educación

| Campo | Descripción |
|---|---|
| `coinSprite` | Libro cerrado con lomo de color, o mochila escolar pequeña |
| `obstacleSprite` | Pupitre roto con pata quebrada, o gotera con charco debajo |
| `enemySprite` | *(opcional — evaluar necesidad; alternativa: sin enemigo, solo obstáculos)* |
| `goalBeforeSprite` | Aula deteriorada, pintura descascarada, pupitres desordenados |
| `goalAfterSprite` | Colegio remodelado, fachada nueva, niños entrando — referencia visual a `nuevo_colegio.png` |

### Nivel 4 — Salud

| Campo | Descripción |
|---|---|
| `coinSprite` | Cruz médica roja/blanca estilizada, o pastilla/cápsula pixel art |
| `obstacleSprite` | Camilla vacía o fila de sillas de espera desordenadas |
| `enemySprite` | *(opcional — evaluar necesidad)* |
| `goalBeforeSprite` | Posta médica antigua, fachada deteriorada, sin ambulancia |
| `goalAfterSprite` | Centro médico ampliado con ambulancia — referencia visual a `centro_medico_rio_seco.png` |

### Nivel 5 — Adulto Mayor

| Campo | Descripción |
|---|---|
| `coinSprite` | Canasta de víveres pequeña o símbolo de bienestar (corazón/manos, estilo cálido, no infantilizante) |
| `obstacleSprite` | Banca de parque rota, o rampa de acceso bloqueada con barrera |
| `enemySprite` | *(opcional — evaluar necesidad, este nivel probablemente funciona mejor solo con obstáculos, sin enemigos)* |
| `goalBeforeSprite` | Terreno vacío o casa antigua sin adaptar |
| `goalAfterSprite` | Casa del Adulto Mayor terminada — referencia visual a `casa_adulto_mayor.png` |

## Nota sobre `enemySprite`

En 4 de los 5 niveles marqué el enemigo como opcional a evaluar. Representar "delincuencia", "enfermedad" o "abandono" como personaje-enemigo perseguible puede salir mal si no se maneja con cuidado (riesgo de caricaturizar un problema social sensible). Alternativa más segura: usar solo obstáculos estáticos en esos niveles y reservar enemigos con movimiento solo donde tenga sentido claro (ej. el smog en Ciudad Limpia, que es un problema ambiental, no una persona).

## Prioridad sugerida de generación

1. Los 5 `coinSprite` (se ven constantemente, mayor impacto visual)
2. Los 5 `goalAfterSprite` (el "premio" — ya tienes 4 de 5 referencias reales, falta la de Ciudad Limpia)
3. Los 5 `obstacleSprite`
4. Los 5 `goalBeforeSprite`
5. Los `enemySprite` que se decida mantener
