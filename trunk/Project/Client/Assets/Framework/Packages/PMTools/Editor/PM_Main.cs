using UnityEditor;

namespace Framework.PM
{
    internal class PM_Main
    {
#if UNITY_EDITOR
        /// <summary>
        /// PM 上传面板
        /// </summary>
        [MenuItem("Assets/PluginManager/UploadPlugin")]
        public static void UploadPluginMain()
        {
            ProtocolManager.RegisterProtocol();
            var uploadWin = EditorWindow.GetWindow<PM_UploadWindow>(true, "上传页");  //WithRect  new Rect(new Vector2(x: 200, y: 200), new Vector2(300, 400)), true, "UploadPlugins"
        }

        /// <summary>
        /// PM 预览面板
        /// </summary>
        [MenuItem("Game/Tools/PM/Window")]
        public static void PMDownloadMain()
        {
            ProtocolManager.RegisterProtocol();
            var dis = EditorWindow.GetWindow<PM_DisplayWindow>(true, "浏览页");
        }

        /// <summary>
        /// 是否启用本地服务器
        /// </summary>
        private static bool mIsLocalServer = true;

        public static bool IsLocalServer
        {
            get { return mIsLocalServer; }
        }
        
        [MenuItem("Game/Tools/PM/IsLocalServer")]
        private static void SwitchSimulationMode()
        {
            mIsLocalServer = !mIsLocalServer;
        }

        [MenuItem("Game/Tools/PM/IsLocalServer", true)]
        private static bool SwitchSimulationModeValidate()
        {
            Menu.SetChecked("Game/Tools/PM/IsLocalServer", mIsLocalServer);
            return true;
        }        
#endif
    }
}