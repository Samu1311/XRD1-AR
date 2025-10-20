using UnityEngine;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
using TouchControl = UnityEngine.InputSystem.Controls.TouchControl;
#endif

public class RotateOnlyPivot : MonoBehaviour
{
    [Header("Assign the mesh to rotate (your *Model child)")]
    public Transform targetModel;   // MUST be assigned (SkullModel/KidneyModel)

    [Header("Rotation (one-finger drag)")]
    public float yawSpeed = 0.15f;    // deg per px horizontal
    public float pitchSpeed = 0.10f;  // deg per px vertical
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Smoothing")]
    public float smoothTime = 0.05f;

    [Header("Optional highlight")]
    public Renderer[] highlightRenderers;    // assign mesh renderers to tint
    public Color highlightColor = new Color(1f, 0.85f, 0.2f, 1f);
    private Color[] _origEmissionColors;

    // State
    private bool rotating;
    private bool isTracking = true;
    private Vector2 lastFingerPos;

    private float targetYaw, targetPitch;
    private float yawSmoothed, pitchSmoothed, yawVel, pitchVel;

    private Camera _cam;

    void Awake()
    {
        if (targetModel == null)
        {
            Debug.LogError("[RotateOnlyPivot] Please assign targetModel (your mesh child).");
            enabled = false;
            return;
        }

        _cam = Camera.main;

        if (highlightRenderers != null && highlightRenderers.Length > 0)
        {
            _origEmissionColors = new Color[highlightRenderers.Length];
            for (int i = 0; i < highlightRenderers.Length; i++)
            {
                var m = highlightRenderers[i].material;
                _origEmissionColors[i] = m.HasProperty("_EmissionColor")
                    ? m.GetColor("_EmissionColor")
                    : Color.black;
            }
        }

        ApplyTransformsImmediate();
    }

    void Update()
    {
        if (!isTracking)
        {
            SmoothApply();
            return;
        }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Touchscreen.current == null) { SmoothApply(); return; }

        var touches = Touchscreen.current.touches;
        int count = 0;
        Vector2 p0 = Vector2.zero;
        TouchControl firstTouch = null;

        foreach (var t in touches)
        {
            if (t.press.isPressed)
            {
                if (count == 0) { p0 = t.position.ReadValue(); firstTouch = t; }
                count++;
                if (count >= 1) break;
            }
        }

        if (count == 1 && firstTouch != null &&
            firstTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
        {
            if (IsTouchOnThisModel(p0))
            {
                ActivePivotManager.Instance?.SetActive(this);
                rotating = true;
                lastFingerPos = p0;
            }
            else
            {
                rotating = false;
            }
        }
#else
        int count = Input.touchCount;
        Vector2 p0 = Vector2.zero;
        Touch t0 = default;

        if (count >= 1)
        {
            t0 = Input.GetTouch(0);
            p0 = t0.position;
        }

        if (count == 1 && t0.phase == TouchPhase.Began)
        {
            if (IsTouchOnThisModel(p0))
            {
                ActivePivotManager.Instance?.SetActive(this);
                rotating = true;
                lastFingerPos = p0;
            }
            else
            {
                rotating = false;
            }
        }
#endif

        if (ActivePivotManager.Instance == null || ActivePivotManager.Instance.Active != this)
        {
            SmoothApply();
            return;
        }

        // --- Rotation only ---
        if (count == 1)
        {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var phase = firstTouch.phase.ReadValue();
            if (phase == UnityEngine.InputSystem.TouchPhase.Moved && rotating)
#else
            if (t0.phase == TouchPhase.Moved && rotating)
#endif
            {
                Vector2 delta = p0 - lastFingerPos;
                lastFingerPos = p0;

                targetYaw += delta.x * yawSpeed;
                targetPitch -= delta.y * pitchSpeed;
                targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
            }
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     phase == UnityEngine.InputSystem.TouchPhase.Canceled)
#else
            else if (t0.phase == TouchPhase.Ended || t0.phase == TouchPhase.Canceled)
#endif
            {
                rotating = false;
            }
        }
        else
        {
            rotating = false;
        }

        SmoothApply();
    }

    private void SmoothApply()
    {
        yawSmoothed = Mathf.SmoothDampAngle(yawSmoothed, targetYaw, ref yawVel, smoothTime);
        pitchSmoothed = Mathf.SmoothDampAngle(pitchSmoothed, targetPitch, ref pitchVel, smoothTime);
        transform.localRotation = Quaternion.Euler(pitchSmoothed, yawSmoothed, 0f);
    }

    private void ApplyTransformsImmediate()
    {
        yawSmoothed = targetYaw;
        pitchSmoothed = targetPitch;
        transform.localRotation = Quaternion.Euler(pitchSmoothed, yawSmoothed, 0f);
    }

    bool IsTouchOnThisModel(Vector2 screenPos)
    {
        if (_cam == null || targetModel == null) return false;
        Ray ray = _cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 20f))
            return hit.collider && hit.collider.transform.IsChildOf(targetModel);
        return false;
    }

    public void ResetToDefaultPublic()
    {
        targetYaw = 0f;
        targetPitch = 0f;
    }

    public void SetHighlight(bool on)
    {
        if (highlightRenderers == null) return;
        for (int i = 0; i < highlightRenderers.Length; i++)
        {
            var m = highlightRenderers[i].material;
            if (!m.HasProperty("_EmissionColor")) continue;
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", on ? highlightColor : _origEmissionColors[i]);
        }
    }

    public void SetTrackingState(bool tracking)
    {
        isTracking = tracking;
        if (!tracking)
        {
            rotating = false;
            ActivePivotManager.Instance?.ClearActive(this);
        }
    }
}
