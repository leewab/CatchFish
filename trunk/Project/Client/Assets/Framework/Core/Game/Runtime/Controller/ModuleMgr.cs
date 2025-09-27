using System;
using System.Collections.Generic;
using Game.Core;

namespace Game
{
    public class ModuleMgr
    {
        // Module执行优先级
        private static int _Priority = 0;
        
        /// <summary>
        /// Handler管理
        /// </summary>
        private static List<BaseModule> mModuleList = new List<BaseModule>();
        
        public static T AddModule<T>() where T : BaseModule, new()
        {
            string fullName = typeof(T).FullName;
            if (!string.IsNullOrEmpty(fullName))
            {
                BaseModule t = new T();
                t.Priority = _Priority++;
                mModuleList.Add(t);
                return (T)t;
            }

            throw new Exception("Add Handler 异常!");
        }

        public static T GetModule<T>() where T : BaseModule, new()
        {
            foreach (var baseModule in mModuleList)
            {
                if (baseModule is T module) return module;
            }

            return AddModule<T>();
        }

        public static bool RemoveModule(BaseModule baseModule)
        {
            if (baseModule == null) return false;
            if (mModuleList == null) return true;
            return mModuleList.Remove(baseModule);
        }
        
        public static void InitModule()
        {
            AddModule<GameModule>();
            AddModule<DeviceModule>();
            AddModule<VersionModule>();
#if UNITY_TOLUA
            AddModule<LuaModule>();
#endif
            if (mModuleList != null)
            {
                mModuleList.Sort((a, b) => a.Priority.CompareTo(a.Priority));
            }
        }

        public static void Start()
        {
            if (mModuleList == null) return;
            for (int i = 0; i < mModuleList.Count; i++)
            {
                if (mModuleList[i] != null)
                {
                    mModuleList[i].Start();
                }
            }
        }

        //限定逻辑帧率
        private const float MaxLogicUpdateFrameTime = 0.03f;//逻辑调用的最大帧率
        private static DateTime mLastLogicUpdateTime = DateTime.Now;
        
        public static void Update()
        {
            if (mModuleList == null) return;
            for (int i = 0; i < mModuleList.Count; i++)
            {
                if (mModuleList[i] != null)
                {
                    var mModule = mModuleList[i];
                    if (mModule != null)
                    {
                        ProfilerHelper.BeginSample("update module : " + mModuleList[i].GetType().ToString());
                        mModule.Update();
                        ProfilerHelper.EndSample();
                    }
                }
            }

            DateTime now = DateTime.Now;        
            float secs = (float)(now - mLastLogicUpdateTime).TotalSeconds;
            if (secs >= MaxLogicUpdateFrameTime)
            {
                // NextFrame
                // ExecuteNextFrameAction();
                // Handler.LogicUpdate()
                for (int i = 0; i < mModuleList.Count; i++)
                {
                    var mModule = mModuleList[i];
                    if (mModule != null)
                    {
                        ProfilerHelper.BeginSample("LogicUpdate module : " + mModuleList[i].GetType().ToString());
                        mModule.LogicUpdate();
                        ProfilerHelper.EndSample();
                    }
                }
                mLastLogicUpdateTime = now;
            }
        }
        
        public static void OnApplicationPause(bool pauseStatus)
        {
#if UNITY_TOLUA
            LuaModule.OnApplicationPause(pauseStatus);
#endif
        }
        
        public static void OnApplicationFocus(bool hasFocus)
        {
#if UNITY_TOLUA
            LuaModule.OnApplicationFocus(hasFocus);
#endif
        }

        public static void OnApplicationQuit()
        {
#if UNITY_TOLUA
            LuaModule.OnApplicationQuit();
#endif
            D.Dispose();
            ModuleMgr.Dispose();
        }

        public static void Dispose()
        {
            if (mModuleList == null) return;
            for (int i = 0; i < mModuleList.Count; i++)
            {
                if (mModuleList[i] != null)
                {
                    mModuleList[i]?.Dispose();
                    RemoveModule(mModuleList[i]);
                }
            }
        }
        
        private static void ExecuteNextFrameAction()
        {
            if (!Common.NextFrameExecute.IsNull())
            {
                Action action = Common.NextFrameExecute;
                Common.NextFrameExecute.Dispose();
                action();
            }
        }
        
    }
}