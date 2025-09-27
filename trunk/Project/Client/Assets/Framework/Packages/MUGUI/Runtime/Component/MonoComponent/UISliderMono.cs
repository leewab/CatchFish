namespace Game.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    [RequireComponent(typeof(Slider))]
    public class UISliderMono : UISliderBase
    {
        private Slider mSlider;

        public override UnityEvent<float> OnSliderValueChange
        {
            get
            {
                if (this.mSlider == null) this.mSlider = GetComponent<Slider>();
                return this.mSlider.onValueChanged;
            }
        }

        public override float CurrValue
        {
            set
            {
                this.mCurrValue = value;
                if (this.mSlider) this.mSlider.value = this.mCurrValue;
            }
            get { return this.mCurrValue; }
        }

        public override bool Enable
        {
            get { return this.mSlider && this.mSlider.enabled; }
            set
            {
                if (this.mSlider == null && this.mEnable == value) return;
                this.mEnable = value;
                this.mSlider.enabled = this.mEnable;
            }
        }

        public override bool Visible
        {
            get { return this.mSlider && this.mSlider.gameObject.activeSelf; }
            set
            {
                if (this.mSlider == null || this.mVisible == value) return;
                this.mVisible = value;
                this.mSlider.gameObject.SetActive(this.mVisible);
            }
        }

        private void Start()
        {
            this.mSlider = this.GetComponent<Slider>();
            this.ClearCurrValue();
        }

        /// <summary>
        /// 直接设置数据
        /// </summary>
        /// <param name="val"></param>
        public void SetValueDirectly(float val)
        {
            if (mSlider == null) mSlider = GetComponent<Slider>();
            this.TargetValue = Mathf.Clamp01(val);
            this.CurrValue = this.TargetValue;
            TweenDone();
        }

    }

}