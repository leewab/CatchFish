using Game.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    public class UIEventListener
    {
        private GameObject gameObject;
        
        public UIEventListener(GameObject obj)
        {
            this.gameObject = obj;
        }

        #region ClickCallBack

#if UNITY_TOLUA
	    
	    /// <summary>
	    /// Lua
	    /// </summary>
	    /// <returns></returns>
	    public int AddClickCallBack()
	    {
		    if (gameObject == null) return -1;
		    EventTriggerListener listener = EventTriggerListener.Get(this.gameObject);
		    listener.onClick = LuaUIUtil.UIClickHandler;
		    listener.event_id = LuaUIUtil.GetClickEventID();
		    return listener.event_id;
	    }
        
	    public void RemoveClickCallBack()
	    {
		    if (gameObject == null) return;
		    EventTriggerListener listener = EventTriggerListener.Get(this.gameObject);
		    listener.event_id = -1;
		    listener.onClick = null;
	    }
	    
#endif
        
        public virtual void AddClickCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.parameter = this;
	        if (GameConfig.OpenRecommender)
	        {
		        lis.onDown -= func;
		        lis.onDown += func;
	        }
	        else
	        {
		        lis.onClick -= func;
		        lis.onClick += func;
	        }
        }
		
        public virtual void RemoveClickCallBack(EventTriggerListener.VoidDelegate func)
        {
	        if (gameObject == null)
	        {
		        return;
	        }

	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        if (GameConfig.OpenRecommender)
	        {
		        lis.onDown -= func;
	        }
	        else
	        {
		        lis.onClick -= func;
	        }
        }

        public virtual void ClearClickCallBack()
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onClick = null;
        }

        #endregion


        
        #region PressDownCallBack

#if UNITY_TOLUA

        public int AddPressDownCallBack()
        {
	        if (gameObject == null) return -1;
	        EventTriggerListener listener = EventTriggerListener.Get(this.gameObject);
	        listener.onClick = LuaUIUtil.OnPressDownCallBack;
	        listener.event_id = LuaUIUtil.GetPressDownID();
	        return listener.event_id;
        }
        
        public void RemovePressDownCallBack()
        {
	        if (gameObject == null) return;
	        EventTriggerListener listener = EventTriggerListener.Get(this.gameObject);
	        listener.event_id = -1;
	        listener.onDown = null;
        }
