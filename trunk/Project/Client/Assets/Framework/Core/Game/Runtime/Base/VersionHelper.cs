#if !UNITY_EDITOR && (ANDROID_AR_TEST || IOS_AR_TEST)
    #define AR_TEST_UPDATE
#endif

using System;
using System.IO;
using Game.UI;
using UnityEngine;
using MUEngine;

namespace MUGame
{
    public class VersionHelper
    {
        private static VersionHelper _instance;

        public static VersionHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VersionHelper();
                    _instance.Init();
                }
                return _instance;
            }
        }

        private VersionLoader _loader;
        private string _serverPath = string.Empty;
        public string ServerPath
        {
            get
            {
                return _serverPath;
            }
        }
        private string _localVersionFullPath = string.Empty;
        private string _versionContent = string.Empty;
        
        private string[] _localVersionArr;
        private string[] _serverVerionArr;

        private Action _versionCallBack;

        private Action<string, byte[]> _confCallBack;
        private Action<string, AssetBundle> _luaCallBack;

        private static string DEBUG_FILE = "Debug.bytes";


        public string LocalServerListMD5
        {
            get
            {
                if (_localVersionArr != null && _localVersionArr.Length > 0)
                {
                    return _localVersionArr[0];
                }
                return string.Empty;
            }
        }
        public string ServerListMD5
        {
            get
            {
                if (_serverVerionArr != null && _serverVerionArr.Length > 0)
                {
                    return _serverVerionArr[0];
                }
                return string.Empty;
            }
        }
        public string ConfBundleName
        {
            get
            {
#if AR_TEST_UPDATE
                return "game_conf.bundle";
                //return string.Empty;
#endif
                if (_serverVerionArr != null && _serverVerionArr.Length > 1)
                {
                    return _serverVerionArr[1];
                }
                return string.Empty;
            }
        }
        public string LuaBundleName
        {
            get
            {
#if AR_TEST_UPDATE
                    return "lua.bundle";
#endif
                if (_serverVerionArr != null && _serverVerionArr.Length > 2)
                {
                    return _serverVerionArr[2];
                }
                return string.Empty;
            }
        }
        public string LocalBundleMapMD5
        {
            get
            {
                if (_localVersionArr != null && _localVersionArr.Length > 3)
                {
                    return _localVersionArr[3];
                }
                return string.Empty;
            }
        }
        public string BundleMapMD5
        {
            get
            {
                if (_serverVerionArr != null && _serverVerionArr.Length > 3)
                {
                    return _serverVerionArr[3];
                }
                return string.Empty;
            }
        }

        public string ServerAPPVersion
        {
            get
            {
                if (_serverVerionArr != null && _serverVerionArr.Length > 4)
                {
                    return _serverVerionArr[4];
                }
                return string.Empty;
            }
        }

        public string ResDir
        {
            get
            {
                if (_serverVerionArr != null && _serverVerionArr.Length > 5)
                {
                    return _serverVerionArr[5];
                }
                return string.Empty;
            }
        }

        //是否需要更新App
        public bool IsNeedUpdateApp
        {
            get
            {
                if(string.IsNullOrEmpty(ServerAPPVersion) == false)
                {
                    float sv = ToVersionNum(ServerAPPVersion);
                    float lv = ToVersionNum(Application.version);
                    return sv > lv;
                }
                return false;
            }
        }
        
        private float ToVersionNum(string str)
        {
            float code = 0;
            int bitValue = 100;
            string[] nums = str.Split('.');
            int len = nums.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                code += Mathf.Pow(bitValue, len - 1 - i) * float.Parse(nums[i]);
            }
            return code;
        }


        private void Init()
        {
            
        }

        private void SetResDir(string dir)
        {
            _serverPath = "http://47.100.166.242/lhrgyd/client/" + dir + "/";
            string _serverPath1 = "https://lhrg-res.aligames.com/lhrgyd/client/" + dir + "/";

            //_serverPath = "http://10.4.4.23:8081/update/" + dir + "/";
            MUEngine.SysConf.GAME_RES_URL = new string[]
            {
                _serverPath1 + "/Res",
                _serverPath + "/Res"
            };
        }

        // private void ReadLocalVersion()
        // {
        //     string localVersion = LuaUtil.ReadTextFromFile(_localVersionFullPath);
        //     if(string.IsNullOrEmpty(localVersion))
        //         return;
        //     _localVersionArr = ParseVersion(localVersion);
        // }

        private string[] ParseVersion(string content)
        {
            string[] strs = content.Split('|');
            return strs;
        }

        private string GetPath(string fileName)
        {
            string path = Application.persistentDataPath + "/" + fileName;
            return path;
        }

        private string[] GetServerPath(string fileName)
        {
            string[] paths = new string[MUEngine.SysConf.GAME_RES_URL.Length];
            for(int i = 0; i < MUEngine.SysConf.GAME_RES_URL.Length; i++)
            {
                paths[i] = MUEngine.SysConf.GAME_RES_URL[i] + "/" + fileName;
            }
            return paths;
        }

        private string LocalPathToURL(string path)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                path = "file://" + path;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = "file:///" + path;
            }
            else
            {
                path = "file:///" + path;
            }
            return path;
        }
        
        // public void SaveVersionToLocal()
        // {
        //     if (!string.IsNullOrEmpty(_versionContent))
        //     {
        //         LuaUtil.WriteTextToFile(_localVersionFullPath, _versionContent);
        //     }
        // }

        //设置的reset包的记录，给Lua用,区分是新包还是老包
        string NEW_RESET_KEY = "newResetKey";
        private void SetResetRecord()
        {
            PlayerPrefs.SetInt(NEW_RESET_KEY, 1);
        }

        string OLD_ONLINE_KEY = "oldonlinekey";
        // 是否走老版本线上更新
        private bool IsOldOnLine()
        {
            if (!PlayerPrefs.HasKey(OLD_ONLINE_KEY))
                return false;

            bool isOld = PlayerPrefs.GetInt(OLD_ONLINE_KEY) == 1;
            return isOld;
        }
        public void initUpdateInfo()
        {
            SetResetRecord();
            if (!GameConfig.IsVersionUpdate)
            {
                // MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
                MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
#if LHRG_TW
#if LHRG_OB
                //tw and google use this
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://twsm-download2.gamamobi.com/smdlydgt/client/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://twsm-download2.gamamobi.com/smdlydgt/client/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://twsm-cdn.gamamobi.com/smdlydgt/client/";
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_TW/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_TW/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_TW/";
#endif

#elif LHRG_KR
#if LHRG_OB
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://cdn-wx-studio.gtarcade.com/product-2011304/cdn1/smdlydkr_rsync/client/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://cdn-wx-studio.gtarcade.com/product-2011304/cdn1/smdlydkr_rsync/client/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://web-smdl.gtarcade.com/smdlydkr_rsync/client/";
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_KR/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_KR/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_KR/";
#endif
#elif LHRG_DALAN
#if LHRG_OB
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/";
                MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/";
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_DALAN/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_DALAN/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_DALAN/";
#endif
#elif LHRG_QIANJUN
#if LHRG_OB
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/QianJun/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/QianJun/";
                MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/QianJun/";
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_QIANJUN/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_QIANJUN/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_QIANJUN/";
#endif
#else
#if LHRG_OB
                if(IsOldOnLine())
                {
                    MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
                    MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyd/client/";
                    MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
                }
                else
                {
                    MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/reset/";
                    MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/reset/";
                    MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/reset/";
                }
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
#endif
#endif
                MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
                return;
            }
#if LHRG_TW
            MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
            MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
            MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://twsm-download2.gamamobi.com/smdlydgt/client/";
            MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://twsm-download2.gamamobi.com/smdlydgt/client/";
            MUEngine.MUUpdateConfig.UpdateUrl = "http://twsm-cdn.gamamobi.com/smdlydgt/client/";            //MUEngine.MUUpdateConfig.PatchType = EUpdateType.MiniPackage;
            //MUEngine.MUUpdateConfig.PatchType = EUpdateType.MiniPackage;
            MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
#elif LHRG_KR
        MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
        MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
        MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://cdn-wx-studio.gtarcade.com/product-2011304/cdn1/smdlydkr_rsync/client/";
        MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://cdn-wx-studio.gtarcade.com/product-2011304/cdn1/smdlydkr_rsync/client/";
        MUEngine.MUUpdateConfig.UpdateUrl = "http://web-smdl.gtarcade.com/smdlydkr_rsync/client/";
        //MUEngine.MUUpdateConfig.PatchType = EUpdateType.MiniPackage;
        MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
#elif LHRG_DALAN
            MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
            MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
            MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/";
            MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/";
            MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/";
            MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
#elif LHRG_QIANJUN
            MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
            MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
            MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/QianJun/";
            MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/QianJun/";
            MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/QianJun/";
            MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
#else
            // MUEngine.MUUpdateConfig.ChannelRes = OneSDK.GetInstance().GetChannelDir();
            MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;

            if (IsOldOnLine())
            {
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyd/client/";
                MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
            }
            else
            {
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/reset/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyddalan/client/reset/";
                MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyddalan/client/reset/";
            }

            MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
#endif
        }



        private void OnLoadVersion(WWW www)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                _versionContent = www.text;
                _serverVerionArr = ParseVersion(_versionContent);
            }
            else
            {
                Debug.LogError("VersionHelper load Version.txt has an error:" + www.error);
            }

            CheckBundleMap();

        }

        private void CheckBundleMap()
        {
            if(string.IsNullOrEmpty(ResDir) == false)
            {
                SetResDir(ResDir);
            }

            if(string.IsNullOrEmpty(BundleMapMD5))
            {
                MUEngine.CoreDelegate.DynamicResource = false;
            }
            else
            {
                MUEngine.SysConf.BUNDLEMAP_FILE_SERVER = BundleMapMD5;
                MUEngine.CoreDelegate.DynamicResource = true;
            }

            if (string.IsNullOrEmpty(LocalBundleMapMD5) == false && LocalBundleMapMD5 != BundleMapMD5)
            {
                //删除上一次map文件
                string bundlemapPath = MUEngine.SysConf.GAME_RES_LOCAL_PATH + "/" + LocalBundleMapMD5;
                if (File.Exists(bundlemapPath))
                {
                    File.Delete(bundlemapPath);
                }
            }

            if (_versionCallBack != null)
            {
                _versionCallBack();
            }
        }
