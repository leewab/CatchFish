using System;
using Game.Core;
using Game.UI;
using UnityEngine;

namespace Game
{
    public enum AlertType
    {
        Normal,   
    }

    public class AlertInfo
    {
        // 显示标题
        public string Title;
        // 显示描述内容
        public string Content;
        // 显示背景图
        public string ImgBg;
        // 确认
        public string Confirm;
        // 取消
        public string Cancel;
        // 其他参数
        public string Param;
    }
    
    public class AlertInfoHandler : BaseHandler
    {
        public static AlertInfoHandler Instance => HandlerModule.AlertInfoHandler;
        
        public void Start(AlertType alertType, AlertInfo alertInfo, Action<bool> callBack)
        {
            var uiAlertView = UIManager.Instance.OpenUI<UIAlertView>(UILayerEnums.UILoadingLayer);
            if (uiAlertView != null)
            {
                uiAlertView.OnOperationCallBack += result =>
                {
                    Debug.LogError("加载完毕");
                    // UIManager.Instance.OpenPanel<UILoginPanel>();
                    callBack?.Invoke(result);
                }; 
            }
        }


    }
}