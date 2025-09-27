using System;
using Game;
using Game.Core;
using Game.UI;
using MUEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class UIProcess : IProcess
    {
        public const string UI_ROOT_NAME = "UIRoot";
        public const string UI_CAMERA_NAME = "UICamera";
        public const string UI_EVENTSYSTEM_NAME = "EventSystem";

        private void LoginGame()
        {
            // 打开Loading
            LoadingHandler.Instance.Start(LoadingType.LoginLoading, null);
            // 打开登录界面
            UIManager.Instance.OpenUI<UILoginView>(UILayerEnums.UIDefaultLayer);
            // 注册摇杆资源
            GameObject joystick = GameObject.Find("EasyTouchRoot");
            if (joystick != null)
            {
                MUEngine.MURoot.ResMgr.RegisterAsset("EasyTouchRoot.prefab", joystick);
            }
        }
        
        private void OnLoadedUIRoot(string name, Object cObj)
        {
            if (cObj == null) throw new Exception("加载失败: " + UI_ROOT_NAME);
            GameObject obj = cObj as GameObject;
            if (obj == null) throw new Exception(UI_ROOT_NAME + " 转GameObject失败！");
            obj.name = UI_ROOT_NAME;
            obj.AddOneComponent<UIRoot>().InitUIRoot();
            obj.SetActive(true);
            this.LoginGame();
            this.Finish();
        }

        private void Finish()
        {
            this.OnFinishedEvent?.Invoke();
            this.OnFinishedEvent = null;
        }

        public string PreloadDesc => "UIProcess";
        public Action OnFinishedEvent { get; set; }

        public void Start()
        {
            MURoot.ResMgr.GetAsset(UI_ROOT_NAME + ".prefab", OnLoadedUIRoot, LoadPriority.HighPrior, ECacheType.AutoDestroy);
        }
        
    }
}