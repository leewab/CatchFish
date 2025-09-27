using System.Collections.Generic;
using Game.Core;
using MUEngine;
using MUGame;
using UnityEngine;

namespace Game
{
    public class UpdateHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static UpdateHandler Instance = HandlerModule.UpdateHandler;

        private bool bNeedTrafficWin = true;
        private Dictionary<string, int> logDic = new Dictionary<string, int>();
        private Dictionary<string, string> msg_params = new Dictionary<string, string>();

        public override void Init()
        {
            base.Init();
            this.RegisterEvent();
        }

        public override void Dispose()
        {
            this.UnRegisterEvent();
            base.Dispose();
        }

        public void RegisterEvent()
        {
            CoreDelegate.OnUpdateResource += onUpdateRes;
            CoreDelegate.OnUpdateLog += onUpdateLog;
            CoreDelegate.OnBackgroundUpdate += onBackgroundUpdate;
#if UNITY_IPHONE
            // TODO:记得引入Zeus下载插件
            CoreDelegate.OriginalHttpInterface = ZeusHttpInterface.GetInstance();
#endif
        }

        public void UnRegisterEvent()
        {
            CoreDelegate.OnUpdateResource -= onUpdateRes;
            CoreDelegate.OnUpdateLog -= onUpdateLog;
            CoreDelegate.OnBackgroundUpdate -= onBackgroundUpdate;
        }

