using System;
using Game.Core;
using MUEngine;
using UnityEngine;

namespace Game
{
    public class GameModule : BaseModule
    {
        public static GameModule Instance => ModuleMgr.GetModule<GameModule>();
        
        public string RemoteGMCmd { get; set; }
        
        private bool bEngingeReady = false;
        public bool EngineReady => bEngingeReady;
        
        private int mLastFps;
        private int mFrameCount;
        private bool isLowFPS = false;
        private bool lowFPSAlertDispatched = false;
        private const int LowFPSThreshold = 20;
        private DateTime mLastFrameTime = DateTime.Now;
        private DateTime mLastLowFPSTime = DateTime.Now;
        private DateTime mLastLogicUpdateFrameTime = DateTime.Now;
        public Func<bool> OnLowFPSAlert { get; set; }
        private DateTime mLastHighFPSTime = DateTime.Now;
        private int mLastLogicFPS;
        private int mLogicFrameCount = 0;

        public override void Init()
        {
            base.Init();
            // AssetBundle缓存清理
            Caching.expirationDelay = 7 * 24 * 60 * 60;
            EngineDelegate.DefaultMoveSpeed = 5;
            WWWUtil.DisableLoadCache = false;
            GameState.EnterState(States.INIT);
        }

        public override void Start()
        {
            base.Start();
            D.InitLogCallback();
            SDKHandler.Instance.Init();
            NextFrameUpdate(StartEngine);
        }
        
        public override void RegisterEvent()
        {
            CoreDelegate.PrefabUnInstantiateRule += UnInstantiateRule;
        }

        public override void UnRegisterEvent()
        {
            CoreDelegate.PrefabUnInstantiateRule -= UnInstantiateRule;
        }

        public override void Update()
        {
            base.Update();
            CheckFPS();
        }

        private void CheckFPS()
        {
            DateTime now = DateTime.Now;
            mFrameCount++;
            float secs = (float)(now - mLastFrameTime).TotalSeconds;
            if (secs >= 1)
            {
                mLastFps = (int)(mFrameCount / secs);
                mFrameCount = 0;
                mLastFrameTime = now;
            }
            if (secs > 2)
            {
                isLowFPS = false;
            }
            if (!lowFPSAlertDispatched)
            {
                if (isLowFPS)
                {
                    if (mLastFps < LowFPSThreshold)
                    {
                        if ((now - mLastLowFPSTime).TotalSeconds > 30)
                        {
                            if (OnLowFPSAlert.SafeInvoke())
                                lowFPSAlertDispatched = true;
                            else
                                isLowFPS = false;
                        }
                    }
                    else
                    {
                        isLowFPS = false;
                        mLastHighFPSTime = now;
                    }
                }
                else
                {
                    if (mLastFps < LowFPSThreshold)
                    {
                        isLowFPS = true;
                        mLastLowFPSTime = now;
                    }
                }
            }
        }

        private bool UnInstantiateRule(GameObject obj)
        {
            if (obj is AnimationClip || obj is Texture || obj is AudioClip || obj is Sprite)
            {
                return true;
            }
            //if (obj.name == "UI-Loading") return true;   // hard code (temp)
            if (obj.name == "EasyTouchRoot") return true;   // hard code (temp)
            // TODO:暂时注释的
            // return obj.GetComponent<GOGUI.ImageFont>();
            return false;
        }
        
        private void StartEngine()
        {
            MURoot.Start(Main.Instance.gameObject, this.OnEngineReady);
        }
        
        private void OnEngineReady()
        {
            LogHelper.Log("引擎--启动完成");
            bEngingeReady = true;
            
            ProcessModule.AddProcess(new UIProcess());
#if UNITY_EDITOR
            ProcessModule.AddProcess(new EditorProcess());
#endif
            // ProcessModule.AddProcess(new EffectProcess());
            // ProcessModule.AddProcess(new EasyTouchProcess());
            
            ResConfig.GotAssetAction = GetAsset;
            
            NextFrameUpdate(OnQualitySetting);
            NextFrameUpdate(OnEngineSetting);
            NextFrameUpdate(OnHandlerSetting);
        }

        private void OnQualitySetting()
        {
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            MUEngine.MURoot.ResMgr.SetPreferedCacheSize(512 * 1024 * 1024);
            Caching.maximumAvailableDiskSpace = 128 * 1024 * 1024;
#else
            Caching.maximumAvailableDiskSpace = 1024 * 1024 * 1024;
            MURoot.ResMgr.SetPreferedCacheSize(128 * 1024 * 1024);
#endif
            DeviceModule.Instance.NotifyEngineDevicePerformance();
        }

        private void OnEngineSetting()
        {
            MUEngine.MURoot.ResMgr.ScenePoolCount = 0;
            //设置AssetBundle的WWW缓存为256MB
            MUEngine.MURoot.ResMgr.SetLowAsyncLoadPriority(false);
#if UNITY_STANDALONE_WIN || UNITY_ANDROID
            WWWUtil.DisableLoadCache = true;
#endif
            // 离开场景启动GC
            // MUEngine.MURoot.Scene.OnLeaveScene += OnLeaveScene;
        }

        private void OnHandlerSetting()
        {
            // HandlerModule启动
            HandlerModule.Instance.InitHandler();
            MUGUI.GOGUITools.GetAssetAction = MUEngine.MURoot.ResMgr.GetAsset;
            MUGUI.GOGUITools.ReleaseAssetAction = MUEngine.MURoot.ResMgr.ReleaseAsset;
            // State Change
            GameState.EnterState(States.PRELOAD);
        }
        
        private void GetAsset(string name, Action<string, UnityEngine.Object> callback, int priority)
        {
            MUEngine.MURoot.ResMgr.GetAsset(name, callback, (LoadPriority)priority);
        }

        private void OnLeaveScene()
        {
#if UNITY_TOLUA
            LuaModule.Instance.DoStepGC();
#endif
        }

     
    }
}