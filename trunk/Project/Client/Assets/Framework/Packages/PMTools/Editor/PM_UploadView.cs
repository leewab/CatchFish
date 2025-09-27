using System;
using Framework.Core;
using UnityEditor;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_UploadView : BaseEditorView
    {
        private PM_UploadModule mUploadModule = null;

        public override void OnGUI(BaseEditorModule module, params Action[] actions)
        {
            base.OnGUI(module, actions);
            EditorHelper.DrawTitle("PM上传");
            if (null == module) return;
            mUploadModule = module as PM_UploadModule;
            if (null == mUploadModule) return;
            mUploadModule.PackageInfo.Name = EditorHelper.DrawField("名     称:", mUploadModule.PackageInfo.Name, new Vector2(60, 0), Vector2.zero);
            mUploadModule.PackageInfo.Type = EditorHelper.DrawField("分     组:", mUploadModule.PackageInfo.Type, new Vector2(60, 0), Vector2.zero);
            mUploadModule.PackageInfo.ClientVersion = EditorHelper.DrawField("版 本 号:", mUploadModule.PackageInfo.ClientVersion, new Vector2(60, 0), Vector2.zero);
            mUploadModule.PackageInfo.Des = EditorHelper.DrawField("描     述:", mUploadModule.PackageInfo.Des, new Vector2(60, 0), new Vector2(0, 100));
            GUILayout.Space(10);
            EditorHelper.DrawButton("初始化设置", Vector2.zero, () =>
            {
                var initWindow = ScriptableObject.CreateInstance<PM_PackageInitWindow>();
                initWindow.Show();
                initWindow.ReadyInit(AssetDatabase.GUIDToAssetPath(mUploadModule.PackageInfo.ClientResPath));
            });
            GUILayout.Space(10);
            EditorHelper.DrawButton("开始上传", new Vector2(0, 24), actions[0]);
        }
    }
}