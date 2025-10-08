using UnityEngine;
using UnityEngine.InputSystem;

public class RotateNewInput : MonoBehaviour
{
    [Header("Rotation Speeds")]
    public float rotationSpeedY = 3f;   // twisting left/right
    public float rotationSpeedX = 0.5f; // tilting up/down

    private float lastAngle;
    private Vector2 lastMidpoint;

    void Update()
    {
        if (Touchscreen.current == null) return;
        var touches = Touchscreen.current.touches;

        // We need exactly two active touches
        int activeCount = 0;
        TouchControl t0 = null;
        TouchControl t1 = null;

        foreach (var t in touches)
        {
            if (t.press.isPressed)
            {
                if (activeCount == 0) t0 = t;
                else if (activeCount == 1) t1 = t;
                activeCount++;
            }
        }

        if (activeCount < 2)
            return;

        Vector2 pos0 = t0.position.ReadValue();
        Vector2 pos1 = t1.position.ReadValue();

        Vector2 midpoint = (pos0 + pos1) * 0.5f;
        Vector2 dir = pos1 - pos0;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Use pressure start as 'began'
        if (t0.delta.ReadValue() == Vector2.zero || t1.delta.ReadValue() == Vector2.zero)
        {
            lastAngle = angle;
            lastMidpoint = midpoint;
            return;
        }

        // --- Y rotation (twist) ---
        float deltaAngle = angle - lastAngle;
        transform.Rotate(Vector3.up, -deltaAngle * rotationSpeedY, Space.Self);

        // --- X rotation (vertical drag of both fingers) ---
        float deltaY = midpoint.y - lastMidpoint.y;
        transform.Rotate(Vector3.right, deltaY * 0.1f * rotationSpeedX, Space.Self);

        // Save for next frame
        lastAngle = angle;
        lastMidpoint = midpoint;
    }
}
