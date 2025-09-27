using System;
using System.IO;
using System.Collections;
using UnityEngine;

namespace MUGame
{
    public class VersionLoader : MonoBehaviour
    {
        private string[] _loadPaths;
        private string _loadPath = string.Empty;
        private string _savePath = string.Empty;
        private Action<WWW> _callBack;
        private WWW _www;
        private int nLoadTime = 0;

        public void Load(string loadpath, string savepath, Action<WWW> callBack)
        {
            _loadPath = loadpath;
            _loadPaths = null;
            _savePath = savepath;
            _callBack = callBack;
            _www = new WWW(_loadPath);
        }

        public void Load(string[] loadpaths, string savepath, Action<WWW> callBack)
        {
            _loadPaths = loadpaths;
            _savePath = savepath;
            _callBack = callBack;
            _www = new WWW(_loadPaths[nLoadTime]);
        }

        void Update()
        {
            if (_www == null)
                return;
            if (!_www.isDone)
                return;
            if(!string.IsNullOrEmpty(_www.error))
            {
                LoadAgain();
                return;
            }
            OnLoadComplete();
        }

        private void LoadAgain()
        {
            _www.Dispose();
            _www = null;
            if (_loadPaths != null )
            {
                nLoadTime++;
                if(nLoadTime > _loadPaths.Length - 1)
                {
                    nLoadTime = 0;
                }
                _www = new WWW(_loadPaths[nLoadTime]);
            }
            else
            {
                _www = new WWW(_loadPath);
            }
        }
        private void OnLoadComplete()
        {
            //在Callback中，可能会调用本类中的Load方法，再然后，这里直接Dispose新的WWW，会导致后面的回调永远不会被执行
            //打个补丁 modify by liujunjie in 2019/7/9

            WWW localWWW = _www;
            SaveFile(localWWW.bytes);
            if (_callBack != null)
            {
                _callBack(localWWW);
            }

            localWWW.Dispose();
            if(localWWW == _www)
            {
                _www = null;
            }
            //_www.Dispose();
            //_www = null;

            nLoadTime = 0;
        }

        private void SaveFile(byte[] bytes)
        {
            if (string.IsNullOrEmpty(_savePath))
                return;
            FileStream fs = new FileStream(_savePath, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }
}
