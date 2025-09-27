using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.UI;
using UnityEngine.UI;
using MUEngine;

namespace Game.UI
{
    public class RollPaperGraphicLoader : BasePaperGraphicLoader
    {
        [SerializeField]
        private string _backTextureName;

        private Texture loadedBackTexture;
        private string loadedBackTextureName;
        private bool isBackTextureLoading = false;
        
        public RollPaperGraphic RollPaperGraphic
        {
            get
            {
                return _paperGraphic as RollPaperGraphic;
            }
            set
            {
                _paperGraphic = value;
            }
        }

        public string BackTextureName
        {
            get
            {
                return _backTextureName;
            }
            set
            {
                if (_backTextureName != value)
                {
                    ReleaseBackTexture();
                    _backTextureName = value;
                }
            }
        }

        public static RollPaperGraphicLoader GetOrAddRollPaperGraphicLoader(RollPaperGraphic paperGraphic)
        {
            if(paperGraphic == null)
            {
                return null;
            }
            RollPaperGraphicLoader paperGraphicLoader = paperGraphic.GetComponent<RollPaperGraphicLoader>();
            if(paperGraphicLoader == null)
            {
                paperGraphicLoader = paperGraphic.gameObject.AddComponent<RollPaperGraphicLoader>();
                paperGraphicLoader.PaperGraphic = paperGraphic;
            }
            return paperGraphicLoader;
        }

        public override void Reload()
        {
            base.Reload();
            ReloadBackTexture();
        }

        public override void Release()
        {
            base.Release();
            ReleaseBackTexture();
        }

        private void ReloadBackTexture()
        {
            if(isBackTextureLoading || string.IsNullOrEmpty(BackTextureName) || RollPaperGraphic == null)
            {
                return;
            }
            if(loadedBackTexture != null && loadedBackTextureName == BackTextureName)
            {
                return;
            }

            isBackTextureLoading = true;
            MURoot.ResMgr.GetAsset(BackTextureName, OnBackTextureLoaded, Priority, ECacheType.AutoDestroy);
            ImageLoaderLogger.Register(this, BackTextureName);
        }
        private void OnBackTextureLoaded(string name,UnityEngine.Object obj)
        {
            isBackTextureLoading = false;

            Texture tex = obj as Texture;
            if (tex == null && obj is Sprite)
            {
                //目前BasePaperGraphic只能接收Texture类型的资源，Sprite的资源由于会进行打包，所以目前可能会有问题（在生成Mesh的时候，并没有进行UV的转换）
                //但是，目前有的应用场景，Editor工程会将Sprite赋值给对应的属性，如果能保证这种Sprite不会打包到图集，或者单个图一个图集，也许不会有什么问题？
                //这不是一个好的方案，正确的方案应该是 ： 1 目前，将BasePaperGraphic视为一种特殊的RawImage，只允许给它赋值真正的Texture
                //2 ： 之后可以可以更改BasePaperGraphic生成Mesh的代码，让它支持Sprite
                tex = (obj as Sprite).texture;
            }
            if (tex != null)
            {
                loadedBackTexture = tex;
                loadedBackTextureName = name;
                RollPaperGraphic.BackImage = tex;

                RaiseLoaded(name, obj);
            }
            else
            {
                MURoot.ResMgr.ReleaseAsset(name, obj);
                UnityEngine.Debug.LogErrorFormat("加载RollPaperGraphic的BackTexture : {0} 失败", name);
                ImageLoaderLogger.Unregister(this, name);
            }
        }
        private void ReleaseBackTexture()
        {
            if(RollPaperGraphic != null && RollPaperGraphic.BackImage != null)
            {
                RollPaperGraphic.BackImage = DefaultTexture;
            }

            if (isBackTextureLoading)
            {
                MURoot.ResMgr.ReleaseAssetCallback(BackTextureName, OnBackTextureLoaded);
                isBackTextureLoading = false;
                ImageLoaderLogger.Unregister(this, BackTextureName);
            }
            else
            {
                if(loadedBackTexture != null)
                {
                    MURoot.ResMgr.ReleaseAsset(loadedBackTextureName, loadedBackTexture);
                    ImageLoaderLogger.Unregister(this, loadedBackTextureName);
                    loadedBackTexture = null;
                    loadedBackTextureName = null;
                }
            }
        }
        
    }
}

