using System.Collections.Generic;

namespace Framework.Core
{
    /// <summary>
    /// 本地记录AB
    /// </summary>
    public class ResLocalConfig
    {
        /// <summary>
        /// 唯一Id 由Group+Platfrom组成
        /// </summary>
        public string ResId;
        
        /// <summary>
        /// AB的组名称
        /// </summary>
        public string ResGroup;

        /// <summary>
        /// Res的输出路径
        /// </summary>
        public string ResOutPath;

        /// <summary>
        /// 资源平台
        /// </summary>
        public string ResPlatform;

        /// <summary>
        /// 当前组所有的Res Asset源资源路径
        /// </summary>
        public List<AssetInfo> ResAssetBundle;
    }

    public class AssetInfo
    {
        public string ABName;
        public string[] AssetPaths;
    }
}