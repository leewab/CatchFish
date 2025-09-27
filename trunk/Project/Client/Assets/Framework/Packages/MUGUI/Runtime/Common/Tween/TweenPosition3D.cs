using UnityEngine;

namespace MUGUI
{
    /// <summary>
    /// 坐标补间
    /// </summary>
    public class TweenPosition3D : UITweener
    {
        public Vector3 from;
        public Vector3 to;

        /// <summary>
        /// 是否是世界坐标
        /// </summary>
        [HideInInspector]
        public bool worldSpace = false;
        public Vector3 value
        {
            get
            {
                return worldSpace ? transform.position : transform.localPosition;
            }
            set
            {
                if (worldSpace) transform.position = value;
                else transform.localPosition = value;

            }
        }

        void Awake()
        {

        }


        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }

        /// <summary>
        /// 开始补间操作
        /// </summary>

        static public TweenPosition3D Begin(GameObject go, float duration, Vector3 pos)
        {
            TweenPosition3D comp = UITweener.Begin<TweenPosition3D>(go, duration);
            comp.from = comp.value;
            comp.to = pos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        static public TweenPosition3D BeginFrom(GameObject go, float duration, Vector3 fromPos,Vector3 toPos)
        {
            go.transform.localPosition = fromPos;
            TweenPosition3D comp = UITweener.Begin<TweenPosition3D>(go, duration);
            comp.from = fromPos;
            comp.to = toPos;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
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