        private void onUpdateRes(string[] bundles, int loadSize, int totalSize, int type)
        {
            Debug.Log("onUpdateRes------------- " + type);
            switch ((EProgressType)type)
            {
                case EProgressType.ChooseLanguage:
                    return;
                case EProgressType.UpdateLanguageDownload:
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                    return;
                case EProgressType.UpdateLanguageDeCompress:
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                    return;
                case EProgressType.UpdateDeCompressUpdate:
                    OnUpdateDeCompressUpdate(bundles, loadSize, totalSize, type);
                    return;
                case EProgressType.UpdateDeCompress:
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                    return;
                case EProgressType.UpdateFullPackage:
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    // UpdateResView.ShowLoading(type, loadSize, totalSize);
                    return;
                case EProgressType.UpdateBundleMap:
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                    return;
            }


            if (bundles != null && bundles.Length > 0)
            {
                if (bNeedTrafficWin && totalSize > 50 * 1024f && SDKHelper.IsMobileNetwork())
                {
                    bNeedTrafficWin = false;
                    string mb = (totalSize / 1024f).ToString("#0.00M");
                    // 这里进行弹窗提示
                    AlertInfo alertInfo = new AlertInfo();
                    alertInfo.Content = string.Format(CSDisplayText.GetMessage("UpdateResDesc5"), mb);
                    alertInfo.Confirm = "确认";
                    AlertInfoHandler.Instance.Start(AlertType.Normal, alertInfo, param =>
                    {
                        MURoot.ResMgr.SetProgress(bundles, null, type);
                        UpdateMgrUtil.SetFlag((int)EUpdateFlag.Continue_Download);
                    });
                }
                else
                {
                    MURoot.ResMgr.SetProgress(bundles, null, type);
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.Continue_Download);
                }

                LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                // LoadingScreen.DownloadReward();
            }
        }

        public void onUpdateLog(int type, string url, string err = "")
        {
            msg_params.Clear();
            switch ((ELogType)type)
            {
                case ELogType.VersionBegin:
                {
                    Debug.Log("开始下载newversion.txt");
                    msg_params.Add("url", url);
                    string strJson = fastJSON.JSON.ToJSON(msg_params);
                    // GameController.Instance.SDKHandler.LogEvent("gameResReqBegin", strJson, (int)SDKType.One);
                }
                    break;
                case ELogType.VersionFail:
                {
                    Debug.Log("下载失败newversion.txt");
                    msg_params.Add("url", url);
                    msg_params.Add("error", err);
                    string strJson = fastJSON.JSON.ToJSON(msg_params);
                    // GameController.Instance.SDKHandler.LogEvent("gameResReqError", strJson, (int)SDKType.One);
                }
                    break;
                case ELogType.VersionEnd:
                {
                    Debug.Log("完成下载newversion.txt");
                    msg_params.Add("url", url);
                    string strJson = fastJSON.JSON.ToJSON(msg_params);
                    // GameController.Instance.SDKHandler.LogEvent("gameResReqSuccess", strJson, (int)SDKType.One);
                }
                    break;
                case ELogType.SubPackageBegin:
                {
                    Debug.Log("开始更新subpackage");
                    // GameController.Instance.SDKHandler.LogEventOnlyName("subpackageBegin", (int)SDKType.One);
                }
                    break;
                case ELogType.SubPackageDownload:
                {
                    int part = int.Parse(url) / 20;
                    if (part > 0)
                    {
                        for (int i = 1; i <= part; i++)
                        {
                            string key = "subpackage" + i;
                            if (!logDic.ContainsKey(key))
                            {
                                //分包下载阶段打点
                                Debug.Log("SubPackageDownload阶段打点：" + key);
                                logDic.Add(key, 1);
                                SDKHandler.Instance.LogEventOnlyName(key, (int)SDKType.One);
                            }
                        }
                    }
                }
                    break;
                case ELogType.SubPackageDecompressFail:
                {
                    Debug.Log(string.Format("解压失败 name={0}", url));
                    msg_params.Add("url", url);
                    string strJson = fastJSON.JSON.ToJSON(msg_params);
                    SDKHandler.Instance.LogEvent("subpackageDecompressFail", strJson, (int)SDKType.One);
                }
                    break;
                case ELogType.SubPackageEnd:
                {
                    Debug.Log("subpackage更新完成");
                    SDKHandler.Instance.LogEventOnlyName("subpackageEnd", (int)SDKType.One);
                }
                    break;
                case ELogType.ResourceBegin:
                {
                    Debug.Log("开始热更资源");
                    SDKHandler.Instance.LogEventOnlyName("resourceBegin", (int)SDKType.One);
                    //msg_params.Add("url", url);
                    //one sdk 打点url,不能为空
                    msg_params.Add("url", MUEngine.MUUpdateConfig.UpdateUrl);
                    string strJson = fastJSON.JSON.ToJSON(msg_params);
                    SDKHandler.Instance.LogEvent("gameUpdateAssetBegin", strJson, (int)SDKType.One);
                }
                    break;
                case ELogType.BundleMapBegin:
                {
                    Debug.Log("开始下载bundlemap");
                    SDKHandler.Instance.LogEventOnlyName("bundlemapBegin", (int)SDKType.One);
                }
                    break;
                case ELogType.BundleMapEnd:
                {
                    Debug.Log("结束下载bundlemap");
                    SDKHandler.Instance.LogEventOnlyName("bundlemapEnd", (int)SDKType.One);
                }
                    break;
                case ELogType.ResourceDownload:
                {
                    int part = int.Parse(url) / 20;
                    if (part > 0)
                    {
                        for (int i = 1; i <= part; i++)
                        {
                            string key = "hotupdate" + i;
                            if (!logDic.ContainsKey(key))
                            {
                                //热更新阶段打点
                                Debug.Log("ResourceDownload阶段打点：" + key);
                                logDic.Add(key, 1);
                                SDKHandler.Instance.LogEventOnlyName(key, (int)SDKType.One);
                            }
                        }
                    }
                }
                    break;
                case ELogType.ResourceEnd:
                {
                    {
                        Debug.Log("热更资源完成");
                        SDKHandler.Instance.LogEventOnlyName("resourceEnd", (int)SDKType.One);
                        //msg_params.Add("url", url);
                        //one sdk 打点url,不能为空
                        msg_params.Add("url", MUEngine.MUUpdateConfig.UpdateUrl);
                        string strJson = fastJSON.JSON.ToJSON(msg_params);
                        SDKHandler.Instance.LogEvent("gameUpdateAssetSuccess", strJson, (int)SDKType.One);
                    }
                }
                    break;
            }
            //Debug.Log("onUpdateLog type=" + type + " url=" + url + " err=" + err);
        }
         
        private void OnUpdateDeCompressUpdate(string[] bundles, int loadSize, int totalSize, int type)
        {
            bool needFull = PlayerPrefs.GetInt("BGDownload_Full") == 1;
            if (needFull)
            {
                AlertInfo info = new AlertInfo();
                info.Content = string.Format(CSDisplayText.GetMessage("UpdateResDesc1"), ((totalSize - loadSize) / 1024f).ToString("#0.00M"));
                info.Confirm = CSDisplayText.GetMessage("UpdateResDesc8");
                info.Cancel = CSDisplayText.GetMessage("UpdateResDesc4");
                AlertInfoHandler.Instance.Start(AlertType.Normal, info, result =>
                {
                    if (result)
                    {
                        MURoot.ResMgr.SetProgress(bundles, null, type);
                        UpdateMgrUtil.SetFlag((int)EUpdateFlag.Continue_Download);
                        LoadingHandler.Instance.SetLoadType(type, loadSize, totalSize);
                    }
                    else
                    {
                        UpdateMgrUtil.SetFlag((int)EUpdateFlag.Create_BG_Download);
                        UpdateMgrUtil.SetFlag((int)EUpdateFlag.Start_BG_Download);
                        UpdateMgrUtil.SetMaxSpeed(3 * 1024 * 1024);
                    }
                });
            }
            else
            {
                UpdateMgrUtil.SetFlag((int)EUpdateFlag.Create_BG_Download);
                UpdateMgrUtil.SetFlag((int)EUpdateFlag.Start_BG_Download);
                UpdateMgrUtil.SetMaxSpeed(3 * 1024 * 1024);
            }
        }
        
        private void onBackgroundUpdate(int flag)
        {
            EUpdateFlag mFlag = (EUpdateFlag)flag;
            if(mFlag == EUpdateFlag.DiskFull_Download)
            {
                UpdateMgrUtil.SetFlag((int)EUpdateFlag.Pause_SubPackage_Download);
                // TODO: 这里的参数走特殊的配置表
                AlertInfo info = new AlertInfo();
                info.Title = "温馨提示";
                info.Content = "您的设备存储空间已满，快去清理吧！";
                info.Confirm = "确认";
                AlertInfoHandler.Instance.Start(AlertType.Normal, info, (result) =>
                {
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.Resume_SubPackage_Download);
                });
            }
            else if(mFlag == EUpdateFlag.ShowUse4G_Hint_BG_Download)
            {
                bool use4G = PlayerPrefs.GetInt("BGDownload_Use4G") == 1;
                if(use4G)
                {
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.Skip_FullPackage_Download);
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.AllowUse4G_BG_Download);
                }
                else
                {
                    Main.Instance.StartInvoke(OnShowUse4GHandler,  0.1f);
                    // Invoke("OnShowUse4GHandler", 0.1f);
                }
            }
        }
        
        private void OnShowUse4GHandler()
        {
            // TODO: 这里的参数走特殊的配置表
            AlertInfo info = new AlertInfo();
            info.Title = "温馨提示";
            info.Content = "当前无WIFI连接，是否边玩边下载完整游戏资源？\n<size=18>下载完整游戏资源后可领取丰厚奖励！</size>";
            info.Confirm = "确定下载";
            info.Cancel = "暂不下载";
            AlertInfoHandler.Instance.Start(AlertType.Normal, info, (result) =>
            {
                bool isConfirm = (bool)result;
                if (isConfirm)
                {
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.Skip_FullPackage_Download);
                }
                else
                {
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.Skip_FullPackage_Download);
                    UpdateMgrUtil.SetFlag((int)EUpdateFlag.AllowUse4G_BG_Download);
                }
            });
        }
        
         
    }
}