#if UNITY_TOLUA
using Game.Core;
using MUGUI;
using UnityEngine;

namespace Game
{
    public class LuaUIUtil
    {
        private static Material grayMaterial;

        protected static Material GrayMaterial
        {
            get
            {
                if (!grayMaterial)
                {
                    Shader grayShader = Shader.Find("UI/Gray");
                    grayMaterial = new Material(grayShader);
                }

                return grayMaterial;
            }
        }
        
        public static void UIAddTouch(GameObject obj, int id)
        {
            if (id >= 0)
            {
                ETCArea area = obj.GetComponent<ETCArea>();
                if (area == null)
                    area = obj.AddComponent<ETCArea>();
                area.id = id;
            }
            else
            {
                ETCArea area = obj.GetComponent<ETCArea>();
                if (area != null)
                    GameObject.DestroyImmediate(area);
            }
        }

        public static void SetColor(UnityEngine.UI.Graphic graphic, Color value)
        {
            if (graphic)
            {
                graphic.color = value;
            }
        }

        public static Color GetColor(UnityEngine.UI.Graphic graphic)
        {
            if (graphic) return graphic.color;
            return Color.clear;
        }
        
        public static void SetGradientColor(MUGUI.Gradient gradientComponent, Color c1, Color c2)
        {
            if (gradientComponent != null)
            {
                gradientComponent.SetGradientColor(c1, c2);
            }
        }
        
        public static void SetOutlineColor(UnityEngine.UI.Outline outlineComponent, Color c)
        {
            if (outlineComponent != null)
            {
                outlineComponent.effectColor = c;
            }
        }

        public static Color GetOutlineColor(UnityEngine.UI.Outline outlineComponent)
        {
            if (outlineComponent != null)
            {
                return outlineComponent.effectColor;
            }

            return new Color(0,0,0);
        }

        public static void SetShadowEffectColor(UnityEngine.UI.Shadow shadow, Color c)
        {
            if (shadow == null)
                return;
            shadow.effectColor = c;
        }
        
        public static void SetColorWithHexStr(UnityEngine.UI.Graphic graphic, string hexStr)
        {
            if (graphic)
            {
                //#3F3B36
                byte r = 0;
                byte g = 0;
                byte b = 0;
                byte.TryParse(hexStr.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
                byte.TryParse(hexStr.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out g);
                byte.TryParse(hexStr.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out b);
                byte a = 255;
                graphic.color = new Color32(r, g, b, a);
            }
        }
        
        public static void SetGray(UnityEngine.UI.Graphic graphic, bool value)
        {
            if (graphic)
            {
                ButtonExtend btnex = graphic.gameObject.GetComponent<MUGUI.ButtonExtend>();
                if (btnex != null)
                {
                    btnex.SetEnable(!value);
                    return;
                }
                if (value)
                    graphic.material = GrayMaterial;
                else
                    graphic.material = null;
            }
        }
        

        #region event listener

        private static int _ClickEventID = 1;
        private static LuaInterface.LuaFunction _ClickEventFunc = null;
        
        private static int _PressDownID = 1;
        private static LuaInterface.LuaFunction _PressDownFunc = null;

        public static int GetClickEventID()
        {
            //Debug.Log("Click Event ID = " + _ClickEventID);
            return _ClickEventID++;
        }

        public static void UIClickHandler(GameObject obj)
        {
            if (obj == null) return;
            if (_ClickEventFunc == null)
            {
                _ClickEventFunc = LuaClient.Instance.GetLuaFunction("OnClickUI");
            }

            int event_id = EventTriggerListener.Get(obj).event_id;
            if (_ClickEventFunc != null)
            {
                _ClickEventFunc.BeginPCall();
                _ClickEventFunc.Push(event_id);
                _ClickEventFunc.PCall();
                _ClickEventFunc.EndPCall();
            }
        }
        
        
        public static int GetPressDownID()
        {
            return _PressDownID++;
        }
        
        public static void OnPressDownCallBack(GameObject obj)
        {
            if (obj == null) return;
            if (_PressDownFunc == null)
            {
                _PressDownFunc = LuaClient.Instance.GetLuaFunction("OnPressDownCallBack");
            }

            int event_id = EventTriggerListener.Get(obj).event_id;
            if (_PressDownFunc != null)
            {
                _PressDownFunc.BeginPCall();
                _PressDownFunc.Push(event_id);
                _PressDownFunc.PCall();
                _PressDownFunc.EndPCall();
            }
        }
        
        #endregion
  
    }
}
#endif