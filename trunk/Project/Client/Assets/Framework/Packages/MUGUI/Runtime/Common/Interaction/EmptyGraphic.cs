using UnityEngine;
using UnityEngine.UI;

namespace MUGUI
{
   
    /// <summary>
    /// 图文混排
    /// </summary>
    [AddComponentMenu("UI/Empty Graphic", 15)]
    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        //让 EmptyGraphic 可以不被 CustomGraphicRaycaster 忽略掉
        //象征性的给个配置项
        [SerializeField]
        private bool autoRegister = false;

        public bool AutoRegister
        {
            get
            {
                return autoRegister;
            }
            set
            {
                autoRegister = value;
            }
        }

        private bool ShouldRegister
        {
            get
            {
                return autoRegister && raycastTarget;
            }
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();

            if (ShouldRegister)
            {
                RaycastTargetRegistry.RegisterGraphicForCanvas(canvas, this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            RaycastTargetRegistry.UnregisterGraphicForCanvas(canvas, this);
        }

        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            if (ShouldRegister)
            {
                RaycastTargetRegistry.RegisterGraphicForCanvas(canvas, this);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RaycastTargetRegistry.UnregisterGraphicForCanvas(canvas, this);
        }

    }
}