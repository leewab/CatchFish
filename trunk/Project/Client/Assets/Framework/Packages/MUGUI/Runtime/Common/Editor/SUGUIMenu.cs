using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace MUGUI
{
    /// <summary>
    /// 绘制顶级导航菜单
    /// </summary>
    static public class NGUIMenu
    {
        #region Tweens

        [MenuItem("MUGUI/Tween/Tween Alpha", false, 8)]
        static void Tween1() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenAlpha>(); }

        [MenuItem("MUGUI/Tween/Tween Alpha", true)]
        static bool Tween1a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<MaskableGraphic>() != null); }

        [MenuItem("MUGUI/Tween/Tween Color", false, 8)]
        static void Tween2() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenColor>(); }

        [MenuItem("MUGUI/Tween/Tween Color", true)]
        static bool Tween2a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<MaskableGraphic>() != null); }

        [MenuItem("MUGUI/Tween/Tween Width", false, 8)]
        static void Tween3() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenWidth>(); }

        [MenuItem("MUGUI/Tween/Tween Width", true)]
        static bool Tween3a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<RectTransform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Height", false, 8)]
        static void Tween4() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenHeight>(); }

        [MenuItem("MUGUI/Tween/Tween Height", true)]
        static bool Tween4a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<RectTransform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Position", false, 8)]
        static void Tween5() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenPosition>(); }

        [MenuItem("MUGUI/Tween/Tween Position", true)]
        static bool Tween5a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Rotation", false, 8)]
        static void Tween6() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenRotation>(); }

        [MenuItem("MUGUI/Tween/Tween Rotation", true)]
        static bool Tween6a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Scale", false, 8)]
        static void Tween7() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenScale>(); }

        [MenuItem("MUGUI/Tween/Tween Scale", true)]
        static bool Tween7a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Transform", false, 8)]
        static void Tween8() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenTransform>(); }

        [MenuItem("MUGUI/Tween/Tween Transform", true)]
        static bool Tween8a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Tween/Tween Volume", false, 8)]
        static void Tween9() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenVolume>(); }

        [MenuItem("MUGUI/Tween/Tween Volume", true)]
        static bool Tween9a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<AudioSource>() != null); }

        [MenuItem("MUGUI/Tween/Tween Text", false, 8)]
        static void Tween10() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenText>(); }

        [MenuItem("MUGUI/Tween/Tween Text", true)]
        static bool Tween10a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Text>() != null); }

        [MenuItem("MUGUI/Tween/Tween Slider", false, 8)]
        static void Tween11() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<TweenSlider>(); }

        [MenuItem("MUGUI/Tween/Tween Slider", true)]
        static bool Tween11a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Slider>() != null); }
        #endregion

        #region Interaction


        [MenuItem("MUGUI/Interaction/Button Offset", false, 9)]
        static void Interaction1() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<UIButtonOffset>(); }

        [MenuItem("MUGUI/Interaction/Button Offset", true)]
        static bool Interaction1a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Interaction/Button Rotation", false, 9)]
        static void Interaction2() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<UIButtonRotation>(); }

        [MenuItem("MUGUI/Interaction/Button Rotation", true)]
        static bool Interaction2a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Interaction/Button Scale", false, 9)]
        static void Interaction3() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<UIButtonScale>(); }

        [MenuItem("MUGUI/Interaction/Button Scale", true)]
        static bool Interaction3a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Interaction/Button Activate", false, 9)]
        static void Interaction4() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<UIButtonActivate>(); }

        [MenuItem("MUGUI/Interaction/Button Activate", true)]
        static bool Interaction4a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Transform>() != null); }

        [MenuItem("MUGUI/Interaction/Slider Colors", false, 9)]
        static void Interaction5() { if (Selection.activeGameObject != null) Selection.activeGameObject.AddMissingComponent<UISliderColors>(); }

        [MenuItem("MUGUI/Interaction/Slider Colors", true)]
        static bool Interaction5a() { return (Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<Slider>() != null); }
        #endregion

        [MenuItem("MUGUI/Help", false, 12)]
        static public void Help() { ShowHelp(); }

        /// <summary>
        /// 显示帮助
        /// </summary>
        static public void ShowHelp()
        {
            Application.OpenURL("http://www.xiaobao1993.com");
        }
    }
}