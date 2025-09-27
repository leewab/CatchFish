using System.Collections.Generic;
using System.IO;
using System.Linq;
using Framework.Core;
using UnityEditor;
using UnityEngine;

namespace Framework.PM
{
    //根据每个Package中的initialization.json进行各自package的初始化，initialization.json中记录了各自需要初始化的内容，包括
    // 1、需要移动的文件夹
    // 2、是否已经进行初始化
    public class PM_PackageInitWindow : EditorWindow
    {
        private bool mIsStartDraw = false;
        private string mCurModulePath = "";
        private InitState mCurInitState = InitState.ReadyInit;

        public void StartInit(string _path)
        {
            mCurInitState = InitState.StartInit;
            mCurModulePath = _path;
            var initInfo = LoadInitInfo();
            mStartCoreDirList = initInfo.CoreDirectory.ToList();
            mStartTipsContent = initInfo.TipsContent;
            mIsStartDraw = true;
        }

        public void ReadyInit(string _path)
        {
            mCurInitState = InitState.ReadyInit;
            mCurModulePath = _path;
            mIsStartDraw = true;
        }
        
        void OnGUI()
        {
            EditorHelper.DrawTitle("初始化Package");
            if (!mIsStartDraw) return;
            switch (mCurInitState)
            {
                case InitState.StartInit:
                    
                    DrawStartInitPanel();
                    break;
                case InitState.ReadyInit:
                    
                    DrawReadyInitPanel();
                    break;
            }
        }

        #region StartInit

                
        private List<string> mStartCoreDirList = new List<string>();
        private string mStartTipsContent = "";
        
        private void DrawStartInitPanel()
        {
            EditorHelper.DrawLable("备    注：",  new Vector2(60, 0), mStartTipsContent, new Vector2(0, 40));
            EditorHelper.DrawLable("初始化内容：", new Vector2(60, 0), $"{mStartCoreDirList.Count}个", Vector2.zero);
            for (int i = 0; i < mStartCoreDirList.Count; i++)
            {
                string strTemp = mStartCoreDirList[i];
                GUILayout.BeginHorizontal();
                EditorHelper.DrawLable(strTemp);
                EditorHelper.DrawButton("—", new Vector2(26, 20), () => { mStartCoreDirList.Remove(strTemp); });
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(20);
            EditorHelper.DrawButton("确认初始化", StartInit, null,GUILayout.Height(26));
        }

        private InitInfo LoadInitInfo()
        {
            if (!Directory.Exists(mCurModulePath)) return null;
            mCurModulePath = mCurModulePath + "/initialization.json";
            if(!File.Exists(mCurModulePath)) return null;
            return JsonHelper.LoadSync<InitInfo>(mCurModulePath);
        }

        private void StartInit()
        {
//            string targetDir = Application.dataPath + $"/{AppMgr.CurAppAsset.RootName}";
//            for (int i = 0; i < mStartCoreDirList.Count; i++)
//            {
//                string sourcePath = Application.dataPath.Replace("Assets", mStartCoreDirList[i]);
//                IOHepler.MoveDirectory(sourcePath, targetDir);
//            }
//            Close();
        }


        #endregion

        #region ReadyInit

        private List<string> mCoreDirList = new List<string>();
        private string mTipsContent = "";
        
        private void DrawReadyInitPanel()
        {
            mTipsContent = EditorHelper.DrawField("备    注：", mTipsContent, new Vector2(60, 0), new Vector2(0, 40));
            EditorHelper.DrawLable("初始化内容：", new Vector2(60, 0), $"{mCoreDirList.Count}个", Vector2.zero);
            for (int i = 0; i < mCoreDirList.Count; i++)
            {
                string strTemp = mCoreDirList[i];
                GUILayout.BeginHorizontal();
                EditorHelper.DrawLable(strTemp, Vector2.zero);
                EditorHelper.DrawButton("—", new Vector2(26, 20), () => { mCoreDirList.Remove(strTemp); });
                GUILayout.EndHorizontal();
            }
            var objs = EditorHelper.DrawDragArea(new Vector2(100, 30), "Drag", GUILayout.ExpandWidth(true));
            foreach (var t in objs)
            {
                mCoreDirList.Add(AssetDatabase.GetAssetPath(t));
            }
            GUILayout.Space(20);
            EditorHelper.DrawButton("确    认", WriteInitInfo, null,GUILayout.Height(26));
        }

        private void WriteInitInfo()
        {
            if (!Directory.Exists(mCurModulePath)) return;
            mCurModulePath = mCurModulePath + "/initialization.json";
            if(File.Exists(mCurModulePath)) File.Delete(mCurModulePath);
            InitInfo initInfo = new InitInfo();
            initInfo.CoreDirectory = mCoreDirList.ToArray();
            initInfo.IsInitialized = false;
            initInfo.TipsContent = mTipsContent;
            JsonHelper.Save(initInfo, mCurModulePath);
            AssetDatabase.Refresh();
            Close();
        }

        #endregion
       
        private enum InitState
        {
            StartInit,                  //开始初始化
            ReadyInit                   //准备初始化数据（设置）
        }
    }

    public class InitInfo
    {
        public string[] CoreDirectory;          //核心需要移动到项目根目录的文件路径
        public bool IsInitialized;              //是否完成了初始化
        public string TipsContent;              //备注提示信息
    }
}