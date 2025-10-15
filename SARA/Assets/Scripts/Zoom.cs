using UnityEngine;

public class Zoom : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 0.05f;
    public float minScale = 0.5f;
    public float maxScale = 3.0f;

    [Header("Activation Settings")]
    public bool requireObjectActive = true;
    public float activationDelay = 0.5f;

    [Header("Debug")]
    public bool enableDebug = true;

    private Vector3 initialScale;
    private float previousDistance = 0f;
    private bool isZoomEnabled = false;
    private float timeObjectBecameActive = 0f;

    void Start()
    {
        initialScale = transform.localScale;

        if (enableDebug)
        {
            Debug.Log($"[Zoom] Initialized on: {gameObject.name}");
        }
    }

    void OnEnable()
    {
        isZoomEnabled = false;
        timeObjectBecameActive = Time.time;
    }

    void Update()
    {
        // Check if zoom should be enabled
        if (requireObjectActive && !isZoomEnabled)
        {
            if (Time.time >= timeObjectBecameActive + activationDelay)
            {
                isZoomEnabled = true;
                if (enableDebug) Debug.Log("[Zoom] Enabled!");
            }
            else
            {
                return;
            }
        }

        // Only zoom with exactly 2 fingers
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            float currentDistance = Vector2.Distance(touch0.position, touch1.position);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                previousDistance = currentDistance;
                if (enableDebug) Debug.Log("[Zoom] Pinch started");
            }
            else if ((touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved) && previousDistance > 0)
            {
                // Calculate how much distance changed
                float deltaDistance = currentDistance - previousDistance;

                // Calculate new scale
                float scaleMultiplier = 1f + (deltaDistance * zoomSpeed * 0.01f);
                Vector3 newScale = transform.localScale * scaleMultiplier;

                // Clamp to min/max relative to initial scale
                float currentScaleFactor = newScale.x / initialScale.x;
                currentScaleFactor = Mathf.Clamp(currentScaleFactor, minScale, maxScale);

                transform.localScale = initialScale * currentScaleFactor;

                if (enableDebug)
                {
                    Debug.Log($"[Zoom] Delta: {deltaDistance:F1}, Scale: {currentScaleFactor:F2}x");
                }

                // Update for next frame
                previousDistance = currentDistance;
            }
        }
        else
        {
            // Reset when fingers lift
            previousDistance = 0f;
        }
    }

    public void ResetScale()
    {
        transform.localScale = initialScale;
        if (enableDebug) Debug.Log("[Zoom] Reset");
    }
}