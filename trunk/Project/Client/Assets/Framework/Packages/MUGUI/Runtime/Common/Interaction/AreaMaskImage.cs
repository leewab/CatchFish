using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    //该类的材质必须为AreaMask,该类通过修改AreaMask的参数来达到修改显示效果的目的
    public class AreaMaskImage : Graphic
    {

        private float Aspect { get { return rectTransform.rect.width / rectTransform.rect.height; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (material == defaultMaterial)
            {
                var shader = Shader.Find("UI/AreaMask");
                if (shader) material = new Material(shader);
            }
            material.SetFloat("_Aspect", Aspect);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            material.SetFloat("_Aspect", Aspect);
        }

        public void SetCenter(Vector2 center)
        {
            SetNormalCenter(center.x / rectTransform.rect.width, center.y / rectTransform.rect.height);
        }

        public void SetNormalCenter(float normalCenterX, float normalCenterY)
        {
            material.SetFloat("_CenterX", normalCenterX);
            material.SetFloat("_CenterY", normalCenterY);
        }

        public void SetSize(Vector2 size)
        {
            SetNormalSize(size.x / rectTransform.rect.width,size.y / rectTransform.rect.height);
        }

        public void SetNormalSize(float normalSizeX,float normalSizeY)
        {
            material.SetFloat("_SizeX", normalSizeX);
            material.SetFloat("_SizeY", normalSizeY);
        }

        //shader中保证，两条边的宽度一致
        public void SetEdgeX(float edgeX)
        {
            SetNormalEdgeX(edgeX / rectTransform.rect.width);
        }

        public void SetNormalEdgeX(float normalEdgeX)
        {
            material.SetFloat("_EdgeX", normalEdgeX);
        }

        public void SetBackgroundColor(Color color)
        {
            material.SetColor("_BackgroundColor", color);
        }

        public void SetForegroundColor(Color color)
        {
            material.SetColor("_MaskColor",color);
        }

        public void SetCircleRadiusFactor(float radiusFactor)
        {
            material.SetFloat("_CircleRadiusFactor", radiusFactor);
        }
    }
}
