// using MUEngine;
// using System.Collections.Generic;
// using UnityEngine;
//
// /// <summary>
// /// 透明度补间
// /// </summary>
// namespace MUGUI
// {
//     [AddComponentMenu("GOGUI/Tween/Tween ImageBar")]
//     public class TweenImageBar : MonoBehaviour
//     {
//         [Range(0f, 1f)]
//         public float bar1from = 1f;
//         [Range(0f, 1f)]
//         public float bar1to = 1f;
//         [Range(0f, 1f)]
//         public float bar2from = 1f;
//         [Range(0f, 1f)]
//         public float bar2to = 1f;
//         [Range(0f, 1f)]
//         public float bar3from = 1f;
//         [Range(0f, 1f)]
//         public float bar3to = 1f;
//
//         MUGame.GameImageBar mImageBar;
//
//         /// <summary>
//         /// 补间是否忽略时标
//         /// </summary>
//         [HideInInspector]
//         public bool ignoreTimeScale = true;
//         /// <summary>
//         /// 延迟
//         /// </summary>
//         [HideInInspector]
//         public float bar1delay = 0f;
//         [HideInInspector]
//         public float bar2delay = 0f;
//         [HideInInspector]
//         public float bar3delay = 0f;
//
//         /// <summary>
//         /// 补间时常
//         /// </summary>
//         public float bar1duration = 1f;
//         public float bar2duration = 1f;
//         public float bar3duration = 1f;
//
//         bool bar1enabled = false;
//         bool bar2enabled = false;
//         bool bar3enabled = false;
//
//         bool bar1Started = false;
//         bool bar2Started = false;
//         bool bar3Started = false;
//         float bar1Factor = 0f;
//         float bar2Factor = 0f;
//         float bar3Factor = 0f;
//         float bar1AmountPerDelta = 1000f;
//         float bar2AmountPerDelta = 1000f;
//         float bar3AmountPerDelta = 1000f;
//         float bar1StartTime = 0f;
//         float bar2StartTime = 0f;
//         float bar3StartTime = 0f;
//         float bar1Duration = 0f;
//         float bar2Duration = 0f;
//         float bar3Duration = 0f;
//
//         [HideInInspector]
//         public List<EventDelegate> onFinishedBar1 = new List<EventDelegate>();
//         [HideInInspector]
//         public List<EventDelegate> onFinishedBar2 = new List<EventDelegate>();
//         [HideInInspector]
//         public List<EventDelegate> onFinishedBar3 = new List<EventDelegate>();
//
//         /// <summary>
//         /// 当前的补间动画触发回调函数。
//         /// </summary>
//         static public TweenImageBar currentBar1;
//         static public TweenImageBar currentBar2;
//         static public TweenImageBar currentBar3;
//
//         public float bar1
//         {
//             get
//             {
//                 if (mImageBar != null)
//                     return mImageBar.GetBar1();
//                 return 0;
//             }
//             set
//             {
//                 if (mImageBar != null)
//                     mImageBar.SetBar1(value);
//             }
//         }
//
//         public float bar2
//         {
//             get
//             {
//                 if (mImageBar != null)
//                     return mImageBar.GetBar2();
//                 return 0;
//             }
//             set
//             {
//                 if (mImageBar != null)
//                     mImageBar.SetBar2(value);
//             }
//         }
//
//         public float bar3
//         {
//             get
//             {
//                 if (mImageBar != null)
//                     return mImageBar.GetBar3();
//                 return 0;
//             }
//             set
//             {
//                 if (mImageBar != null)
//                     mImageBar.SetBar3(value);
//             }
//         }
//         
//         /// <summary>
//         /// 每次增量
//         /// </summary>
//
//         public float amountPerDeltaBar1
//         {
//             get
//             {
//                 if (bar1Duration != bar1duration)
//                 {
//                     bar1Duration = bar1duration;
//                     bar1AmountPerDelta = Mathf.Abs((bar1duration > 0f) ? 1f / bar1duration : 1000f) * Mathf.Sign(bar1AmountPerDelta);
//                 }
//                 return bar1AmountPerDelta;
//             }
//         }
//
//         public float amountPerDeltaBar2
//         {
//             get
//             {
//                 if (bar2Duration != bar2duration)
//                 {
//                     bar2Duration = bar2duration;
//                     bar2AmountPerDelta = Mathf.Abs((bar2duration > 0f) ? 1f / bar2duration : 1000f) * Mathf.Sign(bar2AmountPerDelta);
//                 }
//                 return bar2AmountPerDelta;
//             }
//         }
//
//         public float amountPerDeltaBar3
//         {
//             get
//             {
//                 if (bar3Duration != bar3duration)
//                 {
//                     bar3Duration = bar3duration;
//                     bar3AmountPerDelta = Mathf.Abs((bar3duration > 0f) ? 1f / bar3duration : 1000f) * Mathf.Sign(bar3AmountPerDelta);
//                 }
//                 return bar3AmountPerDelta;
//             }
//         }
//
//         public void SetImageBar(IGameUIComponent imgbar)
//         {
//             mImageBar = (MUGame.GameImageBar)imgbar;
//         }
//
//         void Start()
//         {
//             Update();
//         }
//
//         void Update()
//         {
//             float delta = ignoreTimeScale ? RealTime.deltaTime : Time.deltaTime;
//             float time = ignoreTimeScale ? RealTime.time : Time.time;
//
//             UpdateBar1(delta, time);
//             UpdateBar2(delta, time);
//             UpdateBar3(delta, time);
//         }
//
//         List<EventDelegate> mTemp = null;
//
//         void UpdateBar1(float delta, float time)
//         {
//             if (!bar1enabled)
//                 return;
//             if (!bar1Started)
//             {
//                 bar1Started = true;
//                 bar1StartTime = time + bar1delay;
//             }
//             if (time < bar1StartTime)
//                 return;
//             bar1Factor += amountPerDeltaBar1 * delta;
//             if (bar1duration == 0f || bar1Factor > 1f || bar1Factor < 0f)
//             {
//                 bar1Factor = Mathf.Clamp01(bar1Factor);
//                 SampleBar1(bar1Factor, true);
//
//                 if (bar1duration == 0f || (bar1Factor == 1f && bar1AmountPerDelta > 0f || bar1Factor == 0f && bar1AmountPerDelta < 0f))
//                     SetBar1Enable(false);
//
//                 if (currentBar1 == null)
//                 {
//                     currentBar1 = this;
//
//                     if (onFinishedBar1 != null)
//                     {
//                         mTemp = onFinishedBar1;
//                         onFinishedBar1 = new List<EventDelegate>();
//
//                         EventDelegate.Execute(mTemp);
//
//                         for (int i = 0; i < mTemp.Count; ++i)
//                         {
//                             EventDelegate ed = mTemp[i];
//                             if (ed != null) EventDelegate.Add(onFinishedBar1, ed, ed.oneShot);
//                         }
//                         mTemp = null;
//                     }
//
//                     currentBar1 = null;
//                 }
//             }
//             else
//                 SampleBar1(bar1Factor, false);
//         }
//
//         void UpdateBar2(float delta, float time)
//         {
//             if (!bar2enabled)
//                 return;
//             if (!bar2Started)
//             {
//                 bar2Started = true;
//                 bar2StartTime = time + bar2delay;
//             }
//             if (time < bar2StartTime)
//                 return;
//             bar2Factor += amountPerDeltaBar2 * delta;
//             if (bar2duration == 0f || bar2Factor > 1f || bar2Factor < 0f)
//             {
//                 bar2Factor = Mathf.Clamp01(bar2Factor);
//                 SampleBar2(bar2Factor, true);
//
//                 if (bar2duration == 0f || (bar2Factor == 1f && bar2AmountPerDelta > 0f || bar2Factor == 0f && bar2AmountPerDelta < 0f))
//                     SetBar2Enable(false);
//
//                 if (currentBar2 == null)
//                 {
//                     currentBar2 = this;
//
//                     if (onFinishedBar2 != null)
//                     {
//                         mTemp = onFinishedBar2;
//                         onFinishedBar2 = new List<EventDelegate>();
//
//                         EventDelegate.Execute(mTemp);
//
//                         for (int i = 0; i < mTemp.Count; ++i)
//                         {
//                             EventDelegate ed = mTemp[i];
//                             if (ed != null) EventDelegate.Add(onFinishedBar2, ed, ed.oneShot);
//                         }
//                         mTemp = null;
//                     }
//
//                     currentBar2 = null;
//                 }
//             }
//             else
//                 SampleBar2(bar2Factor, false);
//         }
//
//         void UpdateBar3(float delta, float time)
//         {
//             if (!bar3enabled)
//                 return;
//             if (!bar3Started)
//             {
//                 bar3Started = true;
//                 bar3StartTime = time + bar3delay;
//             }
//             if (time < bar3StartTime)
//                 return;
//             bar3Factor += amountPerDeltaBar3 * delta;
//             if (bar3duration == 0f || bar3Factor > 1f || bar3Factor < 0f)
//             {
//                 bar3Factor = Mathf.Clamp01(bar3Factor);
//                 SampleBar3(bar3Factor, true);
//
//                 if (bar3duration == 0f || (bar3Factor == 1f && bar3AmountPerDelta > 0f || bar3Factor == 0f && bar3AmountPerDelta < 0f))
//                     SetBar3Enable(false);
//
//                 if (currentBar3 == null)
//                 {
//                     currentBar3 = this;
//
//                     if (onFinishedBar3 != null)
//                     {
//                         mTemp = onFinishedBar3;
//                         onFinishedBar3 = new List<EventDelegate>();
//
//                         EventDelegate.Execute(mTemp);
//
//                         for (int i = 0; i < mTemp.Count; ++i)
//                         {
//                             EventDelegate ed = mTemp[i];
//                             if (ed != null) EventDelegate.Add(onFinishedBar3, ed, ed.oneShot);
//                         }
//                         mTemp = null;
//                     }
//
//                     currentBar3 = null;
//                 }
//             }
//             else
//                 SampleBar3(bar3Factor, false);
//         }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar1OnFinished(EventDelegate.Callback del) { EventDelegate.Set(onFinishedBar1, del); }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar1OnFinished(EventDelegate del) { EventDelegate.Set(onFinishedBar1, del); }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar2OnFinished(EventDelegate.Callback del) { EventDelegate.Set(onFinishedBar2, del); }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar2OnFinished(EventDelegate del) { EventDelegate.Set(onFinishedBar2, del); }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar3OnFinished(EventDelegate.Callback del) { EventDelegate.Set(onFinishedBar3, del); }
//
//         /// <summary>
//         /// 设置一个新的委托事件
//         /// </summary>
//         public void SetBar3OnFinished(EventDelegate del) { EventDelegate.Set(onFinishedBar3, del); }
//
//
//         public void SetBar1Enable(bool enable)
//         {
//             bar1enabled = enable;
//             enabled = bar1enabled || bar2enabled || bar3enabled;
//         }
//
//         public void SetBar2Enable(bool enable)
//         {
//             bar2enabled = enable;
//             enabled = bar1enabled || bar2enabled || bar3enabled;
//         }
//         public void SetBar3Enable(bool enable)
//         {
//             bar3enabled = enable;
//             enabled = bar1enabled || bar2enabled || bar3enabled;
//         }
//
//         public void SampleBar1(float factor, bool isFinished)
//         {
//             float val = Mathf.Clamp01(factor);
//             val = EaseManager.EasingFromType(0, 1, val, EaseType.linear);
//             bar1 = bar1from * (1f - val) + bar1to * val;
//         }
//
//         public void SampleBar2(float factor, bool isFinished)
//         {
//             float val = Mathf.Clamp01(factor);
//             val = EaseManager.EasingFromType(0, 1, val, EaseType.linear);
//             bar2 = bar2from * (1f - val) + bar2to * val;
//         }
//
//         public void SampleBar3(float factor, bool isFinished)
//         {
//             float val = Mathf.Clamp01(factor);
//             val = EaseManager.EasingFromType(0, 1, val, EaseType.linear);
//             bar3 = bar3from * (1f - val) + bar3to * val;
//         }
//
//         static public TweenImageBar BeginBar1(GameObject go, float duration, float value)
//         {
//             TweenImageBar comp = go.GetComponent<TweenImageBar>();
//             if (comp == null)
//                 comp = go.AddComponent<TweenImageBar>();
//             comp.bar1Started = false;
//             comp.bar1duration = duration;
//             comp.bar1Factor = 0f;
//             comp.bar1AmountPerDelta = Mathf.Abs(comp.amountPerDeltaBar1);
//             comp.bar1from = comp.bar1;
//             comp.bar1to = value;
//             comp.SetBar1Enable(true);
//
//             if (duration <= 0f)
//             {
//                 comp.SampleBar1(1f, true);
//                 comp.SetBar1Enable(false);
//             }
//             return comp;
//         }
//
//         static public TweenImageBar BeginBar2(GameObject go, float duration, float value)
//         {
//             TweenImageBar comp = go.GetComponent<TweenImageBar>();
//             if (comp == null)
//                 comp = go.AddComponent<TweenImageBar>();
//             comp.bar2Started = false;
//             comp.bar2duration = duration;
//             comp.bar2Factor = 0f;
//             comp.bar2AmountPerDelta = Mathf.Abs(comp.amountPerDeltaBar2);
//             comp.bar2from = comp.bar2;
//             comp.bar2to = value;
//             comp.SetBar2Enable(true);
//
//             if (duration <= 0f)
//             {
//                 comp.SampleBar2(1f, true);
//                 comp.SetBar2Enable(false);
//             }
//             return comp;
//         }
//
//         static public TweenImageBar BeginBar3(GameObject go, float duration, float value)
//         {
//             TweenImageBar comp = go.GetComponent<TweenImageBar>();
//             if (comp == null)
//                 comp = go.AddComponent<TweenImageBar>();
//             comp.bar3Started = false;
//             comp.bar3duration = duration;
//             comp.bar3Factor = 0f;
//             comp.bar3AmountPerDelta = Mathf.Abs(comp.amountPerDeltaBar3);
//             comp.bar3from = comp.bar3;
//             comp.bar3to = value;
//             comp.SetBar3Enable(true);
//
//             if (duration <= 0f)
//             {
//                 comp.SampleBar3(1f, true);
//                 comp.SetBar3Enable(false);
//             }
//             return comp;
//         }
//
//         static public TweenImageBar GetTweenImageBar(GameObject go)
//         {
//             return go.GetComponent<TweenImageBar>();
//         }
//
//         /// <summary>
//         /// 标记为未启动
//         /// </summary>
//         private void OnDisable()
//         {
//             bar1Started = false;
//             bar2Started = false;
//             bar3Started = false;
//         }
//     }
// }