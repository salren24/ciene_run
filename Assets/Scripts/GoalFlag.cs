using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GoalFlag : MonoBehaviour
{
    [Header("Transformacion tematica (opcional)")]
    public Sprite beforeSprite;
    public Sprite afterSprite;
    public SpriteRenderer artRenderer;

    [Header("Completitud")]
    public int totalCollectibles = 0;
    [Range(0f, 1f)] public float completionThreshold = 0.7f;

    [Header("Transicion")]
    public float revealDelay = 1f;

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

        bool hasTransformation = artRenderer != null && afterSprite != null;
        if (hasTransformation)
        {
            RevealTransformation();
            StartCoroutine(FinishAfterDelay(revealDelay));
        }
        else
        {
            Finish();
        }
    }

    void RevealTransformation()
    {
        artRenderer.sprite = afterSprite;
        artRenderer.color = IsComplete() ? Color.white : new Color(0.6f, 0.6f, 0.6f, 1f);
    }

    bool IsComplete()
    {
        if (totalCollectibles <= 0 || GameManager.Instance == null)
            return true;

        return (float)GameManager.Instance.LevelCoinsCollected / totalCollectibles >= completionThreshold;
    }

    IEnumerator FinishAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Finish();
    }

    void Finish()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(5000);
            GameManager.Instance.ReachGoal();
        }
    }
}
