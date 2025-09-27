using UnityEngine;

namespace Game
{
    public class AgentBase
    {
        public virtual void Init()
        {

        }

        public virtual void Login()
        {

        }

        public virtual void Logout()
        {

        }

        public virtual void IsShowQrCodeScan()
        {

        }

        public virtual void ScanBusiness()
        {

        }

        public virtual void ScanBusinessAllPlatform(string param)
        {

        }

        public virtual bool IsHasLogin()
        {
            return false;
        }

        public virtual bool IsSDKInit()
        {
            return false;
        }

        public virtual int GetLoginCount()
        {
            return 0;
        }

        public virtual void Exit()
        {

        }

        public virtual void StartPersonHome()
        {
        }

        public virtual void EnterAppBBS()
        {
        }

        //2.2 设置主页面客服tab是否可见
        public virtual void SetCustomerServiceVisibility()
        {

        }

        public virtual void SyncUserInfo(string RoleId, string RoleName, string Lv, int ZoneId, string ZoneName,
            string PartyName, string Balance,
            string Vip, string RoleCreateTime)
        {

        }

        public virtual void SyncUserInfo2(string ZoneIdStr, string UnionId, string ProfessionId, string ProfessionName,
            string Sex,
            string Fight, string UnionTitleId, string UnionTitleName, string FriendList)
        {

        }

        public virtual void SubmitUserInfo_LOGIN(string info)
        {

        }

        public virtual void SubmitUserInfo_LOGIN_ERROR(string info)
        {

        }

        public virtual void SubmitUserInfo_CREATE_ROLE(string info)
        {

        }

        public virtual void SubmistUserInfo_ROLE_LEVEL_CHANGE(string info)
        {

        }

        public virtual void SubmitUserInfo_ROLE_EXIT(string info)
        {

        }

        public virtual void Pay(string orderid, int amount, string name, string id, string describe, string jsonparam,
            string callbackUrl)
        {

        }

        /// <summary>
        /// 海外支付
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="amount"></param>
        /// <param name="name"></param>
        /// <param name="productid"></param>
        /// <param name="describe"></param>
        /// <param name="jsonparam"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="serverid"></param>
        /// <param name="ext1"></param>
        /// <param name="ext2"></param>
        public virtual void Pay2(string orderid, int amount, string name, string productid, string describe,
            string jsonparam, string callbackUrl, string serverid, string ext1, string ext2)
        {

        }

        public virtual void LogEvent(string name, string param, int sdktype)
        {

        }

        public virtual void LogEventOnlyName(string eventName, int sdktype)
        {

        }

        public virtual void PushLocalNotification(long id, string title, string content, string date)
        {

        }

        public virtual void PushLocalNotificationLoop(long id, string title, string content, string date, int dayloop)
        {

        }

        public virtual void ClearLocalNotification()
        {

        }

        public virtual void OpenLaohuWebView(string url)
        {

        }

        public virtual string GetPacketName()
        {
            return Application.identifier;
        }

        public virtual string MakePhotoDir()
        {
            return "";
        }

        public virtual void LogPhone(string log)
        {
#if UNITY_EDITOR
            D.log(log);
#endif
        }

        public virtual void UnityInitFinished()
        {

        }

        public virtual void OpenAliService(string url)
        {

        }

        public virtual void RegisterBattery()
        {

        }

        public virtual void UploadFile(string key, string filePath)
        {

        }

        public virtual void UpLoadFileBucket(string bucket, string key, string filePath)
        {

        }

        public virtual void SetAccountId(string accountId)
        {

        }

        public virtual void DebugMemory(bool isDebug)
        {

        }

        public virtual string GetManufacture()
        {
            return "";
        }

        public virtual void IsNeedShowYYB()
        {

        }

        public virtual void ShowYYB()
        {

        }

        public virtual void ReStart()
        {

        }

        public virtual void ShowUserSurveyWebView()
        {

        }

        public virtual void ShowExitView()
        {

        }

        public virtual void UpdatePriceOnRegion(string ids)
        {

        }

        public virtual void CheckPermission(int permissionCode, string permission)
        {
        }

        public virtual bool IsNotch()
        {
            return false;
        }

        public virtual void OpenWebService()
        {

        }

        /// <summary>
        /// 打开网页
        /// </summary>
        /// <param name="urlStr"></param>
        public virtual void OpenWebUrl(string urlStr)
        {

        }

        public virtual void ShowPlatform()
        {

        }

        public virtual void HiddenPlatform()
        {

        }

        public virtual void CheckPermissionNew(int requestCode, string permissionName, string permissionDes,
            string tipStr1,
            string tipStr2, string tipStr3)
        {

        }

        public virtual void SyncPhoneInfoToUnity(string uid)
        {

        }

