using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MUEngine;
using MUGUI;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// AnimatedImage的图片资源加载器
    /// </summary>
    public class AnimatedImageLoader : BaseImageLoader
    {
        /// <summary>
        /// 对应的AnimatedImage控件
        /// </summary>
        public AnimatedImage UIAnimatedImage
        {
            get { return _uiAnimatedImage; } 
            set { _uiAnimatedImage = value; }
        }
        [SerializeField]
        private AnimatedImage _uiAnimatedImage;

        /// <summary>
        /// 图片名称数组
        /// </summary>
        public string[] ImageNames
        {
            get { return _imageNames; } 
            set { _imageNames = value; }
        }
        [SerializeField]
        private string[] _imageNames;

        /// <summary>
        /// 已加载的图片
        /// </summary>
        private Sprite[] LoadedImages
        {
            get
            {
                if (_loadedImages == null)
                {
                    _loadedImages = new Sprite[ImageNames == null ? 0 : ImageNames.Length];
                }
                return _loadedImages;
            }
        }
        private Sprite[] _loadedImages = null;

        private Action<string, UnityEngine.Object>[] LoadedCallbacks
        {
            get
            {
                if (_loadedCallbacks == null)
                {
                    _loadedCallbacks = new Action<string, UnityEngine.Object>[ImageNames == null ? 0 : ImageNames.Length];
                }
                return _loadedCallbacks;
            }
        }
        private Action<string, UnityEngine.Object>[] _loadedCallbacks = null;

        /// <summary>
        /// 是否正在加载。
        /// 以当前是否还有回调来作为判断依据
        /// </summary>
        private bool IsLoading
        {
            get
            {
                for (int i = 0; i < LoadedCallbacks.Length; i++)
                {
                    if (LoadedCallbacks[i] != null)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override void Reload()
        {
            if (UIAnimatedImage == null || ImageNames == null || ImageNames.Length == 0)
            {
                return;
            }

            //AnimatedImage不会被代码动态修改，因此已经加载完毕后不再重新加载
            if (IsLoading)
            {
                return;
            }

            for (int i = 0; i < ImageNames.Length; i++)
            {
                int index = i;  //为了使用匿名方法做闭包，这里额外定义一个变量
                Action<string, UnityEngine.Object> callback = null;
                callback = (name, obj) =>
                {
                    LoadedCallbacks[index] = null;

                    Sprite sprite = obj as Sprite;
                    if (sprite != null)
                    {
                        UIAnimatedImage.Sprites[index] = sprite;
                        LoadedImages[index] = sprite;
                    }
                    else
                    {
                        MURoot.ResMgr.ReleaseAsset(name, obj);
                        MUUtility.Warning("加载AnimatedImage中的图片 [{0}] 时得到了非Sprite类型的对象。", name);
                        ImageLoaderLogger.Unregister(this, name);
                    }

                    if (!IsLoading)
                    {
                        RaiseLoaded(string.Empty, null);
                    }
                };
                LoadedCallbacks[index] = callback;

                MURoot.ResMgr.GetAsset(ImageNames[index], callback, Priority, ECacheType.AutoDestroy);
                ImageLoaderLogger.Register(this, ImageNames[index]);
            }
        }

        public override void Release()
        {
            if (UIAnimatedImage != null)
            {
                if (UIAnimatedImage.sprite != null)
                {
                    UIAnimatedImage.sprite = DefaultSprite;
                }
                for (int i = 0; i < UIAnimatedImage.Sprites.Length; i++)
                {
                    UIAnimatedImage.Sprites[i] = null;
                }
            }

            //以缓存的回调是否为空来判断是否正在加载
            for (int i = 0; i < LoadedCallbacks.Length; i++)
            {
                var callback = LoadedCallbacks[i];
                if (callback != null)
                {
                    LoadedCallbacks[i] = null;
                    MURoot.ResMgr.ReleaseAssetCallback(ImageNames[i], callback);
                    ImageLoaderLogger.Unregister(this, ImageNames[i]);
                }
                else
                {
                    Sprite curSprite = LoadedImages[i];
                    if (curSprite != null)
                    {
                        LoadedImages[i] = null;
                        MURoot.ResMgr.ReleaseAsset(ImageNames[i], curSprite);
                        ImageLoaderLogger.Unregister(this, ImageNames[i]);
                    }
                }
            }
        }

    }

}
