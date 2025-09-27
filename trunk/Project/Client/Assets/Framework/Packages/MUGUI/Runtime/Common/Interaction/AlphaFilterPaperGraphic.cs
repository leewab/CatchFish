using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    //该类使用特定的shader，能对结果的alpha值进行一定的筛选和修改，使用RenderTexture时，有部分的Alpha值是应该被忽视的
    //这里并不能完全筛选中应该被忽视的alpha值，但是允许通过配置的方式来尝试找到一个最佳的筛选方式。
    //shader 没有继承的说法，但是继承该类的类，使用的shader 也应该包含该类所用shader中的必要代码，否则会导致该类的设置失效
    public class AlphaFilterPaperGraphic : BasePaperGraphic
    {
        //UI默认材质的Blend Mode 似乎都是 SrcAlpha OneMinusSrcAlpha ，颜色值和Alpha都会通过这种方式进行混合
        //所以如果把渲染结果作为一个RenderTexture，它的Alpha可能表示两种含义：（1）此处真正的透明度，（2）混合颜色的时候“无意”间产生，需要忽略的透明度
        //所以RenderTexture中的 Alpha 有时是需要用于混合的，有时有时需要忽略的。
        //想要将两者区分开来，主要有两种思路：（1）更改将要“拍摄”的UI元素渲染的方式，主要将它们的Alpha混合操作改为Max(对应的，RenderTexture的基础alpha应该为0)
        //（2）使用RenderTexture时，对Alpha进行一定的筛选。第一种方式更精确，但是代价比较大，第二种方法不够精确，但是代价较小
        //这里尝试使用第二种方式来实现，如果无法调节出满意的效果，则可能需要换用第一种方式（第一种方式的具体实现方式也有多种，需要进行选择）

        //鉴于UI排布的特点，可以将UI分为两大区域：透明区和非透明区。透明区的alpha值用于与其它显示元素进行混合（比如说半透明的黑色背景区，或者无任何UI元素的区域），
        //非透明区则是内部各个UI元素之间混合得到最终的颜色，它的颜色不会与其它显示元素混合（但是由于alpha的混合方式，它写到渲染目标中的alpha值并不是1）
        //在很多界面中，非透明区都会有一个大的不透明底图，这种情况下，此UI内的范围，alpha值都可以被忽略

        //是否开启透明模式
        [SerializeField]
        private bool isTransparent = true;
        //不透明区域的范围，x,y 表示左下角的u,v，z,w表示右上角的u,v
        [SerializeField]
        private Vector4 ignoreArea;
        //不透明区域的范围，用一个RectTransform表示，设置该值之后，会保证忽略范围始终与该RectTransform范围保持一致
        [SerializeField]
        private RectTransform ignoreRect;
        //用于渲染RenderTexture，只有这个值有合适值时，才能通过ignoreRect算出正确的ignoreArea
        [System.NonSerialized]
        public Camera renderCamera;
        //透明区域的alpha阈值，高于该值会被强制设置为1，低于该值会被保持原样
        [SerializeField]
        [Range(0, 1)]
        private float alphaThreashold = 0.1f; 

        public bool IsTransparent { get { return isTransparent; } set { if (SetPropertyUtility.SetStruct<bool>(ref isTransparent, value)) SetMaterialDirty(); } }
        public Vector4 IgnoreArea { get { return ignoreArea; } set { if (SetPropertyUtility.SetStruct<Vector4>(ref ignoreArea, value)) SetMaterialDirty(); } }
        public RectTransform IgnoreRect { get { return ignoreRect; } set { if (SetPropertyUtility.SetClass<RectTransform>(ref ignoreRect, value)) SetMaterialDirty(); } }
        public float AlphaThreshold { get { return alphaThreashold; } set { if (SetPropertyUtility.SetStruct<float>(ref alphaThreashold, value)) SetMaterialDirty(); } }


        //检查是否需要更新的代价似乎不比直接更新小多少，所以直接更新
        protected void UpdateAlphaFilter()
        {
            if (IsDefaultMaterial)
            {
                return;
            }
            material.SetInt("_SrcColorBlend", isTransparent ? (int)UnityEngine.Rendering.BlendMode.SrcAlpha : (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstColorBlend", isTransparent ? (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha : (int)UnityEngine.Rendering.BlendMode.Zero);
            if (!isTransparent)
            {
                return;
            }
            material.SetFloat("_AlphaThreshold", alphaThreashold);
            if(ignoreRect != null && renderCamera != null)
            {
                //左下角的点在特定摄像机的视口坐标，其x,y值就是对应的UV坐标
                Vector3 leftBottom = renderCamera.WorldToViewportPoint(ignoreRect.TransformPoint(Vector2.Scale(ignoreRect.rect.size, -ignoreRect.pivot)));
                //右上角
                Vector3 rightUp = renderCamera.WorldToViewportPoint(ignoreRect.TransformPoint(Vector2.Scale(ignoreRect.rect.size, ignoreRect.pivot)));
                //如果目标Rect是背面朝向摄像机，则将将忽略局域“翻转”
                if(leftBottom.x > rightUp.x)
                {
                    float temp = leftBottom.x;
                    leftBottom.x = rightUp.x;
                    rightUp.x = temp;
                }
                if(leftBottom.y > rightUp.y)
                {
                    float temp = leftBottom.y;
                    leftBottom.y = rightUp.y;
                    rightUp.y = temp;
                }
                ignoreArea = new Vector4(leftBottom.x, leftBottom.y, rightUp.x, rightUp.y);
            }
            if(ignoreArea.z - ignoreArea.x < 0.001f || ignoreArea.w - ignoreArea.y < 0.001f)
            {
                return;
            }
            material.SetVector("_IgnoreAlphaArea", ignoreArea);
        }

        protected override void UpdateMaterial()
        {
            UpdateAlphaFilter();
            base.UpdateMaterial();
        }

        public override void SetRenderCamera(Camera renderCamera)
        {
            this.renderCamera = renderCamera;
        }

    }
}

