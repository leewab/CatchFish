using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class UIRectSizeSync : MonoBehaviour
{

    public RectTransform mRectToSync;
    public Vector2 mSizeOffset;
    RectTransform mCachedRectTransform;
    MUGUI.AdvancedText mCachedAdvancedText;
    int mLeftUpdateCount = 0;
    // Use this for initialization
    void Start()
    {
        mCachedRectTransform = GetComponent<RectTransform>();
        mCachedAdvancedText = mRectToSync.GetComponent<MUGUI.AdvancedText>();
    }

    void LateUpdate()
    {
        mLeftUpdateCount--;
        if (mLeftUpdateCount < 0)
        {
            return;
        }
        DoUpdateBgSize();
    }

    public void UpdateBgSize()
    {
        mLeftUpdateCount = 2;
    }

    public void DoUpdateBgSize()
    {
        float width = mRectToSync.rect.width;
        float height = mRectToSync.rect.height;
        float preferredWidth = mCachedAdvancedText.preferredWidth;
        if (preferredWidth < mRectToSync.rect.width)
        {
            width = preferredWidth;
            mCachedAdvancedText.alignment = TextAnchor.UpperCenter;
        }
        else
        {
            mCachedAdvancedText.alignment = TextAnchor.UpperLeft;
        }
        mCachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + mSizeOffset.x);
        mCachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height + mSizeOffset.y);
    }
}
