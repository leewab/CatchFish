using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Core
{
    public class UIEventTriggerListener : EventTrigger
    {
        public delegate void VoidDelegate (GameObject go);
        public delegate void PointEventDataDelegate (GameObject go, PointerEventData ped);
        
        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
 
        static public UIEventTriggerListener Get (GameObject go)
        {
            UIEventTriggerListener listener = go.GetComponent<UIEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<UIEventTriggerListener>();
            return listener;
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(gameObject);
        }
        public override void OnPointerDown (PointerEventData eventData)
        {
            onDown?.Invoke(gameObject);
        }
        public override void OnPointerEnter (PointerEventData eventData)
        {
            onEnter?.Invoke(gameObject);
        }
        public override void OnPointerExit (PointerEventData eventData)
        {
            onExit?.Invoke(gameObject);
        }
        public override void OnPointerUp (PointerEventData eventData)
        {
            onUp?.Invoke(gameObject);
        }
        public override void OnSelect (BaseEventData eventData)
        {
            onSelect?.Invoke(gameObject);
        }
        public override void OnUpdateSelected (BaseEventData eventData)
        {
            onUpdateSelect?.Invoke(gameObject);
        }
    }
}