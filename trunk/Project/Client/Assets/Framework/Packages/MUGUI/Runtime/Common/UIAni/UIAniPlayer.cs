using System;
using UnityEngine;
using System.Collections.Generic;
using MUEngine;

namespace MUGame
{
    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent]
    public class UIAniPlayer : MonoBehaviour
    {
        [Serializable]
        public class EffectData
        {
            [SerializeField]
            public string Name;
            [SerializeField]
            public Transform Parent;
            [SerializeField]
            public string EffectName;   //真正加载时用资源名称去加载
            [SerializeField]
            public float ElapseTime = 3;
            [SerializeField]
            public bool forceOnlyOne = true;
            [SerializeField]
            public bool releaseWhenDisable = true;  //本UiAniPlayer对象Disable是否需要被销毁

            [System.NonSerialized]
            public GameObject EffectPrefab;
        }
        [SerializeField]
        private bool IgnoreRebindWhenPlaying = false;
        [SerializeField]
        private List<EffectData> mEffList = new List<EffectData>();
        //允许增加“子动画控制器”，通过发送事件的方式来让子控制其播放对应的动画，来达到多个动画同时播放的目的
        //这种使用发生于Unity的设计 以及该类的原始设计不符合，而且有可能导致一些Bug，但是某些情况下可以减少制作动画的工作量
        //（允许UI动画的粒度更细，进而有机会提高动画的复用度）
        [SerializeField]
        private List<UIAniPlayer> subAniPlayers = new List<UIAniPlayer>();

        public List<EffectData> EffList
        {
            get
            {
                return mEffList;
            }
        }
        private Action<string> mEventFunc;
        public Action<string> AniEventFunc
        {
            get
            {
                return mEventFunc;
            }
            set
            {
                mEventFunc = value;
            }

        }

        private Animator animator;
        private Animator mAnimator
        {
            get
            {
                if (animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        /// <summary>
        /// 设置特效数据，编辑器调用
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <param name="eff_dat">Eff dat.</param>
        public void SetEffDat(int idx, EffectData eff_dat)
        {
            mEffList[idx] = eff_dat;
        }
        /// <summary>
        /// 播放一段动画，客户端调用
        /// </summary>
        /// <param name="aniname">Aniname.</param>
        public void PlayAni(string aniname, bool resetBefore = true)
        {
            //Debug.LogError("try to play animation  " + aniname);
            if (resetBefore && !string.IsNullOrEmpty(currentAnimationName))
            {
                mAnimator.ResetTrigger(currentAnimationName);
            }
            currentAnimationName = aniname;
            if (IsAnimatorDirty)
            {
                ReBindAnimations();
            }
            mAnimator.SetTrigger(aniname);
        }

        /// <summary>
        /// 重置trigger
        /// </summary>
        /// <param name="aniname">Aniname.</param>
        public void ResetTrigger(string aniname)
        {
            mAnimator.ResetTrigger(aniname);
        }

        public AnimatorControllerParameter[] GetAllParams()
        {
            return mAnimator.parameters;
        }

        public List<AnimationEvent> GetAllAnimationEvent()
        {
            List<AnimationEvent> eventList = new List<AnimationEvent>();
            foreach (var clip in mAnimator.runtimeAnimatorController.animationClips)
            {
                eventList.AddRange(clip.events);
            }
            return eventList;
        }
        public void SetAnimatorSpeed(float speed)
        {
            mAnimator.speed = speed;
        }

        //当前是否真正在播放动画
        public bool IsRealPlaying()
        {
            if (mAnimator == null || !mAnimator.enabled)
            {
                return false;
            }
            if (mAnimator.runtimeAnimatorController == null)
            {
                return false;
            }
            var currentState = mAnimator.GetCurrentAnimatorStateInfo(0);
            //约定，如果当前状态名为default(大小写敏感)，那么也认为不处于真正播放状态。
            if (currentState.IsName("default"))
            {
                return false;
            }
            //Debug.LogError("current state is : " + currentState.shortNameHash + " current time is : " + currentState.normalizedTime);
            return currentState.normalizedTime < 1 || mAnimator.IsInTransition(0);
        }

        /// <summary>
        /// 播放声音，关键帧调用
        /// </summary>
        /// <param name="sound_name">Sound name.</param>
        public void PlaySound(string sound_name)
        {
            MURoot.MUAudioMgr.AddMainSound(sound_name, false, true);
        }
        /// <summary>
        /// 触发特效，关键帧调用
        /// </summary>
        /// <param name="name">Name.</param>
        public void TriggerEffect(string name)
        {
            //CheckSetEffectName();
            foreach (var data in mEffList.FindAll(data => data.Name == name))
            {
                if (!string.IsNullOrEmpty(data.EffectName) && data.Parent != null)
                {
                    AddEffect(data);
                    //允许同时显示多个特效
                    //break;
                }
            }
        }

        //用于保证某个资源同时只能有一个的Dictionary
        private Dictionary<string, Entity> uniqueEffectDic = new Dictionary<string, Entity>();

        //真正添加特效并且播放的代码，基本照抄GameUIEffect中的代码，保证是通过ResMgr加载的资源，并且做为一个Entity，与其它特效走同样的处理
        private const int UILayer = 5;
        public void AddEffect(EffectData data)
        {
            var effect = MURoot.Scene.AddEffect(data.EffectName, Vector3.zero, Quaternion.identity, 1, data.ElapseTime);
            if (effect == null)
            {
                return;
            }
            //effect.EntityType = EntityType.UIEffectEntity;
            effect.Parent = data.Parent;
            effect.OnLoadResource += () => {
                if (effect == null || effect.GameObject == null)
                {
                    return;
                }
                GameObjectUtil.SetLayer(effect.GameObject, UILayer, true);
                if (data.releaseWhenDisable)
                {
                    effect.GameObject.GetOrAddComponent<EffectAutoDestroy>().SetAttchedEntity(effect);
                }
            };
            if (data.forceOnlyOne)
            {
                //目前，使用自定义Name作为Key，多个Effect的管理交给使用者
                if (uniqueEffectDic.ContainsKey(data.Name))
                {
                    var lastEffect = uniqueEffectDic[data.Name];
                    MURoot.Scene.DelEntity(lastEffect);
                }
                uniqueEffectDic[data.Name] = effect;
            }
            effect.Load();
        }

        private void OnDestroy()
        {

        }

        private void OnDisable()
        {

        }

        /// <summary>
        /// 删除一个特效，编辑器用
        /// </summary>
        /// <param name="idx">Index.</param>
        public void DelEffect(int idx)
        {
            mEffList.RemoveAt(idx);
        }
        /// <summary>
        /// 触发一个动画事件，关键帧调用
        /// </summary>
        /// <param name="param">Parameter.</param>
        public void OnAniEvent(string param)
        {
            if (mEventFunc != null)
            {
                mEventFunc(param);
            }
        }
        //让某个子状态机播放动画，目前只允许播放同名动画
        public void PlayAnimationInChild(int index)
        {
            if (subAniPlayers == null || subAniPlayers.Count <= index || index < 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(currentAnimationName))
            {
                return;
            }
            UIAniPlayer child = subAniPlayers[index];
            if (child == null)
            {
                return;
            }
#if UNITY_EDITOR
            //设计上暂时不希望两个控制器互相调用，这里稍微加一些判断
            if (IsSubOf(child))
            {
                Debug.LogError("暂时不允许AniPlayer之间的互相引用，如果发现这个Log,请去殴打瞿宗昊，该log目前只有程序可见");
                return;
            }
#endif
            child.PlayAni(currentAnimationName);
        }
        //当某个子节点发生变动，可能需要重新绑定路径的时候调用，目前只在运行时被ViewPartComponent调用
        [NonSerialized]
        private bool IsAnimatorDirty = false;
        public void OnViewPartTransfromAdded(Transform viewpartTransform)
        {
            //理论上这里应该做一些判断，判断新加入的Transform是否需要让Animator重新绑定
            //不过做出正确判断的代价似乎比直接刷新更大 。。。
            IsAnimatorDirty = true;
            //Debug.LogError("get viewpart load message");
            if (IgnoreRebindWhenPlaying)
            {
                return;
            }
            TryRePlayAnimation();
        }

        //目前动画接口的设计就是一个根节点管理所有动画，但是UI预制体却可能被拆分成多个
        //如果根节点需要播放子节点的动画，就必须确保它加载完毕才可以进行，如果这个逻辑写在逻辑层，逻辑层的逻辑会变得既复杂又很维护
        //暂时尝试一种折中的方案：如果当前正在播放动画，播放的过程中子节点加载完毕了，那么重新绑定动画节点，然后重播该动画（并且直接跳到某个时间点开始播)
        [NonSerialized]
        private string currentAnimationName = null;
        private void TryRePlayAnimation()
        {
            //Debug.LogError("try to re play animation");
            if (string.IsNullOrEmpty(currentAnimationName))
            {
                return;
            }
            //如果不在播放任何动画，则直接返回，合适的方式是既判断是否超出时间，又判断当前是否处于转换中，不过这里即使处于状态转化中，也不会重播动画
            //if (Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !Animator.IsInTransition(0))
            var currentState = mAnimator.GetCurrentAnimatorStateInfo(0);
            //Debug.LogError("current state is : " + currentState.shortNameHash + " current time is : " + currentState.normalizedTime);
            if (currentState.IsName("default") || currentState.normalizedTime > 1)
            {
                return;
            }
            //重播动画
            //mAnimator.Rebind();
            //Debug.LogError("before replay ");
            var shortNameHash = currentState.shortNameHash;
            var currentNormalizedTime = currentState.normalizedTime;
            //尝试重置到默认值，并没有成功 。。。。
            //mAnimator.ResetTrigger(currentAnimationName);
            //mAnimator.Play("default",-1,0.5f);
            //mAnimator.enabled = false;
            //mAnimator.enabled = true;
            ReBindAnimations();
            mAnimator.Play(shortNameHash, -1, currentNormalizedTime);
        }

        private void ReBindAnimations()
        {
            //Rebind方法 似乎是会重置Animator的默认值（设置enable也会重置默认值）
            //如果在播放动画的过程中调用了Rebind方法，可能会导致之后其它状态写会默认值的时候发生一些异常情况
            //对付这种问题，有以下几种方式：（1）对于某些Player，设置IgnoreRebindWhenPlaying为true （最简单，但是不适用所有场景）
            //(2)其它动画状态不依赖默认值，而是自己手动设置相关的值 (最靠谱，最通用，但是工作量比较大)
            //3)如果可以确保需要重新绑定的动画的最终状态是对的，那么其它状态可以设置Over Write Default 为false(不是很推荐)
            mAnimator.Rebind();
            IsAnimatorDirty = false;
            //Debug.LogError("animator rebind ! current trigger name is : " + currentAnimationName);
        }

        private bool IsSubOf(UIAniPlayer uiAniPlayer)
        {
            if (uiAniPlayer == null)
            {
                return false;
            }
            if (this == uiAniPlayer)
            {
                return true;
            }
            return uiAniPlayer.subAniPlayers != null && uiAniPlayer.subAniPlayers.Contains(this);
        }

    }


}