/************************************************************
 *** Copyright IBing  author:L-Xiansheng
 ************************************************************/

using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_AdminWindow : BaseEditorWindow
    {
        private PM_AdminView adminView;
        public PM_AdminModule adminModel;

        protected override void Awake()
        {
            if (adminView == null) adminView = new PM_AdminView();
        }

        public void Init(PM_AdminModule adminModule)
        {
            adminModel = adminModule;
        }

        public override void RegisterEvent()
        {
            base.RegisterEvent();
            ProtocolManager.S2CProtocolHandler.OnUserLoginEvent = OnUserLoginEvent;
            ProtocolManager.S2CProtocolHandler.OnUserRegisterEvent = OnUserRegisterEvent;
        }

        public override void UnRegisterEvent()
        {
            base.UnRegisterEvent();
            ProtocolManager.S2CProtocolHandler.OnUserLoginEvent = null;
            ProtocolManager.S2CProtocolHandler.OnUserRegisterEvent = null;
        }

        protected override void OnGUI()
        {
            adminView.OnGUI();
        }

        private void OnUserLoginEvent(ProtocolInfo _protocolInfo)
        {
            Debug.Log("<color=green>OnUserLoginEvent</color>");
            Debug.Log(_protocolInfo.Id);
            Debug.Log(_protocolInfo.ProtocolAction);
            Debug.Log(_protocolInfo.ProtocolResult);
            Debug.Log(_protocolInfo.ProtocolData);
            adminModel.IsAdmin = true;
        }

        private void OnUserRegisterEvent(ProtocolInfo _protocolInfo)
        {
            Debug.Log("<color=green>"+_protocolInfo.ProtocolResult+"</color>");
            
        }
    }
}