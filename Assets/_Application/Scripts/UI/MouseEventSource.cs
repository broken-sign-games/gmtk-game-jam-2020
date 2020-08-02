using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GMTK2020.UI
{
    public class MouseEventSource : MonoBehaviour,
        IPointerClickHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public event Action<Vector2> DragStartedAt;
        public event Action<Vector2> DraggedTo;
        public event Action<Vector2> DragStoppedAt;
        public event Action<Vector2> ClickedAt;

        private bool isDragging = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = true;

            DragStartedAt?.Invoke(eventData.pressPosition);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            DraggedTo?.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isDragging = false;

            DragStoppedAt?.Invoke(eventData.position);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isDragging || eventData.button != PointerEventData.InputButton.Left)
                return;

            ClickedAt?.Invoke(eventData.position);
        }
    }
}
