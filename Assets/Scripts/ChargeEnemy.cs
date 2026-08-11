using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ChargeEnemy : MonoBehaviour
{
    [Header("Sprites (ciclo de animacion, mismos frames para patrulla y embestida)")]
    public Sprite[] frames;
    public float frameRate = 10f;

    [Header("Patrulla")]
    public float patrolSpeed = 1.4f;
    public float patrolRange = 3f;

    [Header("Embestida")]
    public float chargeSpeed = 5.5f;
    public float detectionRadius = 5.5f;
    public float loseRadius = 8f;

    [Header("Golpe al jugador")]
    public int scoreOnStomp = 100;
    public float stompBounceForce = 10f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Collider2D col;
    Transform player;
    Vector3 startPos;
    int direction = -1;
    float frameTimer;
    int frameIndex;
    bool charging;
    bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPos = transform.position;
        FindPlayer();
    }

    void FindPlayer()
    {
        PlayerController pc = FindAnyObjectByType<PlayerController>();
        if (pc != null)
            player = pc.transform;
    }

    void FixedUpdate()
    {
        if (dead)
            return;

        if (player == null)
            FindPlayer();

        UpdateState();

        float speed = charging ? chargeSpeed : patrolSpeed;
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (sr != null)
            sr.flipX = direction > 0;
    }

    void UpdateState()
    {
        if (player == null)
            return;

        float dist = Mathf.Abs(player.position.x - transform.position.x);

        if (charging)
        {
            if (dist > loseRadius)
                charging = false;
            else
                direction = player.position.x >= transform.position.x ? 1 : -1;

            return;
        }

        if (dist <= detectionRadius)
        {
            charging = true;
            direction = player.position.x >= transform.position.x ? 1 : -1;
            return;
        }

        if (transform.position.x <= startPos.x - patrolRange && direction < 0)
            direction = 1;
        else if (transform.position.x >= startPos.x + patrolRange && direction > 0)
            direction = -1;
    }

    void Update()
    {
        if (frames == null || frames.Length == 0 || sr == null)
            return;

        frameTimer += Time.deltaTime;
        float interval = 1f / Mathf.Max(1f, frameRate);
        if (frameTimer < interval)
            return;

        frameTimer -= interval;
        frameIndex = (frameIndex + 1) % frames.Length;
        sr.sprite = frames[frameIndex];
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (dead || !collision.collider.CompareTag("Player"))
            return;

        if (WasStompedFromAbove(collision))
            Die(collision.collider);
        else
            HurtPlayer();
    }

    bool WasStompedFromAbove(Collision2D collision)
    {
        Rigidbody2D playerRb = collision.rigidbody;
        if (playerRb != null && playerRb.linearVelocity.y > 0.1f)
            return false;

        float enemyTop = col.bounds.max.y;
        float playerBottom = collision.collider.bounds.min.y;

        return playerBottom >= enemyTop - 0.25f;
    }

    void Die(Collider2D playerCollider)
    {
        dead = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreOnStomp);

        PlayerController pc = playerCollider.GetComponent<PlayerController>();
        if (pc != null)
            pc.Bounce(stompBounceForce);

        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        Destroy(gameObject, 0.05f);
    }

    void HurtPlayer()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoseLife();
    }
}
