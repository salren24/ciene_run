using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player"))
            return;

        PlayerController pc = collision.collider.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeHit();
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.LoseLife();
    }
}
