using UnityEngine;
using UnityEngine.InputSystem; 
using TouchControl = UnityEngine.InputSystem.Controls.TouchControl;

public class Rotate : MonoBehaviour
{
    [Header("Rotation Speeds")]
    public float rotationSpeedY = 3f;   // twisting left/right
    public float rotationSpeedX = 0.5f; // tilting up/down

    private float lastAngle;
    private Vector2 lastMidpoint;
    private bool initialized = false;

    void Update()
    {
        // Ensure touchscreen exists (device has a touchscreen)
        if (Touchscreen.current == null)
            return;

        var touches = Touchscreen.current.touches;
        var activeTouches = new System.Collections.Generic.List<TouchControl>();

        // Collect pressed touches
        foreach (var t in touches)
        {
            if (t.press.isPressed)
                activeTouches.Add(t);
        }

        if (activeTouches.Count < 2)
        {
            initialized = false;
            return;
        }

        // Read positions
        Vector2 pos0 = activeTouches[0].position.ReadValue();
        Vector2 pos1 = activeTouches[1].position.ReadValue();

        // Compute midpoint and twist angle
        Vector2 midpoint = (pos0 + pos1) * 0.5f;
        Vector2 dir = pos1 - pos0;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (!initialized)
        {
            lastAngle = angle;
            lastMidpoint = midpoint;
            initialized = true;
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
