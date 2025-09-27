using System;
using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GameImage : GameUIComponent
    {
        private Shadow mShadow;
        private ImageLoader mImgLoader;
        private GraphicRaycastRegister mRaycastRegister;
        
        #region 重写方法

        protected override void OnInit()
        {
            base.OnInit();
            if (Img != null)
            {
                mImgLoader = ImageLoader.GetOrAddImageLoader(Img);
                mRaycastRegister = new GraphicRaycastRegister();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            OnSpriteLoadDone = null;
            mImg = null;
            mImgLoader = null;
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
                case UIProperty.Image:
                    this.Sprite = (string)val;
                    break;
                case UIProperty.FillAmmount:
                    this.FillAmmount = Convert.ToSingle(val);
                    break;
                case UIProperty.Alpha:
                    UnityEngine.Color color = this.Color;
                    color.a = Convert.ToSingle(val);
                    this.Color = color;
                    break;
                case UIProperty.NativeSize:
                    this.NeedSetNativeSize = (bool)val;
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
                case UIProperty.Image:
                    ret = this.Sprite;
                    break;
                case UIProperty.FillAmmount:
                    ret = this.FillAmmount;
                    break;
                case UIProperty.Alpha:
                    ret = this.Color.a;
                    break;
                case UIProperty.NativeSize:
                    ret = this.NeedSetNativeSize;
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

        #region 特有公共方法
        
        public void SetGray(bool gray)
        {
            if (Img)
            {
                if (gray)
                {
                    Img.material = UITexture.GrayMaterial;
                }
                else
                {
                    Img.material = null;
                }
            }
        }
        
        public void SetActiveGraphic(bool isActiveGraphic)
        {
            if (Img != null)
            {
                if (mRaycastRegister == null) mRaycastRegister = new GraphicRaycastRegister();
                if(isActiveGraphic)
                {
                    mRaycastRegister.RegisterGraphicForCanvas(Img);
                }
                else
                {
                    mRaycastRegister.UnRegisterGraphicForCanvas(Img);
                }
            }
        }
        
        /// <summary>
        /// 清除脏数据，并重新渲染
        /// </summary>
        public void SetMaterialDirty()
        {
            if (Img == null) return;
            Img.SetMaterialDirty();
        }
        
        public void SetSpriteByURL(string url)
        {
            
        }

        public void SetColorWithHexStr(string hexStr)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            byte.TryParse(hexStr.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
            byte.TryParse(hexStr.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out g);
            byte.TryParse(hexStr.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out b);
            byte a = (byte)(Color.a * 255f);
            this.Color = new Color32(r, g, b, a);
        }

        #endregion

        private void OnLoadDone(string name, UnityEngine.Object obj)
        {
            if (NeedSetNativeSize)
            {
                if (Img != null)
                {
                    Img.SetNativeSize();
                }
            }

            OnSpriteLoadDone?.Invoke();
        }
        
        protected void CalcHandlePos(RectTransform rectTrans)
        {
            if(mFillHandle == null || Img == null)
            {
                return;
            }

            if(Img.type != UnityEngine.UI.Image.Type.Filled)
            {
                return;
            }

            Vector2 ancherdPos = rectTrans.anchoredPosition;
            switch(Img.fillMethod)
            {
                case UnityEngine.UI.Image.FillMethod.Horizontal:
                    switch ((UnityEngine.UI.Image.OriginHorizontal)Img.fillOrigin)
                    {
                        // 从左到右
                        case UnityEngine.UI.Image.OriginHorizontal.Left:
                            ancherdPos.x = Width * Img.fillAmount;
                            rectTrans.anchoredPosition = ancherdPos;
                            break;
                        // 从右到昨
                        case UnityEngine.UI.Image.OriginHorizontal.Right:
                            ancherdPos.x = Width * (1-Img.fillAmount);
                            rectTrans.anchoredPosition = ancherdPos;
                            break;
                        default:
                            break;
                    }
                    break;
                case UnityEngine.UI.Image.FillMethod.Vertical:
                    switch((UnityEngine.UI.Image.OriginVertical)Img.fillOrigin)
                    {
                        // 从上到下
                        case UnityEngine.UI.Image.OriginVertical.Bottom:
                            ancherdPos.y = Height * Img.fillAmount;
                            rectTrans.anchoredPosition = ancherdPos;
                            break;
                        // 从下到上
                        case UnityEngine.UI.Image.OriginVertical.Top:
                            ancherdPos.y = Height * (1-Img.fillAmount);
                            rectTrans.anchoredPosition = ancherdPos;
                            break;
                        default:
                            break;
                    }
                    break;
                // 90°内旋转
                case UnityEngine.UI.Image.FillMethod.Radial90:
                    rectTrans.localRotation = Quaternion.Euler(0, 0, 90 * Img.fillAmount);
                    break;
                // 180°内旋转
                case UnityEngine.UI.Image.FillMethod.Radial180:
                    rectTrans.localRotation = Quaternion.Euler(0, 0, 180 * Img.fillAmount);
                    break;
                // 360°内旋转
                case UnityEngine.UI.Image.FillMethod.Radial360:
                    rectTrans.localRotation = Quaternion.Euler(0, 0, 360 * Img.fillAmount);
                    break;
                default:
                    break;
            }
        }
        
        
        /// <summary>
        /// 加载Sprite完成回调
        /// </summary>
        public System.Action OnSpriteLoadDone;

        
        private Image mImg;
        public Image Img
        {
            get
            {
                if (gameObject)
                {
                    if (this.mImg) return this.mImg;
                    this.mImg = gameObject.GetComponent<Image>();
                    if (!this.mImg)
                    {
                        ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Image 失败, Image 为空!");
                    }
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Image 失败, gameObject 为空!");
                }

                return this.mImg;
            }
        }
        

        private string mSprite;
        public string Sprite
        {
            get
            {
                if (this.Img)
                {
                    return this.Img.sprite ? this.Img.sprite.name : string.Empty;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Sprite 失败, Image 为空!");
                    return string.Empty;
                }
            }
            set
            {
                if (this.mImgLoader)
                {
                    if (this.mImgLoader.ImageName.Equals(value)) return;
                    mImgLoader.ImageName = value;
                    mImgLoader.Loaded = OnLoadDone;
                    mImgLoader.Reload();
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Sprite 失败, Image 为空!");
                }
            }
        }

        public bool Enable
        {
            get => this.Img != null && this.Img.enabled;
            set
            {
                if (this.Img != null)
                {
                    this.Img.enabled = value;
                    this.SetActiveGraphic(value);
                    // OnEnableStatusChanged();
                }
            }
        }
        
        public UnityEngine.Color Color
        {
            get
            {
                if (Img)
                {
                    return Img.color;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - Color 失败, Image 为空!");
                }

                return Color.black;
            }
            set
            {
                if (Img)
                {
                    Img.color = value;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - Color 失败, Image 为空!");
                }
            }
        }

        public float FillAmmount
        {
            get
            {
                if(Img)
                {
                    return Img.fillAmount;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - FillAmmount 失败, Image 为空!");
                }

                return 0;
            }
            set
            {
                if(Img)
                {
                    Img.fillAmount = value;
                    if (mFillHandle == null)
                    {
                        var handlerObj = gameObject.GetGameObjectByID("Handle");
                        if (handlerObj) mFillHandle = handlerObj.GetComponent<RectTransform>();
                    }
                    CalcHandlePos(mFillHandle);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - FillAmmount 失败, Image 为空!");
                }
            }
        }

        private RectTransform mFillHandle;

        /// <summary>
        /// 设置原图大小
        /// </summary>
        public bool NeedSetNativeSize
        {
            get
            {
                if (Img)
                {
                    return mIsNeedSetNativeSize;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - NeedSetNativeSize 失败, Image 为空!");
                }

                return false;
            }
            set
            {
                
                if (Img)
                {
                    this.mIsNeedSetNativeSize = value;
                    if (this.mIsNeedSetNativeSize) this.Img.SetNativeSize();
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - NeedSetNativeSize 失败, Image 为空!");
                }
            }
        }
        private bool mIsNeedSetNativeSize;
        
        
        public bool RaycastTargetEnabled
        {
            get
            {
                if(Img != null)
                {
                    return Img.raycastTarget;
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("GetProperty - RaycastTargetEnabled 失败, Image 为空!");
                }
                
                return false;
            }
            set
            {
                if (Img != null)
                {
                    Img.raycastTarget = value;
                    this.SetActiveGraphic(value);
                }
                else
                {
                    ExceptionHelper.ThrowExceptionToBroadcast("SetProperty - RaycastTargetEnabled 失败, Image 为空!");
                }
            }
        }
        
        /// <summary>
        /// 加载优先级
        /// </summary>
        public int LoadPriority
        {
            set
            {
                this.mPriority = (MUEngine.LoadPriority)value;
                if (mImgLoader != null)
                {
                    mImgLoader.Priority = this.mPriority;
                }
            }
        }
        private MUEngine.LoadPriority mPriority = MUEngine.LoadPriority.Prior;
        
    }
}