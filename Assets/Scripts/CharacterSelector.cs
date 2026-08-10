using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("Escena de juego")]
    public string levelSceneName = "Level1";

    Font font;

    void Start()
    {
        GameManager.EnsureExists();
        Time.timeScale = 1f;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
               ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        EnsureEventSystem();
        BuildTitleUI();
    }

    public void SelectMale()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Male");
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelSceneName);
    }

    public void SelectFemale()
    {
        PlayerPrefs.SetString("SelectedCharacter", "Female");
        PlayerPrefs.Save();
        SceneManager.LoadScene(levelSceneName);
    }

    void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    void BuildTitleUI()
    {
        if (FindAnyObjectByType<Canvas>() != null)
            return;

        GameObject canvasGo = new GameObject("TitleCanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Fondo estilo cielo Mario
        CreateFullscreenPanel(canvasGo.transform, "Sky", new Color(0.36f, 0.58f, 0.95f, 1f));

        // Franja de “pasto” inferior
        GameObject grass = CreatePanel(canvasGo.transform, "Grass", new Vector2(0, 70), new Vector2(1400, 140),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Color(0.22f, 0.72f, 0.28f, 1f));

        CreatePanel(canvasGo.transform, "Dirt", new Vector2(0, 20), new Vector2(1400, 40),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Color(0.55f, 0.32f, 0.14f, 1f));

        // Título
        CreateText(canvasGo.transform, "Title", "CIENE RUN", new Vector2(0, 210), new Vector2(700, 80),
            64, new Color(1f, 0.2f, 0.2f, 1f));
        CreateText(canvasGo.transform, "Subtitle", "WORLD 1-1", new Vector2(0, 145), new Vector2(400, 40),
            28, new Color(1f, 0.9f, 0.2f, 1f));
        CreateText(canvasGo.transform, "SelectHint", "ELIGE TU PERSONAJE", new Vector2(0, 80), new Vector2(500, 36),
            24, Color.white);

        CreateCharacterCard(canvasGo.transform, "HOMBRE", new Vector2(-190, -20),
            new Color(0.2f, 0.45f, 0.9f, 1f), SelectMale);
        CreateCharacterCard(canvasGo.transform, "MUJER", new Vector2(190, -20),
            new Color(0.85f, 0.25f, 0.55f, 1f), SelectFemale);

        CreateText(canvasGo.transform, "Controls",
            "En juego: ← → mover  ·  Shift correr  ·  Espacio saltar  ·  Esc pausa",
            new Vector2(0, 55), new Vector2(900, 28), 16, new Color(1f, 1f, 1f, 0.85f),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));

        // Evita warning unused
        _ = grass;
    }

    void CreateCharacterCard(Transform parent, string label, Vector2 pos, Color color, UnityEngine.Events.UnityAction onClick)
    {
        GameObject card = CreatePanel(parent, label + "Card", pos, new Vector2(240, 160),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), color);

        CreateText(card.transform, "Label", label, new Vector2(0, 35), new Vector2(220, 40), 28, Color.white);

        GameObject play = CreatePanel(card.transform, "Play", new Vector2(0, -35), new Vector2(160, 44),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Color(0.15f, 0.7f, 0.25f, 1f));

        Button button = card.AddComponent<Button>();
        button.targetGraphic = card.GetComponent<Image>();
        button.onClick.AddListener(onClick);

        // También el botón verde
        Button playBtn = play.AddComponent<Button>();
        playBtn.targetGraphic = play.GetComponent<Image>();
        playBtn.onClick.AddListener(onClick);
        CreateText(play.transform, "PlayLabel", "¡JUGAR!", Vector2.zero, new Vector2(160, 44), 20, Color.white);
    }

    void CreateFullscreenPanel(Transform parent, string name, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        go.AddComponent<Image>().color = color;
    }

    GameObject CreatePanel(Transform parent, string name, Vector2 pos, Vector2 size,
        Vector2 anchorMin, Vector2 anchorMax, Color color)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;
        go.AddComponent<Image>().color = color;
        return go;
    }

    Text CreateText(Transform parent, string name, string content, Vector2 pos, Vector2 size,
        int fontSize, Color color)
    {
        return CreateText(parent, name, content, pos, size, fontSize, color,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
    }

    Text CreateText(Transform parent, string name, string content, Vector2 pos, Vector2 size,
        int fontSize, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return text;
    }
}
