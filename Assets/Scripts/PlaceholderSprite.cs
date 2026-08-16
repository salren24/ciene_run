using UnityEngine;

public static class PlaceholderSprite
{
    static Sprite square;
    static Sprite circle;
    static Sprite ring;
    static Sprite physicalButton;
    static Sprite softShadow;

    public static Sprite Square()
    {
        if (square != null)
            return square;

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        texture.filterMode = FilterMode.Point;

        square = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return square;
    }

    public static Sprite Circle()
    {
        if (circle != null)
            return circle;

        circle = BuildCircleSprite(outlineThickness: 0f);
        return circle;
    }

    public static Sprite Ring()
    {
        if (ring != null)
            return ring;

        ring = BuildCircleSprite(outlineThickness: 10f);
        return ring;
    }

    static Sprite BuildCircleSprite(float outlineThickness)
    {
        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        float innerRadius = outlineThickness > 0f ? radius - outlineThickness : -1f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float outerAlpha = Mathf.Clamp01((radius - dist) / 2f + 0.5f);
                float alpha = outerAlpha;
                if (innerRadius > 0f)
                {
                    float innerAlpha = Mathf.Clamp01((innerRadius - dist) / 2f + 0.5f);
                    alpha = outerAlpha - innerAlpha;
                }
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    // Boton tactil con acabado "fisico": relleno con degradado radial (mas claro al centro),
    // borde oscuro de ~2-3px y un highlight superior (brillo especular), todo en gris/blanco
    // sobre alfa circular - se tine multiplicando por el color de acento de cada boton
    // (el borde oscuro y el highlight claro se leen bien sea cual sea ese color).
    public static Sprite PhysicalButton()
    {
        if (physicalButton != null)
            return physicalButton;

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;
        const float borderThickness = 5f; // ~2-3px reales a los tamaños tipicos de render en pantalla
        float innerRadius = radius - borderThickness;

        Vector2 highlightCenter = center + new Vector2(0f, radius * 0.4f);
        float highlightRadius = radius * 0.8f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 p = new Vector2(x + 0.5f, y + 0.5f);
                float dist = Vector2.Distance(p, center);

                float outerAlpha = Mathf.Clamp01((radius - dist) / 1.5f + 0.5f);
                if (outerAlpha <= 0f)
                {
                    texture.SetPixel(x, y, new Color(0f, 0f, 0f, 0f));
                    continue;
                }

                float shade;
                if (dist > innerRadius)
                {
                    float borderT = Mathf.Clamp01((dist - innerRadius) / borderThickness);
                    shade = Mathf.Lerp(0.45f, 0.12f, borderT);
                }
                else
                {
                    float t = Mathf.Clamp01(dist / innerRadius);
                    shade = Mathf.Lerp(1f, 0.55f, t * t);

                    float hDist = Vector2.Distance(p, highlightCenter);
                    float highlight = Mathf.Clamp01(1f - hDist / highlightRadius);
                    highlight *= highlight;
                    shade = Mathf.Lerp(shade, 1f, highlight * 0.5f);
                }

                texture.SetPixel(x, y, new Color(shade, shade, shade, outerAlpha));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        physicalButton = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return physicalButton;
    }

    // Sombra proyectada con caida suave (mas ancha y difusa que el borde fino de Circle()),
    // pensada para verse detras de un boton fisico, no como un mero circulo desplazado.
    public static Sprite SoftShadow()
    {
        if (softShadow != null)
            return softShadow;

        const int size = 128;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float t = Mathf.Clamp01(dist / radius);
                float alpha = Mathf.Pow(1f - t, 1.8f);
                texture.SetPixel(x, y, new Color(0f, 0f, 0f, alpha));
            }
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        softShadow = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        return softShadow;
    }

    // Silueta plana de cerros para la capa de fondo lejano, usada solo cuando no hay arte
    // propio asignado (hillsSprites vacio en LevelBootstrap). Blanco solido para que se
    // pueda tenir libremente via SpriteRenderer.color (con el fogColor de la paleta).
    // El ruido se muestrea sobre un circulo (en vez de una linea recta) para que el borde
    // izquierdo y el derecho calcen exacto y la silueta haga loop sin costura visible.
    public static Sprite PerlinHills(int width = 512, int height = 160)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        float sampleRadius = 1.6f;
        float seedX = Random.Range(0f, 1000f);
        float seedY = Random.Range(0f, 1000f);

        for (int x = 0; x < width; x++)
        {
            float angle = (x / (float)width) * Mathf.PI * 2f;
            float nx = seedX + Mathf.Cos(angle) * sampleRadius;
            float ny = seedY + Mathf.Sin(angle) * sampleRadius;
            float n = Mathf.PerlinNoise(nx, ny);

            int hillHeight = Mathf.RoundToInt(height * (0.2f + n * 0.55f));

            for (int y = 0; y < height; y++)
                texture.SetPixel(x, y, y < hillHeight ? Color.white : new Color(1f, 1f, 1f, 0f));
        }

        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        // Pivot en la base (0.5, 0): la posicion Y del GameObject queda como la linea de
        // "piso" de los cerros. PPU = height -> alto nativo de 1 unidad, facil de escalar.
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0f), height);
    }
}
