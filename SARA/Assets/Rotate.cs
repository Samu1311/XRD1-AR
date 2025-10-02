using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Speeds")]
    public float rotationSpeedY = 3f;   // twisting left/right
    public float rotationSpeedX = 0.5f; // tilting up/down

    private float lastAngle;
    private Vector2 lastMidpoint;

    void Update()
    {
        // Need two fingers on screen
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Midpoint between fingers
            Vector2 midpoint = (t0.position + t1.position) * 0.5f;

            // Vector between fingers (for twist angle)
            Vector2 dir = t1.position - t0.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Reset references when gesture begins
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                lastAngle = angle;
                lastMidpoint = midpoint;
            }
            else
            {
                // --- Y rotation (twist) ---
                float deltaAngle = angle - lastAngle;
                transform.Rotate(Vector3.up, -deltaAngle * rotationSpeedY, Space.Self);

                // --- X rotation (vertical drag of both fingers) ---
                float deltaY = midpoint.y - lastMidpoint.y;
                transform.Rotate(Vector3.right, deltaY * 0.1f * rotationSpeedX, Space.Self);

                // Save values for next frame
                lastAngle = angle;
                lastMidpoint = midpoint;
            }
        }
    }
}
