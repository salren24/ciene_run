using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchControls : MonoBehaviour
{
    [Header("Tamaños")]
    public float buttonSize = 110f;
    public float jumpButtonSize = 140f;

    static readonly Color Normal = new Color(1f, 1f, 1f, 0.35f);
    static readonly Color Pressed = new Color(1f, 1f, 1f, 0.6f);

    PlayerController player;
    Font font;

    bool leftHeld;
    bool rightHeld;
    bool runHeld;

    void Awake()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
               ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        EnsureEventSystem();
        BuildUI();
    }

    void Update()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<PlayerController>();
            if (player == null)
                return;
        }

        float move = 0f;
        if (leftHeld) move -= 1f;
        if (rightHeld) move += 1f;

        player.SetMoveInput(move);
        player.SetRunHeld(runHeld);
    }

    void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    void BuildUI()
    {
        GameObject canvasGo = new GameObject("TouchControls_Canvas");
        canvasGo.transform.SetParent(transform, false);

        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        float gap = 20f;

        CreateHoldButton(canvasGo.transform, "LeftButton", "<",
            new Vector2(90, 90), new Vector2(buttonSize, buttonSize),
            () => leftHeld = true, () => leftHeld = false);

        CreateHoldButton(canvasGo.transform, "RightButton", ">",
            new Vector2(90 + buttonSize + gap, 90), new Vector2(buttonSize, buttonSize),
            () => rightHeld = true, () => rightHeld = false);

        CreateHoldButton(canvasGo.transform, "RunButton", "RUN",
            new Vector2(90 + (buttonSize + gap) * 2, 90), new Vector2(buttonSize, buttonSize),
            () => runHeld = true, () => runHeld = false);

        CreateJumpButton(canvasGo.transform);
    }

    void CreateJumpButton(Transform parent)
    {
        GameObject go = new GameObject("JumpButton");
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 0f);
        rect.anchorMax = new Vector2(1f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(jumpButtonSize, jumpButtonSize);
        rect.anchoredPosition = new Vector2(-110, 100);

        Image image = go.AddComponent<Image>();
        image.color = Normal;
        CreateLabel(go.transform, "SALTO", jumpButtonSize);

        EventTrigger trigger = go.AddComponent<EventTrigger>();
        AddTriggerEntry(trigger, EventTriggerType.PointerDown, () =>
        {
            image.color = Pressed;
            if (player != null)
                player.TryJump();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerUp, () =>
        {
            image.color = Normal;
            if (player != null)
                player.TryJumpRelease();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, () =>
        {
            image.color = Normal;
            if (player != null)
                player.TryJumpRelease();
        });
    }

    void CreateHoldButton(Transform parent, string name, string label, Vector2 pos, Vector2 size,
        System.Action onDown, System.Action onUp)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = pos;

        Image image = go.AddComponent<Image>();
        image.color = Normal;
        CreateLabel(go.transform, label, size.y);

        EventTrigger trigger = go.AddComponent<EventTrigger>();
        AddTriggerEntry(trigger, EventTriggerType.PointerDown, () =>
        {
            image.color = Pressed;
            onDown();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerUp, () =>
        {
            image.color = Normal;
            onUp();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, () =>
        {
            image.color = Normal;
            onUp();
        });
    }

    void CreateLabel(Transform parent, string content, float refSize)
    {
        GameObject go = new GameObject("Label");
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = font;
        text.fontSize = Mathf.RoundToInt(refSize * 0.3f);
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
    }

    void AddTriggerEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }
}
