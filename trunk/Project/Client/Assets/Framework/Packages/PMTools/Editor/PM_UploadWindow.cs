using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_UploadWindow : BaseEditorWindow
    {
        private PM_UploadModule mUploadModule = null;
        private PM_UploadView mUploadView = null;

        protected override void Awake()
        {
            base.Awake();
            if (null == mUploadModule) mUploadModule = new PM_UploadModule();
            if (null == mUploadView) mUploadView = new PM_UploadView();
        }

        protected override void OnEnable()
        {
            ProtocolManager.S2CProtocolHandler.OnUploadEvent = OnResUploadEvent;

            base.OnEnable();
            Start();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (!mUploadModule.IsInited) return;
            mUploadView.OnGUI(mUploadModule, StartUpload);
        }

        public override void RegisterEvent()
        {
            base.RegisterEvent();
        }

        public override void UnRegisterEvent()
        {
            base.UnRegisterEvent();
            ProtocolManager.S2CProtocolHandler.OnUploadEvent = null;
        }

        protected void OnResUploadEvent(ProtocolInfo _protocolInfo)
        {
            if (_protocolInfo == null) return;
            if (string.IsNullOrEmpty(_protocolInfo.ProtocolData))
            {
                Debug.Log("<color=yellow>服务器数据获取失败</color>");
                return;
            }
            int protocolAction = 0;
            if (int.TryParse(_protocolInfo.ProtocolAction, out protocolAction))
            {
                switch (protocolAction)
                {
                    case (int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD:
                        
                        break;
                    case (int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD_PACKAGEVERSION:
                        mUploadModule.ConvertInfo(_protocolInfo.ProtocolData);
                        break;
                }
            }
            
           
        }

        private void Start()
        {
            mUploadModule.RequestServer();
        }

        private void StartUpload()
        {
           mUploadModule.GenerateLocalInfo(Close);
        }
    }
}