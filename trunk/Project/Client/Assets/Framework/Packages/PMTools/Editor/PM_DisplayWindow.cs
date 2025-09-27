using UnityEditor;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_DisplayWindow : EditorWindow
    {
        //DisplayModel
        private PM_DisplayModule mDisplayModle = new PM_DisplayModule();
        //DisplayView
        private PM_DisplayView mDisplayView = new PM_DisplayView();
        
        protected void Awake()
        {
            if(mDisplayModle == null) mDisplayModle = new PM_DisplayModule();
            if(mDisplayView == null) mDisplayView = new PM_DisplayView();
        }
        
        protected void OnGUI()
        {
            mDisplayView.OnGUI(mDisplayModle);
        }

        private void Update()
        {
            mDisplayView.Update();
        }

        protected void OnEnable()
        {
            Debug.Log("PM_DisplayWindow OnEnable");
            ProtocolManager.S2CProtocolHandler.OnDisplayEvent = OnResDisplayEvent;
            mDisplayModle.RequestData();
        }
        
        protected void OnDisable()
        {
            ProtocolManager.S2CProtocolHandler.OnDisplayEvent = null;
        }

        private void OnDestroy()
        {
            Clear();
        }

        protected void Clear()
        {
            mDisplayModle.Clear();
            mDisplayView.Clear();
        }

        private void OnResDisplayEvent(ProtocolInfo _protocolInfo)
        {
            Debug.Log("<color=green>OnResDisplayEvent</color>");
            if (_protocolInfo == null) return;
            if (string.IsNullOrEmpty(_protocolInfo.ProtocolData))
            {
                Debug.Log("<color=yellow>服务器暂时没有数据</color>");
                return;
            }
            //初始化服务器获取的数据
            bool isSuccess = mDisplayModle.ConvertInfos(_protocolInfo.ProtocolData);
            if (!isSuccess)
            {
                EditorUtility.DisplayDialog("PluginManager", "远端服务器暂无数据!", "确认");
            }
        }
    }
}