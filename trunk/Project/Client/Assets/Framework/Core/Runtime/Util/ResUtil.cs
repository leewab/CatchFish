using System;
using System.IO;
using UnityEngine;

namespace Game.Core
{
    public class ResUtil
    {
        private static bool mIsAssetBundleState = true;

        /// <summary>
        /// 是否走AssetBundle加载模式
        /// </summary>
        public static bool IsAssetBundleState
        {
            get
            {
#if UNITY_EDITOR
                mIsAssetBundleState = PlayerPrefs.GetInt("ResIsAssetBundleState") == 1;
#endif
                return mIsAssetBundleState;
            }
        }
        
        /// <summary>
        /// 通过AssetBundleGroup获取当前平台下的绝对路径
        /// </summary>
        /// <param name="abGroup"></param>
        /// <returns></returns>
        public static string GetAbsolutePath(string resPath, ResGroup abGroup)
        {
            return abGroup == ResGroup.None ? $"{GetDirectoryWithPlatform}/{resPath}" : $"{GetDirectoryWithPlatform}/AssetBundles/{abGroup.ToString()}/{resPath}";
        }
        
        /// <summary>
        /// 获取渠道平台的绝对路径
        /// </summary>
        /// <param name="resPath"></param>
        /// <param name="abGroup">资源分组</param>
        /// <param name="channelGroup">渠道分组</param>
        /// <returns></returns>
        public static string GetChannelAbsolutePath(string resPath, ResGroup abGroup, ChannelGroup channelGroup)
        {
            if (channelGroup == ChannelGroup.None)
            {
                //return abGroup == ResGroup.None ? resPath : $"{abGroup.ToString()}/{resPath}";
                return GetAbsolutePath(resPath, abGroup);
            }
            else
            {
                return abGroup == ResGroup.None ? $"{channelGroup.ToString()}/{resPath}" : $"{channelGroup.ToString()}/{abGroup.ToString()}/{resPath}";
            }
        }

        /// <summary>
        /// 获取写入文件的路径(自动获取平台对应的资源文件夹)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="abGroup"></param>
        /// <returns></returns>
        public static string GetWriteFilePath(string path)
        {
#if !UNITY_EDITOR //&& (UNITY_ANDROID || UNITY_IPHONE)
            return GetChannelResPath(path, ResRoot.PersistentData);
#else
            return GetChannelResPath(path, ResRoot.StreamingAssets);
#endif
        }
        
        /// <summary>
        /// 获取文件全路径(首先判断persistent下有没有，如果没有返回streaming路径)
        /// (自动获取平台对应的资源文件夹)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetLoadFilePath(string path)
        {
            string persistentPath = GetResRelativePath(path, ResRoot.PersistentData);
            Debug.Log("persistentPath::" + persistentPath);
            if (File.Exists(persistentPath)) { return persistentPath; }
            string streamingPath = GetResRelativePath(path, ResRoot.StreamingAssets);
            Debug.Log("streamingPath::" + streamingPath);
            if (!File.Exists(streamingPath)) { Debug.LogError("persistentPath和streamingPath下都没有该文件！" + path); }
            return streamingPath;
        }
                
        /// <summary>
        /// 获取渠道资源路径
        /// </summary>
        /// <param name="resPath">资源名称</param>
        /// <param name="resRoot">资源根节点</param>
        /// <returns></returns>
        public static string GetChannelResPath(string resPath, ResRoot resRoot)
        {
            return GetResRelativePath(resPath, resRoot);
        }

        /// <summary>
        /// 获取资源的相对路径
        /// </summary>
        /// <param name="resRoot">资源根目录</param>
        /// <param name="resPath">资源所在根目录文件夹下的绝对路径</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetResRelativePath(string resPath, ResRoot resRoot)
        {
            switch (resRoot)
            {
                case ResRoot.StreamingAssets:
#if !UNITY_EDITOR && UNITY_ANDROID
                    return $"{Application.dataPath}/!assets{resPath}";
#else
                    return Path.Combine(Application.streamingAssetsPath, resPath);
#endif
                case ResRoot.PersistentData:
                    return Path.Combine(Application.persistentDataPath, resPath);
                case ResRoot.AssetDataBase:
                    return Path.Combine("Assets", resPath);
                case ResRoot.FullPath:
                    return  Path.Combine(Application.dataPath, resPath);
                case ResRoot.ResWeb:
                    return Path.Combine(AppManager.AppFileData.ResURL, resPath);
                case ResRoot.AppWeb:
                    return Path.Combine(AppManager.AppFileData.AppURL, resPath);
                case ResRoot.ServerListWeb:
                    return Path.Combine(AppManager.AppFileData.ServerListURL, resPath);
                case ResRoot.TemporaryCache:
                    return "";
            }
            
            throw new Exception("资源路径获取失败！resRoot参数有误！");
        }

        /// <summary>
        /// 获取当前平台对应的根目录文件夹
        /// (之后这里换成渠道_平台)
        /// </summary>
        /// <returns></returns>
        public static string GetDirectoryWithPlatform
        {
            get
            {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "IOS";
#elif UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                return "Window";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return "Mac";
#elif UNITY_PS4
            return "PS4";
#elif UNITY_XBOXONE
            return "XboxOne";
#elif UNITY_WEBGL
            return "WebGL";
#elif UNITY_FACEBOOK
            return "Facebook";
#else
            return "Custom";
#endif
            }
        }
        
        /// <summary>
        /// 编辑器模式下判断是否为AssetBundle加载模式，如果是的话走StreamingAsset，否则走AssetdateBase
        /// 非编辑器模式下，首先给与Persistent路径，如果加载过程中找不着该资源，自动切换为Streaming路径
        /// </summary>
        /// <returns></returns>
        public static ResRoot GetResRoot()
        {
            //TODO:通过文件判断当前资源是否属于本地加载、网络加载
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            if (IsAssetBundleState)
            {
                return ResRoot.StreamingAssets;
            }
            else
            {
                return ResRoot.AssetDataBase;
            }
#else
            return ResRoot.PersistentData;
#endif
        }
        
        /// <summary>
        ///  it will always point to the correct location on the platform where the application is running.
        /// </summary>
        /// <returns></returns>
        public static string GetStringAssetPath()
        {
            string path;
            if (Application.platform == RuntimePlatform.Android)
            {
                path = "jar:file://" + Application.dataPath + "!/assets/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = "file:///"+Application.dataPath + "/Raw/";;
            }else
            {
                path = "file:///"+Application.dataPath + "/StreamingAssets/";
            }
            
            return path;
        }

    }
}