using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax (fraccion del movimiento de camara que se traslada a esta capa)")]
    [Range(0f, 1f)] public float parallaxFactorX = 0.2f;
    [Range(0f, 1f)] public float parallaxFactorY = 0f;

    [Header("Loop horizontal infinito (0 = sin loop)")]
    [Tooltip("Ancho total del pool de copias hermanas (spacing * cantidad de copias). " +
             "Todas las copias de un mismo grupo deben compartir el mismo valor.")]
    public float loopWidth = 0f;

    [Header("Auto-scroll (deriva propia, ademas del parallax de camara - para nubes con viento)")]
    public bool autoScroll = false;
    public float autoScrollSpeed = 0f;

    Transform cam;
    Vector3 startPos;
    float startCamX;
    float startCamY;
    float autoScrollOffset;

    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        startPos = transform.position;

        if (cam != null)
        {
            startCamX = cam.position.x;
            startCamY = cam.position.y;
        }

        // Unlit a proposito, igual que el cielo: un fondo de parallax no debe depender
        // del Light2D ni tenirse con el (con Sprite-Lit-Default se veria negro si la luz
        // no alcanza esta sorting layer).
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.material = new Material(Shader.Find("Sprites/Default"));
    }

    void LateUpdate()
    {
        if (cam == null)
            return;

        if (autoScroll)
            autoScrollOffset += autoScrollSpeed * Time.deltaTime;

        float deltaX = (cam.position.x - startCamX) * parallaxFactorX;
        float deltaY = (cam.position.y - startCamY) * parallaxFactorY;

        float x = startPos.x + deltaX + autoScrollOffset;
        float y = startPos.y + deltaY;

        if (loopWidth > 0.01f)
        {
            // Envuelve la posicion X relativa a la camara dentro de [-loopWidth/2, loopWidth/2).
            // Con N copias hermanas espaciadas loopWidth/N entre si, esto las mantiene siempre
            // repartidas alrededor de la camara sin importar cuanto avance el nivel.
            float relativeX = x - cam.position.x;
            relativeX = Mathf.Repeat(relativeX + loopWidth * 0.5f, loopWidth) - loopWidth * 0.5f;
            x = cam.position.x + relativeX;
        }

        transform.position = new Vector3(x, y, startPos.z);
    }
}
