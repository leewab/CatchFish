using System;
using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    public class S2C_ProtocolHandler
    {
        #region 协议响应委托

        /// <summary>
        /// 登录成功
        /// </summary>
        private Action<ProtocolInfo> mOnUserLoginEvent;

        public Action<ProtocolInfo> OnUserLoginEvent
        {
            set
            {
                mOnUserLoginEvent = value;
                //账户登录
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.USER_LOGIN).ToString(), mOnUserLoginEvent);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private Action<ProtocolInfo> mOnUserRegisterEvent;

        public Action<ProtocolInfo> OnUserRegisterEvent
        {
            set
            {
                mOnUserRegisterEvent = value;
                //账户注册
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.USER_REGISTER).ToString(), mOnUserRegisterEvent);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        private Action<ProtocolInfo> mOnUserLogoutEvent;

        public Action<ProtocolInfo> OnUserLogoutEvent
        {
            set
            {
                mOnUserLogoutEvent = value;
                //账户注销
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.USER_LOGOUT).ToString(), mOnUserLogoutEvent);
            }
        }

        /// <summary>
        /// 移除事件
        /// </summary>
        private Action<ProtocolInfo> mOnUserRemoveEvent;

        public Action<ProtocolInfo> OnUserRemoveEvent
        {
            set
            {
                mOnUserRemoveEvent = value;
                //账户移除
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.USER_REMOVE).ToString(), mOnUserRemoveEvent);
            }
        }

        /// <summary>
        /// PM预览事件
        /// </summary>
        private Action<ProtocolInfo> mOnDisplayEvent;

        public Action<ProtocolInfo> OnDisplayEvent
        {
            set
            {
                mOnDisplayEvent = value;
                //PM预览
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.REQ_PM_DISPLAY).ToString(), mOnDisplayEvent);
            }
        }

        /// <summary>
        /// PM下载事件
        /// </summary>
        private Action<ProtocolInfo> mOnDownloadEvent;

        public Action<ProtocolInfo> OnDownloadEvent
        {
            set
            {
                mOnDownloadEvent = value;
                //PM下载
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.REQ_PM_DOWNLIAD).ToString(), mOnDownloadEvent);
            }
        }


        /// <summary>
        /// PM上传事件
        /// </summary>
        private Action<ProtocolInfo> mOnUploadEvent;

        public Action<ProtocolInfo> OnUploadEvent
        {
            set
            {
                mOnUploadEvent = value;
                //PM上传
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD).ToString(), mOnUploadEvent);
                //PackageVersion请求
                RegisterProtocol(((int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD_PACKAGEVERSION).ToString(), mOnUploadEvent);
            }
        }

        #endregion

        #region 协议记录执行
        
        /// <summary>
        /// 协议事件字典
        /// </summary>
        private static Dictionary<string, Action<ProtocolInfo>> mProtocolDic = new Dictionary<string, Action<ProtocolInfo>>();
        
        /// <summary>
        /// 获取注册事件
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public void SendMessage(string content)
        {
            if (string.IsNullOrEmpty(content)) return;
            Debug.LogWarning(content);
            var jsonContent = JsonHelper.Convert<ProtocolInfo>(content);
            if (jsonContent?.ProtocolData != null && !string.IsNullOrEmpty(jsonContent.ProtocolAction))
            {
                if (mProtocolDic.ContainsKey(jsonContent.ProtocolAction))
                {
                    mProtocolDic[jsonContent.ProtocolAction]?.Invoke(jsonContent);
                }
                else
                {
                    Debug.LogError("未注册协议事件、、、" + jsonContent.ProtocolAction);
                }
            }
        }

        
        /// <summary>
        /// 注册协议
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        public void RegisterProtocol(string key, Action<ProtocolInfo> action)
        {
            if (mProtocolDic.ContainsKey(key))
            {
                mProtocolDic[key] = null;
                mProtocolDic[key] = action;
            }
            else
            {
                mProtocolDic.Add(key, action);
            }
            Debug.Log(key + "<color=green>///Key Register成功</color>");
        }

        /// <summary>
        /// 卸载协议
        /// </summary>
        /// <param name="key"></param>
        public void UnRegisterProtocol(string key)
        {
            if (mProtocolDic.ContainsKey(key))
            {
                Debug.Log("<color=red>UnRegister" + key + "</color>");
                mProtocolDic[key] = null;
            }
        }

        #endregion
    }
}