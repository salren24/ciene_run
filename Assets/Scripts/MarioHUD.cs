using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MarioHUD : MonoBehaviour
{
    Text scoreTitle;
    Text scoreLabel;
    Text coinsLabel;
    Text worldLabel;
    Text timeLabel;
    Text livesLabel;
    GameObject pausePanel;
    GameObject gameOverPanel;
    GameObject completedPanel;
    GameObject summaryPanel;
    Text summaryScoreLabel;
    Text summaryNormalCoinsLabel;
    Text summarySpecialCoinsLabel;
    Text powerUpStatusLabel;
    Font font;

    PlayerController cachedPlayer;

    static readonly Color Gold = new Color(1f, 0.85f, 0.2f, 1f);
    static readonly Color Panel = new Color(0.06f, 0.06f, 0.18f, 0.94f);
    static readonly Color PowerUpText = new Color(0.55f, 0.9f, 1f, 1f);

    void Awake()
    {
        GameManager.EnsureExists();
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf")
               ?? Resources.GetBuiltinResource<Font>("Arial.ttf");

        EnsureEventSystem();
        BuildHud();
        BuildPausePanel();
        BuildGameOverPanel();
        BuildCompletedPanel();
        BuildLevelSummaryPanel();
    }

    void OnEnable()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.OnHudChanged += Refresh;
        GameManager.Instance.OnPausedChanged += RefreshPause;
        GameManager.Instance.OnGameOver += ShowGameOver;
        GameManager.Instance.OnGameCompleted += ShowCompleted;
        GameManager.Instance.OnLevelSummary += ShowLevelSummary;
        Refresh();
        RefreshPause();
    }

    void OnDisable()
    {
        if (GameManager.Instance == null)
            return;

        GameManager.Instance.OnHudChanged -= Refresh;
        GameManager.Instance.OnPausedChanged -= RefreshPause;
        GameManager.Instance.OnGameOver -= ShowGameOver;
        GameManager.Instance.OnGameCompleted -= ShowCompleted;
        GameManager.Instance.OnLevelSummary -= ShowLevelSummary;
    }

    void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
            return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    void BuildHud()
    {
        GameObject canvasGo = new GameObject("MarioHUD_Canvas");
        canvasGo.transform.SetParent(transform, false);

        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject bar = CreatePanel(canvasGo.transform, "TopBar", new Vector2(0, -48), new Vector2(1220, 90),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Color(0f, 0f, 0f, 0.4f));

        scoreTitle = CreateText(bar.transform, "ScoreTitle", "CIENE", new Vector2(-450, 18), new Vector2(180, 28), 20, Color.white);
        scoreLabel = CreateText(bar.transform, "ScoreValue", "000000", new Vector2(-450, -14), new Vector2(180, 32), 26, Gold);

        CreateText(bar.transform, "CoinsTitle", "COINS", new Vector2(-180, 18), new Vector2(140, 28), 20, Color.white);
        coinsLabel = CreateText(bar.transform, "CoinsValue", "x00", new Vector2(-180, -14), new Vector2(140, 32), 26, Gold);

        CreateText(bar.transform, "WorldTitle", "WORLD", new Vector2(80, 18), new Vector2(140, 28), 20, Color.white);
        worldLabel = CreateText(bar.transform, "WorldValue", "1-1", new Vector2(80, -14), new Vector2(140, 32), 26, Gold);

        CreateText(bar.transform, "TimeTitle", "TIME", new Vector2(320, 18), new Vector2(120, 28), 20, Color.white);
        timeLabel = CreateText(bar.transform, "TimeValue", "300", new Vector2(320, -14), new Vector2(120, 32), 26, Gold);

        CreateText(bar.transform, "LivesTitle", "LIVES", new Vector2(520, 18), new Vector2(120, 28), 20, Color.white);
        livesLabel = CreateText(bar.transform, "LivesValue", "x3", new Vector2(520, -14), new Vector2(120, 32), 26, Gold);

        Text controlsHint = CreateText(canvasGo.transform, "ControlsHint",
            "← → Mover   |   Shift Correr   |   Espacio Saltar   |   ↓ Mirar abajo   |   Esc / P Pausa",
            new Vector2(0, 26), new Vector2(1000, 28), 15, new Color(1f, 1f, 1f, 0.7f),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f));
        // En mobile los controles son los botones tactiles, no el teclado - este texto solo
        // aplica en desktop, y sobre el area de juego es lo que mas delata un build de prueba.
        if (Application.isMobilePlatform)
            controlsHint.gameObject.SetActive(false);

        powerUpStatusLabel = CreateText(canvasGo.transform, "PowerUpStatus", "",
            new Vector2(0, -98), new Vector2(900, 30), 18, PowerUpText,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f));
    }

    void Update()
    {
        if (powerUpStatusLabel == null)
            return;

        if (cachedPlayer == null)
        {
            cachedPlayer = FindAnyObjectByType<PlayerController>();
            if (cachedPlayer == null)
                return;
        }

        string status = "";
        if (cachedPlayer.HasShield)
            status += "🛡 ESCUDO   ";
        if (cachedPlayer.DoubleJumpActive)
            status += "⇈ DOBLE SALTO   ";
        if (cachedPlayer.SpeedBoostActive)
            status += "⚡ VELOCIDAD   ";

        powerUpStatusLabel.text = status;
    }

    void BuildPausePanel()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        pausePanel = CreatePanel(canvas.transform, "PausePanel", Vector2.zero, new Vector2(440, 340),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Panel);
        pausePanel.SetActive(false);

        CreateText(pausePanel.transform, "Title", "PAUSA", new Vector2(0, 110), new Vector2(360, 50), 42, Gold);
        CreateMenuButton(pausePanel.transform, "CONTINUAR", new Vector2(0, 30), () => GameManager.Instance.Resume());
        CreateMenuButton(pausePanel.transform, "REINICIAR", new Vector2(0, -40), () => GameManager.Instance.RestartLevel());
        CreateMenuButton(pausePanel.transform, "MENÚ", new Vector2(0, -110), () => GameManager.Instance.QuitToMenu());
    }

    void BuildGameOverPanel()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel", Vector2.zero, new Vector2(460, 300),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Panel);
        gameOverPanel.SetActive(false);

        CreateText(gameOverPanel.transform, "Title", "GAME OVER", new Vector2(0, 80), new Vector2(400, 50), 42,
            new Color(1f, 0.35f, 0.35f));
        CreateMenuButton(gameOverPanel.transform, "REINTENTAR", new Vector2(0, -10),
            () => GameManager.Instance.RetryFromGameOver());
        CreateMenuButton(gameOverPanel.transform, "MENÚ", new Vector2(0, -80),
            () => GameManager.Instance.QuitToMenu());
    }

    void BuildCompletedPanel()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        completedPanel = CreatePanel(canvas.transform, "CompletedPanel", Vector2.zero, new Vector2(480, 320),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Panel);
        completedPanel.SetActive(false);

        CreateText(completedPanel.transform, "Title", "¡JUEGO COMPLETADO!", new Vector2(0, 100), new Vector2(440, 50), 32, Gold);
        CreateText(completedPanel.transform, "Subtitle", "Superaste todos los niveles", new Vector2(0, 55), new Vector2(440, 30), 18, Color.white);
        CreateMenuButton(completedPanel.transform, "JUGAR DE NUEVO", new Vector2(0, -10),
            () => GameManager.Instance.RetryFromGameOver());
        CreateMenuButton(completedPanel.transform, "MENÚ", new Vector2(0, -80),
            () => GameManager.Instance.QuitToMenu());
    }

    void BuildLevelSummaryPanel()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        summaryPanel = CreatePanel(canvas.transform, "LevelSummaryPanel", Vector2.zero, new Vector2(520, 440),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Panel);
        summaryPanel.SetActive(false);

        CreateText(summaryPanel.transform, "Title", "¡NIVEL COMPLETADO!", new Vector2(0, 185), new Vector2(480, 40), 28, Gold);

        CreateText(summaryPanel.transform, "NormalCoinsTitle", "MONEDAS NORMALES", new Vector2(0, 100), new Vector2(420, 26), 18, Color.white);
        summaryNormalCoinsLabel = CreateText(summaryPanel.transform, "NormalCoinsValue", "0",
            new Vector2(0, 65), new Vector2(420, 34), 28, Color.white);

        CreateText(summaryPanel.transform, "SpecialCoinsTitle", "MONEDAS ESPECIALES", new Vector2(0, 15), new Vector2(420, 26), 18, Gold);
        summarySpecialCoinsLabel = CreateText(summaryPanel.transform, "SpecialCoinsValue", "0",
            new Vector2(0, -20), new Vector2(420, 34), 28, Gold);

        summaryScoreLabel = CreateText(summaryPanel.transform, "SummaryScore", "SCORE: 000000",
            new Vector2(0, -80), new Vector2(420, 34), 24, Color.white);

        CreateMenuButton(summaryPanel.transform, "CONTINUAR", new Vector2(0, -170),
            () => GameManager.Instance.ContinueAfterSummary());
    }

    void Refresh()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        if (scoreTitle != null)
            scoreTitle.text = gm.playerLabel;
        if (scoreLabel != null)
            scoreLabel.text = gm.Score.ToString("000000");
        if (coinsLabel != null)
            coinsLabel.text = "x" + gm.Coins.ToString("00");
        if (worldLabel != null)
            worldLabel.text = gm.worldLabel;
        if (timeLabel != null)
            timeLabel.text = Mathf.CeilToInt(gm.TimeLeft).ToString();
        if (livesLabel != null)
            livesLabel.text = "x" + gm.Lives;
    }

    void RefreshPause()
    {
        var gm = GameManager.Instance;
        if (gm == null)
            return;

        if (pausePanel != null)
            pausePanel.SetActive(gm.IsPaused && !gm.IsGameOver && !gm.IsGameCompleted && !gm.IsLevelSummary);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(gm.IsGameOver);

        if (completedPanel != null)
            completedPanel.SetActive(gm.IsGameCompleted);

        if (summaryPanel != null)
            summaryPanel.SetActive(gm.IsLevelSummary);
    }

    void ShowGameOver()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void ShowCompleted()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (completedPanel != null)
            completedPanel.SetActive(true);
    }

    void ShowLevelSummary(Sprite image)
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (summaryScoreLabel != null)
                summaryScoreLabel.text = "SCORE: " + gm.Score.ToString("000000");
            if (summaryNormalCoinsLabel != null)
                summaryNormalCoinsLabel.text = gm.NormalCoinsCollected.ToString();
            if (summarySpecialCoinsLabel != null)
                summarySpecialCoinsLabel.text = gm.SpecialCoinsCollected.ToString();
        }

        if (summaryPanel != null)
            summaryPanel.SetActive(true);
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

    void CreateMenuButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject go = new GameObject(label + "Button");
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(280, 50);
        rect.anchoredPosition = pos;

        Image image = go.AddComponent<Image>();
        image.color = new Color(0.82f, 0.12f, 0.12f, 1f);

        Button button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        CreateText(go.transform, "Label", label, Vector2.zero, new Vector2(280, 50), 22, Color.white);
    }
}
