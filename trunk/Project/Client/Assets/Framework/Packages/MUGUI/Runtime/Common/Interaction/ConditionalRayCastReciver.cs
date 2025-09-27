using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//该脚本与一个Graphic子类挂载在同一GameObject上，允许在某些条件下忽略RayCast
//该脚本允许挂多个，只有全部都“认为”Cast了，最终结果才会是Cast（与Unity UI内部逻辑保持一致）
//add by liujunjie in 2019/1/8
namespace Game.UI
{
    [RequireComponent(typeof(Graphic))]
    public class ConditionalRayCastReciver : MonoBehaviour,ICanvasRaycastFilter
    {
        private Graphic _graphic;
        public Graphic Graphic {
            get
            {
                if (_graphic == null)
                {
                    _graphic = GetComponent<Graphic>();
                }
                return _graphic;
            }
        }
        private RectTransform rectTransform { get { return Graphic.rectTransform; } }

        //条件不分先后，但是暂时不允许重复（重复有意义吗？），所以用Set来保存数据
        private HashSet<Func<Vector2, Camera, bool>> conditionSet = new HashSet<Func<Vector2, Camera, bool>>();

        //增加新的条件
        public void AddCondition(Func<Vector2,Camera,bool> condition)
        {
            if (!conditionSet.Contains(condition)) conditionSet.Add(condition);
        }

        //删除原有的条件
        public void RemoveCondition(Func<Vector2,Camera,bool> condition)
        {
            if (conditionSet.Contains(condition)) conditionSet.Remove(condition);
        }

        //Unity UI 事件系统内部调用
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            foreach(var condition in conditionSet)
            {
                if (!condition(sp, eventCamera)) return false;
            }
            return true;
        }

        //允许外部直接设置IngoreRectTransform (IgnoreRect暂时不支持在Editor中设置)
        void Start()
        {
            if(ignoreRectTransform != null)
            {
                SetIngoreRectTransform(ignoreRectTransform);
            }
        }


        //一些预先定义好的条件函数
        //一个被忽略的矩形范围，这个Rect是以对应Graphic的RectTransfrom的左下角为（0,0）点,如果事件在这个范围内，则忽略本次点击
        [SerializeField]
        private Rect ignoreRect;
        private bool IsInIgnoreRect(Vector2 sp,Camera eventCamera)
        {
            Vector2 localPos;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, sp, eventCamera, out localPos)) return false;
            return !ignoreRect.Contains(localPos + Vector2.Scale(rectTransform.rect.size, rectTransform.pivot));
        }
        public void SetIgnoreRect(Rect rect)
        {
            ignoreRect = rect;
            if(!conditionSet.Contains(IsInIgnoreRect))
                AddCondition(IsInIgnoreRect);
        }

        //一个被忽略的RectTransform，如果本次点击事件可以“触碰到”对应的RectTransform（不管那个UI会不会接收事件），则忽略点击
        [SerializeField]
        private RectTransform ignoreRectTransform = null;
        private bool IsInIgnoreTransform(Vector2 sp,Camera eventCamera)
        {
            if (ignoreRectTransform == null || !ignoreRectTransform.gameObject.activeInHierarchy) return true;
            return !RectTransformUtility.RectangleContainsScreenPoint(ignoreRectTransform, sp, eventCamera);
        }
        public void SetIngoreRectTransform(RectTransform rectTransform)
        {
            ignoreRectTransform = rectTransform;
            if (!conditionSet.Contains(IsInIgnoreTransform))
                AddCondition(IsInIgnoreTransform);
        }

        //一个被忽略的Graphic根节点，该节点与所有子节点的Graphic如果有机会接收到事件，则忽略点击
        private HashSet<RectTransform> ignoreGrphicRootSet = new HashSet<RectTransform>();
        private bool IsInIgnoreGraphic(Vector2 sp,Camera eventCamera)
        {
            foreach(var ignoreGrphicRoot in ignoreGrphicRootSet)
            {
                if (ignoreGrphicRoot == null || !ignoreGrphicRoot.gameObject.activeInHierarchy)
                {
                    continue;
                }
                foreach (var graphic in ignoreGrphicRoot.GetComponentsInChildren<Graphic>())
                {
                    if (!graphic.gameObject.activeInHierarchy || !graphic.raycastTarget || graphic.canvasRenderer.cull)
                    {
                        continue;
                    }
                    if (RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, sp, eventCamera))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public void AddIgnoreGraphicRoot(RectTransform root)
        {
            ignoreGrphicRootSet.Add(root);
            if (!conditionSet.Contains(IsInIgnoreGraphic))
                AddCondition(IsInIgnoreGraphic);
        }
        public void RemoveIgnoreGraphicRoot(RectTransform root)
        {
            ignoreGrphicRootSet.Remove(root);
        }
        public void ClearIngoreGrphic()
        {
            ignoreGrphicRootSet.Clear();
            if (conditionSet.Contains(IsInIgnoreGraphic))
                RemoveCondition(IsInIgnoreGraphic);
        }

       
    }
}

