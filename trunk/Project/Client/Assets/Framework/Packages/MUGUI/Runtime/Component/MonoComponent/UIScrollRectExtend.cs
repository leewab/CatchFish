using System;

namespace Game.UI
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// 滚动条扩展
    /// 1 上下左右(有内容)箭头标示
    /// </summary>
    public class UIScrollRectExtend : MonoBehaviour
    {
        [Serializable]
        public sealed class ArrowObj
        {
            public GameObject Up;
            public GameObject Down;
            public GameObject Left;
            public GameObject Right;

            public bool Valid
            {
                get
                {
                    if (Up != null) return true;
                    if (Down != null) return true;
                    if (Left != null) return true;
                    if (Right != null) return true;
                    return false;
                }
            }
        }

        [SerializeField] private ArrowObj m_ArrowObj = new ArrowObj();

        private ScrollRect mScroll;

        void Awake()
        {
            mScroll = GetComponent<ScrollRect>();
            if (mScroll != null)
            {
                mScroll.onValueChanged.AddListener(OnValueChanged);
            }
        }

        void OnEnable()
        {
            if (mScroll != null)
            {
                //保证Position正确
                if (mScroll.vertical) mScroll.verticalNormalizedPosition = 1;
                if (mScroll.horizontal) mScroll.horizontalNormalizedPosition = 0;
                SetArrowObjState();
            }
        }

        private void OnValueChanged(Vector2 v)
        {
            SetArrowObjState();
        }

        /// <summary>
        /// 外部调用（也可以通过SendMessage触发）
        /// </summary>
        public void SetDirty()
        {
            SetArrowObjState();
        }

        private RectTransform m_ViewRect;

        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null && mScroll != null)
                    m_ViewRect = mScroll.viewport;
                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;
                return m_ViewRect;
            }
        }

        private void SetArrowObjState()
        {
            if (!m_ArrowObj.Valid) return;
            if (mScroll == null || viewRect == null || mScroll.content == null)
            {
                if (m_ArrowObj != null)
                {
                    if (m_ArrowObj.Up != null) m_ArrowObj.Up.SetActive(false);
                    if (m_ArrowObj.Down != null) m_ArrowObj.Down.SetActive(false);
                    if (m_ArrowObj.Right != null) m_ArrowObj.Right.SetActive(false);
                    if (m_ArrowObj.Left != null) m_ArrowObj.Left.SetActive(false);
                }
            }
            else
            {
                //10是容错数值
                if (mScroll.vertical)
                {
                    if (m_ArrowObj != null && (m_ArrowObj.Up != null || m_ArrowObj.Down != null))
                    {
                        float vH = viewRect.rect.height;
                        float cH = mScroll.content.rect.height;
                        float offY = -mScroll.content.localPosition.y; //向上为正，向下为负
                        offY = offY - ((1 - mScroll.content.pivot.y) * cH);
                        float h = offY + cH;
                        if (m_ArrowObj.Down != null)
                        {
                            m_ArrowObj.Down.SetActive((h - vH) > 10);
                        }

                        if (m_ArrowObj.Up != null)
                        {
                            m_ArrowObj.Up.SetActive(offY < -10);
                        }
                    }
                }

                if (mScroll.horizontal)
                {
                    if (m_ArrowObj != null && (m_ArrowObj.Right != null || m_ArrowObj.Left != null))
                    {
                        float vW = viewRect.rect.width;
                        float cW = mScroll.content.rect.width;
                        float offX = mScroll.content.localPosition.x;
                        offX = offX - ((mScroll.content.pivot.x) * cW);
                        float w = offX + cW;
                        if (m_ArrowObj.Right != null)
                        {
                            m_ArrowObj.Right.SetActive((w - vW) > 10);
                        }

                        if (m_ArrowObj.Left != null)
                        {
                            m_ArrowObj.Left.SetActive(offX < -10);
                        }
                    }
                }
            }
        }
    }

}