namespace Game
{
    /// <summary>
    /// 消息枚举类
    /// </summary>
    public static class MsgEvent
    {
        #region 游戏管理类事件枚举

        public enum HotfixEvent
        {
            _Top = 1,
            Begin,
            Complete,
            _End
        }

        #endregion

        public enum TestEvent
        {
            _Top = HotfixEvent._End,
            
            _End
        }
    }
}