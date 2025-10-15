using UnityEngine;

public class ResetRelay : MonoBehaviour
{
    public void ResetActive()
    {
        if (ActivePivotManager.Instance != null)
            ActivePivotManager.Instance.ResetActive();
    }
}