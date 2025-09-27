using System;
using System.Collections.Generic;

namespace Framework.PM
{
    [Serializable]
    public class S2C_PB
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        public class S2C_UserLogin
        {
            //详情    101 = 密码错误； 102 = 无该用户名； 103 = 
            public string Detail;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        public class S2C_UserRegister
        {
            //详情    104 = 已经含有该用户名; 105 = 用户名非法； 
            public string Detail;
        }
        
        /// <summary>
        /// 每一条插件信息
        /// </summary>
        [Serializable]
        public class S2C_PackageInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string ClientResPath { get; set; }
            public List<S2C_PackageData> Datas { get; set; }
        }

        /// <summary>
        /// 每一条插件信息的详细数据
        /// </summary>
        [Serializable]
        public class S2C_PackageData
        {
            public string Version { get; set; }
            public string UpdateTime { get; set; }
            public string ResServerPath { get; set; }
            public string Des { get; set; }
            public int Size { get; set; }
            public string Author { get; set; }
            public int DownloadCount { get; set; }
        }
       
        /// <summary>
        /// 上传是 服务器反馈的 信息 用于Log的记录
        /// </summary>
        public class S2C_LogPackageInfo
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string version { get; set; }
            public string clientResPath { get; set; }
            public string updateTime { get; set; }
            public string des { get; set; }
        }
    }
}