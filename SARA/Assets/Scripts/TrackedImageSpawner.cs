using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct ImagePrefab
    {
        public string imageName;   // exact name from the XR Reference Image Library
        public GameObject prefab;  // Skull_Content or Kidney_Content
        public float yOffset;      // extra lift if you didn't bake it into Pivot
    }

    public ImagePrefab[] mappings;

    private ARTrackedImageManager _manager;
    private readonly Dictionary<string, (GameObject prefab, float y)> _byName = new();
    private readonly Dictionary<TrackableId, GameObject> _spawned = new();

    void Awake()
    {
        _manager = GetComponent<ARTrackedImageManager>();
        foreach (var m in mappings)
            if (!string.IsNullOrEmpty(m.imageName) && m.prefab != null)
                _byName[m.imageName] = (m.prefab, m.yOffset);
    }

    void OnEnable() => _manager.trackedImagesChanged += OnChanged;
    void OnDisable() => _manager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs e)
    {
        foreach (var img in e.added) CreateOrUpdate(img);
        foreach (var img in e.updated) CreateOrUpdate(img);
        foreach (var img in e.removed)
        {
            if (_spawned.TryGetValue(img.trackableId, out var go) && go) Destroy(go);
            _spawned.Remove(img.trackableId);
        }
    }

    void CreateOrUpdate(ARTrackedImage img)
    {
        var name = img.referenceImage.name;
        if (!_byName.TryGetValue(name, out var map)) return;

        if (!_spawned.TryGetValue(img.trackableId, out var go) || !go)
        {
            go = Instantiate(map.prefab);
            _spawned[img.trackableId] = go;
        }

        // parent under the tracked image so it follows
        go.transform.SetParent(img.transform, false);
        go.transform.localPosition = new Vector3(0f, map.y, 0f);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one; // internal mesh keeps its own authored scale

        // visible only when tracking is solid
        bool visible = img.trackingState == TrackingState.Tracking;
        if (go.activeSelf != visible) go.SetActive(visible);
    }
}