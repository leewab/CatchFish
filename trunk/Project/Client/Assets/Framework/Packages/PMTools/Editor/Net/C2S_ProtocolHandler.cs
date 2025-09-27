using System.Collections.Generic;
using Framework.Core;
using UnityEngine;

namespace Framework.PM
{
    public class C2S_ProtocolHandler
    {
        private string mPMURL = PM_Define.ServerURL + "/pm?protocol={0}";
        private string mPMAdminURL = PM_Define.ServerURL + "/pm/admin";
        
        /// <summary>
        /// 请求PM中的所有Package信息
        /// </summary>
        public void Req_PM_Packages()
        {
            HttpHelper.HttpRequestFromGet(string.Format(mPMURL, (int)ProtocolEnum.ProtocolAction.REQ_PM_DISPLAY));
        }

        /// <summary>
        /// 请求PM的后台服务
        /// </summary>
        public void Req_PM_Service()
        {
            Application.OpenURL(string.Format(mPMURL, (int) ProtocolEnum.ProtocolAction.REQ_PM_SERVICE));
        }

        /// <summary>
        /// 根据clientResPath请求当前的PackageVersion
        /// </summary>
        public void Req_PM_Upload_PackageVersion(string _clientResPath)
        {
            Dictionary<string, string> bodyDic = new Dictionary<string, string>
            {
                {"protocol", ((int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD_PACKAGEVERSION).ToString()},
                {"clientResPath", _clientResPath}
            };
            HttpHelper.HttpRequestFormPost(PM_Define.UploadURL, bodyDic, "upload");
        }

        public void Req_PM_Upload()
        {
            Dictionary<string, string> bodyDic = new Dictionary<string, string>
            {
                {"protocol", ((int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD).ToString()},
            };
            
        }

        /// <summary>
        /// 根据operation清除当前的数据
        ///     one  清除本条数据及文件
        ///     *    清除所有的数据及文件
        /// </summary>
        /// <param name="_operation"></param>
        public void Req_PM_DeleteRemote(C2S_PB.C2S_PackageInfo _pmInfo, string _operation = "one")
        {
            Dictionary<string, string> bodyDic = new Dictionary<string, string>
            {
                {"protocol", ((int)ProtocolEnum.ProtocolAction.REQ_PM_SERVICE_CLEAR).ToString()},
                {"operation", _operation},
                {"id", "-1"},
                {"name", _pmInfo.Name},
                {"type", _pmInfo.Type},
                {"version", _pmInfo.ClientVersion},
                {"clientResPath", _pmInfo.ClientResPath},
                {"des", _pmInfo.Des},
                {"author", _pmInfo.Author},
            };
            HttpHelper.HttpRequestFormPost(PM_Define.PmURL, bodyDic, "upload");
        }
    }
}
