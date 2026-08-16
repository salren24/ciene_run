using UnityEngine;

public class ChargeEnemySpawner : MonoBehaviour
{
    [Header("Sprites del enemigo (mismos frames para patrulla y embestida)")]
    public Sprite[] chargeFrames;

    [Header("Apariciones (segundos transcurridos desde el inicio del nivel)")]
    public float[] spawnTimes = new float[] { 60f, 240f };
    public float spawnAheadDistance = 11f;
    public float spawnY;

    bool[] spawned;

    void Update()
    {
        if (GameManager.Instance == null || spawnTimes == null)
            return;

        if (spawned == null || spawned.Length != spawnTimes.Length)
            spawned = new bool[spawnTimes.Length];

        float elapsed = GameManager.Instance.levelTime - GameManager.Instance.TimeLeft;

        for (int i = 0; i < spawnTimes.Length; i++)
        {
            if (!spawned[i] && elapsed >= spawnTimes[i])
            {
                spawned[i] = true;
                SpawnAhead();
            }
        }
    }

    void SpawnAhead()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        float px = player != null ? player.transform.position.x : transform.position.x;
        float x = px + spawnAheadDistance;

        GameObject enemy = new GameObject("ChargeEnemy");
        enemy.transform.position = new Vector3(x, spawnY, 0f);

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = SortingLayers.Objects;
        sr.sortingOrder = SortingLayers.Order.ChargeEnemy;

        if (chargeFrames != null && chargeFrames.Length > 0 && chargeFrames[0] != null)
        {
            sr.sprite = chargeFrames[0];

            const float targetHeight = 1.7f;
            float nativeHeight = chargeFrames[0].rect.height / chargeFrames[0].pixelsPerUnit;
            float scale = nativeHeight > 0f ? targetHeight / nativeHeight : 1f;
            enemy.transform.localScale = new Vector3(scale, scale, 1f);
        }

        Rigidbody2D rb = enemy.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.gravityScale = 3f;

        enemy.AddComponent<BoxCollider2D>();

        ChargeEnemy chargeEnemy = enemy.AddComponent<ChargeEnemy>();
        chargeEnemy.frames = chargeFrames;
    }
}
