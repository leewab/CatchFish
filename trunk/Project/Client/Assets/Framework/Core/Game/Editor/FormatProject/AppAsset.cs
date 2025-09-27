using Game;
using Game.Core;
using Game.UI;
using UnityEngine;

public class AppAsset : BaseAsset
{   
    
    /// <summary>
    /// App名称
    /// </summary>
    [Header("App名称")] public string AppName = "App_";

    /// <summary>
    /// 资源服ID
    /// </summary>
    [Header("服务器ID")] public string ServerID = "";

    /// <summary>
    /// 资源服URL
    /// </summary>
    [Header("URL 资源服")] public string ResURL = "";

    /// <summary>
    /// 服务器列表URL
    /// </summary>
    [Header("URL 服务器列表")] public string WebURL = "";

    /// <summary>
    /// App文件URL
    /// </summary>
    [Header("URL App文件")] public string AppURL = "";

    /// <summary>
    /// 密钥
    /// </summary>
    [Header("密钥")] public string SecretKey = "";

    /// <summary>
    /// 渠道标记
    /// </summary>
    [Header("渠道标记")] public string ChannelTag = "";

    /// <summary>
    /// 渠道分组
    /// </summary>
    [Header("渠道分组")] public ChannelGroup ChannelGroup = ChannelGroup.None;
    
    /// <summary>
    /// 渠道发行平台
    /// </summary>
    [Header("渠道发行平台")] public ChannelPlatform ChannelPlatform = ChannelPlatform.None;

    /// <summary>
    /// 是否开启热更
    /// </summary>
    [Header("开启热更")] public bool OpenHotfix = false;

    /// <summary>
    /// 是否开启RemoteLog收集器
    /// </summary>
    [Header("远程Log")] public bool OpenRemoteLog = false;

    /// <summary>
    /// 是否开启LocalLog收集器
    /// </summary>
    [Header("本地Log")] public bool OpenLocalLog = false;


    protected override void OnAssetDataRefresh()
    {
        base.OnAssetDataRefresh();
        AppAssetHelper.GenerateAppFile();
    }
}
