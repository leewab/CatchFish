namespace Game
{
    // 渠道类型
    public enum CHANNEL_TYPE 
    {
        None     = 0,
        CN       = 1,      //  国服
        INTL     = 2,      //  国际
    }

    // 子渠道类型
    public enum CHANNEL_SUB_TYPE
    {
        None            = 0,      
        Official        = 1,      // 官网
        Google          = 2,      // Google
        HuaWei          = 3,      // 华为
        Dalan           = 4,      // 大蓝
        CLOUD_GAME      = 5,      // 云游戏
    }

    // 运行平台
    public enum RUN_PLATFORM_TYPE
    {
        Normal            = 0,    // 不分平台
        Android           = 1,    // Android
        IPhone            = 2,    // iOS
        Windows           = 3,    // PC
        WindowsEditor     = 4,    // 编辑器平台
    }
}