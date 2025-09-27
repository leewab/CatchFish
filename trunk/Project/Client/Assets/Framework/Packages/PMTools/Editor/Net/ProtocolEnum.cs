namespace Framework.PM
{
    public static class ProtocolEnum
    {
        /// <summary>
        /// 协议事件
        /// </summary>
        public enum ProtocolAction
        {
            //用户账号 注册
            USER_REGISTER = 10000,
            //用户账号 注销
            USER_REMOVE = 10001,
            //用户账号 登录
            USER_LOGIN = 10002,
            //用户账号 登出
            USER_LOGOUT = 10003, 
            
            //请求PM中的所有Package信息
            REQ_PM_DISPLAY = 10004,
            //请求PM中的下载
            REQ_PM_DOWNLIAD = 10005,
            
            //请求PM中的上传
            REQ_PM_UPLOAD = 10006,
            //请求上传的PackageVersion信息
            REQ_PM_UPLOAD_PACKAGEVERSION = 1000601,
            
            //请求PM管理的后台服务
            REQ_PM_SERVICE = 10007,
            //请求PM的清除
            REQ_PM_SERVICE_CLEAR = 1000701,

        }


        /// <summary>
        /// 协议反馈值
        /// </summary>
        public enum ProtocolValue
        {
            //登录成功
            USER_LOGIN_SUCCESS = 20000,
            //登录失败
            USER_LOGIN_FAIL = 20001,

            //注册成功
            USER_REGISTER_SUCCESS = 20002,
            //注册失败
            USER_REGISTER_FAIL = 20003,

            //下载成功
            RES_DOWNLOAD_SUCCESS = 20004,
            //下载失败
            RES_DOWNLOAD_FAIL = 20005,
            
            //上传成功
            RES_UPLOAD_SUCCESS = 20006,
            //上传失败
            RES_UPLOAD_FAIL = 20007,
            
            //预览申请成功
            RES_DISPLAY_SUCCESS = 20008,
            //预览申请失败
            RES_DISPLAY_FAIL = 20009,

            //package version请求成功
            RES_PACKAGEVERSION_SUCCESS = 20010,
            //package version请求失败
            RES_PACKAGEVERSION_FAIL = 20011,
        }
        
        /// <summary>
        /// 协议详情
        /// </summary>
        public enum ProtocolCode
        {
            //100 执行错误
            PROGRESS_ERROR = 100,
            //101 执行成功
            PROGRESS_SUCCESS = 101,
            //102 密码错误
            PASSWORD_ERROR = 102,
            //103 用户名空白
            USERNAME_EMPTY = 103,
            //104 用户名无效
            USERNAME_INVALID = 104,
            //105 用户名已有
            USERNAME_ALREADY = 105,
            //客户端请求的参数出错
            RES_PARAMS_ERROR = 106,
            //107 文件不存在
            RES_FILE_NONE = 107,
        }
    }

    
}