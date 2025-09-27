using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// 用于管理App的数据信息
    /// </summary>
    public class AppManager
    {
        #region AppFile读取

        /// <summary>
        /// AppFile本地文件
        /// </summary>
        public static AppFile AppFileData;

        //TODO:这里未加载
        /// <summary>
        /// 加载AppFile
        /// </summary>
        public static AppFile LoadAppFile()
        {
            if (AppFileData != null) return AppFileData;
            Debug.Log("LoadAppFile");
            string appFilePath = null;
            appFilePath = ResUtil.GetResRelativePath(GameConfig.AppFileName, ResRoot.StreamingAssets);
            appFilePath.ThrowNullException();
            LogHelper.Log("appFilePath::" + appFilePath);
            AppFileData = JsonHelper.LoadSync<AppFile>(appFilePath);
            // mAppFile = TextFileHelper.LoadTextFile<AppFile>(appFilePath);
            AppFileData.ThrowNullException();
            return AppFileData;
        }

        /// <summary>
        /// 更新AppFile
        /// </summary>
        /// <param name="appFile"></param>
        public static void UpdateAppFile(AppFile appFile)
        {
            AppFileData = appFile;
            SaveAppFile();
        }
        
        /// <summary>
        /// 存储AppFile
        /// </summary>
        /// <param name="result"></param>
        private static void SaveAppFile()
        {
            JsonHelper.Save(AppFileData, ResUtil.GetWriteFilePath(GameConfig.AppFileName));
        }

        #endregion
        
        /// <summary>
        /// 是佛开启Tolua
        /// </summary>
        public static bool IsEnableToLua
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
        
    }
}