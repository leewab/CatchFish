using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public class EditorAssetHelper
    {
#if UNITY_EDITOR
        
        private static EditorAsset _EditorAsset;

        public static EditorAsset EditorAsset
        {
            get
            {
                if (_EditorAsset == null) _EditorAsset = LoadEditorAsset();
                return _EditorAsset;
            }
        }
        
        public static EditorAsset LoadEditorAsset()
        {
            return AssetHelper.Instance.LoadAsset<EditorAsset>("EditorAsset");
        }

        public static EditorAsset CreateEditorAsset()
        {
            return AssetHelper.Instance.CreateAsset<EditorAsset>("EditorAsset");
        }

        public static void FormatProject()
        {
            if (EditorAsset == null)
            {
                CreateEditorAsset();
                EditorUtility.DisplayDialog("格式化项目结构", "项目必要文件AssetConfig.asset刚刚创建，请前往配置，然后进行格式化项目结构！", "确认");
                return;
            }

            if (!EditorAsset.ProInited)
            {
                EditorUtility.DisplayDialog("格式化项目结构", "格式化项目文件夹结构请在AppAsset文件中开启ProModule！", "确认");
                return;
            }

            // GenerateAppFile();
            if (EditorUtility.DisplayDialog("格式化项目结构", "AppFile文件创建成功，AssetBundle打包记得引入StreamingAsset，确认继续格式化目录文件夹！",
                "确认"))
            {
                CheckRoot(result =>
                {
                    if (result)
                    {
                        string gameRootDir = Application.dataPath + "/" + EditorAsset.RootName;
                        if (!Directory.Exists(gameRootDir)) Directory.CreateDirectory(gameRootDir);
                        foreach (var kv in ResDefine.ResDirectory)
                        {
                            string resDir = "";
                            if ("ScriptsDirectory".Equals(kv.Key) || "ResourceDirectory".Equals(kv.Key))
                            {
                                resDir = $"{Application.dataPath}/{EditorAsset.RootName}/{kv.Value}";
                            }
                            else
                            {
                                resDir =
                                    $"{Application.dataPath}/{EditorAsset.RootName}/{ResDefine.ResDirectory["ResourceDirectory"]}/{kv.Value}";
                            }

                            if (!Directory.Exists(resDir)) Directory.CreateDirectory(resDir);
                        }

                        AssetDatabase.Refresh();
                    }
                });
            }
        }

        private static void CheckRoot(Action<bool> callBack)
        {
            var dirs = Directory.GetDirectories(Application.dataPath, "Root_*", SearchOption.AllDirectories);
            if (dirs.Length > 0)
            {
                callBack?.Invoke(EditorUtility.DisplayDialog("格式化项目结构",
                    $"项目中已经存在{dirs.Length}个与格式化规则相同的目录，请确保正确性，选择是否继续操作！", "确认", "取消"));
            }
            else
            {
                callBack?.Invoke(true);
            }
        }
        
#endif
    }
}