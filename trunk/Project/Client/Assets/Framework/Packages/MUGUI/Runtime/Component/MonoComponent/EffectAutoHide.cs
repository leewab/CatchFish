using UnityEngine;
using System;
using System.Collections;
[ExecuteInEditMode]
public class EffectAutoHide : MonoBehaviour
{
  
    public float mLifeTime = 0;
    public float mLeftLifeTime = 0;
    private Renderer[] mRenderers = null;
    public void ResetLifeTime()
    {
        mLeftLifeTime = mLifeTime;
    }

    void Start()
    {
        ParticleSystem[] psArray = gameObject.GetComponentsInChildren<ParticleSystem>(true) as ParticleSystem[];
        mLifeTime = 0;
        if (psArray == null || psArray.Length <= 0)
        {
            return;
        }
        for (int i = 0; i < psArray.Length; ++i)
        {
            if (mLifeTime < psArray[i].duration)
            {
                mLifeTime = psArray[i].duration;
            }
        }
        mLeftLifeTime = mLifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        mLeftLifeTime -= Time.deltaTime;
        if (mLeftLifeTime <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
