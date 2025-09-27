using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderValueParamByTime : MonoBehaviour {
    public float StartValue = 0f;
    public float EndValue = 1f;
    public string Key = "";
    public float Duration = 1f;

    private float mCurTime = 0f;
    private bool mComplete = false;
    private Material mMat = null;
	
	void Start () {
        Renderer ren = gameObject.GetComponent<Renderer>();
        if(ren != null)
            mMat = ren.material;
        UpdateValue();
    }
    void OnEnable()
    {
        mCurTime = 0f;
        mComplete = false;
        UpdateValue();
        Debug.Log("OnEnable");
    }
	// Update is called once per frame
	void Update () {
        if (mComplete)
            return;
        mCurTime += Time.deltaTime;
        if(mCurTime > Duration)
        {
            mCurTime = Duration;
            mComplete = true;
        }
        UpdateValue();
	}

    private void UpdateValue()
    {
        float mCurValue = StartValue + (EndValue - StartValue) * mCurTime / Duration;
        if (mMat != null)
        {
            mMat.SetFloat(Key, mCurValue);
        }
    }
}
