using UnityEngine;

public class Rotate : MonoBehaviour
{
    // Twist and Rotate
    public GameObject targetObject;
    public float rotationSpeed = 3f;
    private float lastAngle;
    private Vector2 lastMidpoint;

    void Update()
    {
        if (targetObject == null) return; // Nothing to rotate

        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 midpoint = (t0.position + t1.position) * 0.5f;

            Vector2 dir = t1.position - t0.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                lastAngle = angle;
                lastMidpoint = midpoint;
            }
            else
            {
                // Y-axis rotation
                float deltaAngle = angle - lastAngle;
                targetObject.transform.Rotate(Vector3.up, -deltaAngle * rotationSpeed, Space.Self);

                // X-axis rotation
                float deltaY = midpoint.y - lastMidpoint.y;
                targetObject.transform.Rotate(Vector3.right, deltaY * 0.1f * rotationSpeed, Space.Self);

                lastAngle = angle;
                lastMidpoint = midpoint;
            }
        }
    }
}
