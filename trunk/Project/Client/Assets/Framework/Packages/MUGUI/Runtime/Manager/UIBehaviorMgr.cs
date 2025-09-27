using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class UIBehaviorMgr
    {
           
        #region 静态公共方法 获取所有的父节点Canvas列表

        private static List<Canvas> mTempCanvasList = new List<Canvas>();
        private static Dictionary<Canvas, List<Behaviour>> mNeedAutoEnableObjListDict = new Dictionary<Canvas, List<Behaviour>>();
        
        public static List<Behaviour> GetNeedAutoEnableObjList(Canvas canvas)
        {
            List<Behaviour> ret = null;
            if (mNeedAutoEnableObjListDict.TryGetValue(canvas, out ret))
            {
                return ret;
            }
            return null;
        }

        public static void RegisterNeedAutoEnable(Behaviour obj)
        {
            if (obj == null)
            {
                return;
            }
            Canvas canvas = GetTopParentCanvas(obj);
            if(canvas == null)
            {
                return;
            }
            List<Behaviour> ret = null;
            if (mNeedAutoEnableObjListDict.TryGetValue(canvas, out ret) == false)
            {
                ret = new List<Behaviour>();
                mNeedAutoEnableObjListDict.Add(canvas, ret);
            }
            ret.Add(obj);
        }

        public static void UnRegisterNeedAutoEnable(Behaviour obj)
        {
            if (obj == null)
            {
                return;
            }
            Canvas canvas = GetTopParentCanvas(obj);
            if (canvas == null)
            {
                return;
            }
            List<Behaviour> ret = null;
            if (mNeedAutoEnableObjListDict.TryGetValue(canvas, out ret) == false)
            {
                return;
            }
            ret.Remove(obj);
            if(ret.Count == 0)
            {
                mNeedAutoEnableObjListDict.Remove(canvas);
            }
        }

        public static Canvas GetTopParentCanvas(Behaviour com)
        {
            mTempCanvasList.Clear();
            com.gameObject.GetComponentsInParent<Canvas>(true, mTempCanvasList);
            if (mTempCanvasList.Count > 0)
            {
                return mTempCanvasList[mTempCanvasList.Count - 1];
            }
            return null;
        }

        #endregion
        
    }
}