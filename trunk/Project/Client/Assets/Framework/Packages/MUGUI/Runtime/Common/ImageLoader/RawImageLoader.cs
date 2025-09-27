#if UNITY_ANDROID   //临时性测试代码
//#define UI_VIDEO_TEST
#endif
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
    /// RawImage的图片资源加载器
    /// </summary>
    public class RawImageLoader : BaseImageLoader
    {
        /// <summary>
        /// 对应的RawImage控件
        /// </summary>
        public RawImage UIRawImage { 
            get { return _uiRawImage; }
            set { _uiRawImage = value; } 
        }
        [SerializeField]
        private RawImage _uiRawImage;

        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImageName
        {
            get { return _imageName; }
            set
            {
                if (_imageName != value)
                {
                    Release();
                    _imageName = value;
                }
            }
        }
        [SerializeField]
        private string _imageName;

        
        /// <summary>
        /// 加载上来的贴图
        /// </summary>
        private Texture loadedTexture;

        /// <summary>
        /// 加载上来的贴图的名称
        /// </summary>
        private string loadedTextureName;

        /// <summary>
        /// 是否正在加载
        /// </summary>
        private bool isLoading = false;

        public bool IsLoadedFinish
        {
            get
            {
                return loadedTexture != null;
            }
        }

        /// <summary>
        /// 获取对应的图片加载器，如不存在则创建一个并返回
        /// </summary>
        /// <param name="rawImage">对应的RawImage控件</param>
        /// <returns>对应的图片加载器</returns>
        public static RawImageLoader GetOrAddImageLoader(RawImage rawImage)
        {
            if (rawImage == null)
            {
                return null;
            }

            RawImageLoader imgLoader = rawImage.gameObject.GetComponent<RawImageLoader>();
            if (imgLoader == null)
            {
                imgLoader = rawImage.gameObject.AddComponent<RawImageLoader>();
                imgLoader.UIRawImage = rawImage;
            }
            return imgLoader;
        }

        protected override MaskableGraphic OnGetUsingGraphic()
        {
            return this.UIRawImage;
        }

        public override void Reload()
        {
            if (isLoading || string.IsNullOrEmpty(ImageName) || UIRawImage == null)
            {
                return;
            }

            if (loadedTexture != null && loadedTextureName == ImageName)
            {
                return;
            }

            isLoading = true;

#if UI_VIDEO_TEST
            Debug.LogError("try to load raw texture : " + ImageName);
#endif
            MURoot.ResMgr.GetAsset(ImageName, AssetLoadedHandler, Priority, ECacheType.AutoDestroy);
            ImageLoaderLogger.Register(this, ImageName);
        }


        private void AssetLoadedHandler(string name, UnityEngine.Object obj)
        {
#if UI_VIDEO_TEST
            Debug.LogError("load raw texture success : " + name);
#endif
            isLoading = false;

            Texture tex = obj as Texture;
            if (tex != null)
            {
                loadedTexture = tex;
                loadedTextureName = name;
                UIRawImage.texture = tex;

                RaiseLoaded(name, obj);
            }
            else
            {
                MURoot.ResMgr.ReleaseAsset(name, obj);
                MUUtility.Warning("加载RawImage的贴图 [{0}] 时得到了非Texture类型的对象。", name);
                ImageLoaderLogger.Unregister(this, name);
            }
        }

        public override void Release()
        {
            if (UIRawImage != null && UIRawImage.texture != null)
            {
                UIRawImage.texture = DefaultTexture;
            }

            if (isLoading)
            {
                MURoot.ResMgr.ReleaseAssetCallback(ImageName, AssetLoadedHandler);
                isLoading = false;
                ImageLoaderLogger.Unregister(this, ImageName);
            }
            else
            {
                if (loadedTexture != null)
                {
                    MURoot.ResMgr.ReleaseAsset(loadedTextureName, loadedTexture);
                    ImageLoaderLogger.Unregister(this, loadedTextureName);
                    loadedTexture = null;
                    loadedTextureName = null;
                }
            }
        }
        
    }
}
