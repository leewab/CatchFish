using System;
using Game.Core;
using MUGUI;
using UnityEngine;

namespace Game.UI
{
    public class GamePartUIComponent : GameUIComponent, IRes
    {
        public GamePartUIComponent(string name, Transform father = null) : base(name, father)
        {
        }

        public GamePartUIComponent(GameObject obj) : base(obj)
        {
        }
        
        public GamePartUIComponent(GameUIComponent view, string path)
        {
            this.mView = view;
            this.mPath = path;
            this.ParsePath();
            this.ResLoadedCallBack += OnLoadedEvent;
        }
        
        private Transform mFather;
        private GameUIComponent mView;
        private string mPath = string.Empty;
        private string mNodeName = string.Empty;
        private string mResName = string.Empty;
        private bool mLogicVisible = false;
        private bool mIsLocal = false;              //是否是本地UI(子UI节点，Prefab中已经附带)
        private Canvas mCanvas;
        private Canvas[] mChildCanvas;
        private BaseImageLoader[] mImageLoaderList = null;
        
        public override bool Visible
        {
            get
            {
                return mLogicVisible;
            }
            set
            {
                mLogicVisible = value;
                if (!mIsLocal && mLogicVisible)
                {
                    if (!Load(mResName, mFather)) return;
                }
                if (ResLoaded) SetVisible(mLogicVisible);
            }
        }

        public override void SetVisible(bool value)
        {
            if (mCanvas != null)
            {
                mCanvas.enabled = value;
            }
            else
            {
                base.SetVisible(value);
            }

            if (mChildCanvas != null)
            {
                foreach (var item in mChildCanvas)
                {
                    item.enabled = value;
                }
            }
        }

        public override bool Load(string name, Transform father = null)
        {
            if (mIsLocal) return true;
            return base.Load(name, father);
        }

        public override void UnLoad()
        {
            if (mIsLocal) return;
            this.ReleaseImage();
            base.UnLoad();
            this.mIsLocal = false;
            this.mLogicVisible = false;
        }

        #region ImageLoader 

        public void ReleaseImage()
        {
            if (mImageLoaderList != null)
            {
                for (int i = 0; i < mImageLoaderList.Length; i++)
                {
                    mImageLoaderList[i].Release();
                }
            }
        } 
        
        public void ReloadImage()
        {
            if (mImageLoaderList != null)
            {
                for (int i = 0; i < mImageLoaderList.Length; i++)
                {
                    if (mImageLoaderList[i].IsEnable)
                    {
                        mImageLoaderList[i].Reload();
                    }
                }
            }
        }

        #endregion
        
        private void ParsePath()
        {
            // if (this.mPath.IndexOf(":", StringComparison.Ordinal) != -1)
            // {
            //     this.mPath = this.mPath.Replace(':', '/');
            // }
            if (this.mPath.EndsWith("/")) this.mPath = this.mPath.Remove(this.mPath.Length - 1, 1);
            int start = this.mPath.LastIndexOf('/');
            mNodeName = this.mPath.Substring(start, this.mPath.Length - start);

            UIPanelLayer uiPanelLayer = UIRoot.GetUIPanelLayer(UILayer);
            Transform parent = mView.gameObject.transform;
            while (parent.parent != null && uiPanelLayer != null && parent.parent.name.Equals(uiPanelLayer.name))
            {
                parent = parent.parent;
            }
            mResName = parent.name.Replace("(Clone)", "") + "-" + mNodeName + ".prefab";
            GameObject fatherObj = mView.gameObject.GetGameObjectByID(mPath);
            mFather = fatherObj == null ? null : fatherObj.transform;
        }

        private void OnLoadedEvent(string name, GameObject obj)
        {
            this.mCanvas = this.gameObject.GetComponent<Canvas>();
            this.mChildCanvas = this.gameObject.GetComponentsInChildren<Canvas>();
            this.mImageLoaderList = this.gameObject.GetComponentsInChildren<BaseImageLoader>(true);
            SetVisible(this.mLogicVisible);
            if (!mIsLocal)
            {
                RectTransform viewRectTransform = mView.gameObject.GetComponent<RectTransform>();
                RectTransform curRectTransform = mView.gameObject.GetComponent<RectTransform>();
                if (curRectTransform != null && viewRectTransform != null)
                {
                    curRectTransform.anchoredPosition3D = viewRectTransform.anchoredPosition3D;
                    curRectTransform.sizeDelta = viewRectTransform.sizeDelta;
                }
                this.gameObject.name = mNodeName;
                this.gameObject.transform.localScale = Vector3.one;
                //上层节点的动画可能需要重置
                NotifyViewpartAdded();
            }
        }
        
        //通知某些可能需要更新的Animator
        private void NotifyViewpartAdded()
        {
            // //简单粗暴的方式，不过好像没什么问题
            // foreach(var player in gameObject.GetComponentsInParent<UIAniPlayer>())
            // {
            //     player.OnViewPartTransfromAdded(gameObject.transform);
            // }
        }

    }
}