using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Game.UI
{
	public class UIListBehaviour :MonoBehaviour
	{
		/// <summary>
		/// 是否是纵向列表
		/// </summary>
		/// <value><c>true</c> if this instance is vert; otherwise, <c>false</c>.</value>
		public bool IsVert
		{
			get
			{
				return mIsVert;
			}
			set
			{
				mIsVert = value;
			}
		}
		/// <summary>
		/// 是否是grid
		/// </summary>
		/// <value><c>true</c> if this instance is grid; otherwise, <c>false</c>.</value>
		public bool IsGrid
		{
			get
			{
				return mIsGrid;
			}
			set
			{
				mIsGrid = value;
			}
		}
		/// <summary>
		/// 每个列表条的尺寸
		/// </summary>
		/// <value>The size of the item.</value>
		public Vector2 ItemSize
		{
			get
			{
				return mItemSize;
			}
			set
			{
				mItemSize = value;
			}
		}
		/// <summary>
		/// 一行的数量
		/// </summary>
		/// <value>The count in line.</value>
		public int CntInLine
		{
			get
			{
				return mCntInLine;
			}
			set
			{
				mCntInLine = value;
			}

		}

		public string ToShowItemName
		{
			get
			{
				return mToShowTplName;
			}
			set
			{
				mToShowTplName = value;
			}
		}
		public int ToShowItemCnt
		{
			get
			{
				return mToShowItemCnt;
			}
			set
			{
				mToShowItemCnt = value;
			}
		}
		public int ItemCnt
		{
			get
			{
				return GetItems ().Count;
			}
		}


        public bool IsUseItemSelfSize
        {
            get
            {
                return mUseItemSelfSize;
            }
            set
            {
                mUseItemSelfSize = value;
            }
        }

        public UIListBehaviour()
		{
			mIsVert = false;
			mIsGrid = false;
			mCntInLine = 0;
		}

		public void CreateItems()
		{
			string tplname = mToShowTplName;
			int cnt = mToShowItemCnt;
			ClearItems();
			if (cnt <= 0)
			{
				return;
			}
			GameObject tpl_root = GetTplRoot ();
			if (tpl_root != null) 
			{
				tpl_root.SetActive (false);
			}
			//创建所有列表条
			GameObject tpl = FindTemplate(tplname);	
			List<GameObject> demoitems = new List<GameObject> ();
			for (int k = 0; k < cnt; ++k)
			{
				GameObject new_child = GameObject.Instantiate(tpl);
				new_child.transform.SetParent(this.transform, false);
				demoitems.Add(new_child);
			}
			Vector2 item_size = Vector2.zero;
			int cntinline = 1;
			if (mIsGrid)
			{
				//如果是固定尺寸
				item_size = mItemSize;
				cntinline = mCntInLine;
			}
			else
			{
				//尺寸由列表条本身决定
				item_size = tpl.transform.GetComponent<RectTransform>().rect.size;
				cntinline = 1;
			}

			//执行布局
			DoLayout( demoitems, item_size, mIsVert, cntinline);

		}  
		private void DoLayout( List<GameObject> items, Vector2 itemsize, bool is_vert, int cntinline )
		{
			//横方向向右x值越大
			//纵方向向上y值越大
			for (int k = 0; k < items.Count; ++k)
			{
				GameObject item = items [k];

				float x = 0.0f;
				float y = 0.0f;

				if (is_vert)
				{
					int row = k / cntinline;
					int col = k % cntinline;
					y = -row * itemsize.y;
					x = col * itemsize.x;
				}
				else
				{
					int col = k / cntinline;
					int row = k % cntinline;
					x = col * itemsize.x;
					y = -row * itemsize.y;
				}

				item.transform.localPosition = new Vector3 (x, y, 0.0f);
			}
			RectTransform container_trans = this.GetComponent<RectTransform>();
			if (is_vert)
			{
				int maxrow = (int)Math.Ceiling( items.Count / (float)cntinline);
				container_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxrow * itemsize.y);
			}
			else
			{
				int maxcol = (int)Math.Ceiling( items.Count / (float)cntinline);
				container_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxcol * itemsize.x);
			}
		}


		/// <summary>
		/// 清空所有列表项
		/// </summary>
		public void ClearItems()
		{
			GameObject tpl_root = GetTplRoot ();
			if (tpl_root != null) 
			{
				tpl_root.SetActive (true);
			}
			List<GameObject> child_list = GetItems ();
			for (int k = 0; k < child_list.Count; ++k)
			{
				GameObject item = child_list [k];
				GameObject.DestroyImmediate(item);
			}
            RectTransform container_trans = this.GetComponent<RectTransform>();
            RectTransform parent_trans = this.gameObject.transform.parent.GetComponent<RectTransform>();
            container_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parent_trans.rect.height);
            container_trans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parent_trans.rect.width);


        }
		private List<GameObject> GetItems()
		{
			List<GameObject> child_list = new List<GameObject> ();
			int cnt = this.transform.childCount;
			for (int k = 0; k < cnt; ++k) 
			{
				Transform cur_child = this.transform.GetChild (k);
				if (cur_child.name != TEMPLATE) 
				{
					child_list.Add (cur_child.gameObject);
				}
			}
			return child_list;
		}
		/// <summary>
		/// 搜索模板
		/// </summary>
		/// <returns>The template root.</returns>
		private GameObject FindTemplate( string tplname )
		{
			GameObject tpl_root = GetTplRoot();
			if (tpl_root == null)
			{
				return null;
			}
			int cnt = tpl_root.transform.childCount; 
			for (int k = 0; k < cnt; ++k)
			{
				Transform child = tpl_root.transform.GetChild(k);
				if (child.name == tplname)
				{
					return child.gameObject;
				}
			}
			return null;
		}
		private GameObject GetTplRoot()
		{
			int cnt = this.transform.childCount;
			for (int k = 0; k < cnt; ++k)
			{
				Transform child = this.transform.GetChild(k);
				if (child.name == TEMPLATE)
				{
					return child.gameObject;
				}
			}
			return null;
		}
	
		/// <summary>
		/// 是否是纵向列表
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private bool mIsVert = true;
		/// <summary>
		/// 列表条是否是固定尺寸
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private bool mIsGrid = false;

		/// <summary>
		/// 列表项尺寸
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private Vector2 mItemSize = Vector2.zero;

		/// <summary>
		/// 一行有多少个列表项
		/// </summary>
		[HideInInspector]
		[SerializeField]
		private int mCntInLine = 1;


        [HideInInspector]
        [SerializeField]
        private bool mUseItemSelfSize = false;

        private string TEMPLATE = "Template";

		private string mToShowTplName = "";
		private int mToShowItemCnt = 0;

	}
}

