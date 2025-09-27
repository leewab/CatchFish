using System;
using Game;
using MUEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// UI图片加载器基类
    /// </summary>
    public abstract class BaseImageLoader : MonoBehaviour
    {
        #region 静态通用

        /// <summary>
        /// 默认透明图片名称
        /// </summary>
        public const string DEFAULT_IMAGE_NAME = "TransparentImg.png";

        /// <summary>
        /// 默认透明贴图
        /// </summary>
        protected static Texture2D DefaultTexture
        {
            get
            {
                if (_defaultTexture == null)
                {
                    InitializeDefaultSpriteAndTexture();
                }
                return _defaultTexture;
            }
        }
        private static Texture2D _defaultTexture;

        /// <summary>
        /// 默认透明精灵
        /// </summary>
        protected static Sprite DefaultSprite
        {
            get
            {
                if (_defaultSprite == null)
                {
                    InitializeDefaultSpriteAndTexture();
                }
                return _defaultSprite;
            }
        }
        private static Sprite _defaultSprite;

        /// <summary>
        /// 初始化默认透明精灵和贴图
        /// </summary>
        private static void InitializeDefaultSpriteAndTexture()
        {
            Action<string, UnityEngine.Object> callback = (name, obj) =>
            {
                if (obj is Texture2D)
                {
                    _defaultTexture = obj as Texture2D;
                    _defaultSprite = Sprite.Create(DefaultTexture, new Rect(0, 0, DefaultTexture.width, DefaultTexture.height), new Vector2(0.5f, 0.5f));
                    _defaultSprite.name = "MUTransparentSprite";
                }
                else if (obj is Sprite)
                {
                    _defaultSprite = obj as Sprite;
                    _defaultTexture = _defaultSprite.texture;
                }
                else
                {
                    throw new Exception("The format of default transparent image is invalid.");
                }
            };

            MURoot.ResMgr.GetAsset(DEFAULT_IMAGE_NAME, callback, LoadPriority.MostPrior, ECacheType.Persistent);
        }

        #endregion
        
        #region 点击注册

        /// <summary>
        /// 是否需要注册画布点击
        /// </summary>
        public bool ShouldRegister
        {
            get { return _shouldRegister && usingGraphic != null && usingGraphic.raycastTarget; }
            set { _shouldRegister = value; }
        }
        [SerializeField]
        private bool _shouldRegister;

        /// <summary>
        /// 正在使用的图片
        /// </summary>
        private MaskableGraphic usingGraphic;

        /// <summary>
        /// 获取正在使用的图片
        /// </summary>
        /// <returns></returns>
        protected virtual MaskableGraphic OnGetUsingGraphic()
        {
            return null;
        }

        /// <summary>
        /// 注册画布点击
        /// </summary>
        private void RegisterGraphicForCanvas()
        {
            if (ShouldRegister && usingGraphic.enabled)
            {
                RaycastTargetRegistry.RegisterGraphicForCanvas(usingGraphic.canvas, usingGraphic);
            }
        }

        /// <summary>
        /// 取消画布点击的注册
        /// </summary>
        private void UnregisterGraphicForCanvas()
        {
            if (ShouldRegister)
            {
                RaycastTargetRegistry.UnregisterGraphicForCanvas(usingGraphic.canvas, usingGraphic);
            }
        }

        #endregion


        #region 资源加载

        /// <summary>
        /// 用于对外标识是否已经启用
        /// </summary>
        public bool IsEnable { get; private set; }

        /// <summary>
        /// 加载优先级
        /// </summary>
        public LoadPriority Priority { get { return _priority; } set { _priority = value; } }
        private LoadPriority _priority = LoadPriority.Prior;

        /// <summary>
        /// 资源加载完成的回调
        /// </summary>
        public Action<string, UnityEngine.Object> Loaded { get; set; }

        /// <summary>
        /// 重新加载
        /// </summary>
        public abstract void Reload();

        /// <summary>
        /// 释放资源
        /// </summary>
        public abstract void Release();

        /// <summary>
        /// 发起资源加载完毕回调
        /// </summary>
        /// <param name="name">资源名称</param>
        /// <param name="obj">资源对象</param>
        protected void RaiseLoaded(string name, UnityEngine.Object obj)
        {
            if (Loaded != null)
            {
                Loaded.Invoke(name, obj);
                Loaded = null;
            }
        }

        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            this.IsEnable = true;

            Reload();

            this.usingGraphic = OnGetUsingGraphic();
            RegisterGraphicForCanvas();
        }

        private void OnDisable()
        {
            this.IsEnable = false;

            UnregisterGraphicForCanvas();
        }

        private void OnTransformParentChanged()
        {
            RegisterGraphicForCanvas();
        }

        private void OnDestroy()
        {
            UnregisterGraphicForCanvas();
            Release();
        }

        #endregion

    }
}
