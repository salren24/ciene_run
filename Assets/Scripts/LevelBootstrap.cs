using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Header("Prefabs opcionales")]
    public GameObject coinPrefab;
    public GameObject enemyPrefab;
    public GameObject obstaclePrefab;

    [Header("Sprites tematicos (opcional, si no hay prefab)")]
    public Sprite coinSprite;
    public Sprite enemySprite;
    public Sprite obstacleSprite;
    public Sprite goalBeforeSprite;
    public Sprite goalAfterSprite;

    [Header("Estructura de 3 actos (organizativo)")]
    public float actIEndX = 40f;
    public float actIIEndX = 80f;

    [Header("Checkpoint (opcional)")]
    public bool spawnCheckpoint = false;
    public float checkpointX = 20f;
    public float checkpointY = 1f;

    [Header("Umbral de completitud (meta)")]
    [Range(0f, 1f)] public float completionThreshold = 0.7f;

    [Header("Monedas de prueba")]
    public int coinCount = 8;
    public float coinStartX = 4f;
    public float coinSpacing = 3f;
    public float coinY = 1.5f;

    [Header("Enemigos de prueba")]
    public int enemyCount = 2;
    public float enemyStartX = 14f;
    public float enemySpacing = 10f;
    public float enemyY = 0.5f;

    [Header("Obstáculos de prueba")]
    public float[] obstacleX = new float[] { 24f, 34f };
    public float obstacleY = 0.5f;

    [Header("Meta")]
    public float goalX = 42f;
    public float goalY = 1f;

    [Header("Muerte por caída")]
    public float deathY = -8f;
    public float deathWidth = 120f;

    void Awake()
    {
        GameManager.EnsureExists();

        if (FindAnyObjectByType<MarioHUD>() == null)
        {
            GameObject hud = new GameObject("MarioHUD");
            hud.AddComponent<MarioHUD>();
        }

        if (FindAnyObjectByType<TouchControls>() == null)
        {
            GameObject controls = new GameObject("TouchControls");
            controls.AddComponent<TouchControls>();
        }

        SpawnCoins();
        SpawnEnemies();
        SpawnObstacles();
        SpawnGoal();
        SpawnDeathZone();
        SpawnCheckpoint();
    }

    void SpawnCoins()
    {
        for (int i = 0; i < coinCount; i++)
        {
            Vector3 pos = new Vector3(coinStartX + i * coinSpacing, coinY + (i % 2) * 0.6f, 0f);
            GameObject coin;

            if (coinPrefab != null)
            {
                coin = Instantiate(coinPrefab, pos, Quaternion.identity);
            }
            else
            {
                coin = new GameObject("Coin");
                coin.transform.position = pos;

                SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
                bool hasCoinArt = coinSprite != null;
                sr.sprite = hasCoinArt ? coinSprite : PlaceholderSprite.Square();
                sr.color = hasCoinArt ? Color.white : new Color(1f, 0.85f, 0.15f, 1f);
                sr.sortingOrder = 5;

                CircleCollider2D col = coin.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.35f;
                coin.transform.localScale = Vector3.one * 0.8f;
                coin.AddComponent<CoinPickup>();
            }

            coin.name = "Coin_" + i;
        }
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 pos = new Vector3(enemyStartX + i * enemySpacing, enemyY, 0f);
            GameObject enemy;

            if (enemyPrefab != null)
            {
                enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
            else
            {
                enemy = new GameObject("Enemy");
                enemy.transform.position = pos;

                SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
                bool hasEnemyArt = enemySprite != null;
                sr.sprite = hasEnemyArt ? enemySprite : PlaceholderSprite.Square();
                sr.color = hasEnemyArt ? Color.white : new Color(0.55f, 0.2f, 0.75f, 1f);
                sr.sortingOrder = 5;
                enemy.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

                Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
                rb.freezeRotation = true;
                rb.gravityScale = 3f;

                enemy.AddComponent<BoxCollider2D>();
                enemy.AddComponent<Enemy>();
            }

            enemy.name = "Enemy_" + i;
        }
    }

    void SpawnObstacles()
    {
        if (obstacleX == null)
            return;

        for (int i = 0; i < obstacleX.Length; i++)
        {
            Vector3 pos = new Vector3(obstacleX[i], obstacleY, 0f);
            GameObject obstacle;

            if (obstaclePrefab != null)
            {
                obstacle = Instantiate(obstaclePrefab, pos, Quaternion.identity);
            }
            else
            {
                obstacle = new GameObject("Obstacle");
                obstacle.transform.position = pos;

                SpriteRenderer sr = obstacle.AddComponent<SpriteRenderer>();
                bool hasObstacleArt = obstacleSprite != null;
                sr.sprite = hasObstacleArt ? obstacleSprite : PlaceholderSprite.Square();
                sr.color = hasObstacleArt ? Color.white : new Color(0.75f, 0.15f, 0.15f, 1f);
                sr.sortingOrder = 5;
                obstacle.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

                obstacle.AddComponent<BoxCollider2D>();
                obstacle.AddComponent<Obstacle>();
            }

            obstacle.name = "Obstacle_" + i;
        }
    }

    void SpawnGoal()
    {
        GameObject goal = new GameObject("GoalFlag");
        goal.transform.position = new Vector3(goalX, goalY, 0f);

        SpriteRenderer sr = goal.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Square();
        sr.color = new Color(0.2f, 0.85f, 0.35f, 1f);
        sr.sortingOrder = 6;

        BoxCollider2D col = goal.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.8f, 3.5f);
        goal.transform.localScale = new Vector3(1f, 3.5f, 1f);

        GoalFlag flag = goal.AddComponent<GoalFlag>();
        flag.beforeSprite = goalBeforeSprite;
        flag.afterSprite = goalAfterSprite;
        flag.totalCollectibles = coinCount;
        flag.completionThreshold = completionThreshold;

        if (goalBeforeSprite != null || goalAfterSprite != null)
            flag.artRenderer = SpawnGoalArt();
    }

    SpriteRenderer SpawnGoalArt()
    {
        GameObject art = new GameObject("GoalArt");
        art.transform.position = new Vector3(goalX, goalY + 0.3f, 0f);

        SpriteRenderer artSr = art.AddComponent<SpriteRenderer>();
        artSr.sprite = goalBeforeSprite != null ? goalBeforeSprite : goalAfterSprite;
        artSr.sortingOrder = 4;

        const float targetWidth = 6f;
        float nativeWidth = artSr.sprite.rect.width / artSr.sprite.pixelsPerUnit;
        float scale = nativeWidth > 0f ? targetWidth / nativeWidth : 1f;
        art.transform.localScale = new Vector3(scale, scale, 1f);

        return artSr;
    }

    void SpawnDeathZone()
    {
        GameObject death = new GameObject("DeathZone");
        death.transform.position = new Vector3(20f, deathY, 0f);
        BoxCollider2D col = death.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(deathWidth, 2f);
        death.AddComponent<DeathZone>();
    }

    void SpawnCheckpoint()
    {
        if (!spawnCheckpoint)
            return;

        GameObject checkpoint = new GameObject("Checkpoint");
        checkpoint.transform.position = new Vector3(checkpointX, checkpointY, 0f);
        BoxCollider2D col = checkpoint.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.6f, 3f);
        checkpoint.AddComponent<Checkpoint>();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(actIEndX, -10f, 0f), new Vector3(actIEndX, 10f, 0f));
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(actIIEndX, -10f, 0f), new Vector3(actIIEndX, 10f, 0f));
    }
}
