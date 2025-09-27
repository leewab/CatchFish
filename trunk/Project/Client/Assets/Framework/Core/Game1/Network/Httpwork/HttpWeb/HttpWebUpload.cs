//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Newtonsoft.Json;
//using UnityEngine;
//
//namespace Framework.Core
//{
//    public class HttpWebUpload : MonoSingleton<HttpWebUpload>
//    {
//        private string url = "http://127.0.0.1:8081/file_upload";
//        private string token = "";
//        private string fileName = "";
//        private byte[] bytes;
//        private Action<bool, PMProtocolInfo> onUploadEvent;
//        private Dictionary<string, string> formDic = new Dictionary<string, string>();
//
//        private IEnumerator IStart()
//        {
//            WWWForm form = new WWWForm();
//            form.AddBinaryData("pmForm", bytes, fileName);
//            foreach (var formData in formDic)
//            {
//                if (formData.Key != null && formData.Value != null)
//                {
//                    form.AddField(formData.Key, formData.Value);
//                }
//            }
//
//            WWW upLoad = new WWW(url, form);
//            yield return upLoad;
//            List<PMProtocolInfo> pmInfo = null;
//            //如果失败
//            if (!string.IsNullOrEmpty(upLoad.error) || upLoad.text.Equals("false"))
//            {
//                Debug.LogError("Error://///" + upLoad.error);
//                onUploadEvent?.Invoke(false, null);
//            }
//            else
//            {
//                try
//                {
//                    Debug.Log(upLoad.text);
//                    pmInfo = JsonConvert.DeserializeObject<List<PMProtocolInfo>>(upLoad.text);
//                    onUploadEvent?.Invoke(true, pmInfo[0]);
//                }
//                catch (Exception e)
//                {
//                    GameLog.Error("反馈的Json文件格式有误  " + e);
//                    onUploadEvent?.Invoke(true, null);
//                }
//            }
//        }
//
//        private string[] filePaths;
//
//        private IEnumerator IStarts()
//        {
//            foreach (var filePath in filePaths)
//            {
//                bytes = ConvertHelper.Object2Byte(filePath);
//                fileName = filePath.Split('/').Last();
//                Debug.Log(fileName);
//                var form = new WWWForm();
//                form.AddBinaryData("pmForm", bytes, fileName);
//                WWW upLoad = new WWW(url, form);
//                yield return upLoad;
//                List<PMProtocolInfo> pmInfo = null;
//                //如果失败
//                if (!string.IsNullOrEmpty(upLoad.error) || upLoad.text.Equals("false"))
//                {
//                    Debug.LogError("Error://///" + upLoad.error);
//                    onUploadEvent?.Invoke(false, null);
//                }
//                else
//                {
//                    try
//                    {
//                        pmInfo = JsonConvert.DeserializeObject<List<PMProtocolInfo>>(upLoad.text);
//                        onUploadEvent?.Invoke(true, pmInfo[0]);
//                    }
//                    catch (Exception e)
//                    {
//                        GameLog.Error("反馈的Json文件格式有误  " + e);
//                        onUploadEvent?.Invoke(true, null);
//                    }
//                }
//            }
//        }
//
//
//        public void UploadFile(string filePath, Dictionary<string, string> formDatas,
//            Action<bool, PMProtocolInfo> _onUploadEvent)
//        {
//            bytes = ConvertHelper.Object2Byte(filePath);
//            formDic = formDatas;
//            fileName = filePath.Split('/').Last();
//            onUploadEvent = _onUploadEvent;
//            StartCoroutine(IStart());
//        }
//
//        public void UploadFile(string[] _filePaths, Dictionary<string, string> formDatas,
//            Action<bool, PMProtocolInfo> _onUploadEvent)
//        {
//            filePaths = _filePaths;
//            formDic = formDatas;
//            onUploadEvent = _onUploadEvent;
//            StartCoroutine(IStarts());
//        }
//    }
//
//}