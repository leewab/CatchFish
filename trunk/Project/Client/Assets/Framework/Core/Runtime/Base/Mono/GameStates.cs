namespace Game.Core
{
    public static class GameState
    {
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        private static States _mCurStates = States.NONE;
        public static States CurrentState 
        {
            get => _mCurStates;
            private set => _mCurStates = value;
        }

        /// <summary>
        /// 游戏状态变更
        /// </summary>
        public static SafeAction<States> ActionGameStateChanged;

        public static void EnterState(States state)
        {
            if (CurrentState == state)
            {
                return;
            }

            LogHelper.Log("State => " + state.ToString());
            CurrentState = state;
            Common.NextFrameExecute += OnGameStateChanged;
        }

        private static void OnGameStateChanged()
        {
            ActionGameStateChanged.SafeInvoke(CurrentState);
        }
        
    }
}
