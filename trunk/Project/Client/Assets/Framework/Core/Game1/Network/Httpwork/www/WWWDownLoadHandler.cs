using System;
using UnityEngine;

namespace Framework.Core
{
    public class WWWDownLoadHandler
    {
        private DownloadData downloadData = null;

        public WWWDownLoadHandler(DownloadData data)
        {
            if (downloadData == null) downloadData = new DownloadData();
            downloadData = data;
            StartDownload(0);
        }

        /// <summary>
        /// 开始下载
        /// </summary>
        /// <param name="delay"></param>
        private void StartDownload(float delay)
        {
            var requestData = new RequestData
            {
                URL = downloadData.URL,
                FailRetryDelay = delay,
                RequestTimeoutValue = 60,
                RequestEvent = RequestEvent
            };
            WWWController.Instance.Request(requestData);
        }

        /// <summary>
        /// 请求事件
        /// </summary>
        /// <param name="state"></param>
        /// <param name="www"></param>
        private void RequestEvent(RequestState state, WWW www)
        {
            if (state == RequestState.RequestSuccess)
            {
                RequestCompleted(www);
            }
            else if (state == RequestState.RequestTimeout)
            {
                RequestTimeout();
            }
            else if (state == RequestState.Requesting)
            {
                Requesting(www);
            }
            else if (state == RequestState.RequestFail)
            {
                RequestFail();
            }
        }

        /// <summary>
        /// 请求完成
        /// </summary>
        /// <param name="www"></param>
        private void RequestCompleted(WWW www)
        {
            if (www == null) return;
            downloadData.OnDownloadCompletedEvent?.Invoke(www);
            Clear();
        }

        /// <summary>
        /// 请求超时
        /// </summary>
        /// <param name="time"></param>
        private void RequestTimeout()
        {
            GameLog.Log("请求超时");
            downloadData.OnDownloadTimeoutEvent?.Invoke();
            Clear();
        }

        /// <summary>
        /// 正在请求
        /// </summary>
        /// <param name="www"></param>
        private void Requesting(WWW www)
        {
            if (www == null) return;
            downloadData.OnDownloadProgressEvent?.Invoke(www.text, www.progress);
        }

        /// <summary>
        /// 请求失败
        /// </summary>
        private void RequestFail()
        {
            GameLog.Log("下载失败");
            //下载失败重试下载
            if (downloadData.FailRetryCount >= 0)
            {
                downloadData.FailRetryCount--;
                StartDownload(downloadData.FailRetryDelay);
            }
            else
            {
                downloadData.OnDownloadFailEvent?.Invoke();
                Clear();
            }
        }

        private void Clear()
        {
            if (downloadData != null) downloadData = null;
        }
    }

    public class DownloadData
    {
        public string URL;
        public int FailRetryCount;
        public float FailRetryDelay;
        public Action<string, float> OnDownloadProgressEvent;
        public Action<WWW> OnDownloadCompletedEvent;
        public Action OnDownloadFailEvent;
        public Action OnDownloadTimeoutEvent;
    }
}