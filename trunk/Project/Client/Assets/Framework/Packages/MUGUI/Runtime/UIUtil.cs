using Game.Core;
using MUEngine;
using UnityEngine;

namespace Game.UI
{
    public static class UIUtil
    {
        public static GameObject GetGameObjectByPath(GameObject root,string path)
        {
            if (root == null) return null;
            if (string.IsNullOrEmpty(path)) return root;
            GameObject go = root.GetGameObjectByID(path);
            return go;
        }

        public static T ToGameUIComponent<T>(this GameObject go) where T : GameUIComponent
        {
            if (go)
            {
                EventTriggerListener uel = go.GetComponent<EventTriggerListener>();
                if (uel)
                {
                    return uel.parameter as T;
                }
                else
                    return null;
            }
            else
                return null;
        }
        
        public static GameUIComponent ToGameUIComponent(this GameObject go)
        {
            if (go)
            {
                EventTriggerListener uel = go.GetComponent<EventTriggerListener>();
                if (uel)
                {
                    return uel.parameter as GameUIComponent;
                }
                else
                    return null;
            }
            else
                return null;
        }

        
        //自动设置某个挂载在UI上的Entity的相关Renderer的SortingOrder
        //理论上，任何需要在UI播放特效，放置模型，想要正确的显示顺序，都可以通过这个方法来完成
        public static void AutoSetUIEntitySortingOrder(Entity targetEntity)
        {
            if (targetEntity == null)
            {
                return;
            }
            if(targetEntity.GameObject != null)
            {
                AutoSetUIEntitySortingOrder(targetEntity,targetEntity.GameObject);
            }
            else
            {
                //entity destory 的时候会清除这个回调，不自己去清除这个回调应该没问题
                targetEntity.OnLoadResource += () =>
                {
                    AutoSetUIEntitySortingOrder(targetEntity, targetEntity.GameObject);
                };
            }
        }
        
        private static void AutoSetUIEntitySortingOrder(Entity targetEntity,GameObject gameObject)
        {
            if(targetEntity == null || gameObject == null)
            {
                return;
            }
            GameObjectUtil.SetLayer(targetEntity.GameObject, LayerMask.NameToLayer("UI") , true);
            targetEntity.SetEffectOrderInLayer(GetNearestCanvasSortingOrder(gameObject));
        }
        
        private static int GetNearestCanvasSortingOrder(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return 0;
            }

            Canvas mCanvas = UIRoot.Instance.GetNearestCanvas(gameObject);
            return mCanvas != null ? mCanvas.sortingOrder : 0;
        }

    }
}