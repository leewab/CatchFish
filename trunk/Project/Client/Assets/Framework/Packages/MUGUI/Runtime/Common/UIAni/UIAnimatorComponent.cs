using System;
using System.Collections.Generic;
using MUGame;
using UnityEngine;

namespace Game.UI
{
    public class UIAnimatorComponent
    {
        
        public bool IsRealPlaying
        {
            get
            {
                return mPlr != null && mPlr.IsRealPlaying();
            }
        }

		public UIAnimatorComponent(UIAniPlayer plr)	
		{
			mPlr = plr;
			mPlr.AniEventFunc = this.OnAniEvent;
		}

        public event Action<string> OnAniEventTriggered = null;

        //强制清除之前的mAniEndFunc回调，避免之后的动画强制播放时，触发之前的回调（这可能不是预期的行为）
        public void ClearAniEvent()
        {
            mAniEndFunc = null;
        }

        public void SetAnimatorSpeed(float speed)
        {
            mPlr.SetAnimatorSpeed(speed);
        }


        public bool PlayAni( string aniname, Action lua_func = null,bool force = true, bool reset = true)
		{
#if !GAME_EDITOR

            if (mIsCallback)
            {
                D.log("在上一个动画的结束触发事件时，再触发同一个Animator Controller 的Trigger，可能会导致Unity闪退，这是Unity内部的Bug。\n" +
                      "这里增加检测，并尝试规避这个问题，请对应程序确认问题 ！！\n",0);
                return false;
            }

			if (!force && IsRealPlaying)
			{
				return false;
			}

            //强制触发之前的回调
            if (force && mAniEndFunc != null)
            {
                mAniEndFunc();
                mAniEndFunc = null;
            }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            //检查参数是否存在
            if (!CheckHasTrigger(aniname))
            {
                Debug.LogErrorFormat("对应的trigger 不存在 ！{0}:{1}:{2}" + warningStr,mPlr.gameObject.name,mPlr.name,aniname);
                if(lua_func != null)
                {
                    lua_func();
                }
                return false;
            }
            //如果本次调用需要回调函数，但是没有设置正确的AnimationEvent，则报告一个错误
            if(lua_func != null && !CheckHasFinishEvent(aniname))
            {
                Debug.LogErrorFormat("对应的 onAniEvent 没有设置，这会导致lua端拿不到对应的回调！{0}:{1}:{2}" + warningStr, mPlr.gameObject.name, mPlr.name, aniname);
            }
            //如果有回调事件，但是一直不触发，也是一种错误，但是这种错误概率比较小，检查起来也比较麻烦，暂时不做检查
#endif
            mAniEndFunc = lua_func;
			mPlayingAniName = aniname;
			mPlr.PlayAni (aniname, reset);
			return true;
#endif
            return false;
        }

        private string warningStr = "";

        //检查动画参数是否合法，目前只在Editor模式下会开启检查
        private HashSet<string> triggerParamSet = null;
        public bool CheckHasTrigger(string aniname)
        {
            if(triggerParamSet == null)
            {
                triggerParamSet = new HashSet<string>();
                var parameters = mPlr.GetAllParams();
                foreach(var param in parameters)
                {
                    if(param.type == AnimatorControllerParameterType.Trigger)
                    {
                        triggerParamSet.Add(param.name);
                    }
                }
            }
            return triggerParamSet.Contains(aniname);
        }
        //检查某个trigger是否有对应的完成参数，目前只在Editor模式下会开启检查
        private HashSet<string> finishEventSet = null;
        public bool CheckHasFinishEvent(string aniname)
        {
            if(finishEventSet == null)
            {
                finishEventSet = new HashSet<string>();
                foreach(var animEvent in mPlr.GetAllAnimationEvent())
                {
                    if(animEvent.functionName == "OnAniEvent")
                    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                        if (string.IsNullOrEmpty(animEvent.stringParameter))
                        {
                            Debug.LogErrorFormat("OnAniEvent 的参数不应该为空，如果不需要，请删除此事件，{0}：{1}：{2}" + warningStr, mPlr.gameObject.name, mPlr.name, animEvent.time);
                        }else if(finishEventSet.Contains(animEvent.stringParameter))
                        {
                            //这个不算错误，有的时候可能会发生animator中有多个备用动画，但是目前只使用了其中少数几个，就会发生这种情况
                            Debug.LogWarningFormat("OnAniEvent的参数发生重复，请确认是否是刻意这么设计。 {0}:{1}:{2}" + warningStr,mPlr.gameObject.name,mPlr.name, animEvent.time);
                        }
#endif
                        finishEventSet.Add(animEvent.stringParameter);
                    }
                }
            }
            return finishEventSet.Contains(aniname);
        }

        //重置一个trigger状态，为某些特殊蛋疼情况使用
        public void ResetTrigger(string trigger_name)
        {
            if (CheckHasTrigger(trigger_name))
            {
                mPlr.ResetTrigger(trigger_name);
            }
        }

		private void OnAniEvent( string event_name )
		{
			//事件名和动画名相等时意味着此动画播放完毕
			if (event_name == mPlayingAniName )
			{
				if (mAniEndFunc != null)
				{
                    var endFunc = mAniEndFunc;
                    mIsCallback = true;
                    try
                    {
                        mAniEndFunc();
                    }
                    finally
                    {
                        mIsCallback = false;
                    }
                    mAniEndFunc = null;
                }
            }
            else
            {
                //其它情况下，则认为是自定义事件，由UI与程序共同约定
                if(OnAniEventTriggered != null)
                {
                    mIsCallback = true;
                    try
                    {
                        OnAniEventTriggered(event_name);
                    }
                    finally
                    {
                        mIsCallback = false;
                    }
                }
            }
		}
		private UIAniPlayer mPlr;
		private string mPlayingAniName;
		private Action mAniEndFunc;
        private bool mIsCallback = false;
        
    }
}