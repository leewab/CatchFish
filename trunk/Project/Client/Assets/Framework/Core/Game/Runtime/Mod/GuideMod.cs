using MUEngine;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;
using Game;

namespace MUGame
{
    public class GuideMod : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static GuideMod Instance => HandlerModule.GuideMod;

        private Dictionary<int, GuideEffect> mEffectDic = new Dictionary<int, GuideEffect>();

        public Entity AddEffect(GameObject parent, string name, float offsetX, float offsetY,int order)
        {
            GuideEffect effect = new GuideEffect(name, parent, new Vector3(offsetX, offsetY),order);
            mEffectDic.Add(effect.Actor.ID, effect);
            return effect.Actor;
        }

        public void Remove(int id)
        {
            GuideEffect effect;
            if (mEffectDic.TryGetValue(id,out effect))
            {
                effect.DisposeActor();
                mEffectDic.Remove(id);
            }
        }

        

        class GuideEffect
        {
            private Entity mActor = null;
            public Entity Actor
            {
                get
                {
                    return mActor;
                }
            }
            private Vector3 mOffset = Vector3.zero;
            private GameObject mRoot = null;
            private int mSortingOrder = 0;

            public GuideEffect(string name,GameObject go,Vector3 offset,int order)
            {
                mActor = MURoot.Scene.AddEffect(name, Vector3.zero, Quaternion.identity);
                mRoot = go;
                mOffset = offset;
                mSortingOrder = order;

                mActor.OnLoadResource += OnLoadRes;
                mActor.Load();
            }
            private void OnLoadRes()
            {
                //可能加载中，销毁了资源，加载完成回调判断是否为null
                if (mActor == null)
                    return;
                if (mRoot != null)
                {
                    mActor.Parent = mRoot.transform;
                }
                mActor.Scale = Vector3.one;
                GameObjectUtil.SetLayer(mActor.GameObject, 5, true);
                mActor.GameObject.transform.localPosition = mOffset;
                if (mSortingOrder > 0)
                {
                    RenderQueueComponent queueComponent = mActor.GameObject.GetComponent<RenderQueueComponent>();
                    if (queueComponent == null)
                    {
                        queueComponent = mActor.GameObject.AddComponent<RenderQueueComponent>();
                    }
                    queueComponent.RenderQueue = mSortingOrder;
                }
            }
            public void DisposeActor()
            {
                if (mActor != null)
                {
                    MURoot.Scene.DelEntity(mActor);
                    MURoot.ResMgr.RemoveAsset(mActor.Name);
                    mActor = null;
                }
            }
        }
    }
}
