using UnityEngine;
using UnityEngine.UI;
namespace MUGUI
{
    /// <summary>
    /// 补间Width
    /// </summary>

    [RequireComponent(typeof(LayoutElement))]
    [AddComponentMenu("MUGUI/Tween/Tween Layout Width")]
    public class TweenLayoutWidth : UITweener
    {
        public int from = 100;
        public int to = 100;

        LayoutElement mLayoutElement;
        //UITable mTable;

        public LayoutElement CachedLayoutElement
        {
            get
            {
                if (mLayoutElement == null)
                    mLayoutElement = GetComponent<LayoutElement>();
                return mLayoutElement;
            }

        }



        public int value
        {
            get
            {
                return (int)CachedLayoutElement.preferredWidth;
            }
            set
            {
                CachedLayoutElement.preferredWidth = value;
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
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