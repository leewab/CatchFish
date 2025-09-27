using System;
using System.Collections;
using UnityEngine;

namespace Framework.Core
{
    public class WWWController : MonoSingleton<WWWController>
    {
        private float curTime = 0;
        private float frameTime = 0;
        private bool isStartRequest = false;

        private RequestData requestData;
        
        public void Request(RequestData data)
        {
            if (requestData == null) requestData = new RequestData();
            requestData = data;
            StartCoroutine(IEWWWRequest());
        }

        private IEnumerator IEWWWRequest()
        {
            yield return new WaitForSeconds(requestData.FailRetryDelay);
            using (WWW www = new WWW(requestData.URL))
            {
                yield return www;
                if (www.error != null)
                {
                    GameLog.Error(www.error);
                    requestData.RequestEvent(RequestState.RequestFail, null);
                    Clear();
                    yield break;
                }

                requestData.RequestEvent?.Invoke(RequestState.Requesting, www);
                if (www.isDone)
                {
                    GameLog.Log("请求成功！");
                    requestData.RequestEvent?.Invoke(RequestState.RequestSuccess, www);
                    Clear();
                }
            }
        }

        private void Update()
        {
            if (!isStartRequest) return;
            frameTime++;
            if (frameTime >= 60)
            {
                frameTime = 0;
                curTime++;
                if (curTime >= requestData.RequestTimeoutValue)
                {
                    requestData.RequestEvent?.Invoke(RequestState.RequestTimeout, null);
                }
            }
        }

        private void Clear()
        {
            isStartRequest = false;
            curTime = 0;
            frameTime = 0;
            StopCoroutine(IEWWWRequest());
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (requestData != null) requestData = null;
        }
    }

    public class RequestData
    {
        public string URL;
        public float FailRetryDelay;
        public float RequestTimeoutValue;
        public Action<RequestState, WWW> RequestEvent;
    }
    
    public enum RequestState
    {
        RequestSuccess,           //请求成功
        RequestFail,              //请求失败
        RequestTimeout,           //请求超时
        Requesting                //请求中
    }
}