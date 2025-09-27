using UnityEngine;
using System;
using System.Collections;
[ExecuteInEditMode]
public class UIEffectScale : MonoBehaviour
{
    public class ScaleInfo
    {
        public ParticleSystem ps = null;
        public float mOriginalScale = 1f;
    }

    public float mSize = 1.0f;
    private float mOldSize = 1.0f;
    private ScaleInfo[] mScaleInfoArray = null;
    private Renderer[] mRenderers = null;
    private bool mDirty = true;
    int renderqueue = -1;
    int oldRenderqueue = -1;
    public bool mAutoHide = true;
    public float mLifeTime = 0;
    public float mLeftLifeTime = 0;
    bool mIsFirstEnabled = true;
    public int mPlayCount = 1;
    public Action mAutoHideCallback = null;
    public float Scale
    {
        get { return mSize; }
        set
        {
            mSize = value;
        }
    }

    public int RenderQueue
    {
        get
        {
            return renderqueue;
        }
        set
        {
            if (value != renderqueue)
            {
                renderqueue = value;
            }
        }
    }

    public Action AutoHideCallback
    {
        set
        {
            mAutoHideCallback -= value;
            mAutoHideCallback += value;
        }
    }

    public bool Dirty
    {
        get { return mDirty; }
        set { mDirty = value; }
    }

    public void ResetLifeTime()
    {
        mLeftLifeTime = mLifeTime;
    }

    void Start()
    {
        mRenderers = gameObject.GetComponentsInChildren<Renderer>(true);

        ParticleSystem[] psArray = gameObject.GetComponentsInChildren<ParticleSystem>(true) as ParticleSystem[];

        if (psArray == null || psArray.Length <= 0)
        {
            return;
        }

        mScaleInfoArray = new ScaleInfo[psArray.Length];
        for (int i = 0; i < psArray.Length; ++i)
        {
            mScaleInfoArray[i] = new ScaleInfo();
            mScaleInfoArray[i].ps = psArray[i];
            mScaleInfoArray[i].mOriginalScale = psArray[i].startSize;
            if(mLifeTime < psArray[i].duration)
            {
                mLifeTime = psArray[i].duration;
            }
        }
        mLeftLifeTime = mLifeTime;
        SetSacle();
    }

    // Update is called once per frame
    void Update()
    {
        if (mOldSize != mSize)
        {
            mDirty = true;
            mOldSize = mSize;
        }
        if (renderqueue != oldRenderqueue)
        {
            mDirty = true;
            oldRenderqueue = renderqueue;
        }

        if (mDirty)
        {
            mDirty = false; 
            SetSacle();
        }

        if(mAutoHide)
        {
            mLeftLifeTime -= Time.deltaTime;
            if(mLeftLifeTime <= 0)
            {
                mPlayCount--;
                if(mPlayCount > 0)
                {
                    ResetLifeTime();
                    SetSacle();
                }
                else
                {
                    if (this.mAutoHideCallback != null)
                    {
                        this.mAutoHideCallback();
                    }
                    gameObject.SetActive(false);
                }
            }
        }
    }

    void SetSacle()
    {
        if (gameObject == null)
        {
            return;
        }

        if (mScaleInfoArray != null)
        {
            for (int i = 0; i < mScaleInfoArray.Length; ++i)
            {
                if (mScaleInfoArray[i].ps != null)
                {
                    mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].mOriginalScale;
                    if (mScaleInfoArray[i].ps.gameObject.activeSelf)
                    {
                        //if (renderqueue >= 0)
                        //    mScaleInfoArray[i].ps.GetComponent<Renderer>().sortingOrder = renderqueue;
                    }
                    else
                    {
                        mDirty = true;
                    }

                }
            }

            for (int k = 0; k < mScaleInfoArray.Length; ++k)
            {
                ParticleSystem ps = mScaleInfoArray[k].ps;
                if (ps != null)
                {
                    ps.Clear(true);
                    ps.startSize = mScaleInfoArray[k].mOriginalScale * mSize;

                    ps.Play(true);
                }
            }
        }

        if(mRenderers != null && renderqueue >= 0)
        {
            for (int i = 0; i < mRenderers.Length; ++i)
            {
                mRenderers[i].sortingOrder = renderqueue;
            }
        }
        
    }

    void OnEnable()
    {
        if(mIsFirstEnabled)
        {
            mIsFirstEnabled = false;
        }
        else
        {
            SetSacle();
        }
        
    }

    void OnDisable()
    {
        mAutoHideCallback = null;
        if (gameObject == null)
        {
            return;
        }

        if (mScaleInfoArray != null)
        {
            for (int i = 0; i < mScaleInfoArray.Length; ++i)
            {
                if (mScaleInfoArray[i].ps != null)
                {
                    mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].mOriginalScale;
                }
            }
        }
    }
}
