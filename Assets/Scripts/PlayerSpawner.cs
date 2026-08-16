using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Prefabs de personaje")]
    public GameObject malePrefab;
    public GameObject femalePrefab;

    [Header("Opciones")]
    public bool destroyExistingPlayer = true;

    void Start()
    {
        SpawnSelectedCharacter();
    }

    public GameObject SpawnSelectedCharacter()
    {
        if (destroyExistingPlayer)
        {
            PlayerController existing = FindAnyObjectByType<PlayerController>();
            if (existing != null)
                Destroy(existing.gameObject);
        }

        string selected = PlayerPrefs.GetString("SelectedCharacter", "Male");
        GameObject prefab = selected == "Female" ? femalePrefab : malePrefab;

        if (prefab == null)
        {
            Debug.LogError("PlayerSpawner: falta asignar el prefab de " + selected);
            return null;
        }

        GameObject player = Instantiate(prefab, transform.position, Quaternion.identity);
        player.name = prefab.name;

        SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingLayerName = SortingLayers.Player;

        CameraFollow cameraFollow = FindAnyObjectByType<CameraFollow>();
        if (cameraFollow != null)
            cameraFollow.target = player.transform;

        return player;
    }
}
