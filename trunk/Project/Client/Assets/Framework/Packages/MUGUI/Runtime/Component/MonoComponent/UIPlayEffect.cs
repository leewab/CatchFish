using MUEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
    //模仿UIPlaySound，增加一个在适合时机播放特效的功能
    public class UIPlayEffect : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Trigger
        {
            OnClick,
            OnPress,
            OnRelease,
            OnEnable,
            OnDisable,
            Custom,
        }
        public enum PosType
        {
            CenterPos,
            FingerPos,
        }

        //虽然要直接输名字不是很友好，但是省事啊！ = =
        public string EffectName;
        public float EffectDestroyTime = 3.0f;
        public Trigger trigger = Trigger.OnClick;
        //默认特效挂点是自己，但是允许挂在其它节点上（主要用于控制特效的位置，大小，旋转等）
        public Transform EffectParent = null;
        public float EffectScale = 1;
        public string CustomTriggerCondition = string.Empty;

        public bool forceOnlyOne = true;
        public bool releaseOnDisable = true;
        //一个很特殊的值，表示该组件控制的特效是否需要与某些其它组件对象控制的特效互斥，0表示不进行互斥处理，其它表示与同Id的特效互斥
        public int globalUniqueId = 0;
        //特效默认的优先显示位置，如果PosType为FingerPos，则优先在手指点击的位置显示特效
        public PosType posType = PosType.FingerPos;

        private static Dictionary<int, int> GlobalUniqueEffectDic = new Dictionary<int, int>();

        private Transform Parent
        {
            get
            {
                return EffectParent == null ? transform : EffectParent;
            }
        }

        //private HashSet<Entity> allEffect = new HashSet<Entity>();
        private int currentEntityId = Entity.INVALID_ID;

        void OnEnable()
        {
            if (trigger == Trigger.OnEnable)
            {
                PlayEffect(Vector3.zero);
            }
        }

        void OnDisable()
        {
            //if (releaseOnDisable)
            //{
            //    //由于底层的对象复用会更改Entity对应GameObject的父节点，但是在Unity中，不允许再父节点被DeActive过程中更改子节点的层级
            //    //这回导致Unity的报错。所以这里将真正的删除延迟到下一帧，但是逻辑上的删除立即完成。只有OnDisable时需要这么做，其它时候直接删除就好
            //    List<Entity> effects = new List<Entity>();
            //    foreach (var effect in allEffect)
            //    {
            //        effects.Add(effect);
            //        //MURoot.Scene.DelEntity(effect);
            //    }
            //    RemoveEffectsInNextFrame(effects);
            //    allEffect.Clear();
            //    if (currentEntity != null)
            //    {
            //        CheckRemoveUnique(currentEntity);
            //    }
            //    currentEntity = null;
            //}
            if (trigger == Trigger.OnDisable)
            {
                PlayEffect(Vector3.zero);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (trigger == Trigger.OnClick)
            {
                var pos = posType == PosType.FingerPos ? ScreenPosToRootLocal(eventData.position) : Vector3.zero;
                PlayEffect(pos);
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (trigger == Trigger.OnPress)
            {
                var pos = posType == PosType.FingerPos ? ScreenPosToRootLocal(eventData.position) : Vector3.zero;
                PlayEffect(pos);
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (trigger == Trigger.OnRelease)
            {
                var pos = posType == PosType.FingerPos ? ScreenPosToRootLocal(eventData.position) : Vector3.zero;
                PlayEffect(pos);
            }
        }

        public void OnCustomPlay(string condition)
        {
            if (trigger == Trigger.Custom && CustomTriggerCondition == condition)
            {
                PlayEffect(Vector3.zero);
            }
        }

        private Vector3 ScreenPosToRootLocal(Vector2 pos)
        {
            //Debug.LogError("try to convert world pos , world pos is : " + pos + ",result is : " + Parent.InverseTransformPoint(pos));
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null) canvas = canvas.rootCanvas;
            Vector3 worldPos = Vector3.zero;
            Camera camera = null;
            if (canvas == null || canvas.worldCamera == null)
            {
                camera = Camera.main;
            }
            else
            {
                camera = canvas.worldCamera;
            }
            if (camera == null)
            {
                //玩个毛线，怎么算都是错的，直接放弃治疗吧。
                //虽然可以遍历场景找个摄像机或者新建摄像机，但是根本没有必要
                return Vector3.zero;
            }
            float distance = (Parent.position - camera.transform.position).magnitude;
            worldPos = camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, distance));
            return Parent.InverseTransformPoint(worldPos);
        }

        public void PlayEffect(Vector3 localPos)
        {
            if (string.IsNullOrEmpty(EffectName))
            {
                return;
            }
            if (forceOnlyOne && currentEntityId != Entity.INVALID_ID)
            {
                //它可能已经被销毁过，不过应该没有问题
                MURoot.Scene.DelEntity(currentEntityId);
                CheckRemoveUnique(currentEntityId);
                //if (allEffect.Contains(currentEntity))
                //{
                //    allEffect.Remove(currentEntity);
                //}
                currentEntityId = Entity.INVALID_ID;
            }
            //var parentTransform = EffectParent != null ? EffectParent : transform;
            //如果优先在手指显示，就用手指的位置( 需要区分OnEnable和OnDisable触发的特效吗 ？ 暂时不考虑这个问题)
            //var pos = Vector3.zero;
            var effect = MURoot.Scene.AddEffect(EffectName, localPos, Quaternion.identity, EffectScale, EffectDestroyTime);
            if (effect == null)
            {
                return;
            }
            effect.Parent = Parent;
            //走统一的设置UISortingOrder
            // LuaUtil.AutoSetUIEntitySortingOrder(effect);
            effect.OnLoadResource += () =>
            {
                if (releaseOnDisable)
                {
                    effect.GameObject.GetOrAddComponent<MUGame.EffectAutoDestroy>().SetAttchedEntity(effect);
                }
            };
            currentEntityId = effect.ID;
            //只有挂载在自己的特效，会随着自己的变成InActive状态而被销毁
            //if (releaseOnDisable && Parent == transform)
            //{
            //    allEffect.Add(effect);
            //}
            CheckAddUnique(currentEntityId);
            effect.Load();
        }

        private void RemoveEffectsInNextFrame(List<Entity> effects)
        {
#if !UNITY_EDITOR_PROJECT
            Game.Core.Common.NextFrameExecute += () =>
            {
                foreach (var effect in effects)
                {
                    MURoot.Scene.DelEntity(effect);
                }
            };
#else
            foreach (var effect in effects)
            {
                MURoot.Scene.DelEntity(effect);
            }
#endif
        }


        private void CheckAddUnique(int effectId)
        {
            if (globalUniqueId == 0)
            {
                return;
            }
            if (GlobalUniqueEffectDic.ContainsKey(globalUniqueId))
            {
                MURoot.Scene.DelEntity(GlobalUniqueEffectDic[globalUniqueId]);
            }
            GlobalUniqueEffectDic[globalUniqueId] = effectId;
        }
        private void CheckRemoveUnique(int effectId)
        {
            if (globalUniqueId == 0)
            {
                return;
            }
            if (GlobalUniqueEffectDic.ContainsKey(globalUniqueId) && GlobalUniqueEffectDic[globalUniqueId] == effectId)
            {
                GlobalUniqueEffectDic.Remove(globalUniqueId);
            }
        }
    }
}

