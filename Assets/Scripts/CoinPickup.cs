using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    public int coinValue = 1;
    public int scoreValue = 200;
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

        collected = true;

        if (GameManager.Instance != null)
            GameManager.Instance.AddCoin(coinValue, scoreValue);
        else
            Debug.Log("Moneda recogida (+" + scoreValue + ")");

        Destroy(gameObject);
    }
}
