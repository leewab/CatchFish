using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public delegate void VoidDelegate ();
    public delegate void VoidDelegateParam (GameObject go);
    public delegate void PointEventDataDelegate (GameObject go, PointerEventData ped);
    public delegate void AxisEventDataDelegate (GameObject go, AxisEventData aed);
    
    public class UIEventTriggerListener :  
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        IDropHandler,
        IScrollHandler,
        IUpdateSelectedHandler,
        ISelectHandler,
        IDeselectHandler,
        IMoveHandler,
        ISubmitHandler,
        ICancelHandler
    {
        public VoidDelegateParam onClick;
        public VoidDelegateParam onPointerEnter;
        public VoidDelegateParam onPointerExit;
        public VoidDelegateParam onPointerDown;
        public VoidDelegateParam onPointerUp;
        public VoidDelegateParam onSelect;
        public VoidDelegateParam onUpdateSelect;
        public VoidDelegateParam onDeselect;
        public PointEventDataDelegate onDrag;
        public PointEventDataDelegate onBeginDrag;
        public PointEventDataDelegate onEndDrag;
        public PointEventDataDelegate onInitializePotentialDrag;
        public PointEventDataDelegate onDrop;
        public PointEventDataDelegate onScroll;
        public AxisEventDataDelegate onMove;
        public VoidDelegate onSubmit;
        public VoidDelegate onCancel;
 
        public static UIEventTriggerListener Get (GameObject go)
        {
            UIEventTriggerListener listener = go.GetComponent<UIEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<UIEventTriggerListener>();
            return listener;
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(gameObject);
        }
        public virtual void OnPointerDown (PointerEventData eventData)
        {
            onPointerDown?.Invoke(gameObject);
        }
        public virtual void OnPointerEnter (PointerEventData eventData)
        {
            onPointerEnter?.Invoke(gameObject);
        }
        public virtual void OnPointerExit (PointerEventData eventData)
        {
            onPointerExit?.Invoke(gameObject);
        }
        public virtual void OnPointerUp (PointerEventData eventData)
        {
            onPointerUp?.Invoke(gameObject);
        }
        public virtual void OnSelect (BaseEventData eventData)
        {
            onSelect?.Invoke(gameObject);
        }
        public virtual void OnUpdateSelected (BaseEventData eventData)
        {
            onUpdateSelect?.Invoke(gameObject);
        }
        public virtual void OnDeselect(BaseEventData eventData)
        {
            onDeselect?.Invoke(gameObject);
        }
        
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            onInitializePotentialDrag?.Invoke(gameObject, eventData);
        }
        public virtual void OnDrag(PointerEventData eventData)
        {
            onDrag?.Invoke(gameObject, eventData);
        }
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            onBeginDrag?.Invoke(gameObject, eventData);
        }
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            onEndDrag?.Invoke(gameObject, eventData);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            onDrop?.Invoke(gameObject, eventData);
        }

        public virtual void OnScroll(PointerEventData eventData)
        {
            onScroll?.Invoke(gameObject, eventData);
        }

        public virtual void OnMove(AxisEventData eventData)
        {
            onMove?.Invoke(gameObject, eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            onSubmit?.Invoke();
        }

        public virtual void OnCancel(BaseEventData eventData)
        {
            onCancel?.Invoke();
        }
    }

    public class UIClickEventTriggerListener :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler,
        IPointerClickHandler 
    {
        public VoidDelegateParam onClick;
        public VoidDelegateParam onPointerEnter;
        public VoidDelegateParam onPointerExit;
        public VoidDelegateParam onPointerDown;
        public VoidDelegateParam onPointerUp;
        public VoidDelegateParam onLongPress;           //长按事件
        public VoidDelegateParam onLongPressPlus;       //长按事件加时
        public VoidDelegateParam onDoubleClick;         //双击
        public VoidDelegateParam onTripleClick;         //三击

        // 连击
        private int curClickTimes = 0;                  //点击次数
        private float totalClickTime = 0.4f;            //点击间隔
        
        // 长按
        private float totalLongPressTime = 0.5f;
        private float totalLongPressPlusTime = 1f;
        private float curPressTime = 0;
        private bool isPressDown = false;
        private bool isPressUp = false;
        
        public static UIClickEventTriggerListener Get(GameObject go)
        {
            UIClickEventTriggerListener listener = go.GetComponent<UIClickEventTriggerListener>();
            if (listener == null) listener = go.AddComponent<UIClickEventTriggerListener>();
            return listener;
        }
        
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            onClick?.Invoke(gameObject);
        }
        public virtual void OnPointerDown (PointerEventData eventData)
        {
            onPointerDown?.Invoke(gameObject);
            isPressDown = true;
            curPressTime = 0;
        }
        public virtual void OnPointerUp (PointerEventData eventData)
        {
            onPointerUp?.Invoke(gameObject);
            isPressUp = true;
            curClickTimes++;
        }
        public virtual void OnPointerEnter (PointerEventData eventData)
        {
            onPointerEnter?.Invoke(gameObject);
        }
        public virtual void OnPointerExit (PointerEventData eventData)
        {
            onPointerExit?.Invoke(gameObject);
        }

        private void Update()
        {
            CalculateLongPress();
        }

        private void CalculateLongPress()
        {
            if (!isPressDown) return;
            curPressTime += Time.time;
            if (isPressUp)
            {
                isPressUp = false;
                isPressDown = false;
                // 长按检测
                if (curPressTime > totalLongPressPlusTime)
                {
                    onLongPressPlus?.Invoke(gameObject);
                }
                else if (curPressTime > totalLongPressTime)
                {
                    onLongPress?.Invoke(gameObject);
                }
                
                // 双击检测
                if (curPressTime <= totalClickTime)
                {
                    if (curClickTimes == 3)
                    {
                        curClickTimes = 0;
                        onTripleClick?.Invoke(gameObject);
                    }
                    else if (curClickTimes == 2)
                    {
                        curClickTimes = 0;
                        onDoubleClick?.Invoke(gameObject);
                    }
                }
                else
                {
                    curClickTimes = 0;
                }
            }
        }
    }
}