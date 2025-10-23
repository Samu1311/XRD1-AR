using TMPro;
using UnityEngine;
using UnityEngine.UI;   // for LayoutRebuilder

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(LineRenderer))]
public class AnnotationLabel : MonoBehaviour
{
    public SkullAnnotationAnchor target;
    public Camera cam;
    public LayerMask occlusionMask;

    // Assign in prefab:
    public RectTransform panel;      // Background (Image) RectTransform
    public TextMeshProUGUI text;     // TMP text inside Background

    [Header("Tuning")]
    public float raycastSkin = 0.005f;
    public float smooth = 12f;
    public float lineStartOffset = 0.015f;
    public Vector2 screenOffset = new Vector2(0.2f, 0.08f);

    [HideInInspector] public bool opened = false;
    [HideInInspector] public bool expanded = false;

    Canvas canvas;
    LineRenderer line;
    Vector3 desiredPos;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        line = GetComponent<LineRenderer>();
        if (!cam) cam = Camera.main;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;

        line.positionCount = 2;
        line.useWorldSpace = true;
        if (line.widthMultiplier <= 0f) line.widthMultiplier = 0.0035f;
        line.widthCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));

        // TMP 3.x wrapping/overflow defaults
        if (text)
        {
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow; // or Truncate if you prefer
        }

        SetVisible(false);
    }

    public void Initialize(SkullAnnotationAnchor t, Camera c, LayerMask mask)
    {
        target = t;
        cam = c ? c : Camera.main;
        occlusionMask = mask;
        canvas.worldCamera = cam;
        SetVisible(false);
    }

    // -------- Public open/close & click --------
    public void Open(SkullAnnotationAnchor t)
    {
        // Close other labels so only one stays open
        foreach (var l in FindObjectsOfType<AnnotationLabel>())
            if (l != this) l.Close();

        target = t;
        opened = true;
        expanded = false; // start collapsed (title only)
        ApplyTitleOnly();
    }

    public void Open() => Open(target);

    public void OnLabelClick()
    {
        if (!opened || !target || !text) return;

        expanded = !expanded;
        if (expanded) ApplyTitleAndDescription();
        else ApplyTitleOnly();
    }

    public void Close()
    {
        opened = false;
        expanded = false;
        SetVisible(false);
    }

    // -------- Text & layout helpers --------
    void ApplyTitleOnly()
    {
        if (!text || !target) return;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.text = $"<b>{target.title}</b>";
        RefreshLayout();
    }

    void ApplyTitleAndDescription()
    {
        if (!text || !target) return;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.overflowMode = TextOverflowModes.Overflow;
        text.text = $"<b>{target.title}</b>\n\n{target.description}";
        RefreshLayout();
    }

    void RefreshLayout()
    {
        if (!panel) return;

        // Immediate rebuild (World Space canvases often need this)
        LayoutRebuilder.ForceRebuildLayoutImmediate(panel);

        // And once next frame to catch late layout changes
        StopAllCoroutines();
        StartCoroutine(DelayedRebuild());
    }

    System.Collections.IEnumerator DelayedRebuild()
    {
        yield return null; // next frame
        if (panel) LayoutRebuilder.ForceRebuildLayoutImmediate(panel);
    }

    // -------- Update positioning/visibility --------
    void LateUpdate()
    {
        if (!target || !cam) { SetVisible(false); return; }
        if (!opened) { SetVisible(false); return; }

        // Occlusion & on-screen
        Vector3 toAnchor = target.transform.position - cam.transform.position;
        float dist = toAnchor.magnitude;
        Vector3 dir = toAnchor.normalized;

        Vector3 vp = cam.WorldToViewportPoint(target.transform.position);
        bool inFront = vp.z > 0f;
        bool inViewport = vp.x > 0f && vp.x < 1f && vp.y > 0f && vp.y < 1f;
        bool blocked = Physics.Raycast(cam.transform.position, dir, dist - raycastSkin, occlusionMask, QueryTriggerInteraction.Ignore);

        bool visible = inFront && inViewport && !blocked;
        SetVisible(visible);
        if (!visible) return;

        // Position label
        Vector3 outward = (target.outwardHint.sqrMagnitude > 0.0001f
            ? target.outwardHint
            : (cam.transform.position - target.transform.position)).normalized;

        Vector3 anchorPos = target.transform.position;
        Vector3 labelPos = anchorPos + outward * target.labelOutOffset
                         + cam.transform.right * screenOffset.x
                         + cam.transform.up * screenOffset.y;

        desiredPos = Vector3.Lerp(transform.position, labelPos, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        transform.position = desiredPos;

        transform.LookAt(cam.transform);
        transform.Rotate(0f, 180f, 0f);

        Vector3 lineStart = anchorPos + outward * lineStartOffset;
        line.SetPosition(0, lineStart);

        Vector3 lineEnd = panel
            ? panel.TransformPoint(new Vector3(-panel.rect.width * 0.5f, 0f, 0f))
            : transform.position;

        line.SetPosition(1, lineEnd);
    }

    void SetVisible(bool on)
    {
        if (canvas) canvas.enabled = on;
        if (line) line.enabled = on;
        if (panel) panel.gameObject.SetActive(on);
        if (text) text.enabled = on;
    }
}
