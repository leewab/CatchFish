using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game
{
    public class AssetHelper : Singleton<AssetHelper>
    {
        /// <summary>
        /// AssetData在编辑器中的相对位置
        /// </summary>
        public static string AssetDataDirPath = string.Concat(GameConfig.AppConfigDirPath, "AssetData/");
        
        /// <summary>
        /// AssetData的全路径
        /// </summary>
        public static string AssetDataDirFullPath = string.Concat(GameConfig.AppConfigDirFullPath, "AssetData/");
        
        #region AssetData

        public T LoadAsset<T>(string assetName) where T : BaseAsset
        {
            if (File.Exists(string.Concat(AssetHelper.AssetDataDirFullPath, assetName, ".asset")))
            {
#if UNITY_EDITOR
                return AssetDatabase.LoadAssetAtPath<T>(string.Concat(AssetHelper.AssetDataDirPath, assetName, ".asset"));
#else
                return resCtrl.Load<T>("AssetData", assetName);
#endif
            }

            return null;
        }

#if UNITY_EDITOR

        public T CreateAsset<T>(string assetName) where T : BaseAsset
        {
            if (!Directory.Exists(AssetHelper.AssetDataDirFullPath)) Directory.CreateDirectory(AssetHelper.AssetDataDirFullPath);
            if (File.Exists(string.Concat(AssetHelper.AssetDataDirFullPath, assetName, ".asset"))) return LoadAsset<T>(assetName);
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, string.Concat(AssetHelper.AssetDataDirPath, assetName, ".asset"));
            AssetDatabase.Refresh();
            Debug.Log("成功创建文件：" + string.Concat(AssetHelper.AssetDataDirFullPath, assetName, ".asset"));
            return asset;
        }

        public void RemoveAsset<T>(string assetName) where T : BaseAsset
        {
            T asset = LoadAsset<T>(assetName);
            if (asset != null) AssetDatabase.RemoveObjectFromAsset(asset);
        }

#endif
        #endregion
    }
}