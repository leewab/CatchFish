using UnityEngine;

namespace Game
{
    public class BaseLogicComponent
    {
        private bool mIsEnable = false;
        public bool IsEnable => mIsEnable;
        
        protected GameObject gameObject;

        public virtual void Init(GameObject obj)
        {
            if (this.mIsEnable) return;
            this.mIsEnable = true;
            this.gameObject = obj;
            this.RegisterEvent();
        }
        
        public virtual void Dispose()
        {
            this.mIsEnable = false;
            this.UnRegisterEvent();
        }
        
        public virtual void Update()
        {
           
        }
        
        public virtual void LogicUpdate()
        {
            
        }
        
        public virtual void FixedUpdate()
        {
            
        }

        public virtual void RegisterEvent()
        {

        }

        public virtual void UnRegisterEvent()
        {

        }

    }
}