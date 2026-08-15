using UnityEngine;

public enum PowerUpType
{
    DoubleJump,
    SpeedBoost,
    Shield
}

[RequireComponent(typeof(Collider2D))]
public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType type;
    public float duration = 12f;
    public int scoreValue = 100;
    bool collected;

    void Reset()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player"))
            return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
            return;

        collected = true;

        switch (type)
        {
            case PowerUpType.DoubleJump:
                player.ActivateDoubleJump(duration);
                break;
            case PowerUpType.SpeedBoost:
                player.ActivateSpeedBoost(duration);
                break;
            case PowerUpType.Shield:
                player.ActivateShield();
                break;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        Destroy(gameObject);
    }
}
