using System.Collections.Generic;

namespace Game.UI
{
    public class ResDefine
    {
        //TODO：暂时根据Assetbundle打包所输入的路径保持一致，后面再想办法互通一次性设置
        /// <summary>
        /// 根据运行平台获取AB的相对路径
        /// </summary>
        /// <param name="runtimePlatform"></param>
        /// <returns></returns>
        // public static string AssetBundleRelativePath => $"AssetBundles/{ResUtil.GetDirectoryWithPlatform}";

        /// <summary>
        /// 资源文件夹
        /// </summary>
        private static Dictionary<string, string> mResDirectory = null;
        public static Dictionary<string, string> ResDirectory => InitResDirectory();
        
        /// <summary>
        /// 初始化资源目录
        /// </summary>
        private static Dictionary<string, string> InitResDirectory()
        {
            if (mResDirectory != null) return mResDirectory;
            mResDirectory = new Dictionary<string, string>
            {
                //脚本文件夹
                {"ScriptsDirectory", "Scripts"},
                //资源文件夹
                {"ResourceDirectory", "ArtRes"},
                //Scene场景目录
                {"SceneDirectory", "Scene"},
                //预制体目录
                {"PrefabDirectory", "Prefabs"},
                //AudioSource音频目录
                {"AudioSourceDirectory", "AudioSource"},
                //Video视频目录
                {"VideoDirectory", "Video"},
                //Texture图集目录
                {"TextureDirectory", "Texture"},
                //Icon图集目录
                {"IconDirectory", "Icon"},
            };
            return mResDirectory;
        }
    }
}