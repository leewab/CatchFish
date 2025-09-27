namespace Game.Core
{
    /// <summary>
    /// Game State
    /// 游戏状态机
    /// </summary>
    public enum States
    {
        NONE,
        INIT,                   // 初始化
        PRELOAD,                // 预加载
        LOGIN,                  // 登录
        SWITCHACCOUNT,          // 切换登录
        CREATEROLE,             // 创角
        NORMAL,                 // 正常
        DISCONNECT,             // 断联
    }
    
    /// <summary>
    /// 渠道发行平台
    /// </summary>
    public enum ChannelPlatform
    {
        None          = 0,
        China         = 1,
    }

    /// <summary>
    /// 渠道分组
    /// </summary>
    public enum ChannelGroup
    {
        None           = 0,
        DaLan_Android  = 1,   //渠道_平台
        DaLan_IOS      = 2,
    }
    
    public enum LayerEnum
    {
        //normal
        Default = 0,
        TransparentFX = 1,
        IgnoreRaycast = 2,
        Water = 4,
        UI = 5,
        //custom
    }
    
    /// <summary>
    /// 应用状态
    /// </summary>
    public enum AppState
    {
        /// <summary>
        /// 开发状态
        /// </summary>
        Development,
        /// <summary>
        /// 测试状态
        /// </summary>
        Beta,
        /// <summary>
        /// 上线状态
        /// </summary>
        Online
    }

    public enum RES_LOAD_STATE
    {
        UNDIFINE,   // 等待加载状态
        LOADING,    // 加载中
        NORAML,     // 加载完成
    }
    
    /// <summary>
    /// 资源根目录
    /// </summary>
    public enum ResRoot
    {
        ResWeb,                      //资源网络加载
        AppWeb,                      //App网络加载
        ServerListWeb,               //区服列表网络加载
        FullPath,                    //全路径
        WebMemory,                   //Web网络下载Memory加载
        StreamingAssets,             //Streaming目录
        PersistentData,              //Persistent目录
        TemporaryCache,              //临时目录
        AssetDataBase,               //AssetDataBase 相对路径加载
    }
    
    /// <summary>
    /// 资源状态
    /// </summary>
    public enum ResState
    {
        Waiting,
        Loading,
        Loaded
    }
    
    /// <summary>
    /// Res分组 两大作用
    /// 1.AssetBundle分组枚举，AB创建的分组需要在这里注册一下
    /// 2.用于资源的文件夹划分
    ///     （渠道_平台/AssetBundle/ResGroup枚举中大于0的）
    ///     （渠道_平台/其他资源文件）
    /// </summary>
    public enum ResGroup
    {
        None  = 0,     //非AssetBundle资源（意味着不在AssetBundle文件夹下）
        Common = 1,    //公共AssetBundle资源（在Common文件夹下）
    }
}