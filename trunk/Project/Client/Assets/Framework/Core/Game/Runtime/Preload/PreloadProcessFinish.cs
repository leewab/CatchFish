using MUGame;

namespace Game
{
    /// <summary>
    /// Finish进程 必须是最后一个注册
    /// </summary>
    public class PreloadProcessFinish : PreloadProcess
    {
        public string PreloadDesc => "结束Preload";
        
        public override void Start()
        {
            base.Log(PreloadDesc);
            this.FinishPreload();
            this.Finish(PreloadDesc);
        }

        private void FinishPreload()
        {
            GameConfig.TickTime(" 预加载结束");
            VersionHelper.Instance.Dispose();
#if UNITY_EDITOR
#if UNITY_TOLUA
            LuaInterface.LuaFileUtils.Instance.beZip = false;
#endif
            GameConfig.IsDebug = true;
#else
#if UNITY_TOLUA
            LuaInterface.LuaFileUtils.Instance.beZip = true;
#endif
            GameConfig.IsDebug = false;
#endif
            DefaultQualityChecking.Instance.StopCpuIOTest();
#if UNITY_TOLUA
            LuaInterface.LuaClient.Instance.bPreload = true;
            LuaModule.Instance.StartLua();
#endif
            GMHandler.Instance.StartServer();
        }
        
    }
}