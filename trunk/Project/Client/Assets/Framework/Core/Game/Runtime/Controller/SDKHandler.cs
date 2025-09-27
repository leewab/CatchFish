using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    struct CacheEventInfo
    {
        public string name;
        public string param;
        public int sdktype;
    }
    
    public class SDKHandler : MonoSingleton<SDKHandler>
    {
        private AgentBase mAgent;
        private List<KeyValuePair<string, int>> mEventLogCache = new List<KeyValuePair<string, int>>();
        private List<CacheEventInfo> mEventLogCache2 = new List<CacheEventInfo>();
        
        protected override void  OnSingletonDispose()
        {
            // sdk要求退出前调用onRelease
            if (SDKHandler.Instance.IsNeedSDK)
            {
                SDKHandler.Instance.Exit();
            }
        }
        
        public bool IsNeedSDK
        {
            get
            {
                return false;
            }
        }
        
        // 记录SDK是否初始化成功
        private bool mIsSDKLegal = false;
        public bool IsSDKLegal
        {
            get
            {
                this.LogPhone("mIsSDKLegal : " + mIsSDKLegal);
                return mIsSDKLegal;
            }
            set
            {
                this.LogPhone("mIsSDKLegal set : " + value);
                mIsSDKLegal = value;
            }
        }

        public void Init()
        {
            Debug.Log("OneSDK GetInstance Init1");
#if UNITY_ANDROID && !UNITY_EDITOR
            mAgent = new AgentAndroid();
            Debug.Log("AgentAndroid mAgent 2");
#elif UNITY_IPHONE && !UNITY_EDITOR
            mAgent = new AgentIOS();
            Debug.Log("AgentIOS mAgent 3");
#else
            mAgent = new AgentBase();
            Debug.Log("AgentBase mAgent 4");
#endif
            mAgent?.Init();
            Debug.Log("mAgent?.Init 5");
            // listener.AddComponent<OneSDKCallback>().SetSDK(this);
        }

        public bool IsSDKInit()
        {
            return mAgent.IsSDKInit();
        }
        
        /// <summary>
        /// 获取渠道资源目录，用来区分下载资源，服务器列表等
        /// </summary>
        /// <returns></returns>
        public string GetChannelDir()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
#if LHRG_OB
                return "Android";
#else
                return "AndroidTest";
#endif
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if LHRG_OB
                return "iPhone";
#else
                return "iPhoneTest";
#endif
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
#if LHRG_OB
                return "PC";
#else
                return "PCTest";
#endif
            }

            return "PCTest";
        }

        public int GetChannelType()
        {
            
#if LHRG_CH
            return (int)CHANNEL_TYPE.CN;
#elif LHRG_INTL
            return (int)CHANNEL_TYPE.INTL;
#endif
            return (int)CHANNEL_TYPE.None;
        }

        public int GetChannelSubType()
        {
#if LHRG_SUB_OFFICIAL
            return (int)CHANNEL_SUB_TYPE.Official;
#elif LHRG_SUB_GOOGLE
            return (int)CHANNEL_SUB_TYPE.Google;
#elif LHRG_SUB_HUAWEI
            return (int)CHANNEL_SUB_TYPE.HuaWei;
#endif
            return (int)CHANNEL_SUB_TYPE.None;
        }

        public int GetPlatformType()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                return (int)RUN_PLATFORM_TYPE.Android;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return (int)RUN_PLATFORM_TYPE.IPhone;
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                return (int)RUN_PLATFORM_TYPE.Windows;
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return (int)RUN_PLATFORM_TYPE.WindowsEditor;
            }

            return (int)RUN_PLATFORM_TYPE.Normal;
        }
        
        public static bool IsSimulator()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return false;
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (m_activity == null)
            {
                InitAndroidActivity();
            }
            return m_activity.Call<bool>("IsSimulator");
#else
            return false;     
