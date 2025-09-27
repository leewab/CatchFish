using System;

namespace Game.UI
{
    public class GameProgressBar : GameUIComponent
    {
        protected UISliderBase mSlider;
        
        /// <summary>
        /// 是否使用Image制作的Slider
        /// </summary>
        public bool IsSliderImg = true;

        /// <summary>
        /// Slider值发生变化
        /// </summary>
        public Action<float> OnValueChanged;

        /// <summary>
        /// Slider Tween动画完成
        /// </summary>
        public Action OnTweenFinished
        {
            get
            {
                if (this.mSlider == null) return null;
                return this.mSlider.OnTweenFinished;
            }
            set
            {
                if (this.mSlider == null) return;
                this.mSlider.OnTweenFinished = value;
            }
        }
        
        public bool Enable
        {
            set => this.mSlider.Enable = value;
            get => this.mSlider.Enable;
        }
        
        public bool DoIncrease
        {
            set
            {
                if (this.mSlider != null)
                {
                    this.mSlider.ControlIncrease = value;
                }
            }
        }

        public bool DoDecrease
        {
            set
            {
                if (this.mSlider != null)
                {
                    this.mSlider.ControlDecrease = value;
                }
            }
        }

        public float SliderSpeedPerSecond
        {
            set
            {
                if (this.mSlider != null)
                {
                    this.mSlider.SliderSpeedPerSecond = value;
                }
            }
        }
        
        protected override void OnInit()
        {
            base.OnInit();
            if (IsSliderImg)
            {
                this.mSlider = GetComponent<UISliderImg>();
            }
            else
            {
                this.mSlider = GetComponent<UISliderMono>();
            }
            
            if (this.mSlider)
            {
                this.mSlider.OnTweenFinished = OnTweenDone;
                this.mSlider.OnSliderValueChange.AddListener(ValueChangeHandler);
            }
        }
        
        public override void Dispose()
        {
            base.Dispose();
            OnValueChanged = null;
            OnTweenFinished = null;
        }

        

        public void SetValueTween(float val)
        {
            if (this.mSlider != null)
            {
                this.mSlider.TargetValue = val;
            }
        }

        public void AddTweenSliderMono()
        {
            if (this.mSlider == null)
            {
                this.mSlider = gameObject.AddComponent<UISliderMono>();
                if (this.mSlider != null)
                {
                    this.mSlider.OnTweenFinished = OnTweenDone;
                }
            }
        }
        
        public void AddTweenSliderImg()
        {
            if (this.mSlider == null)
            {
                this.mSlider = gameObject.AddComponent<UISliderImg>();
                if (this.mSlider != null)
                {
                    this.mSlider.OnTweenFinished = OnTweenDone;
                }
            }
        }

        public void ClearTweenCurrValue()
        {
            if (this.mSlider != null)
            {
                this.mSlider.ClearCurrValue();
            }
        }

        public virtual float Value
        {
            get
            {
                if (this.mSlider != null)
                {
                    return this.mSlider.CurrValue;
                }

                return 0;
            }
            set
            {
                if (float.IsNaN(value))
                {
                    value = 0;
                }

                if (this.mSlider != null)
                {
                    this.mSlider.SetValueDirectly(value);
                }
            }
        }

        private void ValueChangeHandler(float val)
        {
            if (OnValueChanged != null) OnValueChanged(val);
        }
        
        public void ClearValueChangeHandler()
        {
            OnValueChanged = null;
        }

        public void AddValueChangeHandler(System.Action<float> handler)
        {
            OnValueChanged -= handler;
            OnValueChanged += handler;
        }
        
        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = true;
            if (key == UIProperty.ProgressValue)
            {
                this.Value = Convert.ToSingle(val);
            }
            else
            {
                succ = base.SetPropertyImpl(key, val);
            }
            
            return succ;
        }
        
        protected override bool GetPropertyImpl(UIProperty key, ref object ret)
        {
            bool succ = true;
            if (key == UIProperty.ProgressValue)
            {
                ret = Value;
            }
            else
            {
                succ = base.GetPropertyImpl(key, ref ret);
            }
            
            return succ;
        }

        private void OnTweenDone()
        {
            if (OnTweenFinished != null)
            {
                OnTweenFinished();
            }
        }

    }
}
