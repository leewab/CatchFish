using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Game
{
    public class UnityWebController : MonoSingleton<UnityWebController>
    {
        public string baseUrl = "https://127.0.0.1:3000/";
        public string sKey = "zoo_visit_key";
    
        public void Get(string url, Dictionary<string, string> headData, Action<string> callback)
        {
            StartCoroutine(RequestGet(url, headData, callback));
        }

        public IEnumerator RequestGet(string url, Dictionary<string, string> headData, Action<string> callBack)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                //http 设置header 的内容
                webRequest.SetRequestHeader("Content-Type", "application/json");
                //get 请求数据
                foreach (var v in headData)
                {
                    webRequest.SetRequestHeader(v.Key, v.Value);   
                }
                yield return webRequest.SendWebRequest();

                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callBack?.Invoke(null);
                }
                else
                {
                    callBack?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }

        
        public void Post(string url, Dictionary<string, string> postData, Action<string> callBack)
        {
            StartCoroutine(RequestPost(url, postData, callBack));
        }

        public IEnumerator RequestPost(string url, Dictionary<string, string> postData, Action<string> callBack)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, postData))
            {
                //http 设置header 的内容
                webRequest.SetRequestHeader("Content-Type", "application/json");
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callBack?.Invoke(null);
                }
                else
                {
                    callBack?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }
        
        public void Post(string url, WWWForm postData, Action<string> callBack)
        {
            StartCoroutine(RequestPost(url, postData, callBack));
        }
        
        public IEnumerator RequestPost(string url, WWWForm postData, Action<string> callBack)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Post(url, postData))
            {
                //http 设置header 的内容
                webRequest.SetRequestHeader("Content-Type", "application/json");
                yield return webRequest.SendWebRequest();
                if (webRequest.isHttpError || webRequest.isNetworkError)
                {
                    Debug.LogError(webRequest.error + "\n" + webRequest.downloadHandler.text);
                    callBack?.Invoke(null);
                }
                else
                {
                    callBack?.Invoke(webRequest.downloadHandler.text);
                }
            }
        }
    }
}