using System.Collections.Generic;
using MUEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public static class RaycastSystem
    {
        private static CachedRaycastHelper raycastHelper;

        //大部分的RaycastAll，同一帧内，在大概同一点进行检测，结果都应该相同
        //Unity内部的EventSystem要考虑同一帧，两次检测，有可能各种UI位置，Raycaster什么的发生了变化，所以每次都会重新进行计算
        //但是对于大多数情景，缓存本帧的第一次检测结果，是完全足够用的，所以增加一个特殊的，带缓存的RayCastAll扩展
        //碰撞结果List也不需要外部分配，内部会进行统一缓存，但是需要注意：这个List不应该进行更改，也不应该在本帧之后读取数据
        //如果需要保留结果，则应该自行复制一份数据
        /// <summary>
        /// 使用带缓存的RaycastAll,同一帧结果将被缓存。返回的数据不要进行更改，也不应该保存超过一帧。如果需要保存结果，需要自行复制结果
        /// </summary>
        /// <param name="eventSystem"></param>
        /// <param name="eventData"></param>
        /// <returns></returns>
        public static List<RaycastResult> CachedRaycastAll(this EventSystem eventSystem, PointerEventData eventData)
        {
            if(eventSystem == null)
            {
                return null;
            }
            if(raycastHelper == null)
            {
                raycastHelper = new CachedRaycastHelper();
            }
            return raycastHelper.CachedRaycastAll(eventSystem, eventData);
        }

        private class CachedRaycastHelper
        {
            private class PointResultPair
            {
                public Vector2 point;
                public List<RaycastResult> raycastResults = new List<RaycastResult>();
            }
            //PointResultPair缓存池
            private MemoryPool<PointResultPair> pointResultPairPools = new MemoryPool<PointResultPair>(10);
            private const float epsilon = 1;

            // Vector2 - > List<RaycastResult> 的映射，该数据只在同一帧有效
            private int lastCaculateFrameCount = -1;
            private List<PointResultPair> cachedRaycastResults = new List<PointResultPair>();
            private void ClearCachedRaycastResult()
            {
                for(int i = 0; i < cachedRaycastResults.Count; i++)
                {
                    var pair = cachedRaycastResults[i];
                    pair.raycastResults.Clear();
                    pointResultPairPools.Free(pair);
                }
                cachedRaycastResults.Clear();
            }

            public List<RaycastResult> CachedRaycastAll(EventSystem eventSystem,PointerEventData eventData) {
                if(lastCaculateFrameCount != Time.frameCount)
                {
                    ClearCachedRaycastResult();
                    lastCaculateFrameCount = Time.frameCount;
                }
                //尝试找到之前对同一个点的碰撞检测
                Vector2 currentPos = eventData.position;
                for (int i = 0; i < cachedRaycastResults.Count; i++)
                {
                    var pair = cachedRaycastResults[i];
                    var point = pair.point;
                    if (Mathf.Abs(point.x - currentPos.x) < epsilon && Mathf.Abs(point.y - currentPos.y) < epsilon)
                    {
#if UNITY_EDITOR && false
                        Debug.LogFormat("<color=#00ff00> {0} : 返回缓存碰撞结果，此时PointPOS 为 : {1} </color> \n" +
                            "当前堆栈为: \n {2}", lastCaculateFrameCount, currentPos,Environment.StackTrace);
#endif
                        return pair.raycastResults;
                    }
                }
                //真正进行碰撞检测，并且将结果缓存
                var pointResultPair = pointResultPairPools.Alloc();
                if(pointResultPair == null)
                {
                    pointResultPair = new PointResultPair();
                }
                eventSystem.RaycastAll(eventData, pointResultPair.raycastResults);

                pointResultPair.point = eventData.position;
                cachedRaycastResults.Add(pointResultPair);

#if UNITY_EDITOR
                if(cachedRaycastResults.Count > 10)
                {
                    Debug.LogErrorFormat("当前缓存的碰撞结果过多 ： {0}，请检查此处 ！！此Log目前只在Editor中可见", cachedRaycastResults.Count);
                }
#endif
                return pointResultPair.raycastResults;

            }
        }
    }
}