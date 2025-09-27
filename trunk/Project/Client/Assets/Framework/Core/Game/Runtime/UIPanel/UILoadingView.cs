
namespace Game.UI
{
    public class UILoadingView : GamePanel
    {
        private GameLabel mTxtLoadingTips;
        private GameLabel mTxtLoadingValue;
        private GameImage mSliderValue;

        protected override void OnInit()
        {
            base.OnInit();
            this.mTxtLoadingTips = Make<GameLabel>("Txt_Loading_Content");
            this.mTxtLoadingValue = Make<GameLabel>("Txt_Loading_Value");
            this.mSliderValue = Make<GameImage>("Slider_Progress/Slider_Fill");
        }
        
        protected override void OnDispose()
        {
            base.OnDispose();
            this.mTxtLoadingTips = null;
            this.mTxtLoadingValue = null;
            this.mSliderValue = null;
        }
        
        protected override void OnShow()
        {
            base.OnShow();
            this.SetStartState();
        }

        protected override void OnHide()
        {
            base.OnHide();
            this.SetEndState();
        }

        public void Refresh(string tipsLabelStr, string loadLabelStr, float value)
        {
            float fillValue = value < 0 ? 0 : value;
            if (this.mTxtLoadingValue != null) this.mTxtLoadingValue.Text = tipsLabelStr ?? "";
            if (this.mTxtLoadingTips != null) this.mTxtLoadingTips.Text = loadLabelStr ?? "";
            if (this.mSliderValue != null) this.mSliderValue.FillAmmount = fillValue;
        }
        
        public void SetStartState()
        {
            if (this.mTxtLoadingValue != null) this.mTxtLoadingValue.Text = "0%";
            if (this.mTxtLoadingTips != null) this.mTxtLoadingTips.Text = "";
            if (this.mSliderValue != null) this.mSliderValue.FillAmmount = 0;
        }
        
        public void SetEndState()
        {
            if (this.mTxtLoadingValue != null) this.mTxtLoadingValue.Text = "100%";
            if (this.mTxtLoadingTips != null) this.mTxtLoadingTips.Text = "";
            if (this.mSliderValue != null) this.mSliderValue.FillAmmount = 1;
        }
        
    }
}