using System.Collections.Generic;
using System.Linq;
using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    internal class PM_DisplayModule : BaseEditorModule
    {
        /// <summary>
        /// 记录服务器获取的所有package数据
        /// </summary>
        private List<S2C_PB.S2C_PackageInfo> mPMAllPackageInfos = null;
        public List<S2C_PB.S2C_PackageInfo> PMAllPackageInfos => mPMAllPackageInfos;

        /// <summary>
        /// 通过标题类型缓存package数据
        /// </summary>
        private Dictionary<string, List<C2S_PB.C2S_PackageInfo>> mPMAllPackagesDic = null;
        public Dictionary<string, List<C2S_PB.C2S_PackageInfo>> PMAllPackagesDic => mPMAllPackagesDic;

        /// <summary>
        /// 标题列表
        /// </summary>
        private List<string> mTitles = null;
        public List<string> Titles => mTitles;

        /// <summary>
        /// 是否初始化数据结束
        /// </summary>
        private bool mIsInited = false;
        public bool IsInited => mIsInited;

        /// <summary>
        /// 申请数据
        /// </summary>
        public void RequestData()
        {
            if (mPMAllPackageInfos == null) ProtocolManager.C2SProtocolHander.Req_PM_Packages();
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="_result"></param>
        /// <returns></returns>
        public bool ConvertInfos(string _result)
        {
            Debug.Log(_result);
            mPMAllPackageInfos = JsonHelper.Convert<List<S2C_PB.S2C_PackageInfo>>(_result);
            if (mPMAllPackageInfos == null || mPMAllPackageInfos.Count <= 0)
            {
                return false;
            }
            //初始化数据
            InitInfos();
            return true;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="_pmRemoteInfos"></param>
        public void InitInfos()
        {
            if (null == mTitles)
                mTitles = new List<string>();
            else
                mTitles.Clear();

            if (null == mPMAllPackagesDic)
                mPMAllPackagesDic = new Dictionary<string, List<C2S_PB.C2S_PackageInfo>>();
            else
                mPMAllPackagesDic.Clear();
            
            foreach (var remoteInfo in mPMAllPackageInfos)
            {
                if (remoteInfo?.Type == null) continue;
                if (!mTitles.Contains(remoteInfo.Type))
                {
                    mTitles.Add(remoteInfo.Type);
                }

                var clientVersion = GetClientVersion(remoteInfo.ClientResPath) ?? " ";
                List<C2S_PB.C2S_PackageInfo> pmInfos = null;
                if (mPMAllPackagesDic.TryGetValue(remoteInfo.Type, out pmInfos))
                {
                    var pData = remoteInfo.Datas?.Last();
                    pmInfos.Add(new C2S_PB.C2S_PackageInfo
                    {
                        Id = remoteInfo.Id,
                        Name = remoteInfo.Name,
                        Type = remoteInfo.Type,
                        ClientVersion = clientVersion,
                        ClientResPath = remoteInfo.ClientResPath,
                        ServerVersion = pData?.Version,
                        UpdateTime = pData?.UpdateTime,
                        Des = pData?.Des,
                        Size = pData?.Size,
                        Author = pData?.Author,
                    });
                }
                else
                {
                    var pData = remoteInfo.Datas?.Last();
                    pmInfos = new List<C2S_PB.C2S_PackageInfo>
                    {
                        new C2S_PB.C2S_PackageInfo
                        {
                            Id = remoteInfo.Id,
                            Name = remoteInfo.Name,
                            Type = remoteInfo.Type,
                            ClientVersion = clientVersion,
                            ClientResPath = remoteInfo.ClientResPath,
                            ServerVersion = pData?.Version,
                            UpdateTime = pData?.UpdateTime,
                            Des = pData?.Des,
                            Size = pData?.Size,
                            Author = pData?.Author,
                        }
                    };
                    mPMAllPackagesDic.Add(remoteInfo.Type, pmInfos);
                }
            }

            mIsInited = true;
        }

        public void Clear()
        {
            mPMAllPackageInfos = null;
            mPMAllPackagesDic = null;
            mTitles = null;
            mIsInited = false;
        }
        
        /// <summary>
        /// 获取客户端的版本信息
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        private string GetClientVersion(string _assetPath)
        {
            var file = _assetPath + "/VersionInfo.json";
            if (!IOHepler.FileExists(file)) return null;
            var pmInfos = JsonHelper.LoadSync<List<C2S_PB.C2S_PackageInfo>>(file);
            return pmInfos.Last()?.ClientVersion;
        }

        /// <summary>
        /// 比较localVersion 和 remoteVersion
        ///     -2 表示本地无版本
        ///     -1 表示本地localVersion低版本
        ///     0  表示版本同步
        ///     1  表示本地高出remoteVersion版本
        ///     2  表示远端无版本
        /// </summary>
        /// <param name="_localVersion"></param>
        /// <param name="_remoteVersion"></param>
        /// <returns></returns>
        public int CompareVersion(string _localVersion, string _remoteVersion)
        {
            if (null == _localVersion || _localVersion.Equals("") || _localVersion.Equals(" ")) return -2;   //本地无版本
            if (null == _remoteVersion || _remoteVersion.Equals("") || _remoteVersion.Equals(" ")) return 2; //远端无版本

            string[] localVs = _localVersion.Split('.');
            string[] remoteVs = _remoteVersion.Split('.');
            if (localVs.Length != remoteVs.Length) Debug.LogError("Version 位数不对！");

            for (int i = 0; i < localVs.Length; i++)
            {
                int localVer = ConvertHelper.String2Int(localVs[i]);
                int remoteVer = ConvertHelper.String2Int(remoteVs[i]);
                if (localVer > remoteVer) return 1;                       //本地 > 远端
                if (localVer == remoteVer) continue;                      //本地 = 远端
                if (localVer < remoteVer) return -1;                      //本地 < 远端
            }

            return 0;
        }
    }
}