using UnityEngine;
using UnityEngine.InputSystem;
using TouchControl = UnityEngine.InputSystem.Controls.TouchControl;

public class RotateAndZoomPivot : MonoBehaviour
{
    [Header("Assign the mesh to scale (your SkullModel child)")]
    public Transform targetModel;   // MUST be assigned

    [Header("Rotation (one-finger drag)")]
    public float yawSpeed = 0.15f;
    public float pitchSpeed = 0.10f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    [Header("Zoom (two-finger pinch)")]
    public float pinchSpeed = 0.0025f;
    public float minScale = 0.10f;     // relative to baseline
    public float maxScale = 2.00f;     // relative to baseline
    public float initialScale = 1.00f; // relative to baseline (1 = keep authored size)

    [Header("Smoothing")]
    public float smoothTime = 0.05f;

    [Header("Double-Tap")]
    public float doubleTapThreshold = 0.35f;
    public float doubleTapDistance = 60f;

    // Internal state
    private bool rotating, pinching, isTracking = true;
    private Vector2 lastFingerPos, lastTapPosition;
    private float lastPinchDist, lastTapTime = -1f;

    private float targetYaw, targetPitch;
    private float yawSmoothed, pitchSmoothed, yawVel, pitchVel;

    // --- scale handling relative to authored (baseline) scale ---
    private Vector3 baselineScale = Vector3.one; // captured from targetModel at Awake
    private float targetScaleFactor;             // scalar relative to baseline
    private float scaleSmoothed, scaleVel;

    void Awake()
    {
        if (targetModel == null)
        {
            Debug.LogError("[RotateAndZoom] Please assign targetModel (your SkullModel child).");
            enabled = false;
            return;
        }

        // capture authored size as baseline
        baselineScale = targetModel.localScale;

        // start at initial factor (1 = authored size)
        targetScaleFactor = Mathf.Clamp(initialScale, minScale, maxScale);
        scaleSmoothed = targetScaleFactor;

        ApplyTransformsImmediate();
    }

    void Update()
    {
        if (!isTracking || Touchscreen.current == null)
        {
            SmoothApply();
            return;
        }

        var touches = Touchscreen.current.touches;
        int activeCount = 0;
        Vector2 p0 = Vector2.zero, p1 = Vector2.zero;

        foreach (var t in touches)
        {
            if (t.press.isPressed)
            {
                if (activeCount == 0) p0 = t.position.ReadValue();
                else if (activeCount == 1) p1 = t.position.ReadValue();
                if (++activeCount >= 2) break;
            }
        }

        if (activeCount == 1)
        {
            pinching = false;

            TouchControl touch = touches[0];
            Vector2 pos = p0;

            var phase = touch.phase.ReadValue();
            if (phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // double-tap detect
                if (Time.time - lastTapTime <= doubleTapThreshold &&
                    Vector2.Distance(pos, lastTapPosition) <= doubleTapDistance)
                {
                    ResetToDefault();
                    lastTapTime = -1f;
                }
                else
                {
                    lastTapTime = Time.time;
                    lastTapPosition = pos;
                }

                rotating = true;
                lastFingerPos = pos;
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Moved)
            {
                if (rotating)
                {
                    Vector2 delta = pos - lastFingerPos;
                    lastFingerPos = pos;

                    targetYaw += delta.x * yawSpeed;
                    targetPitch -= delta.y * pitchSpeed;
                    targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
                }
            }
            else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                     phase == UnityEngine.InputSystem.TouchPhase.Canceled)
            {
                rotating = false;
            }
        }
        else if (activeCount == 2)
        {
            rotating = false;

            float pinchDist = Vector2.Distance(p0, p1);
            if (!pinching)
            {
                pinching = true;
                lastPinchDist = pinchDist;
            }
            else
            {
                float pinchDelta = pinchDist - lastPinchDist;
                lastPinchDist = pinchDist;

                float newFactor = targetScaleFactor * (1f + pinchDelta * pinchSpeed);
                targetScaleFactor = Mathf.Clamp(newFactor, minScale, maxScale);
            }
        }
        else
        {
            rotating = false;
            pinching = false;
        }

        SmoothApply();
    }

    private void SmoothApply()
    {
        // rotation (clean yaw/pitch)
        yawSmoothed = Mathf.SmoothDampAngle(yawSmoothed, targetYaw, ref yawVel, smoothTime);
        pitchSmoothed = Mathf.SmoothDampAngle(pitchSmoothed, targetPitch, ref pitchVel, smoothTime);
        transform.localRotation = Quaternion.Euler(pitchSmoothed, yawSmoothed, 0f);

        // scale relative to baseline
        scaleSmoothed = Mathf.SmoothDamp(scaleSmoothed, targetScaleFactor, ref scaleVel, smoothTime);
        targetModel.localScale = baselineScale * scaleSmoothed;
    }

    private void ApplyTransformsImmediate()
    {
        yawSmoothed = targetYaw;
        pitchSmoothed = targetPitch;
        transform.localRotation = Quaternion.Euler(pitchSmoothed, yawSmoothed, 0f);

        scaleSmoothed = targetScaleFactor;
        targetModel.localScale = baselineScale * scaleSmoothed;
    }

    private void ResetToDefault()
    {
        targetYaw = 0f;
        targetPitch = 0f;
        targetScaleFactor = Mathf.Clamp(initialScale, minScale, maxScale);
        // don’t change positions; Pivot’s Y offset stays the same
        Debug.Log("🌀 Double-tap → reset yaw/pitch and scale (relative to authored baseline).");
    }

    public void SetTrackingState(bool tracking)
    {
        isTracking = tracking;
        if (!tracking)
        {
            rotating = false;
            pinching = false;
        }
    }
}