using Game.Core;

namespace Game
{
    public partial class HandlerModule : BaseModule
    {
        /// <summary>
        /// TimerHandler
        /// </summary>
        private static TimerHandler _TimerHandler;
        public static TimerHandler TimerHandler => _TimerHandler;

    }
}