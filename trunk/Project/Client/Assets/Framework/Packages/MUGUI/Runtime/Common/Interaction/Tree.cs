/* add by songshaofei 2017.9.22
 * 通用树形控件
 * 支持无限层
 * Tree控制着所有节点，统一刷新
 * 每个节点都是一个TreeItem,节点中都有自己的父节点，孩子节点列表，和唯一总控制器（Tree）
 * 每个节点都可以继续添加孩子节点，新添加的孩子节点都会在Tree中备份；
 * 节点也可以删除孩子节点，删除操作会递归删除所有孩子节点
 */
using System;
using System.Collections.Generic;
using Game.UI;
using UnityEngine;
using GraphicColorChanger = Game.UI.GraphicColorChanger;

namespace MUGUI
{
    public class Tree : MonoBehaviour
    {
        #region 成员变量
        public const string TEMPLATE = "Template";          // 模板节点名字 //
        public const int NUM_TEMPLATE = 10;                 // 模板数量 //

        [HideInInspector]
        public float HIDE_POS = 1000.0f;                    // 看不见的位置 //   

        [HideInInspector]
        public int HorizontalItemSpace = 0;                 // 树形菜单的横向排列间距 //

        [SerializeField]
        public int TypeId = 1;                              // 模板类型 //

        [SerializeField]
        public int RootCount = 0;                           // 根的数量 //

        private RectTransform mCurtrans;                                        // Tree节点 //
        private UnityEngine.UI.ScrollRect mSrollRect;
        private RectTransform mViewport;
        private RectTransform[] mTemplateAttr = new RectTransform[NUM_TEMPLATE];// 模板数组 //
        private Stack<TreeItem>[] mUIPool = new Stack<TreeItem>[NUM_TEMPLATE];  // 条目缓存池 //

        List<TreeItem> mList = new List<TreeItem>();        // 所有节点列表 //
        private float mHight = 0;                           // 刷新用高度 //
        private int mHierarchy = 0;                         // 刷新用层级 //
        private List<TreeItem> _treeViewItemsClone;         // 当前树形菜单中的所有元素克隆体（刷新树形菜单时用于过滤计算）
        public bool mMultiUnfold = false;                     // 是否可展开多个父节点 //
        public bool mAlwaysShowArrow = false;               // 是否一直显示箭头 //
        #endregion
        
        #region 内部方法或者给TreeItem使用的方法
        public void Start()
        {
            mCurtrans = gameObject.transform as RectTransform;
            mSrollRect = gameObject.GetComponentInParent<UnityEngine.UI.ScrollRect>();
            mViewport = gameObject.transform.parent as RectTransform;
            if (mSrollRect != null)
            {
                UIBehaviorMgr.RegisterNeedAutoEnable(mSrollRect);
            }
            
            // 模板节点固定名称 //
            GameObject tpl_root = null;
            Transform t = mCurtrans.GetChild(0);
            if (t != null)
            {
                if (t.name == "Template")
                {
                    tpl_root = t.gameObject;
                }
            }

            for (int i = 0; i < NUM_TEMPLATE; i++)
            {
                var trans = tpl_root.transform.Find(TEMPLATE + i.ToString());
                //没有相应的模板则直接略过。
                if (trans == null)
                {
                    continue;
                }

                if (mUIPool[i] == null)
                {
                    mUIPool[i] = new Stack<TreeItem>();
                }

                mTemplateAttr[i] = trans as RectTransform;
                mTemplateAttr[i].anchoredPosition = new Vector2(mTemplateAttr[i].anchoredPosition.x, HIDE_POS);
            }
        }

        /// <summary>
        /// 获取新rootid
        /// </summary>
        /// <returns></returns>
        private int GetNewRootId()
        {
            int count = 0;
            for (int i = 0; i < mList.Count; i++)
            {
                if (mList[i].Data.ParentId == 0)
                {
                    count++;
                }
            }
            return count + 1;
        }

        /// <summary>
        /// 实例化一个树形条目
        /// </summary>
        /// <param name="item">条目</param>
        public TreeItem Generate(int templateId)
        {
            Transform go = GameObject.Instantiate(mTemplateAttr[templateId].transform);
            TreeItem com = go.gameObject.GetComponent<TreeItem>();
            if (com == null)
            {
                com = go.gameObject.AddComponent<TreeItem>();
            }

            Vector3 oldScale = go.localScale;
            go.SetParent(mCurtrans);

            go.localScale = oldScale;
            go.localPosition = Vector3.zero;
            go.localRotation = Quaternion.identity;

            com.Init(go);
            return com;
        }

