/* 
 * 通用列表条
 * 多行多列，自动分帧；单行单列，同一帧刷新；
 * 内存缓存机制，最大内存数 = 模板数量 * （视口内数量 + 2）
 * 视口内计算和刷新，无论多少条目，计算量总保证视口内数量
 * 创建和刷新时回调lua层，刷新采用差异刷新，或者用户强制刷新
 * 支持多种固定模板混排
*/
using System;
using System.Collections.Generic;
using Game.Core;
using MUGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameCommonList : GameUIComponent
    {
        #region 常量定义
        public const string TEMPLATE = "Template";  // 模板节点名字 //
        public const int NUM_TEMPLATE = 10;         // 模板数量 //
        public const float CHECK_TIME = 0.04f;      // Update间隔 //
        public const float CHECK_DELTA = 5f;        // 触发更新距离 //
        public const float HIDE_POS_Y = 100000f;     // 隐藏的高度 //
        public const float CHECK_DELTA_SPEED = 10f;  // 触发更新速度 //
        #endregion
        #region 变量定义
        private List<ItemData> mItemDataList = new List<ItemData>();             // 列表条数据列表 //
        private Stack<ItemUI>[] mUIPool = new Stack<ItemUI>[NUM_TEMPLATE];  // 列表条缓存池 //
        private List<ItemUI> mActiveUIList = new List<ItemUI>();               // 视口内有效列表条 //
        private RectTransform mCurtrans;            // 该列表控件所在节点 //
        private RectTransform mViewport;            // 滚动条控件节点，用来获取滚动条，和获取视口高度 //
        private ScrollRect mScrollRect;             // 滚动条 //
        private RectTransform[] mTemplateAttr;      // 模板数组 //
        private int[] mTemplatecnt;                 // 计算列表条编号用
        private int mMaxCount;                      // 最大列表条数量
        private int mTimerID;                       // 计时器id
        private int mOneRowOrColCount;              // 单行或者单列的数量
        private int mBeginCalculatePos;             // 开始计算的位置
        private bool mHasCalculatedHeight;          // 是否已经计算过高度
        private bool mIsVertical;                   // 是否纵向排列
        private bool mIsReverse;                    // 是否反向排列
        private bool mIsFirstActive;                // 是否第一次有效
        private bool mIsDirty;                      // 是否dirty 
        private float mBorderSizeX;                  // 条目边界大小(x方向)
        private float mBorderSizeY;                 // 条目边界大小(y方向)
        protected float mCheckTime;                 // update间隔
        protected Layout mLayout;                   // 用于做布局的对象
        private int mFrameBindCount;                // 分帧后，每一帧刷新数量 
        private int mInstantiateCount;              // 实例化控件的数量
        private int mBindCount;                     // 统计执行_Bind方法数量
        private bool mIsInstantiateEnough;          // 实例化数量是否足够多
        private float mTotalSize;
        private bool mIsNotifyMoveToEnd;            // 用来通知用户移动到了最后
        private bool mIsNotifyMoveToBegin;          // 用来通知用户移动到了最开头
        private bool mTraceScrollEnd;               // 表示当前是否处于对滚动停止的追踪状态。
        private int mLoopOffsetItemId;              // 循环滚动时使用的id偏移，通过结算，返回lua层的总是正确的itemId
        private bool mAutoAlign;                    // 自动对齐
        private int mAlignMode;                     // 对齐模式 0:居中,-1:靠左上（下界）,1:靠右下（上界）
        private int mFocusedItemId;                 // 当前焦点的条目id
        private bool mInsertLoopUpdate;             // 当前插入滚动列时调用的Update，这里不响应OnFocusedItemChange
        private int mMovingToItemId;                // 正在通过MoveToItem移动的条目，通过InsertLoopItem改变，要立即重新计算
        private float mMovingToItemSpeed;           // 正在通过MoveToItem移动的速度
        private float mLastCheckPosTime;            // 上一次CheckDeltaChange调用时间
        private CommonMutexComp _mutexComp;         // 外挂互斥组件

        // 创建列表条时的回调 
        private UnityEngine.Events.UnityAction<ItemUI> OnCreateGameChatUI;
        private UnityEngine.Events.UnityAction OnPositionChange;
        private UnityEngine.Events.UnityAction OnMoveToEnd;
        private UnityEngine.Events.UnityAction OnMoveToBegin;
        private UnityEngine.Events.UnityAction<int, int> OnAlignStop;
        private UnityEngine.Events.UnityAction OnScrollEnd;
        private UnityEngine.Events.UnityAction<int, int> OnFocusedItemChange;
        private UnityEngine.Events.UnityAction OnDragStart;
        private UnityEngine.Events.UnityAction OnDragEnd;
        private Action OnMoveToItem; // 内部回调
		//内部使用的一个回调，每次Update执行完毕之后会进行调用，用于将一些会立即更改UI显示效果的操作延迟到下一次Update，避免UI上的抖动
		//add by liujunjie in 2018/12/6
		private UnityEngine.Events.UnityAction OnUpdate;
        #endregion

        protected override void OnDispose()
        {
            base.Dispose();
            UIBehaviorMgr.UnRegisterNeedAutoEnable(mScrollRect);
            OnCreateGameChatUI = null;
            OnPositionChange = null;
            OnMoveToEnd = null;
            OnMoveToBegin = null;
            OnAlignStop = null;
            OnScrollEnd = null;
            OnFocusedItemChange = null;
            OnDragStart = null;
            OnDragEnd = null;
            OnMoveToItem = null;
            mUIPool = null;
            mActiveUIList = null;
            EndUpdate();
            mScrollRect = null;
        }
        
        #region 初始化
        
        protected override void OnInit()
        {
            base.OnInit();

            mCurtrans = gameObject.transform as RectTransform;
            mViewport = gameObject.transform.parent as RectTransform;

            // ScrollRect组件可有可无 //
            mScrollRect = mViewport.GetComponent<ScrollRect>();
            if (mScrollRect == null)
            {
                mScrollRect = mViewport.parent.GetComponent<ScrollRect>();
            }
            _mutexComp = this.GetComponent<CommonMutexComp>();

            UIBehaviorMgr.RegisterNeedAutoEnable(mScrollRect);

            mIsDirty = false;
            mMaxCount = 500;
            mTimerID = -1;
            mBeginCalculatePos = 0;
            mHasCalculatedHeight = false;
            mIsFirstActive = false;
            mCheckTime = 0.04f;
            mFrameBindCount = 0;
            mInstantiateCount = 0;
            mBindCount = 0;
            mIsInstantiateEnough = false;
            mTotalSize = 0;
            mIsNotifyMoveToEnd = true;
            mIsNotifyMoveToBegin = true;
            mTraceScrollEnd = false;
            mLoopOffsetItemId = 0;
            mAutoAlign = false;
            mAlignMode = 0;
            mFocusedItemId = 0;
            mInsertLoopUpdate = false;
            mMovingToItemId = 0;
            mLastCheckPosTime = 0f;

            GameCommonListPreview listPreview = mCurtrans.gameObject.GetComponent<GameCommonListPreview>();
            if (listPreview != null)
            {
                mBorderSizeX = listPreview.borderX;
                mBorderSizeY = listPreview.borderY;
                mOneRowOrColCount = listPreview.count;
                mIsVertical = listPreview.IsVertical;
            }
            else
            {
                mBorderSizeX = 0;
                mBorderSizeY = 0;
                mOneRowOrColCount = 1;
                if (mScrollRect != null)
                {
                    mIsVertical = !mScrollRect.horizontal;
                }
                else
                {
                    mIsVertical = true;
                }
            }
            
            if (mIsVertical)
            {
                mLayout = new LayoutY();
                mLayout.SetLayout(mCurtrans, mViewport);
                mIsReverse = (mCurtrans.pivot.y == 1) ? false : true;
            }
            else
            {
                mLayout = new LayoutX();
                mLayout.SetLayout(mCurtrans, mViewport);
                mIsReverse = (mCurtrans.pivot.x == 1) ? true : false;
            }

            if (mScrollRect != null)
            {
                mScrollRect.horizontal = !mIsVertical;
                mScrollRect.vertical = mIsVertical;
            }

            mFrameBindCount = mOneRowOrColCount > 1 ? mOneRowOrColCount : 0;
            GameObject tpl_root = null;
            if (mCurtrans.childCount > 0)
            {
                Transform t = mCurtrans.Find(TEMPLATE);
                if (t != null)
                {
                    if (t.name == TEMPLATE)
                    {
                        tpl_root = t.gameObject;
                    }
                }
            }
            mTemplateAttr = new RectTransform[NUM_TEMPLATE];
            mTemplatecnt = new int[NUM_TEMPLATE];

            for (int i = 0; i < NUM_TEMPLATE; i++)
            {
                mTemplatecnt[i] = 0;
                var trans = tpl_root.transform.Find(TEMPLATE + i.ToString());
                //没有相应的模板则直接略过。
                if (trans == null)
                    continue;
                mUIPool[i] = new Stack<ItemUI>();
                RectTransform rect = trans as RectTransform;
                //rect.anchorMin = new Vector2(0f, 1.0f);
                //rect.anchorMax = new Vector2(0f, 1.0f);
                mTemplateAttr[i] = rect;
                mTemplateAttr[i].anchoredPosition = new Vector2(mTemplateAttr[i].anchoredPosition.x, HIDE_POS_Y);
            }

            if (mScrollRect != null)
            {
                DragEventTriggerListener lis = DragEventTriggerListener.Get(mScrollRect.gameObject);
                lis.onDragStart += (GameObject go, Vector2 detta) =>
                {
                    mTraceScrollEnd = false;
                    if (OnDragStart != null)
                        OnDragStart();
                };
                lis.onDragEnd += (GameObject go, Vector2 detta) =>
                {
                    mTraceScrollEnd = true;
                    if (OnDragEnd != null)
                        OnDragEnd();
                };
            }
            if (mTimerID < 0)
            {
                BeginUpdate();
            }
        }

        #endregion
        #region 内部实现
        private ItemUI GetTemplate(ItemData data)
        {
            /* 分帧处理，最新修改为：实例化数量小于等于视口容纳数量时,
               可进行分帧刷新，否则不用分帧 */
            if (mFrameBindCount > 0 &&
                mBindCount > mFrameBindCount &&
                !mIsInstantiateEnough)
            {
                mIsDirty = true;
                return null;
            }

            // 缓存池里有，就用池里的 //
            var curpool = mUIPool[data.typedid];
            if (curpool.Count > 0)
            {
                return curpool.Pop();
            }

            // 缓存池没有就新创建一个 //
            ItemUI ret = new ItemUI();
            Transform go = GameObject.Instantiate(mTemplateAttr[data.typedid].transform);
            Vector3 oldScale = go.localScale;
            go.SetParent(mCurtrans);
            go.name = string.Format("template_new_{0}_{1}", data.typedid, mTemplatecnt[data.typedid]++);
            go.localScale = oldScale;
            go.localPosition = Vector3.zero;
            go.localRotation = Quaternion.identity;
            ret.Init(go, this);
            ret.typeid = data.typedid;

            // 计算实例化的数量 //
            mInstantiateCount++;
            if (!mIsInstantiateEnough)
            {
                int maxCount = GetViewPortMaxItemCount();
                int count = (mItemDataList.Count > maxCount) ? maxCount : mItemDataList.Count;
                if (mInstantiateCount >= count)
                {
                    mIsInstantiateEnough = true;
                }
            }

            // 回调lua层，让lua脚本创建控件 //
            if (OnCreateGameChatUI != null)
            {
                OnCreateGameChatUI(ret);
            }

            return ret;
        }

        /// <summary>
        /// 获取视口内能容纳的最大条目数，由于可能有多个模板，按最小模板尺寸计算
        /// </summary>
        /// <returns>返回数量</returns>
        private int GetViewPortMaxItemCount()
        {
            float minSize = 1000;
            for (int i = 0; i < mTemplateAttr.Length; i++)
            {
                if (mIsVertical)
                {
                    RectTransform rt = mTemplateAttr[i];
                    if (rt != null && rt.rect.height < minSize)
                    {
                        minSize = mTemplateAttr[i].rect.height;
                    }
                }
                else
                {
                    RectTransform rt = mTemplateAttr[i];
                    if (rt != null && rt.rect.width < minSize)
                    {
                        minSize = mTemplateAttr[i].rect.width;
                    }
                }
            }

            float viewportSize = mLayout.GetViewport();
            if (mIsVertical)
            {
                minSize = minSize + mBorderSizeY;
            }
            else
            {
                minSize = minSize + mBorderSizeX;
            }

            if (minSize == 0f)
            {
                return 1;
            }
            else
            {
                int count = (int)viewportSize / (int)minSize;
                return count * mOneRowOrColCount;
            }
        }

        public void Update()
        {
            // Timer停止，立刻停止
            if (mTimerID < 0)
            {
                return;
            }

            mBindCount = 0;

			// 自己计算anchoredPosition，因为RectTransform.anchoredPosition受布局的影响 //
			mLayout.CalculateAnchoredPosition(mIsReverse);

            bool isScrollEnd;
            bool isDeltaChange = mLayout.CheckDeltaChange(mLastCheckPosTime, out isScrollEnd);
            mLastCheckPosTime = Time.unscaledTime;

            // 触发列表滚动结束的事件，真正的惯性停止 //
            CheckScrollEnd(isScrollEnd);

            if (!mIsDirty && !isDeltaChange)
            {
                return;
            }

            if (isDeltaChange && this.OnPositionChange != null)
            {
                this.OnPositionChange();
            }

            if (mIsDirty && !mHasCalculatedHeight)
            {
                mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
				//由于CalculateAndAdjust可能会更改相关的数据，所以应该在CalculateAndAdjust调用完毕后再执行这个计算
				mLayout.CalculateAnchoredPosition(mIsReverse);
            }

            float min = mLayout.GetMinViewport(mIsReverse);
            float max = mLayout.GetMaxViewport(mIsReverse);

            // 如果列表移动到了最开头，触发一个事件 //
            CheckMoveToBegin(min);

            // 如果列表移动到了最后，触发一个事件 //
            CheckMoveToEnd(max);

            // 在之前有效的列表条中筛选，剔除超界的，留下新视口内的 //
            for (int i = mActiveUIList.Count - 1; i >= 0; i--)
            {
                var cur = mActiveUIList[i];
                if (!cur.cid.alive || (cur.cid.max <= min || cur.cid.min >= max))
                {
                    mUIPool[cur.cid.typedid].Push(cur);
                    cur.Unbind();
                    mActiveUIList.Remove(cur);
                }
                else
                {
                    if (mIsDirty)
                    {
                        mLayout.SetAnchoredPosition(cur);
                    }
                }
            }

            if (!mInsertLoopUpdate && mAutoAlign && OnFocusedItemChange != null)
            {
                int cid = GetFocusItemId(mAlignMode);
                int id = GetOriginItemId(cid);
                if (cid != mFocusedItemId)
                {
                    mFocusedItemId = cid;
                    if (mFocusedItemId > 0)
                    {
                        //Debug.Log(string.Format("--------OnFocusedItemChange {0}, {1}, {2} ", id, cid, mLoopOffsetItemId));
                        OnFocusedItemChange(id, cid);
                    }
                }
            }

            mIsDirty = false;
            int len = mItemDataList.Count;
            if (mBeginCalculatePos < 0 || mBeginCalculatePos >= len)
            {
                mBeginCalculatePos = 0;
            }

            if (mBeginCalculatePos >= len)
            {
                return;
            }

            mIsFirstActive = false;

            int pos;
            // 反向排列
            if (mIsReverse)
            {
                if (mItemDataList[mBeginCalculatePos].min < max)
                {
                    //首先迅速走到可行位置.
                    pos = mBeginCalculatePos;
                    for (; pos >= 0 && mItemDataList[pos].max <= min; pos--) ;
                    for (; pos >= 0 && mItemDataList[pos].min < max; pos--)
                    {
                        UpdateActiveUI(pos);
                    }
                }

                if (mBeginCalculatePos + 1 < len && mItemDataList[mBeginCalculatePos + 1].max > min)
                {
                    //首先迅速走到可行位置.
                    pos = mBeginCalculatePos + 1;
                    for (; pos < len && mItemDataList[pos].min >= max; pos++) ;
                    for (; pos < len && mItemDataList[pos].max > min; pos++)
                    {
                        UpdateActiveUI(pos);
                    }
                }
            }
            else
            {
                // 正向排列
                if (mItemDataList[mBeginCalculatePos].max > min)
                {
                    //首先迅速走到可行位置.
                    pos = mBeginCalculatePos;
                    for (; pos >= 0 && mItemDataList[pos].min >= max; pos--) ;
                    for (; pos >= 0 && mItemDataList[pos].max > min; pos--)
                    {
                        UpdateActiveUI(pos);
                    }
                }

                if (mBeginCalculatePos + 1 < len && mItemDataList[mBeginCalculatePos + 1].min < max)
                {
                    //首先迅速走到可行位置.
                    pos = mBeginCalculatePos + 1;
                    for (; pos < len && mItemDataList[pos].max <= min; pos++) ;
                    for (; pos < len && mItemDataList[pos].min < max; pos++)
                    {
                        UpdateActiveUI(pos);
                    }
                }
            }
			if (OnUpdate != null) {
				OnUpdate ();
			}
        }

        private void UpdateActiveUI(int pos)
        {
            ItemData item = mItemDataList[pos];
            if (!mIsFirstActive)
            {
                mBeginCalculatePos = pos;
                mIsFirstActive = true;
            }

            if (item.active)
            {
                return;
            }

            var cur = GetTemplate(item);
            if (cur != null)
            {
                cur.Bind(item);
                mLayout.SetAnchoredPosition(cur);
                mActiveUIList.Add(cur);
                mBindCount++;
            }

        }

        protected void SetDirty()
        {
            mIsDirty = true;
            mHasCalculatedHeight = false;
            mIsNotifyMoveToEnd = true;
            mIsNotifyMoveToBegin = true;

            if (_mutexComp != null)
            {
                _mutexComp.CheckSetMutex(mItemDataList.Count == 0);
            }
        }

        /// <summary>
        /// 触发列表滚动结束的事件，真正的惯性停止
        /// </summary>
        /// <param name="isScrollEnd">是否停止</param>
        protected void CheckScrollEnd(bool isScrollEnd)
        {
            if (mTraceScrollEnd && isScrollEnd)
            {
                mTraceScrollEnd = false;
                //滚动真正地停止了，触发事件。
                if (OnScrollEnd != null)
                {
                    OnScrollEnd();
                }
                if (mAutoAlign)
                {
                    if (mScrollRect != null)
                        mScrollRect.StopMovement();
                    int id = GetFocusItemId(mAlignMode);
                    MoveToItem(id, mAlignMode, 100);
                }
            }
        }

        /// <summary>
        /// 检测列表条移动到最后的事件，精度为5
        /// </summary>
        /// <param name="max">视口最大值</param>
        protected void CheckMoveToEnd(float max)
        {
            if (mItemDataList.Count > 0 && mIsNotifyMoveToEnd)
            {
                ItemData id = mItemDataList[mItemDataList.Count - 1];
                if ((max - 5) >= id.max)
                {
                    if (OnMoveToEnd != null)
                    {
                        OnMoveToEnd();
                        mIsNotifyMoveToEnd = false;
                    }
                }

            }
        }

        /// <summary>
        /// 检测列表条移动到最开头的事件，精度为5
        /// </summary>
        /// <param name="max">视口最小值</param>
        protected void CheckMoveToBegin(float min)
        {
            if (mItemDataList.Count > 0 && mIsNotifyMoveToBegin)
            {
                ItemData id = mItemDataList[0];
                if ((min + 5) <= id.min)
                {
                    if (OnMoveToBegin != null)
                    {
                        OnMoveToBegin();
                        mIsNotifyMoveToBegin = false;
                    }
                }

            }
        }

        protected void AutoAlignStop()
        {
            if (OnAlignStop != null)
            {
                int cid = GetFocusItemId(mAlignMode);
                int id = GetOriginItemId(cid);
                OnAlignStop(id, cid);
                SetDirty();
            }
        }

        #endregion
        #region 对外接口

        /// <summary>
        /// 清理条目，仅仅清理数据
        /// </summary>
        public void Clear()
        {
            foreach (var cur in mItemDataList)
            {
                cur.alive = false;
            }

            mItemDataList.Clear();
            SetDirty();
        }

        /// <summary>
        /// 清理条目，真正的删除GameObject
        /// </summary>
        public void ClearItem()
        {
            EndUpdate();
            
            for ( int i = 0; i < mActiveUIList.Count; i++ )
            {
                mActiveUIList[i].DestroySelf();
            }
            mActiveUIList.Clear();

            for ( int i = 0; i < mUIPool.Length; i++ )
            {
                Stack<ItemUI> stack = mUIPool[i];
                if (stack == null)
                {
                    continue;
                }

                foreach( ItemUI ui in stack )
                {
                    ui.DestroySelf();
                }
                stack.Clear();
            }

            mLoopOffsetItemId = 0;
            mFocusedItemId = 0;
            mMovingToItemId = 0;
        }

        /// <summary>
        /// 添加所有条目
        /// </summary>
        /// <param name="typeId">模板类型</param>
        /// <param name="count">条目数量</param>
        public void AddItem(int typeId, int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddItem(typeId);
            }
        }

        /// <summary>
        /// 从尾部添加一个条目
        /// </summary>
        /// <param name="typeId">模板类型</param>
        /// <returns>真实的位置，从1开始</returns>
        public int AddItem(int typeId)
        {
            if (typeId < 0 || typeId >= NUM_TEMPLATE || mTemplateAttr[typeId] == null)
            {
                Debug.Log("需要的模板类型" + typeId + "在当前层级下不存在。");
                return 1;
            }

            ItemData data = new ItemData();
            data.itemid = mItemDataList.Count + 1;
            data.typedid = typeId;

            // 计算尺寸
            mLayout.CalLayoutData(data, mTemplateAttr[typeId], mIsVertical);

            mItemDataList.Add(data);
            if (mItemDataList.Count > mMaxCount)
            {
                mItemDataList[0].alive = false;
                mItemDataList.RemoveAt(0);
            }

            SetDirty();
            return data.itemid;
        }

        /// <summary>
        /// 从尾部添加一个条目
        /// </summary>
        /// <param name="typeId">模板类型</param>
        /// <returns>真实的位置，从1开始</returns>
        /// <returns>该条目的高度</returns>
        public int AddItemWithHeight(int typeId, float height)
        {
            if (typeId < 0 || typeId >= NUM_TEMPLATE || mTemplateAttr[typeId] == null)
            {
                Debug.Log("需要的模板类型" + typeId + "在当前层级下不存在。");
                return 1;
            }

            ItemData data = new ItemData();
            data.itemid = mItemDataList.Count + 1;
            data.typedid = typeId;

            // 计算尺寸
            mLayout.CalLayoutData(data, mTemplateAttr[typeId], mIsVertical);
            data.height = height;
            mItemDataList.Add(data);
            if (mItemDataList.Count > mMaxCount)
            {
                mItemDataList[0].alive = false;
                mItemDataList.RemoveAt(0);
            }

            SetDirty();
            return data.itemid;
        }

        /// <summary>
        /// 插入一个条目
        /// </summary>
        /// <param name="typeId">模板类型</param>
        /// <param name="pos">插入位置，从1开始</param>
        public void InsertItem(int typeId, int pos)
        {
            if (typeId < 0 || typeId >= NUM_TEMPLATE || mTemplateAttr[typeId] == null)
            {
                Debug.Log("需要的模板类型" + typeId + "在当前层级下不存在。");
                return;
            }

            //Insert应该是把原来位置的item"挤到"后面去，当pos == mItemDataList.Count时，表示当前最后一个Item会往后移动一位
            //此处之前的处理有问题,将其进行修改。如果之前有地方依赖了这个错误处理，请自行修改 modify by liujunjie in 2018/11/13
            if (pos - 1 == mItemDataList.Count)
            {
                AddItem(typeId);
                return;
            }

            ItemData data = new ItemData();
            data.itemid = pos;
            data.typedid = typeId;
            // 计算尺寸
            mLayout.CalLayoutData(data, mTemplateAttr[typeId], mIsVertical);

            mItemDataList.Insert(pos - 1, data);
            for (int i = pos; i < mItemDataList.Count; i++)
            {
                mItemDataList[i].itemid++;
            }

            if (mItemDataList.Count > mMaxCount)
            {
                mItemDataList[0].alive = false;
                mItemDataList.RemoveAt(0);
            }

            SetDirty();
        }

        /// <summary>
        /// 获取当前逻辑Item数据数量 （不一定有对应的UI）
        /// </summary>
        /// <returns></returns>
        public int GetItemDataCount()
        {
            if (mItemDataList == null) return 0;
            return mItemDataList.Count;
        }
        /// <summary>
        /// 获取当前ItemUI的数量
        /// </summary>
        /// <returns></returns>
        public int GetItemUICount()
        {
            if (mActiveUIList == null) return 0;
            return mActiveUIList.Count;
        }

        /// <summary>
        /// 插入一个循环条目，同时删除一个条目，用于循环滚动
        /// </summary>
        /// <param name="typeId">模板类型</param>
        /// <param name="pos">插入位置，最开头还是最后</param>
        /// <param name="mode">预期对齐类型，最终移动的位置会尽量符合预期,0:居中,-1:靠左上（下界）,1:靠右下（上界）</param>
        /// <param name="mode">替换位置的条目数量</param>
        public void InsertLoopItem(int typeId, bool isHead, int mode = 0, int cnt = 1)
        {
            if (typeId < 0 || typeId >= NUM_TEMPLATE || mTemplateAttr[typeId] == null)
            {
                Debug.Log("需要的模板类型" + typeId + "在当前层级下不存在。");
                return;
            }
            if (cnt < 1)
            {
                Debug.Log("模板类型" + typeId + "交换数量为0，无法交换位置。");
                return;
            }
            if (mActiveUIList.Count + cnt > mItemDataList.Count)
            {
                Debug.Log("模板类型" + typeId + "激活控件和交换数量之和大于当前层数数量，交换会出现穿帮，无法交换位置。");
                return;
            }
            int focusId = 0; // GetFocusItemId(mode);
            for (int i = 0; i < mActiveUIList.Count; ++i)
            {
                var cur = mActiveUIList[i];
                if (cur.cid.alive)
                {
                    focusId = cur.cid.itemid;
                    break;
                }
            }
            if (focusId == 0)
            {
                Debug.Log("模板类型" + typeId + "当前无法找到焦点条目，无法交换位置。");
                return;
            }

            // 正在进行MoveToItem，暂停住，否则添加之后又回到之前的焦点
            if (moveTimerId != -1)
            {
                TimerHandler.RemoveTimeactionByID(moveTimerId);
                moveTimerId = -1;
            }

            float oriPos = mItemDataList[focusId-1].min;
            if (isHead)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    RemoveItem(mItemDataList.Count - i);
                    InsertItem(typeId, 1 + i);
                }
                mLoopOffsetItemId += cnt;
                mLoopOffsetItemId = CalcMod(mLoopOffsetItemId);
                focusId += cnt;
            }
            else
            {
                for (int i = 0; i < cnt; ++i)
                {
                    RemoveItem(1);
                    InsertItem(typeId, mItemDataList.Count);
                }
                mLoopOffsetItemId -= cnt;
                mLoopOffsetItemId = CalcMod(mLoopOffsetItemId);
                focusId -= cnt;
            }

            // 计算新的位置
            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            //由于CalculateAndAdjust可能会更改相关的数据，所以应该在CalculateAndAdjust调用完毕后再执行这个计算
            mLayout.CalculateAnchoredPosition(mIsReverse);

            int newFocusId = (focusId - 1) % mItemDataList.Count + 1;
            float newPos = mItemDataList[newFocusId - 1].min;
            // 计算偏移，将content重置为对应位置，保证显示不变
            float offset = newPos - oriPos;
            delayMoveOffset = mIsVertical ? new Vector2(0, offset) : new Vector2(-offset, 0);
            mCurtrans.anchoredPosition += delayMoveOffset;
            if (mScrollRect != null && mScrollRect.GetType() == typeof(LoopScrollRect))
                ((LoopScrollRect)mScrollRect).SetOffsetAfterInsert(delayMoveOffset);
            mInsertLoopUpdate = true;
            ForceUpdate();
            mInsertLoopUpdate = false;
            ForceUpdate();
            // 刷新新位置
            if (mAutoAlign)
                AutoAlignStop();
        }

        private int CalcMod(int num)
        {
            num = num - 1;
            if (num < 0)
            {
                float s = Mathf.Floor(num * 0.1f / mItemDataList.Count);
                return (int)(num - s * mItemDataList.Count) + 1;
            }
            else
                return num % mItemDataList.Count + 1;
        }

        /// <summary>
        /// 得到特定对齐类型下，当前的焦点条目Id
        /// </summary>
        /// <param name="mode">预期对齐类型，最终移动的位置会尽量符合预期,0:居中,-1:靠左上（下界）,1:靠右下（上界）</param>
        /// <returns>焦点条目id</returns>
        public int GetFocusItemId(int mode = 0)
        {
            int id = 0;
            if (mode == 0)
            {
                float min = mLayout.GetMinViewport(mIsReverse);
                float max = mLayout.GetMaxViewport(mIsReverse);
                float mid = (min + max) / 2;
                for (int i = 0; i < mActiveUIList.Count; ++i)
                {
                    var cur = mActiveUIList[i];
                    if (cur.cid.alive && (mid >= cur.cid.min && mid <= cur.cid.max ))
                    {
                        id = cur.cid.itemid;
                        break;
                    }
                }
            }
            else if (mode == -1)
            {
                float min = -999999;
                for (int i = 0; i < mActiveUIList.Count; ++ i)
                {
                    var cur = mActiveUIList[i];
                    if (cur.cid.alive && cur.cid.min >= min)
                    {
                        min = cur.cid.min;
                        id = cur.cid.itemid;
                    }
                }
            }
            else if (mode == 1)
            {
                float max = 0;
                for (int i = mActiveUIList.Count - 1; i >= 0; -- i)
                {
                    var cur = mActiveUIList[i];
                    if (cur.cid.alive && cur.cid.max <= max)
                    {
                        max = cur.cid.max;
                        id = cur.cid.itemid;
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// 得到列表最初建立时的索引，可能滚动循环的时候，会改变整个dataList的顺序，但索引始终保持最初的，这样lua层使用起来比较方便
        /// </summary>
        /// <param name="mode">ItemData中的itemId</param>
        /// <returns>初始列表顺序的itemId</returns>
        public int GetOriginItemId(int itemid)
        {
            if (mItemDataList.Count == 0)
                return itemid;
            int a = itemid - mLoopOffsetItemId - 1;
            if (a < 0)
            {
                float s = Mathf.Floor(a * 0.1f / mItemDataList.Count);
                return (int)(a - s * mItemDataList.Count) + 1;
            }
            else
                return a % mItemDataList.Count + 1;
        }

        /// <summary>
        /// 修改某个位置的条目
        /// </summary>
        /// <param name="pos">位置，从1开始</param>
        public void ModifyItem(int pos)
        {
            if (pos > mItemDataList.Count || pos < 1)
            {
                return;
            }

            ItemData data = mItemDataList[pos - 1];

            for (int i = 0; i < mActiveUIList.Count; i++)
            {
                ItemUI itemUI = mActiveUIList[i];
                if (itemUI.cid.itemid == pos)
                {
                    itemUI.Bind(data);
                    break;
                }
            }
        }

        /// <summary>
        /// 删除条目
        /// </summary>
        /// <param name="pos">删除的位置，从1开始</param>
        public void RemoveItem(int pos)
        {
            if (pos > mItemDataList.Count || pos < 1)
            {
                return;
            }

            ItemData data = mItemDataList[pos - 1];
            int itemId = data.itemid;
            for (int i = 0; i < mActiveUIList.Count; i++)
            {
                var cur = mActiveUIList[i];
                if (cur.cid.itemid == itemId)
                {
                    mUIPool[cur.cid.typedid].Push(cur);
                    cur.Unbind();
                    mActiveUIList.Remove(cur);
                    break;
                }
            }

            mItemDataList.RemoveAt(pos - 1);
            for (int i = pos - 1; i < mItemDataList.Count; i++)
            {
                mItemDataList[i].itemid--;
            }

            SetDirty();
        }

        /// <summary>
        /// 立刻刷新
        /// </summary>
        public void RepositionNow()
        {
            for (int i = 0; i < mItemDataList.Count; i++)
            {
                var cur = mItemDataList[i];
                mLayout.CalLayoutData(cur, mTemplateAttr[cur.typedid], mIsVertical);
            }

            SetDirty();
            Update();
        }

        /// <summary>
        /// 下一帧刷新
        /// </summary>
        public void RepositionNextFrame()
        {
            SetDirty();
        }

        /// <summary>
        /// 当前视口内条目，执行刷新方法
        /// </summary>
        public void RefreshNow()
        {
            for (int i = 0; i < mActiveUIList.Count; i++)
            {
                ItemUI itemUI = mActiveUIList[i];
                if (itemUI.cid.alive)
                    itemUI.Bind(itemUI.cid);
            }
        }

        /// <summary>
        /// 设置总条目
        /// </summary>
        /// <param name="maxCount">最大数量</param>
        public void SetMaxItemsCount(int maxCount)
        {
            this.mMaxCount = maxCount;
        }

        /// <summary>
        /// 设置单行或者单列可容纳数量
        /// </summary>
        /// <param name="count">可容纳数量</param>
        public void SetOneRowOrColCount(int count)
        {
            mOneRowOrColCount = count;
        }

        /// <summary>
        /// 设置创建新列表条回调
        /// </summary>
        /// <param name="OnCreateGameChatUI">lua 方法</param>
        public void SetUICreateListener(UnityEngine.Events.UnityAction<ItemUI> OnCreateGameChatUI)
        {
            this.OnCreateGameChatUI = OnCreateGameChatUI;
        }

        /// <summary>
        /// 滚动条改变回调
        /// </summary>
        /// <param name="OnPositionChange"></param>
        public void SetPositionChange(UnityEngine.Events.UnityAction OnPositionChange)
        {
            this.OnPositionChange = OnPositionChange;
        }

        /// <summary>
        /// 滚动条改变回调
        /// </summary>
        /// <param name="OnFocusedItemChange"></param>
        public void SetFocusedItemChange(UnityEngine.Events.UnityAction<int, int> OnFocusedItemChange)
        {
            this.OnFocusedItemChange = OnFocusedItemChange;
        }

        /// <summary>
        /// 列表条移动到最后，触发一次事件，回调给lua层
        /// </summary>
        /// <param name="OnMoveToEnd"></param>
        public void SetMoveToEndListener(UnityEngine.Events.UnityAction OnMoveToEnd)
        {
            this.OnMoveToEnd = OnMoveToEnd;
        }

        /// <summary>
        /// 列表条移动到最开头，触发一次事件，回调给lua层
        /// </summary>
        /// <param name="OnMoveToBegin"></param>
        public void SetMoveToBeginListener(UnityEngine.Events.UnityAction OnMoveToBegin)
        {
            this.OnMoveToBegin = OnMoveToBegin;
        }

        /// <summary>
        /// 列表条的滚动真正停止，考虑惯性。
        /// </summary>
        /// <param name="OnScrollEnd"></param>
        public void SetScrollEndListener(UnityEngine.Events.UnityAction OnScrollEnd)
        {
            this.OnScrollEnd = OnScrollEnd;
        }

        /// <summary>
        /// 拖拽开始
        /// </summary>
        /// <param name="OnDragStart"></param>
        public void SetDragStartListener(UnityEngine.Events.UnityAction OnDragStart)
        {
            this.OnDragStart = OnDragStart;
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        /// <param name="OnDragEnd"></param>
        public void SetDragEndListener(UnityEngine.Events.UnityAction OnDragEnd)
        {
            this.OnDragEnd = OnDragEnd;
        }

        /// <summary>
        /// 移动到开始位置
        /// </summary>
        public void MoveToBegin()
        {
            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            mHasCalculatedHeight = true;
            if (mIsVertical)
            {
                if (mScrollRect != null)
                {
                    mScrollRect.verticalNormalizedPosition = mIsReverse ? 0.0f : 1.0f;
                }
            }
            else
            {
                if (mScrollRect != null)
                {
                    mScrollRect.horizontalNormalizedPosition = mIsReverse ? 1.0f : 0.0f;
                }
            }

            mLayout.SetDeltaChanged();

            Update();
        }

        /// <summary>
        /// 移动到末尾
        /// </summary>
        public void MoveToEnd()
        {
            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            mHasCalculatedHeight = true;
            if (mIsVertical)
            {
                if (mScrollRect != null)
                {
                    mScrollRect.verticalNormalizedPosition = mIsReverse ? 1.0f : 0.0f;
                }
            }
            else
            {
                if (mScrollRect != null)
                {
                    mScrollRect.horizontalNormalizedPosition = mIsReverse ? 0.0f : 1.0f;
                }
            }

            mLayout.SetDeltaChanged();

            Update();
        }

        private int moveTimerId = -1;
        /// <summary>
        /// 原有的MoveToItem功能比较单一，这里重写一个功能更多的MoveToItem函数,理论上可以完整的实现原来MoveToItem的功能
        /// add by liujunjie in 2018/11/15
        /// </summary>
        /// <param name="itemId">条目Id,从1开始</param>
        /// <param name="mode">预期对齐类型，最终移动的位置会尽量符合预期,0:居中,-1:靠左上（下界）,1:靠右下（上界）</param>
        /// <param name="moveSpeed">移动速度，为0表示立刻到达</param>
        /// <param name="onMoveEnd">移动完成的回调</param>
        public void MoveToItem(int itemId,int mode = 0,float moveSpeed = 0,Action onMoveEnd = null)
        {
            ItemData itemData = mItemDataList.Find(item => item.itemid == itemId);
            if (itemData == null) return;
            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            mHasCalculatedHeight = true;
			mLayout.CalculateAnchoredPosition (mIsReverse);	//GetMinViewport和GetMaxViewport会依赖这个函数的调用结果
            Vector2 totalOffset = GetMoveOffset(itemData, mode);
            Vector2 targetPos = mCurtrans.anchoredPosition + totalOffset;
            //如果moveSpeed为0或者移动距离很短，则立即移动到指定位置（逻辑上移动完毕，UI上不一定立刻刷新）
            if (moveSpeed == 0 || totalOffset.magnitude < Mathf.Max(moveSpeed * 0.01f, 10))
            {
                mCurtrans.anchoredPosition = targetPos;
                mLayout.SetDeltaChanged();
                if (mScrollRect != null && mAutoAlign)
                    mScrollRect.StopMovement();
                if (onMoveEnd != null)
                {
                    onMoveEnd();
                }
                else
                {
                    if (mAutoAlign)
                        AutoAlignStop();
                }
                return;
            }
            if (moveTimerId != -1)
            {
                TimerHandler.RemoveTimeactionByID(moveTimerId);
            }
            mMovingToItemId = itemId;
            mMovingToItemSpeed = moveSpeed;
            mAlignMode = mode;
            OnMoveToItem = onMoveEnd;
            Vector2 normalizedOffset = totalOffset.normalized;  //理论上，这个值应该是(0,1),(0,-1),(1,0)或者(-1,0)，代表着移动方向
            moveTimerId = TimerHandler.SetTimeout(() => {
                var moveOffset = normalizedOffset * moveSpeed * MUEngine.MURoot.RealTime.DeltaTime;
                totalOffset -= moveOffset;  //记录当前移动的进度，用于判断是否移动完成
                if (Vector2.Dot(totalOffset,normalizedOffset) > 0)  //经过多次减少之后的总位移仍然与位移方向在同一方向，则说明没有位移到达到或超过终点的位置
                {
                    mCurtrans.anchoredPosition += moveOffset;
                    //ForceUpdate();
                }
                else
                {   //位移已经到达或者超过终点，直接让它强制移动到目标点
                    mCurtrans.anchoredPosition = targetPos;
                    TimerHandler.RemoveTimeactionByID(moveTimerId);
                    moveTimerId = -1;
                    if (mScrollRect != null && mAutoAlign)
                        mScrollRect.StopMovement();
                    if (onMoveEnd != null)
                    {
                        onMoveEnd();
                    }
                    else
                    {
                        if (mAutoAlign)
                            AutoAlignStop();
                    }
                    OnMoveToItem = null;
                }
                mLayout.SetDeltaChanged();
            }, 0.001f, true, false);
        }
        //计算把某个ItemData移动到指定位置，Content的AnchorPosition需要改变的的位移量
        //该函数的假定与MoveItemToViewport一致
        private Vector2 GetMoveOffset(ItemData itemData,int mode)
        {
            //调用该函数前，所有的ItemData位置不应该出现超过界限的情况(第一个ItemData相对视口下界（左，上）位置超过0，或者最后一个ItemData相对视口上界（右，下）位置小于0)
            //这里使用相对位移，不对Content的锚点和轴点做任何假定
            float offset = 0;
            //此时，整个视口内的Content中，相对起始点（左，上）的最小偏移量和最大偏移量，也就是视口最 左/上 和 右/下 的“位置”
            float minPosInViewport = mLayout.GetMinViewport(mIsReverse);
            float maxPosInViewport = mLayout.GetMaxViewport(mIsReverse);
            //增加一种特殊情况的处理：当Content总长度小于Viewport的长度时。使用原来的Clamp会导致计算结果无法预料（maxPosInViewport - mTotalSize将大于minPosInViewport）
            //这种情况下，应该根据是否是reverse,始终返回移动到起始或者末尾位置的偏移量
            if(mTotalSize <= maxPosInViewport - minPosInViewport)
            {
                offset = mIsReverse ? maxPosInViewport - mTotalSize : minPosInViewport;
            }
            else
            {
                //计算移动到目标位置需要的偏移量
                if (mode == 0)
                {
                    offset = (minPosInViewport + maxPosInViewport) / 2 - (itemData.max + itemData.min) / 2;
                }
                else if (mode == 1)
                {
                    offset = maxPosInViewport - itemData.max;
                }
                else if (mode == -1)
                {
                    offset = minPosInViewport - itemData.min;
                }
                //检查这个偏移量（向正方向（右，下）移动为正，向反方向（左，上）移动为负）是否在范围内，最终的偏移量应该保证：
                //1.小于minPosInViewport ,如果大于该值，最终会向正方向移动到把第一个ItemData的下界（左，上）超过视口下界（左，上）的位置
                //2.大于maxPosInViewport - mTotalSize ,如果小于该值，最终会向反方向移动到最后一个ItemData的上界（右，下）超过视口上界（右，下）的位置
                offset = Mathf.Clamp(offset, maxPosInViewport - mTotalSize, minPosInViewport);
            }
            //根据最终的位移计算最终的目标AnchorPosition
            return mIsVertical ? new Vector2(0, -offset) : new Vector2(offset, 0);
        }

        /// <summary>
        /// 把某个item移动到视口，尽量保证这个item全部在视口中，如果这个item已经全部在视口中，则不会做任何操作
        /// 该函数会尽量少的进行移动，如果item大于视口范围，则会通过尽量少的移动让它占满整个视口
        /// add by liujunjie in 2018/11/13
        /// </summary>
        /// <param name="itemId"></param>
		/// <param name="delayMove">是否延迟更新，如果为true，那会等到下一次界面刷新时才能更改位置，目前只有很特殊的情况下才可能需要设置</param>
        /// <returns>最后进行的偏移量</returns>
		public Vector2 MoveItemToViewport(int itemId,bool delayMove = false)
        {
            var itemData = mItemDataList.Find(data => data.itemid == itemId);
            if (itemData == null) return Vector2.zero;
            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            mHasCalculatedHeight = true;
			mLayout.CalculateAnchoredPosition (mIsReverse);	//GetMinViewport和GetMaxViewport会依赖这个函数的调用结果
            //此时，itemData对应item的当前位置（并非实际位置，而是理论计算位置）
            //之前的代码中有一些隐含的假定，这里会进行使用，用到的假定有：
            //1.当List是横向时，无论是否反向，每个Item（由对应的Template确定）的锚点和轴点都在左上角(anchorMin = anchorMax = pivot = Vector2(0,1))
            //2.当List是纵向时，无论是否反向（从上到下为正向），每个Item的锚点和轴点都在左上角（与横向一致）
            //3.所有的Item都在一个Content中，这个Content的父节点必定就是Viewport，通过mLayout的GetMinViewport和GetMaxViewport方法能得到此时Content在视口下
            //的最小（横向的话，是视口中的最左端与Content整体的最左端的距离、纵向的话，就是视口中的最上端与Content整体的最上端的距离）值和最大值，无论是否反向。
            //纵向反向的计算似乎有点问题，但是这里将直接使用GetMinViewport和GetMaxViewport函数的结果
            var leftOrTopOffset = mLayout.GetMinViewport(mIsReverse) - itemData.min;
            var rightOrBottomOffset = mLayout.GetMaxViewport(mIsReverse) - itemData.max;
            if (leftOrTopOffset <= 0 && rightOrBottomOffset >= 0)
            {
                //上下界都在视口内，不需要移动
				//如果之前有其它的MoveItemToViewport，则覆盖掉它预期的位移
				delayMoveOffset = Vector2.zero;
                return Vector2.zero;
            }
            //尽量小的移动，保证至少有一端对齐
            var finalOffset = Mathf.Abs(leftOrTopOffset) < Mathf.Abs(rightOrBottomOffset) ? leftOrTopOffset : rightOrBottomOffset;
            //横向和纵向的“正向”移动，对应于AnchorPosition的偏移量有些区别
            Vector2 offset = mIsVertical ? new Vector2(0, -finalOffset) : new Vector2(finalOffset, 0);
			if (!delayMove) {
				mCurtrans.anchoredPosition += offset;
			} else {
				//如果立即移动，此时UI还没刷新，却已经“预先”移动到了正确位置，反而会导致视觉上的不正确，所以延迟到下一次Update来完成移动
				//目前只有通过SetItemSize更改item的大小之后，再设法将其移动到视口中，才需要使用到延迟位移，避免视觉上的不一致
				OnUpdate -= DoDelayMove;
				OnUpdate += DoDelayMove;
				delayMoveOffset = offset;
			}
            mLayout.SetDeltaChanged();
            return offset;
        }

		private Vector2 delayMoveOffset = Vector2.zero; 
		private void DoDelayMove(){
			if (delayMoveOffset != Vector2.zero) {
				mCurtrans.anchoredPosition += delayMoveOffset;
				delayMoveOffset = Vector2.zero;
			}
		}

        /// <summary>
        /// 判断某个itemId对应的UI是否在视口内（计算后的理论结果，不一定已经有对应的UI了）
        /// add by liujunjie in 2018/11/15
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="forceReCalculate">是否强制重新计算，如果本帧内已经改变了布局相关的数据，则应该置为true</param>
        /// <returns></returns>
        public bool IsItemInViewport(int itemId,bool forceReCalculate = false)
        {
            var itemData = mItemDataList.Find(data => data.itemid == itemId);
            if (itemData == null) return false;
            if (forceReCalculate)
            {
                mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
                mHasCalculatedHeight = true;
				mLayout.CalculateAnchoredPosition (mIsReverse);	//GetMinViewport和GetMaxViewport会依赖这个函数的调用结果
            }
            return itemData.min > mLayout.GetMinViewport(mIsReverse) && itemData.max < mLayout.GetMaxViewport(mIsReverse); 
        }


        /// <summary>
        /// 设置某一个Item的大小，可以选择更改后立刻刷新UI，也可以选择是否要将数据的更改应用到对应的UI（如果有的话）身上
        /// 这个函数与之后对应的几个函数是为了实现特殊的UI效果而做的，其它地方谨慎调用
        /// add by liujunjie in 2018/11/16
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="size"></param>
        /// <param name="realChange"></param>
		public void SetItemSize(int itemId,float size,bool realChange = false)
        {
            var itemData = mItemDataList.Find(data => data.itemid == itemId);
            if (itemData == null) return;
            if (mIsVertical)
            {
                itemData.height = size;
            }
            else
            {
                itemData.width = size;
            }
            if (realChange)
            {
				//对同一个Item的多次更改，后面的更改会覆盖前面的更改
				OnUpdate -= DoDelayChangeRealSize;
				OnUpdate += DoDelayChangeRealSize;
				if(delayChangeSizeList == null){
					delayChangeSizeList = new List<DelaySizeChangeInfo> ();
				}
				//由于延迟到下一帧更新，那个时候，本次函数调用时需要更改的目标可能已经发生了变化，这里通过两种机制来避免这一点
				//1.保存一个ItemData，如果更新时发现这个ItemData已经没有对应的UI了，则不进行更改
				//（不保存RectTransfrom是因为此时ItemData对应的RectTransform不一定存在）
				//2.提供给外部一个清除下一帧更新操作的接口，如果外部确定本帧中需要消除之前的延迟操作，可以调用该函数进行清除
				delayChangeSizeList.Add (new DelaySizeChangeInfo (this, itemData, size));
            }
			SetDirty();
        }

		private class DelaySizeChangeInfo
		{
            public GameCommonList commonList;
			public ItemData itemData;
			public float size;
			public DelaySizeChangeInfo(GameCommonList commonList,ItemData itemData,float size){
                this.commonList = commonList;
				this.itemData = itemData;
				this.size = size;
			}
			public void ChangeSize(){
				//延迟到下一帧去更新，由于是根据ItemId更新，如果本帧中ItemId已经发生了变化，需要通过ClearNextUpdateAction
				var itemUI = commonList.mActiveUIList.Find(ui => ui.cid == itemData);
				if (itemUI == null) return;
				var rectTransform = itemUI.GetRectTransform();
				if (rectTransform == null) return;
				RectTransform.Axis axis = commonList.mIsVertical ? UnityEngine.RectTransform.Axis.Vertical : UnityEngine.RectTransform.Axis.Horizontal;
				rectTransform.SetSizeWithCurrentAnchors (axis, size);
			}
		}
		private List<DelaySizeChangeInfo> delayChangeSizeList = null;
		private void DoDelayChangeRealSize(){
			if (delayChangeSizeList != null && delayChangeSizeList.Count > 0) {
				for (int i = 0; i < delayChangeSizeList.Count; i++) {
					delayChangeSizeList [i].ChangeSize ();
				}
				delayChangeSizeList.Clear ();
			}
		}

		/// <summary>
		/// 由于当前延迟到下一帧的更改不会立即执行，如果本帧中之后的某些操作会需要清除之前的更改，则需要通过此函数来完成清除
		/// </summary>
		public void ClearNextUpdateAction(){
			OnUpdate = null;
			delayChangeSizeList = null;
			delayMoveOffset = Vector2.zero;
		}

        /// <summary>
        /// 获取某一个ItemId对应的size，获取到的size可以只是“虚假”的size，并不一定对应UI的真正大小
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="useOriginSize">是否获取“原本”的大小，如果是，那么会返回对应模板的大小</param>
        /// <returns></returns>
        public float GetItemSize(int itemId, bool useOriginSize = false)
        {
            float size = 0;
            var itemData = mItemDataList.Find(data => data.itemid == itemId);
            if(itemData != null)
            {
                if (useOriginSize)
                {
                    var rect = mTemplateAttr[itemData.typedid].rect;
                    size = mIsVertical ? rect.height : rect.width;
                }
                else
                {
                    size = mIsVertical ? itemData.height : itemData.width;
                }
            }
            else// Add 当对应的itemId没有条目时返回第一个模板的高度
            {
                var rect = mTemplateAttr[1].rect;
                size = mIsVertical ? rect.height : rect.width;
            }
            return size;
        }
        /// <summary>
        /// 获取某一个ItemId对应的宽度,与GetItemSize相对应
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="useOriginSize">是否获取“原本”的大小，如果是，那么会返回对应模板的大小</param>
        /// <returns></returns>
        public float GetItemWidth(int itemId, bool useOriginSize = false) 
        {
            float length = 0;
            var itemData = mItemDataList.Find(data => data.itemid == itemId);
            if (itemData != null)
            {
                if (useOriginSize)
                {
                    var rect = mTemplateAttr[itemData.typedid].rect;
                    length = mIsVertical ? rect.width : rect.height;
                }
                else
                {
                    length = mIsVertical ? itemData.width : itemData.height;
                }
            }
            else          // Add 当对应的itemId没有条目时返回第一个模板的宽度
            {
                var rect = mTemplateAttr[1].rect;
                length = mIsVertical ? rect.width : rect.height;
            }
            return length;
        }

        /// <summary>
        /// 强制刷新，即使现在已经EndUpdate了，也可以手动刷新，用于禁用EndUpdate后手动刷新
        /// </summary>
        public void ForceUpdate()
        {
            bool changedTimerId = false;
            if(mTimerID <= 0)
            {
                mTimerID = 1;
                changedTimerId = true;
            }
            SetDirty();
            Update();
            if (changedTimerId)
            {
                mTimerID = -1;
            }
        }

        /// <summary>
        /// 设置列表滚动是否失效
        /// </summary>
        /// <param name="enable">是否失效</param>
        public void SetScrollEnable(bool enable)
        {
            if (mScrollRect != null)
            {
                mScrollRect.gameObject.GetComponent<ScrollRect>().enabled = enable;
            }
        }

        public void SetScrollValue(float value)
        {
            if (mIsVertical)
            {
                if (mScrollRect != null)
                {
                    mScrollRect.verticalNormalizedPosition = value;
                }
            }
            else
            {
                if (mScrollRect != null)
                {
                    mScrollRect.horizontalNormalizedPosition = value;
                }
            }
        }

        public float GetScrollValue()
        {
            if (mIsVertical)
            {
                if (mScrollRect != null)
                {
                    return mScrollRect.verticalNormalizedPosition;
                }
            }
            else
            {
                if (mScrollRect != null)
                {
                    return mScrollRect.horizontalNormalizedPosition;
                }
            }

            return 0;
        }

        /// <summary>
        /// 设置列表的排列方向
        /// </summary>
        /// <param name="isVertical">是否纵向</param>
        public void SetIsVertical(bool isVertical)
        {
            mIsVertical = isVertical;
        }

        /// <summary>
        /// 开启计时器
        /// </summary>
        public void BeginUpdate()
        {
            if (mTimerID < 0)
            {
                mLastCheckPosTime = Time.unscaledTime; 
                mTimerID = TimerHandler.SetTimeout(this.Update, mCheckTime, true, false);
            }
        }

        /// <summary>
        /// 结束计时器
        /// </summary>
        public void EndUpdate()
        {
            if (mTimerID >= 0)
            {
                TimerHandler.RemoveTimeactionByID(mTimerID);
                mTimerID = -1;
            }
        }

        /// <summary>
        /// 获取条目的间隔，X方向
        /// </summary>
        /// <returns>间隔大小</returns>
        public float GetBorderSizeX()
        {
            return mBorderSizeX;
        }

        /// <summary>
        /// 设置条目间的间隔,X方向
        /// </summary>
        /// <param name="size">间隔大小</param>
        public void SetBorderSizeX(int size)
        {
            mBorderSizeX = size;
        }

        /// <summary>
        /// 获取条目的间隔，Y方向
        /// </summary>
        /// <returns>间隔大小</returns>
        public float GetBorderSizeY()
        {
            return mBorderSizeY;
        }

        /// <summary>
        /// 设置条目间的间隔,Y方向
        /// </summary>
        /// <param name="size">间隔大小</param>
        public void SetBorderSizeY(int size)
        {
            mBorderSizeY = size;
        }

        /// <summary>
        /// 设置分帧后，每帧刷新数量
        /// </summary>
        /// <param name="count">每帧刷新数量</param>
        public void SetFrameBindCount(int count)
        {
            mFrameBindCount = count;
        }


        public void SetCheckTime(float time)
        {
            if (time <= 0)
                return;
            mCheckTime = time;
        }

        /// <summary>
        /// 配合布局组设置布局元素
        /// </summary>
        /// <param name="isheight">是否是高度</param>
        public void SetElementPreferred(bool isheight)
        {
            LayoutElement le = mCurtrans.GetComponent<LayoutElement>();
            if (le == null)
            {
                return;
            }

            mLayout.CalculateAndAdjust(mItemDataList, mOneRowOrColCount, mBorderSizeX, mBorderSizeY, mIsReverse, out mTotalSize);
            if (isheight)
            {
                le.preferredHeight = mTotalSize;
            }
            else
            {
                le.preferredWidth = mTotalSize;
            }

            mLayout.SetDeltaChanged();
        }

        /// <summary>
        /// 设置自动对齐信息，如果自动对齐，那么每次移动停下来之后都会靠近对齐规则的位置
        /// </summary>
        /// <param name="auto">是否自动对齐</param>
        /// <param name="align">对齐类型0:居中,-1:靠左上（下界）,1:靠右下（上界）</param>
        /// <param name="OnAlignStop">停靠后的回调</param>
        public void SetAutoAlgin(bool auto, int align, UnityEngine.Events.UnityAction<int, int> OnAlignStop)
        {
            mAutoAlign = auto;
            mAlignMode = align;
            this.OnAlignStop = OnAlignStop;
        }
        #endregion

        public GameCommonList(string name, Transform father = null) : base(name, father)
        {
        }

        public GameCommonList(GameObject obj) : base(obj)
        {
        }
        
        public GameCommonList()
        {
        }
    }

    /// <summary>
    /// 导出到Lua中的类
    /// </summary>
    public class ICommonListItemData
    {
        public int typedid;
        public int itemid;
    }

    public class ItemData : ICommonListItemData
    {
        public float min, max;
        public float x, y;
        public float height;
        public float width;
        public bool alive = true, active = false;
    }

    /// <summary>
    /// 导出到lua中的类
    /// </summary>
    public class ICommonListItemUI
    {
        public UnityEngine.Events.UnityAction<int, int> _Bind, _Unbind;
        public UnityEngine.Events.UnityAction<ItemData> _BindAll;
        public string name;
        public int typeid;
        public ItemData cid;
    }

    public class ItemUI : ICommonListItemUI
    {
        private RectTransform curtrans;
        private GameObject curgo;
        private GameCommonList parent;
        public virtual void Init(Transform _root, GameCommonList parent)
        {
            curgo = _root.gameObject;
            this.curtrans = _root as RectTransform;
            name = curtrans.name;
            cid = null;
            this.parent = parent;
        }

        public void Bind(ItemData cid)
        {
            //这里可能是未经unbind的bind,所以要对以前的数据作适当的标记。
            if (this.cid != null)
            {
                this.cid.active = false;
            }

            //Debug.Log("Bind itemid = " + cid.itemid);
            this.cid = cid;
            curgo.name = string.Format("template_now_{0}_{1}", cid.typedid, cid.itemid - 1);
            cid.active = true;
            if (!curgo.activeSelf)
            {
                curgo.SetActive(true);
            }

            if (_Bind != null)
            {
                _Bind(parent.GetOriginItemId(cid.itemid), cid.typedid);
            }

            if (_BindAll != null)
            {
                // 刷新回调，回调lua层，参数类型为itemdata //
                ItemData data = new ItemData();
                data.min = cid.min;
                data.max = cid.max;
                data.x = cid.x;
                data.y = cid.y;
                data.height = cid.height;
                data.width = cid.width;
                data.alive = cid.alive;
                data.active = cid.active;
                data.typedid = cid.typedid;
                data.itemid = parent.GetOriginItemId(cid.itemid);
                _BindAll(cid);
            }
        }

        public void Unbind()
        {
            if (_Unbind != null)
            {
                _Unbind(parent.GetOriginItemId(cid.itemid), cid.typedid);
            }

            //Debug.Log("Unbind itemid = " + cid.itemid);
            curgo.name = string.Format("template_old_{0}", cid.typedid);
            cid.active = false;
            cid = null;

            // 这里消耗很大，所以不再调用 //
            //curgo.SetActive(false);
            Vector2 pos = curgo.GetComponent<RectTransform>().anchoredPosition;
            pos.y = GameCommonList.HIDE_POS_Y;
            curgo.GetComponent<RectTransform>().anchoredPosition = pos;
        }

        public void SetAnchoredPosition(Vector2 pos)
        {
            pos.x += this.curtrans.pivot.x * cid.width;
            pos.y -= (1 - this.curtrans.pivot.y) * cid.height;
            this.curtrans.anchoredPosition = pos;
        }
        public RectTransform GetRectTransform()
        {
            return curtrans;
        }
        public void DestroySelf()
        {
            if (curgo != null)
            {
                GameObject.DestroyImmediate(curgo);
            }
        }
    }

    public class Layout
    {
        public void SetLayout(RectTransform Curtrans, RectTransform Viewport)
        {
            this.mCurtrans = Curtrans;
            this.mViewport = Viewport;
            this.mLast = -1f;
        }

        protected RectTransform mCurtrans;
        protected RectTransform mViewport;
        protected float mLast;
        protected Vector3 mAnchoredPosition;
        //private Vector3 mLeftUpperPos = new Vector3();

        public virtual void CalculateAndAdjust(List<ItemData> list, int count, float borderSize, float borderSizeY, bool isReverse, out float totalsize)
        {
            if (isReverse)
            {
                CalculateItemReverse(list, count, borderSize, borderSizeY, out totalsize);
            }
            else
            {
                CalculateItem(list, count, borderSize, borderSizeY, out totalsize);
            }
        }

        protected virtual void CalculateItemReverse(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            totalsize = 0;
        }

        protected virtual void CalculateItem(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            totalsize = 0;
        }

        public virtual float GetViewport()
        {
            return 0;
        }

        public virtual float GetMinViewport(bool isReverse)
        {
            return 0;
        }

        public virtual float GetMaxViewport(bool isReverse)
        {
            return 0;
        }

        public virtual bool CheckDeltaChange(float lastTime, out bool isScrollEnd)
        {
            isScrollEnd = false;
            return false;
        }

        public void SetDeltaChanged()
        {
            mLast += GameCommonList.CHECK_DELTA + 1f;
        }

        public void CalLayoutData(ItemData data, RectTransform template, bool isVertical)
        {
            if (isVertical)
            {
                data.height = template.rect.height;
                data.width = template.rect.width;
            }
            else
            {
                data.width = template.rect.width;
                data.height = template.rect.height;
            }
        }

        public virtual void SetAnchoredPosition(ItemUI cur)
        {

        }

        // 把自己的左上点，转化到父节点的局部坐标系中 //
        public void CalculateAnchoredPosition(bool isReverse)
        {
            Rect cur = mCurtrans.rect;
            RectTransform parent = mCurtrans.parent as RectTransform;
            Rect par = parent.rect;

            //既然不管正反，“正方向”都是一致的（右方和下方为正方向），那完全可以统一进行处理
            //这样AnchoredPosition将有统一的意义：自己的左上角点相对于父物体的左上角点的相对位置
            //modify by liujunjie in 2018/11/22

            //先转换到世界坐标系，在转换到父节点的坐标系，在根节点scale为0的时候会出现问题（目前动画会设置根节点的scale）
            //所以改成直接转换到父节点坐标系

            Matrix4x4 currentToParent = Matrix4x4.TRS(mCurtrans.localPosition, mCurtrans.localRotation, mCurtrans.localScale);
            //自身leftTop在父节点坐标下的坐标
            Vector3 leftTopPointInParent = currentToParent.MultiplyPoint3x4(new Vector3(cur.xMin, cur.yMax, 0));
            //父节点自身leftTop的局部坐标系下坐标
            Vector3 parentLeftTop = new Vector3(par.xMin, par.yMax, 0);
            mAnchoredPosition = leftTopPointInParent - parentLeftTop;

			//mAnchoredPosition = mCurtrans.TransformPoint(cur.xMin, cur.yMax, 0);

   //         mLeftUpperPos.x = par.xMin;
   //         mLeftUpperPos.y = par.yMax;
   //         mAnchoredPosition = parent.InverseTransformPoint(mAnchoredPosition) - mLeftUpperPos;
        }
    }

    public class LayoutX : Layout
    {
        public override void CalculateAndAdjust(List<ItemData> list, int count, float borderSize, float borderSizeY, bool isReverse, out float totalsize)
        {
            base.CalculateAndAdjust(list, count, borderSize, borderSizeY, isReverse, out totalsize);
        }



        protected override void CalculateItem(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            if (list.Count == 0)
            {
                totalsize = 0;
                return;
            }

            float totalSize = 0;
            float begin = 0f;
            float maxWidth = 0f;

            for (int i = 0; i < list.Count; i++)
            {
                int col = (i / count) + 1;
                int row = 0;
                if (i < count)
                {
                    row = i + 1;
                }
                else
                {
                    row = (i % count) + 1;
                }

                var cur = list[i];
                cur.min = begin;
                cur.x = begin;
                if (row == 1)
                {
                    cur.y = 0f;
                }
                else
                {
                    cur.y = list[i - 1].y + list[i - 1].height + borderSizeY;
                }

                maxWidth = (cur.width > maxWidth) ? cur.width : maxWidth;
                totalSize = begin + maxWidth + borderSize;

                if ((i + 1) % count == 0)
                {
                    maxWidth = 0;
                    begin = totalSize;
                }

                cur.max = totalSize;
            }

            totalSize -= borderSize;
            mCurtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, totalSize);
            totalsize = totalSize;
        }

        protected override void CalculateItemReverse(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            if (list.Count == 0)
            {
                totalsize = 0;
                return;
            }

            float total = 0f;
            CalculateItem(list, count, borderSize, borderSizeY, out totalsize);
            total = totalsize;

            float begin = total - list[0].width;

            float maxWidth = 0f;

            for (int i = 0; i < list.Count; i++)
            {
                int col = (i / count) + 1;
                int row = 0;
                if (i < count)
                {
                    row = i + 1;
                }
                else
                {
                    row = (i % count) + 1;
                }

                var cur = list[i];
                cur.min = begin;
                cur.x = begin;

                if (row == 1)
                {
                    cur.y = 0f;
                }
                else
                {
                    cur.y = list[i - 1].y + list[i - 1].height + borderSizeY;
                }

                maxWidth = (cur.width > maxWidth) ? cur.width : maxWidth;
                cur.max = begin + maxWidth;

                if ((i + 1) % count == 0)
                {
                    begin = begin - maxWidth - borderSize;
                    maxWidth = 0;
                }

            }
        }

        public override float GetMaxViewport(bool isReverse)
        {
//            if (isReverse)
//            {
//                return mCurtrans.rect.width - mAnchoredPosition.x + mViewport.rect.width;
//            }
//            else
//            {
//                return -mAnchoredPosition.x + mViewport.rect.width;
//            }
			return -mAnchoredPosition.x + mViewport.rect.width;
        }

        public override float GetMinViewport(bool isReverse)
        {
//            if (isReverse)
//            {
//                return mCurtrans.rect.width - mAnchoredPosition.x;
//            }
//            else
//            {
//                return -mAnchoredPosition.x;
//            }
			return -mAnchoredPosition.x;
        }

        public override float GetViewport()
        {
            return mViewport.rect.width;
        }

        public override bool CheckDeltaChange(float lastTime, out bool isScrollEnd)
        {
            float delta = Math.Abs(mAnchoredPosition.x - mLast);
            float deltaTm = Time.unscaledTime - lastTime;
            mLast = mAnchoredPosition.y;
            if (deltaTm <= 0)
            { // 时间非常小，那速度趋向于无穷大了
                isScrollEnd = false;
                return true;
            }
            float speed = delta / deltaTm;
            bool isChange = (speed > GameCommonList.CHECK_DELTA_SPEED);
            isScrollEnd = !isChange;
            return isChange;
        }

        public override void SetAnchoredPosition(ItemUI cur)
        {
            cur.SetAnchoredPosition(new Vector2(cur.cid.x, -cur.cid.y));
        }
    }

    public class LayoutY : Layout
    {
        public override void CalculateAndAdjust(List<ItemData> list, int count, float borderSize, float borderSizeY, bool isReverse, out float totalsize)
        {
            base.CalculateAndAdjust(list, count, borderSize, borderSizeY, isReverse, out totalsize);
        }

        protected override void CalculateItemReverse(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            if (list.Count == 0)
            {
                totalsize = 0;
                return;
            }

            float total = 0f;
            CalculateItem(list, count, borderSize, borderSizeY, out totalsize);
            total = totalsize;

            float begin = total - list[0].height;
            float maxHeight = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                int row = (i / count) + 1;
                int col = 0;
                if (i < count)
                {
                    col = i + 1;
                }
                else
                {
                    col = (i % count) + 1;
                }

                var cur = list[i];
                cur.min = begin;
                cur.y = begin;

                if (col == 1)
                {
                    cur.x = 0f;
                }
                else
                {
                    cur.x = list[i - 1].x + list[i - 1].width + borderSize;
                }

                maxHeight = (cur.height > maxHeight) ? cur.height : maxHeight;
                cur.max = begin + maxHeight;

                if ((i + 1) % count == 0)
                {
                    begin = begin - maxHeight - borderSizeY;
                    maxHeight = 0;
                }

            }
        }

        protected override void CalculateItem(List<ItemData> list, int count, float borderSize, float borderSizeY, out float totalsize)
        {
            if (list.Count == 0)
            {
                totalsize = 0;
                return;
            }

            float totalSize = 0;
            float begin = 0f;
            float maxHeight = 0f;
            for (int i = 0; i < list.Count; i++)
            {
                int row = (i / count) + 1;
                int col = 0;
                if (i < count)
                {
                    col = i + 1;
                }
                else
                {
                    col = (i % count) + 1;
                }

                var cur = list[i];
                cur.min = begin;
                cur.y = begin;

                if (col == 1)
                {
                    cur.x = 0f;
                }
                else
                {
                    cur.x = list[i - 1].x + list[i - 1].width + borderSize;
                }

                maxHeight = (cur.height > maxHeight) ? cur.height : maxHeight;
                totalSize = begin + maxHeight + borderSizeY;

                if ((i + 1) % count == 0)
                {
                    maxHeight = 0;
                    begin = totalSize;
                }

                cur.max = totalSize;
            }

            totalSize -= borderSizeY;
            mCurtrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalSize);

            totalsize = totalSize;
        }

        public override float GetMaxViewport(bool isReverse)
        {
//            if (isReverse)
//            {
//                return mCurtrans.anchoredPosition.y + mCurtrans.rect.height + mViewport.rect.height;
//            }
//            else
//            {
//                return mAnchoredPosition.y + mViewport.rect.height;
//            }
			return mAnchoredPosition.y + mViewport.rect.height;
        }

        public override float GetMinViewport(bool isReverse)
        {
//            if (isReverse)
//            {
//                return mCurtrans.anchoredPosition.y + mCurtrans.rect.height;
//            }
//            else
//            {
//                return mAnchoredPosition.y;
//            }
			return mAnchoredPosition.y;
        }

        public override float GetViewport()
        {
            return mViewport.rect.height;
        }

        public override bool CheckDeltaChange(float lastTime, out bool isScrollEnd)
        {
            float delta = Math.Abs(mAnchoredPosition.y - mLast);
            float deltaTm = Time.unscaledTime - lastTime;
            mLast = mAnchoredPosition.y;
            if (deltaTm <= 0)
            { // 时间非常小，那速度趋向于无穷大了
                isScrollEnd = false;
                return true;
            }
            float speed = delta / deltaTm;
            bool isChange = (speed > GameCommonList.CHECK_DELTA_SPEED);
            isScrollEnd = !isChange;
            return isChange;
        }

        public override void SetAnchoredPosition(ItemUI cur)
        {
            cur.SetAnchoredPosition(new Vector2(cur.cid.x, -cur.cid.y));
        }
    }
    
}