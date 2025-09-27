using System;
using UnityEngine;

namespace Game
{
    public class EasyTouchProcess : IProcess
    {
        public string PreloadDesc => "EasyTouchProcess";
        public Action OnFinishedEvent { get; set; }

        public void Start()
        {
            //注册摇杆资源
            GameObject joystick = GameObject.Find("EasyTouchRoot");
            if (joystick != null)
            {
                MUEngine.MURoot.ResMgr.RegisterAsset("EasyTouchRoot.prefab", joystick);
            }
            
            this.OnFinishedEvent?.Invoke();
            this.OnFinishedEvent = null;
        }
        
    }
}