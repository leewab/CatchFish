namespace Game.Core
{
    /// <summary>
    /// BaseHandler 处理器模块，用于管理基础小模块
    /// 生命周期由Module管理
    /// </summary>
    public class BaseHandler : BaseModule
    {
        public BaseHandler()
        {
            this.Init();
        }

        public override bool Enable
        {
            get => base.Enable;
            set => base.Enable = value;
        }

        public virtual bool InGameState
        {
            get => this.Enable && EnableInCurrentState(GameState.CurrentState);
        }

        /// <summary>
        /// 响应游戏状态变更
        /// </summary>
        /// <param name="newState">新的游戏状态</param>
        public virtual void OnGameStateChanged(States newState)
        {
            
        }

        /// <summary>
        /// 检测在当前游戏状态下是否可用
        /// </summary>
        /// <param name="currentState">当前游戏状态</param>
        /// <returns>模块在当前游戏状态下是否可用</returns>
        protected virtual bool EnableInCurrentState(States currentState)
        {
            return false;
        }
      
    }
}