using UnityEngine;
using UnityEngine.EventSystems;

public class AnchorHotspot : MonoBehaviour, IPointerClickHandler
{
    public SkullAnnotationAnchor anchor;
    public AnnotationLabel label;
    public Camera cam;
    public LayerMask occlusionMask;
    public Transform visual;            // assign the child "Visual"

    [Header("Visuals")]
    [Tooltip("Seconds per full pulse cycle (higher = slower).")]
    public float pulsePeriod = 2.0f;    // 2s = nice slow pulse
    [Tooltip("How much the visual scales during the pulse (0..1).")]
    public float pulseAmount = 0.25f;
    [Tooltip("Absolute world size for the hotspot (set by manager).")]
    public float baseSize = 1f;

    [Header("Placement")]
    [Tooltip("How far to push the hotspot away from the skull surface, as a fraction of baseSize.")]
    public float surfaceOffset = 0.1f;

    [Header("Visibility (anti-blink)")]
    [Tooltip("Extra distance we keep from the ray target to avoid self/edge hits (scaled by baseSize).")]
    public float raycastSkin = 0.02f;
    [Tooltip("Minimum time (seconds) occlusion state must remain the same before toggling.")]
    public float visibilityHysteresis = 0.15f;

    Vector3 _baseScale;

    // Debounce state
    bool _desiredVisible = true;
    bool _appliedVisible = true;
    float _stateChangedAt = 0f;

    public void Initialize(SkullAnnotationAnchor a, AnnotationLabel l, Camera c, LayerMask mask, float sizeWorld)
    {
        anchor = a; label = l; cam = c; occlusionMask = mask;
        baseSize = sizeWorld;
        _baseScale = Vector3.one * baseSize;
        transform.localScale = _baseScale;

        // Start at the correct offset right away
        UpdateWorldPosition();

        // Initialize visibility state
        _desiredVisible = _appliedVisible = true;
        _stateChangedAt = Time.time;
        SetActive(_appliedVisible);
    }

    void Update()
    {
        if (!cam || !anchor) return;

        // Keep the hotspot offset from the skull every frame
        UpdateWorldPosition();

        // Billboard + pulse
        // Orient hotspot to lie on skull surface instead of facing camera
        if (visual)
        {
            // Determine outward direction (same as anchor normal)
            Vector3 outward;
            if (anchor.outwardHint != Vector3.zero)
                outward = anchor.outwardHint.normalized;
            else
                outward = (anchor.transform.position - cam.transform.position).normalized;

            // Set the hotspot's forward to match skull surface
            visual.rotation = Quaternion.LookRotation(outward, Vector3.up);

            // Optional: make it slightly tilt toward camera (adds visibility)
            visual.rotation = Quaternion.Slerp(
                Quaternion.LookRotation(outward, Vector3.up),
                Quaternion.LookRotation((cam.transform.position - transform.position).normalized, Vector3.up),
                0.15f); // 0 = full flat, 1 = full billboard

            // Keep the pulsing animation
            float t = Mathf.Sin((Mathf.PI * 2f) * (Time.time / Mathf.Max(0.01f, pulsePeriod)));
            t = (t * 0.5f) + 0.5f; // 0..1
            float s = 1f + Mathf.Lerp(0f, pulseAmount, t);
            visual.localScale = Vector3.one * s;
        }


        // --- Debounced visibility (prevents fast blinking from occlusion jitter) ---
        bool nowDesired = !label.opened && IsVisibleToCamera();
        if (nowDesired != _desiredVisible)
        {
            _desiredVisible = nowDesired;
            _stateChangedAt = Time.time; // start debounce window
        }

        if (_appliedVisible != _desiredVisible && (Time.time - _stateChangedAt) >= visibilityHysteresis)
        {
            _appliedVisible = _desiredVisible;
            SetActive(_appliedVisible);
        }
    }

    void UpdateWorldPosition()
    {
        Vector3 outward;

        // Prefer the anchor's outward hint; otherwise, use camera-to-anchor
        if (anchor.outwardHint != Vector3.zero)
            outward = anchor.outwardHint.normalized;
        else
            outward = (anchor.transform.position - cam.transform.position).normalized;

        float offsetWorld = surfaceOffset * baseSize;
        transform.position = anchor.transform.position + outward * offsetWorld;
    }

    bool IsVisibleToCamera()
    {
        Vector3 to = anchor.transform.position - cam.transform.position;
        float d = to.magnitude;
        if (d <= 0.0001f) return false;
        Vector3 dir = to / d;

        // Screen test
        Vector3 vp = cam.WorldToViewportPoint(anchor.transform.position);
        bool inFront = vp.z > 0;
        bool inScreen = vp.x > 0 && vp.x < 1 && vp.y > 0 && vp.y < 1;

        // Raycast with a skin so we don't hit edges/self and flicker
        float stopShort = Mathf.Max(0f, d - (raycastSkin * baseSize));
        bool blocked = Physics.Raycast(cam.transform.position, dir, stopShort, occlusionMask, QueryTriggerInteraction.Ignore);

        return inFront && inScreen && !blocked;
    }

    void SetActive(bool on)
    {
        if (visual && visual.gameObject.activeSelf != on) visual.gameObject.SetActive(on);
        var col = GetComponent<Collider>();
        if (col && col.enabled != on) col.enabled = on;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!label || !anchor) return;

        label.Open(anchor);   // <— IMPORTANT: pass which part was clicked
        SetActive(false);
        _appliedVisible = false;
        _desiredVisible = false;
        _stateChangedAt = Time.time;
    }
}
