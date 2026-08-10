# Tarea para Claude Code: importar y asignar los 16 sprites nuevos

## Contexto

Ya tengo 16 sprites PNG procesados y listos (tamaño final correcto, fondo transparente, sin deformación), correspondientes a los campos `obstacleSprite`, `enemySprite`, `goalBeforeSprite` y `goalAfterSprite` de `LevelBootstrap` en los 5 niveles temáticos (Ciudad Limpia, Seguridad Ciudadana, Educación, Salud, Adulto Mayor).

Archivos (en `sprites_listos_unity.zip`, ya descomprimido en una carpeta):

```
nivel1_obstacleSprite.png   (48x48)
nivel2_obstacleSprite.png   (48x48)
nivel3_obstacleSprite.png   (48x48)
nivel4_obstacleSprite.png   (48x48)
nivel5_obstacleSprite.png   (48x48)
nivel1_enemySprite.png      (32x48)   <- único enemigo generado, solo Nivel 1 (smog)
nivel1_goalBeforeSprite.png (480x270)
nivel2_goalBeforeSprite.png (480x270)
nivel3_goalBeforeSprite.png (480x270)
nivel4_goalBeforeSprite.png (480x270)
nivel5_goalBeforeSprite.png (480x270)
nivel1_goalAfterSprite.png  (480x270)
nivel2_goalAfterSprite.png  (480x270)
nivel3_goalAfterSprite.png  (480x270)
nivel4_goalAfterSprite.png  (480x270)
nivel5_goalAfterSprite.png  (480x270)
```

**Fuera de este paquete (no tocar)**: `coinSprite` de los 5 niveles — todavía no está generado, queda pendiente para otra tarea.

## Checklist de implementación

- [ ] **Importar los 16 PNG** a `Assets/Sprites/Levels/` (o la carpeta de sprites que ya uses en el proyecto).
- [ ] **Configurar cada import setting** vía script (`AssetPostprocessor` o un editor script con `TextureImporter`) para no tener que hacerlo a mano 16 veces:
  - Texture Type = Sprite (2D and UI)
  - Filter Mode = Point (no filter)
  - Compression = None
  - Pixels Per Unit: usar el mismo valor que ya está configurado en el proyecto para el resto de sprites de gameplay (revisar qué PPU se usó al configurar `Pixel Perfect Camera` / `coinSprite` existente) — aplicar ese mismo valor a `obstacleSprite` y `enemySprite`. Para `goalBeforeSprite`/`goalAfterSprite`, el PPU solo importa si se usan como `SpriteRenderer` en el mundo; si se muestran como `UI Image` dentro de un Canvas, el PPU no aplica.
- [ ] **Verificar cómo quedó implementado el sistema de meta** (`goalBeforeSprite`/`goalAfterSprite`): confirmar si `GoalFlag`/`LevelBootstrap` los usa como `SpriteRenderer` en el mundo o como `Image` de UI, y asignar del modo que corresponda a esa implementación real (no asumir).
- [ ] **Asignar los sprites a cada nivel automáticamente**, sin hacerlo a mano en 5 escenas: escribir un script de editor (`[MenuItem]` o ejecutar una vez y luego se puede borrar) que:
  1. Recorra las 5 escenas de nivel.
  2. En cada una, busque el GameObject `LevelSystems_<tema>` con el componente `LevelBootstrap`.
  3. Según el número de nivel (usar el orden/mapeo real de escenas — confirmarlo antes de asignar, no asumir que Level1=nivel1, verificar contra el tema real de cada escena), asigne:
     - `obstacleSprite` ← `nivelN_obstacleSprite.png`
     - `goalBeforeSprite` ← `nivelN_goalBeforeSprite.png`
     - `goalAfterSprite` ← `nivelN_goalAfterSprite.png`
     - `enemySprite` ← `nivel1_enemySprite.png` **solo en el nivel de Ciudad Limpia** (los demás niveles quedan con `enemySprite` vacío, tal como se definió: representar antagonistas humanos en los otros temas no es apropiado, se dejan solo con obstáculos).
  4. Marque la escena como "dirty" (`EditorSceneManager.MarkSceneDirty`) y la guarde.
- [ ] **Confirmar mapeo de escenas antes de asignar**: si el nombre/orden de las 5 escenas no deja claro cuál es cuál tema, listar las escenas y su `LevelSystems_<tema>` encontrado antes de tocar nada, para verificar el mapeo conmigo si hay ambigüedad.

## Fuera de alcance

- [ ] No generar ni tocar `coinSprite` — pendiente para otra tarea.
- [ ] No modificar `TouchControls`, input, ni cámara/Pixel Perfect (salvo lectura del PPU ya configurado, para replicarlo).

## Entregable esperado

1. Los 16 sprites importados con la configuración correcta.
2. Confirmación de qué campo quedó asignado en qué escena (tabla resumen).
3. Aviso si algún nivel no tenía el GameObject `LevelSystems_<tema>` esperado o si el mapeo de escena→tema no era claro.
