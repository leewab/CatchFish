using System;
using System.Collections.Generic;
using Game.Core;
using MUEngine;

namespace Game
{
    public  class TimerHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static TimerHandler Instance => HandlerModule.TimerHandler;

        public static bool Pause { get; set; }
        private static List<TimeFunc> funs = new List<TimeFunc>();

        public static List<TimeFunc> Functions { get { return funs; } }
		private static int sIDSeed = 0;


        public static int SetTimeout(Action fun, float time)
        {
            return SetTimeout(fun, time, false, true);
        }

        public static int SetTimeout(Action fun, float time, bool loop, bool canPause )
        {
            if (time > 0)
            {
                int funcIndex = checkDuplicate(fun);
                if (funcIndex >= 0)
                {
                    checkTFInClearListByID(funs[funcIndex].ID);
                    checkTFInClearList(fun);
                    funs[funcIndex].time = time;
                    funs[funcIndex].maxTime = time;
                    funs[funcIndex].loop = loop;
                    funs[funcIndex].canPause = canPause;
                    return funs[funcIndex].ID;
                }
                
				TimeFunc tf = new TimeFunc(fun, time, ++sIDSeed);
                tf.loop = loop;
                tf.canPause = canPause;
                funs.Add(tf);
                //UnityEngine.Debug.Log(string.Format("Add Funs Num:{0}", funs.Count));

                Instance.Enable = true;
                checkTFInClearList(fun);
                return sIDSeed;
            }
            else
            {
                fun();
				return 0;
            }
        }


        public static bool HasAction(Action fun)
        {
            foreach (var i in funs)
            {
                if (i.handler == fun)
                    return true;
            }
            return false;
        }
        
		/// <summary>
		/// 是否有对应id的callback
		/// </summary>
		/// <returns><c>true</c> if has action by I the specified id; otherwise, <c>false</c>.</returns>
		/// <param name="id">Identifier.</param>
		public static bool HasActionByID( int id )
		{
			int len = funs.Count;
			for (int i = 0; i < len; i++)
			{
				if (funs [i].ID == id)
				{
					return true;
				}
			}
			return false;
		}
        
        public static int SetTimeInterval(Action fun, float interval, bool canPause = true)
        {
            if (interval > 0)
            {
                return SetTimeout(fun, interval, true, canPause);
            }
			return 0;
        }

        public static void RemoveTimeaction(Action fun)
        {
            if (!ClearActionList.Contains(fun))
            {
                ClearActionList.Add(fun);
            }
        }

        private static void checkTFInClearList(Action fun)
        {
            if(ClearActionList.Contains(fun))
            {
                ClearActionList.Remove(fun);
            }
        }

        private static void checkTFInClearListByID(int funcId)
        {
            if (ClearIDList.Contains(funcId))
            {
                ClearIDList.Remove(funcId);
            }
        }


        private static List<int> ClearIDList = new List<int>();
        private static List<Action> ClearActionList = new List<Action>();
		/// <summary>
		/// 根据ID删除计时器
		/// </summary>
		/// <param name="id">Identifier.</param>
		public static void RemoveTimeactionByID( int id )
		{
            if (!ClearIDList.Contains(id))
            {
                ClearIDList.Add(id);
            }
            //int len = funs.Count;
            //for (int i = 0; i < len; i++)
            //{
            //    if (funs[i].ID == id)
            //    {
            //        funs.RemoveAt(i);
            //        return;
            //    }
            //}
		}

        public static void ClearAllCanPaused()
        {
            for( int i = 0; i < funs.Count; i ++ )
            {
                if(funs[i].canPause)
                {
                    funs.RemoveAt(i);
                    i--;
                }
            }
        }

        private static int checkDuplicate(Action fun)
        {
            int len = funs.Count;
            for (int i = 0; i < len; i++)
            {
                if (funs[i].handler == fun)
                    return i;
            }
            return -1;
        }

        public override void Update()
        {
            base.Update();

#if !GAME_EDITOR
            float time = MURoot.RealTime.DeltaTime;
            ClearFunc();
            int len = funs.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                if (funs[i].canPause && Pause)
                    continue;
                funs[i].time -= time;
                if (funs[i].time <= 0)
                {
                    if (UnityEngine.Profiling.Profiler.enabled)
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("do timeout action : " + funs[i].handler.Method.Name);
                    }

                    funs[i].handler();

                    if (UnityEngine.Profiling.Profiler.enabled)
                    {
                        UnityEngine.Profiling.Profiler.EndSample();
                    }
                    if(funs[i].loop)
                    {
                        funs[i].Reset();
                    }
                    else
                    {
                        //Events.Common.NextFrameExecute += funs[i].handler;
                        funs.RemoveAt(i);
                    }
                }
            }
            if (funs.Count == 0)
                Enable = false;
#endif
        }

        private static void ClearFunc()
        {
            for (int i = 0; i < ClearActionList.Count; i++)
            {
                for (int i2 = funs.Count - 1; i2 >= 0; i2--)
                {
                    if (funs[i2].handler == ClearActionList[i])
                    {
                        funs.RemoveAt(i2);
                        break;
                    }
                }
            }
            ClearActionList.Clear();

            for (int i = 0; i < ClearIDList.Count; i++)
            {
                for (int i2 = funs.Count - 1; i2 >= 0; i2--)
                {
                    if (funs[i2].ID == ClearIDList[i])
                    {
                        funs.RemoveAt(i2);
                        break;
                    }
                }
            }
            ClearIDList.Clear();
        }

        public static string GetDoubleTimer(int time)
        {
            if (time > 9)
            {
                return time.ToString();
            }
            else
            {
                return "0" + time.ToString();
            }
        }
    }

    public class TimeFunc
    {
        public Action handler;
        public float time;
        public float maxTime;
        public bool loop;
        public bool canPause;
		public int ID;
		public TimeFunc(Action fun, float tm, int id)
        {
            handler = fun;
            time = tm;
            maxTime = time;
            loop = false;
            canPause = true;
			ID = id;
        }

        public void Reset()
        {
            time = maxTime;
        }
    }
}
