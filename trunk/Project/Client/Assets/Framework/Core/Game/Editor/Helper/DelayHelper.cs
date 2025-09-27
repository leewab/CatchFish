using System;
using System.Collections.Generic;

namespace Game.UI
{
    public class DelayHelper
    {
        private static Dictionary<string, TimeData> mTimeDelayDic = new Dictionary<string, TimeData>();

        public void AddDelayListener(string key, TimeData timeData)
        {
            if (!mTimeDelayDic.ContainsKey(key))
            {
                mTimeDelayDic.Add(key, timeData);
            }
        }

        public void RemoveDelayListener(string key)
        {
            mTimeDelayDic.Remove(key);
            if (mTimeDelayDic.Count <= 0) RemoveAllDelayListener();
        }

        public void RemoveAllDelayListener()
        {
            mTimeDelayDic.Clear();
        }

        public void Update()
        {
            if (mTimeDelayDic.Count > 0)
            {
                List<string> timeKeyList = new List<string>(mTimeDelayDic.Keys);
                for (int i = 0; i < timeKeyList.Count; i++)
                {
                    if(timeKeyList[i] == null || mTimeDelayDic[timeKeyList[i]] == null) continue;
                    mTimeDelayDic[timeKeyList[i]].DelayTime -= mTimeDelayDic[timeKeyList[i]].StepTime;
                    mTimeDelayDic[timeKeyList[i]].UpdateCallback?.Invoke(mTimeDelayDic[timeKeyList[i]].DelayTime);
                    if (mTimeDelayDic[timeKeyList[i]].DelayTime <= 0)
                    {
                        mTimeDelayDic[timeKeyList[i]].FinishCallback?.Invoke();
                        RemoveDelayListener(timeKeyList[i]);
                    }
                }
            }
        }
    }
    
    public class TimeData
    {
        public float StepTime = 0.1f;
        public float DelayTime = 0f;
        public Action FinishCallback;
        public Action<float> UpdateCallback;

        public TimeData(float delayTime, Action finishCallback)
        {
            DelayTime = delayTime;
            FinishCallback = finishCallback;
        }
        
        public TimeData(float delayTime, Action finishCallback, float stepTime = 0.1f)
        {
            StepTime = stepTime;
            DelayTime = delayTime;
            FinishCallback = finishCallback;
        }
        
        public TimeData(float delayTime, Action finishCallback, Action<float> updateCallback = null)
        {
            DelayTime = delayTime;
            FinishCallback = finishCallback;
            UpdateCallback = updateCallback;
        }
        
        public TimeData(float delayTime, Action finishCallback, Action<float> updateCallback = null, float stepTime = 0.1f)
        {
            StepTime = stepTime;
            DelayTime = delayTime;
            FinishCallback = finishCallback;
            UpdateCallback = updateCallback;
        }

        public void Update()
        {
            
        }
    }
}