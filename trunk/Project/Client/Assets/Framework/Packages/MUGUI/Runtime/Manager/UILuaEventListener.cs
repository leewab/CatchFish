using UnityEngine;

namespace Game.UI
{
    class UILuaEventListener
	{
        
        private GameObject gameObject;
        private GameComponent mComponent;
        
		public UILuaEventListener(GameComponent component)
		{
			if (component == null || component.gameObject == null) return;
			this.mComponent = component;
			this.gameObject = component.gameObject;
		}

        public GameComponent TargetComponent
        {
            set => mComponent = value;
            get => mComponent;
        }
        
        

	}
}