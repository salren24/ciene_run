using UnityEngine;

public class LevelBootstrap : MonoBehaviour
{
    [Header("Prefabs opcionales")]
    public GameObject coinPrefab;
    public GameObject enemyPrefab;
    public GameObject obstaclePrefab;

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
                sr.sprite = PlaceholderSprite.Square();
                sr.color = new Color(1f, 0.85f, 0.15f, 1f);
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
                sr.sprite = PlaceholderSprite.Square();
                sr.color = new Color(0.55f, 0.2f, 0.75f, 1f);
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
                sr.sprite = PlaceholderSprite.Square();
                sr.color = new Color(0.75f, 0.15f, 0.15f, 1f);
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
        goal.AddComponent<GoalFlag>();
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
}
