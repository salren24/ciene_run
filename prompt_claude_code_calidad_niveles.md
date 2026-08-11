# Tarea para Claude Code: elevar calidad visual y diseño de niveles

## Contexto

El nivel de detalle actual (ver captura de `Level4`) está muy por debajo del objetivo: suelo con textura repetitiva plana, monedas/personaje sin sprites definitivos, niveles cortos con poca variación de altura. Se usaron capturas de Super Mario World como referencia de **calidad y ritmo de diseño** — importante: NO se debe copiar el tileset ni el diseño de nivel de Nintendo (tubos verdes, bloques `?`, paleta exacta), es material con copyright. Lo que se replica es el **estándar de calidad y principios de diseño**, con arte 100% original del proyecto.

## Principios de diseño a replicar (de las referencias, sin copiar assets)

- **Variación de elevación constante**: nunca más de 4-5 segundos de suelo plano sin cambio de altura, escalón, hueco o plataforma.
- **Densidad de monedas en patrones**: las monedas no van sueltas al azar, se agrupan en arcos, líneas diagonales o filas que guían el salto del jugador (como se ve en las referencias).
- **Tileset con bordes definidos**: el suelo necesita un tile de "borde/top" con contorno visible (no una textura repetida sin definición) y un tile de "relleno" distinto para la profundidad — ya existe la estructura para esto en `LevelMapBuilder` (`groundTop` vs `groundFill`), solo falta el arte real.
- **Fondos con profundidad (parallax simple)**: capas de fondo (cielo, colinas lejanas, elementos del primer plano) en vez de un color plano celeste sólido.
- **Silueta de personaje legible**: el sprite del jugador debe distinguirse claramente contra el fondo en cualquier tramo del nivel.

## Checklist de implementación

- [ ] **Antes de tocar código**: generar/solicitar el tileset original de suelo (`groundTop`, `groundFill`) por tema de nivel — 5 variantes (una por tema: Ciudad Limpia, Seguridad, Educación, Salud, Adulto Mayor), coherente con la paleta de cada tema pero SIN reutilizar diseño de Mario.
- [ ] **Antes de tocar código**: solicitar sprite de jugador definitivo (spritesheet con Idle/Run/Jump) — actualmente sigue siendo un placeholder básico.
- [ ] **Antes de tocar código**: solicitar 2-3 elementos de fondo por tema (capas de parallax simple: cielo con gradiente, silueta de edificios/montañas, algún elemento temático).
- [ ] **No avanzar a generar/pedir arte sin mi aprobación del diseño de CADA nivel primero.** Flujo por nivel:
  1. Proponer el layout del nivel (dónde van los cambios de altura, huecos, agrupaciones de monedas, ubicación de obstáculos/enemigos ya existentes) en formato texto o diagrama simple — sin escribir código todavía.
  2. Esperar mi aprobación explícita del diseño de ESE nivel.
  3. Recién ahí implementar el layout en `LevelMapBuilder`/`LevelBootstrap` para ese nivel.
  4. Pasar al siguiente nivel solo después de que el anterior esté aprobado e implementado.
- [ ] **Orden sugerido**: empezar por Nivel 1 (Ciudad Limpia), ya que es el que se está probando actualmente.
- [ ] **Extender duración de forma variada**: no solo alargar `endX` en línea recta — usar el array `platforms` para generar tramos con altura variable, similar al ritmo de las referencias (secciones de subida, bajada, huecos, plataformas flotantes).

## Fuera de alcance

- [ ] No copiar ni recrear assets reconocibles de Super Mario World (tubos, bloques `?`, Bowser, hongos, etc.) ni su paleta de colores específica.
- [ ] No tocar `TouchControls`, input, ni cámara/Pixel Perfect.
- [ ] No reasignar `coinSprite`, `obstacleSprite`, etc. — eso ya está resuelto en los 5 niveles.

## Entregable esperado (por cada nivel, en este orden)

1. Propuesta de layout del nivel + lista de assets nuevos necesarios (tileset, fondo, etc.) — esperar aprobación.
2. Una vez aprobado: implementación del layout con los assets ya generados.
3. Confirmación de que el nivel quedó funcional en Play antes de pasar al siguiente.
