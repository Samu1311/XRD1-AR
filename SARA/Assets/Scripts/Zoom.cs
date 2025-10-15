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
    private bool isThisObjectSelected = false;
    private float timeObjectBecameActive = 0f;
    private Camera arCamera;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void OnEnable()
    {
        isZoomEnabled = false;
        timeObjectBecameActive = Time.time;
    }

    void Update()
    {
        if (requireObjectActive && !isZoomEnabled)
        {
            if (Time.time >= timeObjectBecameActive + activationDelay)
            {
                isZoomEnabled = true;
            }

        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                Vector2 midpoint = (touch0.position + touch1.position) / 2f;
                isThisObjectSelected = IsTouchingThisObject(midpoint);

                if (isThisObjectSelected)
                {
                    previousDistance = Vector2.Distance(touch0.position, touch1.position);
                }
            }
            else if (isThisObjectSelected && (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved))
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);

                if (previousDistance > 0)
                {
                    float deltaDistance = currentDistance - previousDistance;
                    float scaleMultiplier = 1f + (deltaDistance * zoomSpeed * 0.01f);
                    Vector3 newScale = transform.localScale * scaleMultiplier;

                    float currentScaleFactor = newScale.x / initialScale.x;
                    currentScaleFactor = Mathf.Clamp(currentScaleFactor, minScale, maxScale);

                    transform.localScale = initialScale * currentScaleFactor;

                    previousDistance = currentDistance;
                }
            }
        }
        else
        {
            previousDistance = 0f;
            isThisObjectSelected = false;
        }
    }

    private bool IsTouchingThisObject(Vector2 touchPosition)
    {
        if (arCamera == null) return false;

        Ray ray = arCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }

        return false;
    }

    public void ResetScale()
    {
        transform.localScale = initialScale;
    }
}