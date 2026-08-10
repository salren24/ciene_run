using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalFlag : MonoBehaviour
{
    bool reached;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (reached || !other.CompareTag("Player"))
            return;

        reached = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(5000);
            GameManager.Instance.ReachGoal();
        }
    }
}
