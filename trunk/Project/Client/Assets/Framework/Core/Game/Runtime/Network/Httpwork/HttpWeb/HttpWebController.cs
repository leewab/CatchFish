using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class HttpWebController : MonoSingleton<HttpWebController>
    {
        #region Download

        public float progress { get; private set; }
        public bool isDone { get; private set; }

        public void DownloadFile(string _url, string _rootFilePath, Action _callBack)
        {
            StartCoroutine(StartHttp(_url, _rootFilePath, _callBack));
        }

        private bool isStop;
        public void Stop()
        {
            isStop = true;
        }

        private IEnumerator StartHttp(string url, string filePath, Action callBack)
        {
            var headRequest = UnityWebRequest.Head(url);

            yield return headRequest.SendWebRequest();

            var totalLength = long.Parse(headRequest.GetResponseHeader("Content-Length"));

            var dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            Debug.Log(filePath);
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                var fileLength = fs.Length;

                if (fileLength < totalLength)
                {
                    fs.Seek(fileLength, SeekOrigin.Begin);

                    var request = UnityWebRequest.Get(url);
                    request.SetRequestHeader("Range", "bytes=" + fileLength + "-" + totalLength);
                    request.SendWebRequest();

                    var index = 0;
                    while (!request.isDone)
                    {
                        if (isStop) break;
                        yield return null;
                        var buff = request.downloadHandler.data;
                        if (buff != null)
                        {
                            var length = buff.Length - index;
                            fs.Write(buff, index, length);
                            index += length;
                            fileLength += length;

                            if (fileLength == totalLength)
                            {
                                progress = 1f;
                            }
                            else
                            {
                                progress = fileLength / (float) totalLength;
                            }
                        }
                    }
                }
                else
                {
                    progress = 1f;
                }

                fs.Close();
                fs.Dispose();
            }

            if (progress >= 1f)
            {
                isDone = true;
                if (callBack != null)
                {
                    callBack();
                }
            }
        }

        #endregion


        #region MyRegion

        
        
        #endregion
    }
}