using System;
using System.Collections.Generic;
using Game.Core;
using Game.UI;
using UnityEngine;

namespace Game
{
    public class GameComponent : IComponentMaker<GameComponent> //, ILife
    {
        public bool IsInit = false;
        public bool ResLoaded = false;
        public Action<string, GameObject> ResLoadedCallBack;
        public GameObject gameObject { get; private set; }
        protected List<GameComponent> mChildren = null;

        private ResLoader mResLoader = null;
        
        public GameComponent(string name, Transform parent = null)
        {
            this.Load(name, parent);
        }
        
        public GameComponent(GameObject obj, Transform parent = null)
        {
            this.Init(obj, parent);
        }
        
        public GameComponent()
        {
            
        }
        
        protected virtual void Init(GameObject obj, Transform parent)
        {
            if (this.gameObject != null) return;
            if (!obj)
            {
                ExceptionHelper.ThrowExceptionToBroadcast("gameObject为空!");
                throw new Exception("gameObject为空!");
            }
            
            this.gameObject = obj;
            this.Parent = parent;
            this.IsInit = true;
            this.OnInit();
        }
        
        /// <summary>
        /// 释放数据结构
        /// </summary>
        public virtual void Dispose()
        {
            this.IsInit = false;
            this.ResLoaded = false;
            this.ClearProperties();
            this.ClearLuaProperties();
            this.UnLoad();
            if (this.mChildren != null)
            {
                foreach (GameComponent i in this.mChildren)
                {
                    i.Dispose();
                }

                this.mChildren = null;
            }
            this.mResLoader.Dispose();
            this.gameObject = null;
            this.OnDispose();
        }

        #region 自定义生命周期 该生命周期由C#驱动同步到Lua，确保Unity资源到位之后同步

        public Action OnInitEvent;
        public Action OnShowEvent;
        public Action OnHideEvent;
        public Action OnDisposeEvent;

        protected virtual void OnInit()
        {
            this.OnInitEvent?.Invoke();
        }

        protected virtual void OnShow()
        {
            this.OnShowEvent?.Invoke();
        }

        protected virtual void OnHide()
        {
            this.OnHideEvent?.Invoke();
        }

        protected virtual void OnDispose()
        {
            this.OnDisposeEvent?.Invoke();
        }

        #endregion


        #region 公共方法

        public virtual void ClearProperties()
        {
            // this.gameObject = null;
            // this.ClearCustomDict();
        }
        
        public virtual void ClearLuaProperties()
        {
#if UNITY_TOLUA
            LuaInterface.ObjectTranslator.Get(IntPtr.Zero).RemoveComponentObject(this);
            if (UIEventListener != null) UIEventListener.ClearLuaEvents();
#endif
        }
        
        public virtual bool Load(string name, Transform father = null)
        {
            if (this.mResLoader == null) this.mResLoader = new ResLoader();
            return this.mResLoader.Load(name, (_name, _obj) =>
            {
                this.OnLoadEvent(_name, _obj, father);
            });
        }

        public virtual void UnLoad()
        {
            this.ResLoaded = false;
            if (this.mResLoader != null) this.mResLoader.UnLoad();
        }
        
        private void OnLoadEvent(string name, GameObject obj, Transform parent)
        {
            this.ResLoaded = obj != null;
            this.Init(obj, parent);
            this.gameObject.transform.localPosition = Vector3.zero;
            this.gameObject.transform.localScale = Vector3.one;
            this.ResLoadedCallBack?.Invoke(name, obj);
            this.ResLoadedCallBack = null;
        }

        public void AddChildComponent(GameComponent child, GameComponent parent)
        {
            if (mChildren == null) mChildren = new List<GameComponent>();
            if (!mChildren.Contains(child)) mChildren.Add(child);
            if (parent != null)
            {
                child.ParentComponent = parent;
            }
        }
        