#endif
        }
        
        /// <summary>
        /// 获取登录次数
        /// </summary>
        /// <returns></returns>
        public int GetLoginCount()
        {
            if (!mIsSDKLegal)
            {
                return 0;
            }

            return mAgent.GetLoginCount();
        }
        
        /// <summary>
        /// 退出游戏
        /// </summary>
        public void Exit()
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.Exit();
        }
        
        /// <summary>
        /// 重启应用
        /// </summary>
        public void ReStart()
        {
            mAgent.ReStart();
        }
        
        //打开用户中心界面
        public void StartPersonHome()
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.StartPersonHome();
        }
        
        //2.2 设置主页面客服tab是否可见
        public void SetCustomerServiceVisibility()
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.SetCustomerServiceVisibility();
        }
        
        /// <summary>
        /// 角色进入场景后，由lua调用，用来同步角色信息，填充sdk打点需要的信息
        /// </summary>
        /// <param name="RoleId">   角色id</param>
        /// <param name="RoleName"> 角色名字</param>
        /// <param name="Lv">       人物等级</param>
        /// <param name="ZoneId">   区id</param>
        /// <param name="ZoneName"> 区名字</param>
        /// <param name="PartyName">帮会名字</param>
        /// <param name="Balance">  金钱</param>
        /// <param name="Vip">      Vip等级</param>
        /// <param name="RoleCreateTime">创建角色的时间</param>
        public void SyncUserInfo(string RoleId, string RoleName, string Lv, int ZoneId, string ZoneName,
            string PartyName, string Balance,
            string Vip, string RoleCreateTime)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.SyncUserInfo(RoleId, RoleName, Lv, ZoneId, ZoneName, PartyName, Balance, Vip, RoleCreateTime);
        }
        
        
        /// <summary>
        /// 支付
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="amount">金额（元）</param>
        /// <param name="name">商品名称</param>
        /// <param name="id">商品id</param>
        /// <param name="describe">商品描述</param>
        public void Pay(string orderid, int amount, string name, string id, string describe, string jsonparam, string callbackUrl)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.Pay(orderid, amount, name, id, describe, jsonparam, callbackUrl);
        }
        
        /// <summary>
        /// 海外支付
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="amount">金额</param>
        /// <param name="name">商品名称</param>
        /// <param name="productid">商品id</param>
        /// <param name="describe">商品描述</param>
        /// <param name="jsonparam">透传参数</param>
        /// <param name="callbackUrl">回调</param>
        /// <param name="serverid">服务器地址</param>
        /// <param name="ext1">附加参数1</param>
        /// <param name="ext2">附加参数2</param>
        public void Pay2(string orderid, int amount, string name, string productid, string describe, string jsonparam, string callbackUrl, string serverid, string ext1, string ext2)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.Pay2(orderid, amount, name, productid, describe, jsonparam, callbackUrl, serverid, ext1, ext2);
        }
        
        /// <summary>
        /// 本地推送
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="date">时间</param>
        public void PushLocalNotification(long id, string title, string content, string date)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.PushLocalNotification(id, title, content, date);
        }
        
        /// <summary>
        /// 清理所有本地推送内容
        /// </summary>
        public void ClearLocalNotification()
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.ClearLocalNotification();
        }
        
        /// <summary>
        /// 注册电池事件
        /// </summary>
        public void RegisterBattery()
        {
            mAgent.RegisterBattery();
        }
        
        /// <summary>
        /// 本地推送（多次）
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="date">时间</param>
        /// <param name="dayloop">次数</param>
        public void PushLocalNotificationLoop(long id, string title, string content, string date, int dayloop)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent.PushLocalNotificationLoop(id, title, content, date, dayloop);
        }
        
        //打开客服页面
        public void OpenWebService()
        {
            mAgent.OpenWebService();
        }
        
        //打开悬浮按钮，存储角色信息后
        public void ShowPlatform()
        {
            mAgent.ShowPlatform();
        }
        
        /// <summary>
        /// 权限申请
        /// </summary>
        /// <param name="requestCode">回调id 回调时原样返回</param>
        /// <param name="permissionName">权限名称</param>
        /// <param name="permissionDes"权限描述></param>
        /// <param name="tipStr1">第一次权限申请时提示内容 （填"" 则不会显示）</param>
        /// <param name="tipStr2">权限拒绝后提示内容（填"" 则不会显示）</param>
        /// <param name="tipStr3">权限永久拒绝后提示内容 （填"" 则不会显示）</param>
        public void CheckPermissionNew(int requestCode, string permissionName, string permissionDes, string tipStr1,
            string tipStr2, string tipStr3)
        {
            mAgent.CheckPermissionNew(requestCode, permissionName, permissionDes, tipStr1, tipStr2, tipStr3);
        }

        //关闭悬浮按钮
        public void HiddenPlatform()
        {
            mAgent.HiddenPlatform();
        }
        
        //是否实名
        public void CheckCertification()
        {
            mAgent.CheckCertification();
        }
        
        //实名认证
        public void IntentCertification()
        {
            mAgent.IntentCertification();
        }
        
        /// <summary>
        /// 判断是否安装了某个应用
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public bool CheckPackage(string packageName)
        {
            return mAgent.CheckPackage(packageName);
        }
        
        /// <summary>
        /// 查询是否支持线性马达振动能力
        /// </summary>
        /// <returns></returns>
        public string GetHWHapticsGradeValue()
        {
            return mAgent.GetHWHapticsGradeValue();
        }

        /// <summary>
        /// 设置线性马达振动参数
        /// </summary>
        public void SetHWHapticsGradeValue(string strType)
        {
            mAgent.SetHWHapticsGradeValue(strType);
        }

        #region Fatigue

        /// <summary>
        /// 防沉迷预登录接口（获取防沉迷信息）
        /// </summary>
        /// <param name="userId"></param>
        public void FatigueBeforeRoleLogin(string userId)
        {
            mAgent.FatigueBeforeRoleLogin(userId);
        }

        /// <summary>
        /// 防沉迷角色登录接口
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="roleId"></param>
        public void FatigueAfterRoleLogin(string userId, int serverId, string roleId)
        {
            mAgent.FatigueAfterRoleLogin(userId, serverId, roleId);
        }

        /// <summary>
        /// 防沉迷角色登出接口
        /// </summary>
        public void FatigueAfterRoleLogout()
        {
            mAgent.FatigueAfterRoleLogout();
        }

        /// <summary>
        /// 打开相关法律条款页面接口
        /// </summary>
        public void OpenComplianceOnWebView()
        {
            mAgent.OpenComplianceOnWebView();
        }

        /// <summary>
        /// 清除防沉迷缓存
        /// </summary>
        public void FatigueClearCache()
        {
            mAgent.FatigueClearCache();
        }

        #endregion
        
        public void LogPhone(string content)
        {
            mAgent?.LogPhone(content);
        }

        /// <summary>
        /// 统计事件，打点
        /// </summary>
        /// <param name="eventName">统计的事件</param>
        /// <param name="sdktype">sdk类型，1是公司打点，2是阿里打点</param>
        public void LogEventOnlyName(string eventName, int sdktype)
        {
            if (!IsNeedSDK)
            {
                return;
            }

            if (!mIsSDKLegal)
            {
                // sdk 没初始化成功呢，先缓存下点
                KeyValuePair<string, int> pair = new KeyValuePair<string, int>(eventName, sdktype);
                mEventLogCache.Add(pair);
                return;
            }
            mAgent?.LogEventOnlyName(eventName, sdktype);
        }
        
        /// <summary>
        /// 统计事件（打点）
        /// </summary>
        /// <param name="name">统计的事件名，运营提供</param>
        /// <param name="param">参数集合</param>
        /// <param name="sdktype">sdk类型，1是公司打点，2是阿里打点</param>
        public void LogEvent(string name, string param, int sdktype)
        {
            if (!IsNeedSDK)
            {
                return;
            }
        
            if (!mIsSDKLegal)
            {
                // sdk 没初始化成功呢，先缓存下点
                var info = new CacheEventInfo();
                info.name = name;
                info.param = param;
                info.sdktype = sdktype;
                mEventLogCache2.Add(info);
                return;
            }
            mAgent?.LogEvent(name, param, sdktype);
        }
        
        /// <summary>
        /// 推送缓存打点记录
        /// </summary>
        public void LogCacheEvent()
        {
            if (!IsNeedSDK || !mIsSDKLegal)
            {
                return;
            }
            foreach (var e in mEventLogCache)
            {
                LogEventOnlyName(e.Key, e.Value);
            }
            foreach(var e in mEventLogCache2)
            {
                LogEvent(e.name, e.param, e.sdktype);
            }

            mEventLogCache.Clear();
            mEventLogCache2.Clear();
        }
        
        /// <summary>
        /// 通知系统相册更新
        /// </summary>
        /// <param name="path"></param>
        public void NotifyAlbum(string path)
        {
            mAgent?.NotifyAlbum(path);

        }
        
        /// <summary>
        /// 建立一个公共目录，存放一些项目产生的图片，这些图片的特点是能在手机相册中看到，例如二维码
        /// </summary>
        /// <returns></returns>
        public string MakePhotoDir()
        {
            return mAgent.MakePhotoDir();
        }
        
        /// <summary>
        /// SDK特殊方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="jsonStr"></param>
        public void SDKCommonCallFunction(string methodName, string jsonStr)
        {
            mAgent?.SDKCommonCallFunction(methodName, jsonStr);
        }
        
        /// <summary>
        /// 登录失败时同步给OneSDK
        /// </summary>
        public void SubmitUserInfo_LOGIN_ERROR(string info)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent?.SubmitUserInfo_LOGIN_ERROR(info);
        }
        
        /// <summary>
        /// 登录时同步给OneSDK
        /// </summary>
        public void SubmitUserInfo_LOGIN(string info)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent?.SubmitUserInfo_LOGIN(info);
        }
        
        /// <summary>
        /// 创角时同步给OneSDK
        /// </summary>
        public void SubmitUserInfo_CREATE_ROLE(string info)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent?.SubmitUserInfo_CREATE_ROLE(info);
        }
        
        /// <summary>
        /// 升级时同步给OneSDK
        /// </summary>
        public void SubmistUserInfo_ROLE_LEVEL_CHANGE(string info)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent?.SubmistUserInfo_ROLE_LEVEL_CHANGE(info);
        }
        
        /// <summary>
        /// 角色返回登录，退出时同步
        /// </summary>
        public void SubmitUserInfo_ROLE_EXIT(string info)
        {
            if (!mIsSDKLegal)
            {
                return;
            }

            mAgent?.SubmitUserInfo_ROLE_EXIT(info);
        }

        public void OpenWebUrl(string url)
        {
            mAgent?.OpenWebUrl(url);
        }
        
        public string GetSDKDeviceId()
        {
            return mAgent?.GetSDKDeviceId();
        }
        
        /// <summary>
        /// 回传SDK AccountId
        /// </summary>
        /// <param name="accountId"></param>
        public void SetAccountId(string accountId)
        {
            mAgent?.SetAccountId(accountId);
        }
        
        /// <summary>
        /// 获取包的名字，可能只有android平台需要
        /// </summary>
        /// <returns></returns>
        public string GetPacketName()
        {
            return mAgent?.GetPacketName();
        }        
        
        public string GetLocationInfo()
        {
            return "";
        }
        
    }
}