using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceAndMove : MonoBehaviour
{
    public GameObject modelPrefab;
    private GameObject spawnedObject;

    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool isPlaced = false;
    private bool isDragging = false;

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        // Case 1: To place a new object or drag immediately after placing
        if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose pose = hits[0].pose;

            if (spawnedObject == null && touch.phase == TouchPhase.Began)
            {
                spawnedObject = Instantiate(modelPrefab, pose.position, pose.rotation);
                isPlaced = true;
                isDragging = true;
            }
            else if (isDragging && spawnedObject != null && touch.phase == TouchPhase.Moved)
            {
                spawnedObject.transform.position = pose.position;
            }
            else if (isDragging && spawnedObject != null && touch.phase == TouchPhase.Ended)
            {
                isDragging = false;
            }
        }

        // Case 2: To tap on the object later to re-enable dragging
        if (touch.phase == TouchPhase.Began && spawnedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hitObject;

            if (Physics.Raycast(ray, out hitObject))
            {
                if (hitObject.collider.gameObject == spawnedObject)
                {
                    isDragging = true;
                }
            }
        }
    }
}
