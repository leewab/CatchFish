using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

namespace Game
{
    public class AgentIOS : AgentBase
    {
#if UNITY_IOS
        //SDK初始化
        [DllImport("__Internal")]
        private static extern void _Init();
        //SDK登录
        [DllImport("__Internal")]
        private static extern void _Login();
        //SDK登出
        [DllImport("__Internal")]
        private static extern void _Logout();
        //扫码
        [DllImport("__Internal")]
        private static extern bool _QrScan();
        //扫码带参数
        [DllImport("__Internal")]
        private static extern bool _QrScanWithExt(string extinfo);
        
        //监测SDK初始化是否完成（保证时序安全）
        [DllImport("__Internal")]
        private static extern void _UnityInitFinished();
        //是否登录
        [DllImport("__Internal")]
        private static extern bool _IsHasLogin();
        //是否初始化
        [DllImport("__Internal")]
        private static extern bool _IsSDKInit();
        //登录次数
        [DllImport("__Internal")]
        private static extern int _GetLoginCount();
        [DllImport("__Internal")]
        private static extern void _LogEvent(string eventName, string paramMap, int type);
        [DllImport("__Internal")]
        private static extern void _LogEventOnlyName(string eventName, int type);
        [DllImport("__Internal")]
        private static extern void _Purchase(string orderid, string amount, string name, string id, string describe, string jsonparam, string callbackUrl);

        //登陆游戏服务器”后提交信息：SDKBase.UserInfoType.LOGIN
        [DllImport("__Internal")]
        private static extern void _SubmitUserInfo_LOGIN(string info);
        //登录失败”后提交信息：SDKBase.UserInfoType.LOGIN
        [DllImport("__Internal")]
        private static extern void _SubmitUserInfo_LOGIN_ERROR(string info);
        //创建游戏角色”后提交信息：SDKBase.UserInfoType.CREATE_ROLE
        [DllImport("__Internal")]
        private static extern void _SubmitUserInfo_CREATE_ROLE(string info);
        //角色升级”后提交信息：SDKBase.UserInfoType.ROLE_LEVEL_CHANGE
        [DllImport("__Internal")]
        private static extern void _SubmitUserInfo_ROLE_LEVEL_CHANGE(string info);
        //角色退出时，提交信息
        [DllImport("__Internal")]
        private static extern void _SubmitUserInfo_ROLE_EXIT(string info);
        [DllImport("__Internal")]
        private static extern void _SyncUserInfo(string roleId, string roleName, string lv, int zoneId, string zoneName, string partyName, string balance, string vip, string roleCreateTime);
        [DllImport("__Internal")]
        private static extern void _SyncUserInfo2(string zoneIdStr, string unionId, string professionId, string professionName, string sex, string fight, string unionTitleId, string unionTitleName, string friendList);


        [DllImport("__Internal")]
        private static extern void _StartWebView(string url);

        [DllImport("__Internal")]
        private static extern void _PushLocalNotification(long pushid, string title, string content, string data);
        [DllImport("__Internal")]
        private static extern void _PushLocalNotificationLoop(long pushid, string title, string content, string data, int dayLoop);
        [DllImport("__Internal")]
        private static extern void _ClearLocalNotification();
        [DllImport("__Internal")]
        private static extern void _RegisterBattery();
        [DllImport("__Internal")]
        private static extern string _GetPacketName();

        [DllImport("__Internal")]
        private static extern void _UpLoadFile(string key, string filePath);

        [DllImport("__Internal")]
        private static extern void _UpLoadFileBucket(string bucket, string key, string filePath);
        
        [DllImport("__Internal")]
        private static extern void _SetAccountId(string accountId);

        [DllImport("__Internal")]
        private static extern void _DebugMomory(bool isDebug);

        [DllImport("__Internal")]
        private static extern void _ReStart();
        //打开阿里用研界面
        [DllImport("__Internal")]
        private static extern void _ShowUserSurveyWebView();

        // 防沉迷预登录接口（获取防沉迷信息）
        [DllImport("__Internal")]
        private static extern void _FatigueBeforeRoleLogin(string userId);
        
