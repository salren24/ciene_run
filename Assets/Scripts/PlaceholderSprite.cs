using UnityEngine;

public static class PlaceholderSprite
{
    static Sprite square;

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
}
