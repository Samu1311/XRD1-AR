using UnityEngine;

public class SkullAnnotationManager : MonoBehaviour
{
    public Transform skullRoot;
    public Camera cam;
    public AnnotationLabel labelPrefab;
    public AnchorHotspot hotspotPrefab;
    public LayerMask occlusionMask; // only the "Skull" layer

    void Start()
    {
        if (!cam) cam = Camera.main;
        Rebuild(skullRoot);
    }

    public void Rebuild(Transform newSkullRoot)
    {
        if (newSkullRoot) skullRoot = newSkullRoot;
        if (!skullRoot || !labelPrefab || !hotspotPrefab) { Debug.LogError("Assign skullRoot, labelPrefab, hotspotPrefab."); return; }

        // clear old
        for (int i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        // bounds & scale
        var rs = skullRoot.GetComponentsInChildren<Renderer>(true);
        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        float diag = Mathf.Max(0.0001f, b.size.magnitude);

        float outOff = 0.08f * diag;
        float sideX = 0.20f * diag;
        float sideY = 0.08f * diag;
        float lineW = 0.0040f * diag;
        float lineOff = 0.015f * diag;
        float hotspotSize = 0.03f * diag;

        var anchors = skullRoot.GetComponentsInChildren<SkullAnnotationAnchor>(true);
        foreach (var a in anchors)
        {
            if (a.outwardHint == Vector3.zero) a.outwardHint = (a.transform.position - b.center).normalized;
            if (a.labelOutOffset < 0.5f * outOff) a.labelOutOffset = outOff;

            // create label (hidden)
            var label = Instantiate(labelPrefab, a.transform.position, Quaternion.identity, transform);
            label.Initialize(a, cam, occlusionMask);
            label.screenOffset = new Vector2(sideX, sideY);
            label.lineStartOffset = lineOff;

            var lr = label.GetComponent<LineRenderer>();
            if (lr) { lr.widthMultiplier = lineW; lr.widthCurve = AnimationCurve.Linear(0, 1, 1, 1); }

            // create hotspot
            var hs = Instantiate(hotspotPrefab, a.transform.position, Quaternion.identity, transform);
            hs.visual = hs.transform.GetChild(0);                // assumes child "Visual"
            hs.Initialize(a, label, cam, occlusionMask, hotspotSize);
        }
    }
}
