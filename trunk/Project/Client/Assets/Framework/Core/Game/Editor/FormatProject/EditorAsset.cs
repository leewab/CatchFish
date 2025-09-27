using UnityEngine;

namespace Game.UI
{
    public class EditorAsset : BaseAsset
    {
        /// <summary>
        /// 根目录
        /// </summary>
        [Header("根目录")] 
        public string RootName = "Root_";
        
        /// <summary>
        /// 项目源目录结构初始化
        /// </summary>
        [Header("目录结构初始化")] 
        public bool ProInited = false;

        protected override void OnAssetDataRefresh()
        {
            base.OnAssetDataRefresh();
            AppAssetHelper.GenerateAppFile();
        }
    }
}