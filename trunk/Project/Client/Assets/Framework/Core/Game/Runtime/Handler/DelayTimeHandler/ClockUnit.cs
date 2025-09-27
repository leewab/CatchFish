using System;
using UnityEngine;

namespace Game
{
    public class ClockUnit
    {
        private bool isStart;               //是否开启
        private float curTime;              //当前时间
        private float totalTime;            //总时长
        
        /// <summary>
        /// 时间监听
        /// </summary>
        public Action OnTimeEnd;
        public Action OnTimeStart;
        public Action<float> OnTimeUpdate;

        public ClockUnit(float time, Action endCallBack, Action startCallBack = null, Action<float> updateCallBack = null)
        {
            this.curTime = 0;
            this.totalTime = 0;
            this.isStart = false;
            this.totalTime = time;
            this.OnTimeEnd = endCallBack;
            this.OnTimeStart = startCallBack;
            this.OnTimeUpdate = updateCallBack;
        }
        
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="time"></param>
        public void Start()
        {
            isStart = true;
            OnTimeStart?.Invoke();
        }
        
        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            if (!isStart) return;
            curTime += Time.deltaTime;
            OnTimeUpdate?.Invoke(curTime);
            if (curTime >= totalTime)
            {
                isStart = false;
                OnTimeEnd?.Invoke();
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Reset();
            this.OnTimeEnd = null;
            this.OnTimeStart = null;
            this.OnTimeUpdate = null;
        }

        /// <summary>
        /// Reset
        /// </summary>
        private void Reset()
        {
            curTime = 0;
            totalTime = 0;
            isStart = false;
        }
    }
}