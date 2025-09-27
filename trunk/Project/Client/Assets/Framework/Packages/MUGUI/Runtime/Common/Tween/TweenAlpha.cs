using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 透明度补间
/// </summary>
namespace MUGUI
{
    [AddComponentMenu("GOGUI/Tween/Tween Alpha")]
    public class TweenAlpha : UITweener
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;
        ImageEffectOpaque dd;
        MaskableGraphic mMaskableGraphic;
        CanvasGroup canvasG;
        Color mColor;
        bool triedCanvas = false;
        public Action<GameObject> AlphaChange;

        public CanvasGroup CachedCanvasGroup
        {
            get
            {
                if (canvasG == null && !triedCanvas)
                {
                    triedCanvas = true;
                    canvasG = GetComponent<CanvasGroup>();
                    if (canvasG == null) canvasG = GetComponentInChildren<CanvasGroup>();
                }
                return canvasG;
            }
        }
        public MaskableGraphic cachedMaskableGraphic
        {
            get
            {
                if (mMaskableGraphic == null)
                {
                    mMaskableGraphic = GetComponent<MaskableGraphic>();
                    if (mMaskableGraphic == null) mMaskableGraphic = GetComponentInChildren<MaskableGraphic>();
                }
                mColor = mMaskableGraphic.color;
                return mMaskableGraphic;
            }
        }

        public float value
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }

        public float alpha
        {
            get
            {
                if (CachedCanvasGroup)
                    return CachedCanvasGroup.alpha;
                else
                    return cachedMaskableGraphic.color.a;
            }
            set
            {
                if (CachedCanvasGroup)
                    CachedCanvasGroup.alpha = value;
                else
                    cachedMaskableGraphic.color = new Color(mColor.r, mColor.g, mColor.b, value);
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Mathf.Lerp(from, to, factor);
            if (AlphaChange != null && !isFinished)
            {
                AlphaChange(this.gameObject);
            }
        }

        /// <summary>
        /// 开始补间操作
        /// </summary>

        static public TweenAlpha Begin(GameObject go, float duration, float alpha)
        {
            TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
        static public TweenAlpha GetTweenAlpha(GameObject go)
        {
            return go.GetComponent<TweenAlpha>();
        }
        public override void SetStartToCurrentValue() { from = value; }
        public override void SetEndToCurrentValue() { to = value; }
    }
}