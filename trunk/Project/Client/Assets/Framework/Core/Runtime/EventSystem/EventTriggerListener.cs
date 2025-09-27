using UnityEngine;
using UnityEngine.EventSystems;


namespace Game.Core
{
    public class DragEventTriggerListener : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        GameObject go;
        public event EventTriggerListener.Vector2Delegate onDrag;
        public event EventTriggerListener.VectorDelegate onDragStart;
        public event EventTriggerListener.VectorDelegate onDragEnd;

        static public DragEventTriggerListener Get(GameObject go)
        {
            DragEventTriggerListener listener = go.GetComponent<DragEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<DragEventTriggerListener>();
            listener.go = go;
            return listener;
        }

        static public DragEventTriggerListener Get(Transform transform)
        {
            DragEventTriggerListener listener = transform.GetComponent<DragEventTriggerListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<DragEventTriggerListener>();
            return listener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(go, eventData.delta, eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onDragStart != null)
                onDragStart(go, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
                onDragEnd(go, eventData.position);
        }
    }

    public class DragEventTriggerListener2 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        GameObject go;
        public event EventTriggerListener.Vector2Delegate onDrag;
        public event EventTriggerListener.VectorDelegate onDragStart;
        public event EventTriggerListener.VectorDelegate onDragEnd;

        static public DragEventTriggerListener2 Get(GameObject go)
        {
            DragEventTriggerListener2 listener = go.GetComponent<DragEventTriggerListener2>();
            if (listener == null) listener = go.AddComponent<DragEventTriggerListener2>();
            listener.go = go;
            return listener;
        }

