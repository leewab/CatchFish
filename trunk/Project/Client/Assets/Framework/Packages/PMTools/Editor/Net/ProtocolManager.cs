using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    public static class ProtocolManager
    {
        /// <summary>
        /// 服务器响应处理器
        /// </summary>
        public static S2C_ProtocolHandler S2CProtocolHandler = new S2C_ProtocolHandler();

        /// <summary>
        /// 客户端申请处理器
        /// </summary>
        public static C2S_ProtocolHandler C2SProtocolHander = new C2S_ProtocolHandler();


        private static bool mIsRegister = false;
        
        /// <summary>
        /// 注册HTTP响应事件
        /// </summary>
        public static void RegisterProtocol()
        {
            if (mIsRegister) UnRegisterProtocol();
            HttpHelper.OnHttpResponseGetEvent += OnResponseGetEvent;
            HttpHelper.OnHttpResponsePostEvent += OnResponsePostEvent;
            mIsRegister = true;
        }

        /// <summary>
        /// 注销HTTP响应事件
        /// </summary>
        public static void UnRegisterProtocol()
        {
            mIsRegister = false;
            HttpHelper.OnHttpResponseGetEvent -= OnResponseGetEvent;
            HttpHelper.OnHttpResponsePostEvent -= OnResponsePostEvent;
        }
        
        private static void OnResponseGetEvent(string _result)
        {
            Debug.Log("Get---");
            Debug.Log(_result);
            if (string.IsNullOrEmpty(_result)) return;
            S2CProtocolHandler.SendMessage(_result);
        }

        private static void OnResponsePostEvent(string _result)
        {
            Debug.Log("Post---" + _result);
            if (string.IsNullOrEmpty(_result)) return;
            S2CProtocolHandler.SendMessage(_result);
        }
    }
}