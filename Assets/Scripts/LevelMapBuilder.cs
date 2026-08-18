using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TerrainSegment
{
    public int startX;
    public int endX;
    public int height;
}

[RequireComponent(typeof(Tilemap))]
public class LevelMapBuilder : MonoBehaviour
{
    [Header("Tiles")]
    public TileBase groundTop;
    public TileBase groundFill;

    [Header("RuleTile (opcional) - si esta asignado, reemplaza groundTop/groundFill por completo: " +
        "se pinta el mismo tile en todas las celdas y el RuleTile resuelve el sprite (borde/esquina/" +
        "relleno) por vecindad. Si es null, se mantiene el comportamiento actual.")]
    public TileBase groundRuleTile;

    [Header("Franja continua de suelo")]
    public int startX = -8;
    public int endX = 45;
    public int groundY = -1;

    [Header("El relleno de la franja principal llega hasta esta fila (no hasta groundY - groundDepth): " +
        "asi el terreno macizo toca el borde inferior de pantalla en vez de flotar sobre el vacio.")]
    public int groundBottomY = -20;

    [Header("Grosor SOLO de las plataformas flotantes de platforms[] (la franja principal usa groundBottomY)")]
    public int groundDepth = 2;

    [Header("Plataformas opcionales (x, y, ancho)")]
    public Vector3Int[] platforms = new Vector3Int[0];

    [Header("Tile opcional para platforms[] (si tiene elementos, reemplaza groundRuleTile/groundTop " +
        "en esas celdas - se cicla por indice para variar entre piezas sueltas, p.ej. bloques de " +
        "concreto en vez de una franja de tierra)")]
    public TileBase[] platformTiles = new TileBase[0];

    [Header("Huecos en la franja principal (x inicio, x fin) - ambos inclusive")]
    public Vector2Int[] gaps = new Vector2Int[0];

    [Header("Perfil de terreno con altura variable (opcional, vacio = franja plana a groundY)")]
    public TerrainSegment[] terrainProfile = new TerrainSegment[0];

    [Header("Opciones")]
    public bool buildOnAwake = true;
    public bool clearBeforeBuild = true;

    Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        if (buildOnAwake)
            Build();
    }

    [ContextMenu("Build Continuous Ground")]
    public void Build()
    {
        if (tilemap == null)
            tilemap = GetComponent<Tilemap>();

        bool usingRuleTile = groundRuleTile != null;

        if (!usingRuleTile && groundFill == null && groundTop == null)
        {
            Debug.LogWarning("LevelMapBuilder: asigna al menos un tile de suelo.");
            return;
        }

        TileBase top = groundTop != null ? groundTop : groundFill;
        TileBase fill = groundFill != null ? groundFill : groundTop;

        if (clearBeforeBuild)
            tilemap.ClearAllTiles();

        // Franja principal, saltando las columnas marcadas como hueco y respetando el perfil de altura.
        // El relleno baja hasta groundBottomY (no una cantidad fija de filas), asi el terreno macizo
        // siempre toca el borde inferior de pantalla en vez de flotar sobre el vacio.
        // Con RuleTile se pinta el mismo tile en todas las filas (el RuleTile decide el sprite segun
        // vecinos); sin RuleTile se mantiene el esquema anterior de top + fill.
        for (int x = startX; x <= endX; x++)
        {
            if (IsGap(x))
                continue;

            int y = GetGroundY(x);

            if (usingRuleTile)
            {
                for (int py = y; py >= groundBottomY; py--)
                    tilemap.SetTile(new Vector3Int(x, py, 0), groundRuleTile);
            }
            else
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), top);
                for (int py = y - 1; py >= groundBottomY; py--)
                    tilemap.SetTile(new Vector3Int(x, py, 0), fill);
            }
        }

        if (platforms == null)
            return;

        // Las plataformas flotantes de platforms[] SI usan groundDepth (grosor fijo, no llegan al piso).
        TileBase platformTile = usingRuleTile ? groundRuleTile : top;
        for (int p = 0; p < platforms.Length; p++)
        {
            Vector3Int platform = platforms[p];
            TileBase overrideTile = (platformTiles != null && platformTiles.Length > 0)
                ? platformTiles[p % platformTiles.Length]
                : null;

            int width = Mathf.Max(1, platform.z);
            for (int i = 0; i < width; i++)
            {
                if (overrideTile != null)
                {
                    // Pieza suelta (p.ej. bloque de concreto): mismo tile en todas las filas,
                    // sin distincion top/fill, no participa del RuleTile de piso.
                    for (int d = 0; d < groundDepth; d++)
                        tilemap.SetTile(new Vector3Int(platform.x + i, platform.y - d, 0), overrideTile);
                }
                else if (usingRuleTile)
                {
                    for (int d = 0; d < groundDepth; d++)
                        tilemap.SetTile(new Vector3Int(platform.x + i, platform.y - d, 0), groundRuleTile);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(platform.x + i, platform.y, 0), platformTile);
                    for (int d = 1; d < groundDepth; d++)
                        tilemap.SetTile(new Vector3Int(platform.x + i, platform.y - d, 0), fill);
                }
            }
        }
    }

    bool IsGap(int x)
    {
        if (gaps == null)
            return false;

        foreach (Vector2Int gap in gaps)
        {
            if (x >= gap.x && x <= gap.y)
                return true;
        }

        return false;
    }

    int GetGroundY(int x)
    {
        if (terrainProfile != null)
        {
            foreach (TerrainSegment segment in terrainProfile)
            {
                if (x >= segment.startX && x <= segment.endX)
                    return groundY + segment.height;
            }
        }

        return groundY;
    }
}
