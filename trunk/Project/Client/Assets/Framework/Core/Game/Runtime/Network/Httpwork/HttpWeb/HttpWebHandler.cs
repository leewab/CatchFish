using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace Game
{
    public class HttpWebHandler : Singleton<HttpWebHandler>
    {
        //超时时间
        private const int timeoutCount = 20; //超时还没有做

        #region Post Part

        /// <summary>
        /// Post响应超时
        /// </summary>
        public Action OnHttpResponsePoseTimeoutEvent;

        /// <summary>
        /// Post响应
        /// </summary>
        public Action<string> OnHttpResponsePostEvent;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="formDic"></param>
        public void HttpRequestFormPost(string url, Dictionary<string, string> formDic, string formName)
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

            using (WebResponse wr = req.GetResponse())
            {
                //对接收到的内容进行处理
                Stream respStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                string result = reader.ReadToEnd();
                wr.Close();
                OnHttpResponsePostEvent?.Invoke(result);
            }
        }

        #endregion

        #region Get Part

        /// <summary>
        /// Get响应超时
        /// </summary>
        public Action OnHttpResponseGetTimeoutEvent;

        /// <summary>
        /// Get响应
        /// </summary>
        public Action<string> OnHttpResponseGetEvent;

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        public void HttpRequestFromGet(string url)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = "GET";
            using (WebResponse wr = req.GetResponse())
            {
                //对接收到的内容进行处理
                Stream respStream = wr.GetResponseStream();
                StreamReader reader = new StreamReader(respStream, Encoding.UTF8);
                string result = reader.ReadToEnd();
                respStream.Close();
                wr.Close();
                OnHttpResponseGetEvent?.Invoke(result);
            }
        }
        
        public void HttpRequestDownloadFromGet(string url)
        {
            
        }
        
        public void DownFile(string uRLAddress, string localPath, string filename)
        {
            WebClient client = new WebClient();
            Stream str = client.OpenRead(uRLAddress);
            StreamReader reader = new StreamReader(str);
            byte[] mbyte = new byte[1000000];
            int allmybyte = (int)mbyte.Length;
            int startmbyte = 0;
 
            while (allmybyte > 0)
            {
 
                int m = str.Read(mbyte, startmbyte, allmybyte);
                if (m == 0)
                {
                    break;
                }
                startmbyte += m;
                allmybyte -= m;
            }
 
            reader.Dispose();
            str.Dispose();
 
            //string paths = localPath + System.IO.Path.GetFileName(uRLAddress);
            string path = localPath + filename;
            FileStream fstr = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            fstr.Write(mbyte, 0, startmbyte);
            fstr.Flush();
            fstr.Close();
        }

      

        #endregion
    }
}