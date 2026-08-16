using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SkyGradient : MonoBehaviour
{
    const int TextureHeight = 256;
    const float LocalDepth = 50f; // lejos de la camara en su eje local, detras de todo lo demas

    Camera cam;
    SpriteRenderer sr;
    Vector2 nativeSpriteSize;

    public void Initialize(Camera targetCamera, SkyPalette palette)
    {
        cam = targetCamera;
        sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = SortingLayers.Sky;
        sr.sortingOrder = 0;

        // Unlit a proposito: el cielo es un fondo plano, no debe depender del Light2D
        // global ni tenirse con el (Sprite-Lit-Default se ve negro si la luz no llega
        // a la sorting layer Sky).
        sr.material = new Material(Shader.Find("Sprites/Default"));

        Gradient gradient = palette != null && palette.skyGradient != null ? palette.skyGradient : FlatFallbackGradient();
        Texture2D texture = BuildGradientTexture(gradient);

        // PPU = TextureHeight -> el sprite nativo mide exactamente 1 unidad de alto (y
        // 1/TextureHeight de ancho), asi el Scale Y resultante en Resize() coincide
        // directamente con la altura visible en unidades (facil de verificar a ojo en el
        // inspector), en vez de depender del PPU por defecto de Sprite.Create (100).
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, TextureHeight), new Vector2(0.5f, 0.5f), TextureHeight);
        sr.sprite = sprite;
        nativeSpriteSize = sprite.bounds.size;

        Resize();
    }

    // Mismo celeste que tenia la camara como color solido, por si algun dia se instancia sin paleta.
    static Gradient FlatFallbackGradient()
    {
        Gradient gradient = new Gradient();
        Color flat = new Color(0.45f, 0.72f, 0.95f);
        gradient.SetKeys(
            new[] { new GradientColorKey(flat, 0f), new GradientColorKey(flat, 1f) },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        return gradient;
    }

    static Texture2D BuildGradientTexture(Gradient gradient)
    {
        Texture2D texture = new Texture2D(1, TextureHeight, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < TextureHeight; y++)
        {
            float t = y / (float)(TextureHeight - 1);
            texture.SetPixel(0, y, gradient.Evaluate(t));
        }

        texture.Apply();
        return texture;
    }

    void LateUpdate()
    {
        Resize();
    }

    void Resize()
    {
        if (cam == null || sr == null || sr.sprite == null)
            return;

        // orthographicSize es dinamico (CameraFollow lo recalcula segun aspect ratio), asi
        // que se recalcula el encuadre del cielo cada frame en vez de cachearlo.
        transform.localPosition = new Vector3(0f, 0f, LocalDepth);

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;

        transform.localScale = new Vector3(width / nativeSpriteSize.x, height / nativeSpriteSize.y, 1f);
    }
}
