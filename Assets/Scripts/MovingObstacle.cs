using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class MovingObstacle : MonoBehaviour
{
    [Header("Recorrido (A <-> B)")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 1.5f;

    Rigidbody2D rb;
    Vector3 target;
    bool initialized;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void FixedUpdate()
    {
        if (!initialized)
        {
            target = pointB;
            initialized = true;
        }

        Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector3.Distance(next, target) < 0.02f)
            target = target == pointA ? pointB : pointA;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        PlayerController player = collision.collider.GetComponent<PlayerController>();
        if (player != null)
            player.TakeHit();
    }
}