        public void RemoveChildComponent(GameComponent child)
        {
            if (mChildren == null || child == null) return;
            if (mChildren.Contains(child)) mChildren.Remove(child);
            child.Dispose();
        }
        
        public List<GameComponent> GetChildCSharpComponents()
        {
            return mChildren;
        }
        
        public int GetChildCSharpComponentsCount()
        {
            return mChildren?.Count ?? 0;
        }
        
        /// <summary>
        /// 实际上，这个函数是 GetComponentInChildren ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            T res = gameObject.GetComponent<T>();
            if (res == null)
            {
                T[] array = gameObject.GetComponentsInChildren<T>(true);
                if (array.Length > 0)
                    res = array[0];
            }
            if (res == null)
            {
                //D.warn("Cannot find any {0} component in {1}.", typeof(T), gameObject);
            }
            return res;
        }
        
        /// <summary>
        /// 实际上，这是 GetComponentsInChildren ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] GetComponents<T>() where T : Component
        {
            T[] array = gameObject.GetComponentsInChildren<T>(true);
            //if (array.Length > 0)
            //    res = array[0];
            return array;
        }

        public Component GetMonoComponent(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) throw new Exception("GetComponent 类型名称为空！");
            Type component = AssemblyHelper.GetAssemblyType(fullName);
            if (component == null) return null;
            return this.gameObject.GetComponent(component);
        }
        
        public Component SetMonoComponent(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) throw new Exception("SetComponent 类型名称为空！");
            Type component = AssemblyHelper.GetAssemblyType(fullName);
            if (component == null) return null;
            return this.gameObject.AddComponent(component);
        }
        
        public void SetProperty(int key, object val)
        {
            UIProperty p = (UIProperty)key;
            bool succ = this.SetPropertyImpl(p, val);
            if (!succ)
            {
                ExceptionHelper.ThrowExceptionToBroadcast("Property " + p.ToString() + " Not Exist!");
                // throw new LuaException("Property " + p.ToString() + " Not Exist!");
            }
        }
        
        public object GetProperty(int key)
        {
            UIProperty p = (UIProperty)key;
            object ret = null;
            bool succ = this.GetPropertyImpl(p, ref ret);
            if (!succ)
            {
                ExceptionHelper.ThrowExceptionToBroadcast("Property " + p.ToString() + " Not Exist!");
                // throw new LuaException("Property " + p.ToString() + " Not Exist!");
            }
            return ret;
        }

        #endregion
        
        /// <summary>
        /// 改变Visible并且不触发关闭动画
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetVisible(bool value)
        {
            if (Visible != value) this.gameObject.SetActive(value);
            if (value)
            {
                OnShow();
            }
            else
            {
                OnHide();
            }
        }
        
        public virtual void SetAsLastSibling()
        {
            if (this.gameObject)
            {
                this.gameObject.transform.SetAsLastSibling();
            }
        }

        public virtual void SetAsFirstSibling()
        {
            if (this.gameObject)
            {
                this.gameObject.transform.SetAsFirstSibling();
            }
        }
        
        protected virtual bool SetPropertyImpl(UIProperty key, object val)
		{
			bool succ = true;
            switch (key)
            {
                case UIProperty.Name:
                    this.Name = val.ToString();
                    break;
                case UIProperty.Visible:
                    this.Visible = (bool)val;
                    break;
                case UIProperty.Layer:
                    int newLayer = int.Parse(val.ToString());
                    this.Layer = newLayer;
                    break;
                case UIProperty.Tag:
                    string tagStr = val.ToString();
                    this.Tag = tagStr;
                    break;
                case UIProperty.Scale:
                    Vector3 valueScale = Convert.ToSingle(val) * Vector3.one;
                    if (valueScale != this.Scale) this.Scale = valueScale;
                    break;
                case UIProperty.ScaleX:
                    Vector3 valueScaleX = new Vector3(Convert.ToSingle(val), Scale.y, Scale.z);
                    if (valueScaleX != this.Scale) this.Scale = valueScaleX;
                    break;
                case UIProperty.ScaleY:
                    Vector3 valueScaleY = new Vector3(Scale.x, Convert.ToSingle(val), Scale.z);
                    if (valueScaleY != this.Scale) this.Scale = valueScaleY;
                    break;
                case UIProperty.ScaleZ:
                    Vector3 valueScaleZ = new Vector3(Scale.x, Scale.y, Convert.ToSingle(val));
                    if (valueScaleZ != this.Scale) this.Scale = valueScaleZ;
                    break;
                case UIProperty.Position:
                    var valueArr = (float[])val;
                    var valueVector = new Vector3(valueArr[0], valueArr[1], valueArr[2]);
                    if (valueVector != this.Position) this.Position = valueVector;
                    break;
                case UIProperty.PositionX:
                    float positionX = Convert.ToSingle(val);
                    if (Math.Abs(positionX - this.Position.x) > 0) this.Position = new Vector3(positionX, this.Position.y, this.Position.z);
                    break;
                case UIProperty.PositionY:
                    float positionY = Convert.ToSingle(val);
                    if (Math.Abs(positionY - this.Position.y) > 0) this.Position = new Vector3(this.Position.x, positionY, this.Position.z);
                    break;
                case UIProperty.PositionZ:
                    float positionZ = Convert.ToSingle(val);
                    if (Math.Abs(positionZ - this.Position.z) > 0) this.Position = new Vector3(this.Position.x, this.Position.y, positionZ);
                    break;
                case UIProperty.LocalPosition:
                    var valueLocalArr = (float[])val;
                    var valueLocalVector = new Vector3(valueLocalArr[0], valueLocalArr[1], valueLocalArr[2]);
                    if (valueLocalVector != this.LocalPosition) this.LocalPosition = valueLocalVector;
                    break;
                case UIProperty.LocalPositionX:
                    float localPositionX = Convert.ToSingle(val);
                    if (Math.Abs(localPositionX - this.LocalPosition.x) > 0) this.Position = new Vector3(localPositionX, this.LocalPosition.y, this.LocalPosition.z);
                    break;
                case UIProperty.LocalPositionY:
                    float localPositionY = Convert.ToSingle(val);
                    if (Math.Abs(localPositionY - this.LocalPosition.y) > 0) this.Position = new Vector3(this.LocalPosition.x, localPositionY, this.LocalPosition.z);
                    break;
                case UIProperty.LocalPositionZ:
                    float localPositionZ = Convert.ToSingle(val);
                    if (Math.Abs(localPositionZ - this.LocalPosition.z) > 0) this.LocalPosition = new Vector3(this.LocalPosition.x, this.LocalPosition.y, localPositionZ);
                    break;
                case UIProperty.Rotation:
                    var valueRotationArr = (float[])val;
                    var vectorRotation = new Vector3(valueRotationArr[0], valueRotationArr[1], valueRotationArr[2]);
                    var valueRotationQuaternion1 = Quaternion.Euler(vectorRotation);
                    if (valueRotationQuaternion1 != this.Rotation) this.Rotation = valueRotationQuaternion1;
                    break;
                case UIProperty.RotationQuaternion:
                    var valueRotationQuaternionArr = (float[])val;
                    var valueRotationQuaternion2 = new Quaternion(valueRotationQuaternionArr[0], valueRotationQuaternionArr[1], valueRotationQuaternionArr[2], valueRotationQuaternionArr[3]);
                    if (valueRotationQuaternion2 != this.Rotation) this.Rotation = valueRotationQuaternion2;
                    break;
                case UIProperty.LocalRotation:
                    var valueLocalRotationArr = (float[])val;
                    var vectorLocalRotation = new Vector3(valueLocalRotationArr[0], valueLocalRotationArr[1], valueLocalRotationArr[2]);
                    var valueLocalRotationQuaternion1 = Quaternion.Euler(vectorLocalRotation);
                    if (valueLocalRotationQuaternion1 != this.LocalRotation) this.LocalRotation = valueLocalRotationQuaternion1;
                    break;
                case UIProperty.LocalRotationQuaternion:
                    var valueLocalRotationQuaternionArr = (float[])val;
                    var valueLocalRotationQuaternion2 = new Quaternion(valueLocalRotationQuaternionArr[0], valueLocalRotationQuaternionArr[1], valueLocalRotationQuaternionArr[2], valueLocalRotationQuaternionArr[3]);
                    if (valueLocalRotationQuaternion2 != this.LocalRotation) this.LocalRotation = valueLocalRotationQuaternion2;
                    break;
                default:
                    succ = false;
                    break;

            }
            
			return succ;
		}
        
		protected virtual bool GetPropertyImpl(UIProperty key, ref object ret)
		{
            //TODO: 这里的object类型转换还没有搞清楚Lua传递的性能问题，后续搞清楚之后确定是通过string传递然后解析还是直接传递数据结构，然后进行强制转换
			bool succ = true;
            switch (key)
            {
                case UIProperty.Name:
                    ret = this.Name;
                    break;
                case UIProperty.Visible:
                    ret = this.Visible;
                    break;
                case UIProperty.Layer:
                    ret = this.Layer;
                    break;
                case UIProperty.Tag:
                    ret = this.Tag;
                    break;
                case UIProperty.Scale:
                    ret = new[] { this.Scale.x, this.Scale.y, this.Scale.z };
                    break;
                case UIProperty.ScaleX:
                    ret = this.Scale.x;
                    break;
                case UIProperty.ScaleY:
                    ret = this.Scale.y;
                    break;
                case UIProperty.ScaleZ:
                    ret = this.Scale.z;
                    break;
                case UIProperty.Position:
                    ret = new[] { this.Position.x, this.Position.y, this.Position.z };
                    break;
                case UIProperty.PositionX:
                    ret = this.Position.x;
                    break;
                case UIProperty.PositionY:
                    ret = this.Position.y;
                    break;
                case UIProperty.PositionZ:
                    ret = this.Position.z;
                    break;
                case UIProperty.LocalPosition:
                    ret = new[] { this.LocalPosition.x, this.LocalPosition.y, this.LocalPosition.z };
                    break;
                case UIProperty.LocalPositionX:
                    ret = this.LocalPosition.x;
                    break;
                case UIProperty.LocalPositionY:
                    ret = this.LocalPosition.y;
                    break;
                case UIProperty.LocalPositionZ:
                    ret = this.LocalPosition.z;
                    break;
                case UIProperty.Rotation:
                    var rotationVector = this.Rotation.eulerAngles;
                    ret = new[] { rotationVector.x, rotationVector.y, rotationVector.z };
                    break;
                case UIProperty.RotationQuaternion:
                    ret = new[] { this.Rotation.x, this.Rotation.y, this.Rotation.z, this.Rotation.w };
                    break;
                case UIProperty.LocalRotation:
                    var localRotationVector = this.LocalRotation.eulerAngles;
                    ret = new[] { localRotationVector.x, localRotationVector.y, localRotationVector.z };
                    break;
                case UIProperty.LocalRotationQuaternion:
                    ret = new[] { this.LocalRotation.x, this.LocalRotation.y, this.LocalRotation.z, this.LocalRotation.w };
                    break;
                default:
                    succ = false;
                    break;

            }
            
			return succ;
		}
        
        protected virtual bool GetPropertyByParamImpl(UIProperty key, object param, ref object ret)
        {
            ret = null;
            return false;
        }
        

        #region 公共属性
        
        public int InstanceID
        {
            get
            {
                if (this.gameObject)
                {
                    return gameObject.GetInstanceID();
                }
                
                return -1;
            }
        }
        
        public virtual string Name
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.name;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Name 失败, gameObject为空!");
                    throw new Exception("GetProperty - Name 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    if (gameObject.name.Equals(value)) gameObject.name = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Name 失败, gameObject为空!");
                    throw new Exception("GetProperty - Name 失败, gameObject为空!");
                }
            }
        }

        public virtual bool Visible
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.activeSelf;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Visible 失败, gameObject为空!");
                    throw new Exception("GetProperty - Visible 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    SetVisible(value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Visible 失败, gameObject为空!");
                    throw new Exception("GetProperty - Visible 失败, gameObject为空!");
                }
            }
        }
        
        public virtual int Layer
        {
            get
            {
                if (gameObject)
                {
                    return this.gameObject.layer;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Layer 失败, gameObject为空!");
                    throw new Exception("GetProperty - Layer 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    if (value != this.gameObject.layer) this.gameObject.layer = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Layer 失败, gameObject为空!");
                    throw new Exception("GetProperty - Layer 失败, gameObject为空!");
                }
            }
        }
        
        public virtual string Tag
        {
            get
            {
                if (gameObject)
                {
                    return this.gameObject.tag;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Tag 失败, gameObject为空!");
                    throw new Exception("GetProperty - Tag 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    if (!this.gameObject.CompareTag(value)) this.gameObject.tag = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Tag 失败, gameObject为空!");
                    throw new Exception("GetProperty - Tag 失败, gameObject为空!");
                }
            }
        }
        
        public Vector3 Scale
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.transform.localScale;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Scale 失败, gameObject为空!");
                    throw new Exception("GetProperty - Scale 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    gameObject.transform.localScale = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Scale 失败, gameObject为空!");
                    throw new Exception("SetProperty - Scale 失败, gameObject为空!");
                }
            }
        }
        
        public Vector3 Position
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.transform.position;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Position 失败, gameObject为空!");
                    throw new Exception("GetProperty - Position 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    gameObject.transform.position = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Position 失败, gameObject为空!");
                    throw new Exception("SetProperty - Position 失败, gameObject为空!");
                }
            }
        }
        
        public Vector3 LocalPosition
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.transform.localPosition;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - LocalPosition 失败, gameObject为空!");
                    throw new Exception("GetProperty - LocalPosition 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    gameObject.transform.localPosition = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - LocalPosition 失败, gameObject为空!");
                    throw new Exception("SetProperty - LocalPosition 失败, gameObject为空!");
                }
            }
        }
        
        public Quaternion Rotation
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.transform.rotation;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Rotation 失败, gameObject为空!");
                    throw new Exception("GetProperty - Rotation 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    gameObject.transform.rotation = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Rotation 失败, gameObject为空!");
                    throw new Exception("SetProperty - Rotation 失败, gameObject为空!");
                }
            }
        }
        
        public Quaternion LocalRotation
        {
            get
            {
                if (gameObject)
                {
                    return gameObject.transform.localRotation;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - LocalRotation 失败, gameObject为空!");
                    throw new Exception("GetProperty - LocalRotation 失败, gameObject为空!");
                }
            }
            set
            {
                if (gameObject)
                {
                    gameObject.transform.localRotation = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Rotation 失败, gameObject为空!");
                    throw new Exception("SetProperty - Rotation 失败, gameObject为空!");
                }
            }
        }

        private GameComponent _mParentComponent;
        public GameComponent ParentComponent
        {
            get => _mParentComponent;
            private set => _mParentComponent = value;
        }

        private Transform _mParent;
        public Transform Parent
        {
            get => _mParent;
            private set
            {
                if (this.gameObject == null || value == null) return;
                if (this._mParent != value)
                {
                    this._mParent = value;
                    this.gameObject.transform.SetParent(this._mParent);
                }
            }
        }

        /// <summary>
        /// 维护lua端事件挂载的监听器
        /// </summary>
        private UIEventListener mUIEventListener;
        public UIEventListener UIEventListener
        {
            get
            {
                if (mUIEventListener == null) mUIEventListener = new UIEventListener(this.gameObject);
                return mUIEventListener;
            }
        }

        #endregion

        #region EventListener

