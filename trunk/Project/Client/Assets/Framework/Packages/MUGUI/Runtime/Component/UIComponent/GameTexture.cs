using UnityEngine;

namespace Game.UI
{
    public class GameTexture : GameUIComponent
    {
        protected UnityEngine.UI.RawImage tex;
        private RawImageLoader imgLoader;
        public bool NoCache = false;

        protected override void OnInit()
        {
            base.OnInit();
            tex = GetComponent<UnityEngine.UI.RawImage>();
            imgLoader = RawImageLoader.GetOrAddImageLoader(tex);
        }

        public string TextureName
        {
            get { return tex.texture.name; }
            set
            {
                imgLoader.ImageName = value;
                imgLoader.Reload();
            }
        }

        public Texture2D Texture
        {
            set
            {
                if (tex != null)
                {
                    tex.texture = value;
                }
            }
        }

        protected override bool SetPropertyImpl(UIProperty key, object val)
        {
            bool succ = true;
            switch (key)
            {
                case UIProperty.TextureName:
                    this.TextureName = (string)val;
                    break;
                case UIProperty.Texture:
                    this.Texture = (Texture2D)val;
                    break;
                case UIProperty.Material:
                    Material m = Resources.Load<Material>((string)val);
                    m.SetFloat("_Gamma", 2.2f);
                    tex.material = m;
                    break;
                default:
                    succ = base.SetPropertyImpl(key, val);
                    break;
            }
            
            return succ;
        }

    }
}