        /// <summary>
        /// 刷新孩子节点,不能单独使用
        /// </summary>
        /// <param name="tvi">孩子节点</param>
        private void RefreshTreeChild(TreeItem tvi)
        {
            int count = tvi.GetChildNumber();

            // 伸展箭头显隐判断 //
            if (tvi.ArrowDown != null && tvi.ArrowUp != null)
            {
                if (count == 0 && !AlwaysShowArrow)
                {
                    tvi.ArrowDown.gameObject.SetActive(false);
                    tvi.ArrowUp.gameObject.SetActive(false);
                }
                else
                {
                    if (tvi.IsExpanding)
                    {
                        tvi.ArrowDown.gameObject.SetActive(false);
                        tvi.ArrowUp.gameObject.SetActive(true);
                    }
                    else
                    {
                        tvi.ArrowDown.gameObject.SetActive(true);
                        tvi.ArrowUp.gameObject.SetActive(false);
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                TreeItem child = tvi.GetChildByIndex(i);

                // 设置当前条目高度 //
                child.mRectTranform.localPosition = new Vector3(child.GetHierarchy() * HorizontalItemSpace, mHight, 0);
                child.Data.Y = mHight;

                // 计算下一条目的起始高度 //
                float ItemHeight = child.mRectTranform.rect.height;
                mHight += (-(ItemHeight));

                if (child.GetHierarchy() > mHierarchy)
                {
                    mHierarchy = child.GetHierarchy();
                }

                if (child.Title != null)
                {
                    child.Title.text = child.Data.Title;
                }

                if (child.IsExpanding)
                {
                    RefreshTreeChild(child);
                }
                else
                {
                    SetChildAlive(child, false);
                }

                int index = _treeViewItemsClone.IndexOf(child);
                if (index >= 0)
                {
                    _treeViewItemsClone[index] = null;
                }
            }
        }

        private void SetChildAlive( TreeItem ti, bool alive )
        {
            int count = ti.GetChildNumber();
            for (int v = 0; v < count; v++)
            {
                TreeItem child = ti.GetChildByIndex(v);
                child.SetVisible( alive );

                if (child.GetChildNumber() > 0)
                {
                    SetChildAlive(child, alive);
                }
            }
        }

        /// <summary>
        /// 从缓存中取控件
        /// </summary>
        /// <param name="templateId">模板类型</param>
        /// <returns>节点</returns>
        public TreeItem AllocItem(int templateId)
        {
            // 缓存池里有，就用池里的 //
            var curpool = mUIPool[templateId];
            if (curpool.Count > 0)
            {
                TreeItem item = curpool.Pop();
                item.Data.Alive = true;
                return item;
            }
            else
            {
                return Generate(templateId);
            }
        }

        /// <summary>
        /// 删除的条目，缓存起来
        /// </summary>
        /// <param name="item">条目</param>
        public void FreeItem(TreeItem item)
        {
            // 缓存前，先删除该条目在总列表中的引用 //
            for (int i = 0; i < mList.Count; i++)
            {
                if (mList[i] == item)
                {
                    mList.RemoveAt(i);
                    break;
                }
            }
            mUIPool[item.Data.TemplateId].Push(item);
        }

        /// <summary>
        /// 该方法给TreeItem使用，仅仅往列表中加一个条目
        /// </summary>
        /// <param name="item">条目</param>
        public void AddItemNoGenerate(TreeItem item)
        {
            mList.Add(item);
        }
        #endregion
        #region 对外接口

        /// <summary>
        /// 添加一个根节点
        /// </summary>
        /// <param name="templateId">模板id</param>
        /// <param name="title">标题内容</param>
        /// <returns>返回根节点</returns>
        public TreeItem AddRoot(int templateId, string title)
        {
            Start();
            TreeItem item = AllocItem(templateId);
            item.Controler = this;
            item.SetParent(null);
            item.Data.ParentId = 0;
            item.Data.SelfId = GetNewRootId();
            item.Data.Title = title;
            item.Data.TemplateId = templateId;
            item.SetHierarchy(0);

            string strName = "Level" + (item.GetHierarchy() + 1).ToString() + "_Parent" + item.Data.ParentId.ToString() + "_Id" + item.Data.SelfId.ToString();
            item.mRectTranform.name = strName;
            item.Data.Name = strName;

            mList.Add(item);
            return item;
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public void Clear()
        {
            while (mList.Count > 0)
            {
                TreeItem item = mList[0];
                item.RemoveSelf();
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                TreeItem item = mList[i];
                item.Dispose();
            }

            if (mSrollRect != null)
            {
                UIBehaviorMgr.UnRegisterNeedAutoEnable(mSrollRect);
            }
        }

        /// <summary>
        /// 删除一个子节点，包括根节点
        /// </summary>
        /// <param name="item">子节点</param>
        public void RemoveChild(TreeItem item)
        {
            // 自己本身就是根节点 //
            if (item.GetParent() == null)
            {
                item.RemoveSelf();
            }
            else
            {
                // 自己不是根节点，找到自己的父亲 //
                for (int i = 0; i < mList.Count; i++)
                {
                    if (item.GetParent() == mList[i])
                    {
                        mList[i].RemoveChild(item);
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 刷新所有子节点
        /// </summary>
        public void Refresh()
        {
            mHight = 0;
            mHierarchy = 0;

            // 复制一份菜单,递归使用 //
            _treeViewItemsClone = new List<TreeItem>(mList);

            // 用复制的菜单进行刷新计算 //
            for (int i = 0; i < _treeViewItemsClone.Count; i++)
            {
                // 已经计算过或者不需要计算位置的元素，过滤掉 //
                if (_treeViewItemsClone[i] == null || !_treeViewItemsClone[i].Data.Alive)
                {
                    continue;
                }

                TreeItem ti = _treeViewItemsClone[i];

                // 伸展箭头显隐判断 //
                if (ti.ArrowDown != null && ti.ArrowUp != null)
                {
                    if (ti.GetChildNumber() == 0 && !AlwaysShowArrow)
                    {
                        ti.ArrowDown.gameObject.SetActive(false);
                        ti.ArrowUp.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (ti.IsExpanding)
                        {
                            ti.ArrowDown.gameObject.SetActive(false);
                            ti.ArrowUp.gameObject.SetActive(true);
                        }
                        else
                        {
                            ti.ArrowDown.gameObject.SetActive(true);
                            ti.ArrowUp.gameObject.SetActive(false);
                        }
                    }
                }

                if (ti.Title != null)
                {
                    ti.Title.text = ti.Data.Title;
                }

                // 设置当前条目的高度 //
                ti.mRectTranform.localPosition = new Vector3(ti.GetHierarchy() * HorizontalItemSpace, mHight, 0);
                ti.Data.Y = mHight;

                // 计算下一条目的起始高度 //
                float ItemHeight = ti.mRectTranform.rect.height;
                mHight += (-(ItemHeight));

                if (ti.GetHierarchy() > mHierarchy)
                {
                    mHierarchy = ti.GetHierarchy();
                }

                // 如果是展开的，刷新孩子节点 //
                if (ti.IsExpanding)
                {
                    RefreshTreeChild(ti);
                }
                else
                {
                    SetChildAlive(ti, false);
                }
                ti = null;
            }

            //重新计算滚动视野的区域
            if (mSrollRect != null)
            {
                float x = mSrollRect.content.sizeDelta.x;//mHierarchy * HorizontalItemSpace + ItemWidth;
                float y = Mathf.Abs(mHight);
                mSrollRect.content.sizeDelta = new Vector2(x, y);
            }

            //清空复制的菜单
            _treeViewItemsClone.Clear();
        }

        /// <summary>
        /// 设置可多层展开
        /// </summary>
        /// <param name="multi">是否可多层展开</param>
        public void SetMultiUnfold(bool multi)
        {
            mMultiUnfold = multi;
        }

        /// <summary>
        /// 是否一直显示箭头
        /// </summary>
        public bool AlwaysShowArrow
        {
            get { return mAlwaysShowArrow; }
            set { mAlwaysShowArrow = value; }
        }

        /// <summary>
        /// 设置伸展
        /// </summary>
        /// <param name="isExpand">是否伸展</param>
        public void SetExpand(bool isExpand)
        {
            for (int i = 0; i < mList.Count; i++)
            {
                TreeItem item = mList[i];
                if (item.GetChildNumber() > 0)
                {
                    item.IsExpanding = isExpand;
                }
                else
                {
                    item.IsExpanding = false;
                }
                
                if (item.Data.ParentId == 0)
                {
                    continue;
                }

                if (isExpand)
                {
                    item.SetVisible(true);
                }
                else
                {
                    item.SetVisible(false);
                }
            }

            Refresh();
        }

        public void SetExpandExcept(TreeItem item)
        {
            for (int i = 0; i < mList.Count; i++)
            {
                TreeItem it = mList[i];
                if (it.Data.ParentId != 0)
                {
                    continue;
                }

                if (it == item)
                {
                    continue;
                }

                it.SetExpand(false);
            }
        }

        /// <summary>
        /// 清除所有节点的选中状态
        /// </summary>
        public void ClearCheckState()
        {
            for (int i = 0; i < mList.Count; i++)
            {
                mList[i].SetCheckbox(false);
            }
        }

        /// <summary>
        /// 设置树形菜单的横向排列间距
        /// </summary>
        /// <param name="space">间距</param>
        public void SetHorizontalItemSpace(int space)
        {
            HorizontalItemSpace = space;
        }

        /// <summary>
        /// 移动到指定条目
        /// </summary>
        /// <param name="item">具体的条目</param>
        public void MoveToItem(TreeItem item)
        {
            if (item == null || mViewport == null)
            {
                return;
            }

            // 如果这个条目是未激活的，强制激活 //
            if (!item.Data.Alive)
            {
                if (item.GetParent() == null)
                {
                    item.OnClickCheckbox();
                }
                else
                {
                    item.GetParent().OnClickCheckbox();
                }
            }

            float viewH = mViewport.rect.height;
            // 从开头找
            float h = Math.Abs(item.Data.Y) + item.mRectTranform.rect.height;
            if (h < viewH)
            {
                Vector3 pos = mCurtrans.anchoredPosition;
                pos.y = 0;
                mCurtrans.anchoredPosition = pos;
                return;
            }

            // 从尾部找
            float h1 = Math.Abs(item.Data.Y);
            float h2 = Math.Abs(mHight) - viewH;
            if ( h1 > h2 )
            {
                Vector3 pos = mCurtrans.anchoredPosition;
                pos.y = Math.Abs(mHight) - viewH;
                mCurtrans.anchoredPosition = pos;
                return;
            }

            // 中间居中
            h = viewH / 2;
            h1 = Math.Abs(item.Data.Y) - h;
            h2 = h1 + (item.mRectTranform.rect.height / 2);
            Vector3 pos1 = mCurtrans.anchoredPosition;
            pos1.y = h2;
            mCurtrans.anchoredPosition = pos1;
        }
        #endregion
        
    }

    public class TreeData
    {
        public int ParentId = 0;    // 父亲id //
        public int SelfId = 0;      // 自己id //
        public string Name = "";    // 控件名字 //
        public int TemplateId = 1;  // 模板号 //
        public float Y = 0f;        // Y轴高度 //
        public string Title = "";   // 标题 //
        public bool Alive = true;   // 活跃性 //
    }

    public class TreeItem : MonoBehaviour
    {
        
        [HideInInspector]
        public Tree Controler;
        [HideInInspector]
        public RectTransform mRectTranform;
        [HideInInspector]
        public bool IsExpanding = false;
        [HideInInspector]
        public TreeData Data = new TreeData();
        [HideInInspector] public GameUIComponent UICom;
        [HideInInspector] public Action clickCheckCallBack = null;

        // 约定的控件部分 //
        [HideInInspector]
        public Transform ArrowUp;                   // 向上箭头 //
        [HideInInspector]
        public Transform ArrowDown;                 // 向下箭头 //
        [HideInInspector]
        public UnityEngine.UI.Text Title;           // 标题 //
        [HideInInspector]
        public UnityEngine.UI.Image ShowCheckbox;   // 用于显示高亮的图片 //
        [HideInInspector]
        public UnityEngine.UI.Button ClickCheckbox; // 用于点击高亮的按钮 //
        [HideInInspector]
        public GraphicColorChanger colorChanger;    //用于在选中状态发生变化时，切换某些UI的Color值

        [SerializeField]
        public int TypeId = 2;
        [SerializeField]
        public int ChildCount = 2;
        [SerializeField]
        public bool SpecialExtend = false;

        // 当前元素在树形图中所属的层级
        private int mHierarchy = 0;

        // 当前元素指向的父元素
        private TreeItem mParent;

        // 当前元素的所有子元素
        private List<TreeItem> mChildren;

        private bool mCheckClickEnable = true;

        protected void Start()
        {

        }
        public void Dispose()
        {
            this.UICom.Dispose();
            if (mChildren != null)
            {
                foreach (var item in mChildren)
                {
                    item.UICom.Dispose();
                }
            }
            clickCheckCallBack = null;
        }
        
        public void Init(Transform transform)
        {
            mRectTranform = transform.gameObject.GetComponent<RectTransform>();

            // 初始化约定的旋转箭头 //
            ArrowUp = mRectTranform.Find("Tree-Arrowup");
            ArrowDown = mRectTranform.Find("Tree-Arrowdown");

            // 初始化约定的标题 //
            Transform label = mRectTranform.Find("Tree-Title");
            if (label != null)
            {
                Title = label.GetComponent<UnityEngine.UI.Text>();
            }

            // 初始化约定的选中 //
            Transform image = mRectTranform.Find("Tree-ShowCheck");
            if (image != null)
            {
                ShowCheckbox = image.GetComponent<UnityEngine.UI.Image>();
                ShowCheckbox.gameObject.SetActive(false);
            }

            Transform button = mRectTranform.Find("Tree-ClickCheck");
            if (button != null)
            {
                ClickCheckbox = button.GetComponent<UnityEngine.UI.Button>();
                if (ClickCheckbox != null)
                {
                    ClickCheckbox.onClick.AddListener(OnClickCheckbox);
                }
            }

            //初始化约定的颜色更改控件，该控件目前挂载在根节点上
            colorChanger = GetComponent<GraphicColorChanger>();

            UICom = new GameUIComponent(transform.gameObject);
        }
        /// <summary>
        /// click回调lua
        /// </summary>
        public void SetClickCheckboxCallBack(Action luaFunc)
        {
            clickCheckCallBack = luaFunc;
        }
        /// <summary>
        /// 模拟Checkbox 效果
        /// </summary>
        public void OnClickCheckbox()
        {
            // 操作伸展 //
            if (!(SpecialExtend  && IsExpanding == true))
            {
                SetExpand(!IsExpanding);
            }

            // 处理选中 //
            if (mParent == null)
            {
                bool isHaveCheck = IsHaveCheckedInChildren();
                if (!isHaveCheck)
                {
                    Controler.ClearCheckState();
                    SetCheckbox(true);
                    SetChildrenChecked(0);
                }
            }
            else
            {
                Controler.ClearCheckState();
                mParent.ClearChildrenChecked();
                mParent.SetCheckbox(true);
                SetCheckbox(true);
            }

            // 处理伸展互斥,除了当前展开，其它根全部收起 //
            if (!Controler.mMultiUnfold)
            {
                if (IsExpanding || mParent == null)
                {
                    TreeItem root = GetMyRoot();
                    Controler.SetExpandExcept(root);
                }
            }

            //刷新树形菜单
            Controler.Refresh();

            if(clickCheckCallBack != null)
            {
                clickCheckCallBack();
            }
        }

        /// <summary>
        /// 设置选中
        /// </summary>
        /// <param name="isChecked">是否选中</param>
        public void SetCheckbox(bool isChecked)
        {
            if (ShowCheckbox != null)
            {
                //if (isChecked)
                //{
                //Controler.ClearCheckState();
                //}
                ShowCheckbox.gameObject.SetActive(isChecked);
            }
            //根据选中状态，更改某些UI的颜色，约定0未被选中，1是被选中的颜色
            if(colorChanger != null)
            {
                colorChanger.SetColorByIndex(isChecked ? 1 : 0);
            }
        }

        public bool GetChecked()
        {
            if (ShowCheckbox != null)
            {
                return ShowCheckbox.gameObject.activeSelf;
            }

            return false;
        }

        /// <summary>
        /// 清除孩子节点中的选中状态
        /// </summary>
        public void ClearChildrenChecked()
        {
            int count = GetChildNumber();
            for (int i = 0; i < count; i++)
            {
                TreeItem item = GetChildByIndex(i);
                if (item != null)
                {
                    item.SetCheckbox(false);
                }
            }
        }

        /// <summary>
        /// 设置某个孩子选中
        /// </summary>
        /// <param name="index">孩子索引</param>
        public void SetChildrenChecked(int index)
        {
            int count = GetChildNumber();
            for (int i = 0; i < count; i++)
            {
                TreeItem item = GetChildByIndex(i);
                if (index == i)
                {
                    item.SetCheckbox(true);
                    break;
                }
                else
                {
                    item.SetCheckbox(false);
                }
            }
        }

        public bool IsHaveCheckedInChildren()
        {
            int count = GetChildNumber();
            for (int i = 0; i < count; i++)
            {
                TreeItem item = GetChildByIndex(i);
                if (item.GetChecked())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 设置选中点击失效
        /// </summary>
        /// <param name="enable">是否失效</param>
        public void SetCheckClickEnable(bool enable)
        {
            mCheckClickEnable = enable;
        }

        /// <summary>
        /// 改变某一节点和它所有子节点的显示状态
        /// </summary>
        /// <param name="tvi">某一节点</param>
        /// <param name="value">显示状态</param>
        public void ChangeChildren(TreeItem tvi, bool value)
        {
            int count = tvi.GetChildNumber();
            for (int v = 0; v < count; v++)
            {
                TreeItem child = tvi.GetChildByIndex(v);
                child.SetVisible(value);

                if (child.GetChildNumber() > 0)
                {
                    ChangeChildren(child, value);
                }
            }
        }
        public void SetSpecialExpand(bool flag)
        {
            SpecialExtend = flag;
        }

        /// <summary>
        /// 通过不点击按钮的方式来控制条目的展开与折叠
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetExpand(bool value)
        {
            if (IsExpanding != value)
            {
                if (IsExpanding)
                {
                    if (GetChildNumber() > 0 || Controler.AlwaysShowArrow)
                    {
                        if (ArrowUp != null)
                        {
                            ArrowUp.gameObject.SetActive(false);
                        }

                        if (ArrowDown != null)
                        {
                            ArrowDown.gameObject.SetActive(true);
                        }
                    }

                    IsExpanding = false;
                    ChangeChildren(this, false);
                }
                else
                {
                    int count = GetChildNumber();
                    if (count > 0 || Controler.AlwaysShowArrow)
                    {
                        if (ArrowUp != null)
                        {
                            ArrowUp.gameObject.SetActive(true);
                        }

                        if (ArrowDown != null)
                        {
                            ArrowDown.gameObject.SetActive(false);
                        }
                    }
                    if (count > 0)
                    {
                        IsExpanding = true;
                    }
                    else
                    {
                        IsExpanding = false;
                    }

                    ChangeChildren(this, true);
                }
            }
        }
        /// <summary>
        /// 设置显隐，为了效率，是移动到不可见位置
        /// </summary>
        /// <param name="isVisible">是否显示</param>
        public void SetVisible(bool isVisible)
        {
            Vector2 pos = mRectTranform.anchoredPosition;
            if (isVisible)
            {
                pos.y = Data.Y;
                Data.Alive = true;
            }
            else
            {
                pos.y = Controler.HIDE_POS;
                Data.Alive = false;
            }

            Data.Y = pos.y;
            mRectTranform.anchoredPosition = pos;
        }

        /// <summary>
        /// 获取层级
        /// </summary>
        /// <returns>层级</returns>
        public int GetHierarchy()
        {
            return mHierarchy;
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="hierarchy"></param>
        public void SetHierarchy(int hierarchy)
        {
            mHierarchy = hierarchy;
        }

        /// <summary>
        /// 获取模板类型
        /// </summary>
        /// <returns>模板类型</returns>
        public int GetTemplateId()
        {
            return Data.TemplateId;
        }

        /// <summary>
        /// 获取该分支的根节点
        /// </summary>
        /// <returns></returns>
        public TreeItem GetMyRoot()
        {
            TreeItem parent = GetParent();
            if (GetParent() == null)
            {
                return this;
            }

            return parent.GetMyRoot();
        }

        /// <summary>
        /// 获取父亲节点
        /// </summary>
        /// <returns>父亲节点</returns>
        public TreeItem GetParent()
        {
            return mParent;
        }

        /// <summary>
        /// 设置父亲节点
        /// </summary>
        /// <param name="parent">父亲节点</param>
        public void SetParent(TreeItem parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="templateId">模板类型</param>
        /// <param name="title">标题</param>
        /// <returns>新的孩子节点</returns>
        public TreeItem AddChild(int templateId, string title)
        {
            if (mChildren == null)
            {
                mChildren = new List<TreeItem>();
            }

            TreeItem item = Controler.AllocItem(templateId);
            // this.UI.AddChildComponent(item.UI, this.UI);
            item.Controler = this.Controler;
            item.SetParent(this);
            item.Data.ParentId = this.Data.SelfId;
            item.Data.SelfId = mChildren.Count + 1;
            item.mHierarchy = this.mHierarchy + 1;
            item.Data.TemplateId = templateId;
            item.Data.Title = title;

            string strName = "Level" + (item.GetHierarchy() + 1).ToString() + "_Parent" + item.Data.ParentId.ToString() + "_Id" + item.Data.SelfId.ToString();
            item.mRectTranform.name = strName;
            item.Data.Name = strName;

            mChildren.Add(item);

            this.Controler.AddItemNoGenerate(item);
            return item;
        }

        /// <summary>
        /// 删除自己
        /// </summary>
        public void RemoveSelf()
        {
            RemoveAllChildren();

            KillSelf();
        }

        /// <summary>
        /// 删除自己所有子节点
        /// </summary>
        public void RemoveAllChildren()
        {
            while (GetChildNumber() > 0)
            {
                TreeItem item = GetChildByIndex(0);
                if (item != null)
                {
                    RemoveChild(item);
                }
            }

            if (mChildren != null)
            {
                mChildren.Clear();
            }
        }

        /// <summary>
        /// 删除某个固定子节点
        /// </summary>
        /// <param name="child">某个子节点</param>
        public void RemoveChild(TreeItem child)
        {
            if (mChildren == null)
            {
                return;
            }

            if (child == null)
            {
                return;
            }

            int count = child.GetChildNumber();
            for (int i = 0; i < count; i++)
            {
                TreeItem item = child.GetChildByIndex(i);
                if (item != null)
                {
                    child.RemoveChild(item);
                }
            }

            child.KillSelf();
        }

        /// <summary>
        /// 从孩子中移除一个节点
        /// </summary>
        /// <param name="child"></param>
        private void RemoveChildFromList(TreeItem child)
        {
            if (mChildren == null)
                return;

            for (int i = 0; i < mChildren.Count; i++)
            {
                if (mChildren[i] == child)
                {
                    mChildren.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 干掉自己，实际上是把自己移动到不可视区域，并加入缓存池中
        /// </summary>
        private void KillSelf()
        {
            // 把自己移动到不可视位置 //
            SetVisible(false);

            // 条目缓存起来 //
            Controler.FreeItem(this);

            // 清除自己在父亲列表中的引用 //
            if (mParent != null)
            {
                mParent.RemoveChildFromList(this);
            }

            // 清除自己的孩子 //
            if (mChildren != null)
            {
                mChildren.Clear();
            }

            // 清除自身的选中状态
            if (ShowCheckbox != null)
            {
                ShowCheckbox.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 按索引删除子节点
        /// </summary>
        /// <param name="index">索引</param>
        public void RemoveChild(int index)
        {
            if (mChildren == null || index < 0 || index >= mChildren.Count)
            {
                return;
            }

            RemoveChild(mChildren[index]);
        }

        /// <summary>
        /// 获取子节点个数
        /// </summary>
        /// <returns>个数</returns>
        public int GetChildNumber()
        {
            if (mChildren == null)
            {
                return 0;
            }
            return mChildren.Count;
        }

        /// <summary>
        /// 根据索引，获取子节点
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>对应节点</returns>
        public TreeItem GetChildByIndex(int index)
        {
            if (index >= mChildren.Count)
            {
                return null;
            }
            return mChildren[index];
        }

    }
}
