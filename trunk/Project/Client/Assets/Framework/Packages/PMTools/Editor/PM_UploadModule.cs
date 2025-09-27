using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using System.Linq;
using Framework.Core;

namespace Framework.PM
{
    internal class PM_UploadModule : BaseEditorModule
    {
        /// <summary>
        /// 是否初始化完毕
        /// </summary>
        private bool mIsInited = false;
        public bool IsInited => mIsInited;

        /// <summary>
        /// 服务器获取有关该插件的协议信息 随后编辑上传的数据
        /// </summary>
        private C2S_PB.C2S_PackageInfo mPackageInfo = null;
        public C2S_PB.C2S_PackageInfo PackageInfo => mPackageInfo;
        
        /// <summary>
        /// 本地VersionInfo中的PackageInfo数据
        /// </summary>
        private List<C2S_PB.C2S_PackageInfo> mLocalPackageInfos = new List<C2S_PB.C2S_PackageInfo>();

        //VersionInfo文本
        private string versionInfoPath = "/VersionInfo.json";

        //当前选中对象的路径
        private string curSelectedPath = null;
        private string curSelectedName = null;

        /// <summary>
        /// 生产本地VersionInfo
        /// </summary>
        internal void GenerateLocalInfo(Action _callBack)
        {
            if (mPackageInfo == null) return;
            //服务器版本号同步客户端版本号
            mPackageInfo.ServerVersion = mPackageInfo.ClientVersion;
            mPackageInfo.Author = "leewab";
            //添加新一条
            mLocalPackageInfos.Add(mPackageInfo);
            //写入VersionInfo
            JsonHelper.Save(mLocalPackageInfos, versionInfoPath, result =>
            {
                AssetDatabase.Refresh();
                //开始上传
                StartUpload(mPackageInfo, _callBack);
            });
        }

        /// <summary>
        /// 开始上传 
        /// </summary>
        /// <param name="_pmUploadInfo"> 为根据客户端填写之后 修改之后的值 </param>
        /// <param name="_callBack"></param>
        private void StartUpload(C2S_PB.C2S_PackageInfo _pmUploadInfo, Action _callBack)
        {
            if (null == _pmUploadInfo) return;
            EditorHelper.ShowProgressBar(0, 4, "上传文件", "正在导出...");
            var file = $"{Application.dataPath}/unity_{_pmUploadInfo.Type}_{_pmUploadInfo.Name}_v{_pmUploadInfo.ServerVersion}.unitypackage";
            Debug.Log(_pmUploadInfo.ClientResPath);
            Debug.Log(file);
            AssetDatabase.ExportPackage(_pmUploadInfo.ClientResPath, file, ExportPackageOptions.Recurse);
            AssetDatabase.Refresh();
            EditorHelper.ShowProgressBar(1, 4, "上传文件", "导出成功...");
            Dictionary<string, string> nvc = new Dictionary<string, string>
            {
                {"protocol", ((int)ProtocolEnum.ProtocolAction.REQ_PM_UPLOAD).ToString()},
                {"id", _pmUploadInfo.Id.ToString()},
                {"name", _pmUploadInfo.Name},
                {"type", _pmUploadInfo.Type},
                {"version", _pmUploadInfo.ClientVersion},
                {"clientResPath", _pmUploadInfo.ClientResPath},
                {"des", _pmUploadInfo.Des},
                {"author", _pmUploadInfo.Author},
            };
            EditorHelper.ShowProgressBar(2, 4, "上传文件", "正在上传...");
            UploadPack(new string[] {file}, nvc, (b, info) =>
            {
                if (b)
                {
                    Debug.Log("上传成功");
                    EditorHelper.ShowProgressBar(3, 4, "上传文件", "上传成功...");
                }
                else
                {
                    Debug.Log("上传失败");
                }

                EditorHelper.ShowProgressBar(4, 4, "上传文件", $"正在写入VersionInfo.../n {versionInfoPath}");
                //删除导入文件
                IOHepler.DeleteFile(file);
                //刷新
                AssetDatabase.Refresh();
                //结束回调
                _callBack?.Invoke();
            });
        }
        
