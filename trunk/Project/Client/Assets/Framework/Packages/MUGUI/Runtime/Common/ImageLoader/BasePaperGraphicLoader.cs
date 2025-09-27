using UnityEngine;
using UnityEngine.UI;
using MUEngine;

namespace Game.UI
{
    public class BasePaperGraphicLoader : BaseImageLoader
    {
        [SerializeField]
        protected BasePaperGraphic _paperGraphic;
        [SerializeField]
        private string _frontTextureName;

        private Texture loadedFrontTexture;
        private string loadedFrontTextureName;
        private bool isFrontTextureLoading = false;

        public BasePaperGraphic PaperGraphic
        {
            get
            {
                return _paperGraphic;
            }
            set
            {
                _paperGraphic = value;
            }
        }

        public string FrontTextureName
        {
            get
            {
                return _frontTextureName;
            }
            set
            {
                if(_frontTextureName != value)
                {
                    ReleaseFrontTexture();
                    _frontTextureName = value;
                }
            }
        }
        
        public static BasePaperGraphicLoader GetOrAddRollPaperGraphicLoader(BasePaperGraphic paperGraphic)
        {
            if (paperGraphic == null)
            {
                return null;
            }
            BasePaperGraphicLoader paperGraphicLoader = paperGraphic.GetComponent<BasePaperGraphicLoader>();
            if (paperGraphicLoader == null)
            {
                paperGraphicLoader = paperGraphic.gameObject.AddComponent<BasePaperGraphicLoader>();
                paperGraphicLoader.PaperGraphic = paperGraphic;
            }
            return paperGraphicLoader;
        }

        protected override MaskableGraphic OnGetUsingGraphic()
        {
            return this.PaperGraphic;
        }

        public override void Reload()
        {
            ReloadFrontTexture();
        }

        public override void Release()
        {
            ReleaseFrontTexture();
        }

        private void ReloadFrontTexture()
        {
            if(isFrontTextureLoading || string.IsNullOrEmpty(FrontTextureName) || PaperGraphic == null)
            {
                return;
            }
            if(loadedFrontTexture != null && loadedFrontTextureName == FrontTextureName)
            {
                return;
            }

            isFrontTextureLoading = true;
            MURoot.ResMgr.GetAsset(FrontTextureName, OnFrontTextureLoaded, Priority, ECacheType.AutoDestroy);
            ImageLoaderLogger.Register(this, FrontTextureName);
        }
        private void OnFrontTextureLoaded(string name,UnityEngine.Object obj)
        {
            isFrontTextureLoading = false;

            Texture tex = obj as Texture;
            if(tex == null && obj is Sprite)
            {
                //目前BasePaperGraphic只能接收Texture类型的资源，Sprite的资源由于会进行打包，所以目前可能会有问题（在生成Mesh的时候，并没有进行UV的转换）
                //但是，目前有的应用场景，Editor工程会将Sprite赋值给对应的属性，如果能保证这种Sprite不会打包到图集，或者单个图一个图集，也许不会有什么问题？
                //这不是一个好的方案，正确的方案应该是 ： 1 目前，将BasePaperGraphic视为一种特殊的RawImage，只允许给它赋值真正的Texture
                //2 ： 之后可以可以更改BasePaperGraphic生成Mesh的代码，让它支持Sprite
                tex = (obj as Sprite).texture;
            }
            if(tex != null)
            {
                loadedFrontTexture = tex;
                loadedFrontTextureName = name;
                PaperGraphic.FrontImage = tex;

                RaiseLoaded(name, obj);
            }
            else
            {
                MURoot.ResMgr.ReleaseAsset(name, obj);
                UnityEngine.Debug.LogErrorFormat("加载PaperGraphic的FrontTexture : {0} 失败", name);
                ImageLoaderLogger.Unregister(this, name);
            }
        }
        private void ReleaseFrontTexture()
        {
            if(PaperGraphic != null && PaperGraphic.FrontImage != null)
            {
                PaperGraphic.FrontImage = DefaultTexture;
            }
            if (isFrontTextureLoading)
            {
                MURoot.ResMgr.ReleaseAssetCallback(FrontTextureName, OnFrontTextureLoaded);
                isFrontTextureLoading = false;
                ImageLoaderLogger.Unregister(this, FrontTextureName);
            }
            else
            {
                if(loadedFrontTexture != null)
                {
                    MURoot.ResMgr.ReleaseAsset(loadedFrontTextureName, loadedFrontTexture);
                    ImageLoaderLogger.Unregister(this, loadedFrontTextureName);
                    loadedFrontTexture = null;
                    loadedFrontTextureName = null;
                }
            }
        }

    }
}

