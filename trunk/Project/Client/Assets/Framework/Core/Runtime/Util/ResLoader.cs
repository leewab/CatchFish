
using System;
using UnityEditor;

namespace Game.Core
{
    using UnityEngine;
    using MUEngine;

    public class ResLoader : IResLoader
    {
        protected GameObject gameObject;
        public string Name { private set; get; }
        public RES_LOAD_STATE State { private set; get; }

        public virtual void UnLoad()
        {
            this.State = RES_LOAD_STATE.UNDIFINE;
            MURoot.ResMgr.ReleaseAsset(this.Name, this.gameObject);
            this.Dispose();
            // Object.Destroy(this.gameObject);
        }

        public virtual bool Load(string name, Action<string, GameObject> onLoadCallBack)
        {
            if (this.State == RES_LOAD_STATE.NORAML) return false;
            if (this.State == RES_LOAD_STATE.LOADING) return false;
            this.State = RES_LOAD_STATE.LOADING;
            this.Name = name;
            MURoot.ResMgr.GetAsset(name, (cName, cObj) => { OnLoadCallBack(cName, cObj, onLoadCallBack); },
                LoadPriority.HighPrior, ECacheType.AutoDestroy);
            return true;
        }

        public virtual void OnLoadCallBack(string name, Object obj,  Action<string, GameObject> onLoadCallBack)
        {
            if (obj == null)
            {
                LogHelper.Error("资源加载对象为空！name:" + name);
                UnLoad();
                onLoadCallBack?.Invoke(name, null);
                return;
            }

            if (this.State == RES_LOAD_STATE.UNDIFINE)
            {
                LogHelper.Log("资源加载对象已被取消！name:" + name);
                UnLoad();
                onLoadCallBack?.Invoke(name, null);
                return;
            }

            this.gameObject = obj as GameObject;
            if (this.gameObject != null)
            {
                this.State = RES_LOAD_STATE.NORAML;
                this.gameObject.transform.localScale = Vector3.one;
                this.gameObject.transform.localPosition = Vector3.zero;
            }

            onLoadCallBack?.Invoke(name, this.gameObject);
        }

        public void Dispose()
        {
            this.Name = null;
            this.gameObject = null;
            this.State = RES_LOAD_STATE.UNDIFINE;
        }
    }
}