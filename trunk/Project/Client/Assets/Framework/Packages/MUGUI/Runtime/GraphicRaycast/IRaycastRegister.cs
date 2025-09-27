using UnityEngine.UI;

/// <summary>
/// GraphicRaycast 注册器
/// </summary>
public interface IRaycastRegister
{
    /// <summary>
    /// 注册画布点击
    /// </summary>
    public void RegisterGraphicForCanvas(MaskableGraphic curGraphic);

    /// <summary>
    /// 取消画布点击的注册
    /// </summary>
    public void UnRegisterGraphicForCanvas(MaskableGraphic curGraphic);
}