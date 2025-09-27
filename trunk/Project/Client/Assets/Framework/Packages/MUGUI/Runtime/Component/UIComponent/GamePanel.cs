using System;
using System.Collections.Generic;
using Game.Core;
using MUEngine;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// TODO:后期的销毁可以改成定时检测复用率，自动销毁复用率低的UI
    /// </summary>
    public class GamePanel : GameUIComponent, ICanvasMaker, IRes
    {
        public void Create(string name, Transform parent = null)
        {
            this.Load(name, parent);
        }
        
        public void Create(GameObject obj, Transform parent = null)
        {
            this.Init(obj, parent);
        }

        /// <summary>
        /// 设置界面平铺模式 面板归位
        /// </summary>
        public void SetNativeStretch()
        {
            if (this.RectTransform == null) return;
            this.RectTransform.localPosition = Vector3.zero;
            this.RectTransform.SetAnchorsStretch();
            this.RectTransform.localScale = Vector3.one;
        }
        
        //UI销毁时间
        private const float UIRELEASETIME = 15f;
        //释放图片
        private int releaseID = -1;
        //销毁UI
        private int disposeID = -1;
        //ImageLoader
        private ImageLoader[] mImageLoaderList = null;
        
        public object Parameter { get; set; }

        /// <summary>
        /// 加载资源
        /// </summary>
        public void ReLoadRes()
        {
            TimerHandler.RemoveTimeactionByID(releaseID);
            TimerHandler.RemoveTimeactionByID(disposeID);
            //已经释放过
            if (releaseID == -1)
            {
                ReloadImage();
            }
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void ReleaseRes()
        {
            TimerHandler.RemoveTimeactionByID(releaseID);
            TimerHandler.RemoveTimeactionByID(disposeID);
            if (mDisposeTime > 0)
                disposeID = TimerHandler.SetTimeout(OnTimerDispose, mDisposeTime);
            //Debug.Log("----------------------OnCanvasEnableChanged " + this.gameObject.name + "," + Enable);
            if (MURoot.IsLowMemoryMod) // 低内存模式下 关闭界面后立刻清理
            {
                OnTimerReleaseImage();
            }
            else if(mIsReleaseImg)
            {
                releaseID = TimerHandler.SetTimeout(OnTimerReleaseImage, UIRELEASETIME);
            }
        }
        
        //对Lua层提供接口(loading界面是自己销毁的）
        public void ReleaseImage()
        {
            this.OnTimerReleaseImage();
        }
        
        private void ReleaseImage(bool all = false)
        {
            if (gameObject == null)
            {
                Debug.Log("Destroy ui" + this.Name);
                return;
            }
            //Debug.Log("releaseImage ui" + this.Name + "," + all);
            mImageLoaderList = gameObject.GetComponentsInChildren<ImageLoader>(true);
            if (mImageLoaderList != null)
            {
                for (int i = 0; i < mImageLoaderList.Length; i++)
                {
                    mImageLoaderList[i].Release();
                }
            }
            //在卸载ui的时候，会在GamePartComponent中释放
            if (all && mChildren != null)
            {
                foreach (var item in mChildren)
                {
                    if (item is GamePartUIComponent)
                    {
                        (item as GamePartUIComponent).ReleaseImage();
                    }
                }
            }
        }
        
        public void ReloadImage()
        {
            if (mImageLoaderList == null)
            {
                mImageLoaderList = gameObject.GetComponentsInChildren<ImageLoader>(true);
            }

            for (int i = 0; i < mImageLoaderList.Length; i++)
            {
                if (mImageLoaderList[i].IsEnable)
                {
                    mImageLoaderList[i].Reload();
                }
            }

            if (mChildren != null)
            {
                foreach (var item in mChildren)
                {
                    if (item is GamePartUIComponent)
                    {
                        (item as GamePartUIComponent).ReloadImage();
                    }
                }
            }
        }

        private void OnTimerReleaseImage()
        {
            TimerHandler.RemoveTimeactionByID(releaseID);
            releaseID = -1;
            ReleaseImage(true);
        }
        
        private void OnTimerDispose()
        {
            //界面销毁前把图片销毁的去掉
            if (releaseID != -1)
            {
                TimerHandler.RemoveTimeactionByID(releaseID);
            }
            TimerHandler.RemoveTimeactionByID(disposeID);
            disposeID = -1;
            releaseID = -1;
            this.Dispose();
        }
        
        #region UIView

        /// <summary>
        /// 是否打开面板
        /// </summary>
        public bool IsOpened { get; private set; }

        public void Open(params object[] _data)
        {
            this.Parameter = _data;
            this.IsOpened = true;
            if (!this.ResLoaded)
            {
                this.ResLoadedCallBack = (_name, _obj) =>
                {
                    Debug.Log(_name + " Loaded成功！");
                    if (!this.IsOpened)
                    {
                        this.Close();
                        return;
                    }
                    
                    this.SetNativeStretch();
                    this.SetAsLastSibling();
                    this.SetVisible(true);
                };
                return;
            }
            
            this.SetNativeStretch();
            this.SetAsLastSibling();
            this.SetVisible(true);
        }
        
        public void Close(bool isDestroy = false)
        {
            this.IsOpened = false;
            if (!this.ResLoaded) return;
            this.SetVisible(false);
            if (isDestroy) OnTimerDispose();
        }

        //手动设置z轴
        public void ManualSetZPos(int pos)
        {
            if (Canvas != null)
                Canvas.planeDistance = pos;
        }
        
        //多个面板
        public void AutoSetZPos()
        {
            int count = UIModelMgr.GetLogicCount(Canvas);
            int distance = 100 - count * 30;
            if(distance < 10)
            {
                distance = 10;
            }
            if (Canvas != null)
                Canvas.planeDistance = distance;
        }

        #region 生命周期重新

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnDispose()
        {
            this.ReleaseImage();
            this.OnDisposeEvent?.Invoke();
            this.OnDisposeEvent = null;
            base.OnDispose();
        }

        protected override void OnShow()
        {
            this.ResUpdate(true);
            this.gameObject.SetActive(true);
            base.OnShow();
        }

        protected override void OnHide()
        {
            this.gameObject.SetActive(false);
            this.ResUpdate(false);
            base.OnHide();
        }

        #endregion

        /// <summary>
        /// 资源变化
        /// </summary>
        private void ResUpdate(bool isShow)
        {
            if (isShow)
            {
                ReLoadRes();
            }
            else
            {
                ReleaseRes();
            }

            if (Canvas != null)
            {
                List<Behaviour> objList = UIBehaviorMgr.GetNeedAutoEnableObjList(Canvas);
                if (objList == null || objList.Count == 0) return;
                int count = objList.Count;
                for (int i = 0; i < count; ++i)
                {
                    Behaviour obj = objList[i];
                    if (obj) obj.enabled = isShow;
                }
            }
        }
        
        #endregion

        #region 重写方法

        public override void ClearProperties()
        {
            base.ClearProperties();
            this.Parameter = null;
        }

        public override void SetVisible(bool value)
        {
            base.SetVisible(value);
        }

        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = base.SetPropertyImpl(key, val);
            if (succ) return true;
            if (key == UIProperty.Enable)
            {
                this.Enable = (bool)val;
                succ = true;
            }

            return succ;
        }

        protected override bool GetPropertyImpl(UIProperty key, ref object ret)
        {
            bool succ = base.GetPropertyImpl(key, ref ret);
            if (succ) return true;
            if (key == UIProperty.Enable)
            {
                ret = this.Enable;
                succ = true;
            }

            return succ;
        }

        #endregion
        
        #region 公共变量

        private Canvas mCanvas;
        public Canvas Canvas
        {
            get
            {
                if (this.mCanvas) return this.mCanvas;
                if (gameObject)
                {
                    this.mCanvas = gameObject.GetComponent<Canvas>();
                    if (!this.Canvas)
                    {
                        ExceptionHelper.ThrowExceptionToBroadcast("new LuaGamePanel - 失败, Canvas 为空!");
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("new LuaGamePanel - 失败, gameObject 为空!");
                }
               
                return this.mCanvas;
            }
        }

        public int SortingOrder
        {
            get
            {
                if (Canvas)
                {
                    return Canvas.sortingOrder;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("Get SortOrder - 失败, Canvas 为空!");
                    return 0;
                }
            }
            set
            {
                if (Canvas)
                {
                    if (Canvas.sortingOrder != value) Canvas.sortingOrder = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("Set SortOrder - 失败, Canvas 为空!");
                }
            }
        }
        
        public bool Enable
        {
            get
            {
                if (Canvas)
                {
                    return Canvas.enabled;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("Get Enable - 失败, Canvas 为空!");
                    return false;
                }
            }
            set
            {
                if (Canvas)
                {
                    if (Canvas.enabled != value)
                    {
                        Canvas.enabled = value;
                        ResUpdate(value);
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("Get Enable - 失败, Canvas 为空!");
                    ResUpdate(false);
                }
            }
        }
        
        private int mDisposeTime = -1;
        public int DisposeTime
        {
            get => mDisposeTime;
            set => mDisposeTime = value;
        }
        
        private bool mIsReleaseImg = true;
        public bool IsReleaseImg
        {
            set => mIsReleaseImg = value;
        }
        
        public Action OnDisposeEvent;
        
       #endregion

    }
}