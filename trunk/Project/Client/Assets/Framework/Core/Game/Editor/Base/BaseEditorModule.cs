using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class BaseEditorModule
    {
        
        private DelayHelper mDelayHelper;
        
        public void Delay(string key, TimeData timeData)
        {
            mDelayHelper?.AddDelayListener(key, timeData);
        }
        
        public virtual void Awake()
        {
            if (mDelayHelper == null) mDelayHelper = new DelayHelper();
        }
        
        public virtual void Update()
        {
            mDelayHelper.Update();
        }

        public virtual void Clear()
        {
            
        }
    }
}