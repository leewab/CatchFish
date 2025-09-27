using UnityEngine;

namespace Game.Core
{
    public abstract class BaseMono : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            RegisterEvent();
        }

        protected virtual void OnDisable()
        {
            UnRegisterEvent();
        }
        
        protected virtual void RegisterEvent()
        {
            
        }

        protected virtual void UnRegisterEvent()
        {
            
        }
      
    }
}