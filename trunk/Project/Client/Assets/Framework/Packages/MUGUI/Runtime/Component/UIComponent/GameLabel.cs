using System;
using Game.Core;
using MUGUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameLabel : GameUIComponent
    {
        private Color mStartedColor;
        private GraphicRaycastRegister mRaycastRegister;
        
        #region 重写方法

        protected override void OnInit()
        {
            base.OnInit();
            this.mStartedColor = this.Label.color;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            if (mLabel != null) mLabel.text = string.Empty;
        }

        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = base.SetPropertyImpl(key, val);
            if (succ) return true;
            succ = true;
            switch (key)
            {
                case UIProperty.Enable:
                    this.Enable = (bool)val;
                    break;
                case UIProperty.Gray:
                    this.SetGray((bool)val);
                    break;
                case UIProperty.Text:
                    this.Text = (string)val;
                    break;
                case UIProperty.FontSize:
                    this.FontSize = int.Parse(val.ToString());
                    break;
                case UIProperty.Alpha:
                    UnityEngine.Color color = this.Color;
                    color.a = Convert.ToSingle(val);
                    this.Color = color;
                    break;
                case UIProperty.Alignment:
                    this.mLabel.alignment = (TextAnchor)int.Parse(val.ToString());
                    break;
                default:
                    succ = false;
                    break;
            }

            return succ;
        }

        protected override bool GetPropertyImpl(UIProperty key, ref object ret)
        {
            bool succ = base.GetPropertyImpl(key, ref ret);
            if (succ) return true;
            succ = true;
            switch (key)
            {
                case UIProperty.Text:
                    ret = this.Text;
                    break;
                case UIProperty.FontSize:
                    ret = this.mLabel.fontSize;
                    break;
                case UIProperty.Alpha:
                    ret = this.Color.a;
                    break;
                case UIProperty.TxtLength:
                    ret = this.Text.Length;
                    break;
                case UIProperty.PreferredWidth:
                    if (!this.mLabel)
                        ret = 0;
                    else if (float.IsNaN(this.mLabel.preferredWidth))
                        ret = 0;
                    else
                        ret = this.mLabel.preferredWidth;
                    break;
                case UIProperty.PreferredHeight:
                    if (!this.mLabel)
                        ret = 0;
                    else if (float.IsNaN(this.mLabel.preferredHeight))
                        ret = 0;
                    else
                        ret = this.mLabel.preferredHeight;
                    break;
                default:
                    succ = false;
                    break;
            }

            if (!succ)
            {
                ExceptionHelper.ThrowExceptionToBroadcast($"GetProperty - 失败, {key} 不可读取!");
            }
            
            return succ;
        }

        #endregion

        #region 公共方法

        public void SetGray(bool gray)
        {
            if (this.Label)
            {
                if (gray)
                {
                    this.Label.color = Color.gray;
                }
                else
                {
                    this.Label.color = this.mStartedColor;
                }
            }
        }

        /// <summary>
        /// 设置颜色
        /// </summary>
        /// <param name="hexStr"></param>
        public void SetColorWithHexStr(string hexStr, float alpha = 1)
        {
            if (Math.Abs(alpha - 1) > float.NaN) alpha = this.Color.a;
            this.Color = UITexture.GetColorWithHexStr(hexStr, alpha);
        }

        /// <summary>
        /// 设置渐变色
        /// </summary>
        /// <param name="hexStr1"></param>
        /// <param name="hexStr2"></param>
        /// <param name="alpha1"></param>
        /// <param name="alpha2"></param>
        public void SetGradientColor(string hexStr1, string hexStr2, float alpha1 = 1, float alpha2 = 1)
        {
            if (Math.Abs(alpha1 - 1) > float.NaN) alpha1 = this.Color.a;
            if (Math.Abs(alpha2 - 1) > float.NaN) alpha2 = this.Color.a;
            Color color1 = UITexture.GetColorWithHexStr(hexStr1, alpha1);
            Color color2 = UITexture.GetColorWithHexStr(hexStr2, alpha2);
            SetGradientColor(color1, color2);
        }
        
        /// <summary>
        /// 设置边缘色
        /// </summary>
        /// <param name="hexStr"></param>
        /// <param name="alpha"></param>
        public void SetOutlineColor(string hexStr, float alpha = 1)
        {
            if (Math.Abs(alpha - 1) > float.NaN) alpha = this.Color.a;
            Color color = UITexture.GetColorWithHexStr(hexStr, alpha);
            SetOutlineColor(color);
        }
        
        /// <summary>
        /// 自定义画布事件的注册和卸载
        /// </summary>
        /// <param name="isActiveGraphic"></param>
        public void SetActiveGraphic(bool isActiveGraphic)
        {
            if (this.Label != null)
            {
                if (mRaycastRegister == null) mRaycastRegister = new GraphicRaycastRegister();
                if(isActiveGraphic)
                {
                    mRaycastRegister.RegisterGraphicForCanvas(this.Label);
                }
                else
                {
                    mRaycastRegister.UnRegisterGraphicForCanvas(this.Label);
                }
            }
        }

        /// <summary>
        /// 获取文本当前的高度
        /// </summary>
        /// <returns></returns>
        public float GetUpdatedLabelHeight()
        {
            if(!gameObject || gameObject.GetComponent<RectTransform>() == null)
            {
                return 0;
            }
            UnityEngine.UI.ContentSizeFitter conFitter = gameObject.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            if(conFitter != null)
            {
                conFitter.SetLayoutVertical();
            }
            return gameObject.GetComponent<RectTransform>().rect.height;
        }
        
        /// <summary>
        /// 获取文本当前的宽度
        /// </summary>
        /// <returns></returns>
        public float GetUpdatedLabelWidth()
        {
            if(!gameObject || gameObject.GetComponent<RectTransform>() == null)
            {
                return 0;
            }
            UnityEngine.UI.ContentSizeFitter conFitter = gameObject.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            if(conFitter != null)
            {
                conFitter.SetLayoutVertical();
            }
            return gameObject.GetComponent<RectTransform>().rect.width;
        }
        
        public void BeginRandomText(float duration, float from, float to)
        {
            TweenRandomText tweenComp = gameObject.GetComponent<TweenRandomText>();
            if (tweenComp == null)
            {
                tweenComp = gameObject.AddComponent<TweenRandomText>();
            }
            tweenComp.from = from;
            tweenComp.to = to;
            tweenComp.PlayRandomLoop(duration);
        }

        public void StopRandomText(float finish)
        {
            TweenRandomText tweenComp = gameObject.GetComponent<TweenRandomText>();
            if (tweenComp == null)
            {
                return;
            }
            tweenComp.StopRandom(finish);
        }
        
        // public void BeginTypeWriterShow(string str, int firstShow,float speed)
        // {
        //     UITypeWriter typeWriter = gameObject.GetComponent<UITypeWriter>();
        //     if (typeWriter == null)
        //     {
        //         typeWriter = gameObject.AddComponent<UITypeWriter>();
        //     }
        //     typeWriter.FirstShowCharCount = firstShow;
        //     typeWriter.ShowSpeed = speed;
        //     typeWriter.BeginShowText(str);
        // }
        //
        // public bool IsTypeWriterShowTextFinished()
        // {
        //     UITypeWriter typeWriter = gameObject.GetComponent<UITypeWriter>();
        //     if (typeWriter == null)
        //     {
        //         return true;
        //     }
        //     return typeWriter.IsShowTextFinished();
        // }
        //
        // public void FinishTypeWriterShowText()
        // {
        //     UITypeWriter typeWriter = gameObject.GetComponent<UITypeWriter>();
        //     if (typeWriter == null)
        //     {
        //         return;
        //     }
        //     typeWriter.FinishShowText();
        // }

        #endregion
        
        private void SetGradientColor(Color c1,Color c2)
        {
            if(GradientComponent != null)
            {
                GradientComponent.SetGradientColor(c1, c2);
            }
        }
        
        private void SetOutlineColor(Color c)
        {
            if (OutlineComponent != null)
            {
                OutlineComponent.effectColor = c;
            }
        }

        #region Property   

        protected Text mLabel;
        public virtual Text Label
        {
            get
            {
                if (gameObject != null && this.mLabel == null)
                {
                    this.mLabel = gameObject.GetComponent<Text>();
                    if (!this.mLabel) ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Label 失败, Text 为空!");
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Label 失败, gameObject 为空!");
                }

                return this.mLabel;
            }
        }
        
        public string Text
        {
            get
            {
                if (this.Label)
                {
                    return this.Label.text;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Text 失败, Label 为空!");
                }

                return null;
            }
            set
            {
                if (this.Label)
                {
                    this.Label.text = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Text 失败, Label 为空!");
                }
            }
        }
        
        public bool Enable
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.enabled;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Enable 失败, Label 为空!");
                }

                return false;
            } 
            set
            {
                if (this.Label)
                {
                    this.mLabel.enabled = value;
                    this.SetActiveGraphic(value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Enable 失败, Label 为空!");
                }
            }
        }
        
        public bool RaycastTargetEnabled
        {
            get
            {
                if(this.Label != null)
                {
                    return this.Label.raycastTarget;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - RaycastTargetEnabled 失败, Label 为空!");
                }
                
                return false;
            }
            set
            {
                if (this.Label != null)
                {
                    this.Label.raycastTarget = value;
                    this.SetActiveGraphic(value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - RaycastTargetEnabled 失败, Label 为空!");
                }
            }
        }
        
        public Color Color
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.color;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Color 失败, Label 为空!");
                    throw new Exception("GetProperty - Color 失败, Label 为空!");
                }
            }
            set
            {
                if (this.Label)
                {
                    this.mLabel.color = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Color 失败, Label 为空!");
                }
            }
        }
        
        public float Alpha
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.color.a;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Alpha 失败, Label 为空!");
                    throw new Exception("GetProperty - Alpha 失败, Label 为空!");
                }
            }
            set
            {
                if (this.Label)
                {
                    Color c = this.mLabel.color;
                    c.a = value;
                    this.mLabel.color = c;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Alpha 失败, Label 为空!");
                }
            }
        }
        
        public int CurrentLines
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.cachedTextGenerator.lineCount;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - CurrentLines 失败, Label 为空!");
                    throw new Exception("GetProperty - CurrentLines 失败, Label 为空!");
                }
            }
        }
        
        public float LineHeight
        {
            get
            {
                if (this.Label && this.mLabel.font)
                {
                    return this.mLabel.font.lineHeight;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - LineHeight 失败, Label 或者 Font 为空!");
                    throw new Exception("GetProperty - LineHeight 失败, Label 或者 Font 为空!");
                }
            }
        }
        
        public Font FontFile
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.font;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Font 失败, Label 为空!");
                    throw new Exception("GetProperty - Font 失败, Label 为空!");
                }
            }
            set
            {
                if (this.Label)
                {
                    this.mLabel.font = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Font 失败, Label 为空!");
                }
            }
        }
        
        public string FontName
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.font.name;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - FontName 失败, Label 为空!");
                    throw new Exception("GetProperty - FontName 失败, Label 为空!");
                }
            }
            set
            {
                if (this.Label)
                {
                    this.mLabel.font = UITexture.LoadFont(value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - FontName 失败, Label 为空!");
                }
            }
        }

        public int FontSize
        {
            get
            {
                if (this.Label)
                {
                    return this.mLabel.fontSize;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - FontSize 失败, Label 为空!");
                    throw new Exception("GetProperty - FontSize 失败, Label 为空!");
                }
            }
            set
            {
                if (this.Label)
                {
                    this.mLabel.fontSize = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - FontSize 失败, Label 为空!");
                }
            }
        }
        
        
        private MUGUI.Gradient GradientComponent
        {
            get
            {
                if (gameObject)
                {
                    if (mGradientComponent == null)
                    {
                        mGradientComponent = gameObject.GetComponent<MUGUI.Gradient>();
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - GradientComponent 失败, gameObject 为空!");
                }

                return mGradientComponent;
            }
        }

        private MUGUI.Gradient mGradientComponent;
        
        
        private UnityEngine.UI.Outline OutlineComponent
        {
            get
            {
                if (gameObject)
                {
                    if (mOutlineComponent == null)
                    {
                        mOutlineComponent = gameObject.GetComponent<UnityEngine.UI.Outline>();
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - OutlineComponent 失败, gameObject 为空!");
                }
                
                return mOutlineComponent;
            }
        }

        private UnityEngine.UI.Outline mOutlineComponent;

        #endregion
    }
}