#if AR_TEST_UPDATE
        public void GetConf(Action<string,byte[]> _callBack)
        {
            CheckInitLoader();
            _confCallBack = _callBack;

            string localPath = GetPath("game_conf.bundle");
#if IOS_AR_TEST
            const string confBundlePath = "http://10.4.4.23:8081/update/AR_Test_Lua_Bundle/game_conf.bundle";
#elif ANDROID_AR_TEST
            const string confBundlePath = "http://10.4.4.23:8081/update/AR_Android_Test_Lua_Bundle/game_conf.bundle";
#endif
            _loader.Load(confBundlePath, localPath, OnLoadConfig);
        }
#else
        public void GetConf(Action<string,byte[]> _callBack)
        {
            _confCallBack = _callBack;

            string localPath = GetPath(ConfBundleName);
            if(File.Exists(localPath))
            {
                _loader.Load(LocalPathToURL(localPath), "", OnLoadConfig);
            }
            else
            {
                _loader.Load(GetServerPath(ConfBundleName), localPath, OnLoadConfig);
            }
        }
#endif


        private void OnLoadConfig(WWW www)
        {
            if(string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogError("VersionHelper load game_conf.bundle has an error:"+www.error);
                return;
            }
            if(www.assetBundle == null)
            {
                Debug.LogError("VersionHelper load game_conf.bundle  www.assetBundle = null");
                return;
            }
            TextAsset ta = www.assetBundle.LoadAsset<TextAsset>(GameConfig.GAME_CONF_BUNDLE_NAME);
            if (ta == null)
            {
                Debug.LogError("VersionHelper load game_conf.bundle load textAsset = null");
                return;
            }
            if (_confCallBack != null)
            {
                _confCallBack(GameConfig.GAME_CONF_BUNDLE_NAME, ta.bytes);
            }
            www.assetBundle.Unload(true);
        }
