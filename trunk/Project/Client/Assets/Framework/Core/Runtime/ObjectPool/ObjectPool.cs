using UnityEngine;

namespace Game.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// 数量接口
    /// </summary>
    public interface ICountObserveAble
    {
        int CurCount { get; }
        int MaxCacheCount { set; get; }
    }

    /// <summary>
    /// 池子能力
    /// </summary>
    public interface IPoolAble
    {
        /// <summary>
        /// 是否已回收
        /// </summary>
        bool IsRecycled { get; set; }

        /// <summary>
        /// 自身回收(可以由自己实现)
        /// </summary>
        void Recycle2Cache();
    }

    /// <summary>
    /// 对象能力
    /// </summary>
    public interface IObjectAble
    {
        /// <summary>
        /// 响应初始化对象
        /// </summary>
        void Init();

        /// <summary>
        /// 响应释放对象
        /// </summary>
        void Release();
    }

    /// <summary>
    /// 对象个体
    /// </summary>
    public interface IObjectEntity : IObjectAble, IPoolAble
    {
    }

    public class ObjectPool : Singleton<ObjectPool>, ICountObserveAble
    {
        protected int mMaxCount = 12;
        protected Stack<IObjectEntity> mCacheStack = new Stack<IObjectEntity>();

        /// <summary>
        /// 当前数量
        /// </summary>
        public int CurCount => mCacheStack.Count;

        /// <summary>
        /// 最大缓存数量
        /// </summary>
        public int MaxCacheCount
        {
            get => mMaxCount;
            set
            {
                mMaxCount = value;
                if (mMaxCount > 0 && mMaxCount < mCacheStack.Count)
                {
                    int removeCount = mCacheStack.Count - mMaxCount;
                    while (removeCount > 0)
                    {
                        mCacheStack.Pop();
                        --removeCount;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="maxCount">最大缓存数量.</param>
        /// <param name="initCount">初时数量.</param>
        public void Init<T>(int maxCount, int initCount) where T : IObjectEntity, new()
        {
            if (maxCount > 0) initCount = System.Math.Min(maxCount, initCount);
            initCount = System.Math.Min(maxCount, initCount);
            if (CurCount < initCount)
            {
                for (int i = CurCount; i < initCount; ++i)
                {
                    Recycle(new T());
                }
            }
        }

        /// <summary>
        /// 开辟对象
        /// </summary>
        /// <returns></returns>
        public T Allocate<T>() where T : IObjectEntity, new()
        {
            IObjectEntity result = mCacheStack.Count == 0 ? new T() : mCacheStack.Pop();
            result.IsRecycled = false;
            result.Init();
            return (T)result;
        }

        /// <summary>
        /// 回收利用
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Recycle<T>(T t) where T : IObjectEntity
        {
            if (t == null || t.IsRecycled) return false;
            if (mMaxCount > 0)
            {
                if (mCacheStack.Count >= mMaxCount)
                {
                    t.Release();
                    return false;
                }
            }

            t.Release();
            t.IsRecycled = true;
            mCacheStack.Push(t);
            return true;
        }
    }

    /// <summary>
    /// 池子对象
    /// </summary>
    public abstract class ObjectEntity : IObjectEntity
    {
        public bool IsRecycled { get; set; }

        public abstract void Init();
        public abstract void Release();

        public virtual void Recycle2Cache()
        {
            ObjectPool.Instance.Recycle(this);
        }
    }

    public abstract class GameObjectEntity : ObjectEntity
    {
        /// <summary>
        /// GameObject对象
        /// </summary>
        private GameObject mObject;

        public GameObject GameObject => mObject;

        public GameObjectEntity(GameObject obj)
        {
            mObject = obj;
        }

        public virtual void Recycle2Cache(bool isDestroy)
        {
            if (isDestroy)
            {
                if (GameObject) UnityEngine.Object.DestroyImmediate(GameObject);
            }
            else
            {
                SetActive(false);
            }

            base.Recycle2Cache();
        }

        /// <summary>
        /// 设置模型显隐
        /// </summary>
        /// <param name="isActive"></param>
        public virtual void SetActive(bool isActive)
        {
            if (GameObject != null) GameObject.SetActive(isActive);
        }
    }
}