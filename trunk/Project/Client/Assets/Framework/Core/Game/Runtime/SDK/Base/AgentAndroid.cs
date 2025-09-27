using UnityEngine;

namespace Game
{
    public class AgentAndroid : AgentBase
    {
        static AndroidJavaObject m_activity;

        public override void Init()
        {
            base.Init();

            using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                m_activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        // 登录
        public override void Login()
        {
#if UNITY_ANDROID
            m_activity.Call("Login");
#endif 
            base.Login();
        }

        // 登出
        public override void Logout()
        {
#if UNITY_ANDROID
            m_activity.Call("Logout");
#endif
            base.Logout();
        }

        public override void IsShowQrCodeScan()
        {
#if UNITY_ANDROID
            m_activity.Call("IsShowQrCodeScan");
#endif        
            base.IsShowQrCodeScan();
        }

        public override void ScanBusiness()
        {
#if UNITY_ANDROID
            m_activity.Call("ScanBusiness");
#endif
            base.ScanBusiness();
        }

        public override void ScanBusinessAllPlatform(string param)
        {
#if UNITY_ANDROID
            m_activity.Call("ScanBusinessAllPlatform", param);
#endif
            base.ScanBusinessAllPlatform(param);
        }

        // 判断sdk是否登录
        public override bool IsHasLogin()
        {
#if UNITY_ANDROID
            return m_activity.Call<bool>("IsHasLogin");
#endif
            return base.IsHasLogin();
        }

        // 判断sdk是否已经初始化
        public override bool IsSDKInit()
        {
#if UNITY_ANDROID
            return m_activity.Call<bool>("IsSDKInit");
#endif
            return base.IsSDKInit();
        }

        // 获取登录次数
        public override int GetLoginCount()
        {
#if UNITY_ANDROID
            return m_activity.Call<int>("GetLoginCount");
#endif
            return base.GetLoginCount();
        }

        // 退出游戏
        public override void Exit()
        {
#if UNITY_ANDROID
            m_activity.Call("Exit");
#endif
            base.Exit();
        }

        // 同步玩家信息
        public override void SyncUserInfo(string RoleId, string RoleName, string Lv, int ZoneId, string ZoneName, string PartyName, string Balance, string Vip, string RoleCreateTime)
        {
#if UNITY_ANDROID
            m_activity.Call("SyncUserInfo", RoleId, RoleName, Lv, ZoneId, ZoneName, PartyName, Balance, Vip, RoleCreateTime );
#endif
            base.SyncUserInfo(RoleId, RoleName, Lv, ZoneId, ZoneName, PartyName, Balance, Vip, RoleCreateTime);
        }

        // 同步玩家信息(阿里补充)
        public override void SyncUserInfo2(string ZoneIdStr, string UnionId, string ProfessionId, string ProfessionName, string Sex,
                                  string Fight, string UnionTitleId, string UnionTitleName, string FriendList)
        {
#if UNITY_ANDROID
            m_activity.Call("SyncUserInfo2", ZoneIdStr, UnionId, ProfessionId, ProfessionName, Sex, Fight, UnionTitleId, UnionTitleName, FriendList);      
#endif
            base.SyncUserInfo2(ZoneIdStr, UnionId, ProfessionId, ProfessionName, Sex, Fight, UnionTitleId, UnionTitleName, FriendList);
        }

        // 登录时同步给OneSDK
        public override void SubmitUserInfo_LOGIN(string info)
        {
#if UNITY_ANDROID
            m_activity.Call("SubmitUserInfo_LOGIN", info);
#endif
            base.SubmitUserInfo_LOGIN(info);
        }

        // 登录失败时同步给OneSDK
        public override void SubmitUserInfo_LOGIN_ERROR(string info)
        {
#if UNITY_ANDROID
            m_activity.Call("SubmitUserInfo_LOGIN_ERROR", info);
#endif
            base.SubmitUserInfo_LOGIN_ERROR(info);
        }

        // 创角时同步给OneSDK
        public override void SubmitUserInfo_CREATE_ROLE(string info)
        {
#if UNITY_ANDROID
            m_activity.Call("SubmitUserInfo_CREATE_ROLE", info);
#endif
            base.SubmitUserInfo_CREATE_ROLE(info);
        }

        // 升级时同步给OneSDK
        public override void SubmistUserInfo_ROLE_LEVEL_CHANGE(string info)
        {
#if UNITY_ANDROID
            m_activity.Call("SubmitUserInfo_ROLE_LEVEL_CHANGE");
#endif
            base.SubmistUserInfo_ROLE_LEVEL_CHANGE(info);
        }

        public override void SubmitUserInfo_ROLE_EXIT(string info)
        {
#if UNITY_ANDROID
            m_activity.Call("SubmitUserInfo_ROLE_EXIT");
#endif
            base.SubmitUserInfo_ROLE_EXIT(info);
        }

        // 支付
        public override void Pay(string orderid, int amount, string name, string id, string describe, string jsonparam,string callbackUrl)
        {
#if UNITY_ANDROID
            m_activity.Call("Pay", orderid, amount, name, id, describe, jsonparam, callbackUrl);
#endif
            base.Pay(orderid, amount, name, id, describe, jsonparam, callbackUrl);
        }

        // 统计事件（打点）
        public override void LogEvent(string name, string param, int sdktype)
        {
#if UNITY_ANDROID

            m_activity.Call("LogEvent", name, param, sdktype);
#endif
            base.LogEvent(name, param, sdktype);
        }

        // 统计事件（打点）
        public override void LogEventOnlyName(string eventName, int sdktype)
        {
#if UNITY_ANDROID
            m_activity.Call("LogEventOnlyName", eventName, sdktype);
#endif
            base.LogEventOnlyName(eventName, sdktype);
        }

        // 本地推送（单次）
        public override void PushLocalNotification(long id, string title, string content, string date)
        {
#if UNITY_ANDROID
            m_activity.Call("PushLocalNotification", id, title, content, date);
#endif
            base.PushLocalNotification(id, title, content, date);
        }

        // 本地推送（多次）
        public override void PushLocalNotificationLoop(long id, string title, string content, string date, int dayloop)
        {
#if UNITY_ANDROID
            m_activity.Call("PushLocalNotificationLoop", id, title, content, date, dayloop);
#endif
            base.PushLocalNotificationLoop(id, title, content, date, dayloop);
        }

        // 清理本地推送
        public override void ClearLocalNotification()
        {
#if UNITY_ANDROID
            m_activity.Call("ClearLocalNotification");
#endif
            base.ClearLocalNotification();
        }

        // 老虎平台内，调用浏览器
        public override void OpenLaohuWebView(string url)
        {
#if UNITY_ANDROID
            m_activity.Call("OpenLaohuWebView", url);
#endif
            base.OpenLaohuWebView(url);
        }

        // 获取包名 
        public override string GetPacketName()
        {
            return base.GetPacketName();
        }

        // 建立一个公共目录，存放一些项目产生的图片，这些图片的特点是能在手机相册中看到，例如二维码
        public override string MakePhotoDir()
        {
#if UNITY_ANDROID
            return m_activity.Call<string>("MakePhotoDir");
#endif
            return base.MakePhotoDir();
        }

        // 写手机log
        public override void LogPhone(string log)
        {
#if UNITY_ANDROID
            m_activity.Call("LogPhone", log);
#endif
            base.LogPhone(log);
        }

        public override void UnityInitFinished()
        {
#if UNITY_ANDROID
            m_activity.Call("UnityInitFinished");
#endif
            base.UnityInitFinished();
        }

        public override void OpenAliService(string url)
        {
#if UNITY_ANDROID
            m_activity.Call("StartWebView", url);
#endif
            base.OpenAliService(url);
        }

        public override void RegisterBattery()
        {
#if UNITY_ANDROID
            m_activity.Call("RegisterBattery");
#endif
            base.RegisterBattery();
        }

        public override void UploadFile(string key, string filePath)
        {
#if UNITY_ANDROID
            m_activity.Call("UploadFile", key, filePath);
#endif
            base.UploadFile(key, filePath);
        }

        public override void UpLoadFileBucket(string bucket, string key, string filePath)
        {
#if UNITY_ANDROID
            m_activity.Call("UpLoadFileBucket", bucket, key, filePath);
#endif
            base.UpLoadFileBucket(bucket, key, filePath);
        }

        public override void SetAccountId(string accountId)
        {

            base.SetAccountId(accountId);
        }

        public override void DebugMemory(bool isDebug)
        {
#if UNITY_ANDROID
            m_activity.Call("DebugMemory", isDebug);
#endif
            base.DebugMemory(isDebug);
        }

        public override string GetManufacture()
        {
#if UNITY_ANDROID
            m_activity.Call<string>("GetManufacture");
#endif
            return base.GetManufacture();
        }

        public override void IsNeedShowYYB()
        {
#if UNITY_ANDROID
            m_activity.Call("IsNeedShowYYB");
#endif
            base.IsNeedShowYYB();
        }

        public override void ShowYYB()
        {
#if UNITY_ANDROID
            m_activity.Call("ShowYYB");
#endif
            base.ShowYYB();
        }

        public override void ReStart()
        {
#if UNITY_ANDROID
            m_activity.Call("ReStart");
#endif
            base.ReStart();
        }
        public override void CheckPermission(int permissionCode, string permission)
        {
#if UNITY_ANDROID
            m_activity.Call("CheckPermission", permissionCode, permission);
#endif
            base.CheckPermission(permissionCode, permission);
        }

        public override bool IsNotch()
        {
#if UNITY_ANDROID
            return m_activity.Call<bool>("IsNotch");
#endif
            return base.IsNotch();
        }

        public override void CheckPermissionNew(int requestCode, string permissionName, string permissionDes, string tipStr1,
  string tipStr2, string tipStr3)
        {
#if UNITY_ANDROID
            m_activity.Call("CheckPermissionNew", requestCode, permissionName, permissionDes, tipStr1, tipStr2, tipStr3);
#endif
            base.CheckPermissionNew(requestCode, permissionName, permissionDes, tipStr1, tipStr2, tipStr3);
        }

        /// <summary>
        /// 防沉迷预登录接口（获取防沉迷信息）
        /// </summary>
        /// <param name="userId"></param>
        public override void FatigueBeforeRoleLogin(string userId)
        {
#if UNITY_ANDROID
            m_activity.Call("FatigueBeforeRoleLogin", userId);
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
#if UNITY_ANDROID
            m_activity.Call("FatigueAfterRoleLogin", userId, serverId, roleId);
#endif
            base.FatigueAfterRoleLogin(userId, serverId, roleId);
        }

        /// <summary>
        /// 防沉迷角色登出接口
        /// </summary>
        public override void FatigueAfterRoleLogout()
        {
#if UNITY_ANDROID
            m_activity.Call("FatigueAfterRoleLogout");
#endif
            base.FatigueAfterRoleLogout();
        }
        
        /// <summary>
        /// 打开相关法律条款页面接口
        /// </summary>
        public override void OpenComplianceOnWebView()
        {
#if UNITY_ANDROID
            m_activity.Call("OpenComplianceOnWebView");
#endif
            base.OpenComplianceOnWebView();
        }
        /// <summary>
        /// 清除防沉迷缓存
        /// </summary>
        public override void FatigueClearCache()
        {
#if UNITY_ANDROID
            m_activity.Call("FatigueClearCache");
#endif
            base.FatigueClearCache();
        }

        public override void OpenWebUrl(string urlStr)
        {
#if UNITY_ANDROID
            m_activity.Call("OpenWebUrl", urlStr);
#endif
            base.OpenWebUrl(urlStr);
        }
        
        /// <summary>
        /// SDK特殊方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="jsonStr"></param>
        public override void SDKCommonCallFunction(string methodName, string jsonStr)
        {
#if UNITY_ANDROID
            m_activity.Call("SDKCommonCallFunction", methodName, jsonStr);
#endif
            base.SDKCommonCallFunction(methodName, jsonStr);
        }


        #region live sdk
        public override void LiveSetAccountId(string accountId)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveSetUserId", accountId); 
#endif
        }

