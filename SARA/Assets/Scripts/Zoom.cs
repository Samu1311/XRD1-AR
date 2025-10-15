using UnityEngine;
using UnityEngine.InputSystem;

public class Zoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float pinchSpeed = 0.003f;
    public float minScale = 0.5f;
    public float maxScale = 3.0f;
    public float smoothTime = 0.08f;

    [Header("Debug")]
    public bool enableDebug = true;

    private Vector3 originalScale;
    private float currentScale = 1f;
    private float targetScale = 1f;
    private float scaleVelocity = 0f;

    private bool isPinching = false;
    private float lastPinchDistance = 0f;
    private Camera cam;

    private static Zoom activeObject = null;

    void Start()
    {
        originalScale = transform.localScale;
        cam = Camera.main;
    }

    void Update()
    {
        if (Touchscreen.current == null)
        {
            ZoomSmooth();
            return;
        }

        var touches = Touchscreen.current.touches;
        int count = 0;
        Vector2 pos1 = Vector2.zero, pos2 = Vector2.zero;

        foreach (var t in touches)
        {
            if (t.press.isPressed)
            {
                if (count == 0) pos1 = t.position.ReadValue();
                else if (count == 1) pos2 = t.position.ReadValue();
                count++;
                if (count >= 2) break;
            }
        }

        if (count == 2)
        {
            float dist = Vector2.Distance(pos1, pos2);

            if (!isPinching)
            {
                // check if this is the object we should grab
                if (activeObject == null)
                {
                    Vector2 midpoint = (pos1 + pos2) * 0.5f;
                    if (IsTouchingThis(midpoint))
                    {
                        activeObject = this;
                        lastPinchDistance = dist;
                        isPinching = true;
                    }
                }
            }
            else if (activeObject == this)
            {
                float change = dist - lastPinchDistance;
                lastPinchDistance = dist;

                targetScale *= 1f + (change * pinchSpeed);
                targetScale = Mathf.Clamp(targetScale, minScale, maxScale);
            }
        }
        else
        {
            if (activeObject == this)
            {
                activeObject = null;
            }
            isPinching = false;
        }

        ZoomSmooth();
    }

    bool IsTouchingThis(Vector2 screenPos)
    {
        if (cam == null) return false;

        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }

        return false;
    }

    // make it smooooooth 
    void ZoomSmooth()
    {
        currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, smoothTime);
        transform.localScale = originalScale * currentScale;
    }

    public void ResetScale()
    {
        targetScale = 1f;
        currentScale = 1f;
        transform.localScale = originalScale;
    }
}