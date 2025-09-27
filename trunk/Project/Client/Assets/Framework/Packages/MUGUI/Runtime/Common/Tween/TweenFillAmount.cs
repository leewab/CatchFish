using UnityEngine;
using UnityEngine.UI;
namespace MUGUI
{
    /// <summary>
    /// 补间Width
    /// </summary>

    [RequireComponent(typeof(Image))]
    [AddComponentMenu("GOGUI/Tween/Tween Fill Amount")]
    public class TweenFillAmount : UITweener
    {
        public float from = 0;
        public float to = 1;

        Image mImage;

        public Image CachedLayoutElement
        {
            get
            {
                if (mImage == null)
                    mImage = GetComponent<Image>();
                return mImage;
            }

        }



        public float value
        {
            get
            {
                return CachedLayoutElement.fillAmount;
            }
            set
            {
                CachedLayoutElement.fillAmount = value;
            }
        }

        /// <summary>
        /// Tween the value.
        /// </summary>

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = from * (1f - factor) + to * factor;
        }

        /// <summary>
        /// 开始补间操作
        /// </summary>
        static public TweenFillAmount Begin(GameObject go, float duration, float value)
        {
            TweenFillAmount comp = UITweener.Begin<TweenFillAmount>(go, duration);
            comp.from = comp.value;
            comp.to = value;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        static public TweenFillAmount GetTweenFillAmount(GameObject go)
        {
            return go.GetComponent<TweenFillAmount>();
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
