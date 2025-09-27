using UnityEngine;

[ExecuteInEditMode]
public class RenderQueueComponent : MonoBehaviour
{
    public int mRenderQueue = -1;
   
    public int RenderQueue
    {
        get
        {
            return mRenderQueue;
        }
        set
        {
            if (value != mRenderQueue)
            {
                mRenderQueue = value;
                UpdateQueue();
            }
        }
    }

    private Renderer[] mRenderers = null;

    void Start()
    {
        mRenderers = gameObject.GetComponentsInChildren<Renderer>(true);
        UpdateQueue();
    }

    private void UpdateQueue()
    {
        if (mRenderers != null && mRenderQueue >= 0)
        {
            for (int i = 0; i < mRenderers.Length; ++i)
            {
                mRenderers[i].sortingOrder = mRenderQueue;
            }
        }
    }
}

