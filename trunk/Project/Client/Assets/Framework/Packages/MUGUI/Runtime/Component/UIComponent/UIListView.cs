using System;
using MUGame;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIListView : GameUIComponent
	{
		
#if !GAME_EDITOR

		public const string TEMPLATE = "Template";
		public const string POOLOBJECT = "pool";
		public UIListView()
		{}

        private CommonMutexComp _mutexComp;

        protected override void OnInit()
		{
			base.OnInit();
			Transform container_trans = this.gameObject.transform;

			//模板
			GameObject tpl_root = null;
			if (container_trans.childCount > 0)
			{
				Transform t = container_trans.GetChild(0);
				if (t != null)
				{
					if (t.name == TEMPLATE)
					{
						tpl_root = t.gameObject;
						t.gameObject.SetActive(false);
					}
				}
			}
            _mutexComp = this.gameObject.GetComponent<CommonMutexComp>();
            //缓存
            GameObject recycle_pool = new GameObject();
			recycle_pool.name = POOLOBJECT + "_" + this.gameObject.name;
			recycle_pool.SetActive(false);
			recycle_pool.transform.SetParent(container_trans);

			mListViewImpl = new UIListViewImpl();
			UIListBehaviour listparam = this.GetComponent<UIListBehaviour>();
			IListViewLayout layout = null;
            
			if (listparam.IsGrid)
			{
				layout = new UIGridLayout(this.gameObject.transform, this.gameObject.transform.parent, listparam.ItemSize, listparam.IsVert, listparam.CntInLine);
			}
			else
			{
				layout = new ListViewLayout(this.gameObject.transform, this.gameObject.transform.parent, listparam.IsVert);
			}

			mListViewImpl.Init(this.gameObject.transform, recycle_pool.transform, tpl_root.transform, layout);

            if(this.gameObject.transform.parent != null)
            {
                ScrollRect scrollRect = this.gameObject.transform.parent.GetComponent<ScrollRect>();
                if(scrollRect != null)
                {
                    UIBehaviorMgr.RegisterNeedAutoEnable(scrollRect);
                }
            }
		}

		protected override void OnShow()
		{
			base.OnShow();
            //每帧更新
            float checktime = 0.0001f;
            
			if (mTimerID < 0)
			{
				mTimerID = TimerHandler.SetTimeout(this.Update, checktime, true, false);
			}
		}

        public override void Dispose()
		{
			base.Dispose();
            if (this.gameObject && this.gameObject.transform.parent != null)
            {
                ScrollRect scrollRect = this.gameObject.transform.parent.GetComponent<ScrollRect>();
                if (scrollRect != null)
                {
                    UIBehaviorMgr.UnRegisterNeedAutoEnable(scrollRect);
                }
            }
            mListViewImpl.Release();
            if (mTimerID >= 0)
            {
	            TimerHandler.RemoveTimeactionByID(mTimerID);
                mTimerID = -1;
            }
            mScrollNotifyFunc = null;
            mScrollPosChangeFunc = null;

        }
		public void SetOnCreateRawItemFunc( Action<GameUIComponent> oncreate_func )
		{
			mListViewImpl.SetOnCreateRawItemFunc(oncreate_func);

		}
		public void SetOnDestroyRawItemFunc( Action<string> ondestroy_func )
		{
			mListViewImpl.SetOnDestroyRawItemFunc(ondestroy_func);
		}
		public void SetUpdateListItemFunc( Action<int, string> update_func )
		{
			mListViewImpl.SetUpdateListItemFunc(update_func);
		}
		public void SetDetachRawItemFunc(Action<int> detach_func)
		{
			mListViewImpl.SetDetachRawItemFunc(detach_func);
		}
		/// <summary>
		/// 添加列表项
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="tplname">Tplname.</param>
		public int AddItem( string tplname )
		{
			return mListViewImpl.AddItem(tplname);
		}
		public int InsertItem(int idx, string tplname)
		{
			return mListViewImpl.InsertItem (idx, tplname);	
		}

		/// <summary>
		/// 根据id删除列表项
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void RemoveItemByID( int id )
		{
			mListViewImpl.RemoveItemByID(id);

		}

		public void Clear()
		{
			mListViewImpl.Clear();
		}
        private void Update()
        {            
            //每帧更新
            if ( gameObject == null || !gameObject.activeInHierarchy)
            {
                return;
            }
            mListViewImpl.Update();
            if (mLastCount != mListViewImpl.GetListCount())
            {
                mLastCount = mListViewImpl.GetListCount();
                if (_mutexComp != null)
                {
                    _mutexComp.CheckSetMutex(mLastCount == 0);
                }
            }
            //更新通知
            float pos = mListViewImpl.GetDocPosition();
            float doc_size = mListViewImpl.GetDocSize();
            float perc_pos = pos / doc_size;
            if( mScrollNotifyFunc != null )
            {
                if (mLastScrollPos < mScrollNotifyVal && perc_pos >= mScrollNotifyVal)
                {
                    mScrollNotifyFunc();
                }
            }
            mLastScrollPos = perc_pos;
            
            if( mScrollPosChangeFunc != null)
            {
                if (mLastPos != pos && Math.Abs(mLastPos - pos) > 20f)
                {
                    mScrollPosChangeFunc();
                    mLastPos = pos;
                }
                
            }
        }

		/// <summary>
		/// 设置属性
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		/// <param name="propName">Property name.</param>
		/// <param name="propVal">Property value.</param>
		/// <param name="key">Key.</param>
		/// <param name="val">Value.</param>
        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = base.SetPropertyImpl(key, val);
            if (succ)
            {
                return true;
            }

            if (key == UIProperty.DocPosition)
            {
                mListViewImpl.SetDocPosition(float.Parse(val.ToString()));
                succ = true;
            }
            else if (key == UIProperty.ScrollNotifyFunc)
            {
                // mScrollNotifyFunc = DelegateFactory.CreateDelegate(typeof(Action), val as LuaInterface.LuaFunction) as Action;
                // succ = true;
            }
            else if (key == UIProperty.ScrollNotifyVal)
            {
                mScrollNotifyVal = float.Parse(val.ToString());
                succ = true;
            }
            else if (key == UIProperty.AddScrollCallBack)
            {
                // mScrollPosChangeFunc = DelegateFactory.CreateDelegate(typeof(Action), val as LuaInterface.LuaFunction) as Action;
                // mLastPos = mListViewImpl.GetDocPosition();
                // succ = true;
            }
            else if (key == UIProperty.RemoveScrollCallBack)
            {
                mScrollPosChangeFunc = null;
                succ = true;
            }
            
            return succ;
        }
		
		/// <summary>
		/// 获取属性
		/// </summary>
		/// <returns><c>true</c>, if property impl was gotten, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="ret">Ret.</param>
        protected override bool GetPropertyImpl(UIProperty key, ref object ret)
        {
            bool succ = base.GetPropertyImpl(key, ref ret);
            if (succ)
            {
                return true;
            }
            if (key == UIProperty.DocPosition)
            {
                ret = mListViewImpl.GetDocPosition();
                succ = true;
            }
            else if (key == UIProperty.DocSize)
            {
                ret = mListViewImpl.GetDocSize();
                succ = true;
            }
            else if (key == UIProperty.ScrollNotifyFunc)
            {
                ret = mScrollNotifyFunc;
                succ = true;
            }
            else if (key == UIProperty.ScrollPosChangeFunc)
            {
                ret = mScrollPosChangeFunc;
                succ = true;
            }
            else if (key == UIProperty.ScrollNotifyVal)
            {
                ret = mScrollNotifyVal;
                succ = true;
            }
            else
            {
                ret = null;
                succ = false;
            }
            return succ;
        }
		
        protected bool GetPropertyByParamImpl(UIProperty key, object param, ref object ret)
        {
	        bool succ = base.GetPropertyByParamImpl(key, param, ref ret);
	        if (succ) return true;
	        succ = false;
            if (key == UIProperty.ItemSize)
            {
                string p = (string)param;
                if (p.EndsWith(".ItemSize"))
                {
                    string item_name = p.Substring(0, p.Length - ".ItemSize".Length);
                    Vector2 tpl_size = mListViewImpl.GetItemTemplateSize(item_name);
                    ret = tpl_size;
                    succ = true;
                }
            }

            return succ;
        }
        
        /// <summary>
        /// 计时器
        /// </summary>
        private int mTimerID = -1;
		/// <summary>
		/// 列表实现
		/// </summary>
		private IListView mListViewImpl;
        private float mScrollNotifyVal = 1.0f;
        private Action mScrollNotifyFunc = null;
        private Action mScrollPosChangeFunc = null;
        private float mLastScrollPos = 0.0f;
        private float mLastPos = 0.0f;
        private int mLastCount = -1;
        
#endif
    }
}