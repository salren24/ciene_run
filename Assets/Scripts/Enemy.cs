using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [Header("Patrulla")]
    public float speed = 2f;
    public float patrolDistance = 3f;

    [Header("Detección de borde / pared (opcional)")]
    public Transform edgeCheck;
    public Transform wallCheck;
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Derrota al pisarlo")]
    public float stompBounceForce = 10f;
    public int scoreOnStomp = 100;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Collider2D col;
    Vector3 startPos;
    int direction = -1;
    bool dead;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        startPos = transform.position;

        if (groundLayer.value == 0)
            groundLayer = LayerMask.GetMask("Ground");
    }

    void FixedUpdate()
    {
        if (dead)
            return;

        if (ShouldTurn())
            direction *= -1;

        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        if (sr != null)
            sr.flipX = direction > 0;
    }

    bool ShouldTurn()
    {
        float limitLeft = startPos.x - patrolDistance;
        float limitRight = startPos.x + patrolDistance;

        if (transform.position.x <= limitLeft && direction < 0)
            return true;
        if (transform.position.x >= limitRight && direction > 0)
            return true;

        if (wallCheck != null && Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer))
            return true;

        if (edgeCheck != null && !Physics2D.OverlapCircle(edgeCheck.position, checkRadius, groundLayer))
            return true;

        return false;
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
        // Comparar posiciones/velocidad en vez de puntos de contacto individuales:
        // con caidas rapidas el motor de fisica a veces reporta el contacto en la
        // esquina superior del collider en vez de la cara de arriba, lo que hacia
        // fallar el pisoton de forma intermitente.
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

        PlayerController player = playerCollider.GetComponent<PlayerController>();
        if (player != null)
            player.Bounce(stompBounceForce);

        rb.linearVelocity = Vector2.zero;
        col.enabled = false;
        Destroy(gameObject, 0.05f);
    }

    void HurtPlayer()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.LoseLife();
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? startPos : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin + Vector3.left * patrolDistance, origin + Vector3.right * patrolDistance);

        if (edgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(edgeCheck.position, checkRadius);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }
    }
}
