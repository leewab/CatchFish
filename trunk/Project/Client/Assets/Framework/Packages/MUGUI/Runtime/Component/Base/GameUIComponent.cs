using System;
using System.Collections.Generic;
using Game.Core;
using MUGame;
using UnityEngine;

namespace Game.UI
{
    public class GameUIComponent : GameComponent
    {
        public bool FromPool { get; set; }

        private UIAnimatorComponent mAniComponent;
        
        public GameUIComponent(string name, Transform father = null) : base(name, father)
        {
            
        }
        
        public GameUIComponent(GameObject obj, Transform father = null) : base(obj, father)
        {
            
        }
        
        public GameUIComponent()
        {
            
        }

        protected override void Init(GameObject obj, Transform father = null)
        {
            base.Init(obj, father);

        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // public bool PlayAnimation(string aniname, Action lua_func = null, bool force = true, bool reset = true)
        // {
        //     if (mAniComponent == null) mAniComponent = CreateUIAnimationComponent();
        //     if (mAniComponent == null) return false;
        //     return this.mAniComponent.PlayAni(aniname, lua_func, force, reset);
        // }
        //
        // public void ClearAnimationEvent()
        // {
        //     if (mAniComponent == null) mAniComponent = CreateUIAnimationComponent();
        //     if (mAniComponent == null) return;
        //     this.mAniComponent.ClearAniEvent();
        // }

        // private UIAnimatorComponent CreateUIAnimationComponent()
        // {
        //     //每个节点都可以创造动画播放器，如果自己身上没有绑定UIAniPlayer，会使用祖先节点的UIAniPlayer
        //     //Unity提供的GetComponentInParent会忽略InActive的GameObject上的组件，GetComponentsInParent又不能确定顺序，所以自己写一个一路向上找的方法
        //     UIAniPlayer aniplr = null;
        //     var trans = gameObject.transform;
        //     while(aniplr == null && trans != null)
        //     {
        //         aniplr = trans.GetComponent<UIAniPlayer>();
        //         trans = trans.parent;
        //     }
        //     if (aniplr == null) return null;
        //     mAniComponent = new UIAnimatorComponent(aniplr);
        //     return mAniComponent;
        // }


#if UNITY_TOLUA
        
        //public void SetEventListener(int event_type, LuaInterface.LuaFunction func)
        //{
        //    UIEventListener.SetEventListener(event_type, func);
        //}
        
        //public void AddClickCallBack()
        //{
        //    UIEventListener.SetEventListener((int)UIEventType.OnClick, null);
        //}
        
        //public void RemoveClickCallBack()
        //{
        //    UIEventListener.RemoveClickCallBack();
        //}
        
#endif
         public virtual void AddClickCallBack(EventTriggerListener.VoidDelegate func)
         {
            UIEventListener.AddClickCallBack(func);
         }
         
        public virtual void RemoveClickCallBack(EventTriggerListener.VoidDelegate func)
        {
            UIEventListener.RemoveClickCallBack(func);
        }
        
        public virtual void ClearClickCallBack()
        {
            UIEventListener.ClearClickCallBack();
        }

        public virtual void AddDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            UIEventListener.AddDragCallBack(func);
        }
        
        public virtual void RemoveDragCallBack(EventTriggerListener.Vector2Delegate func)
        {
            UIEventListener.RemoveDragCallBack(func);
        }
       

        #region IComponentMaker<GameUIComponent>
        
        /// <summary>
        /// 创建(C#实现的)基础组件，并添加到自身的成员当中
        /// </summary>
        /// <param name="type">组件类型名称</param>
        /// <param name="path">组件的路径</param>
        /// <returns>创建出来的组件</returns>
        public GameUIComponent Make(string type, string path)
        {
            return base.LMake(type, path) as GameUIComponent;
        }
        
        public T Make<T>(string path) where T : GameComponent
        {
            return base.LMake<T>(path) as T;
        }
        
        public T Make<T>(GameObject node) where T : GameComponent, new()
        {
            return base.LMake<T>(node);
        }

        public GameUIComponent MakeLuaComponent(string path)
        {
           return base.LMakeLuaComponent(path) as GameUIComponent;
        }
        
        #endregion
        
