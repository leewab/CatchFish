using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MaskableText : Text
    {
        private Material realMaterial
        {
            get
            {
                var mat = canvasRenderer.GetMaterial(0);
                if(mat == null)
                {
                    mat = material;
                }
                return mat;
            }
        }

        public void SetNormalCenterY(float centerY)
        {
            realMaterial.SetFloat("_CenterY", centerY);
        }
        public void SetNormalSizeY(float sizeY)
        {
            realMaterial.SetFloat("_SizeY", sizeY);
        }
    }
}