#if AR_TEST_UPDATE
        //AR 测试用，从某个固定网络路径上下载bundle包
        public void GetLua(Action<string, AssetBundle> _callBack)
        {
            CheckInitLoader();
            _luaCallBack = _callBack;
            //写死的下载路径
            string localPath = GetPath("lua.bundle");
#if IOS_AR_TEST
            const string luabundlePath = "http://10.4.4.23:8081/update/AR_Test_Lua_Bundle/lua.bundle";
#elif ANDROID_AR_TEST
            const string luabundlePath = "http://10.4.4.23:8081/update/AR_Android_Test_Lua_Bundle/lua.bundle";
#endif
            //此路径通过浏览器可以直接访问，在Mac的SMB：//10.4.4.23 中，对应与OurPerfect/release/xy/update/AR_Test_Lua_Bundle/lua.bundle
            _loader.Load(luabundlePath, localPath, OnLoadLuA);
        }
        public void GetLuaPB(Action<string,AssetBundle> _callBack){
            CheckInitLoader();
            string localPath = GetPath("lua_message_pb.bundle");
#if IOS_AR_TEST
            const string luaPbBundlePath = "http://10.4.4.23:8081/update/AR_Test_Lua_Bundle/lua_message_pb.bundle";
#elif ANDROID_AR_TEST
            const string luaPbBundlePath = "http://10.4.4.23:8081/update/AR_Android_Test_Lua_Bundle/lua_message_pb.bundle";
#endif
            _loader.Load(luaPbBundlePath,localPath,www => {
                if (string.IsNullOrEmpty(www.error) == false)
                {
                    Debug.LogError("VersionHelper load lua_message_pb.bundle has an error:" + www.error);
                    return;
                }
                if (_callBack != null)
                {
                    _callBack("lua_message_pb.bundle", www.assetBundle);
                }
            });
        }
        public void CheckInitLoader(){
            if(_loader == null){
                GameObject go = new GameObject("VersionObj");
                _loader = go.AddComponent<VersionLoader>();
                GameObject.DontDestroyOnLoad(go);
            }
        }
#else
        public void GetLua(Action<string, AssetBundle> _callBack)
        {
            _luaCallBack = _callBack;
            string localPath = GetPath(LuaBundleName);
            if (File.Exists(localPath))
            {
                _loader.Load(LocalPathToURL(localPath), "", OnLoadLuA);
            }
            else
            {
                _loader.Load(GetServerPath(LuaBundleName), localPath, OnLoadLuA);
            }
        }
#endif

        

        private void OnLoadLuA(WWW www)
        {
            if (string.IsNullOrEmpty(www.error) == false)
            {
                Debug.LogError("VersionHelper load lua.bundle has an error:" + www.error);
                return;
            }
            if (_luaCallBack != null)
            {
                _luaCallBack("lua.bundle", www.assetBundle);
            }
        }


        public void Dispose()
        {
            if(_loader != null)
            {
                GameObject.Destroy(_loader.gameObject);
                _loader = null;
            }
            _instance = null;
        }
        
    }
}
