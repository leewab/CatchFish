using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameScrollView : GameUIComponent
    {
        private ScrollRect scrollView;
        protected override void OnInit()
        {
            base.OnInit();
            scrollView = GetComponent<ScrollRect>();
        }

        public float VerticalValue
        {
            get { return scrollView.verticalNormalizedPosition; }
            set { scrollView.verticalNormalizedPosition = value; }
        }

        public float HorizontalValue
        {
            get { return scrollView.horizontalNormalizedPosition; }
            set { scrollView.horizontalNormalizedPosition = value; }
        }

        public void ScrllToTop()
        {
            ScrollTo(0, 1);
        }
        public void ScrollToBottom()
        {
            ScrollTo(1, 0);
        }

        public void ScrollTo(float x, float y)
        {
            scrollView.verticalNormalizedPosition = y;
            scrollView.horizontalNormalizedPosition = x;
        }

        public bool Enable
        {
            set { scrollView.enabled = value; }
        }

        //center 0 顶部
        //1 是否是在视口中间显示
        //2 如果ui没有显示全（被底部截掉）整体贴着底显示
        public void ShowItemInViewport(RectTransform tran, int pos)
        {
            if (tran == null)
                return;
            bool reset = true;
            RectTransform m_ViewRect = null;
            if (m_ViewRect == null)
                m_ViewRect = scrollView.viewport;
            if (m_ViewRect == null)
                m_ViewRect = scrollView.transform as RectTransform;

            Vector3 p = scrollView.content.localPosition;
            float tempy = 0;
            if (pos == 1)
            {
                if (tran.parent != scrollView.content)
                    tempy = Mathf.Abs(scrollView.content.InverseTransformPoint(tran.position).y);
                else
                    tempy = Mathf.Abs(tran.localPosition.y);
                tempy = tempy - m_ViewRect.rect.height / 2;//可以再减去item的高一半
                tempy = Mathf.Min(tempy, scrollView.content.rect.height - m_ViewRect.rect.height);
                tempy = Mathf.Max(tempy, 0);
            }
            else if (pos == 2)
            {
                tempy = Mathf.Abs(scrollView.content.parent.InverseTransformPoint(tran.position).y);
                if (tempy + tran.rect.height > m_ViewRect.rect.height)
                {
                    tempy = Mathf.Abs(p.y + Mathf.Abs(m_ViewRect.rect.height - tran.rect.height - tempy));
                }
                else reset = false;
            }
            if (reset)
            {
                p.y = tempy;
                scrollView.content.localPosition = p;
            }
        }

        #region 拖拽出边界刷新/分页

        enum Anchor { 
            NONE,

            Hor_LEFT,
            Hor_CENTER,
            Hor_RIGHT,

            Ver_TOP,
            Ver_CENTER,
            Ver_BOTTOM
        };
		/// <summary>
		/// 刷新方向
		/// </summary>
        enum RefreshDir
        {
            None,
            UP,
            DOWN
        };

        Anchor anchor = Anchor.NONE;
        RefreshDir refreshDir = RefreshDir.None;

        Action<float> RefreshAction;
        bool _isDragOutRefresh = false;

        public int TriggerOffset = 100;
        //Data
        RectTransform content;
        RectTransform scrollViewRect;
        float scrollViewLimit = 0.0f;
        float contentLimit = 0.0f;

        int PageIndex = -1;
        int PageCount = 0;
        bool isHorizontal = false;

        bool isTrigger = false;
        public bool IdDragOutRefresh
        {
            set {
                if (_isDragOutRefresh != value)
                {
                    _isDragOutRefresh = value;

                    if (value)
                    {
                        InitDragOutRefresh();
                    }
                    else { 
                    
                    }
                }
            }
            get
            {
                return _isDragOutRefresh;
            }
        }
		/// <summary>
		/// 初始化移到边缘时的刷新行为
		/// </summary>
        public void InitDragOutRefresh()
        {

            content = scrollView.content;

            DragEventTriggerListener dragListener = DragEventTriggerListener.Get(gameObject);
            dragListener.onDrag += OnDrag;
            dragListener.onDragStart += OnDragStart;
            dragListener.onDragEnd += OnDragEnd;

            isHorizontal = scrollView.horizontal;
            scrollViewRect = scrollView.gameObject.GetComponent<RectTransform>();
            if (isHorizontal)
            {
                scrollViewLimit = scrollViewRect.sizeDelta.x;

                float value = content.anchorMin.x;
                if (value == 0.0f)
                {
                    anchor = Anchor.Hor_LEFT;
                }
                else if (value == 0.5f)
                {
                    anchor = Anchor.Hor_CENTER;
                }
                else if (value == 1.0f)
                {
                    anchor = Anchor.Hor_RIGHT;
                }
                else
                {
                    D.error("InitDragOutRefresh Error");
                }
            }
            else
            {
                scrollViewLimit = scrollViewRect.sizeDelta.y;

                float value = content.anchorMin.y;
                if (value == 0.0f)
                {
                    anchor = Anchor.Ver_TOP;
                }
                else if (value == 0.5f)
                {
                    anchor = Anchor.Ver_CENTER;
                }
                else if (value == 1.0f)
                {
                    anchor = Anchor.Ver_BOTTOM;
                }
                else
                {
                    D.error("InitDragOutRefresh Error");
                }
            }
        }

        public void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            switch (anchor)
            {
                case Anchor.Ver_TOP:
                    {
                        float contentHieght = content.sizeDelta.y;
                        float y = content.anchoredPosition3D.y;

                        if (y < 0)
                        {
                            if (Math.Abs(y) > 100)
                            {
                                refreshDir = RefreshDir.UP;
                                isTrigger = true;
                                return;
                            }
                            else {
                                refreshDir = RefreshDir.None;
                                isTrigger = false;
                            }
                        }

                        if(contentHieght < scrollViewLimit)
                        {
                            return ;
                        }
                        float offset = y + scrollViewLimit - contentHieght;
                        if (offset > TriggerOffset)
                        {
                            refreshDir = RefreshDir.DOWN;
                            isTrigger = true;
                        }
                        else {
                            refreshDir = RefreshDir.None;
                            isTrigger = false;
                        }
                    }
                    break;
                case Anchor.Ver_CENTER:
                    {
                        float contentHieght = content.sizeDelta.y;
                        float y = content.anchoredPosition3D.y;
                        y -= scrollViewLimit / 2;
                        if (y < 0)
                        {
                            if (Math.Abs(y) > 100)
                            {
                                refreshDir = RefreshDir.UP;
                                isTrigger = true;
                                return;
                            }
                            else
                            {
                                refreshDir = RefreshDir.None;
                                isTrigger = false;
                            }
                        }

                        if (contentHieght < scrollViewLimit)
                        {
                            return;
                        }
                        float offset = y + scrollViewLimit - contentHieght;
                        if (offset > TriggerOffset)
                        {
                            refreshDir = RefreshDir.DOWN;
                            isTrigger = true;
                        }
                        else
                        {
                            refreshDir = RefreshDir.None;
                            isTrigger = false;
                        }
                    }
                    break;
                default:
                    D.error("OnDragStart.error");
                    break;
            }
        }

        public void OnDragStart(GameObject go, Vector2 delta)
        {

        }

        public void OnDragEnd(GameObject go, Vector2 delta)
        {
            if (isTrigger)
            {
                if (refreshDir == RefreshDir.DOWN)
                {
                    PageIndex += 1;

                    RefreshAction.SafeInvoke((float)(PageIndex) * PageCount);
                }
                else
                {
                    if (PageIndex > 0)
                    {
                        PageIndex -= 1;
                        RefreshAction.SafeInvoke((float)(PageIndex) * PageCount);
                    }
                }
            }
            isTrigger = false;
        }

        /// <summary>
        ///一页刷新多少数据 方便逻辑层计数
        /// </summary>
        /// <param name="pageIndex"> 起始页 </param>
        /// /// <param name="pageCount"></param>
        public void SetRefreshInfo(int pageIndex , int pageCount)
        {
            PageIndex = pageIndex;
            PageCount = pageCount;
            IdDragOutRefresh = true;
        }

        public void AddRefreshCallBack(Action<float> func)
        {
            if(IdDragOutRefresh == false)
            {
                D.error("First Call ScrollView.SetRefreshInfo");
            }
            RefreshAction += func;
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            RefreshAction = null;
        }

    }
}