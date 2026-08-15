using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class ElevatorPlatform : MonoBehaviour
{
    [Header("Recorrido (A <-> B), ej. vertical para simular ascensor")]
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;
    public float waitTimeAtEnds = 0.5f;

    Rigidbody2D rb;
    Vector3 target;
    float waitCounter;
    Transform passenger;
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
            transform.position = pointA;
            target = pointB;
            initialized = true;
            return;
        }

        if (waitCounter > 0f)
        {
            waitCounter -= Time.fixedDeltaTime;
            return;
        }

        Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (Vector3.Distance(next, target) < 0.02f)
        {
            target = target == pointA ? pointB : pointA;
            waitCounter = waitTimeAtEnds;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                passenger = collision.transform;
                passenger.SetParent(transform);
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform != passenger)
            return;

        passenger.SetParent(null);
        passenger = null;
    }
}