        // 防沉迷角色登录接口
        [DllImport("__Internal")]
        private static extern void _FatigueAfterRoleLogin(string userId, int serverId, string roleId);
        
        //防沉迷角色登出接口
        [DllImport("__Internal")]
        private static extern void _FatigueAfterRoleLogout();

        //打开相关法律条款页面接口
        [DllImport("__Internal")]
        private static extern void _OpenComplianceOnWebView();
        // 清除防沉迷缓存
        [DllImport("__Internal")]
        private static extern void _FatigueClearCache();
        
        //调用活动SDK
        [DllImport("__Internal")]
        private static extern void _OpenWebUrl(string urlStr);
        
        #region live sdk
        [DllImport("__Internal")]
        private static extern void _LiveSetAccountId(string accountId);

        [DllImport("__Internal")]
        private static extern void _LiveDebugMode(bool b);

        
        [DllImport("__Internal")]
        private static extern void _LiveSetParams(string gameId, string nick);

        
        [DllImport("__Internal")]
        private static extern void _LiveDoStart();

        
        [DllImport("__Internal")]
        private static extern void _LiveDoStartByRoomId(string roomId);

        
        [DllImport("__Internal")]
        private static extern void _LiveStop();

        
        [DllImport("__Internal")]
        private static extern void _LiveGetRoomList();

        
        [DllImport("__Internal")]
        private static extern void _LiveChat(string str);

        
        [DllImport("__Internal")]
        private static extern void _LiveSendGift(string str);

        
        [DllImport("__Internal")]
        private static extern void _LiveGetSchedule();
        #endregion

        // 申请权限
        [DllImport("__Internal")]
        private static extern void _CheckPermissionNew(int requestCode, int permission, string title, string msg, string noStr, string yesStr);
        
        // 日志
        [DllImport("__Internal")]
        private static extern void _LogPhone(string log);

        //检查手机绑定
        [DllImport("__Internal")]
        private static extern void _CheckBindPhone();
    
        //绑定手机
        [DllImport("__Internal")]
        private static extern void _BindPhone();

        //是否实名
        [DllImport("__Internal")]
        private static extern void _CheckCertification();

        //实名认证
        [DllImport("__Internal")]
        private static extern void _IntentCertification();

        
        [DllImport("__Internal")]
        private static extern void _CloseWebView();

        [DllImport("__Internal")]
        private static extern void _ActivitySdkNativeToJs(string type, string data);

        [DllImport("__Internal")]
        private static extern void _ActivitySdkOnJsActionResultCallback(string actionId, string result);

        [DllImport("__Internal")]
        private static extern void _ActivitySdkSetConfig(string configString);
        
        [DllImport("__Internal")]
        private static extern void _ActivitySdkClearConfig();
        
        // 将手机信息同步给Unity
        [DllImport("__Internal")]
        private static extern void _SyncPhoneInfoToUnity();
#endif

        public override void Init()
        {
            base.Init();
#if UNITY_IOS
            _Init();
#endif
        }

        public override void Login()
        {
#if UNITY_IOS
            _Login();
#endif
            base.Login();
        }

        public override void Logout()
        {
#if UNITY_IOS
            _Logout();
#endif
            base.Logout();
        }

        public override void IsShowQrCodeScan()
        {
            base.IsShowQrCodeScan();
        }

        public override void ScanBusiness()
        {
#if UNITY_IOS
            _QrScan();
#endif
            base.ScanBusiness();     
        }

        public override void ScanBusinessAllPlatform( string param )
        {
#if UNITY_IOS
            _QrScanWithExt(param);
#endif
            base.ScanBusinessAllPlatform( param );     
        }

        public override bool IsHasLogin()
        {
#if UNITY_IOS
            return _IsHasLogin();
#else
            return base.IsHasLogin();
#endif

        }

        public override bool IsSDKInit()
        {
#if UNITY_IOS
            return _IsSDKInit();
#else
            return base.IsSDKInit();
#endif
        }

