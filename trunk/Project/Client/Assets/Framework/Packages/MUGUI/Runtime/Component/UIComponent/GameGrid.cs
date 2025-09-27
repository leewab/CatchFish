using System;
using System.Collections.Generic;
using Game.Core;
using Game.Core.Core;
using MUEngine;
using MUGame;
using MUGUI;
using UnityEngine;

namespace Game.UI
{
	
    /// <summary>
    /// 网格容器控件
    /// 可以向该容器中添加数个相同类型的子控件
    /// </summary>
    public class GameGrid : GameUIComponent
    {
        public const string TEMPLATE = "Template";
        public const string POOLOBJECT = "pool";

        private GameObject template;
        private Transform poolTransform;
        private UnityEngine.UI.LayoutGroup _uiTile;

        private MUEngine.MemoryPool<GameUIComponent> childPool = new MUEngine.MemoryPool<GameUIComponent>();
        private List<Transform> childList = new List<Transform>();
        private bool childListDirty = false;

        protected virtual Transform ContainerTransform
        {
            get { return _uiTile.transform; }
        }

        public bool ClearChildWhenShow { get; set; }

        public int ChildCount
        {
            get
            {
                if (childListDirty)
                {
                    childListDirty = false;
                    childList.Clear();
                    for (int i = 0; i < ContainerTransform.childCount; i++)
                    {
                        var t = ContainerTransform.GetChild(i);
                        if (t == poolTransform)
                            continue;
                        if (!t.gameObject.activeSelf)
                            continue;
                        childList.Add(t);
                    }
                }
                return childList.Count;
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            _uiTile = GetComponent<UnityEngine.UI.LayoutGroup>();
            if (ContainerTransform.childCount > 0)
            {
                Transform t = ContainerTransform.GetChild(0);
                if (t != null)
                {
                    if (t.name == TEMPLATE)
                    {
                        template = t.gameObject;
                        t.gameObject.SetActive(false);
                    }
                }
            }
            
            GameObject temp = new GameObject();
            temp.name = POOLOBJECT + "_" + gameObject.name;
            temp.SetActive(false);
            poolTransform = temp.transform;
            poolTransform.parent = ContainerTransform;
        }

        protected override void OnShow()
        {
	        base.OnShow();
            if (ClearChildWhenShow)
                Clear();
        }

        public override void Dispose()
        {
            base.Dispose();
            childPool.Dispose();
        }

        protected GameUIComponent DisposeChild(GameObject child)
        {
            GameUIComponent ui = child.ToGameUIComponent<GameUIComponent>();
            if (ui != null)
                ui.Dispose();
            childListDirty = true;
            if (childPool.Free(ui))
            {
                if (ui != null)
                    ui.Visible = false;
                else
                    child.SetActive(false);
                Vector3 sc = child.transform.localScale;
                Vector3 ps = child.transform.localPosition;
                child.transform.SetParent(poolTransform);
                child.transform.localScale = sc;
                child.transform.localPosition = ps;
                return ui;
            }
            GameObject.DestroyImmediate(child);
            return ui;
        }

        protected GameObject AddChild()
        {
            GameObject gameObj = null;
            GameUIComponent ui = childPool.Alloc();
            if (ui != null)
            {
                gameObj = ui.gameObject;
                Vector3 sc = ui.RectTransform.localScale;
                Vector3 ps = ui.RectTransform.anchoredPosition3D;
                ui.RectTransform.SetParent(ContainerTransform);
                ui.RectTransform.localScale = sc;
                ui.RectTransform.anchoredPosition3D = ps;
                ui.FromPool = true;
                ui.Visible = true;
            }

            if (gameObj == null)
            {
                gameObj = GOGUITools.AddChild(ContainerTransform.gameObject, template);
                gameObj.SetActive(true);
            }
            childListDirty = true;
            return gameObj;
        }

        public T Make<T>(int idx)
            where T : GameUIComponent, new()
        {
            Transform t = GetChildByIndex(idx);
            T res = Make<T>(t.gameObject);
            EventTriggerListener lis = EventTriggerListener.Get(res.gameObject);
            lis.parameter = res;
            return res;
        }

        public virtual T AddChild<T>() where T : GameUIComponent, new()
        {
            GameObject go = AddChild();
            T res = go.ToGameUIComponent() as T;
            if (res != null)
                return res;
            res = Make<T>(go);
            EventTriggerListener lis = EventTriggerListener.Get(go);
            lis.parameter = res;
            return res;
        }

        public T GetChild<T>(int idx)
           where T : GameUIComponent
        {
            Transform val = GetChildByIndex(idx);
            if (val != null)
            {
                return val.gameObject.ToGameUIComponent<T>();
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            while (ChildCount > 0)
            {
                RemoveChild(0);
            }
        }

        public virtual T RemoveChild<T>(int idx)
            where T : GameUIComponent
        {
            return RemoveChild(idx) as T;
        }

        private GameUIComponent RemoveChild(int idx)
        {
            Transform val = GetChildByIndex(idx);
            if (!val)
                return null;
            return DisposeChild(val.gameObject);
        }

        protected virtual Transform GetChildByIndex(int idx)
        {
            if (ChildCount > idx)
                return childList[idx];
            else
                return null;
        }

        public virtual void EnsureSize<T>(int count)
            where T : GameUIComponent, new()
        {
            int cur = ChildCount;
            if (cur < count)
            {
                int diff = count - cur;
                for (int i = 0; i < diff; i++)
                {
                    AddChild<T>();
                }
            }
            else
            {
                int diff = cur - count;
                for (int i = 0; i < diff; i++)
                    RemoveChild<T>(0);
            }
        }

        public GameUIComponent[] LEnsureSize(int count)
        {
            GameUIComponent[] array = new GameUIComponent[count];
            EnsureSize<GameUIComponent>(count);
            for (int i = 0; i < count; i++)
            {
                array[i]=GetChild<GameUIComponent>(i);
            }
            return array;
        }
    }
    
	/// <summary>
	/// 列表raw item回收池
	/// </summary>
	class GameGridRawItemPool
	{
		public GameGridRawItemPool( Transform recycle_parent, Transform tpl_root )
		{
			mRecycleParent = recycle_parent;
			mTplRoot = tpl_root;
		}
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init( Transform container_trans )
		{       
			BuildTemplate(container_trans);
		}
		public void SetOnCreateRawItemFunc(Action<GameUIComponent> oncreate_func)
		{
			mOnCreateRawItemFunc = oncreate_func;
		}
		public void SetOnDestroyRawItemFunc( Action<string> ondestroy_func )
		{
			mOnDestroyRawItemFunc = ondestroy_func;
		}
		public void Release()
		{
			//销毁所有
			foreach (KeyValuePair<string, ObjectCache<GameUIComponent>> pair in mRawItemRecyclePool)
			{
				do
				{
                    if (pair.Value.IsEmpty)
                    {
                        break;
                    }
					GameUIComponent ui = pair.Value.GetFromPool();					
					ReleaseUI(ui);
				}
				while(true);
			}
            mOnCreateRawItemFunc = null;
            mOnDestroyRawItemFunc = null;
		}
		/// <summary>
		/// 释放列表条
		/// 返回值:是否加入到池中
		/// </summary>
		/// <param name="tplName">Tpl name.</param>
		/// <param name="go">Go.</param>
		public void FreeItem( GameUIComponent ui )
		{
            //命名规则：tplname_XXX
            if (ui == null)
            {
                return;
            }
            string uid = (string)ui.GetCustomData("UID");
            int pos = uid.IndexOf("_UID_");
			string tplName = uid.Substring(0, pos);
			//放到回收池中
			ObjectCache<GameUIComponent> pool = this.FetchPool(tplName);            
			ui.gameObject.transform.SetParent(mRecycleParent, false);
			if (!pool.IsFull)
			{
				pool.AddToPool(ui);
			}
			else
			{
				if (mOnDestroyRawItemFunc != null)
				{
					mOnDestroyRawItemFunc(uid);
				}
				ReleaseUI(ui);
			}

		}
		/// <summary>
		/// 获取一个列表条
		/// </summary>
		/// <param name="tplName">Tpl name.</param>
		public GameUIComponent Allocitem( string tplName, Transform parent )
		{
			ObjectCache<GameUIComponent> pool = this.FetchPool(tplName);
			GameUIComponent ui = null;
            string uid = tplName + "_UID_" + NextID();
            if (pool.IsEmpty)
			{
				//如果池中是空的，创建个新的
				GameObject tpl = FindTemplate (tplName);
				if (tpl == null) 
				{
					return null;
				}
				GameObject newobj = null;
				if( parent != null )
				{
					newobj = GOGUITools.AddChild(parent.gameObject, tpl);
				}
				else
				{
					newobj = GOGUITools.AddChild (this.mRecycleParent.gameObject, tpl);
				}
                newobj.name = uid;
                ui = new GameUIComponent(newobj);
                //重新设置新的UID            
                ui.SetCustomData("UID", uid);
				if (mOnCreateRawItemFunc != null)
				{					
					mOnCreateRawItemFunc(ui);
				}
			}
			else
			{
				//池中存在，取出来设置新的parent
				ui = pool.GetFromPool();
                if (parent != null)
                {
                    ui.gameObject.transform.SetParent(parent, false);
                }
                else
                {
                    ui.gameObject.transform.SetParent(this.mRecycleParent, false);
                }
				
                //重新设置新的UID            
                ui.SetCustomData("UID", uid);
            }
            
            return ui;
		}
		public bool IsPoolEmpty( string tplname )
		{
			ObjectCache<GameUIComponent> pool = this.FetchPool(tplname);
			return pool.IsEmpty;
		}
		/// <summary>
		/// 搜索模板
		/// </summary>
		/// <returns>The template.</returns>
		/// <param name="tplname">Tplname.</param>
		public GameObject FindTemplate( string tplname )
		{
            if (mTplList == null)
            {
                return null;
            }
			for (int k = 0; k < mTplList.Length; ++k)
			{
				GameObject tpl = mTplList [k];
				if ( tpl != null && tpl.name == tplname)
				{
					return tpl;
				}
			}
			return null;
		}

		private void ReleaseUI( GameUIComponent ui )
		{
			GameObject go = ui.gameObject;
			ui.Dispose();
			if( go != null )
			{
				GameObject.Destroy(go);
			}
		}
		/// <summary>
		/// 注册模版列表
		/// </summary>
		private void BuildTemplate( Transform container_trans )
		{
            int min_idx = -1;
            float min_area = 99999.0f;

			List<GameObject> tplist = new List<GameObject> ();
			//添加到模版列表
			for (int k = 0; k < mTplRoot.childCount; ++k) 
			{
				Transform cur_child =  mTplRoot.GetChild (k);
                RectTransform rect_trans = cur_child.GetComponent<RectTransform>();
                Vector2 rect_size = rect_trans.rect.size;
                float area = rect_size.x * rect_size.y;
                if (min_area > area)
                {
                    min_area = area;
                    min_idx = k;
                }
				tplist.Add(cur_child.gameObject);
			}
            //估算池的容量
            if (min_idx < 0)
            {
                //最小20个
                mPoolSize = 20;
            }
            else
            {
                RectTransform rect_trans = container_trans.GetComponent<RectTransform>();
                Vector2 container_size = rect_trans.rect.size;
                float container_area = container_size.x * container_size.y;
                //估算池的尺寸，因为加载区域是显示区域的2倍，所以要 x 2
                mPoolSize = (int)(container_area * 2.0f / min_area);
                mPoolSize = mPoolSize < 20 ? 20 : mPoolSize;
            }
			mTplList = tplist.ToArray();
		}
       

	
		/// <summary>
		/// Fetchs the pool.45
		/// </summary>
		/// <returns>The pool.</returns>
		/// <param name="tplName">Tpl name.</param>
		private ObjectCache<GameUIComponent> FetchPool( string tplName )
		{
			ObjectCache<GameUIComponent> tpl_pool = null;
			if (!mRawItemRecyclePool.ContainsKey(tplName))
			{
				tpl_pool = new ObjectCache<GameUIComponent> (mPoolSize);
				mRawItemRecyclePool [tplName] = tpl_pool;
			}
			else
			{
				tpl_pool = mRawItemRecyclePool [tplName];
			}
			return tpl_pool;
		}
		/// <summary>
		/// 生成id
		/// </summary>
		/// <returns>The I.</returns>
		private int NextID()
		{
			return ++mItemIDSeed;
		}
		/// <summary>
		/// 用于回收的父节点
		/// </summary>
		private Transform mRecycleParent;
		/// <summary>
		/// 列表条模板
		/// </summary>
		private Transform mTplRoot;
		private GameObject[] mTplList;
		/// <summary>
		/// 回收池
		/// </summary>
		private Dictionary<string, ObjectCache<GameUIComponent>> mRawItemRecyclePool = new Dictionary<string, ObjectCache<GameUIComponent>>();
		private int mItemIDSeed = 0;
		private int mPoolSize = 20;
		private Action<GameUIComponent> mOnCreateRawItemFunc;
		private Action<string> mOnDestroyRawItemFunc;
	}
	/// <summary>
	/// 对象缓存
	/// </summary>
	class ObjectCache<T>
	{
		public bool IsEmpty
		{
			get
			{
				return mCache.Count == 0;
			}
		}
		public bool IsFull
		{
			get
			{
				return mCache.Count >= mMaxSize;
			}
		}
		public ObjectCache( int max_size )
		{
			mMaxSize = max_size;
		}
		public bool AddToPool( T obj )
		{
			if (IsFull)
			{
				return false;
			}
			mCache.Enqueue(obj);
			return true;
		}
		public T GetFromPool()
		{
			return mCache.Dequeue();
		}

		private int mMaxSize;
		private Queue<T> mCache = new Queue<T>();

	}



	class ListLogicItem
	{
		public int ID;
		public string Tpl;
        public GameUIComponent mRawItem;
	}
	/// <summary>
	/// ListView接口声明
	/// </summary>
	interface IListView
	{
		void Init(Transform container_trans, Transform recycle_trans, Transform tpl_root, IListViewLayout layout);

		void Release();
        /// <summary>
        /// 添加列表条
        /// </summary>
        /// <param name="tplname"></param>
        /// <returns></returns>
		int AddItem(string tplname);
        /// <summary>
        /// 添加列表条
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="tplname"></param>
        /// <returns></returns>
		int InsertItem(int idx, string tplname);
        /// <summary>
        /// 通过id删除列表条
        /// </summary>
        /// <param name="idx"></param>
		void RemoveItemByID(int idx);
        /// <summary>
        /// 获得列表条的对应索引
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetListItemIdx(ListViewLogicItem item);
        /// <summary>
        /// 清空
        /// </summary>
        void Clear();
        /// <summary>
        /// 每帧更新
        /// </summary>
		void Update();
        int GetListCount();
        void SetOnCreateRawItemFunc( Action<GameUIComponent> oncreate_func );
		void SetOnDestroyRawItemFunc( Action<string> ondestroy_func );
		void SetUpdateListItemFunc( Action<int, string> update_func );
		void SetDetachRawItemFunc(Action<int> detach_func);
        /// <summary>
        /// 请求创建列表条
        /// </summary>
        /// <param name="tplname"></param>
        /// <param name="callback"></param>
		void AskCreateItem(string tplname, Action<GameUIComponent> callback);
		void AddRawItemToListView (string rawitem_name);
		void AskReleaseItem(int ID, string rawitem_name);
		void AskUpdateListItem(int itemid, string raw_name);
        /// <summary>
        /// 获得文档尺寸
        /// </summary>
        /// <returns></returns>
        float GetDocSize();
        /// <summary>
        /// 获得当前滚动位置
        /// </summary>
        /// <returns></returns>
        float GetDocPosition();
        /// <summary>
        /// 设置当前滚动位置
        /// </summary>
        /// <param name="pos"></param>
        void SetDocPosition(float pos);
        /// <summary>
        /// 获得列表条模板的尺寸
        /// </summary>
        /// <param name="tplname"></param>
        /// <param name="item_size"></param>
        /// <returns></returns>
        Vector2 GetItemTemplateSize( string tplname );
        
	}

	
	class UIListViewImpl :IListView
	{
		public void Init(Transform container_trans, Transform recycle_trans, Transform tpl_root, IListViewLayout layout)
		{
			mContainerTrans = container_trans;

			mItemPool = new GameGridRawItemPool (recycle_trans, tpl_root);
			mItemPool.Init( container_trans.parent );
			//布局
			mLayout = layout;
			//列表项更新管理器
			mItemGraphicUpdateMgr = new ListItemGraphicUpdateMgr();
		}
		public void Release()
		{
			mItemPool.Release();
            mUpdateFunc = null;
            mOnDetachRawItemFunc = null;

        }
		public void Update()
		{
            if( IsDirty() )
            {
				//需要更新
                //列表条图形更新
                mItemGraphicUpdateMgr.UpdateListItemVisible(mLayout, this.mItemList);
                mDirty = false;
            }
			
		}
        private bool IsDirty()
        {
#if !GAME_EDITOR
            if (mDirty)
            {
                return true;
            }
            //如果滚动条发生移动，则也需要刷新
            Vector3 container_pos = mContainerTrans.localPosition;
            float dist = mLastContainerPos.DistanceTo(container_pos);
            if (dist > 2.0f)
            {
                mLastContainerPos = container_pos;
                return true;
            }
            else
            {
                return false;
            }
#else
	        return false;
#endif
        }
		public int AddItem(string tplname)
		{
			GameObject tpl = mItemPool.FindTemplate(tplname);
			if (tpl == null)
			{
				throw new Exception ( "Template Not Found ! " + tplname );
			}
			int new_id = NextID();
			Rect r = tpl.GetComponent<RectTransform>().rect;
			//创建列表条
			ListViewLogicItem new_item = new ListViewLogicItem (new_id, tplname, r.size, this);
			mItemList.Add(new_item);
			//更新其位置
			mLayout.DoLayoutAppend(mItemList);
            mDirty = true;

			return new_id;	
		}
		public int InsertItem( int idx, string tplname )
		{
			GameObject tpl = mItemPool.FindTemplate(tplname);
			if (tpl == null)
			{
				throw new Exception ( "Template Not Found ! " + tplname );
			}
			int new_id = NextID();
			Rect r = tpl.GetComponent<RectTransform>().rect;
			//创建列表条
			ListViewLogicItem new_item = new ListViewLogicItem (new_id, tplname, r.size, this);
			mItemList.Insert (idx, new_item);
			mLayout.DoLayout (mItemList);
			mItemGraphicUpdateMgr.CalcNeedRefreshItems (null);
			mDirty = true;
			return new_id;
		}
        /// <summary>
        /// 获得列表条所在索引
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int GetListItemIdx( ListViewLogicItem item )
        {
            return mItemList.IndexOf(item);
        }
		public void RemoveItemByID(int id)
		{
			int cnt = mItemList.Count;
			for (int k = 0; k < cnt; ++k)
			{
				ListViewLogicItem item = mItemList [k];
				if (item.ID == id)
				{
					RemoveItem(k);                    
					return;
				}
			}
		}
		private void RemoveItem( int idx )
		{
			ListViewLogicItem item = mItemList [idx];
			item.Release();
			mItemList.RemoveAt(idx);
			//更新每个列表项的位置
			mLayout.DoLayout(mItemList);
			//删除一个列表项之后，周围正在显示的列表项也会跟着发生变化，但是这里并没有删除强制刷新列表中已经被release的项，在每帧更新的时候会剔除被release的列表条
			mItemGraphicUpdateMgr.CalcNeedRefreshItems(item);
            mDirty = true;
            //立即刷新
            this.Update();
		}
		/// <summary>
		/// 清空所有列表内容
		/// </summary>
		public void Clear()
		{
			for (int k = 0; k < mItemList.Count; ++k)
			{
				ListViewLogicItem item = mItemList [k];
				item.Release();
			}
			mItemList.Clear();
            mDirty = true;
		}
        public int GetListCount()
        {
            return mItemList.Count;
        }
		public void SetOnCreateRawItemFunc( Action<GameUIComponent> oncreate_func )
		{
			mItemPool.SetOnCreateRawItemFunc(oncreate_func);
		}
		public void SetOnDestroyRawItemFunc( Action<string> ondestroy_func )
		{
			mItemPool.SetOnDestroyRawItemFunc(ondestroy_func);
		}
		public void SetUpdateListItemFunc( Action<int, string> update_func )
		{
			mUpdateFunc = update_func;
		}
		public void SetDetachRawItemFunc(Action<int> detach_func)
		{
			mOnDetachRawItemFunc = detach_func;
		}
		/// <summary>
		/// 请求创建列表条资源
		/// </summary>
		/// <param name="tplname">Tplname.</param>
		/// <param name="callback">Callback.</param>
		public void AskCreateItem(string tplname, Action<GameUIComponent> callback)
		{
			//如果池里有缓存的列表条，就直接使用，否则就需要走创建流程
			bool is_empty = mItemPool.IsPoolEmpty(tplname);
			if (!is_empty )
			{
				CreateItemImpl(tplname, callback);
			}
			else
			{
                TaskMod task_mod = TaskMod.Instance;
                MUEngine.TaskArgs args = task_mod.GetTempArgs();
                args.AddArg(tplname);
                args.AddArg(callback);
				TaskMod.Instance.CreateTask( CreateItemImpl_TaskFunc, args );
			}

		}
        private void CreateItemImpl_TaskFunc(object[] args)
        {
            string tplname = args[0] as string;
            Action<GameUIComponent> callback = args[1] as Action<GameUIComponent>;
            CreateItemImpl(tplname, callback);
        }
		private void CreateItemImpl(string tplname, Action<GameUIComponent> callback)
		{
			GameUIComponent ui = mItemPool.Allocitem(tplname, null);
			mRawItemList.Add(ui);
			if( callback != null )
			{
				callback(ui);
			}
		}
		public void AskReleaseItem(int logic_id, string rawitem_uid)
		{
			GameUIComponent ui = null;
            bool found = false;
			for (int k = 0; k < mRawItemList.Count; ++k)
			{
				ui = mRawItemList [k];
                string uid = ui.GetCustomData("UID") as string;
				if (uid == rawitem_uid)
				{
					mRawItemList.RemoveAt(k);
                    found = true;
					break;
				}
			}
            if (!found)
            {               
                return;
            }

			//回收列表条
			mItemPool.FreeItem(ui);

			//通知解除连接关系
			if (mOnDetachRawItemFunc != null)
			{
				mOnDetachRawItemFunc(logic_id);
			}
		}
		public void AddRawItemToListView(string rawitem_name)
		{
			for (int k = 0; k < mRawItemList.Count; ++k)
			{
				GameUIComponent ui = mRawItemList [k];
                string uid = ui.GetCustomData("UID") as string;
				if (uid == rawitem_name)
				{
					ui.gameObject.transform.SetParent (this.mContainerTrans, false);
                    mDirty = true;
					break;
				}
			}
		}
		/// <summary>
		/// 更新列表条
		/// </summary>
		/// <param name="itemid">Itemid.</param>
		/// <param name="raw_name">Raw name.</param>
		public void AskUpdateListItem(int itemid, string raw_name)
		{
			if (mUpdateFunc != null)
			{
				mUpdateFunc(itemid, raw_name);
				//更新列表条可能会直接导致列表条尺寸发生改变
			}
		}
        public float GetDocSize()
        {
            return mLayout.GetDocSize();           
        }
        public float GetDocPosition()
        {          
            return mLayout.GetDocPosition();
        }
        public void SetDocPosition( float pos )
        {          
            mLayout.SetDocPosition(pos);
        }
        
		private int NextID()
		{
			return sIDSeed++;
		}
        public Vector2 GetItemTemplateSize( string tplname )
        {
            GameObject tpl = mItemPool.FindTemplate(tplname);
            if (tpl == null)
            {
                throw new Exception("Template Not Found ! " + tplname);
            }
            Rect r = tpl.GetComponent<RectTransform>().rect;
            return r.size;
        }
	
		/// <summary>
		/// 容器
		/// </summary>
		private Transform mContainerTrans;
		/// <summary>
		/// 列表条
		/// </summary>
		private List<ListViewLogicItem> mItemList = new List<ListViewLogicItem>();
		private List<GameUIComponent> mRawItemList = new List<GameUIComponent>();
		private int sIDSeed = 0;
		/// <summary>
		/// 列表条更新函数
		/// </summary>
		private Action<int, string> mUpdateFunc;
		private Action<int> mOnDetachRawItemFunc;
		/// <summary>
		/// 列表条回收池
		/// </summary>
		private GameGridRawItemPool mItemPool;
        /// <summary>
        /// 布局
        /// </summary>
		private IListViewLayout mLayout;
        /// <summary>
        /// 数据更新
        /// </summary>
		private ListItemGraphicUpdateMgr mItemGraphicUpdateMgr;
        
        private Vector3 mLastContainerPos = Vector3.zero;
        private bool mDirty = true;
	}
	class ListItemGraphicUpdateMgr
	{
		public ListItemGraphicUpdateMgr()
		{}
		public void CalcNeedRefreshItems( ListViewLogicItem exclude_item )
		{
			for (int k = 0; k < mLastFrameShow.Count; ++k)
			{
				ListViewLogicItem item = mLastFrameShow [k];
				if (item != exclude_item)
				{
					//如果没有加入到强制刷新列表，则添加
					if (mNeedRefreshItems.IndexOf (item) < 0) 
					{
						mNeedRefreshItems.Add (item);
					}
				}
			}

		}

		public void UpdateListItemVisible( IListViewLayout layout, List<ListViewLogicItem> itemlist )
		{
			//计算这一帧需要显示的
			mThisFrameShow.Clear();
			layout.CalcShowItems(itemlist, ref mThisFrameShow);
			//计算这一帧应该显示和隐藏的，执行显隐
			mToShow.Clear();
			mToHide.Clear();
			mToRefresh.Clear();
			Util.CollectionMinus(mThisFrameShow, mLastFrameShow, ref mToShow);
			Util.CollectionMinus(mLastFrameShow, mThisFrameShow, ref mToHide);
			//计算需要额外强制刷新的列表项
			Util.CollectionMinus(mNeedRefreshItems, mToShow, ref mToRefresh);
			mNeedRefreshItems.Clear();
			Util.CollectionMinus(mToRefresh, mToHide, ref mNeedRefreshItems);

			for (int k = 0; k < mNeedRefreshItems.Count; ++k)
			{
				ListViewLogicItem item = mNeedRefreshItems [k];
				//如果发现强制刷新的项已经被卸载，则不再更新位置
				if (!item.IsReleased) 
				{
					item.ForceRefresh ();
				}
			}
			mNeedRefreshItems.Clear();

			for (int k = 0; k < mToHide.Count; ++k)
			{
				ListViewLogicItem item = mToHide [k];
				item.SetVisible(false);
			}
			for (int k = 0; k < mToShow.Count; ++k)
			{
				ListViewLogicItem item = mToShow [k];
				item.SetVisible(true);
			}

			//交换这一帧和上一帧可见列表的引用
			List<ListViewLogicItem> tmp = mLastFrameShow;
			mLastFrameShow = mThisFrameShow;
			mThisFrameShow = tmp;
		}
		private List<ListViewLogicItem> mToShow = new List<ListViewLogicItem>();
		private List<ListViewLogicItem> mToHide = new List<ListViewLogicItem>();
		private List<ListViewLogicItem> mToRefresh = new List<ListViewLogicItem>();
		private List<ListViewLogicItem> mThisFrameShow = new List<ListViewLogicItem>();
		private List<ListViewLogicItem> mLastFrameShow = new List<ListViewLogicItem>();
		private List<ListViewLogicItem> mNeedRefreshItems = new List<ListViewLogicItem> ();
	}
	/// <summary>
	/// 列表布局接口
	/// </summary>
	abstract class IListViewLayout
	{
        public abstract bool IsVert { get; }
        public abstract void DoLayout(List<ListViewLogicItem> items);
        public abstract void DoLayoutAppend(List<ListViewLogicItem> items);
        public abstract void CalcShowItems(List<ListViewLogicItem> items, ref List<ListViewLogicItem> visible_items);
        public abstract float GetDocSize();
        public abstract float GetDocPosition();
        public abstract void SetDocPosition(float pos);
        /// <summary>
        /// 获得文档尺寸
        /// </summary>
        /// <param name="container_trans"></param>
        /// <param name="is_vert"></param>
        /// <returns></returns>
        public static float GetDocSize( Transform container_trans, Transform vp_trans, bool is_vert )
        {     
            Rect container_rect = container_trans.GetComponent<RectTransform>().rect;
            Rect vp_rect = vp_trans.GetComponent<RectTransform>().rect;
            if (is_vert)
            {
                float doc_size = container_rect.height - vp_rect.height;
                doc_size = doc_size > 0 ? doc_size : 0;
                return doc_size;
            }
            else
            {
                float doc_size = container_rect.width - vp_rect.width;
                doc_size = doc_size > 0 ? doc_size : 0;
                return doc_size;
            }
        }
        /// <summary>
        /// 获得文档位置
        /// </summary>
        /// <param name="container_trans"></param>
        /// <param name="vp_trans"></param>
        /// <param name="is_vert"></param>
        /// <returns></returns>
        public static float GetDocPosition( Transform container_trans, bool is_vert )
        {         
            Vector2 container_tl = container_trans.GetComponent<RectTransform>().anchoredPosition;
            if (is_vert)
            {
                //坐标向上为正，向下为负
                return container_tl.y;
            }
            else
            {
                //坐标向左为负，向右为正
                return -container_tl.x;
            }         
        }
        /// <summary>
        /// 设置文档位置
        /// </summary>
        /// <param n
        /// ame="container_trans"></param>
        /// <param name="vp_trans"></param>
        /// <param name="is_vert"></param>
        public static void SetDocPosition( Transform container_trans, float pos, bool is_vert )
        {
            RectTransform rect_trans = container_trans.GetComponent<RectTransform>();
            Vector2 rect_pos = rect_trans.anchoredPosition;
            if (is_vert)
            {                             
                rect_pos.y = pos;                
            }
            else
            {
                rect_pos.x = -pos;              
            }
            rect_trans.anchoredPosition = rect_pos;
        }
		/// <summary>
		/// 计算视口
		/// </summary>
		/// <param name="is_vert">If set to <c>true</c> is vert.</param>
		/// <param name="vp">Vp.</param>
		public static void CalcViewPort( Transform container_trans, Transform vp_trans, bool is_vert, ref Vector2 vp )
		{
			RectTransform trans = container_trans.GetComponent<RectTransform>();

			Rect container_rect = container_trans.GetComponent<RectTransform>().rect;
			Rect scroll_rect = vp_trans.GetComponent<RectTransform>().rect;
			Vector2 container_tl = trans.anchoredPosition;


			float expand_ratio = 0.5f;
			if (is_vert)
			{
				//坐标向上为正，向下为负
				float vp_height = scroll_rect.size.y;
				float begin_pos = container_tl.y;
				//begin 比 end要大
				begin_pos = -begin_pos;
				//向下为负，因此要减去高度
				float end_pos = begin_pos - vp_height;
				//扩展一下范围
				begin_pos += vp_height * expand_ratio;
				begin_pos = begin_pos > 0 ? 0 : begin_pos;
				end_pos -= vp_height * expand_ratio;
				vp.x = begin_pos;
				vp.y = end_pos;
			}
			else
			{
				float vp_width = scroll_rect.size.x;
				//begin_pos<0，说明视口在往右移动
				float begin_pos = container_tl.x;
				begin_pos = -begin_pos;
				float end_pos = begin_pos + vp_width;
				//扩展一下范围
				begin_pos -= vp_width * expand_ratio;
				begin_pos = begin_pos < 0 ? 0 : begin_pos;
				end_pos += vp_width * expand_ratio;
				vp.x = begin_pos;
				vp.y = end_pos;
			}
		}
	}
	class ListViewLayout :IListViewLayout
	{
        public override bool IsVert
        {
            get
            {
                return mIsVertical;
            }
        }
	   	public ListViewLayout( Transform container_trans, Transform scroll_trans, bool isvert )
		{
			mContainerTrans = container_trans;
			mViewportTrans = scroll_trans;
			mIsVertical = isvert;
		}
		/// <summary>
		/// 重新计算布局
		/// </summary>
		/// <param name="items">Items.</param>
		public override void DoLayout( List<ListViewLogicItem> items )
		{
			RectTransform rect_trans = this.mContainerTrans.GetComponent<RectTransform>();
			if (mIsVertical)
			{
				float height = DoLayout_Vert(items);
				rect_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
			else
			{
				float width = DoLayout_Horz(items);
				rect_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}
		private float DoLayout_Vert( List<ListViewLogicItem> items )
		{
			float begin_pos = 0.0f;
			float total_height = 0.0f;
			for (int k = 0; k < items.Count; ++k)
			{
				ListViewLogicItem cur_item = items [k];
				cur_item.Pos = new Vector2 (0, begin_pos);
				begin_pos -= cur_item.Size.y;
				total_height += cur_item.Size.y;
			}
			return total_height;

		}
		private float DoLayout_Horz( List<ListViewLogicItem> items)
		{
			float begin_pos = 0.0f;
			float total_width = 0.0f;
			for (int k = 0; k < items.Count; ++k)
			{
				ListViewLogicItem cur_item = items [k];
				cur_item.Pos = new Vector2 (begin_pos, 0);
				begin_pos += cur_item.Size.x;
				total_width += cur_item.Size.x;
			}
			return total_width;
		}
		/// <summary>
		/// 重新计算布局，只计算最后一个
		/// </summary>
		/// <param name="items">Items.</param>
		public override void DoLayoutAppend( List<ListViewLogicItem> items)
		{
			if (mIsVertical)
			{
				float size = DoLayoutAppend_Vert(items);
				RectTransform rect = mContainerTrans.GetComponent<RectTransform>();
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			}
			else
			{
				float size = DoLayoutAppend_Horz(items);
				RectTransform rect = mContainerTrans.GetComponent<RectTransform>();
				rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
			}
		}
		private float DoLayoutAppend_Vert(List<ListViewLogicItem> items)
		{
			if (items.Count == 0)
			{
				return 0.0f;
			}
			float begin_pos = 0.0f;
			for (int k = 0; k < items.Count-1; ++k)
			{
				ListViewLogicItem cur_item = items [k];
				begin_pos -= cur_item.Size.y;
			}
			ListViewLogicItem item = items [items.Count - 1];
			item.Pos = new Vector2 (0.0f, begin_pos);
			//负数转为正数，返回总高度
			return -(begin_pos - item.Size.y);
		}
		private float DoLayoutAppend_Horz( List<ListViewLogicItem> items)
		{
			if (items.Count == 0)
			{
				return 0.0f;
			}
			float begin_pos = 0.0f;
			for (int k = 0; k < items.Count-1; ++k)
			{
				ListViewLogicItem cur_item = items [k];
				begin_pos += cur_item.Size.x;
			}
			ListViewLogicItem item = items [items.Count - 1];
			item.Pos = new Vector2 (begin_pos, 0.0f);
			//附属转为正数，返回总宽度
			return begin_pos + item.Size.x;
		}

		public override void CalcShowItems(List<ListViewLogicItem> items, ref List<ListViewLogicItem> visible_items)
		{
			Vector2 vp = Vector2.zero;
			//计算视口
			CalcViewPort(mContainerTrans, mViewportTrans, mIsVertical, ref vp);
			//计算可见列表条
			CalcToShowItems(vp, mIsVertical, items, ref visible_items);
		}

		/// <summary>
		/// vp指的是横向或者纵向上的视口范围
		/// </summary>
		/// <param name="vp">Vp.</param>
		/// <param name="is_vert">If set to <c>true</c> is vert.</param>
		/// <param name="items">Items.</param>
		private void CalcToShowItems( Vector2 vp, bool is_vert, List<ListViewLogicItem> itemlist, ref List<ListViewLogicItem> visible_items )
		{
			if (is_vert)
			{
				for (int k = 0; k < itemlist.Count; ++k)
				{
					ListViewLogicItem item = itemlist [k];
					Vector2 pos = item.Pos;
					Vector2 size = item.Size;
					float item_top = pos.y;
					float item_bottom = item_top - size.y;
					float vp_top = vp.x;
					float vp_bottom = vp.y;
					if (!( vp_top < item_bottom || vp_bottom > item_top ))
					{
						//在视口范围内
						visible_items.Add(item);
					}
				}
			}
			else
			{
				for (int k = 0; k < itemlist.Count; ++k)
				{
					ListViewLogicItem item = itemlist [k];
					Vector2 pos = item.Pos;
					Vector2 size = item.Size;
					float item_left = pos.x;
					float item_right = item_left + size.x;
					float vp_left = vp.x;
					float vp_right = vp.y;
					if (!( vp_left > item_right || vp_right < item_left ))
					{
						//在视口范围内
						visible_items.Add(item);
					}
				}
			}
		}
        public override float GetDocSize()
        {
            return IListViewLayout.GetDocSize(mContainerTrans, mViewportTrans, mIsVertical);          
        }
        public override float GetDocPosition()
        {
            return IListViewLayout.GetDocPosition(mContainerTrans, mIsVertical);
        }
        public override void SetDocPosition(float pos)
        {
            IListViewLayout.SetDocPosition(mContainerTrans, pos, mIsVertical);
        }
        private Transform mContainerTrans;
		private Transform mViewportTrans;
		private bool mIsVertical;
	}
	/// <summary>
	/// Grid layout.
	/// </summary>
	class UIGridLayout :IListViewLayout
	{
        public override bool IsVert
        {
            get
            {
                return mIsVert;
            }
        }
        public UIGridLayout(Transform container_trans, Transform scroll_trans, Vector2 item_size, bool isvert, int cntinline)
		{
			mContainerTrans = container_trans;
			mScrollTrans = scroll_trans;
			mIsVert = isvert;
			mCntInLine = cntinline;
			mItemSize = item_size;
		}
		public override void DoLayout(List<ListViewLogicItem> items)
		{
			RectTransform container_rect = mContainerTrans.GetComponent<RectTransform>();
			if (mIsVert)
			{
				float height = DoLayout_Vert(items);
				container_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
			else
			{
				float width = DoLayout_Horz(items);
				container_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}
		private float DoLayout_Vert( List<ListViewLogicItem> items )
		{
			for (int k = 0; k < items.Count; ++k)
			{
				ListViewLogicItem item = items [k];
				int col = k % mCntInLine;
				int row = k / mCntInLine;
				item.Pos = new Vector2 ( col * mItemSize.x, -row * mItemSize.y );
			}
			return mItemSize.y * (int)Math.Ceiling(items.Count / (float)mCntInLine);
		}
		private float DoLayout_Horz( List<ListViewLogicItem> items )
		{
			for (int k = 0; k < items.Count; ++k)
			{
				ListViewLogicItem item = items [k];
				int col = k / mCntInLine;
				int row = k % mCntInLine;
				item.Pos = new Vector2 (col * mItemSize.x, -row * mItemSize.y);
			}
			return mItemSize.x * (int)Math.Ceiling(items.Count / (float)mCntInLine);
		}
		public override void DoLayoutAppend(List<ListViewLogicItem> items)
		{
			RectTransform container_rect = mContainerTrans.GetComponent<RectTransform>();
			if (mIsVert)
			{
				float height = DoLayoutAppend_Vert(items);
				container_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
			}
			else
			{
				float width = DoLayoutAppend_Horz(items);
				container_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
		}
		private float DoLayoutAppend_Vert( List<ListViewLogicItem> items )
		{
			if (items.Count == 0)
			{
				return 0.0f;
			}
			int last_idx = items.Count - 1;
			ListViewLogicItem item = items [last_idx];
			int row = last_idx / mCntInLine;
			int col = last_idx % mCntInLine;
			item.Pos = new Vector2 (col * mItemSize.x, -row * mItemSize.y);
			return (mItemSize.y * (int)Math.Ceiling(items.Count / (float)mCntInLine)) + item.Size.y - mItemSize.y;
		}
		private float DoLayoutAppend_Horz( List<ListViewLogicItem> items )
		{
			if (items.Count == 0)
			{
				return 0.0f;
			}
			int last_idx = items.Count - 1;
			ListViewLogicItem item = items [last_idx];
			int col = last_idx / mCntInLine;
			int row = last_idx % mCntInLine;
			item.Pos = new Vector2 (col * mItemSize.x, -row * mItemSize.y);
			return (mItemSize.x * (int)Math.Ceiling(items.Count / (float)mCntInLine)) + item.Size.x - mItemSize.x;
		}
		public override void CalcShowItems(List<ListViewLogicItem> items, ref List<ListViewLogicItem> visible_items)
		{
			Vector2 vp = Vector2.zero;
			//计算视口
			CalcViewPort(mContainerTrans, mScrollTrans, mIsVert, ref vp);
			CalcToShowItems(vp, mIsVert, items, ref visible_items);
		}
		/// <summary>
		/// 计算可见列表项
		/// </summary>
		/// <param name="vp">Vp.</param>
		/// <param name="is_vert">If set to <c>true</c> is vert.</param>
		/// <param name="itemlist">Itemlist.</param>
		/// <param name="visible_items">Visible items.</param>
		private void CalcToShowItems(Vector2 vp, bool is_vert, List<ListViewLogicItem> itemlist, ref List<ListViewLogicItem> visible_items)
		{
			if (is_vert)
			{
				int min_row = -(int)(vp.x / mItemSize.y);
				int max_row = -(int)Math.Ceiling( vp.y / (float)mItemSize.y);

				int min_idx = min_row * mCntInLine;
				int max_idx = max_row * mCntInLine;
				max_idx = max_idx >= itemlist.Count ? itemlist.Count - 1 : max_idx;
				for (int k = min_idx; k <= max_idx; ++k)
				{
					visible_items.Add(itemlist[k]);
				}
			}
			else
			{
				int min_col = (int)( vp.x / mItemSize.x );
				int max_col = (int)Math.Ceiling(vp.y / (float)mItemSize.x);

				int min_idx = min_col * mCntInLine;
				int max_idx = max_col * mCntInLine;
				max_idx = max_idx >= itemlist.Count ? itemlist.Count - 1 : max_idx;
				for (int k = min_idx; k <= max_idx; ++k)
				{
					visible_items.Add(itemlist[k]);
				}
			}
		}
        public override float GetDocSize()
        {
            return IListViewLayout.GetDocSize(mContainerTrans, mScrollTrans, mIsVert);
        }
        public override float GetDocPosition()
        {
            return IListViewLayout.GetDocPosition(mContainerTrans, mIsVert);
        }
        public override void SetDocPosition(float pos)
        {
            IListViewLayout.SetDocPosition(mContainerTrans, pos, mIsVert);
        }
        private Transform mContainerTrans;
		private Transform mScrollTrans;
		private bool mIsVert;
		private Vector2 mItemSize;
		private int mCntInLine;
	}
	class ListViewLogicItem
	{
		public int ID
		{
			get
			{
				
				return mID;
			}
		}
		public string Tpl
		{
			get
			{
				return mTpl;
			}
		}

		public Vector2 Size
		{
			get
			{
				return mSize;
			}
		}
		public Vector2 Pos
		{
			get
			{
				return mPos;
			}
			set
			{
				mPos = value;
			}
		}
		public bool IsReleased
		{
			get
			{
				return mReleased;
			}
		}
		public ListViewLogicItem( int id, string tpl, Vector2 size, IListView listview )
		{
			mID = id;
			mTpl = tpl;
			mSize = size;
			mListView = listview;
		}
		public void Release()
		{
			mReleased = true;
			mShow = false;
			DoDestroy();
		}
		public void SetVisible( bool show )
		{
			if (mShow == show)
			{
				return;
			}
			mShow = show;
			if (show)
			{
				//启动创建流程
				BeginCreate();
			}
			else
			{
				//启动销毁流程
				DoDestroy();
			}
		}
        /// <summary>
        /// 刷新
        /// </summary>
		public void ForceRefresh()
		{
			if (mRawItem != null) 
			{
				Vector2 pos = this.Pos;
				mRawItem.gameObject.transform.localPosition = new Vector3 (pos.x, pos.y, 0.0f);
                int idx = mListView.GetListItemIdx(this);
                mRawItem.gameObject.name = mTpl + "_" + (idx + 1);            
			}
		}
		private void BeginCreate()
		{
            if (mCreating)
            {
                return;
            }
            mCreating = true;
			mListView.AskCreateItem(mTpl, this.OnCreateListItem);
		}
		private void OnCreateListItem(GameUIComponent raw_item)
		{
            mCreating = false;
			mRawItem = raw_item;

			if (mShow)
			{
                //当前正在显示这个列表条，填充信息                
                TaskMod.Instance.CreateTask(this.OnUpdateListItem);                
			}
			else
			{
				//当前已经不再显示这个列表条了，直接销毁
				DoDestroy();
			}
		}
		private void OnUpdateListItem()
		{
			if (mShow)
			{
                //将列表条加入到显示列表中
                string uid = mRawItem.GetCustomData("UID") as string;
				mListView.AddRawItemToListView(uid);
				ForceRefresh();
				mListView.AskUpdateListItem( mID, uid);
			}
			else
			{
				DoDestroy();
			}
		}
		private void DoDestroy()
		{
			if (mRawItem == null)
			{
				return;
			}
            string uid = mRawItem.GetCustomData("UID") as string;
            mListView.AskReleaseItem( mID, uid);
			mRawItem = null;
		}

		private int mID;

		/// <summary>
		/// 列表条尺寸
		/// </summary>
		private Vector2 mSize;
		/// <summary>
		/// 列表条位置
		/// </summary>
		private Vector2 mPos;
		/// <summary>
		/// 模板名
		/// </summary>
		private string mTpl;
		private bool mShow = false;
		private bool mReleased = false;
		private IListView mListView;
		private GameUIComponent mRawItem;
        private bool mCreating = false;


	}

}