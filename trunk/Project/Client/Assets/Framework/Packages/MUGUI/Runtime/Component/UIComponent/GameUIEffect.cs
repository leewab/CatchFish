using System;
using Game.Core;
using UnityEngine;
using MUEngine;
using MUGame;

namespace Game.UI
{
    public class GameUIEffect : GameUIComponent
    {
        private string mPrefabName;
        private float mScale;
        private int mSortingOrder;
        private Vector3 mEffectRot;
        private int mPlayCount = 1;
        private Vector3 mOffset;
        private bool mAutoHide = true;
        private bool mHideDestory = false;
        private Entity mActor;                    // 特效Actor
        private int effectId = Entity.INVALID_ID;
        private Action mCompleteCallback = null;
        private Action mLoadCompleteCallback = null;
        private UIEffectScale mEffScale;

                
        /// <summary>
        /// 特效播放完毕回调
        /// </summary>
        public Action CompleteCallback
        {
            set => mCompleteCallback = value;
        }

        /// <summary>
        /// 特效资源加载回调
        /// </summary>
        public Action LoadCompleteCallback
        {
            set => mLoadCompleteCallback = value;
        }
        
        protected override bool GetPropertyImpl(UIProperty key, ref object ret)
        {
            bool succ = true;
            if (key == UIProperty.EffectActor)
            {
                ret = mActor.GameObject;
            }
            else
            {
                succ = base.GetPropertyImpl(key, ref ret);
            }

            return succ;
        }

        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = true;
            if (key == UIProperty.Enable)
            {
                this.EffectVisible = (bool)val;
            }
            else if (key == UIProperty.PlayEffect)
            {
                string str = (string)val;
                string[] param = str.Split(',');
                if (param.Length == 2)
                {
                    PlayEffect(param[0], -1, Vector3.zero, Vector3.zero, int.Parse(param[1].ToString()), false, 1, false);
                }
                else
                {
                    LogHelper.Error("使用SetProperty调用PlayEffect接口，目前只支持两个参数，如需多个参数请直接使用PlayEffect！");
                    succ = false;
                }
            }
            else if (key == UIProperty.DisposeActor)
            {
                ClearEffect(true);
            }
            else if (key == UIProperty.Scale)
            {
                float[] scaleParams = (float[])val;
                if (scaleParams.Length == 2)
                {
                    float scale = scaleParams[0];
                    int renderQueue = (int)scaleParams[1];
                    this.SetScale(scale, renderQueue);
                }
                else if (scaleParams.Length == 1)
                {
                    float scale = scaleParams[0];
                    this.SetScale(scale);
                }
                else
                {
                    succ = false;
                }
            }
            else if (key == UIProperty.Rotation)
            {
                float[] scaleParams = (float[])val;
                if (scaleParams.Length == 3)
                {
                    this.SetRotation(scaleParams[0], scaleParams[1], scaleParams[2]);
                }
                else
                {
                    succ = true;
                }
            }
            else
            {
                succ = base.SetPropertyImpl(key, val);
            }
          
            return succ;
        }

        public override void Dispose()
        {
            base.Dispose();
            mCompleteCallback = null;
            mLoadCompleteCallback = null;
            ClearEffect(true);
        }

        protected override void OnShow()
        {
            this.Visible = false;
            base.OnShow();
        }

        protected override void OnHide()
        {
            this.Visible = false;
            base.OnHide();
        }

        // 注意：此处sortingOrder将不再起效，sortingOrder会在C#端自动进行计算
        // 暂时仍然保留这个字段（Lua那边调用的地方太多了），之后又时间再统一更改吧
        public void PlayEffect(string tPrefabName, float scale, Vector3 rot, Vector3 offset, int sortingOrder, bool autoHide, int playCount, bool hideDestroy = false)
        {
            if (string.IsNullOrEmpty(tPrefabName)) return;
            mPrefabName = tPrefabName;
            mScale = scale;
            mSortingOrder = sortingOrder;
            mEffectRot = rot;
            mPlayCount = playCount;
            mOffset = offset;
            mAutoHide = autoHide;
            mHideDestory = hideDestroy;
            DoPlayEffect();
        }
        
        public void Refresh()
        {
            if (mEffScale != null) mEffScale.Dirty = true;
        }

        public void SetScale(float scale = 0.5f, int renderqueue = -1)
        {
            mScale = scale;
            mSortingOrder = renderqueue;
            if (IsValidEffect() && mActor.GameObject && mEffScale != null)
            {
                mEffScale.RenderQueue = renderqueue;
                mEffScale.Scale = scale;
            }
        }

        public void SetRotation(float raw, float pitch, float roll)
        {
            if (IsValidEffect() && mActor.GameObject)
            {
                Quaternion degree = Quaternion.Euler(raw, 0, roll) * Quaternion.Euler(raw, 0, pitch);
                mActor.GameObject.transform.localRotation = degree;
            }
        }
        
