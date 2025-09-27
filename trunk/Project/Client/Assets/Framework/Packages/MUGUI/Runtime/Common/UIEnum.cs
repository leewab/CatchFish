namespace Game.UI
{
    /// <summary>
    /// UI SortingLayer 枚举
    /// </summary>
    public enum UILayerEnums
    {
        UIBaseLayer    = 10,
        UIGameLayer    = 21,
        UIDefaultLayer = 32,
        UIPopupLayer   = 43,
        UINoticeLayer  = 54,
        UILoadingLayer = 65,
        MAX_VALUE      = 66,
    }
    
    public enum UIAnchorsType
    {
        AnchorsCenter,      //中心锚点
        AnchorsStretch,     //
    }
    
    public enum UIEventType
    {
        OnClick          = 0,
        OnPressUp        = 1,
        OnPressDown      = 2,
        OnDrag           = 3,
        OnBeginDrag      = 4,
        OnEndDrag        = 5,
        OnDoubleClick    = 6,
        Count            = 7,
    }
}