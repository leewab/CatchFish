using MUEngine;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Image控件对应的图片加载器
    /// </summary>
    public class ImageLoader : BaseImageLoader
    {
        /// <summary>
        /// 对应的Image控件
        /// </summary>
        public Image UIImage { get { return _uiImage; } set { _uiImage = value; } }
        
        [SerializeField] private Image _uiImage;

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
        /// 加载上来的图片名称
        /// </summary>
        private string loadedImageName;

        /// <summary>
        /// 加载上来的Texture2D
        /// </summary>
        private Texture2D loadedTexture2D;

        /// <summary>
        /// 加载上来的Sprite
        /// </summary>
        private Sprite loadedSprite;

        /// <summary>
        /// 是否正在加载
        /// </summary>
        private bool isLoading = false;

        /// <summary>
        /// 获取对应的图片加载器，如不存在则创建一个并返回
        /// </summary>
        /// <param name="image">对应的Image控件</param>
        /// <returns>对应的图片加载器</returns>
        public static ImageLoader GetOrAddImageLoader(Image image)
        {
            if (image == null)
            {
                return null;
            }

            ImageLoader imgLoader = image.gameObject.GetComponent<ImageLoader>();
            if (imgLoader == null)
            {
                imgLoader = image.gameObject.AddComponent<ImageLoader>();
                imgLoader.UIImage = image;
            }
            return imgLoader;
        }

        protected override MaskableGraphic OnGetUsingGraphic()
        {
            return this.UIImage;
        }

        public override void Reload()
        {
            if (isLoading || string.IsNullOrEmpty(ImageName) || UIImage == null)
            {
                return;
            }

            //如果已加载图片的名称与当前需要加载的资源名称相同，则不再重复加载
            if (loadedImageName == ImageName)
            {
                return;
            }

            isLoading = true;
            MURoot.ResMgr.GetAsset(ImageName, AssetLoadedHandler, Priority, ECacheType.AutoDestroy);
            ImageLoaderLogger.Register(this, ImageName);
        }

        private void AssetLoadedHandler(string name, UnityEngine.Object obj)
        {
            isLoading = false;

            if (obj is Sprite)
            {
                loadedImageName = name;
                loadedSprite = obj as Sprite;
                loadedTexture2D = null;

                UIImage.sprite = loadedSprite;
                RaiseLoaded(name, obj);
            }
            else if (obj is Texture2D)
            {
                loadedImageName = name;
                loadedTexture2D = obj as Texture2D;
                loadedSprite = null;

                UIImage.sprite = Sprite.Create(loadedTexture2D, new Rect(0, 0, loadedTexture2D.width, loadedTexture2D.height), new Vector2(0.5F, 0.5F));
                RaiseLoaded(name, obj);
            }
            else
            {
                MURoot.ResMgr.ReleaseAsset(name, obj);
                MUUtility.Warning("加载Image的贴图 [{0}] 时得到了其它类型的对象。", name);
                ImageLoaderLogger.Unregister(this, name);
            }
        }

        public override void Release()
        {
            if (UIImage != null && UIImage.sprite != null)
            {
                UIImage.sprite = DefaultSprite;
            }

            if (isLoading)
            {
                MURoot.ResMgr.ReleaseAssetCallback(ImageName, AssetLoadedHandler);
                isLoading = false;
                ImageLoaderLogger.Unregister(this, ImageName);
            }
            else
            {
                if (!string.IsNullOrEmpty(loadedImageName))
                {
                    if (loadedSprite != null)
                    {
                        MURoot.ResMgr.ReleaseAsset(loadedImageName, loadedSprite);
                        ImageLoaderLogger.Unregister(this, loadedImageName);
                    }
                    else if (loadedTexture2D != null)
                    {
                        MURoot.ResMgr.ReleaseAsset(loadedImageName, loadedTexture2D);
                        ImageLoaderLogger.Unregister(this, loadedImageName);
                    }
                    loadedSprite = null;
                    loadedTexture2D = null;
                    loadedImageName = null;
                }
            }
        }
        
    }
}