#endif
        
        public int AddPressDownCallBack(EventTriggerListener.VoidDelegate func)
        {
	        if (gameObject == null) return -1;
	        EventTriggerListener listener = EventTriggerListener.Get(this.gameObject);
	        listener.onClick -= func;
	        listener.onClick += func;
	        return listener.event_id;
        }
        
        public void RemovePressDownCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onDown -= func;
        }
        
        public void ClearPressDownCallBack()
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onDown = null;
        }

        #endregion



        #region PressUpCallBack

        public int AddPressUpCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onUp += func;
	        return gameObject.GetInstanceID();
        }
        
        public void RemovePressUpCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onUp -= func;
        }
        
        public void ClearPressUpCallBack()
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onUp = null;
        }

        #endregion


        
        #region OverCallBack

        public int AddOverCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter += func;
	        return gameObject.GetInstanceID();
        }
        
        public void RemoveOverCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter -= func;
        }
        
        public void ClearOverCallBack()
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter = null;
        }

        #endregion




        #region EnterCallBack

        public int AddEnterCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter += func;
	        return gameObject.GetInstanceID();
        }

        public void RemoveEnterCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter -= func;
        }
        
        public void ClearEnterCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onEnter = null;
        }

        #endregion




        #region ExitCallBack

        public int AddExitCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onExit += func;
	        return gameObject.GetInstanceID();
        }

        public void RemoveExitCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onExit -= func;
        }
        
        public void ClearExitCallBack(EventTriggerListener.VoidDelegate func)
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onExit = null;
        }

        #endregion

        
        
        public int AddTwoFingerPinchZoomCallBack(EventTriggerListener.VectorFloatDelegate func)
        {
            TwoFingerPinchListener lis = TwoFingerPinchListener.Get(gameObject);
            lis.onPinchZoom += func;
            return gameObject.GetInstanceID();
        }

        public int AddTwoFingerPinchStartCallBack(EventTriggerListener.VectorDelegate func)
        {
            TwoFingerPinchListener lis = TwoFingerPinchListener.Get(gameObject);
            lis.onPinchStart += func;
            return gameObject.GetInstanceID();
        }

        public int AddDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDrag += func;
            return gameObject.GetInstanceID();
        }
        
        public void RemoveDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDrag -= func;
        }
        
        public int AddDragStartCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragStart += func;
            return gameObject.GetInstanceID();
        }
        
        public void RemoveDragStartCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragStart -= func;
        }

        public int AddDragEndCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragEnd += func;
            return gameObject.GetInstanceID();
        }
        
        public void RemoveDragEndCallBack(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener lis = DragEventTriggerListener.Get(gameObject);
            lis.onDragEnd -= func;
        }

        public void AddDragStartCallBack3(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener3 lis = DragEventTriggerListener3.Get(gameObject);
            lis.onDragStart += func;
        }
       
        public void AddDragEndCallBack3(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener3 lis = DragEventTriggerListener3.Get(gameObject);
            lis.onDragEnd += func;
        }

        public void AddDragCallBack2(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDrag += func;
        }
        
        public void RemoveDragCallBack2(EventTriggerListener.Vector2Delegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDrag -= func;
        }

        public void AddDragStartCallBack2(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDragStart += func;
        }
        
        public void RemoveDragStartCallBack2(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDragStart -= func;
        }

        public void AddDragEndCallBack2(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDragEnd += func;
        }
        
        public void RemoveDragEndCallBack2(EventTriggerListener.VectorDelegate func)
        {
            DragEventTriggerListener2 lis = DragEventTriggerListener2.Get(gameObject);
            lis.onDragEnd -= func;
        }

        public int AddDoubleClickCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onDoubleClick -= func;
            lis.onDoubleClick += func;
            return gameObject.GetInstanceID();
        }
        
        public void RemoveDoubleClickCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onDoubleClick -= func;
        }
        
        public void ClearDoubleClickCallBack()
        {
	        EventTriggerListener lis = EventTriggerListener.Get(gameObject);
	        lis.onDoubleClick = null;
        }
        
        public void AddPressurePressCallBack(System.Action<GameObject, float, bool> func)
        {
            EventTriggerPressurePressListener lis = EventTriggerPressurePressListener.Get(gameObject);
            lis.parameter = this;
            lis.onPressurePress -= func;
            lis.onPressurePress += func;
        }
        
        public void AddLongPressCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerPressListener lis = EventTriggerPressListener.Get(gameObject);
            lis.parameter = this;
            lis.onLongPress -= func;
            lis.onLongPress += func;
        }
        
        public void RemoveLongPressCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerPressListener lis = EventTriggerPressListener.Get(gameObject);
            lis.onLongPress -= func;
        }

        public void AddDragClickCallBack(EventTriggerDargClickListener.VoidDelegate func)
        {
            EventTriggerDargClickListener lis = EventTriggerDargClickListener.Get(gameObject);
            lis.parameter = this;
            lis.onClick -= func;
            lis.onClick += func;
        }

        public void AddClickCallBackPosition(EventTriggerListener.VectorDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.parameter = this;
            lis.onClickPosition -= func;
            lis.onClickPosition += func;
        }

		public void AddValueChangedCallBack(EventTriggerListener.VoidDelegate func)
		{
			EventTriggerListener lis = EventTriggerListener.Get(gameObject);
			lis.parameter = this;
			lis.onClick -= func;
			lis.onClick += func;
		}

		public void RemoveValueChangedCallBack(EventTriggerListener.VoidDelegate func)
		{
			EventTriggerListener lis = EventTriggerListener.Get(gameObject);
			lis.onClick -= func;
		}

        public int AddSelectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onSelect -= func;
            lis.onSelect += func;
            return gameObject.GetInstanceID();
        }
        
        public void RemoveSelectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onSelect -= func;
        }

        public void AddDeselectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onDeselect -= func;
            lis.onDeselect += func;
        }
        
        public void RemoveDeselectCallBack(EventTriggerListener.VoidDelegate func)
        {
            EventTriggerListener lis = EventTriggerListener.Get(gameObject);
            lis.onDeselect -= func;
        }
        
        public void ForbidLongPressOnClick()
        {
	        EventTriggerPressListener lis = EventTriggerPressListener.Get(gameObject);
	        lis.ForbidOnClick();
        }


        #region LuaInterface

