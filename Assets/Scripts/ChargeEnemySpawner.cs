using UnityEngine;

public class ChargeEnemySpawner : MonoBehaviour
{
    [Header("Sprites del enemigo (mismos frames para patrulla y embestida)")]
    public Sprite[] chargeFrames;

    [Header("Disparo por posicion: se activa la primera vez que el jugador cruza cada X " +
        "(una sola vez por entrada). A diferencia del disparo por tiempo, el desafio queda " +
        "siempre en el mismo punto del nivel sin importar cuanto se demore el jugador en llegar.")]
    public float[] spawnX = new float[] { 60f, 240f };
    public float spawnAheadDistance = 11f;
    public float spawnY;

    bool[] spawned;

    void Update()
    {
        if (GameManager.Instance == null || spawnX == null)
            return;

        if (spawned == null || spawned.Length != spawnX.Length)
            spawned = new bool[spawnX.Length];

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null)
            return;

        float playerX = player.transform.position.x;

        for (int i = 0; i < spawnX.Length; i++)
        {
            if (!spawned[i] && playerX >= spawnX[i])
            {
                spawned[i] = true;
                SpawnAhead(playerX);
            }
        }
    }

    void SpawnAhead(float playerX)
    {
        float x = playerX + spawnAheadDistance;

        GameObject enemy = new GameObject("ChargeEnemy");
        enemy.transform.position = new Vector3(x, spawnY, 0f);

        SpriteRenderer sr = enemy.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = SortingLayers.Objects;
        sr.sortingOrder = SortingLayers.Order.ChargeEnemy;

        if (chargeFrames != null && chargeFrames.Length > 0 && chargeFrames[0] != null)
        {
            sr.sprite = chargeFrames[0];

            const float targetHeight = 0.8f;
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
