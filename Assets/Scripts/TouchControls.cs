using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchControls : MonoBehaviour
{
    [Header("Tamaños")]
    public float buttonSize = 110f;
    public float jumpButtonSize = 150f;

    static readonly Color ShadowColor = new Color(0f, 0f, 0f, 0.35f);
    static readonly Vector2 ShadowOffset = new Vector2(0f, -6f);

    static readonly Color MoveColor = new Color(0.16f, 0.2f, 0.32f, 0.72f);
    static readonly Color MovePressed = new Color(0.26f, 0.32f, 0.48f, 0.9f);

    static readonly Color RunColor = new Color(0.85f, 0.5f, 0.08f, 0.75f);
    static readonly Color RunPressed = new Color(1f, 0.62f, 0.12f, 0.92f);

    static readonly Color LookDownColor = new Color(0.1f, 0.5f, 0.55f, 0.72f);
    static readonly Color LookDownPressed = new Color(0.15f, 0.68f, 0.75f, 0.9f);

    static readonly Color JumpColor = new Color(0.78f, 0.14f, 0.18f, 0.78f);
    static readonly Color JumpPressed = new Color(0.95f, 0.22f, 0.25f, 0.95f);

    static readonly Color RingColor = new Color(1f, 1f, 1f, 0.55f);

    PlayerController player;
    Font font;

    bool leftHeld;
    bool rightHeld;
    bool runHeld;
    bool lookDownHeld;

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
        player.SetLookDown(lookDownHeld);
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

        float gap = 24f;

        // Cluster izquierdo: salto (principal) + mirar abajo
        CreateCircleButton(canvasGo.transform, "JumpButton", "SALTO",
            new Vector2(110, 100), jumpButtonSize, JumpColor, JumpPressed,
            () => { if (player != null) player.TryJump(); },
            () => { if (player != null) player.TryJumpRelease(); });

        CreateCircleButton(canvasGo.transform, "LookDownButton", "▼",
            new Vector2(110 + jumpButtonSize + 30, 100), buttonSize * 0.85f, LookDownColor, LookDownPressed,
            () => lookDownHeld = true, () => lookDownHeld = false);

        // Cluster derecho: movimiento tipo gamepad (◀ ▶ juntos) + correr arriba
        CreateCircleButton(canvasGo.transform, "RightButton", "▶",
            new Vector2(-90, 90), buttonSize, MoveColor, MovePressed,
            () => rightHeld = true, () => rightHeld = false, anchorRight: true);

        CreateCircleButton(canvasGo.transform, "LeftButton", "◀",
            new Vector2(-90 - buttonSize - gap, 90), buttonSize, MoveColor, MovePressed,
            () => leftHeld = true, () => leftHeld = false, anchorRight: true);

        CreateCircleButton(canvasGo.transform, "RunButton", "RUN",
            new Vector2(-90 - (buttonSize + gap) / 2f, 90 + buttonSize + gap), buttonSize, RunColor, RunPressed,
            () => runHeld = true, () => runHeld = false, anchorRight: true);
    }

    void CreateCircleButton(Transform parent, string name, string label, Vector2 pos, float size,
        Color baseColor, Color pressedColor, System.Action onDown, System.Action onUp, bool anchorRight = false)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        Vector2 anchor = anchorRight ? new Vector2(1f, 0f) : Vector2.zero;
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(size, size);
        rect.anchoredPosition = pos;

        // Sombra (da sensacion de boton elevado)
        GameObject shadowGo = new GameObject("Shadow");
        shadowGo.transform.SetParent(go.transform, false);
        RectTransform shadowRect = shadowGo.AddComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.offsetMin = Vector2.zero;
        shadowRect.offsetMax = Vector2.zero;
        shadowRect.anchoredPosition = ShadowOffset;
        Image shadowImage = shadowGo.AddComponent<Image>();
        shadowImage.sprite = PlaceholderSprite.Circle();
        shadowImage.color = ShadowColor;

        // Cuerpo del boton
        GameObject bodyGo = new GameObject("Body");
        bodyGo.transform.SetParent(go.transform, false);
        RectTransform bodyRect = bodyGo.AddComponent<RectTransform>();
        bodyRect.anchorMin = Vector2.zero;
        bodyRect.anchorMax = Vector2.one;
        bodyRect.offsetMin = Vector2.zero;
        bodyRect.offsetMax = Vector2.zero;
        Image bodyImage = bodyGo.AddComponent<Image>();
        bodyImage.sprite = PlaceholderSprite.Circle();
        bodyImage.color = baseColor;

        // Borde/anillo
        GameObject ringGo = new GameObject("Ring");
        ringGo.transform.SetParent(go.transform, false);
        RectTransform ringRect = ringGo.AddComponent<RectTransform>();
        ringRect.anchorMin = Vector2.zero;
        ringRect.anchorMax = Vector2.one;
        ringRect.offsetMin = Vector2.zero;
        ringRect.offsetMax = Vector2.zero;
        Image ringImage = ringGo.AddComponent<Image>();
        ringImage.sprite = PlaceholderSprite.Ring();
        ringImage.color = RingColor;
        ringImage.raycastTarget = false;

        CreateLabel(go.transform, label, size);

        EventTrigger trigger = go.AddComponent<EventTrigger>();
        AddTriggerEntry(trigger, EventTriggerType.PointerDown, () =>
        {
            bodyImage.color = pressedColor;
            onDown();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerUp, () =>
        {
            bodyImage.color = baseColor;
            onUp();
        });
        AddTriggerEntry(trigger, EventTriggerType.PointerExit, () =>
        {
            bodyImage.color = baseColor;
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
        text.fontSize = Mathf.RoundToInt(refSize * (content.Length > 2 ? 0.24f : 0.4f));
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;

        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
        shadow.effectDistance = new Vector2(1.5f, -1.5f);
    }

    void AddTriggerEntry(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }
}
