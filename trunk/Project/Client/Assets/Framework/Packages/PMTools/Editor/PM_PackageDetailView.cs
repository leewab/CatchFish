
using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    public class PM_PackageDetailView : BaseEditorView
    {
        private PM_PackageDetailModule mPackageDetailModule;
        
        public override void OnGUI(BaseEditorModule module)
        {
            base.OnGUI(module);
            mPackageDetailModule = module as PM_PackageDetailModule;
            if (mPackageDetailModule == null) return;
            EditorHelper.DrawTitle("Package详情", new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                margin = new RectOffset(0, 0, 0, 0),
                fixedHeight = 60,
                fontSize = 16,
                fontStyle = FontStyle.Bold
            });
            GUILayout.BeginVertical(pBoxGUIStyle);
            EditorHelper.DrawLable("名        称: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.Name, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("分        类: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.Type, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("本 地 路 劲: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.ClientResPath, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("本地版本号: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.ClientVersion, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("远端版本号: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.ServerVersion, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("时        间: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.UpdateTime, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("大        小: ", new UnityEngine.Vector2(80, 26), $"{mPackageDetailModule.PackageInfo.Size/1000}M", new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("作        者: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.Author, new UnityEngine.Vector2(0, 26));
            EditorHelper.DrawLable("备        注: ", new UnityEngine.Vector2(80, 26), mPackageDetailModule.PackageInfo.Des, new UnityEngine.Vector2(0, 0));
            GUILayout.EndVertical();
        }
        
    }
}