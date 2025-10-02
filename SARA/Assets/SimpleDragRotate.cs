using UnityEngine;
using UnityEngine.UI;

public class SimpleDragRotate : MonoBehaviour
{
    [Header("Rotation speeds")]
    public float rotationSpeedY = 3f;    // twist -> yaw
    public float rotationSpeedX = 0.5f;  // midpoint vertical -> pitch

    [Header("Debug / Safety")]
    public bool createOnscreenDebug = true;
    public bool forceUnparentIfAttached = true; // unparent if the skull is being driven by another object

    // internal
    private float lastAngle;
    private Vector2 lastMidpoint;
    private Vector2 lastMousePos;
    private Text debugText;

    void Start()
    {
        // If this skull is parented to something that might override transform (AR Anchor, tracked image, etc.),
        // optionally unparent it so rotation is visible.
        if (forceUnparentIfAttached && transform.parent != null)
        {
            Debug.Log($"TouchRotateDebugger: unparenting {name} from {transform.parent.name}");
            transform.SetParent(null, true);
        }

        if (createOnscreenDebug) CreateDebugUI();
    }

    void CreateDebugUI()
    {
        // Try to find existing Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("TouchDebugCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create Text
        GameObject txtGO = new GameObject("TouchDebugText");
        txtGO.transform.SetParent(canvas.transform, false);
        debugText = txtGO.AddComponent<Text>();
        debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        debugText.fontSize = 18;
        debugText.alignment = TextAnchor.UpperLeft;
        debugText.horizontalOverflow = HorizontalWrapMode.Overflow;
        debugText.verticalOverflow = VerticalWrapMode.Overflow;
        RectTransform rt = debugText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0f, 1f);
        rt.anchoredPosition = new Vector2(8f, -8f);
        rt.sizeDelta = new Vector2(600f, 200f);
    }

    void Update()
    {
        // Basic sanity: don't do anything if timescale is 0
        if (Time.timeScale == 0f) return;

        // Editor convenience: allow mouse drag to test rotation in Editor
        if (Application.isEditor)
        {
            HandleMouseRotation();
            UpdateDebugText($"Editor mode - Mouse supported. TouchCount: {Input.touchCount}");
            return;
        }

        // On-device touch handling
        int tc = Input.touchCount;
        if (tc == 0)
        {
            UpdateDebugText("No touches");
            return;
        }

        // Log each touch (for on-screen debug)
        string touchInfo = $"TouchCount: {tc}\n";
        for (int i = 0; i < tc; i++)
        {
            Touch t = Input.GetTouch(i);
            touchInfo += $"[{i}] pos:{t.position} phase:{t.phase}\n";
        }

        // If single finger -> basic drag rotate (useful to verify touch works)
        if (tc == 1)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                // Yaw from horizontal delta, pitch from vertical delta
                float rotY = -t.deltaPosition.x * 0.01f * rotationSpeedY;
                float rotX = t.deltaPosition.y * 0.01f * rotationSpeedX;

                transform.Rotate(Vector3.right, rotX, Space.Self);
                transform.Rotate(Vector3.up, rotY, Space.Self);

                Debug.Log($"TouchRotateDebugger: 1-finger moved. rotX:{rotX} rotY:{rotY}");
            }
            UpdateDebugText(touchInfo + "\nMode: 1-finger drag rotate");
            return;
        }

        // Two fingers -> twist + midpoint vertical drag
        if (tc >= 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 midpoint = (t0.position + t1.position) * 0.5f;
            Vector2 dir = t1.position - t0.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // When gesture begins, initialize reference values
            if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began)
            {
                lastAngle = angle;
                lastMidpoint = midpoint;
                Debug.Log("TouchRotateDebugger: two-finger gesture began");
                UpdateDebugText(touchInfo + "\nMode: 2-finger started");
                return;
            }

            // Compute deltas
            float deltaAngle = angle - lastAngle;      // twist (degrees)
            float deltaY = midpoint.y - lastMidpoint.y; // midpoint vertical movement (pixels)

            // Apply rotations (local axes)
            float yaw = -deltaAngle * rotationSpeedY;
            float pitch = deltaY * 0.01f * rotationSpeedX;

            transform.Rotate(Vector3.up, yaw, Space.Self);      // Yaw (left/right)
            transform.Rotate(Vector3.right, pitch, Space.Self); // Pitch (tilt up/down)

            Debug.Log($"TouchRotateDebugger: two-finger deltaAngle:{deltaAngle} yaw:{yaw} deltaY:{deltaY} pitch:{pitch}");

            // Save for next frame
            lastAngle = angle;
            lastMidpoint = midpoint;

            UpdateDebugText(touchInfo + $"\nMode: 2-finger twist\nΔangle:{deltaAngle:F2} yaw:{yaw:F2}\nΔmidY:{deltaY:F1} pitch:{pitch:F2}");
            return;
        }
    }

    private void HandleMouseRotation()
    {
        // Editor mouse test: left button drag rotates
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 mp = Input.mousePosition;
            Vector2 delta = mp - lastMousePos;
            float rotY = -delta.x * 0.2f;
            float rotX = delta.y * 0.2f;
            transform.Rotate(Vector3.right, rotX, Space.Self);
            transform.Rotate(Vector3.up, rotY, Space.Self);
            lastMousePos = mp;
        }
    }

    private void UpdateDebugText(string s)
    {
        if (debugText != null) debugText.text = s;
    }
}
