using UnityEngine;
using UnityEngine.UI;

namespace Game.Core
{

    public class CustomRaycastHelper : MonoBehaviour
    {
        MaskableGraphic mGraphic = null;
        bool mNeedRegister = false;
        void Awake()
        {
            mGraphic = GetComponent<MaskableGraphic>();
            mNeedRegister = (mGraphic != null && mGraphic.raycastTarget);
        }

        void OnEnable()
        {
            if (mNeedRegister && mGraphic.enabled)
            {
                RaycastTargetRegistry.RegisterGraphicForCanvas(mGraphic.canvas, mGraphic);
            }
        }

        void OnDisable()
        {
            if (mNeedRegister)
            {
                RaycastTargetRegistry.UnregisterGraphicForCanvas(mGraphic.canvas, mGraphic);
            }
        }

        void OnTransformParentChanged()
        {
            if (mNeedRegister && mGraphic.enabled)
            {
                RaycastTargetRegistry.RegisterGraphicForCanvas(mGraphic.canvas, mGraphic);
            }
        }

        void OnDestroy()
        {
            if (mNeedRegister)
            {
                RaycastTargetRegistry.UnregisterGraphicForCanvas(mGraphic.canvas, mGraphic);
            }
        }

    }
}