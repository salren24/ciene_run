using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;

    [Header("Seguimiento")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);
    public float smoothSpeed = 8f;
    public bool findPlayerOnStart = true;

    [Header("Límites (opcional)")]
    public bool useBounds;
    public float minX = -5f;
    public float maxX = 40f;
    public float minY = -2f;
    public float maxY = 12f;

    void Start()
    {
        if (findPlayerOnStart && target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                PlayerController controller = FindAnyObjectByType<PlayerController>();
                if (controller != null)
                    player = controller.gameObject;
            }

            if (player != null)
                target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desired = target.position + offset;
        desired.z = offset.z;

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