        private void DoPlayEffect()
        {
            ClearEffect(false);
            if (string.IsNullOrEmpty(mPrefabName)) return;
            mActor = MURoot.Scene.AddEffect(mPrefabName, Vector3.zero, Quaternion.identity);
            if (null == mActor) return;
            effectId = mActor.ID;
            mActor.OnLoadResource += OnLoadRes;
            mActor.Name = mPrefabName;
            if (!MURoot.MUQualityMgr.CanCacheObject) return;
            mActor.Load();
        }
        
        private void ClearEffect(bool removeAsset)
        {
            if (IsValidEffect())
            {
                if(removeAsset)
                {
                    MURoot.ResMgr.RemoveAsset(mActor.Name);
                }
                
                MURoot.Scene.DelEntity(effectId);
                ClearEffectInfo();
            }
        }

        private void ClearEffectInfo()
        {
            mActor = null;
            effectId = Entity.INVALID_ID;
        }
        
        // Effect是否已经被销毁了
        private bool IsValidEffect()
        {
            return mActor != null && mActor.ID == effectId;
        }
        
        private void OnLoadRes()
        {
            if (!IsValidEffect()) return;
            if (mActor.GameObject == null)
            {
                //D.error(string.Format("{0} GameObj is Null", this.Name));
                //一个资源在未加载成功前，如果再次发生load过程，新的actor取代了旧的actor.
                //如果这发生在同一帧上， 资源对应的Gameobject在OnLoadRes时不会写到这个新的actor上了。
                //这种情形也表明，那一次资源加载确实应当被取消.
                return;
            }
            ResetEffect();
            mActor.Parent = gameObject.transform;
            mActor.Scale = Vector3.one;
            //GameObjectUtil.SetLayer(mActor.GameObject,5, true);
            mActor.GameObject.transform.localPosition = mOffset;
            mActor.SetAniCullingType(UnityEngine.AnimationCullingType.AlwaysAnimate);
            mActor.Visible = true;
            //关于SortingOrder和SortingLayer，走统一的设置吧，方便管理
            //大部分情况下应该没问题，少部分情况下可能会有Bug 。。。。 出现之后再说吧
            UIUtil.AutoSetUIEntitySortingOrder(mActor);
            if (mLoadCompleteCallback != null) this.mLoadCompleteCallback();
            if (mHideDestory) mActor.GameObject.GetOrAddComponent<EffectAutoDestroy>().SetAttchedEntity(mActor);
            if (mScale < 0)
            {
                if(mAutoHide)
                {
                    EffectAutoHide tEffAutoHide = mActor.GameObject.GetComponent<EffectAutoHide>();
                    if (tEffAutoHide == null)
                    {
                        tEffAutoHide = mActor.GameObject.AddComponent<EffectAutoHide>();
                    }
                    tEffAutoHide.ResetLifeTime();
                }
                return;
            }
            mEffScale = mActor.GameObject.GetComponent<UIEffectScale>();
            if(mEffScale == null) mEffScale = mActor.GameObject.AddComponent<UIEffectScale>();
            if (mCompleteCallback != null) mEffScale.AutoHideCallback = mCompleteCallback;
            mEffScale.mAutoHide = mAutoHide;
            mEffScale.ResetLifeTime();
            mEffScale.Scale = mScale;
            mEffScale.RenderQueue = mSortingOrder;
            mEffScale.mPlayCount = mPlayCount;
            mActor.GameObject.transform.localRotation = Quaternion.Euler(mEffectRot);
            mEffScale.Dirty = true;
        }
        
        private void ResetEffect()
        {
            mActor.Restart();
            if(mActor.GameObject != null )
            {
                Delay[] ds = mActor.GameObject.GetComponentsInChildren<Delay>(true);
                for (int i = 0; i < ds.Length; ++i)
                {
                    ds[i].Restart();
                }
            }
        }

        public bool EffectVisible
        {
            get
            {
                return IsValidEffect() && mActor.Visible;
            }
            set
            {
                if (IsValidEffect())
                {
                    mActor.Visible = value;
                }
            }
        }

        public string PrefabName
        {
            set
            {
                if (mPrefabName != value)
                {
                    mPrefabName = value;
                    if (Visible)
                    {
                        DoPlayEffect();
                    }
                }
            }
            get
            {
                //返回正在播放的那个特效(mActor对应)的名称。
                if (IsValidEffect()) return mActor.Name;
                return "";
            }
        }

        private void addSound()
        {
            if (!gameObject.activeInHierarchy)
                return;
            if (!MURoot.MUQualityMgr.CanCacheObject)
                return;
        }

    }
}
