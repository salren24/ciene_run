using UnityEngine;

public static class PlaceholderSprite
{
    static Sprite square;
    static Sprite circle;
    static Sprite ring;

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
}
