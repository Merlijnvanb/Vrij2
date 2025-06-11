using UnityEngine;

public class BillboardYAxis : MonoBehaviour
{
    [Tooltip("Optional camera to face. If left empty, defaults to Camera.main")]
    public Camera targetCamera;

    void LateUpdate()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        // Get direction to camera, ignoring Y-axis
        Vector3 direction = cam.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return; // Avoid zero direction

        // Rotate to face the camera horizontally
        transform.rotation = Quaternion.LookRotation(direction);
    }
}
