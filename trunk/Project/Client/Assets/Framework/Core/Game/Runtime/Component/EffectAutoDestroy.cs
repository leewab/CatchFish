using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MUEngine;

namespace MUGame
{
    //挂载在某个特效上的GameObject上的脚本，在Disable时将对应的特效删除
    //理论上这个组件应该写在引擎里，通过一个EntityComponent来将这个组件
    public class EffectAutoDestroy : MonoBehaviour
    {
        private int attachedEntityId = Entity.INVALID_ID;
        public void SetAttchedEntity(Entity entity)
        {
            if(entity == null)
            {
                return;
            }
            attachedEntityId = entity.ID;
        }

        private void OnDisable()
        {
            if (attachedEntityId != Entity.INVALID_ID)
            {
#if !UNITY_EDITOR_PROJECT
                //延迟到下一帧 从场景中删除自己，死循环
                //下一帧时，attatchEntity可能已经发生了变化
                var needDeleteEntityId = attachedEntityId;
                Game.Core.Common.NextFrameExecute += () =>
                {
                    MURoot.Scene.DelEntity(needDeleteEntityId);
#if EFFECT_TEST
                    if (needDeleteEntityId != Entity.INVALID_ID) {
                        Debug.LogError("delete attached effect : " + needDeleteEntityId);
                    }
#endif
                };
#else
                //不考虑复用的问题，直接干掉自己
                Destroy(gameObject);
#endif
            }
        }

    }
}

