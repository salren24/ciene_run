using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class SpriteImportNormalizer
{
    const string SpritesRoot = "Assets/Sprites";
    const int LargeSideThreshold = 800;

    [MenuItem("CIENE RUN/Normalizar import settings")]
    public static void NormalizeImportSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { SpritesRoot });
        int changed = 0;
        int skippedNotPng = 0;
        int withMipmaps = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                skippedNotPng++;
                continue;
            }

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                continue;

            importer.GetSourceTextureWidthAndHeight(out int width, out int height);
            bool useMipmaps = ShouldUseMipmaps(path, width, height);

            // No se toca textureType, spritePixelsPerUnit ni textureCompression aqui:
            // el PPU esta calibrado por asset para el tamaño de mundo correcto, y la
            // compresion se deja en lo que Unity trae por defecto (no forzar Uncompressed).
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = useMipmaps;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;

            EditorUtility.SetDirty(importer);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            changed++;
            if (useMipmaps)
                withMipmaps++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[SpriteImportNormalizer] Import settings normalizados en {changed} texturas bajo {SpritesRoot} " +
                   $"(Bilinear, alphaIsTransparency, Clamp). Mipmaps ON en {withMipmaps} (fondos grandes), OFF en el resto " +
                   $"(tiles/monedas/personajes/obstaculos/enemigos). PPU y compresion intactos. Omitidos (no .png): {skippedNotPng}.");
    }

    // Mipmaps solo tienen sentido quality-wise en fondos que se ven muy reducidos en pantalla
    // (nubes de parallax lejano, fondos de meta). En tiles/sprites de gameplay, que se ven a
    // escala ~1:1 contra la grilla, los mipmaps solo aportan el shimmer/flicker del bug reportado.
    static bool ShouldUseMipmaps(string assetPath, int width, int height)
    {
        string normalized = assetPath.Replace('\\', '/');
        string fileName = Path.GetFileName(normalized);

        if (fileName.StartsWith("nube", StringComparison.OrdinalIgnoreCase))
            return true;

        if (normalized.IndexOf("/Levels/", StringComparison.OrdinalIgnoreCase) >= 0
            && fileName.IndexOf("goal", StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        return Mathf.Max(width, height) > LargeSideThreshold;
    }
}