        public override void LiveDebugMode(bool b)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveSetDebugMode", b); 
#endif
        }

        public override void LiveSetParams(string gameId, string nick)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveSetParams", gameId, nick);
#endif
        }

        public override void LiveDoStart()
        {
#if UNITY_ANDROID
            m_activity.Call("LiveDoStart");
#endif
        }

        public override void LiveDoStartByRoomId(string roomId)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveDoStartByRoomId", roomId);
#endif
        }

        public override void LiveStop()
        {
#if UNITY_ANDROID
            m_activity.Call("LiveStop");
#endif
        }

        public override void LiveGetRoomList()
        {
#if UNITY_ANDROID
            m_activity.Call("LiveGetRoomList");
#endif
        }

        public override void LiveChat(string str)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveChat", str);
#endif
        }
        public override void LiveSendGift(string str)
        {
#if UNITY_ANDROID
            m_activity.Call("LiveSendGift", str);
#endif
        }
        public override void LiveGetSchedule()
        {
#if UNITY_ANDROID
            m_activity.Call("LiveGetSchedule");
#endif
        }


        public override void CheckBindPhone()
        {
#if UNITY_ANDROID
            SDKCommonCallFunction("laohu_getPhone", "");
#endif
            base.CheckBindPhone();
        }

        public override void BindPhone()
        {

#if UNITY_ANDROID
            SDKCommonCallFunction("laohu_startBindOrChangeBindPhone", "");
#endif
            base.BindPhone();
        }

        public override void CheckCertification()
        {

#if UNITY_ANDROID
            SDKCommonCallFunction("funGetCertificationInfo", "");
#endif
            base.CheckCertification();
        }

        public override void IntentCertification()
        {
#if UNITY_ANDROID
            SDKCommonCallFunction("funGetCertificationIntent", "");
#endif
            base.IntentCertification();
        }

        #endregion

        #region ActivitySdk
        
        /// <summary>
        /// 关闭Webview页面
        /// </summary>
        public override void CloseWebView()
        {
#if UNITY_ANDROID
            m_activity.Call("CloseWebView");
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
#if UNITY_ANDROID
            m_activity.Call("ActivitySdkNativeToJs", type, data);
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
#if UNITY_ANDROID
            m_activity.Call("ActivitySdkOnJsActionResultCallback", actionId, result);
#endif
            base.ActivitySdkOnJsActionResultCallback(actionId, result);
        }

        /// <summary>
        /// 设置活动sdk配置
        /// </summary>
        /// <param name="configString"></param>
        public override void ActivitySdkSetConfig(string configString)
        {
#if UNITY_ANDROID
            m_activity.Call("ActivitySdkSetConfig", configString);
#endif
            base.ActivitySdkSetConfig(configString);
        }

        /// <summary>
        /// 重置活动sdk配置
        /// </summary>
        public override void ActivitySdkClearConfig()
        {
#if UNITY_ANDROID
            m_activity.Call("ActivitySdkClearConfig");
#endif
            base.ActivitySdkClearConfig();
        }
        #endregion

		/// <summary>
        /// 判断是否安装了某个应用
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public override bool CheckPackage(string packageName)
        {
#if UNITY_ANDROID
            return m_activity.Call<bool>("CheckPackage", packageName);
#endif
            return base.CheckPackage(packageName);
        }

        public override void SyncPhoneInfoToUnity()
        {
#if UNITY_ANDROID
            m_activity.Call("SyncPhoneInfoToUnity");
#endif
            base.SyncPhoneInfoToUnity();
        }
    }
}
