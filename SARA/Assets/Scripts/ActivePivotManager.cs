using UnityEngine;

public class ActivePivotManager : MonoBehaviour
{
    public static ActivePivotManager Instance { get; private set; }

    // The currently active pivot controller (only this one responds to gestures)
    public RotateAndZoomPivot Active { get; private set; }

    // Optional: visual feedback on selection
    [SerializeField] bool highlightActive = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetActive(RotateAndZoomPivot controller)
    {
        if (Active == controller) return;

        // turn off highlight on previous
        if (highlightActive && Active != null) Active.SetHighlight(false);

        Active = controller;

        // turn on highlight on new
        if (highlightActive && Active != null) Active.SetHighlight(true);
    }

    public void ClearActive(RotateAndZoomPivot controller)
    {
        if (Active == controller)
        {
            if (highlightActive && Active != null) Active.SetHighlight(false);
            Active = null;
        }
    }

    // Hook this to your UI Reset button
    public void ResetActive()
    {
        if (Active != null) Active.ResetToDefaultPublic();
    }
}