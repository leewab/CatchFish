using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class WWWHandler
    {
        /// <summary>
        /// post方式的 response响应结果
        /// </summary>
        private Action<string> response_PostEvet;

        public Action<string> ResponsePostEvet => response_PostEvet;

        /// <summary>
        /// Get方式的 response响应结果
        /// </summary>
        private Action<string> response_GetEvent;

        public Action<string> ResponseGetEvent => response_GetEvent;

        /// <summary>
        /// Post表单请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        public IEnumerator IPostFormRequest(string url, WWWForm form)
        {
            WWW www = new WWW(url, form);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError("PostRequest Error");
            }
            else
            {
                response_PostEvet?.Invoke(www.text);
            }
        }

        /// <summary>
        /// Get表单请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public IEnumerator IGetFormRequest(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.error != null)
            {
                Debug.LogError("GetRequest Error");
            }
            else
            {
                response_GetEvent?.Invoke(www.text);
            }
        }
    }
}