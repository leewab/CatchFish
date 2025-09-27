using System;
using System.Collections.Generic;
using Game.Core;
using MUGame;

namespace Game
{
    public partial class HandlerModule : BaseModule
    {

#region Handler注册

        public static HandlerModule Instance => ModuleMgr.GetModule<HandlerModule>();

        /// <summary>
        /// PreloadHandler
        /// </summary>
        private static PreloadHandler _PreloadHandler;
        public static PreloadHandler PreloadHandler => _PreloadHandler;

        /// <summary>
        /// LoadingHandler
        /// </summary>
        private static LoadingHandler _LoadingHandler;
        public static LoadingHandler LoadingHandler => _LoadingHandler;

        /// <summary>
        /// UpdateHandler
        /// </summary>
        private static UpdateHandler _UpdateHandler;
        public static UpdateHandler UpdateHandler => _UpdateHandler;

        /// <summary>
        /// NetHandler
        /// </summary>
        private static NetHandler _NetHandler;
        public static NetHandler NetHandler => _NetHandler;

        /// <summary>
        /// GuideMod
        /// </summary>
        private static GuideMod _GuideMod;
        public static GuideMod GuideMod => _GuideMod;

        /// <summary>
        /// ManualControlCameraHandler
        /// </summary>
        private static ManualControlCameraHandler _ManualControlCameraHandler;
        public static ManualControlCameraHandler ManualControlCameraHandler => _ManualControlCameraHandler;

        /// <summary>
        /// AlertInfoHandler
        /// </summary>
        private static AlertInfoHandler _AlertInfoHandler;
        public static AlertInfoHandler AlertInfoHandler => _AlertInfoHandler;

        /// <summary>
        /// GMHandler
        /// </summary>
        private static GMHandler _GMHandler;
        public static GMHandler GMHandler => _GMHandler;

        #endregion

        /// <summary>
        /// Handler管理
        /// </summary>
        private static List<BaseHandler> mHandlerAllList = new List<BaseHandler>();

        public T RegisterHandler<T>() where T : BaseHandler, new()
        {
            string fullName = typeof(T).FullName;
            if (!string.IsNullOrEmpty(fullName))
            {
                T t = new T();
                mHandlerAllList.Add(t);
                return t;
            }

            throw new Exception("Add Handler 异常!");
        }

        public bool RemoveHandler(BaseHandler baseHandler)
        {
            if (baseHandler == null) return false;
            if (mHandlerAllList == null) return true;
            return mHandlerAllList.Remove(baseHandler);
        }

        public void InitHandler()
        {
            _PreloadHandler = RegisterHandler<PreloadHandler>();
            _LoadingHandler = RegisterHandler<LoadingHandler>();
            _UpdateHandler = RegisterHandler<UpdateHandler>();

            _NetHandler = RegisterHandler<NetHandler>();
            //HandlerMgr.AddHandler<new InputMod>();
            // HandlerMgr.AddHandler<new PreLoadMod>();
            //HandlerMgr.AddHandler<WindowManager.Instance);
            _TimerHandler = RegisterHandler<TimerHandler>();
            // AddHandler<TaskMod>();
            _GuideMod = RegisterHandler<GuideMod>();
            // _GameMod = RegisterHandler<GameMod>();
            _ManualControlCameraHandler = RegisterHandler<ManualControlCameraHandler>();
            // HandlerMgr.AddHandler<LuaInputMod>();
            // _WeatherSysHandler = RegisterHandler<WeatherSysHandler>();
            _AlertInfoHandler = RegisterHandler<AlertInfoHandler>();
            _GMHandler = RegisterHandler<GMHandler>();
        }

        public override void RegisterEvent()
        {
            GameState.ActionGameStateChanged += OnGameStateChanged;
        }
        
        public override void UnRegisterEvent()
        {
            GameState.ActionGameStateChanged -= OnGameStateChanged;
        }

        public override void Update()
        {
            base.Update();
            if (mHandlerAllList == null) return;
            for (int i = 0; i < mHandlerAllList.Count; i++)
            {
                if (mHandlerAllList[i] != null) mHandlerAllList[i]?.Update();
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (mHandlerAllList == null) return;
            for (int i = 0; i < mHandlerAllList.Count; i++)
            {
                if (mHandlerAllList[i] != null) mHandlerAllList[i]?.LogicUpdate();
            }
        }

        public override void Dispose()
        {
            if (mHandlerAllList == null) return;
            for (int i = 0; i < mHandlerAllList.Count; i++)
            {
                if (mHandlerAllList[i] != null)
                {
                    mHandlerAllList[i]?.Dispose();
                    RemoveHandler(mHandlerAllList[i]);
                }
            }
        }
        
        /// <summary>
        /// 响应游戏状态变更
        /// </summary>
        public virtual void OnGameStateChanged(States gameState)
        {
            if (mHandlerAllList == null || mHandlerAllList.Count == 0) return;
            for (int i = 0; i < mHandlerAllList.Count; i++)
            {
                mHandlerAllList[i]?.OnGameStateChanged(gameState);
            }
        }
        
    }
}