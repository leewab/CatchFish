using System;
using UnityEngine;
using UnityEngine.Events;

public class UISliderBase : MonoBehaviour
{
    public float SliderSpeedPerSecond = 0.2f;
    public bool ControlIncrease = true;
    public bool ControlDecrease = false;

    /// <summary>
    /// 回调完成
    /// </summary>
    public Action OnTweenFinished;

    /// <summary>
    /// slider变化回调
    /// </summary>
    private UnityEvent<float> mOnSliderValueChange;
    public virtual UnityEvent<float> OnSliderValueChange => mOnSliderValueChange;

    /// <summary>
    /// 目标值
    /// </summary>
    private float mTargetValue = 0f;

    public float TargetValue
    {
        set => mTargetValue = Mathf.Clamp01(value);
        get => mTargetValue;
    }

    /// <summary>
    /// 当前值
    /// </summary>
    protected float mCurrValue = 0;

    public virtual float CurrValue
    {
        set { this.mCurrValue = value; }
        get { return this.mCurrValue; }
    }

    protected bool mEnable;
    public virtual bool Enable
    {
        get
        {
            return true;
        }
        set
        {
            
        }
    }
    
    protected bool mVisible;
    public virtual bool Visible
    {
        get
        {
            return true;
        }
        set
        {
           
        }
    }
    
    public virtual float MinValue
    {
        get
        {
            return 0;
        }
        set
        {
            if (float.IsNaN(value))
            {
                value = 0;
            }
        }
    }

    public virtual float MaxValue
    {
        get
        {
            return 1;
        }
        set
        {
            if (float.IsNaN(value))
            {
                value = 0;
            }
        }
    }

    public void ClearCurrValue()
    {
        mCurrValue = 0;
    }

    /// <summary>
    /// 直接设置数据
    /// </summary>
    /// <param name="val"></param>
    public virtual void SetValueDirectly(float val)
    {
        
    }

    private void Update()
    {
        if (Mathf.Abs(mCurrValue - mTargetValue) < 0.00005f)
        {
            return;
        }

        float delta = Time.deltaTime;
        if (mCurrValue < mTargetValue)
        {
            if (ControlIncrease)
            {
                this.CurrValue += SliderSpeedPerSecond * delta;
                if (mCurrValue >= mTargetValue)
                {
                    this.CurrValue = this.TargetValue;
                    TweenDone();
                    return;
                }
            }
            else
            {
                this.CurrValue = this.TargetValue;
                TweenDone();
            }
        }
        else if (mCurrValue > mTargetValue)
        {
            if (ControlDecrease)
            {
                this.CurrValue -= SliderSpeedPerSecond * delta;
                if (mCurrValue <= mTargetValue)
                {
                    this.CurrValue = this.TargetValue;
                    TweenDone();
                    return;
                }
            }
            else
            {
                this.CurrValue = this.TargetValue;
                TweenDone();
            }
        }
    }

    protected void TweenDone()
    {
        if (OnTweenFinished != null)
        {
            OnTweenFinished();
        }
    }
}