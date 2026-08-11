using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Range(0f, 1f)] public float parallaxFactor = 0.2f;

    Transform cam;
    Vector3 startPos;
    float startCamX;

    void Start()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
        startPos = transform.position;

        if (cam != null)
            startCamX = cam.position.x;
    }

    void LateUpdate()
    {
        if (cam == null)
            return;

        float deltaX = (cam.position.x - startCamX) * parallaxFactor;
        transform.position = new Vector3(startPos.x + deltaX, startPos.y, startPos.z);
    }
}
