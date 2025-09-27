using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MUGUI
{
    /// <summary>
    /// 所有补间操作的基类
    /// </summary>
    public abstract class UITweener : MonoBehaviour
    {
        /// <summary>
        /// 补间动画类型
        /// </summary>
        public enum Style
        {
            Once,
            Loop,
            PingPong,
            OnShow,
            OnHide,
            OnPress
        }

        /// <summary>
        /// 当前的补间动画触发回调函数。
        /// </summary>
        public static UITweener current;

        public EaseType easeType = EaseType.linear;

        [HideInInspector]
        public Style style = Style.Once;

        /// <summary>
        /// 动画曲线
        /// </summary>
        [HideInInspector]
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));

        /// <summary>
        /// 补间是否忽略时标
        /// </summary>
        [HideInInspector]
        public bool ignoreTimeScale = true;

        /// <summary>
        /// 延迟
        /// </summary>
        [HideInInspector]
        public float delay = 0f;

        /// <summary>
        /// 补间时常
        /// </summary>
        public float duration = 1f;

        /// <summary>
        /// 是否使用较陡的曲线 便于in/out风格插值。
        /// </summary>
        [HideInInspector]
        public bool steeperCurves = false;

        /// <summary>
        /// 补间序列
        /// </summary>
        [HideInInspector]
        public int tweenGroup = 0;

        /// <summary>
        /// 动画结束时回调
        /// </summary>
        [HideInInspector]
        public List<EventDelegate> onFinished = new List<EventDelegate>();

        /// <summary>
        /// 每次增量
        /// </summary>
        public float amountPerDelta
        {
            get
            {
                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f) * Mathf.Sign(mAmountPerDelta);
                }
                return mAmountPerDelta;
            }
        }

        /// <summary>
        /// 补间因子，0-1
        /// </summary>
        public float tweenFactor { get { return mFactor; } set { mFactor = Mathf.Clamp01(value); } }

        /// <summary>
        /// Direction that the tween is currently playing in.
        /// 当前使用的补间动画
        /// </summary>
        public AnimationOrTween.Direction direction
        {
            get { return amountPerDelta < 0f ? AnimationOrTween.Direction.Reverse : AnimationOrTween.Direction.Forward; }
        }

        private bool mStarted = false;
        private float mStartTime = 0f;
        private float mDuration = 0f;
        private float mAmountPerDelta = 1000f;
        private float mFactor = 0f;
        private List<EventDelegate> mTemp = null;

        #region MonoBehaviour

        /// <summary>
        /// 添加组件时自动重置.
        /// </summary>
        private void Reset()
        {
            if (!mStarted)
            {
                SetStartToCurrentValue();
                SetEndToCurrentValue();
            }
        }

        protected virtual void Start()
        {
            Update();
        }

        private void Update()
        {
            float delta = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
            float time = ignoreTimeScale ? RealTime.time : Time.time;

            if (!mStarted)
            {
                mStarted = true;
                mStartTime = time + delay;
            }

            if (time < mStartTime) return;

            mFactor += amountPerDelta * delta;

            if (style == Style.Loop)
            {
                if (mFactor > 1f)
                {
                    mFactor -= Mathf.Floor(mFactor);
                }
            }
            else if (style == Style.PingPong)
            {
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                }
            }

            if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
            {
                mFactor = Mathf.Clamp01(mFactor);
                Sample(mFactor, true);

                if (duration == 0f || (mFactor == 1f && mAmountPerDelta > 0f || mFactor == 0f && mAmountPerDelta < 0f))
                    enabled = false;

                if (current == null)
                {
                    current = this;

                    if (onFinished != null)
                    {
                        mTemp = onFinished;
                        onFinished = new List<EventDelegate>();

                        EventDelegate.Execute(mTemp);

                        for (int i = 0; i < mTemp.Count; ++i)
                        {
                            EventDelegate ed = mTemp[i];
                            if (ed != null) EventDelegate.Add(onFinished, ed, ed.oneShot);
                        }
                        mTemp = null;
                    }

                    current = null;
                }
            }
            else Sample(mFactor, false);
        }

        /// <summary>
        /// 标记为未启动
        /// </summary>
        private void OnDisable()
        {
            mStarted = false;
        }

        #endregion

        /// <summary>
        /// 设置一个新的委托事件
        /// </summary>
        public void SetOnFinishedOneShot(EventDelegate.Callback del)
        {
            onFinished.Clear();
            EventDelegate.Add(onFinished, del, true);
        }

        public void SetOnFinishedOneShotEvent(EventDelegate del)
        {
            onFinished.Clear();
            EventDelegate.Add(onFinished, del, true);
        }

        public void SetOnFinished(EventDelegate.Callback del)
        {
            EventDelegate.Set(onFinished, del);
        }

        public void ClearOnFinishedCallBack()
        {
            onFinished.Clear();
        }

        /// <summary>
        /// 设置一个新的委托事件
        /// </summary>
        public void SetOnFinishedEvent(EventDelegate del)
        {
            EventDelegate.Set(onFinished, del);
        }

        /// <summary>
        /// 添加新的委托.
        /// </summary>
        public void AddOnFinished(EventDelegate.Callback del)
        {
            EventDelegate.Add(onFinished, del);
        }

        /// <summary>
        /// 添加新的委托
        /// </summary>
        public void AddOnFinishedEvent(EventDelegate del)
        {
            EventDelegate.Add(onFinished, del);
        }

        /// <summary>
        /// 移除委托
        /// </summary>
        public void RemoveOnFinishedEvent(EventDelegate del)
        {
            if (onFinished != null)
            {
                onFinished.Remove(del);
            }

            if (mTemp != null)
            {
                mTemp.Remove(del);
            }
        }

        /// <summary>
        /// 在指定因素采样补间动画
        /// </summary>
        public void Sample(float factor, bool isFinished)
        {
            float val = Mathf.Clamp01(factor);
            val = EaseManager.EasingFromType(0, 1, val, easeType);
            // Add animationCurve By sxb
            OnUpdate((animationCurve != null) ? animationCurve.Evaluate(val) : val, isFinished);
            //OnUpdate(val, isFinished);
        }

        /// <summary>
        /// 正向播放
        /// </summary>
        public void PlayForward()
        {
            Play(true);
        }

        /// <summary>
        /// 反向播放
        /// </summary>
        public void PlayReverse()
        {
            Play(false);
        }

        /// <summary>
        /// (正向)播放补间
        /// </summary>
        public void Play()
        {
            Play(true);
        }

        public void Play(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
            enabled = true;
            Update();
        }

        /// <summary>
        /// 复位补间动画
        /// </summary>
        public void ResetToBeginning()
        {
            mStarted = false;
            mFactor = (amountPerDelta < 0f) ? 1f : 0f;
            Sample(mFactor, false);
        }

        /// <summary>
        /// 反转补间动画方向
        /// </summary>
        public void Toggle()
        {
            if (mFactor > 0f)
            {
                mAmountPerDelta = -amountPerDelta;
            }
            else
            {
                mAmountPerDelta = Mathf.Abs(amountPerDelta);
            }
            enabled = true;
        }

        /// <summary>
        /// 实际补间的逻辑-继承
        /// </summary>
        protected abstract void OnUpdate(float factor, bool isFinished);

        /// <summary>
        /// 开始补间操作
        /// </summary>
        public static T Begin<T>(GameObject go, float duration) where T : UITweener
        {
            T comp = go.GetComponent<T>();
#if UNITY_FLASH
        if ((object)comp == null) comp = (T)go.AddComponent<T>();
#else
            //找到未设置id组的补间
            if (comp != null && comp.tweenGroup != 0)
            {
                comp = null;
                T[] comps = go.GetComponents<T>();
                for (int i = 0, imax = comps.Length; i < imax; ++i)
                {
                    comp = comps[i];
                    if (comp != null && comp.tweenGroup == 0) break;
                    comp = null;
                }
            }

            if (comp == null) comp = go.AddComponent<T>();
#endif
            comp.mStarted = false;
            comp.duration = duration;
            comp.mFactor = 0f;
            comp.mAmountPerDelta = Mathf.Abs(comp.amountPerDelta);
            comp.style = Style.Once;
            comp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
            comp.enabled = true;

            if (duration <= 0f)
            {
                comp.Sample(1f, true);
                comp.enabled = false;
            }
            return comp;
        }

        public static void SetEnable(GameObject go, bool value)
        {
            UITweener comp = go.GetComponent<UITweener>();
            if (comp != null)
            {
                comp.enabled = value;
            }
        }

        /// <summary>
        /// 设置开始(form)值
        /// </summary>
        public virtual void SetStartToCurrentValue() { }

        /// <summary>
        /// 设置结束(to)值
        /// </summary>
        public virtual void SetEndToCurrentValue() { }

        /// <summary>
        /// 延长延时时间
        /// 如果正在播放tween动画则RePlay
        /// </summary>
        public void SetDelayTimeAdd()
        {
            float time = ignoreTimeScale ? RealTime.time : Time.time;

            if (time >= mStartTime)
            {
                //已经运行了
                ResetToBeginning();
                PlayForward();
            }
            else {
                mStartTime = time + delay;
            }
        }
        public void OnDestroy()
        {
            onFinished.Clear();
        }

        public void OnShow()
        {
            if(this.style == Style.OnShow)
            {
                PlayForward();
            }
            if(this.style == Style.OnHide)
            {
                ResetToBeginning();
            }
        }

        public void OnHide()
        {
            if (this.style == Style.OnShow)
            {
                ResetToBeginning();
            }
            if (this.style == Style.OnHide)
            {
                PlayForward();
            }
        }
    }
}