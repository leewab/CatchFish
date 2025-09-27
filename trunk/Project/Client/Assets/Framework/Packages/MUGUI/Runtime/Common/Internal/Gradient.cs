using System.Collections.Generic;
using Game;
using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MUGUI
{
    /// <summary>
    /// 文本渐变色脚本
    /// </summary>
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color32 topColor = Color.white;

        [SerializeField]
        private Color32[] MidColor = { };

        [SerializeField]
        private Color32 bottomColor = Color.black;

        public enum GradientType
        {
            Top2Bottom,
            Right2Left,
            LeftTop2RightBottom,
            RightTop2LeftBottom,
        };
        [SerializeField]
        private GradientType type = GradientType.Top2Bottom;

        /// <summary>
        /// 仅在高配情况下启用
        /// </summary>
        [SerializeField]
        private bool onlyUsedInHighQuality = false;

        private class ColorPoint
        {
            public ColorPoint(Color32 mc, float mp)
            {
                color = mc;
                position = mp;
            }
            public void ChangePosition(float newPosi)
            {
                position = newPosi;
            }
            public Color32 color;
            public float position;
        }

        /// <summary>
        /// 渐变色是否启用
        /// </summary>
        private bool IsGradientEnabled
        {
#if !GAME_EDITOR
            get { return !onlyUsedInHighQuality || DeviceModule.Instance.DeviceType != MUEngine.QualityType.Low; }
#else
            get { return false; }
#endif
        }
        
        public void SetGradientColor(Color32 c1,Color32 c2)
        {
            topColor = c1;
            bottomColor = c2;

            if (IsGradientEnabled)
            {
                Text uiText = gameObject.GetComponent<Text>();
                if (uiText != null)
                {
                    uiText.color = Color.white;
                }

                if (graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
            else
            {
                //中低配情况下直接使用两种颜色的中间色作为文本的颜色
                Text uiText = gameObject.GetComponent<Text>();
                if (uiText != null)
                {
                    uiText.color = Color.Lerp(topColor, bottomColor, 0.5F);
                }
            }
        }


        /// <summary>
        /// 设置是否仅在高配情况下使用渐变色
        /// </summary>
        public void SetOnlyUseGradientInHighQuality(bool onlyUseInHigh)
        {
            this.onlyUsedInHighQuality = onlyUseInHigh;
        }

        List<UIVertex> vertexList = new List<UIVertex>();
        List<float> points = new List<float>();
        List<ColorPoint> colors = new List<ColorPoint>();

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || !IsGradientEnabled)
            {
                return;
            }
            vertexList.Clear();
            points.Clear();
            colors.Clear();

            vh.GetUIVertexStream(vertexList);

            int count = vertexList.Count;
            if (count > 0)
            {
                colors.Add(new ColorPoint(topColor, 0));
                if (MidColor.Length > 0)
                {
                    float para = MidColor.Length + 1;
                    for (int i = 0; i < MidColor.Length; i++)
                    {
                        colors.Add(new ColorPoint(MidColor[i], (i + 1) / para));
                    }
                }
                colors.Add(new ColorPoint(bottomColor, 1));

                float bottom = 0f;
                float top = 0f;
                if (type == GradientType.Top2Bottom)
                {
                    bottom = vertexList[0].position.y;
                    top = vertexList[0].position.y;
                    for (int i = 0; i < count; i++)
                    {
                        float p = vertexList[i].position.y;
                        if (p > top)
                        {
                            top = p;
                        }
                        else if (p < bottom)
                        {
                            bottom = p;
                        }
                        points.Insert(i, p);
                    }
                }
                else if (type == GradientType.Right2Left)
                {
                    bottom = vertexList[0].position.x;
                    top = vertexList[0].position.x;
                    for (int i = 0; i < count; i++)
                    {
                        float p = vertexList[i].position.x;
                        if (p > top)
                        {
                            top = p;
                        }
                        else if (p < bottom)
                        {
                            bottom = p;
                        }
                        points.Insert(i, p);
                    }
                }
                else if (type == GradientType.LeftTop2RightBottom)
                {
                    bottom = vertexList[0].position.y - vertexList[0].position.x;
                    top = vertexList[0].position.y - vertexList[0].position.x;
                    for (int i = 0; i < count; i++)
                    {
                        float p = vertexList[i].position.y - vertexList[i].position.x;
                        if (p > top)
                        {
                            top = p;
                        }
                        else if (p < bottom)
                        {
                            bottom = p;
                        }
                        points.Insert(i, p);
                    }
                }
                else if (type == GradientType.RightTop2LeftBottom)
                {
                    bottom = vertexList[0].position.x + vertexList[0].position.y;
                    top = vertexList[0].position.x + vertexList[0].position.y;
                    for (int i = 0; i < count; i++)
                    {
                        float p = vertexList[i].position.x + vertexList[i].position.y;
                        if (p > top)
                        {
                            top = p;
                        }
                        else if (p < bottom)
                        {
                            bottom = p;
                        }
                        points.Insert(i, p);
                    }
                }

                float uiElementHeight = top - bottom;

                for (int i = 0; i < colors.Count - 1; i++)
                {
                    colors[i].ChangePosition(top - (colors[i].position * uiElementHeight));
                }
                colors[colors.Count - 1].ChangePosition(bottom);

                float realHeight = uiElementHeight / (colors.Count - 1);
                for (int i = 0; i < count; i++)
                {
                    Color32 tc = topColor;
                    Color32 bc = bottomColor;
                    float newbtm = bottom;
                    for (int j = 0; j < colors.Count - 1; j++)
                    {
                        if (points[i] <= colors[j].position && points[i] >= colors[j + 1].position)
                        {
                            tc = colors[j].color;
                            bc = colors[j + 1].color;
                            newbtm = colors[j + 1].position;
                        }
                    }

                    UIVertex uiVertex = vertexList[i];
                    Color o = uiVertex.color;
                    Color32 b = new Color32((byte)(bc.r * o.r), (byte)(bc.g * o.g), (byte)(bc.b * o.b), (byte)(bc.a * o.a));
                    Color32 t = new Color32((byte)(tc.r * o.r), (byte)(tc.g * o.g), (byte)(tc.b * o.b), (byte)(tc.a * o.a));

                    uiVertex.color = Color32.Lerp(b, t, (points[i] - newbtm) / realHeight);
                    vertexList[i] = uiVertex;
                }
                vh.Clear();
                vh.AddUIVertexTriangleStream(vertexList);
            }
        }

    }
}