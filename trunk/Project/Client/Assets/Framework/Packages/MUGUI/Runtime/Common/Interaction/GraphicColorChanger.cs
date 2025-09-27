using UnityEngine;

namespace Game.UI
{
    //该类用于在某些情况下（由Lua端决定）更改目标Graphic的Color值，该类目前只用来动态更改TreeItem的字体颜色
    //add by liujunjie in 2018/11/1
    [ExecuteInEditMode]
    public class GraphicColorChanger : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Graphic[] targetGraphics;
        [SerializeField]
        private Color[] colors;
        [SerializeField]
        private int currentColorIndex = 0;

#if UNITY_EDITOR
        private void Update()
        {
            if (targetGraphics != null && currentColorIndex >= 0 && currentColorIndex < colors.Length)
            {
                var targetColor = colors[currentColorIndex];
                foreach (var graphic in targetGraphics)
                {
                    if (graphic != null)
                    {
                        graphic.color = targetColor;
                    }
                }
            }
        }
#endif
        public void SetColorByIndex(int index)
        {
            currentColorIndex = index;
            if (targetGraphics != null && currentColorIndex >= 0 && currentColorIndex < colors.Length)
            {
                var targetColor = colors[currentColorIndex];
                foreach (var graphic in targetGraphics)
                {
                    if (graphic != null)
                    {
                        graphic.color = targetColor;
                    }
                }
            }
        }
    }
}
