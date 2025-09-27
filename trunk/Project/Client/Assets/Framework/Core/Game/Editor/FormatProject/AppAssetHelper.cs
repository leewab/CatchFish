using System.IO;
using Game;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public static class AppAssetHelper
    {
#if UNITY_EDITOR
        
        private static AppAsset _AppAsset;

        public static AppAsset AppAsset
        {
            get
            {
                if (_AppAsset == null) _AppAsset = LoadAppAsset();
                return _AppAsset;
            }
        }
        
        public static AppAsset LoadAppAsset()
        {
            return AssetHelper.Instance.LoadAsset<AppAsset>("AppAsset");
        }

        /// <summary>
        /// 在初始化项目结构的时候创建一个AppFile文件，因为AppFile文件记录了资源根目录，且在打AssetBundle的时候回创建
        /// 未打AssetBundle的时候本地资源加载也是需要AppFile
        /// </summary>
        public static void GenerateAppFile()
        {
            if (AppAsset == null) return;
            AppFile appFile = new AppFile
            {
                AppVersion = "0",
                ResVersion = "0.0.1",
                ServerID = AppAsset.ServerID,
                IsLocalLog = AppAsset.OpenLocalLog,
                IsRemoteLog = AppAsset.OpenRemoteLog,
                IsOpenHotfix = AppAsset.OpenHotfix,
                RootName = EditorAssetHelper.EditorAsset.RootName,
                AppName = AppAsset.AppName,
                ResURL = AppAsset.ResURL, //http://106.52.125.172:4585/update/PluginManager
                ServerListURL = AppAsset.WebURL, //http://106.52.125.172:4585/update/PluginManager
                AppURL = AppAsset.AppURL, //http://106.52.125.172:4585/update/PluginManager/App
                SecretKey = AppAsset.SecretKey,
                ChannelTag = AppAsset.ChannelTag,
                ChannelGroup = AppAsset.ChannelGroup,
            };
            if (File.Exists(GameConfig.AppFilePath)) File.Delete(GameConfig.AppFilePath);
            // TextFileHelper.CreateTextFile(appFile, GameConfigHelper.AppFilePath);
            JsonHelper.Save(appFile, GameConfig.AppFilePath);
            AssetDatabase.Refresh();
            Debug.Log("成功创建文件：" + GameConfig.AppFilePath);
        }

#endif
    }
}