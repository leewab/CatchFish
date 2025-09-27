
using System;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// BaseModule 所有模块的基类，
    /// 模块的设立意义在于，独立划分游戏模块，各自管理游戏模块，
    /// 但是其必须在同一个生命周期的驱动下执行，避免各个模块之间的不同步执行
    /// 1.初始化Init
    /// 2.时间注册取消 Register/UnRegister
    /// 3.每帧更新、逻辑帧更新 Update/LogicUpdate
    /// 4.销毁释放 Dispose
    /// 5.下一帧执行 下一逻辑帧执行 NextUpdate/NextLogicUpdate
    /// </summary>
    public abstract class BaseModule : ILife
    {
        // 是否初始化
        private bool _IsInit = false;
        // 下一帧执行事件
        private Queue<Action> _NextFrameUpdateAction;
        private Queue<Action> _NextFrameLogicUpdateAction;
        
        // 执行优先级
        private int _Priority = 0;
        public int Priority
        {
            get => _Priority;
            set  => _Priority = value;
        }
        
        // 是否启用
        private bool _Enable = true;
        public virtual bool Enable
        {
            get => _Enable;
            set
            {
                if (_Enable == value) return;
                _Enable = value;
                OnEnableChanged();
            }
        }
 
        public BaseModule()
        {
            this.Init();
        }
        
        /// <summary>
        /// Init
        /// </summary>
        public virtual void Init()
        {
            _IsInit = true;
            RegisterEvent();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            _IsInit = false;
            UnRegisterEvent();
        }

        public virtual void RegisterEvent() { }
        public virtual void UnRegisterEvent() { }

        public virtual void Start() { }

        /// <summary>
        /// 无限制执行帧率的Update
        /// </summary>
        public virtual void Update()
        {
            if (!_IsInit) return;
            ExecuteNextFrameUpdate();
        }

        /// <summary>
        /// 限制执行帧率的逻辑Update
        /// </summary>
        public virtual void LogicUpdate()
        {
            if (!_IsInit) return;
            ExecuteNextFrameLogicUpdate();
        }

        public virtual void FixedUpdate() { }

        public virtual void LateUpdate() { }

        protected virtual void OnEnableChanged() { }
        
        /// <summary>
        /// 下一帧执行
        /// </summary>
        /// <param name="action"></param>
        protected virtual void NextFrameUpdate(Action action)
        {
            if (_NextFrameUpdateAction == null) _NextFrameUpdateAction = new Queue<Action>();
            _NextFrameUpdateAction.Enqueue(action);
        }
        
        /// <summary>
        /// 下一监管帧执行
        /// </summary>
        /// <param name="action"></param>
        protected virtual void NextFrameLogicUpdate(Action action)
        {
            if (_NextFrameLogicUpdateAction == null) _NextFrameLogicUpdateAction = new Queue<Action>();
            _NextFrameLogicUpdateAction.Enqueue(action);
        }
        
        private void ExecuteNextFrameUpdate()
        {
            if (_NextFrameUpdateAction == null || _NextFrameUpdateAction.Count == 0) return;
            _NextFrameUpdateAction.Dequeue().SafeInvoke();
        }
        
        private void ExecuteNextFrameLogicUpdate()
        {
            if (_NextFrameLogicUpdateAction == null || _NextFrameLogicUpdateAction.Count == 0) return;
            _NextFrameLogicUpdateAction.Dequeue().SafeInvoke();
        }
        
    }
}