        public override int GetLoginCount()
        {
#if UNITY_IOS
            return _GetLoginCount();
#else
            return base.GetLoginCount();
#endif
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void SyncUserInfo(string RoleId, string RoleName, string Lv, int ZoneId, string ZoneName, string PartyName, string Balance, string Vip, string RoleCreateTime)
        {
#if UNITY_IOS
            _SyncUserInfo(RoleId, RoleName, Lv, ZoneId, ZoneName, PartyName, Balance, Vip, RoleCreateTime);
            base.SyncUserInfo(RoleId, RoleName, Lv, ZoneId, ZoneName, PartyName, Balance, Vip, RoleCreateTime);
#endif
        }

        public override void SyncUserInfo2(string ZoneIdStr, string UnionId, string ProfessionId, string ProfessionName, string Sex, string Fight, string UnionTitleId, string UnionTitleName, string FriendList)
        {
#if UNITY_IOS
            _SyncUserInfo2(ZoneIdStr, UnionId, ProfessionId, ProfessionName, Sex, Fight, UnionTitleId, UnionTitleId, FriendList);
            base.SyncUserInfo2(ZoneIdStr, UnionId, ProfessionId, ProfessionName, Sex, Fight, UnionTitleId, UnionTitleName, FriendList);
#endif
        }

        public override void SubmitUserInfo_LOGIN(string info)
        {
#if UNITY_IOS
            _SubmitUserInfo_LOGIN(info);
#endif
            base.SubmitUserInfo_LOGIN(info);
        }

        public override void SubmitUserInfo_LOGIN_ERROR(string info)
        {
#if UNITY_IOS
            _SubmitUserInfo_LOGIN_ERROR(info);
#endif
            base.SubmitUserInfo_LOGIN_ERROR(info);
        }

        public override void SubmitUserInfo_CREATE_ROLE(string info)
        {
#if UNITY_IOS
            _SubmitUserInfo_CREATE_ROLE(info);
#endif
            base.SubmitUserInfo_CREATE_ROLE(info);
        }

        public override void SubmistUserInfo_ROLE_LEVEL_CHANGE(string info)
        {
#if UNITY_IOS
            _SubmitUserInfo_ROLE_LEVEL_CHANGE(info);
#endif
            base.SubmistUserInfo_ROLE_LEVEL_CHANGE(info);
        }

        public override void SubmitUserInfo_ROLE_EXIT(string info)
        {
#if UNITY_IOS
            _SubmitUserInfo_ROLE_EXIT(info);
#endif
            base.SubmitUserInfo_ROLE_EXIT(info);
        }

        public override void Pay(string orderid, int amount, string name, string id, string describe, string jsonparam, string callbackUrl)
        {
#if UNITY_IOS
            _Purchase(orderid, amount.ToString(), name, id, describe, jsonparam, callbackUrl);
#endif
            base.Pay(orderid, amount, name, id, describe, jsonparam, callbackUrl);
        }

        public override void LogEvent(string name, string param, int sdktype)
        {
#if UNITY_IOS
            _LogEvent(name, param, sdktype);
#endif
            base.LogEvent(name, param, sdktype);

        }

        public override void LogEventOnlyName(string eventName, int sdktype)
        {
#if UNITY_IOS
            _LogEventOnlyName(eventName, sdktype);
#endif
            base.LogEventOnlyName(eventName, sdktype);
        }

        // 本地推送（单次）
        public override void PushLocalNotification(long id, string title, string content, string date)
        {
#if UNITY_IOS
            _PushLocalNotification(id, title, content, date);
#endif
            base.PushLocalNotification(id, title, content, date);
        }

        // 本地推送（多次）
        public override void PushLocalNotificationLoop(long id, string title, string content, string date, int dayloop)
        {
#if UNITY_IOS
            _PushLocalNotificationLoop(id, title, content, date, dayloop);
#endif
            base.PushLocalNotificationLoop(id, title, content, date, dayloop);
        }

        // 清理本地推送
        public override void ClearLocalNotification()
        {
#if UNITY_IOS
            _ClearLocalNotification();
#endif
            base.ClearLocalNotification();
        }

        public override void OpenLaohuWebView(string url)
        {
            base.OpenLaohuWebView(url);
        }

        public override void LogPhone(string log)
        {
#if UNITY_IOS
            _LogPhone(log);
#endif
            base.LogPhone(log);
        }

        public override void UnityInitFinished()
        {
#if UNITY_IOS
            _UnityInitFinished();
#endif
            base.UnityInitFinished();
        }

        public override void OpenAliService(string url)
        {
#if UNITY_IOS
            _StartWebView(url);
#endif
            base.OpenAliService(url);
        }

        public override void RegisterBattery()
        {

#if UNITY_IOS
            _RegisterBattery();
#endif
            base.RegisterBattery();
        }

        // 获取包名 
        public override string GetPacketName()
        {
#if UNITY_IOS
            return _GetPacketName();
#else

#endif
            return base.GetPacketName();
        }

        public override void UploadFile(string key, string filePath)
        {
#if UNITY_IOS
            _UpLoadFile(key, filePath);
#else

#endif
            base.UploadFile(key, filePath);
        }

        public override void UpLoadFileBucket(string bucket, string key, string filePath)
        {
#if UNITY_IOS
            _UpLoadFileBucket(bucket, key, filePath);
#else

#endif
            base.UpLoadFileBucket(bucket, key, filePath);
        }

        public override void SetAccountId(string accountId)
        {
#if UNITY_IOS
            _SetAccountId(accountId);
#endif
            base.SetAccountId(accountId);
        }

        public override void DebugMemory(bool isDebug)
        {
#if UNITY_IOS
            _DebugMomory(isDebug);
#endif
            base.DebugMemory(isDebug);
        }

        public override string GetManufacture()
        {
#if UNITY_IOS
            return "apple";
#endif
            return base.GetManufacture();
        }

        public override void ReStart()
        {
#if UNITY_IOS
            _ReStart();
#endif
            base.ReStart();
        }

        public override void ShowUserSurveyWebView()
        {
#if UNITY_IOS
            _ShowUserSurveyWebView();
#endif
            base.ShowUserSurveyWebView();
        }
        /// <summary>
        /// 防沉迷预登录接口（获取防沉迷信息）
        /// </summary>
        /// <param name="userId"></param>
        public override void FatigueBeforeRoleLogin(string userId)
        {
#if UNITY_IOS
            _FatigueBeforeRoleLogin(userId);
#endif
            base.FatigueBeforeRoleLogin(userId);
        }

        /// <summary>
        /// 防沉迷角色登录接口
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="roleId"></param>
        public override void FatigueAfterRoleLogin(string userId, int serverId, string roleId)
        {
#if UNITY_IOS
            _FatigueAfterRoleLogin(userId, serverId, roleId);
#endif
            base.FatigueAfterRoleLogin(userId, serverId, roleId);
        }

        /// <summary>
        /// 防沉迷角色登出接口
        /// </summary>
        public override void FatigueAfterRoleLogout()
        {
#if UNITY_IOS
            _FatigueAfterRoleLogout();
#endif
            base.FatigueAfterRoleLogout();
        }
        
        /// <summary>
        /// 打开相关法律条款页面接口
        /// </summary>
        public override void OpenComplianceOnWebView()
        {
#if UNITY_IOS
            _OpenComplianceOnWebView();
#endif
            base.OpenComplianceOnWebView();
        }
        /// <summary>
        /// 清除防沉迷缓存
        /// </summary>
        public override void FatigueClearCache()
        {
#if UNITY_IOS
            _FatigueClearCache();
#endif
            base.FatigueClearCache();
        }

        /// <summary>
        /// 打开网页
        /// </summary>
        /// <param name="urlStr"></param>
        public override void OpenWebUrl(string urlStr)
        {
#if UNITY_IOS
            _OpenWebUrl(urlStr);
#endif
            base.OpenWebUrl(urlStr);
        }

        #region live sdk
        public override void LiveSetAccountId(string accountId)
        {
#if UNITY_IOS
            _LiveSetAccountId(accountId);
#endif
        }

        public override void LiveDebugMode(bool b)
        {
#if UNITY_IOS
            _LiveDebugMode(b);
#endif
        }

        public override void LiveSetParams(string gameId, string nick)
        {
#if UNITY_IOS
            _LiveSetParams(gameId, nick);
#endif
        }

        public override void LiveDoStart()
        {
#if UNITY_IOS
            _LiveDoStart();
#endif
        }

        public override void LiveDoStartByRoomId(string roomId)
        {
#if UNITY_IOS
            _LiveDoStartByRoomId(roomId);
#endif
        }

        public override void LiveStop()
        {
#if UNITY_IOS
            _LiveStop();
#endif
        }

        public override void LiveGetRoomList()
        {
#if UNITY_IOS
            _LiveGetRoomList();
#endif
        }

        public override void LiveChat(string str)
        {
#if UNITY_IOS
            _LiveChat(str);
#endif
        }
        public override void LiveSendGift(string str)
        {
#if UNITY_IOS
            _LiveSendGift(str);
#endif
        }
        public override void LiveGetSchedule()
        {
#if UNITY_IOS
            _LiveGetSchedule();
#endif
        }

        #endregion

        public override void CheckPermissionNew(int requestCode, string permissionName, string permissionDes, string tipStr1, string tipStr2,
            string tipStr3)
        {
#if UNITY_IOS
            int permission = int.Parse(permissionName);
            _CheckPermissionNew(requestCode, permission, permissionDes, tipStr1, tipStr2, tipStr3);
#endif
            base.CheckPermissionNew(requestCode, permissionName, permissionDes, tipStr1, tipStr2, tipStr3);
        }

        public override void CheckBindPhone()
        {
#if UNITY_IOS
            _CheckBindPhone();
#endif
            base.CheckBindPhone();
        }

        public override void BindPhone()
        {

#if UNITY_IOS
            _BindPhone();
#endif
            base.BindPhone();
        }

        public override void CheckCertification()
        {

#if UNITY_IOS
            _CheckCertification();
#endif
            base.CheckCertification();
        }

        public override void IntentCertification()
        {
#if UNITY_IOS
            _IntentCertification();
#endif
            base.IntentCertification();
        }
        
        #region ActivitySdk
        
        /// <summary>
        /// 关闭Webview页面
        /// </summary>
        public override void CloseWebView()
        {
#if UNITY_IOS
            _CloseWebView();
#endif
            base.CloseWebView();
        }
        
        /// <summary>
        /// App/Native调用JS
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public override void ActivitySdkNativeToJs(string type, string data) 
        {
#if UNITY_IOS
            _ActivitySdkNativeToJs(type, data);
#endif
            base.ActivitySdkNativeToJs(type, data);
        }

        /// <summary>
        /// 游戏处理js回调事件
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="result"></param>
        public override void ActivitySdkOnJsActionResultCallback(string actionId, string result) 
        {
#if UNITY_IOS
            _ActivitySdkOnJsActionResultCallback(actionId, result);
#endif
            base.ActivitySdkOnJsActionResultCallback(actionId, result);
        }

        /// <summary>
        /// 设置活动sdk配置
        /// </summary>
        /// <param name="configString"></param>
        public override void ActivitySdkSetConfig(string configString)
        {
#if UNITY_IOS
            _ActivitySdkSetConfig(configString);
#endif
            base.ActivitySdkSetConfig(configString);
        }

        /// <summary>
        /// 重置活动sdk配置
        /// </summary>
        public override void ActivitySdkClearConfig()
        {
#if UNITY_IOS
            _ActivitySdkClearConfig();
#endif
            base.ActivitySdkClearConfig();
        }

        #endregion
        
        /// <summary>
        /// 将手机信息同步给Unity，例如渠道版本号
        /// </summary>
        public override void SyncPhoneInfoToUnity()
        {
#if UNITY_IOS
            _SyncPhoneInfoToUnity();
#endif
            base.SyncPhoneInfoToUnity();
        }
    }

}
