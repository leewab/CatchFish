using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.UI
{
    public static class HttpHelper
    {
        #region Upload

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="parameter">上传文件请求参数</param>
        public static string HttpUploadFile(UploadParameterType parameter)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // 1.分界线
                string boundary = string.Format("----{0}", DateTime.Now.Ticks.ToString("x")), // 分界线可以自定义参数
                    beginBoundary = string.Format("--{0}\r\n", boundary),
                    endBoundary = string.Format("\r\n--{0}--\r\n", boundary);
                byte[] beginBoundaryBytes = parameter.Encoding.GetBytes(beginBoundary),
                    endBoundaryBytes = parameter.Encoding.GetBytes(endBoundary);
                // 2.组装开始分界线数据体 到内存流中
                memoryStream.Write(beginBoundaryBytes, 0, beginBoundaryBytes.Length);
                // 3.组装 上传文件附加携带的参数 到内存流中
                if (parameter.PostParameters != null && parameter.PostParameters.Count > 0)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in parameter.PostParameters)
                    {
                        string parameterHeaderTemplate =
                            string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n{2}",
                                keyValuePair.Key, keyValuePair.Value, beginBoundary);
                        byte[] parameterHeaderBytes = parameter.Encoding.GetBytes(parameterHeaderTemplate);

                        memoryStream.Write(parameterHeaderBytes, 0, parameterHeaderBytes.Length);
                    }
                }

                // 4.组装文件头数据体 到内存流中
                string fileHeaderTemplate =
                    string.Format(
                        "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n",
                        parameter.FileNameKey, parameter.FileNameValue);
                byte[] fileHeaderBytes = parameter.Encoding.GetBytes(fileHeaderTemplate);
                memoryStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                // 5.组装文件流 到内存流中
                byte[] buffer = new byte[1024 * 1024 * 1];
                int size = parameter.UploadStream.Read(buffer, 0, buffer.Length);
                while (size > 0)
                {
                    memoryStream.Write(buffer, 0, size);
                    size = parameter.UploadStream.Read(buffer, 0, buffer.Length);
                }

                // 6.组装结束分界线数据体 到内存流中
                memoryStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                // 7.获取二进制数据
                byte[] postBytes = memoryStream.ToArray();
                memoryStream.Close();
                GC.Collect();
                // 8.HttpWebRequest 组装
                HttpWebRequest webRequest =
                    (HttpWebRequest) WebRequest.Create(new Uri(parameter.Url, UriKind.RelativeOrAbsolute));
                webRequest.AllowWriteStreamBuffering = false;
                webRequest.Method = "POST";
                webRequest.Timeout = 1800000;
                webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                webRequest.ContentLength = postBytes.Length;
                if (Regex.IsMatch(parameter.Url, "^https://"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                     ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                }

                // 9.写入上传请求数据
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(postBytes, 0, postBytes.Length);
                    requestStream.Close();
                }

                // 10.获取响应
                using (HttpWebResponse webResponse = (HttpWebResponse) webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream(), parameter.Encoding))
                    {
                        string body = reader.ReadToEnd();
                        reader.Close();
                        return body;
                    }
                }
            }
        }

        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true; //总是接受
        }

        #endregion

        #region Download

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">所下载的路径</param>
        /// <param name="path">本地保存的路径</param>
        /// <param name="overwrite">当本地路径存在同名文件时是否覆盖</param>
        /// <param name="callback">实时状态回掉函数</param>
        /// Action<文件名,文件的二进制, 文件大小, 当前已上传大小>
        public static void HttpDownloadFile(string url, string path, bool overwrite,
            Action<string, string, byte[], long, long> callback = null)
        {
            Debug.Log("url.." + url);
            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //获取文件名
            string fileName = response.Headers["Content-Disposition"]; //attachment;filename=FileName.txt
            string contentType = response.Headers["Content-Type"];     //attachment;

            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            using (Stream responseStream = response.GetResponseStream())
            {
                long totalLength = response.ContentLength;
                IOHepler.DirectoryCreate(path);
                // fileName为空表示下载文件失败 所以检查是否有Json信息发来
                if (string.IsNullOrEmpty(fileName))
                {
                    //对接收到的内容进行处理
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    var result = reader.ReadToEnd();
                    responseStream.Close();
                    OnHttpResponseGetEvent?.Invoke(result);
                }
                else
                {
                    Debug.Log(Path.Combine(path, fileName));
                    using (Stream stream = new FileStream(Path.Combine(path, fileName),
                        overwrite ? FileMode.Create : FileMode.CreateNew))
                    {
                        byte[] bArr = new byte[1024];
                        int size;
                        while ((size = responseStream.Read(bArr, 0, bArr.Length)) > 0)
                        {
                            stream.Write(bArr, 0, size);
                            callback?.Invoke(fileName, contentType, bArr, totalLength, stream.Length);
                        }
                        stream.Close();
                        responseStream.Close();
                    }
                }
            }

           
            // 这里fileName为空的话取了URL最后一个字段
            //fileName = string.IsNullOrEmpty(fileName)
            //    ? response.ResponseUri.Segments[response.ResponseUri.Segments.Length - 1]
            //    : fileName.Remove(0, fileName.IndexOf("filename=") + 9);
        }

        #endregion


        //超时时间
        private const int timeoutCount = 20; //超时还没有做

        #region Post Part

        /// <summary>
        /// Post响应超时
        /// </summary>
        public static Action OnHttpResponsePoseTimeoutEvent;

        /// <summary>
        /// Post响应
        /// </summary>
        public static Action<string> OnHttpResponsePostEvent;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formDic"></param>
        public static void HttpRequestFormPost(string url, Dictionary<string, string> formDic, string formName)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "POST";

            //拼接Form表单里的信息 string requestForm = "username=test&password=123456";
            StringBuilder sb = new StringBuilder("");
            foreach (var keyValue in formDic)
            {
                sb.Append($"{keyValue.Key}={keyValue.Value}&");
            }

            sb.Append($"form={formName}");
            string requestForm = sb.ToString();
            Debug.Log(requestForm + "//////FormValue");
            byte[] bs = Encoding.ASCII.GetBytes(requestForm);
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                //往请求流中写入表单
                reqStream.Write(bs, 0, bs.Length);
                reqStream.Close();
            }

            string result = "";
            using (WebResponse wr = req.GetResponse())
            {
                //对接收到的内容进行处理
                Stream respStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                result = reader.ReadToEnd();
                wr.Close();
            }
            
            OnHttpResponsePostEvent?.Invoke(result);
        }

        #endregion

        #region Get Part

        /// <summary>
        /// Get响应超时
        /// </summary>
        public static Action OnHttpResponseGetTimeoutEvent;

        /// <summary>
        /// Get响应
        /// </summary>
        public static Action<string> OnHttpResponseGetEvent;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        public static void HttpRequestFromGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "GET";
            string result = "";
            using (WebResponse wr = req.GetResponse())
            {
                //对接收到的内容进行处理
                Stream respStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                result = reader.ReadToEnd();
                respStream.Close();
                wr.Close();
            }
            
            Debug.Log("HttpRequestFromGet");
            OnHttpResponseGetEvent?.Invoke(result);
        }
        
        #endregion
    }


    /// <summary>
    /// 上传文件 - 请求参数类
    /// </summary>
    public class UploadParameterType
    {
        public UploadParameterType()
        {
            FileNameKey = "fileName";
            Encoding = Encoding.UTF8;
            PostParameters = new Dictionary<string, string>();
        }

        /// <summary>
        /// 上传地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件名称key
        /// </summary>
        public string FileNameKey { get; set; }

        /// <summary>
        /// 文件名称value
        /// </summary>
        public string FileNameValue { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// 上传文件的流
        /// </summary>
        public Stream UploadStream { get; set; }

        /// <summary>
        /// 上传文件 携带的参数集合
        /// </summary>
        public IDictionary<string, string> PostParameters { get; set; }
    }
}