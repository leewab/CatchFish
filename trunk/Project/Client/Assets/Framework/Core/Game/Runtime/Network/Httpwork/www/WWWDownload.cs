using System;
using System.Collections;
using Game;
using Game.Core;
using UnityEngine;

namespace Game
{
    public class WWWDownload : DownloadItem
    {
        WWW m_www;

        public WWWDownload(string url, string path) : base(url, path)
        {
            
            Debug.Log(url);
            Debug.Log(path);
        }

        public override void StartDownload(Action callback = null)
        {
            base.StartDownload();
            Main.Instance.StartCoroutine(Download(callback));
        }

        IEnumerator Download(Action callback = null)
        {
            m_www = new WWW(m_srcUrl);
            m_isStartDownload = true;
            yield return m_www;
            //WWW读取完成后，才开始往下执行
            m_isStartDownload = false;

            if (m_www.isDone)
            {
                byte[] bytes = m_www.bytes;
                //创建文件
                IOHelper.CreateFile(m_saveFilePath, bytes);
            }
            else
            {
                Debug.Log("Download Error:" + m_www.error);
            }

            if (callback != null)
            {
                callback();
            }
        }

        public override float GetProcess()
        {
            if (m_www != null)
            {
                return m_www.progress;
            }

            return 0;
        }

        public override long GetCurrentLength()
        {
            if (m_www != null)
            {
                return m_www.bytesDownloaded;
            }

            return 0;
        }

        public override long GetLength()
        {
            return 0;
        }

        public override void Destroy()
        {
            if (m_www != null)
            {
                m_www.Dispose();
                m_www = null;
            }
        }
    }
}