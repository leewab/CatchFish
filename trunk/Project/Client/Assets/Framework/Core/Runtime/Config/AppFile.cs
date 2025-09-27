using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// 资源更新步骤
    /// 1、本地、远端同时存放AppFile文件,本地AppFile加密
    /// 2、本地通过ResURL下载远端AppFile
    /// 3、比较远端AppVersion与本地AppVersion，如果远端版本号高于本地版本号，进行应用软件更新
    /// 4、比较远端ResVersion与本地ResVersion，如果远端版本号高于本地版本号，进行资源更新
    /// 5、远端AppFile覆盖本地AppFile文件
    /// 6、通过WebURL申请区服列表
    /// </summary>
    public class AppFile
    {
        public string AppVersion;                   //应用打包版本号（从0开始，格式 x）
        public string ResVersion;                   //资源版本号（从01开始，格式 x.x）
        public bool IsLocalLog;                     //是否开启本地log (用于打印本地开发log)
        public bool IsRemoteLog;                    //是否开启远端log (用于抓取线上玩家、测试的log)
        public bool IsOpenHotfix;                   //是否开启热更
        public string ServerID;                     //资源服Id
        public string RootName;                     //资源目录
        public string AppName;                      //名称
        public string ResURL;                       //资源地址
        public string ServerListURL;                //区服列表
        public string AppURL;                       //应用地址
        public string SecretKey;                    //密钥
        public string ChannelTag;                   //渠道
        public ChannelGroup ChannelGroup;           //渠道名称
        public ChannelPlatform ChannelPlatform;     //渠道发行平台名称
        
    }
}