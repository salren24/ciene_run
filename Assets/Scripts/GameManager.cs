using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Nivel")]
    public string worldLabel = "1-1";
    public string playerLabel = "CIENE";
    public float levelTime = 300f;
    public int startingLives = 3;

    [Header("Escenas")]
    public string characterSelectScene = "CharacterSelect";
    public string[] levelScenes = { "Level1", "Level2", "Level3", "Level4", "Level5" };

    public int CurrentLevelIndex { get; private set; }
    public string CurrentLevelScene => levelScenes[Mathf.Clamp(CurrentLevelIndex, 0, levelScenes.Length - 1)];
    public bool IsLastLevel => CurrentLevelIndex >= levelScenes.Length - 1;

    public int Score { get; private set; }
    public int Coins { get; private set; }
    public int LevelCoinsCollected { get; private set; }
    public int Lives { get; private set; }
    public float TimeLeft { get; private set; }
    public bool IsPaused { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsGameCompleted { get; private set; }
    public Vector3 RespawnPoint { get; private set; }
    public bool HasRespawnPoint { get; private set; }

    public System.Action OnHudChanged;
    public System.Action OnPausedChanged;
    public System.Action OnGameOver;
    public System.Action OnGameCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetRun(true);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int index = System.Array.IndexOf(levelScenes, scene.name);
        if (index < 0)
            return;

        CurrentLevelIndex = index;
        worldLabel = "1-" + (CurrentLevelIndex + 1);
        TimeLeft = levelTime;
        IsPaused = false;
        IsGameOver = false;
        IsGameCompleted = false;
        LevelCoinsCollected = 0;
        HasRespawnPoint = false;
        Time.timeScale = 1f;
        NotifyHud();
    }

    void Update()
    {
        if (IsGameOver || IsGameCompleted)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            TogglePause();

        if (IsPaused)
            return;

        if (SceneManager.GetActiveScene().name != CurrentLevelScene)
            return;

        TimeLeft -= Time.deltaTime;
        if (TimeLeft <= 0f)
        {
            TimeLeft = 0f;
            NotifyHud();
            LoseLife();
            return;
        }

        OnHudChanged?.Invoke();
    }

    public void ResetRun(bool fullReset)
    {
        if (fullReset)
        {
            Score = 0;
            Coins = 0;
            Lives = startingLives;
            CurrentLevelIndex = 0;
        }

        TimeLeft = levelTime;
        IsPaused = false;
        IsGameOver = false;
        IsGameCompleted = false;
        Time.timeScale = 1f;
        NotifyHud();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        NotifyHud();
    }

    public void AddCoin(int amount = 1)
    {
        Coins += amount;
        LevelCoinsCollected += amount;
        AddScore(200 * amount);

        if (Coins >= 100)
        {
            Coins -= 100;
            Lives++;
        }

        NotifyHud();
    }

    public void SetRespawnPoint(Vector3 point)
    {
        RespawnPoint = point;
        HasRespawnPoint = true;
    }

    public void LoseLife()
    {
        if (IsGameOver)
            return;

        Lives--;
        NotifyHud();

        if (Lives <= 0)
        {
            TriggerGameOver();
            return;
        }

        Time.timeScale = 1f;
        IsPaused = false;

        if (HasRespawnPoint)
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                player.RespawnAt(RespawnPoint);
                return;
            }
        }

        SceneManager.LoadScene(CurrentLevelScene);
    }

    public void ReachGoal()
    {
        int timeBonus = Mathf.FloorToInt(TimeLeft) * 50;
        AddScore(Mathf.Max(0, timeBonus));
        Time.timeScale = 1f;
        IsPaused = false;

        if (!IsLastLevel)
        {
            CurrentLevelIndex++;
            SceneManager.LoadScene(CurrentLevelScene);
        }
        else
        {
            TriggerGameCompleted();
        }
    }

    public void TogglePause()
    {
        if (IsGameOver || IsGameCompleted)
            return;

        if (SceneManager.GetActiveScene().name != CurrentLevelScene)
            return;

        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        OnPausedChanged?.Invoke();
    }

    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        OnPausedChanged?.Invoke();
    }

    public void RestartLevel()
    {
        ResetRun(false);
        SceneManager.LoadScene(CurrentLevelScene);
    }

    public void RetryFromGameOver()
    {
        ResetRun(true);
        SceneManager.LoadScene(levelScenes[0]);
    }

    public void QuitToMenu()
    {
        ResetRun(false);
        SceneManager.LoadScene(characterSelectScene);
    }

    void TriggerGameOver()
    {
        IsGameOver = true;
        IsPaused = true;
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
        OnPausedChanged?.Invoke();
    }

    void TriggerGameCompleted()
    {
        IsGameCompleted = true;
        IsPaused = true;
        Time.timeScale = 0f;
        OnGameCompleted?.Invoke();
        OnPausedChanged?.Invoke();
    }

    void NotifyHud()
    {
        OnHudChanged?.Invoke();
    }

    public static void EnsureExists()
    {
        if (Instance != null)
            return;

        new GameObject("GameManager").AddComponent<GameManager>();
    }
}