        #region 公有方法

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <returns><c>true</c>, if property was set, <c>false</c> otherwise.</returns>
        /// <param name="propName">Property name.</param>
        /// <param name="propVal">Property value.</param>
        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = true;
            switch (key)
            {
                case UIProperty.Width:
                    this.Width = Convert.ToSingle(val);
                    break;
                case UIProperty.Height:
                    this.Height = Convert.ToSingle(val);
                    break;
                case UIProperty.AnchoredPosition:
                    var valueArr = (float[])val;
                    var valueVector = new Vector2(valueArr[0], valueArr[1]);
                    this.AnchoredPosition = valueVector;
                    break;
                default:
                    succ = base.SetPropertyImpl(key, val);
                    break;
            }
            
            return succ;
        }
        
        protected override bool GetPropertyImpl( UIProperty key, ref object ret)
        {
            bool succ = true;
            switch (key)
            {
                case UIProperty.Width:
                    ret = this.Width;
                    break;
                case UIProperty.Height:
                    ret = this.Height;
                    break;
                case UIProperty.AnchoredPosition:
                    ret = new[] { this.AnchoredPosition.x, this.AnchoredPosition.y };
                    break;
                default:
                    succ = base.GetPropertyImpl(key, ref ret);
                    break;
            }
            
            return succ;
        }
		
        public object GetPropertyByParam(int key, object param)
        {
            UIProperty p = (UIProperty)key;
            object ret = null;
            return base.GetPropertyByParamImpl(p, param, ref ret);
        }
        
        public override void ClearProperties()
        {
            base.ClearProperties();
            this.mRectTransform = null;
        }
        
        
        private Dictionary<string, object> mCustomDict;
        public void SetCustomData(string key, object val)
        {
            Dictionary<string, object> custom_dat = GetCustomDict();
            custom_dat[key] = val;
        }
        
        public object GetCustomData(string key)
        {
            Dictionary<string, object> custom_dat = GetCustomDict();
            if (custom_dat.ContainsKey(key))
            {
                return custom_dat[key];
            }
            else
            {
                return null;
            }
            
        }
        
        private Dictionary<string, object> GetCustomDict()
        {
            if( mCustomDict == null )
            {
                mCustomDict = new Dictionary<string, object>();
            }
            return mCustomDict;
        }

        #endregion

        #region 公共属性

        public float Width
        {
            get
            {
                if (this.RectTransform)
                {
                    return this.RectTransform.sizeDelta.x;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Width 失败, RectTransform为空!");
                    throw new Exception("GetProperty - Width 失败, RectTransform为空!");
                }
            }
            set
            {
                if (this.RectTransform)
                {
                    Vector2 old = this.RectTransform.sizeDelta;
                    this.RectTransform.sizeDelta = new Vector2(value, old.y);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Width 失败, RectTransform为空!");
                    throw new Exception("SetProperty - Width 失败, RectTransform为空!");
                }
            }
        }
        
        public float Height
        {
            get
            {
                if (this.RectTransform)
                {
                    return this.RectTransform.sizeDelta.y;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Height 失败, RectTransform为空!");
                    throw new Exception("GetProperty - Height 失败, RectTransform为空!");
                }
            }
            set
            {
                if (this.RectTransform)
                {
                    Vector2 old = this.RectTransform.sizeDelta;
                    this.RectTransform.sizeDelta = new Vector2(old.x, value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Height 失败, RectTransform为空!");
                    throw new Exception("SetProperty - Height 失败, RectTransform为空!");
                }
            }
        }

        private Vector2 mAnchoredPosition;
        public Vector2 AnchoredPosition
        {
            get
            {
                if (this.RectTransform)
                {
                    return this.RectTransform.anchoredPosition;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - AnchoredPosition 失败, RectTransform为空!");
                    throw new Exception("GetProperty - AnchoredPosition 失败, RectTransform为空!");
                }
            }
            set
            {
                if (this.RectTransform)
                {
                    this.RectTransform.anchoredPosition = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - AnchoredPosition 失败, RectTransform为空!");
                    throw new Exception("SetProperty - AnchoredPosition 失败, RectTransform为空!");
                }
            }
        }
        
        private RectTransform mRectTransform;
        public RectTransform RectTransform
        {
            get
            {
                if (this.mRectTransform == null && this.gameObject != null)
                {
                    this.mRectTransform = this.gameObject.GetComponent<RectTransform>();
                }

                return mRectTransform;
            }
        }
        
        public UILayerEnums UILayer { get; set; }

        #endregion
    }
}