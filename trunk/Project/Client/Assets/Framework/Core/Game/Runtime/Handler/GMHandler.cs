using Game.Core;

namespace Game
{
    public class GMHandler : BaseHandler
    {
        public static GMHandler Instance => HandlerModule.GMHandler;

        public static bool IsOpen = false;

        public override void Init()
        {
            base.Init();
            if (GameConfig.IsDebug || !GameConfig.IsVersionUpdate) IsOpen = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            this.StopServer();
        }

        public void StartServer()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (IsOpen)
            {
                RemoteGMServer.StartServer();
            }
#endif
        }

        public void StopServer()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            RemoteGMServer.StopServer();
#endif
        }
        
    }
}