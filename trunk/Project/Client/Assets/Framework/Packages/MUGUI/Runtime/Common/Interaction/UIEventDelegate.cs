using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//该组件用于把该组件上接收到的UI事件传递给目标GameObject
//用于新手引导的事件传递
namespace Game.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Graphic))]
    public class UIEventDelegate : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    , IDragHandler, IBeginDragHandler, IEndDragHandler
    {

        private RectTransform rectTransform { get { return transform as RectTransform; } }

        private RectTransform target;
        public void SetDelegateTarget(RectTransform target)
        {
            this.target = target;
        }

        // Update is called once per frame
        void Update()
        {
            if (target != null)
            {
                rectTransform.pivot = target.pivot;
                rectTransform.position = target.position;
                var size = rectTransform.InverseTransformVector(target.TransformVector(target.rect.size));
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.endDragHandler);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.pointerClickHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.pointerEnterHandler);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.pointerExitHandler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (target == null)
            {
                return;
            }
            ExecuteEvents.Execute(target.gameObject, eventData, ExecuteEvents.pointerUpHandler);
        }
    }

}
