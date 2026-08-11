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

    [Header("Franja continua de suelo")]
    public int startX = -8;
    public int endX = 45;
    public int groundY = -1;
    public int groundDepth = 4;

    [Header("Plataformas opcionales (x, y, ancho)")]
    public Vector3Int[] platforms = new Vector3Int[0];

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

        if (groundFill == null && groundTop == null)
        {
            Debug.LogWarning("LevelMapBuilder: asigna al menos un tile de suelo.");
            return;
        }

        TileBase top = groundTop != null ? groundTop : groundFill;
        TileBase fill = groundFill != null ? groundFill : groundTop;

        if (clearBeforeBuild)
            tilemap.ClearAllTiles();

        // Franja principal, saltando las columnas marcadas como hueco y respetando el perfil de altura
        for (int x = startX; x <= endX; x++)
        {
            if (IsGap(x))
                continue;

            int y = GetGroundY(x);
            tilemap.SetTile(new Vector3Int(x, y, 0), top);
            for (int d = 1; d < groundDepth; d++)
                tilemap.SetTile(new Vector3Int(x, y - d, 0), fill);
        }

        if (platforms == null)
            return;

        foreach (Vector3Int platform in platforms)
        {
            int width = Mathf.Max(1, platform.z);
            for (int i = 0; i < width; i++)
                tilemap.SetTile(new Vector3Int(platform.x + i, platform.y, 0), top);
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