        public virtual void SuperSDKInvoke(string moduleName, string funcName, string param)
        {

        }

        public virtual void CheckBindPhone()
        {

        }

        public virtual void BindPhone()
        {

        }

        public virtual void CheckCertification()
        {

        }

        public virtual void IntentCertification()
        {

        }

        public virtual void OpenCommunity()
        {

        }

        /// <summary>
        /// 防沉迷预登录接口（获取防沉迷信息）
        /// </summary>
        /// <param name="userId"></param>
        public virtual void FatigueBeforeRoleLogin(string userId)
        {

        }

        /// <summary>
        /// 防沉迷角色登录接口
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serverId"></param>
        /// <param name="roleId"></param>
        public virtual void FatigueAfterRoleLogin(string userId, int serverId, string roleId)
        {

        }

        /// <summary>
        /// 防沉迷角色登出接口
        /// </summary>
        public virtual void FatigueAfterRoleLogout()
        {

        }

        /// <summary>
        /// 打开相关法律条款页面接口
        /// </summary>
        public virtual void OpenComplianceOnWebView()
        {

        }

        /// <summary>
        /// 清除防沉迷缓存
        /// </summary>
        public virtual void FatigueClearCache()
        {

        }

        /// <summary>
        /// 获取SDK提供的设备ID
        /// </summary>
        /// <returns></returns>
        public virtual string GetSDKDeviceId()
        {
            return "";
        }        
        
        public virtual void NotifyAlbum(string path)
        {
            
        }

        /// <summary>
        /// SDK特殊方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="jsonStr"></param>
        public virtual void SDKCommonCallFunction(string methodName, string jsonStr)
        {

        }

        public virtual bool CheckObbExists()
        {
            return false;
        }

        public virtual void LiveSetAccountId(string accountId)
        {

        }

        public virtual void LiveDebugMode(bool b)
        {

        }

        public virtual void LiveSetParams(string gameId, string nick)
        {

        }

        public virtual void LiveDoStart()
        {

        }

        public virtual void LiveDoStartByRoomId(string roomId)
        {

        }

        public virtual void LiveStop()
        {

        }

        public virtual void LiveGetRoomList()
        {

        }

        public virtual void LiveChat(string str)
        {

        }

        public virtual void LiveSendGift(string str)
        {

        }

        public virtual void LiveGetSchedule()
        {

        }

        /// <summary>
        /// 关闭Webview页面
        /// </summary>
        public virtual void CloseWebView()
        {

        }

        /// <summary>
        /// 获取设备的系统语言
        /// </summary>
        /// <returns></returns>
        public virtual string GetLanguage()
        {
            return "CH";
        }

        /// <summary>
        /// App/Native调用JS
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public virtual void ActivitySdkNativeToJs(string type, string data)
        {

        }

        /// <summary>
        /// 游戏处理js回调事件
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="result"></param>
        public virtual void ActivitySdkOnJsActionResultCallback(string actionId, string result)
        {

        }

        /// <summary>
        /// 设置活动sdk配置
        /// </summary>
        /// <param name="configString"></param>
        public virtual void ActivitySdkSetConfig(string configString)
        {

        }

        /// <summary>
        /// 重置活动sdk配置
        /// </summary>
        public virtual void ActivitySdkClearConfig()
        {

        }


        /// <summary>
        /// 判断是否安装了某个应用
        /// </summary>
        /// <param name="packageName"></param>
        /// <returns></returns>
        public virtual bool CheckPackage(string packageName)
        {
            return false;
        }


        /// <summary>
        /// 将手机信息同步给Unity，例如渠道版本号
        /// </summary>
        public virtual void SyncPhoneInfoToUnity()
        {

        }

        /// <summary>
        /// 查询是否支持线性马达振动能力
        /// </summary>
        /// <returns></returns>
        public virtual string GetHWHapticsGradeValue()
        {
            return "unsupport";
        }

        /// <summary>
        /// 设置线性马达振动参数
        /// </summary>
        public virtual void SetHWHapticsGradeValue(string strType)
        {

        }

        #region VivoGame

        // vivo游戏引擎初始化
        public virtual void VGBInit()
        {
            
        }

        // vivo游戏引擎开启
        public virtual void VGBStart() 
        {
            
        }
        // vivo游戏引擎关闭
        public virtual void VGBStop() 
        {
            
        }
        // vivo游戏引擎发送消息
        public virtual void VGBSend(string jsonStr)	
        {
            
        }
        // Unity线程 id
        public virtual string GetUnityThreadId()
        {
            return "";
        }

        #endregion
        
        #region Dalan

        /// <summary>
        /// 登陆成功回传SDK
        /// </summary>
        /// <param name="jsonStr"></param>
        public virtual void LoginResponse(string jsonStr)
        {
            
        }

        #endregion
    }
}
