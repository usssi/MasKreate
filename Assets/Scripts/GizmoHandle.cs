using UnityEngine;
using UnityEngine.EventSystems;

public enum GizmoHandleType
{
    Top, Bottom, Left, Right,
    CornerTL, CornerTR, CornerBL, CornerBR,
    Rotate,
    Body
}

public class GizmoHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GizmoHandleType type;
    public SelectionOutline outline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.OnHandleHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.OnHandleHoverExit(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        outline.OnHandleDown(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        outline.OnHandleDrag(this, eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        outline.OnHandleUp(this, eventData);
    }
}
