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

    [Header("Monedas en patron (si tiene elementos, reemplaza la grilla uniforme de arriba)")]
    public Vector3[] coinPositions = new Vector3[0];

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
    public float deathCenterX = 20f;

    [Header("Fondo parallax (opcional)")]
    public float parallaxStartX = -10f;
    public float parallaxEndMargin = 10f;

    [Header("Parallax - horizonte lejano (ej. goalBeforeSprite)")]
    public Sprite[] skylineSprites;
    public int skylineCount = 3;
    public float skylineParallaxFactor = 0.1f;
    public Vector2 skylineScaleRange = new Vector2(0.9f, 1.1f);
    public float skylineY = 3f;

    [Header("Parallax - fondo cercano (ej. fondo.png)")]
    public Sprite[] backdropSprites;
    public int backdropCount = 6;
    public float backdropParallaxFactor = 0.4f;
    public Vector2 backdropScaleRange = new Vector2(0.95f, 1.05f);
    public float backdropY = -1f;

    [Header("Parallax - nubes")]
    public Sprite[] cloudSprites;
    public int farCloudCount = 2;
    public float farParallaxFactor = 0.15f;
    public Vector2 farCloudScaleRange = new Vector2(1.6f, 2.2f);
    public float farCloudY = 6f;

    public int nearCloudCount = 3;
    public float nearParallaxFactor = 0.35f;
    public Vector2 nearCloudScaleRange = new Vector2(0.7f, 1.1f);
    public float nearCloudY = 4f;

    [Header("Fondo estatico unico (una sola instancia, sin loop, sin parallax)")]
    public Sprite staticBackdropSprite;
    public float staticBackdropCenterX;
    public float staticBackdropCenterY;
    public float staticBackdropTargetHeight = 12f;

    [Header("Moneda premio (cerca de la meta)")]
    public Sprite finalCoinSprite;
    public Vector3 finalCoinPosition;
    public float finalCoinScale = 1.6f;
    public int finalCoinScoreValue = 1000;

    [Header("Monedas especiales (posiciones fijas, tinte dorado, valen mas)")]
    public Vector3[] specialCoinPositions = new Vector3[0];
    public float specialCoinScale = 1.3f;
    public int specialCoinScoreValue = 500;

    [System.Serializable]
    public struct MovingObstacleConfig
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public float speed;
    }

    [Header("Obstaculos en movimiento (opcional)")]
    public MovingObstacleConfig[] movingObstacles = new MovingObstacleConfig[0];

    [System.Serializable]
    public struct ElevatorConfig
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public float speed;
        public float width;
    }

    [Header("Ascensores / plataformas verticales moviles (opcional)")]
    public ElevatorConfig[] elevators = new ElevatorConfig[0];

    [Header("Enemigo de embestida (aparece por tiempo, no por posicion fija)")]
    public Sprite[] chargeEnemyFrames;
    public float[] chargeEnemySpawnTimes = new float[] { 60f, 240f };
    public float chargeEnemySpawnAhead = 11f;
    public float chargeEnemySpawnY;

    [System.Serializable]
    public struct PowerUpConfig
    {
        public Vector3 position;
        public PowerUpType type;
        public float duration;
    }

    [Header("Power-ups (opcional)")]
    public PowerUpConfig[] powerUps = new PowerUpConfig[0];

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

        SpawnParallaxBackground();
        SpawnStaticBackdrop();
        SpawnCoins();
        SpawnSpecialCoins();
        SpawnFinalCoin();
        SpawnEnemies();
        SpawnObstacles();
        SpawnMovingObstacles();
        SpawnElevators();
        SpawnChargeEnemySpawner();
        SpawnPowerUps();
        SpawnGoal();
        SpawnDeathZone();
        SpawnCheckpoint();
    }

    void SpawnStaticBackdrop()
    {
        if (staticBackdropSprite == null)
            return;

        GameObject backdrop = new GameObject("StaticBackdrop");
        backdrop.transform.position = new Vector3(staticBackdropCenterX, staticBackdropCenterY, 0f);

        SpriteRenderer sr = backdrop.AddComponent<SpriteRenderer>();
        sr.sprite = staticBackdropSprite;
        sr.sortingOrder = -12;

        float nativeHeight = staticBackdropSprite.rect.height / staticBackdropSprite.pixelsPerUnit;
        float scale = nativeHeight > 0f ? staticBackdropTargetHeight / nativeHeight : 1f;
        backdrop.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void SpawnFinalCoin()
    {
        if (finalCoinScoreValue <= 0)
            return;

        GameObject coin;
        Sprite art = finalCoinSprite != null ? finalCoinSprite : coinSprite;

        if (art == null && coinPrefab != null)
        {
            // Sin sprite premium propio todavia: reusa el prefab real de moneda
            // (con su animacion de giro) agrandado y con tinte dorado, en vez
            // de un cuadrado plano.
            coin = Instantiate(coinPrefab, finalCoinPosition, Quaternion.identity);
            coin.transform.localScale *= finalCoinScale;

            SpriteRenderer prefabSr = coin.GetComponent<SpriteRenderer>();
            if (prefabSr != null)
                prefabSr.color = new Color(1f, 0.95f, 0.6f, 1f);
        }
        else
        {
            coin = new GameObject("FinalCoin");
            coin.transform.position = finalCoinPosition;
            coin.transform.localScale = Vector3.one * finalCoinScale;

            SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
            sr.sprite = art != null ? art : PlaceholderSprite.Square();
            sr.color = art != null ? Color.white : new Color(1f, 0.85f, 0.15f, 1f);
            sr.sortingOrder = 6;

            CircleCollider2D col = coin.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;
            coin.AddComponent<CoinPickup>();
        }

        coin.name = "FinalCoin";
        CoinPickup pickup = coin.GetComponent<CoinPickup>();
        if (pickup != null)
        {
            pickup.scoreValue = finalCoinScoreValue;
            pickup.isSpecial = true;
        }
    }

    void SpawnMovingObstacles()
    {
        if (movingObstacles == null)
            return;

        for (int i = 0; i < movingObstacles.Length; i++)
        {
            MovingObstacleConfig config = movingObstacles[i];
            GameObject obstacle;

            if (obstaclePrefab != null)
            {
                obstacle = Instantiate(obstaclePrefab, config.pointA, Quaternion.identity);
                Obstacle staticBehaviour = obstacle.GetComponent<Obstacle>();
                if (staticBehaviour != null)
                    Destroy(staticBehaviour);
            }
            else
            {
                obstacle = new GameObject("MovingObstacle");
                obstacle.transform.position = config.pointA;

                SpriteRenderer sr = obstacle.AddComponent<SpriteRenderer>();
                bool hasObstacleArt = obstacleSprite != null;
                sr.sprite = hasObstacleArt ? obstacleSprite : PlaceholderSprite.Square();
                sr.color = hasObstacleArt ? Color.white : new Color(0.75f, 0.15f, 0.15f, 1f);
                sr.sortingOrder = 5;
                obstacle.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

                obstacle.AddComponent<BoxCollider2D>();
            }

            MovingObstacle moving = obstacle.AddComponent<MovingObstacle>();
            moving.pointA = config.pointA;
            moving.pointB = config.pointB;
            moving.speed = config.speed;

            obstacle.name = "MovingObstacle_" + i;
        }
    }

    void SpawnElevators()
    {
        if (elevators == null)
            return;

        int groundLayer = LayerMask.NameToLayer("Ground");

        for (int i = 0; i < elevators.Length; i++)
        {
            ElevatorConfig config = elevators[i];
            float width = config.width > 0f ? config.width : 3f;

            GameObject platform = new GameObject("Elevator_" + i);
            platform.transform.position = config.pointA;
            if (groundLayer >= 0)
                platform.layer = groundLayer;

            SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSprite.Square();
            sr.color = new Color(0.62f, 0.4f, 0.18f, 1f);
            sr.sortingOrder = 4;
            platform.transform.localScale = new Vector3(width, 0.5f, 1f);

            BoxCollider2D col = platform.AddComponent<BoxCollider2D>();
            col.size = Vector2.one;

            ElevatorPlatform elevator = platform.AddComponent<ElevatorPlatform>();
            elevator.pointA = config.pointA;
            elevator.pointB = config.pointB;
            elevator.speed = config.speed > 0f ? config.speed : 2f;
        }
    }

    void SpawnChargeEnemySpawner()
    {
        if (chargeEnemyFrames == null || chargeEnemyFrames.Length == 0)
            return;

        GameObject spawnerGo = new GameObject("ChargeEnemySpawner");
        ChargeEnemySpawner spawner = spawnerGo.AddComponent<ChargeEnemySpawner>();
        spawner.chargeFrames = chargeEnemyFrames;
        spawner.spawnTimes = chargeEnemySpawnTimes;
        spawner.spawnAheadDistance = chargeEnemySpawnAhead;
        spawner.spawnY = chargeEnemySpawnY;
    }

    void SpawnParallaxBackground()
    {
        // fitWidthMultiplier>0 hace que cada copia se escale para cubrir (con leve solape) el
        // espacio entre copias, dando una capa continua - usado para horizonte/fondo, no para nubes.
        SpawnScrollingLayer(skylineSprites, skylineCount, skylineParallaxFactor, skylineScaleRange, skylineY, -15, "Skyline", 1.15f);
        SpawnScrollingLayer(backdropSprites, backdropCount, backdropParallaxFactor, backdropScaleRange, backdropY, -13, "Backdrop", 1.1f, alternateFlip: true);
        SpawnScrollingLayer(cloudSprites, farCloudCount, farParallaxFactor, farCloudScaleRange, farCloudY, -10, "CloudFar");
        SpawnScrollingLayer(cloudSprites, nearCloudCount, nearParallaxFactor, nearCloudScaleRange, nearCloudY, -9, "CloudNear");
    }

    void SpawnScrollingLayer(Sprite[] sprites, int count, float parallaxFactor, Vector2 scaleRange, float y,
        int sortingOrder, string label, float fitWidthMultiplier = 0f, bool alternateFlip = false)
    {
        if (sprites == null || sprites.Length == 0 || count <= 0)
            return;

        float spanStart = parallaxStartX;
        float spanEnd = goalX + parallaxEndMargin;
        float step = count > 1 ? (spanEnd - spanStart) / (count - 1) : spanEnd - spanStart;

        for (int i = 0; i < count; i++)
        {
            Sprite sprite = sprites[i % sprites.Length];
            float x = spanStart + i * step;
            float variance = Random.Range(scaleRange.x, scaleRange.y);

            GameObject layer = new GameObject(label + "_" + i);
            layer.transform.position = new Vector3(x, y, 0f);

            SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = sortingOrder;

            float scale;
            if (fitWidthMultiplier > 0f)
            {
                float nativeWidth = sprite.rect.width / sprite.pixelsPerUnit;
                float targetWidth = step * fitWidthMultiplier;
                scale = (nativeWidth > 0f ? targetWidth / nativeWidth : 1f) * variance;
            }
            else
            {
                scale = variance;
            }

            float flipX = (alternateFlip && i % 2 == 1) ? -1f : 1f;
            layer.transform.localScale = new Vector3(scale * flipX, scale, 1f);

            ParallaxLayer parallax = layer.AddComponent<ParallaxLayer>();
            parallax.parallaxFactor = parallaxFactor;
        }
    }

    void SpawnCoins()
    {
        bool usePattern = coinPositions != null && coinPositions.Length > 0;
        int count = usePattern ? coinPositions.Length : coinCount;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = usePattern
                ? coinPositions[i]
                : new Vector3(coinStartX + i * coinSpacing, coinY + (i % 2) * 0.6f, 0f);
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

    static readonly Color SpecialCoinTint = new Color(0.86f, 0.93f, 1f, 1f);

    void SpawnSpecialCoins()
    {
        if (specialCoinPositions == null)
            return;

        for (int i = 0; i < specialCoinPositions.Length; i++)
        {
            Vector3 pos = specialCoinPositions[i];
            GameObject coin;

            if (coinPrefab != null)
            {
                coin = Instantiate(coinPrefab, pos, Quaternion.identity);
                coin.transform.localScale *= specialCoinScale;

                SpriteRenderer prefabSr = coin.GetComponent<SpriteRenderer>();
                if (prefabSr != null)
                    prefabSr.color = SpecialCoinTint;
            }
            else
            {
                coin = new GameObject("SpecialCoin");
                coin.transform.position = pos;
                coin.transform.localScale = Vector3.one * specialCoinScale;

                SpriteRenderer sr = coin.AddComponent<SpriteRenderer>();
                bool hasCoinArt = coinSprite != null;
                sr.sprite = hasCoinArt ? coinSprite : PlaceholderSprite.Square();
                sr.color = SpecialCoinTint;
                sr.sortingOrder = 5;

                CircleCollider2D col = coin.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.35f;
                coin.AddComponent<CoinPickup>();
            }

            GameObject halo = new GameObject("Halo");
            halo.transform.SetParent(coin.transform, false);
            halo.transform.localPosition = Vector3.zero;
            halo.transform.localScale = Vector3.one * 2.1f;
            SpriteRenderer haloSr = halo.AddComponent<SpriteRenderer>();
            haloSr.sprite = PlaceholderSprite.Circle();
            haloSr.color = new Color(SpecialCoinTint.r, SpecialCoinTint.g, SpecialCoinTint.b, 0.35f);
            haloSr.sortingOrder = 3;

            coin.name = "SpecialCoin_" + i;
            CoinPickup pickup = coin.GetComponent<CoinPickup>();
            if (pickup != null)
            {
                pickup.isSpecial = true;
                pickup.scoreValue = specialCoinScoreValue;
            }
        }
    }

    static readonly Color DoubleJumpColor = new Color(0.35f, 0.9f, 0.4f, 1f);
    static readonly Color SpeedBoostColor = new Color(1f, 0.75f, 0.15f, 1f);
    static readonly Color ShieldPowerUpColor = new Color(0.3f, 0.65f, 1f, 1f);

    void SpawnPowerUps()
    {
        if (powerUps == null)
            return;

        for (int i = 0; i < powerUps.Length; i++)
        {
            PowerUpConfig config = powerUps[i];

            GameObject go = new GameObject("PowerUp_" + config.type + "_" + i);
            go.transform.position = config.position;
            go.transform.localScale = Vector3.one * 0.9f;

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSprite.Circle();
            sr.color = PowerUpColorFor(config.type);
            sr.sortingOrder = 5;

            GameObject halo = new GameObject("Halo");
            halo.transform.SetParent(go.transform, false);
            halo.transform.localScale = Vector3.one * 1.8f;
            SpriteRenderer haloSr = halo.AddComponent<SpriteRenderer>();
            haloSr.sprite = PlaceholderSprite.Ring();
            haloSr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.6f);
            haloSr.sortingOrder = 4;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;

            PowerUpPickup pickup = go.AddComponent<PowerUpPickup>();
            pickup.type = config.type;
            pickup.duration = config.duration > 0f ? config.duration : 12f;
        }
    }

    static Color PowerUpColorFor(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.DoubleJump: return DoubleJumpColor;
            case PowerUpType.SpeedBoost: return SpeedBoostColor;
            case PowerUpType.Shield: return ShieldPowerUpColor;
            default: return Color.white;
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
                enemy.transform.localScale = new Vector3(2.86f, 2.86f, 1f);

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

        BoxCollider2D col = goal.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(1.6f, 3.8f);
        col.offset = new Vector2(0.3f, 1.9f);

        BuildGoalVisual(goal.transform);

        GoalFlag flag = goal.AddComponent<GoalFlag>();
        bool usePattern = coinPositions != null && coinPositions.Length > 0;
        flag.beforeSprite = goalBeforeSprite;
        flag.afterSprite = goalAfterSprite;
        flag.totalCollectibles = usePattern ? coinPositions.Length : coinCount;
        flag.completionThreshold = completionThreshold;
    }

    void BuildGoalVisual(Transform parent)
    {
        CreateGoalPart(parent, "Base", new Vector3(0f, 0.1f, 0f), new Vector2(0.7f, 0.2f), new Color(0.35f, 0.35f, 0.38f), 5);
        CreateGoalPart(parent, "Pole", new Vector3(0f, 1.8f, 0f), new Vector2(0.14f, 3.4f), new Color(0.42f, 0.3f, 0.2f), 5);
        CreateGoalPart(parent, "Banner", new Vector3(0.4f, 3.0f, 0f), new Vector2(1.1f, 0.7f), new Color(0.18f, 0.75f, 0.35f), 6);
        CreateGoalPart(parent, "BannerAccent", new Vector3(0.4f, 3.0f, 0f), new Vector2(0.7f, 0.35f), Color.white, 7);
        CreateGoalPart(parent, "Finial", new Vector3(0f, 3.55f, 0f), new Vector2(0.28f, 0.28f), new Color(1f, 0.85f, 0.2f), 5);
    }

    void CreateGoalPart(Transform parent, string name, Vector3 localPos, Vector2 size, Color color, int sortingOrder)
    {
        GameObject part = new GameObject(name);
        part.transform.SetParent(parent, false);
        part.transform.localPosition = localPos;
        part.transform.localScale = new Vector3(size.x, size.y, 1f);

        SpriteRenderer sr = part.AddComponent<SpriteRenderer>();
        sr.sprite = PlaceholderSprite.Square();
        sr.color = color;
        sr.sortingOrder = sortingOrder;
    }

    void SpawnDeathZone()
    {
        GameObject death = new GameObject("DeathZone");
        death.transform.position = new Vector3(deathCenterX, deathY, 0f);
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
