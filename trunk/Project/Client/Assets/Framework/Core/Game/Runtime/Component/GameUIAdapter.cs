using Game.UI;
using MUGame;
using UnityEngine;

public class GameUIAdapter : MonoBehaviour
{
    public static bool NeedAdapter = false;
    public enum Anchor
    {
        Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, TopLeft, Center
    }
    public enum Stretch
    {
        None,
        Vertical,
        Horizontal,
        Detach//分离
    }
    public Canvas m_Canvas;
    public Anchor m_Anctor = Anchor.TopLeft;
    public Stretch m_Stretch = Stretch.None;
    private RectTransform mTransform;

    private float mLeftOffset = -1;
    private float mUpOffset = -1;
    private float mRightOffset = -1;
    private float mDownOffset = -1;
    //private Vector3 mLocalPosition;
    private Vector3 mAnchoredPosition3D;
    private Vector2 mOffsetMax;
    private Vector2 mOffsetMin;
    private ScreenOrientation mOrientation = ScreenOrientation.Unknown;
    private bool mIsChild = false;//如果是嵌套的适配，孩子将不起作用(防止策划多加脚本导致适配不准确)
    private void Awake()
    {
        mTransform = this.GetComponent<RectTransform>();
    }

    private void Start()
    {
        CheckIsChild();
        mAnchoredPosition3D = mTransform.anchoredPosition3D;
        //mLocalPosition = mTransform.localPosition;
        mOffsetMax = mTransform.offsetMax;
        mOffsetMin = mTransform.offsetMin;
    }

    private void LateUpdate()
    {
        if (mIsChild) return;
        if (mLeftOffset != GameConfig.ScreenLeftUpPoint.x ||
            mUpOffset != GameConfig.ScreenLeftUpPoint.y ||
            mRightOffset != GameConfig.ScreenRightDownPoint.x ||
            mDownOffset != GameConfig.ScreenRightDownPoint.y ||
            mOrientation != Screen.orientation
            )
        {
            mLeftOffset = GameConfig.ScreenLeftUpPoint.x;
            mUpOffset = GameConfig.ScreenLeftUpPoint.y;
            mRightOffset = GameConfig.ScreenRightDownPoint.x;
            mDownOffset = GameConfig.ScreenRightDownPoint.y;
            mOrientation = Screen.orientation;
            ResetAnchorPosition();
            ResetSizeDelta();
        }
    }
    private void CheckIsChild()
    {
        Transform parent = mTransform.parent;
        while(parent != null)
        {
            if(parent.GetComponent<GameUIAdapter>() != null)
            {
                mIsChild = true;
                break;
            }
            parent = parent.parent;
        }
    }

    private void ResetAnchorPosition()
    {
        if (!NeedAdapter)
        {
            //mTransform.localPosition = mLocalPosition;
            mTransform.anchoredPosition3D = mAnchoredPosition3D;
            return;
        }

        Vector3 newpos = Vector3.zero;
        newpos.x = GameConfig.ScreenLeftUpPoint.x / sCanvas.scaleFactor;// + mTransform.rect.width * pivot.x;
        newpos.y = 0;
        newpos.z = 0;
        //Vector2 pivot = mTransform.pivot;
        //Vector2 rect = new Vector2(mTransform.rect.width, mTransform.rect.height);

        if (m_Anctor == Anchor.TopLeft || m_Anctor == Anchor.Left || m_Anctor == Anchor.BottomLeft)
        {
            if (mOrientation == ScreenOrientation.LandscapeLeft || mOrientation == ScreenOrientation.LandscapeRight)
            {
                mTransform.anchoredPosition3D = mAnchoredPosition3D + newpos;
            }
            else
            {
                mTransform.anchoredPosition3D = mAnchoredPosition3D;
            }
            //mTransform.localPosition = mLocalPosition + newpos;
        }
        else if (m_Anctor == Anchor.TopRight || m_Anctor == Anchor.Right || m_Anctor == Anchor.BottomRight)
        {
            if (mOrientation == ScreenOrientation.LandscapeRight)
            {
                mTransform.anchoredPosition3D = mAnchoredPosition3D - newpos;
            }
            else
            {
                mTransform.anchoredPosition3D = mAnchoredPosition3D;
            }
            //mTransform.localPosition = mLocalPosition - newpos;
        }

    }
    private Canvas sCanvas
    {
        get
        {
            if (m_Canvas == null)
            {
                Canvas[] lst = mTransform.GetComponentsInParent<Canvas>();
                for (int i = lst.Length - 1; i >= 0; i--)
                {
                    if (lst[i].isRootCanvas)
                    {
                        m_Canvas = lst[i];
                        break;
                    }
                }
            }
            return m_Canvas;
        }
    }

    private void ResetSizeDelta()
    {
        if (m_Stretch == Stretch.None)
            return;
        if (!NeedAdapter)
        {
            mTransform.offsetMax = mOffsetMax;
            mTransform.offsetMin = mOffsetMin;
            return;
        }

        if (m_Stretch == Stretch.Horizontal || m_Stretch == Stretch.Detach)
        {
            float v = GameConfig.ScreenLeftUpPoint.x / sCanvas.scaleFactor;
            if (v > mOffsetMin.x)
            {
                if (mOrientation == ScreenOrientation.LandscapeLeft || mOrientation == ScreenOrientation.LandscapeRight)
                {
                    mTransform.offsetMin = new Vector2(GameConfig.ScreenLeftUpPoint.x / sCanvas.scaleFactor, mOffsetMin.y);
                }
                else
                {
                    mTransform.offsetMin = mOffsetMin;
                }
            }
            else
                mTransform.offsetMin = mOffsetMin;
            if (-v < mOffsetMax.x)
            {
                if (mOrientation == ScreenOrientation.LandscapeRight)
                {
                    mTransform.offsetMax = new Vector2(-GameConfig.ScreenLeftUpPoint.x / sCanvas.scaleFactor, mOffsetMax.y);
                }
                else
                {
                    mTransform.offsetMax = mOffsetMax;
                }
            }
            else
                mTransform.offsetMax = mOffsetMax;
        }
        else if (m_Stretch == Stretch.Vertical)
        {

        }
    }

}

