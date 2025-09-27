using UnityEngine.UI;

public class GraphicRaycastRegister : IRaycastRegister
{
    // /// <summary>
    // /// 获取画布
    // /// </summary>
    // /// <returns></returns>
    // public MaskableGraphic UsingGraphic;
    //
    // public GraphicRaycastRegister(MaskableGraphic usingGraphic)
    // {
    //     this.UsingGraphic = usingGraphic;
    // }

    public void RegisterGraphicForCanvas(MaskableGraphic curGraphic)
    {
        if (curGraphic != null && curGraphic.raycastTarget && curGraphic.enabled)
        {
            RaycastTargetRegistry.RegisterGraphicForCanvas(curGraphic.canvas, curGraphic);
        }
    }

    public void UnRegisterGraphicForCanvas(MaskableGraphic curGraphic)
    {
        if (curGraphic != null)
        {
            RaycastTargetRegistry.UnregisterGraphicForCanvas(curGraphic.canvas, curGraphic);
        }
    }
}