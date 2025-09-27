using System;
using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
	/// 列表
	/// </summary>
	public class GameGridV2 : GameUIComponent
	{
		public const string TEMPLATE = "Template";
		public const string POOLOBJECT = "pool";

		public bool ClearChildWhenShow { get; set; }

		public int ChildCount
		{
			get
			{
				return mItemList.Count;
			}
		}

		protected override void OnInit()
		{
			base.OnInit();
			mContainerTrans = this.gameObject.transform;
			//_uiTile = GetComponent<UnityEngine.UI.LayoutGroup>();

			//模板
			GameObject tpl_root = null;
			if (mContainerTrans.childCount > 0)
			{
				Transform t = mContainerTrans.GetChild(0);
				if (t != null)
				{
					if (t.name == TEMPLATE)
					{
						tpl_root = t.gameObject;
						t.gameObject.SetActive(false);
					}
				}
			}

			//缓存
			GameObject recycle_pool = new GameObject();
			recycle_pool.name = POOLOBJECT + "_" + this.gameObject.name;
			recycle_pool.SetActive(false);
			recycle_pool.transform.SetParent(mContainerTrans);

			mItemPool = new GameGridRawItemPool (recycle_pool.transform, tpl_root.transform);
			mItemPool.Init(mContainerTrans.parent);
 
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
            Clear();
			mItemPool.Release();
            mUpdateFunc = null;

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
		}
		
		/// <summary>
		/// 添加列表条
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="tplName">Tpl name.</param>
		public int AddItem( string tplName )
		{
			int newid = NextID();
			ListLogicItem item = new ListLogicItem ();
			item.ID = newid;
			item.Tpl = tplName;
			mItemList.Add(item);
			//创建控件
			CreateRawItem( item);
			return newid;	
		}
		/// <summary>
		/// 根据id删除列表项
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void RemoveItemByID( int id )
		{
			int cnt = mItemList.Count;
			for (int k = 0; k < cnt; ++k)
			{
				ListLogicItem item = mItemList [k];
				if (item.ID == id)
				{
					RemoveItem(k);
					return;
				}
			}

		}
		private ListLogicItem FindItemByID( int id )
		{
			int cnt = mItemList.Count;
			for (int k = 0; k < cnt; ++k)
			{
				ListLogicItem item = mItemList [k];
				if (item.ID == id)
				{
					return item;
				}
			}
			return null;
		}
		/// <summary>
		/// 创建列表项
		/// </summary>
		/// <param name="tplName">Tpl name.</param>
		/// <param name="OnItemCreate">On item create.</param>
		private void CreateRawItem(ListLogicItem logic_item)
		{
            MUEngine.TaskArgs args = TaskMod.Instance.GetTempArgs();
            args.AddArg(logic_item.ID);

            //异步创建
            TaskMod.Instance.CreateTask (OnCreateItem_TaskFunc, args);

		}
        private void OnCreateItem_TaskFunc(object[] args)
        {
            int id = (int)args[0];
            OnCreateItem(id);
        }
		private void OnCreateItem( int id )
		{
            ListLogicItem logic_item = FindItemByID(id);
            if (logic_item == null)
            {
                //当前列表条已经被删除，什么都不做
                return;
            }
            int logic_id = logic_item.ID;
            string tplName = logic_item.Tpl;
			GameUIComponent ui = AddChild (tplName);
			if (ui != null ) 
			{
                logic_item.mRawItem = ui;
				if (mUpdateFunc != null)
				{
                    string uid = ui.GetCustomData("UID") as string;
					mUpdateFunc(logic_id, uid);
				}
			}

		}
		private void RemoveItem(int idx)
		{
            //删除逻辑数据
            ListLogicItem logic_item = mItemList[idx];
			mItemList.RemoveAt(idx);
            //回收列表条
            if (logic_item.mRawItem != null)
            {
                mItemPool.FreeItem(logic_item.mRawItem);
            }
			
		}

		/// <summary>
		/// 添加一个子控件
		/// </summary>
		/// <returns>The child.</returns>
		/// <param name="tpl_name">Tpl name.</param>
		private GameUIComponent AddChild( string tpl_name )
		{
			GameUIComponent child = mItemPool.Allocitem(tpl_name, this.mContainerTrans);
			return child;
		}
		private int NextID()
		{
			return ++mItemIDSeed;
		}
		/// <summary>
		/// 清空列表
		/// </summary>
		public void Clear()
		{
			while (ChildCount > 0)
			{
				RemoveItem(0);
			}
		}
        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = base.SetPropertyImpl(key, val);
            if (succ)
            {
                return true;
            }

            
            return succ;
        }
		protected override bool GetPropertyImpl(UIProperty key, ref object ret)
		{
			bool succ = base.GetPropertyImpl (key, ref ret);
			if (succ) 
			{
				return true;
			}			
			return succ;
		}

        /// <summary>
        /// 模板root
        /// </summary>
        private GameObject mTplRoot;
		//private Transform poolTransform;
		//private UnityEngine.UI.LayoutGroup _uiTile;

		private List<ListLogicItem> mItemList = new List<ListLogicItem>();
		//private List<GameUIComponent> mRawItemList = new List<GameUIComponent>();
		private Transform mContainerTrans;

		/// <summary>
		/// 列表条更新函数
		/// </summary>
		private Action<int, string> mUpdateFunc;
		/// <summary>
		/// 列表条缓存池
		/// </summary>
		private GameGridRawItemPool mItemPool;

		private int mItemIDSeed = 0;
	}
}