using UnityEngine;

// Ajusta este RectTransform a Screen.safeArea, para que su contenido no quede debajo de
// notches, esquinas redondeadas o barras de gestos. Se aplica sobre un contenedor hijo del
// Canvas (no sobre el Canvas raiz: un Canvas Overlay raiz ignora anchors en su propio
// RectTransform, siempre ocupa toda la pantalla).
[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    RectTransform rect;
    Rect lastSafeArea;
    Vector2Int lastScreenSize;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        Apply();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea || Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y)
            Apply();
    }

    void Apply()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;
        lastScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (Screen.width <= 0 || Screen.height <= 0)
            return;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
    }
}