        /// <summary>
        /// 选中执行
        /// </summary>
        /// <returns></returns>
        private void ExcuteClient()
        {
            //3.搜寻本地缓存信息
            if (string.IsNullOrEmpty(curSelectedPath)) return;
            versionInfoPath = Path.GetFullPath(curSelectedPath) + versionInfoPath;
            mLocalPackageInfos.Clear();
            if (File.Exists(versionInfoPath)) mLocalPackageInfos = JsonHelper.LoadSync<List<C2S_PB.C2S_PackageInfo>>(versionInfoPath);

            //4.整合远端和本地信息 以远端信息为准 否则表示服务器连接失败 关闭 主要是为ClientVersion赋值本地的信息
            if (mLocalPackageInfos != null && mLocalPackageInfos.Count > 0)
            {
                var pmClientInfo = mLocalPackageInfos.Last();
                if (pmClientInfo != null)
                {
                    mPackageInfo.Id = pmClientInfo.Id + 1;
                    mPackageInfo.Type = pmClientInfo.Type;
                    int len = pmClientInfo.ClientVersion.Length;
                    if (len >= 3)
                    {
                        string value = pmClientInfo.ClientVersion.Substring(len - 1, 1);
                        int intValue = int.Parse(value) + 1;
                        mPackageInfo.ClientVersion = pmClientInfo.ClientVersion.Remove(len-1, 1) + intValue;
                    }
                    else
                    {
                        Debug.LogError("版本号格式不符合！！！" + pmClientInfo.ClientVersion);
                    }
                }
            }
            else
            {
                mPackageInfo.ClientVersion = "0.0.1";
            }
            mPackageInfo.Name = curSelectedName;

            //5.传输的数据为整合之后的 主要是获取本地的ClientVersion进行添加
            mIsInited = true;
        }

        #region 服务器请求该版本信息

        public void RequestServer()
        {
            mIsInited = false;

            //1.获取选中的对象信息
            var selectedObjs = Selection.objects;
            if (selectedObjs == null || selectedObjs.Length != 1)
            {
                Debug.LogError("数量超出限制");
                return;
            }
            curSelectedPath = AssetDatabase.GetAssetPath(selectedObjs[0]);
            curSelectedName = selectedObjs[0].name;
            ProtocolManager.C2SProtocolHander.Req_PM_Upload_PackageVersion(curSelectedPath);
        }

        public void ConvertInfo(string _resResult)
        {
            //2.请求服务器当前Package的info 如果没有通过本地数据填充  如果有拷贝服务器共有数据 本地数据更新
            if (!string.IsNullOrEmpty(_resResult) && !_resResult.Equals(" "))
            {
                Debug.Log(_resResult);
                try
                {
                    var pmInfo = JsonConvert.DeserializeObject<S2C_PB.S2C_LogPackageInfo>(_resResult);
                    if (pmInfo != null)
                    {
                        if(mPackageInfo == null) mPackageInfo = new C2S_PB.C2S_PackageInfo();
                        mPackageInfo.Id = pmInfo.id;
                        mPackageInfo.Name = pmInfo.name;
                        mPackageInfo.Type = pmInfo.type;
                        mPackageInfo.ClientVersion = pmInfo.version;
                        mPackageInfo.ServerVersion = pmInfo.version;
                        mPackageInfo.ClientResPath = pmInfo.clientResPath;
                        mPackageInfo.UpdateTime = pmInfo.updateTime;
                        mPackageInfo.Des = pmInfo.des;
                        
                        Debug.Log(mPackageInfo.ClientResPath);
                    }
                    else
                    {
                        //close
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            //执行
            ExcuteClient();
        }
        

        internal static void UploadPack(string[] _files, IDictionary<string, string> _data, Action<bool, string> _callBack)
        {
            for (int i = 0; i < _files.Length; i++)
            {
                var fileName = Path.GetFileName(_files[i]);
                Debug.LogError(fileName);
                using (FileStream fileStream = new FileStream(_files[i], FileMode.Open, FileAccess.Read))
                {
                    var result = HttpHelper.HttpUploadFile(new UploadParameterType
                    {

                        Url = PM_Define.UploadURL,
                        FileNameKey = "pmFile",
                        FileNameValue = fileName,
                        Encoding = Encoding.UTF8,
                        UploadStream = fileStream,
                        PostParameters = _data
                    });

                    fileStream.Close();                                          //这里Close必须在回调执行之前

                    if (result != null)
                    {
                        Debug.Log(result);
                        _callBack?.Invoke(true, result);
                        return;
                    }
                }
            }
            
            _callBack?.Invoke(false, null);
        }

        #endregion
    }
}