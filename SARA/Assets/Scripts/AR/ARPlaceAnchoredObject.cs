using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPlaceAnchoredObject : MonoBehaviour
{
    [Header("Prefab to place")]
    [SerializeField] private GameObject placeablePrefab;

    ARRaycastManager raycaster;
    ARPlaneManager planeManager;
    ARAnchorManager anchorManager;

    ARAnchor currentAnchor;
    GameObject spawned;
    static readonly List<ARRaycastHit> hits = new();

    void Awake()
    {
        raycaster = FindObjectOfType<ARRaycastManager>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        anchorManager = FindObjectOfType<ARAnchorManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0) return;
        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (!raycaster.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon)) return;

        var hit = hits[0];
        Pose pose = hit.pose;

        // 1) Try to attach to the hit plane (best stability)
        ARAnchor newAnchor = null;
        ARPlane plane = planeManager != null && planeManager.trackables.TryGetTrackable(hit.trackableId, out var p) ? p : null;
        if (plane != null && anchorManager != null)
        {
            newAnchor = anchorManager.AttachAnchor(plane, pose);
        }

        // 2) Fallback: create a world anchor GameObject and add ARAnchor
        if (newAnchor == null)
        {
            var anchorGO = new GameObject("WorldAnchor");
            anchorGO.transform.SetPositionAndRotation(pose.position, pose.rotation);
            newAnchor = anchorGO.AddComponent<ARAnchor>(); // works across ARF versions
        }

        if (newAnchor == null)
        {
            Debug.LogWarning("Failed to create/attach anchor.");
            return;
        }

        // Keep only one skull at a time
        if (currentAnchor != null)
        {
            if (spawned != null) Destroy(spawned);
            Destroy(currentAnchor.gameObject);
        }

        currentAnchor = newAnchor;

        spawned = Instantiate(placeablePrefab, pose.position, pose.rotation, currentAnchor.transform);
    }

    public void SetPrefab(GameObject prefab) => placeablePrefab = prefab;
}