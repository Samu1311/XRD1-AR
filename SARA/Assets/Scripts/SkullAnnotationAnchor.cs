using UnityEngine;

public class SkullAnnotationAnchor : MonoBehaviour
{
    [Tooltip("Text to show on the label")]
    public string title = "Sagittal suture";

    [Tooltip("Meters to push the label outward from the skull surface")]
    public float labelOutOffset = 0.03f;

    [Tooltip("Optional manual normal override; if zero, manager estimates from skull center")]
    public Vector3 outwardHint = Vector3.zero;

    // Editor gizmo so we can see anchors
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.003f);
    }
}