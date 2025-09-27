using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MUEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// 按钮图片加载器，用于加载按钮的按下图片
    /// </summary>
    public class ButtonLoader : BaseImageLoader
    {
        /// <summary>
        /// 对应的UI按钮
        /// </summary>
        public Button UIButton
        {
            get { return _uiButton; }
            set { _uiButton = value; }
        }
        [SerializeField]
        private Button _uiButton;

        /// <summary>
        /// 按钮按下的图片名称
        /// </summary>
        public string PressedSpriteName
        {
            get { return _pressedSpriteName; }
            set { _pressedSpriteName = value; }
        }
        [SerializeField]
        private string _pressedSpriteName;

        /// <summary>
        /// 加载上来的Sprite
        /// </summary>
        private Sprite loadedSprite;

        /// <summary>
        /// 加载上来的Texture2D
        /// </summary>
        private Texture2D loadedTexture2D;

        /// <summary>
        /// 是否正在加载
        /// </summary>
        private bool isLoading = false;

        public override void Reload()
        {
            //按钮的按下图片不会被代码控制动态更改，因此只要加载上来便不再执行再次加载
            if (isLoading || loadedSprite != null || loadedTexture2D != null || UIButton == null || string.IsNullOrEmpty(PressedSpriteName))
            {
                return;
            }

            isLoading = true;
            MURoot.ResMgr.GetAsset(PressedSpriteName, AssetLoadedHandler, Priority, ECacheType.AutoDestroy);
            ImageLoaderLogger.Register(this, PressedSpriteName);
        }

        private void AssetLoadedHandler(string name, UnityEngine.Object obj)
        {
            isLoading = false;

            if (obj is Sprite)
            {
                loadedSprite = obj as Sprite;
                loadedTexture2D = null;

                var state = UIButton.spriteState;
                state.pressedSprite = loadedSprite;
                UIButton.spriteState = state;

                RaiseLoaded(name, obj);
            }
            else if (obj is Texture2D)
            {
                loadedTexture2D = obj as Texture2D;
                loadedSprite = null;

                var state = UIButton.spriteState;
                state.pressedSprite = Sprite.Create(loadedTexture2D, new Rect(0, 0, loadedTexture2D.width, loadedTexture2D.height), new Vector2(0.5F, 0.5F));
                UIButton.spriteState = state;

                RaiseLoaded(name, obj);
            }
            else
            {
                MURoot.ResMgr.ReleaseAsset(name, obj);
                MUUtility.Warning("加载按钮图片 [{0}] 时得到了其它类型的对象。", name);
                ImageLoaderLogger.Unregister(this, name);
            }
        }

        public override void Release()
        {
            if (UIButton != null)
            {
                var state = UIButton.spriteState;
                state.pressedSprite = null;
                UIButton.spriteState = state;
            }

            if (isLoading)
            {
                MURoot.ResMgr.ReleaseAssetCallback(PressedSpriteName, AssetLoadedHandler);
                isLoading = false;
                ImageLoaderLogger.Unregister(this, PressedSpriteName);
            }
            else 
            {
                if (loadedSprite != null)
                {
                    MURoot.ResMgr.ReleaseAsset(PressedSpriteName, loadedSprite);
                    ImageLoaderLogger.Unregister(this, PressedSpriteName);
                }
                else if (loadedTexture2D != null)
                {
                    MURoot.ResMgr.ReleaseAsset(PressedSpriteName, loadedTexture2D);
                    ImageLoaderLogger.Unregister(this, PressedSpriteName);
                }

                loadedSprite = null;
                loadedTexture2D = null;
            }
        }

    }
}
