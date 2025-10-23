using UnityEngine;
using UnityEngine.EventSystems;

public class LabelCloser : MonoBehaviour, IPointerClickHandler
{
    public AnnotationLabel label;
    public AnchorHotspot hotspot; // assign the one that opened this label

    public void OnPointerClick(PointerEventData eventData)
    {
        if (label) label.Close();
        if (hotspot) hotspot.gameObject.SetActive(true);
    }
}

