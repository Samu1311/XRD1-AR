using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(LineRenderer))]
public class AnnotationLabel : MonoBehaviour
{
    public SkullAnnotationAnchor target;
    public Camera cam;
    public LayerMask occlusionMask;
    public RectTransform panel;          // assign Background (Panel)
    public TextMeshProUGUI text;         // assign TMP text

    [Header("Tuning")]
    public float raycastSkin = 0.005f;
    public float smooth = 12f;
    public float lineStartOffset = 0.015f;

    // Put the label a bit to the right & up, relative to the camera
    public Vector2 screenOffset = new Vector2(0.22f, 0.08f);

    Canvas canvas;
    LineRenderer line;
    Vector3 desiredPos;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        line = GetComponent<LineRenderer>();
        if (!cam) cam = Camera.main;

        // World-space UI
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;

        // Sane defaults for size/scale
        var rt = (RectTransform)transform;
        if (rt.sizeDelta == Vector2.zero) rt.sizeDelta = new Vector2(220, 80);
        if (transform.localScale == Vector3.one) transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

        // Line defaults (FORCE visible even if prefab curve is wrong)
        line.positionCount = 2;
        line.useWorldSpace = true;
        line.widthMultiplier = 0.0035f;
        line.widthCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));  // <-- important

        if (text && target) text.text = target.title;
    }

    public void Initialize(SkullAnnotationAnchor t, Camera c, LayerMask mask)
    {
        target = t;
        cam = c ? c : Camera.main;
        occlusionMask = mask;
        if (canvas) canvas.worldCamera = cam;
        if (text && target) text.text = target.title;
    }

    void LateUpdate()
    {
        if (!target || !cam) { Show(false); return; }

        // Visibility (occlusion) check
        Vector3 toAnchor = target.transform.position - cam.transform.position;
        float dist = toAnchor.magnitude;
        Vector3 dir = toAnchor.normalized;

        Vector3 vp = cam.WorldToViewportPoint(target.transform.position);
        bool inFront = vp.z > 0f;
        bool inViewport = vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f;
        bool blocked = Physics.Raycast(cam.transform.position, dir, dist - raycastSkin, occlusionMask, QueryTriggerInteraction.Ignore);
        bool visible = inFront && inViewport && !blocked;

        Show(visible);
        if (!visible) return;

        // Push outward from the anchor toward the camera (fallback) or use provided hint
        Vector3 outward = (target.outwardHint.sqrMagnitude > 0.0001f
            ? target.outwardHint
            : (cam.transform.position - target.transform.position)).normalized;

        Vector3 anchorPos = target.transform.position;

        // Add a camera-relative side & up offset so the label sits beside the skull
        Vector3 camRight = cam.transform.right;
        Vector3 camUp = cam.transform.up;

        Vector3 labelPos = anchorPos
                         + outward * target.labelOutOffset   // off the surface
                         + camRight * screenOffset.x         // to the side
                         + camUp * screenOffset.y;        // a bit up

        // Smooth follow
        desiredPos = Vector3.Lerp(transform.position, labelPos, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        transform.position = desiredPos;

        // Billboard
        transform.LookAt(cam.transform);
        transform.Rotate(0f, 180f, 0f);

        // Leader line (from skull point to the left edge of the panel)
        Vector3 lineStart = anchorPos + outward * lineStartOffset;
        line.SetPosition(0, lineStart);
        Vector3 lineEnd = panel
            ? panel.TransformPoint(new Vector3(-panel.rect.width * 0.5f, 0f, 0f))
            : transform.position;
        line.SetPosition(1, lineEnd);

        if (text && text.text != target.title) text.text = target.title;
    }

    void Show(bool on)
    {
        if (canvas) canvas.enabled = on;
        if (line) line.enabled = on;
        if (panel) panel.gameObject.SetActive(on);
        if (text) text.enabled = on;
    }
}
