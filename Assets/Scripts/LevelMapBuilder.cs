using UnityEngine;
using UnityEngine.Tilemaps;

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

        // Franja continua sin huecos (primera prueba)
        for (int x = startX; x <= endX; x++)
        {
            tilemap.SetTile(new Vector3Int(x, groundY, 0), top);
            for (int d = 1; d < groundDepth; d++)
                tilemap.SetTile(new Vector3Int(x, groundY - d, 0), fill);
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
}