        static public DragEventTriggerListener2 Get(Transform transform)
        {
            DragEventTriggerListener2 listener = transform.GetComponent<DragEventTriggerListener2>();
            if (listener == null) listener = transform.gameObject.AddComponent<DragEventTriggerListener2>();
            return listener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(go, eventData.delta, eventData.delta);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onDragStart != null)
                onDragStart(go, eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
                onDragEnd(go, eventData.delta);
        }
    }


    public class DragEventTriggerListener3 : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        GameObject go;
        public event EventTriggerListener.Vector2Delegate onDrag;
        public event EventTriggerListener.VectorDelegate onDragStart;
        public event EventTriggerListener.VectorDelegate onDragEnd;

        static public DragEventTriggerListener3 Get(GameObject go)
        {
            DragEventTriggerListener3 listener = go.GetComponent<DragEventTriggerListener3>();
            if (listener == null) listener = go.AddComponent<DragEventTriggerListener3>();
            listener.go = go;
            return listener;
        }

        static public DragEventTriggerListener3 Get(Transform transform)
        {
            DragEventTriggerListener3 listener = transform.GetComponent<DragEventTriggerListener3>();
            if (listener == null) listener = transform.gameObject.AddComponent<DragEventTriggerListener3>();
            return listener;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (onDrag != null)
                onDrag(go, eventData.delta, eventData.delta);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (onDragStart != null)
                onDragStart(eventData.pointerPress, eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (onDragEnd != null)
                onDragEnd(eventData.pointerPress, eventData.position);
        }
    }


    public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler, IUpdateSelectedHandler, IDeselectHandler
    {
        protected GameObject go;
        public delegate void VoidDelegate(GameObject go);
        public delegate void BoolDelegate(GameObject go, bool state);
        public delegate void FloatDelegate(GameObject go, float delta);
        public delegate void VectorDelegate(GameObject go, Vector2 delta);
        public delegate void Vector2Delegate(GameObject go, Vector2 delta, Vector2 pos);
        public delegate void ObjectDelegate(GameObject go, GameObject obj);
        public delegate void KeyCodeDelegate(GameObject go, KeyCode key);
        public delegate void VectorFloatDelegate(GameObject go, Vector2 pos,float val);

        public VoidDelegate onClick;
        public VoidDelegate onDown;
        public VoidDelegate onEnter;
        public VoidDelegate onExit;
        public VoidDelegate onUp;
        public VoidDelegate onSelect;
        public VoidDelegate onUpdateSelect;
        public VoidDelegate onDeselect;
        public VoidDelegate onDoubleClick;
        public VectorDelegate onClickPosition;
        public object parameter;
        public PointerEventData PointerEventData;
        public int event_id = 0;

        protected bool isTouchDown = false;
        protected bool isLongpress = false;
        protected float touchBegin = 0;

        public static VoidDelegate GlobalClickCallback { get; set; }


        static public EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerListener>();
            listener.go = go;
            return listener;
        }

        static public EventTriggerListener Get(Transform transform)
        {
            EventTriggerListener listener = transform.GetComponent<EventTriggerListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<EventTriggerListener>();
            return listener;
        }

        private float mLastClickTime = 0f;
        public float CLICK_INTERVAL = 0.1f;
        public float DOUBLE_CLICK_INTERVAL = 0.4f;
        private bool CheckClick(float currentTime)
        {
            if (currentTime - mLastClickTime < CLICK_INTERVAL)
                return false;
            return true;
        }

        private bool CheckDoubleClick(float currentTime)
        {
            if (currentTime - mLastClickTime > DOUBLE_CLICK_INTERVAL)
                return false;
            return true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (GlobalClickCallback != null) GlobalClickCallback(go);

            if (onDoubleClick != null && CheckDoubleClick(eventData.clickTime))
            {
                mLastClickTime = eventData.clickTime;
                onDoubleClick(go);
            }
            else
            {
                if (!CheckClick(eventData.clickTime))
                    return;
                mLastClickTime = eventData.clickTime;
                if (onClick != null)
                {
                    UnityEngine.Profiling.Profiler.BeginSample("on click : " + gameObject.name);
                    onClick(go);
                    UnityEngine.Profiling.Profiler.EndSample();
                } 
            }
            if (onClickPosition != null) onClickPosition(go, eventData.position);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onDown != null) onDown(go);

            touchBegin = Time.time;
            isTouchDown = true;
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onEnter != null) onEnter(go);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onExit != null) onExit(go);

            isTouchDown = false;
            isLongpress = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerEventData = eventData;
            if (onUp != null) onUp(go);

            isTouchDown = false;
            isLongpress = false;
        }
        public void OnSelect(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onSelect != null) onSelect(go);
        }
        public void OnUpdateSelected(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onUpdateSelect != null) onUpdateSelect(go);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            PointerEventData = null;
            if (onDeselect != null) onDeselect(go);
        }
    }
    //重压
    public class EventTriggerPressurePressListener: EventTriggerListener
    {
        public System.Action<GameObject, float, bool> onPressurePress;
        new static public EventTriggerPressurePressListener Get(GameObject go)
        {
            EventTriggerPressurePressListener listener = go.GetComponent<EventTriggerPressurePressListener>();
            if (listener == null)
            {
                EventTriggerListener parent = go.GetComponent<EventTriggerListener>();
                listener = go.AddComponent<EventTriggerPressurePressListener>();

                if (parent != null)
                {
                    listener.onClick = parent.onClick;
                    listener.onDown = parent.onDown;
                    listener.onEnter = parent.onEnter;
                    listener.onExit = parent.onExit;
                    listener.onUp = parent.onUp;
                    listener.onSelect = parent.onSelect;
                    listener.onUpdateSelect = parent.onUpdateSelect;
                    listener.onDeselect = parent.onDeselect;
                    listener.onDoubleClick = parent.onDoubleClick;
                    listener.onClickPosition = parent.onClickPosition;
                    listener.parameter = parent.parameter;
                    listener.PointerEventData = parent.PointerEventData;
                    GameObject.DestroyImmediate(parent);
                }
            }
            listener.go = go;
            return listener;
        }
        void Update()
        {
            if (!isTouchDown)
                return;
            bool isPressure = false;
            float p = 1;
            if (Input.touchPressureSupported)
            {
                p = Input.GetTouch(0).pressure;
                if (p > 5)
                {
                    isPressure = true;
                }
            }
            if (onPressurePress != null)
            {
                onPressurePress(go, p, isPressure);
            }
        }
    }
    public class EventTriggerPressListener : EventTriggerListener
    {
        new static public EventTriggerPressListener Get(GameObject go)
        {

            EventTriggerPressListener listener = go.GetComponent<EventTriggerPressListener>();
            if (listener == null)
            {
                EventTriggerListener parent = go.GetComponent<EventTriggerListener>();
                listener = go.AddComponent<EventTriggerPressListener>();

                if (parent != null)
                {
                    listener.onClick = parent.onClick;
                    listener.onDown = parent.onDown;
                    listener.onEnter = parent.onEnter;
                    listener.onExit = parent.onExit;
                    listener.onUp = parent.onUp;
                    listener.onSelect = parent.onSelect;
                    listener.onUpdateSelect = parent.onUpdateSelect;
                    listener.onDeselect = parent.onDeselect;
                    listener.onDoubleClick = parent.onDoubleClick;
                    listener.onClickPosition = parent.onClickPosition;
                    listener.parameter = parent.parameter;
                    listener.PointerEventData = parent.PointerEventData;
                    listener.event_id = parent.event_id;
                    GameObject.DestroyImmediate(parent);
                }
            }
            listener.go = go;
            return listener;
        }

        private float lastInvokeTime = 0;
        public VoidDelegate onLongPress;
        float interval = 0.1f;
        float longPressDelay = 0.5f;

        private void OnDisable()
        {
            if (onUp != null) onUp(go);
            isLongpress = false;
            isTouchDown = false;
        }
        // Update is called once per frame
        void Update()
        {
            if (!isTouchDown)
                return;

            if (isLongpress)
            {
                if ((Time.time - lastInvokeTime) > interval)
                {
                    if (onLongPress != null)
                        onLongPress(go);
                    lastInvokeTime = Time.time;
                }
            }
            else
            {
                isLongpress = Time.time - touchBegin > longPressDelay;
            }

        }

        public void ForbidOnClick()
        {
            EventTriggerPressListener listener = go.GetComponent<EventTriggerPressListener>();
            if (listener != null)
            {
                listener.onClick = null;
            }
        }
    }


    public class EventTriggerDargClickListener : MonoBehaviour, IPointerClickHandler
    {
        protected GameObject go;
        public delegate void VoidDelegate();
        public delegate void BoolDelegate(GameObject go, bool state);
        public delegate void FloatDelegate(GameObject go, float delta);
        public delegate void VectorDelegate(GameObject go, Vector2 delta);
        public delegate void Vector2Delegate(GameObject go, Vector2 delta, Vector2 pos);
        public delegate void ObjectDelegate(GameObject go, GameObject obj);
        public delegate void KeyCodeDelegate(GameObject go, KeyCode key);

        public VoidDelegate onClick;
        public object parameter;
        Vector2 dragStartPos;
        Vector2 pressPos;


        static public EventTriggerDargClickListener Get(GameObject go)
        {
            EventTriggerDargClickListener listener = go.GetComponent<EventTriggerDargClickListener>();
            if (listener == null) listener = go.AddComponent<EventTriggerDargClickListener>();
            listener.go = go;
            return listener;
        }

        static public EventTriggerDargClickListener Get(Transform transform)
        {
            EventTriggerDargClickListener listener = transform.GetComponent<EventTriggerDargClickListener>();
            if (listener == null) listener = transform.gameObject.AddComponent<EventTriggerDargClickListener>();
            return listener;
        }

        public void DoClick()
        {
            if (onClick != null) onClick();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (onClick != null) onClick();
        }
    }


}
