using UnityEngine;

namespace MUGUI
{
    /// <summary>
    /// 坐标补间
    /// </summary>
    [AddComponentMenu("MUGUI/Tween/Tween Position")]
    public class TweenPosition : UITweener
    {
        public Vector3 from;
        public Vector3 to;

        /// <summary>
        /// 是否是世界坐标
        /// </summary>
        [HideInInspector]
        public bool worldSpace = false;
        
        /// <summary>
        /// 是否有ugui
        /// </summary>
        public bool notUGUI = false;
        
        private RectTransform mRectTransform;

        public RectTransform _RectTransform
        {
            get
            {
                if (mRectTransform == null) mRectTransform = GetComponent<RectTransform>();
                if (mRectTransform == null) notUGUI = true;
                return mRectTransform;
            }
        }
        
        public Vector3 value
        {
            get
            {
                if (notUGUI)
                {
                    return worldSpace ? transform.position : transform.localPosition;
                }
                else
                {
                    return worldSpace ? _RectTransform.position : _RectTransform.anchoredPosition3D;
                }
            }
            set
            {
                if (notUGUI)
                {
                    if (worldSpace)
                    {
                        transform.position = value;
                    }
                    else
                    {
                        transform.localPosition = value;
                    }
                }
                else
                {
                    if (worldSpace)
                    {
                        _RectTransform.position = value;
                    }
                    else
                    {
                        _RectTransform.anchoredPosition3D = value;
                    }
                }
            }
        }

        void Awake()
        {
            mRectTransform = GetComponent<RectTransform>();
            if (mRectTransform == null) notUGUI = true;
        }


        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }

        /// <summary>
        /// 开始补间操作
        /// </summary>

        static public TweenPosition Begin(GameObject go, float duration, Vector3 pos)
        {
            TweenPosition comp = UITweener.Begin<TweenPosition>(go, duration);
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        private void OnEnable()
        {
            base.OnShow();
        }

        private void OnDisable()
        {
            base.OnHide();
        }
        [ContextMenu("设置当前值为From的值")]
        public override void SetStartToCurrentValue() { from = value; }

        [ContextMenu("设置当前值为To的值")]
        public override void SetEndToCurrentValue() { to = value; }

        [ContextMenu("切换到From值状态")]
        void SetCurrentValueToStart() { value = from; }

        [ContextMenu("切换到To值状态")]
        void SetCurrentValueToEnd() { value = to; }
    }
}
