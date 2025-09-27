using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 透明度补间
/// </summary>
namespace MUGUI
{
    [AddComponentMenu("GOGUI/Tween/Tween Alpha No Canvas")]
    public class TweenAlphaNoCanvas : UITweener
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;
        List<MaskableGraphic> mMaskableGraphicList = new List<MaskableGraphic>();
        bool mInitList = false;
        public Action<GameObject> AlphaChange;

        public MaskableGraphic[] cachedMaskableGraphicList
        {
            get
            {
                if (!mInitList)
                {
                    mMaskableGraphicList.Clear();
                    MaskableGraphic[] allComponents = GetComponentsInChildren<MaskableGraphic>();
                    foreach (MaskableGraphic mg in allComponents)
                        mMaskableGraphicList.Add(mg);
                    mInitList = true;
                }
                return mMaskableGraphicList.ToArray();
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
                if (cachedMaskableGraphicList.Length > 0)
                {
                    if (cachedMaskableGraphicList[0] == null)
                    {
                        mInitList = false;
                        if (cachedMaskableGraphicList.Length > 0)
                            return cachedMaskableGraphicList[0].color.a;
                        else
                            return 0f;
                    }
                    else
                        return cachedMaskableGraphicList[0].color.a;
                }
                else
                    return 0f;
            }
            set
            {
                foreach (MaskableGraphic mg in cachedMaskableGraphicList)
                {
                    if (mg == null)
                    {
                        mInitList = false;
                        foreach (MaskableGraphic mg2 in cachedMaskableGraphicList)
                            mg2.color = new Color(mg2.color.r, mg2.color.g, mg2.color.b, value);
                        break;
                    }
                    else
                        mg.color = new Color(mg.color.r, mg.color.g, mg.color.b, value);
                }
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

        static public TweenAlphaNoCanvas Begin(GameObject go, float duration, float alpha)
        {
            TweenAlphaNoCanvas comp = UITweener.Begin<TweenAlphaNoCanvas>(go, duration);
            comp.from = comp.value;
            comp.to = alpha;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }
        static public TweenAlphaNoCanvas GetTweenAlphaNoCanvas(GameObject go)
        {
            return go.GetComponent<TweenAlphaNoCanvas>();
        }
        public override void SetStartToCurrentValue() { from = value; }
        public override void SetEndToCurrentValue() { to = value; }
    }
}