#if UNITY_TOLUA

        /// <summary>
        /// 根据事件id设置点击监听 （点击很频繁，使用主动调用LuaFunction方式减少损耗）
        /// </summary>
        /// <param name="event_type"></param>
        public int SetClickEventListener(int event_type)
        {
            return UIEventListener.SetEventListener(event_type, null);
        }
        
        /// <summary>
        /// 根据事件id设置监听
        /// </summary>
        /// <param name="event_type">Event identifier.</param>
        /// <param name="func">Func.</param>
        public void SetEventListener(int event_type, LuaInterface.LuaFunction func)
        {
            UIEventListener.SetEventListener(event_type, func);
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="event_type"></param>
        public void RemoveEventListener(int event_type)
        {
            UIEventListener.RemoveEventListener(event_type);
        }

        /// <summary>
        /// 清理事件回调
        /// </summary>
        /// <param name="event_type"></param>
        public void ClearEventListener(int event_type)
        {
            UIEventListener.ClearEventListener(event_type);
        }
        
        /// <summary>
        /// 清除所有事件
        /// </summary>
        public void ClearAllEventListener()
        {
            UIEventListener.ClearAllListener();
        }
        
#endif

        #endregion


        #region static MakeComponent

        private const string MUGame = "MUGame.";

        public static GameComponent _make(GameComponent fatherCom, string type, string path)
        {
            Type t;
            if (!typeCache.TryGetValue(type.GetHashCode(), out t))
            {
                t = Type.GetType(MUGame + type);
                typeCache.Add(type.GetHashCode(), t);
            }

            return _makeUsingType(fatherCom, t, path);
        }

        public static GameComponent _make<T>(GameComponent fatherCom, string path) where T : GameComponent
        {
            return _makeUsingType(fatherCom, typeof(T), path) as T;
        }

        public static GameComponent _makeGameComponent(GameComponent fatherCom, string path)
        {
            if (fatherCom == null) return null;
            GameObject gObj = fatherCom.gameObject.GetGameObjectByID(path);
            if (gObj == null) return null;
            GameComponent ui = new GameComponent(gObj);
            fatherCom.AddChildComponent(ui, fatherCom);
            return ui;
        }

        private static GameComponent _makeUsingType(GameComponent fatherCom, Type type, string path)
        {
            if (type == null || fatherCom == null) return null;
            GameObject gObj = fatherCom.gameObject.GetGameObjectByID(path);
            if (gObj == null) return null;
            GameComponent objComp = (GameComponent) Activator.CreateInstance(type);
            objComp.Init(gObj, null);
            fatherCom.AddChildComponent(objComp, fatherCom);
            return objComp;
        }
        
        private static GameComponent _makeUsingType(GameComponent fatherCom, Type type, GameObject gObj)
        {
            if (gObj == null || type == null || fatherCom == null) return null;
            GameComponent objComp = (GameComponent) Activator.CreateInstance(type);
            objComp.Init(gObj, null);
            fatherCom.AddChildComponent(objComp, fatherCom);
            return objComp;
        }

        #endregion
            

        #region IComponentMaker<GameUIComponent>
        
        private static Dictionary<int, Type> typeCache = new Dictionary<int, Type>();
        
        /// <summary>
        /// 创建(C#实现的)基础组件，并添加到自身的成员当中
        /// </summary>
        /// <param name="type">组件类型名称</param>
        /// <param name="path">组件的路径</param>
        /// <returns>创建出来的组件</returns>
        public GameComponent LMake(string type, string path)
        {
            return _make(this, type, path);
        }

        public GameComponent LMake<T>(string path) where T : GameComponent
        {
            return _makeUsingType(this, typeof(T), path) as T;
        }
        
        public T LMake<T>(GameObject obj) where T : GameComponent
        {
            return _makeUsingType(this, typeof(T), obj) as T;
        }

        public GameComponent LMakeLuaComponent(string path)
        {
            return _makeGameComponent(this, path);
        }
        
        #endregion

    }
}