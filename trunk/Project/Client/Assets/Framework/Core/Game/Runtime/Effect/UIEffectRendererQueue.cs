using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class UIEffectRendererQueue : MonoBehaviour
{
    public class ScaleInfo
    {
        public ParticleSystem ps = null;
        public float fStartSize = 1f;
        public float fStartSpeed = 1f;
    }
    private ScaleInfo[] mScaleInfoArray = null;
    
    bool mDirty = true;
    public int renderqueue = 3100;
    public float mSize = 0.2f;
    int oldRenderqueue = -1;

    
    public bool Dirty
    {
        get { return mDirty; }
        set { mDirty = value; }
    }

    void Start()
    {
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
            mScaleInfoArray[i].fStartSize = psArray[i].startSize;
            mScaleInfoArray[i].fStartSpeed = psArray[i].startSpeed;
        }
        SetScale();
    }
     
    // Update is called once per frame
    void Update()
    {
        if (renderqueue != oldRenderqueue)
        {
            mDirty = true;
            oldRenderqueue = renderqueue;
        }

        if (mDirty)
        {
            mDirty = false;
            SetScale();
        }
    }

    void SetScale()
    {
        if (gameObject == null || mScaleInfoArray == null)
        {
            return;
        }

        //for (int i = 0; i < mScaleInfoArray.Length; ++i)
        //{
        //    if (mScaleInfoArray[i].ps != null)
        //    {
        //        mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].fStartSize;
        //        mScaleInfoArray[i].ps.startSpeed = mScaleInfoArray[i].fStartSpeed;

        //        if (mScaleInfoArray[i].ps.gameObject.activeSelf)
        //        {
        //            if (renderqueue >= 0 && mScaleInfoArray[i].ps.renderer.sharedMaterial != null)
        //                mScaleInfoArray[i].ps.renderer.sharedMaterial.renderQueue = renderqueue;
        //        }
        //        else
        //            mDirty = true;

        //    }
        //}


        //for (int k = 0; k < mScaleInfoArray.Length; ++k)
        //{
        //    ParticleSystem ps = mScaleInfoArray[k].ps;
        //    if (ps != null)
        //    {
        //        ps.Clear(true);
        //        ps.startSize = mScaleInfoArray[k].fStartSize * mSize;
        //        ps.startSpeed = mScaleInfoArray[k].fStartSpeed * mSize;
        //        ps.Play(true);
        //    }
        //}
    }

    void OnEnable()
    {
        //SetScale();
    }

    void OnDisable()
    {
        if (gameObject == null)
        {
            return;
        }
        if (mScaleInfoArray != null)
        {
            //for (int i = 0; i < mScaleInfoArray.Length; ++i)
            //{
            //    if (mScaleInfoArray[i].ps != null)
            //    {
            //        mScaleInfoArray[i].ps.startSize = mScaleInfoArray[i].fStartSize;
            //        mScaleInfoArray[i].ps.startSpeed = mScaleInfoArray[i].fStartSpeed;
            //    }
            //}
        }
    }
}
