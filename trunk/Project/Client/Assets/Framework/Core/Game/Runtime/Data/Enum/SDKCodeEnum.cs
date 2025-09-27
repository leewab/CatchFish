namespace Game
{
    public class SDKCodeEnum
    {
        // 登录 //
        public const string CALL_BACK_TYPE_LOGIN = "login_result";

        // 登出 //
        public const string CALL_BACK_TYPE_LOGOUT = "logout_result";

        // 登录取消 //
        public const string CALL_BACK_TYPE_LOGCANCEL = "logcancel_result";

        // 用户信息，包括“渠道id”，“渠道名字”等 //
        public const string CALL_BACK_TYPE_USERINFO = "userinfo_result";

        // 安卓回退键 //
        public const string CALL_BACK_KEYCODE_BACK = "keycode_back";

        // 电池电量 //
        public const string CALL_BACK_BATTERY_RECEIVER = "battery_receiver";

        // 阿里云上传文件 //
        public const string ALIYUN_UPLOAD = "aliyun_upload";

        // 扫码请求 //
        public const string CALL_BACK_IS_SHOW_QR_CODE_SCAN = "isShowQrCodeScan";

        // 同步内存信息 //
        public const string CALL_BACK_MEMORY = "memory";

        // 内存不够通知1,进程还在 //
        public const string CALL_BACK_ON_TRIM_MEMORY = "onTrimMemory";

        // 内存不够通知2，进程不在了 //
        public const string CALL_BACK_ON_LOW_MEMORY = "onLowMemory";

        // 需要显示应用宝logo //
        public const string CALL_CACK_SHOW_YYB_LOGO = "ShowYYB";

        //苹果推送token
        public const string CALL_BACK_IOS_PUSH = "ios_push";

        //安卓权限
        public const string CALL_ANDROID_PERMISSION_CHECK = "android_permission_check";

        //安卓区域商品价格
        public const string CALL_BACK_REGION_PRICE_LIST = "android_region_price";

        public const string CALL_BACK_EXIT_GAME = "exit_game";

        // 防沉迷
        public const string CALL_BACK_TYPE_FATIGUE_RESULT = "fatigue_result";

        // 分享
        public const string CALL_BACK_TYPE_SHARE_RESULT = "share_result";

        // SDK特殊方法
        public const string CALL_BACK_TYPE_SDK_COMMON_CALL_FUNCTION_RESULT = "sdk_common_call_function_result";

        // SDK推送token
        public const string CALL_BACK_PUSH_TOKEN_RESULT = "push_token_result";

        // 活动SDK
        public const string CALL_BACK_ACTIVITYSDK_ONJSACTION_RESULT = "activitysdk_onjsaction_result";
        public const string CALL_BACK_ACTIVITYSDK_ONWEBCLOSE_RESULT = "activitysdk_onwebclose_result";

        // 购买
        public const string CALL_BACK_PAY = "pay_result";

        // 手机配置变化
        public const string CALL_BACK_CONFIGURATION_CHANGE = "onConfigurationChanged";

        public const string SUCCESS = "success";
        public const string FAILED = "failed";
    }
}