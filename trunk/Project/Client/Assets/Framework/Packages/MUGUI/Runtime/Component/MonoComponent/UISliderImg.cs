namespace Game.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class UISliderImg : UISliderBase
    {
        private Image mSlider;

        public override float CurrValue
        {
            set
            {
                this.mCurrValue = value;
                if (this.mSlider)
                {
                    this.mSlider.fillAmount = this.mCurrValue;
                    this.OnSliderValueChange?.Invoke(this.mCurrValue);
                }
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

        private Image.Type mImgFillType;

        public Image.Type ImgFillType
        {
            set
            {
                this.mImgFillType = value;
                if (this.mSlider != null) this.mSlider.type = this.mImgFillType;
            }
            get { return this.mImgFillType; }
        }

        private Image.FillMethod mImgFillMethod;

        public Image.FillMethod ImgFillMethod
        {
            set
            {
                this.mImgFillMethod = value;
                if (this.mSlider != null) this.mSlider.fillMethod = this.mImgFillMethod;
            }
            get { return this.mImgFillMethod; }
        }

        private void Start()
        {
            this.mSlider = this.GetComponent<Image>();
            this.ImgFillType = Image.Type.Filled;
            this.ImgFillMethod = Image.FillMethod.Horizontal;
            this.ClearCurrValue();
        }

        public void ClearCurrValue()
        {
            this.CurrValue = 0;
        }

        /// <summary>
        /// 直接设置数据
        /// </summary>
        /// <param name="val"></param>
        public override void SetValueDirectly(float val)
        {
            if (mSlider == null) mSlider = GetComponent<Image>();
            this.TargetValue = Mathf.Clamp01(val);
            this.CurrValue = this.TargetValue;
            TweenDone();
        }

    }

}