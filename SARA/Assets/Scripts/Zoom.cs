using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float zoomSpeed = 0.01f;
    public float minScale = 0.2f;
    public float maxScale = 3.0f;

    private GameObject targetObject;

    void Start()
    {
        targetObject = gameObject;
    }

    void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Get the positions in this frame and previous frame
            Vector2 touchZeroPrev = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrev = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrev - touchOnePrev).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            // Apply scaling
            Vector3 newScale = targetObject.transform.localScale + Vector3.one * (difference * zoomSpeed);

            // Clamp scale
            newScale.x = Mathf.Clamp(newScale.x, minScale, maxScale);
            newScale.y = Mathf.Clamp(newScale.y, minScale, maxScale);
            newScale.z = Mathf.Clamp(newScale.z, minScale, maxScale);

            targetObject.transform.localScale = newScale;
        }
    }
}