#if UNITY_TOLUA
        
        private object[] mFuncMap = new object[(int)UIEventType.Count];

        /// <summary>
        /// 清空所有lua callback
        /// </summary>
        public void ClearLuaEvents()
        {
	        for (int k = 0; k < mFuncMap.Length; ++k)
	        {
		        object func = mFuncMap[k];
		        if (func != null)
		        {
			        ClearEventListener(k);
		        }
		        mFuncMap[k] = null;
	        }
        }
        
        public void ClearAllListener()
        {
	        this.ClearLuaEvents();
	        if(gameObject == null)
	        {
		        return;
	        }
	        foreach(var monobehaviour in gameObject.GetComponents<MonoBehaviour>())
	        {
		        if(monobehaviour is IEventSystemHandler)
		        {
			        UnityEngine.Object.Destroy(monobehaviour);
		        }
	        }
        }

		public int SetEventListener(int event_type, LuaInterface.LuaFunction func)
		{
			int event_id = -1;
			//删除已存在的callback
			if (mFuncMap[event_type] != null)
			{
				RemoveEventListener(event_type);
                mFuncMap[event_type] = null;
			}
            
			if (event_type == (int)UIEventType.OnClick)
			{
				event_id = this.AddClickCallBack();
				mFuncMap[event_type] = event_type;
				return event_id;
			}
			
			if (func == null) return event_id;
			if (event_type == (int)UIEventType.OnPressDown)
			{
				EventTriggerListener.VoidDelegate funcwrap = CreateVoidDelegate(func);
				event_id = this.AddPressDownCallBack(funcwrap);
				mFuncMap[event_type] = event_type;
			}
			else if (event_type == (int)UIEventType.OnPressUp)
			{
				EventTriggerListener.VoidDelegate funcwrap = CreateVoidDelegate(func);
				event_id = this.AddPressUpCallBack(funcwrap);
				mFuncMap[event_type] = funcwrap;
			}
			else if (event_type == (int)UIEventType.OnDrag)
			{
				EventTriggerListener.Vector2Delegate funcwrap = CreateVector2Delegate(func);
				event_id = this.AddDragCallBack(funcwrap);
				mFuncMap[event_type] = funcwrap;
			}
			else if (event_type == (int)UIEventType.OnBeginDrag)
			{
				EventTriggerListener.VectorDelegate funcwrap = CreateVectorDelegate(func);
				event_id = this.AddDragStartCallBack(funcwrap);
				mFuncMap[event_type] = funcwrap;
			}
			else if (event_type == (int)UIEventType.OnEndDrag)
			{
				EventTriggerListener.VectorDelegate funcwrap = CreateVectorDelegate(func);
				event_id = this.AddDragEndCallBack(funcwrap);
				mFuncMap[event_type] = funcwrap;
			}
            else if (event_type == (int)UIEventType.OnDoubleClick)
            {
                EventTriggerListener.VoidDelegate funcwrap = CreateVoidDelegate(func);
                event_id = this.AddDoubleClickCallBack(funcwrap);
                mFuncMap[event_type] = funcwrap;
            }
			
			return event_id;
		}		
		
		public void RemoveEventListener(int event_id)
		{
			if (mFuncMap == null) return;
			//删除已存在的callback
			if (mFuncMap[event_id] != null)
			{
				switch (event_id)
				{
					case (int)UIEventType.OnClick:
						this.RemoveClickCallBack();
						break;
					case (int)UIEventType.OnPressDown:
						this.RemovePressDownCallBack((EventTriggerListener.VoidDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnPressUp:
						this.RemovePressUpCallBack((EventTriggerListener.VoidDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnDrag:
						this.RemoveDragCallBack((EventTriggerListener.Vector2Delegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnBeginDrag:
						this.RemoveDragStartCallBack((EventTriggerListener.VectorDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnEndDrag:
						this.RemoveDragEndCallBack((EventTriggerListener.VectorDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnDoubleClick:
						this.RemoveDoubleClickCallBack((EventTriggerListener.VoidDelegate)mFuncMap[event_id]);
						break;
				}
				
                mFuncMap[event_id] = null;
			}
		}
		
		public void ClearEventListener(int event_id)
		{
			if (mFuncMap == null) return;
			//删除已存在的callback
			if (mFuncMap[event_id] != null)
			{
				switch (event_id)
				{
					case (int)UIEventType.OnClick:
						this.ClearClickCallBack();
						break;
					case (int)UIEventType.OnPressDown:
						this.ClearPressDownCallBack();
						break;
					case (int)UIEventType.OnPressUp:
						this.ClearPressUpCallBack();
						break;
					case (int)UIEventType.OnDrag:
						this.RemoveDragCallBack((EventTriggerListener.Vector2Delegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnBeginDrag:
						this.RemoveDragStartCallBack((EventTriggerListener.VectorDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnEndDrag:
						this.RemoveDragEndCallBack((EventTriggerListener.VectorDelegate)mFuncMap[event_id]);
						break;
					case (int)UIEventType.OnDoubleClick:
						this.ClearDoubleClickCallBack();
						break;
				}
				
				mFuncMap[event_id] = null;
			}
		}
		
		private EventTriggerListener.VoidDelegate CreateVoidDelegate(LuaInterface.LuaFunction func)
		{
			EventTriggerListener.VoidDelegate funcwrap = DelegateFactory.CreateDelegate(typeof(EventTriggerListener.VoidDelegate), func) as EventTriggerListener.VoidDelegate;
			return funcwrap;
		}
		private EventTriggerListener.Vector2Delegate CreateVector2Delegate(LuaInterface.LuaFunction func)
		{
			EventTriggerListener.Vector2Delegate funcwrap = DelegateFactory.CreateDelegate(typeof(EventTriggerListener.Vector2Delegate), func) as EventTriggerListener.Vector2Delegate;
			return funcwrap;
		}
		private EventTriggerListener.VectorDelegate CreateVectorDelegate(LuaInterface.LuaFunction func)
		{
			EventTriggerListener.VectorDelegate funcwrap = DelegateFactory.CreateDelegate(typeof(EventTriggerListener.VectorDelegate), func) as EventTriggerListener.VectorDelegate;
			return funcwrap;
		}

#endif
        #endregion
    }
}