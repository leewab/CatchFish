using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MUGUI
{
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("GOGUI/Tween/Tween Random Text")]
    public class TweenRandomText : UITweener
    {

        public float from;
        public float to;
        public float mFinish;

        float mValue;

        private Text mText;
        public Text cacheText
        {
            get
            {
                mText = GetComponent<Text>();
                if (mText == null)
                {
                    Debug.LogError("Text null");
                }
                return mText;
            }
        }

        /// <summary>
        /// 小数位
        /// </summary>
        public int digits;


        public float value
        {
            get { return mValue; }
            set
            {
                mValue = value;
            }
        }

        protected override void OnUpdate(float factor, bool isFinished)
        {
            value = Random.Range(from, to);
            ValueUpdate(value, isFinished);
        }

        protected void ValueUpdate(float value, bool isFinished)
        {
            if (isFinished)
            {
                cacheText.text = (System.Math.Round(mFinish, digits)).ToString();
            }
            else
            {
                cacheText.text = (System.Math.Round(value, digits)).ToString();
            }
        }

        public static TweenRandomText Begin(Text label, float duration, float delay, float from, float to, float finish)
        {
            TweenRandomText comp = UITweener.Begin<TweenRandomText>(label.gameObject, duration);
            comp.from = from;
            comp.to = to;
            comp.mFinish = finish;
            comp.delay = delay;

            if (duration <= 0)
            {
                comp.Sample(1, true);
                comp.enabled = false;
            }
            return comp;
        }

        public void PlayRandomLoop(float duration)
        {
            this.duration = duration;
            style = Style.Loop;
            Play(true);
        }

        public void PlayRandom(float duration, float finish)
        {
            this.duration = duration;
            mFinish = finish;
            Play(true);
        }

        public void PlayRandom(float duration, float from, float to, float finish)
        {
            this.duration = duration;
            this.from = from;
            this.to = to;
            mFinish = finish;
            Play(true);
        }

        public void StopRandom(float finish)
        {
            mFinish = finish;
            this.duration = 0.1f;
            style = Style.Once;
            Play(true);
        }
    }
}