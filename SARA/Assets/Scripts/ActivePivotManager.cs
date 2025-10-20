using UnityEngine;

public class ActivePivotManager : MonoBehaviour
{
    public static ActivePivotManager Instance { get; private set; }

    // The currently active pivot (can be RotateOnlyPivot or Zoom)
    private MonoBehaviour activePivot;

    [SerializeField] bool highlightActive = true;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetActive(MonoBehaviour controller)
    {
        if (activePivot == controller) return;

        // Disable highlight on previously active object
        if (highlightActive && activePivot != null)
            SetHighlight(activePivot, false);

        activePivot = controller;

        // Enable highlight on new active object
        if (highlightActive && activePivot != null)
            SetHighlight(activePivot, true);
    }

    public void ClearActive(MonoBehaviour controller)
    {
        if (activePivot == controller)
        {
            if (highlightActive && activePivot != null)
                SetHighlight(activePivot, false);
            activePivot = null;
        }
    }

    public void ResetActive()
    {
        if (activePivot == null) return;

        if (activePivot is RotateOnlyPivot rotatePivot)
            rotatePivot.ResetToDefaultPublic();
        else if (activePivot is Zoom zoomPivot)
            zoomPivot.ResetScale();
    }

    private void SetHighlight(MonoBehaviour controller, bool on)
    {
        if (controller is RotateOnlyPivot rotatePivot)
            rotatePivot.SetHighlight(on);
        else if (controller is Zoom zoomPivot)
        {
            // Optional: if you want Zoom objects to highlight visually,
            // you can handle it here (e.g. via a renderer on zoomPivot)
            var renderer = zoomPivot.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                var color = on ? Color.yellow : Color.white;
                renderer.material.color = color;
            }
        }
    }

    public MonoBehaviour Active => activePivot;
}
