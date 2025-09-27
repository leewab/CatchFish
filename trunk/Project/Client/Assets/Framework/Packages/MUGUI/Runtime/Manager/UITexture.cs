using UnityEngine;

namespace Game.UI
{
    public class UITexture
    {
        /// <summary>
        /// 默认透明贴图
        /// </summary>
        public static Texture2D DefaultTexture
        {
            get
            {
                if (_defaultTexture == null)
                {
                    _defaultTexture = Resources.Load<Texture2D>("Gui/Share/TouMing");
                }
                return _defaultTexture;
            }
        }
        
        private static Texture2D _defaultTexture;

        
        /// <summary>
        /// 默认透明精灵
        /// </summary>
        public static Sprite DefaultSprite
        {
            get
            {
                if (_defaultSprite == null)
                {
                    _defaultSprite = Sprite.Create(DefaultTexture,
                        new Rect(0, 0, DefaultTexture.width, DefaultTexture.height), new Vector2(0.5f, 0.5f));
                }

                return _defaultSprite;
            }
        }
        
        private static Sprite _defaultSprite;


        /// <summary>
        /// 默认灰色材质球
        /// </summary>
        public static Material GrayMaterial
        {
            get
            {
                if (!mGrayMaterial)
                {
                    Shader grayShader = Shader.Find("UI/Gray");
                    mGrayMaterial = new Material(grayShader);
                }

                return mGrayMaterial;
            }
        }
        
        private static Material mGrayMaterial;

        
        public static Color GetColorWithHexStr(string hexStr, float alpha = 1)
        {
            byte r = 0;
            byte g = 0;
            byte b = 0;
            byte.TryParse(hexStr.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
            byte.TryParse(hexStr.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out g);
            byte.TryParse(hexStr.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out b);
            byte a = (byte)(alpha * 255f);
            return new Color32(r, g, b, a);
        }
        
        public static Font LoadFont(string fontName)
        {
            return null;
        }
        
        
    }
}