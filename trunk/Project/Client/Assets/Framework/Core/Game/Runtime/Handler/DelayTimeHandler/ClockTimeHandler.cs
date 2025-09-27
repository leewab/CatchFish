using System.Collections.Generic;
using Game;
using Game.Core;

namespace Game
{
    public class ClockTimeHandler : MonoSingleton<ClockTimeHandler>
    {
        // 是否运行DelayTimeController
        private bool _IsRun = true;
        // 最大时钟数量
        private int _MaxClockTimeNum = 50;

        private void Update()
        {
            if (!_IsRun) return;
            if (clockTimes == null) return;
            if (clockTimes.Count > _MaxClockTimeNum)
            {
                LogHelper.Error("查看是否有ClockTime时钟没有在Dictionary中释放，数量超过了最大值！");
                _IsRun = false;
            }
            else
            {
                foreach (var clockTime in clockTimes.Values)
                {
                    clockTime?.Update();
                }  
            }
        }

        #region Clock计时器

        private Dictionary<string, ClockUnit> clockTimes = null;

        public void RegisterClock(string clockName, ClockUnit clock)
        {
            if (clockTimes == null) clockTimes = new Dictionary<string, ClockUnit>();
            if (!clockTimes.ContainsKey(clockName))
            {
                clockTimes.Add(clockName, clock);
                clock.OnTimeEnd += () =>
                {
                    UnRegisterClock(clockName);
                };
            }
            else
            {
                LogHelper.Error("计时器注册出现相同key值或连续多次注册 ：" + clockName);
            }
        }

        public void UnRegisterClock(string clockName)
        {
            if (clockTimes == null) return;
            ClockUnit clockUnit = null;
            if (clockTimes.TryGetValue(clockName, out clockUnit))
            {
                clockTimes?.Remove(clockName);
                clockUnit?.Dispose();
            }
        }

        public void UnAllRegisterClock()
        {
            if (clockTimes == null) return;
            foreach (var clockTime in clockTimes)
            {
                UnRegisterClock(clockTime.Key);
            }
            clockTimes.Clear();
            clockTimes = null;
        }

        #endregion
    }
}