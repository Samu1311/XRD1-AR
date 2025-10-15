using UnityEngine;
using UnityEngine.EventSystems;

public class LabelClickHandler : MonoBehaviour, IPointerClickHandler
{
    public AnnotationLabel label;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (label) label.OnLabelClick();
    }
}

