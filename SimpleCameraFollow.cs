using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    // The object the camera will follow (your HeroKnight)
    public Transform target;

    // How quickly the camera catches up to the target's position.
    // Smaller values are slower, larger values are faster.
    public float smoothSpeed = 0.125f;

    // An offset from the target's position (e.g., to keep the camera
    // a bit above and behind the player).
    public Vector3 offset;

    // We use LateUpdate because it runs after all other Update calls.
    // This ensures the player has finished moving for the frame before the camera updates.
    void LateUpdate()
    {
        // Check if a target has been assigned
        if (target == null)
        {
            Debug.LogWarning("Camera Follow: Target not assigned!");
            return;
        }

        // The desired position for the camera
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move from the camera's current position to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the new position to the camera. We keep the camera's original z-axis value.
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}