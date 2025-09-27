using UnityEngine;

namespace Framework.Core
{
    public class HotfixAsset : ScriptableObject
    {
        /// <summary>
        /// 是否开启热更新模块
        /// </summary>
        public bool IsOpenHotfix = false;

        /// <summary>
        /// 版本资源文件URL
        /// </summary>
        public string Res_URL = $"127.0.0.1:8080/AppName/ResHome";

        /// <summary>
        /// 下载失败重试次数
        /// </summary>
        public int Download_Fail_Retry = 3;
    }
}