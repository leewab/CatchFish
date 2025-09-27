using UnityEngine;

// namespace Game
// {
    public class GameConfig
    {
        /// <summary>
        /// ServerList名称
        /// </summary>
        public const string SERVER_FILE = "REMAIN-ServerList.bytes";
        /// <summary>
        /// 配置表名称
        /// </summary>
        public const string GAME_CONF_BUNDLE_NAME = "ConfData.bytes";
        
        /// <summary>
        /// 是否加载Prefab
        /// </summary>
        public static bool LoadFromPrefab = false;
        
        /// <summary>
        /// 是否热更
        /// </summary>
        public static bool IsVersionUpdate = false;
        
        /// <summary>
        /// 是否开启推荐特殊处理
        /// </summary>
        public static bool OpenRecommender = false;
        
        /// <summary>
        /// 是否Debug模式
        /// </summary>
        public static bool IsDebug;

        /// <summary>
        /// 是否是线上正式PC
        /// </summary>
        public static bool IsOnlinePC = true;
        
        /// <summary>
        /// 是否使用新proto协议
        /// </summary>
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
        public static bool UseNewProtoBuf = true;
#else
        public static bool UseNewProtoBuf = false;
#endif
        //在设置完成BaseUIProportion之后强制进行一次适配操作
        public static bool ForceLayoutUI = false;
        //dof
        public static bool EnableDof = true;
        
        // 资源标记后缀，为空则表示资源名称不变
        public static string ResSignSuffix = "";

        
        
        /// <summary>
        /// 是佛开启Tolua
        /// </summary>
        public static bool ToLuaIsEnable
        {
            get
            {
#if UNITY_TOLUA
                return true;
#else
                return false;
#endif
            }
        }
        
        /// <summary>
        /// 是佛开启Tolua
        /// </summary>
        public static bool ToLuaIsLoadAB
        {
            get
            {
#if UNITY_EDITOR
                return false;
#else
                return true;
#endif
            }
        }
        
        
        //为了适配屏幕有安全区的设置
        public static Vector2 ScreenLeftUpPoint = Vector2.zero;
        public static Vector2 ScreenRightDownPoint = Vector2.zero;
        
        public static string serverIP = "192.168.90.69";
        public static string serverID = "";
        public static string serverName = "";
        public static int serverPort = 10188;
        
        public static bool TryEndConnect = true;
        
        //是否热变小包
        public static bool IsReBianPackage = false;
        
        public const string UI_PREFAB_NAME = "ui.bundle";
        public const string GAME_ACT_BUNDLE_NAME = "act.bundle";
        public const string GAME_VIDEO_BUNDLE_NAME = "video_Open.bundle";
        public const string GAME_LUA_BUNDLE_NAME = "lua.bundle";
        public const string GAME_LUA64_BUNDLE_NAME = "lua64.bundle";
        public const string GAME_MESSAGE_BUNDLE_NAME = "lua_message_pb.bundle";
        public const string GAME_SHADER_BUNDLE_NAME = "shadervariants_shadervariants.bundle";
        /// <summary>
        /// 默认UI字体Bundle名称
        /// </summary>
        public const string GAME_UI_FONT_BUNDLE = "ui_font_fzy3k.bundle";
        public const string GAME_UI_TRANSPARENT_BG = "ui_transparentbg.bundle";
        
        
        
        // <summary>
        /// AssetData在编辑器中的相对位置
        /// </summary>
        public static string AppConfigDirPath = "Assets/Framework/AppConfig/";
        
        /// <summary>
        /// AssetData的全路径
        /// </summary>
        public static string AppConfigDirFullPath = Application.dataPath + "/Framework/AppConfig/";

        /// <summary>
        /// AppFileName
        /// </summary>
        public static string AppFileName = "AppFile.json";

        /// <summary>
        /// AppFile文件在编辑器中的位置
        /// </summary>
        public static string AppFilePath = string.Concat(AppConfigDirPath, AppFileName);
        
        /// <summary>
        /// AppFile文件在编辑器中的位置
        /// </summary>
        public static string AppFileFullPath = string.Concat(AppConfigDirFullPath, AppFileName);
        
        
        public static float FirstTime = 0f;
        public static float LastTime = 0f;
        public static void TickTime(string tickName)
        {
            float time = Time.realtimeSinceStartup - FirstTime;
            //Debug.Log(tickName + " ******  TotalTime ----------------" + time);
            //Debug.Log("DeltaTime ================" + (Time.realtimeSinceStartup - LastTime));
            LastTime = Time.realtimeSinceStartup;
        }

    }
// }