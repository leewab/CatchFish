using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UIManager : Singleton<UIManager>
    {
        //内存中缓存的最大UI数量
        private const int uiMaxNumInCPU = 10;

        //用于缓存当前打开的面板
        private readonly Dictionary<string, GamePanel> uiPanelDic = new Dictionary<string, GamePanel>();

        //缓存关闭隐藏于场景（不销毁）的面板
        private readonly Queue<GamePanel> uiPanelQueue = new Queue<GamePanel>();

        //缓存弹窗面板的层级管理
        private readonly Stack<GamePanel> uiPanelStack = new Stack<GamePanel>();

        #region OpenPanel

        private T OpenUI<T>(string uiName, Transform father = null, params object[] data) where T : GamePanel, new()
        {
            var uiPanel = GenerateUI<T>(uiName, father);
            uiPanel?.Open(data);
            return uiPanel;
        }

        public T OpenUI<T>(UILayerEnums uiLayer, params object[] data) where T : GamePanel, new()
        {
            return OpenUI<T>(typeof(T).Name, uiLayer, data);
        }

        public T OpenUI<T>(string uiName, UILayerEnums uiLayer, params object[] data) where T : GamePanel, new()
        {
            switch (uiLayer)
            {
                case UILayerEnums.UIBaseLayer:
                    return OpenUI<T>(uiName, UIRoot.UIBaseLayer.transform, data);
                case UILayerEnums.UIGameLayer:
                    return OpenUI<T>(uiName, UIRoot.UIGameLayer.transform, data);
                case UILayerEnums.UIDefaultLayer:
                    return OpenUI<T>(uiName, UIRoot.UIDefaultLayer.transform, data);
                case UILayerEnums.UIPopupLayer:
                    var panel = OpenUI<T>(uiName, UIRoot.UIPopupLayer.transform, data);
                    PushUI(panel);
                    return panel;
                case UILayerEnums.UINoticeLayer:
                    return OpenUI<T>(uiName, UIRoot.UINoticeLayer.transform, data);
                case UILayerEnums.UILoadingLayer:
                    return OpenUI<T>(uiName, UIRoot.UILoadingLayer.transform, data);
                default:
                    return OpenUI<T>(uiName, UIRoot.UIDefaultLayer.transform, data);
            }
        }

        #endregion

        public T GenerateUI<T>(string panelName, Transform father) where T : GamePanel, new()
        {
            GamePanel gamePanel;
            if (!uiPanelDic.TryGetValue(panelName, out gamePanel))
            {
                if (!panelName.EndsWith(".prefab")) panelName = panelName + ".prefab";
                gamePanel = UIComponentFactory.MakePanel<T>(panelName, father);
                uiPanelDic.Add(panelName, gamePanel);
            }

            if (gamePanel == null) return null;
            return gamePanel as T;
        }

        
        #region ClosePanel

        public void Close<T>(T t, bool isDestroy = false) where T : GamePanel
        {
            t.Close(isDestroy);
            if (isDestroy)
            {
                RemoveUI(t);
            }
            else
            {
                EnqueueUI(t);
            }

            if (uiPanelQueue.Count > uiMaxNumInCPU) DisposeUI();
        }

        public void Close(string uiName, bool isDestroy = false)
        {
            GamePanel baseView;
            if (uiPanelDic.TryGetValue(uiName, out baseView))
            {
                baseView.Close(isDestroy);
                if (isDestroy)
                {
                    RemoveUI(baseView);
                }
                else
                {
                    EnqueueUI(baseView);
                }

                if (uiPanelQueue.Count > uiMaxNumInCPU) DisposeUI();
            }
        }

        #endregion


        #region HasPanel

        public bool HasOpenPanel<T>() where T : GamePanel
        {
            return HasOpenPanel(typeof(T).Name);
        }

        public bool HasPanel<T>() where T : GamePanel
        {
            return HasPanel(typeof(T).Name);
        }

        public bool HasOpenPanel(string uiName)
        {
            GamePanel baseView;
            if (uiPanelDic.TryGetValue(uiName, out baseView))
            {
                return baseView.IsOpened;
            }

            return false;
        }

        public bool HasPanel(string uiName)
        {
            return uiPanelDic.ContainsKey(uiName);
        }

        #endregion


        #region PopupPanel

        public void PushUI<T>(T t) where T : GamePanel
        {
            uiPanelStack.Push(t);
        }

        public T PopUI<T>() where T : GamePanel
        {
            return uiPanelStack.Pop() as T;
        }

        #endregion

        private void EnqueueUI<T>(T t) where T : GamePanel
        {
            if (uiPanelQueue.Contains(t))
            {
                uiPanelQueue.Peek();
            }

            uiPanelQueue.Enqueue(t);
        }

        private void RemoveUI<T>(T t) where T : GamePanel
        {
            if (uiPanelQueue.Contains(t))
            {
                uiPanelQueue.Peek();
            }
        }

        private void DisposeUI()
        {
            uiPanelQueue.Peek().Close(true);
        }
        
    }
}