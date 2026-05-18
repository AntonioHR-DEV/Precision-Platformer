using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] private float parallaxFactor = 0.95f;
    // 0 = background is world-locked
    // 1 = background is glued to camera
    private Camera cam;

    private Vector3 lastCameraPosition;

    private void Start()
    {
        cam = Camera.main;
        lastCameraPosition = cam.transform.position;
    }

    private void LateUpdate() // LateUpdate so Cinemachine has already moved the camera
    {
        Vector3 delta = cam.transform.position - lastCameraPosition;

        // Move background by only a fraction of how much the camera moved
        transform.position += new Vector3(
            delta.x * parallaxFactor,
            delta.y * parallaxFactor,
            0f
        );

        lastCameraPosition = cam.transform.position;
    }
}