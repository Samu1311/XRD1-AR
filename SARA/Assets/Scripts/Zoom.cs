using UnityEngine;
using UnityEngine.InputSystem;
using TouchControl = UnityEngine.InputSystem.Controls.TouchControl;

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

    void Awake()
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
        int activeCount = 0;
        Vector2 p0 = Vector2.zero, p1 = Vector2.zero;
        TouchControl touch0 = null, touch1 = null;

        foreach (var t in touches)
        {
            if (t.press.isPressed)
            {
                if (activeCount == 0)
                {
                    p0 = t.position.ReadValue();
                    touch0 = t;
                }
                else if (activeCount == 1)
                {
                    p1 = t.position.ReadValue();
                    touch1 = t;
                }
                if (++activeCount >= 2) break;
            }
        }

        if (activeCount == 2)
        {
            float pinchDistance = Vector2.Distance(p0, p1);

            if (!isPinching)
            {
                // Check if this is the object we should grab
                if (activeObject == null)
                {
                    Vector2 midpoint = (p0 + p1) * 0.5f;
                    if (IsTouchingThis(midpoint))
                    {
                        activeObject = this;
                        lastPinchDistance = pinchDistance;
                        isPinching = true;
                    }
                }
            }
            else if (activeObject == this)
            {
                // Check if both touches are still active (moved phase)
                var phase0 = touch0.phase.ReadValue();
                var phase1 = touch1.phase.ReadValue();

                if (phase0 == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase1 == UnityEngine.InputSystem.TouchPhase.Moved ||
                    phase0 == UnityEngine.InputSystem.TouchPhase.Stationary ||
                    phase1 == UnityEngine.InputSystem.TouchPhase.Stationary)
                {
                    float pinchDelta = pinchDistance - lastPinchDistance;
                    lastPinchDistance = pinchDistance;

                    float newScale = targetScale * (1f + pinchDelta * pinchSpeed);
                    targetScale = Mathf.Clamp(newScale, minScale, maxScale);
                }

                // end pinching if either touch ended or was canceled
                if (phase0 == UnityEngine.InputSystem.TouchPhase.Ended ||
                    phase0 == UnityEngine.InputSystem.TouchPhase.Canceled ||
                    phase1 == UnityEngine.InputSystem.TouchPhase.Ended ||
                    phase1 == UnityEngine.InputSystem.TouchPhase.Canceled)
                {
                    if (activeObject == this)
                    {
                        activeObject = null;
                    }
                    isPinching = false;
                }
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