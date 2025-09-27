using System;
using Game.Core;
using UnityEngine;

namespace Game
{
    public partial class Main : MonoSingleton<Main>
    {

#if UNITY_EDITOR
        [SerializeField]
        public bool LoadFromPrefab = false;
#endif
        
        #region 单例生命周期

        protected override void OnSingletonInit()
        {
            base.OnSingletonInit();
            
            LogHelper.Log("<<=== InitDefineCtrl ==>>");
            if (IntPtr.Size == 4)
            {
                LogHelper.Log("Run in 32-bit");
            }
            else if (IntPtr.Size == 8)
            {
                LogHelper.Log("Run in 64-bit");
            }
        }

        protected override void OnSingletonDispose()
        {
            ModuleMgr.Dispose();
        }

        #endregion

        #region MonoBehaviour生命周期

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            LogHelper.Log("引擎--开始启动 Start");
            InitModule();
            InitController();
            ModuleMgr.Start();
        }

        private void Update()
        {
            ModuleMgr.Update();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            ModuleMgr.OnApplicationPause(pauseStatus);
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            ModuleMgr.OnApplicationFocus(hasFocus);
        }

        private void OnApplicationQuit()
        {
            ModuleMgr.OnApplicationQuit();
        }
        
        #endregion

        private void InitModule()
        {
            ModuleMgr.InitModule();
        }

        private void InitController()
        {
#if UI_RAYCAST
            AddController<UIRaycastController>();
#endif
        }

        #region 调用协程

        public Action InvokeAction = null;
        
        public void StartInvoke(Action action, float time)
        {
            Invoke(nameof(InvokeFunction), time);
        }
        
        private void InvokeFunction()
        {
            InvokeAction?.Invoke();
            InvokeAction = null;
        }

        #endregion
        
    }

    public partial class Main
    {
        
#region Handler 游戏逻辑处理器

        ///// <summary>
        ///// LoadingHandler
        ///// </summary>
        //private LoadingHandler mLoadingHandler = null;
        //public LoadingHandler _LoadingHandler
        //{
        //    get
        //    {
        //        if (mLoadingHandler == null) mLoadingHandler = HandlerModule.RegisterHandler<LoadingHandler>();
        //        return mLoadingHandler;
        //    }
        //}
        
        ///// <summary>
        ///// AlertInfoHandler
        ///// </summary>
        //private AlertInfoHandler mAlertInfoHandler = null;
        //public AlertInfoHandler _AlertInfoHandler
        //{
        //    get
        //    {
        //        if (mAlertInfoHandler == null) mAlertInfoHandler = HandlerModule.RegisterHandler<AlertInfoHandler>();
        //        return mAlertInfoHandler;
        //    }
        //}

        
        ///// <summary>
        ///// LuaHandler
        ///// </summary>
        //private LuaHandler mLuaHandler = null;
        //public LuaHandler _LuaHandler
        //{
        //    get
        //    {
        //        if (mLuaHandler == null) mLuaHandler = HandlerModule.RegisterHandler<LuaHandler>();
        //        return mLuaHandler;
        //    }
        //}
        
        ///// <summary>
        ///// 更新处理器
        ///// </summary>
        //private UpdateHandler mUpdateHandler= null;
        //public UpdateHandler _UpdateHandler
        //{
        //    get
        //    {
        //        if (mUpdateHandler == null) mUpdateHandler = HandlerModule.RegisterHandler<UpdateHandler>();
        //        return mUpdateHandler;
        //    }
        //}
        
        ///// <summary>
        ///// GM处理器
        ///// </summary>
        //private GMHandler mGMHandler= null;
        //public GMHandler _GMHandler
        //{
        //    get
        //    {
        //        if (mGMHandler == null) mGMHandler = HandlerModule.RegisterHandler<GMHandler>();
        //        return mGMHandler;
        //    }
        //}
        
#endregion

#region Controller 
        
        #region 添加BaseMono派生组件

        public T AddController<T>() where T : BaseMono
        {
            return gameObject.AddOneComponent<T>();;
        }

        public void RemoveController<T>() where T : BaseMono
        {
            gameObject.RemoveComponent<T>();
        }
        
        #endregion
        

        /// <summary>
        /// SoundCtrl
        /// </summary>
        private SoundController mSoundCtl = null;
        public SoundController SoundCtrl
        {
            get
            {
                if (mSoundCtl == null) mSoundCtl = AddController<SoundController>();
                return mSoundCtl;
            }
        }

        /// <summary>
        /// SplashController 片头动画
        /// </summary>
        private SplashController mSplashCtrl = null;
        public SplashController SplashCtrl
        {
            get
            {
                if (mSplashCtrl == null) mSplashCtrl = AddController<SplashController>();
                return mSplashCtrl;
            }
        }
        
#endregion

    }
}
