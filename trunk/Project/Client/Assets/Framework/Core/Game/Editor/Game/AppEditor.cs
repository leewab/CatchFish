using System;
using Game;
using Game.Core;
using UnityEditor;

namespace Game.UI
{
    public class AppEditor
    {
        
        [MenuItem("Game/项目初始化/1. 创建AppConfig", false, 1)]
        public static void CreateAppConfig_MenuItem()
        {
            EditorAssetHelper.CreateEditorAsset();
        }

        [MenuItem("Game/项目初始化/2. 根据AppConfig格式化项目结构", false, 2)]
        public static void InitProject_MenuItem()
        {
            EditorAssetHelper.FormatProject();
        }

        [MenuItem("Game/项目初始化/5. 初始化设置UISortingLayer", false, 5)]
        public static void InitProject_UISortingLayer()
        {
            string[] sortingLayers = Enum.GetNames(typeof(UILayerEnums));
            foreach (var sortingLayer in sortingLayers)
            {
                UnityEditorUtil.AddSortingLayers(sortingLayer);
            }
        }
    }
}
