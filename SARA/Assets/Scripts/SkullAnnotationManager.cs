using UnityEngine;

public class SkullAnnotationManager : MonoBehaviour
{
    public Transform skullRoot;
    public Camera cam;
    public AnnotationLabel labelPrefab;
    public LayerMask occlusionMask; // set to only the "Skull" layer

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!skullRoot || !labelPrefab)
        {
            Debug.LogError("Assign skullRoot and labelPrefab.");
            return;
        }

        var anchors = skullRoot.GetComponentsInChildren<SkullAnnotationAnchor>(true);
        // Estimate skull center once for outward hints
        var rend = skullRoot.GetComponentInChildren<Renderer>();
        Vector3 center = rend ? rend.bounds.center : skullRoot.position;

        foreach (var a in anchors)
        {
            if (a.outwardHint == Vector3.zero)
                a.outwardHint = (a.transform.position - center).normalized;

            var label = Instantiate(labelPrefab, a.transform.position, Quaternion.identity, transform);
            label.cam = cam;
            label.target = a;
            label.occlusionMask = occlusionMask;
        }
    }
}
