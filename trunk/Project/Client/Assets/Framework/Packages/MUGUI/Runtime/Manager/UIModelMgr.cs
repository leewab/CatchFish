using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// UI模型管理器
    /// </summary>
    public static class UIModelMgr
    {
        private static Dictionary<Canvas, List<GameUIModel3D>> mActiveDict = new Dictionary<Canvas, List<GameUIModel3D>>();
        
        public static bool HasActive => mActiveDict.Count > 0;

        public static int CanvasCount => mActiveDict.Count;

        //后打开的层级小，就不要做Z的变更了，矛盾
        public static int GetLogicCount(Canvas c)
        {
            if (c == null)
                return 0;

            int count = 0;
            foreach (var item in mActiveDict)
            {
                if (item.Key != null && item.Key.sortingOrder <= c.sortingOrder)
                {
                    count++;
                }
            }
            
            return count;
        }

        public static List<GameUIModel3D> GetUIModel3D(Canvas c)
        {
            if (mActiveDict.ContainsKey(c))
            {
                return mActiveDict[c];
            }
            
            return null;
        }
        
        public static bool SetUIModel3D(Canvas c, GameUIModel3D uiModel3D)
        {
            if (!mActiveDict.ContainsKey(c)) mActiveDict[c] = new List<GameUIModel3D>();
            if (mActiveDict[c].IndexOf(uiModel3D) == -1)
            {
                mActiveDict[c].Add(uiModel3D);
            }
            
            return true;
        }
        
        public static bool RemoveUIModel3D(Canvas c, GameUIModel3D uiModel3D)
        {
            if (mActiveDict.ContainsKey(c))
            {
                mActiveDict[c].Remove(uiModel3D);
                if (mActiveDict[c].Count == 0) mActiveDict.Remove(c);
            }
            
            return true;
        }
        
    }
}