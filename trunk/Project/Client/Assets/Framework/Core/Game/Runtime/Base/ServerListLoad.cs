using System.Collections;
using Game;
using Game.Core;
using Game.UI;
using UnityEngine;

namespace MUGame
{
    public class ServerListLoad : MonoBehaviour {

        private string _serverPath = string.Empty;
        private string _localListFullPath = string.Empty;

        private string _localVersion = string.Empty;
        private string _serverVersion = string.Empty;

        private string _content = string.Empty;
        private bool _isDone = false;
        

        void Awake()
        {
            _serverPath = MUEngine.MUUpdateConfig.UpdateUrl + "/";
            _localListFullPath = MUEngine.MUUpdateConfig.ResCachePath + "/" + GameConfig.SERVER_FILE;

            if(GameConfig.IsVersionUpdate)
            {
                StartCoroutine(_loadServerList());
            }
            else
            {
                StartCoroutine(_editorLoadServer());
            }

            StartCoroutine(_loadReBianFlag());
            // 埋点 请求服务器列表
            // OneSDK.GetInstance().LogEventOnlyName("RequireSeverList", (int)SDKType.One);
        }

        IEnumerator _editorLoadServer()
        {
            WWW www = new WWW(ResUtil.GetStringAssetPath() + "REMAIN-ServerList.lua");
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                _content = www.text;
                _isDone = true;
            }
            else
            {
                //正常下载流程
                StartCoroutine(_loadServerList());
            }
        }

        IEnumerator _loadReBianFlag()
        {
            WWW www = new WWW(ResUtil.GetStringAssetPath() + "tryflag");
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                GameConfig.IsReBianPackage = true;
            }
            else
            {
                GameConfig.IsReBianPackage = false;
            }
        }

        IEnumerator _loadServerList()
        {
            WWW www = new WWW(MUEngine.MUUpdateConfig.UpdateUrl + "/" + GameConfig.SERVER_FILE);
            yield return www;
            //Debug.Log("ServerListLoad--_loadServerList--------error:" + www.error + " text:" + www.text);
            if (string.IsNullOrEmpty(www.error))
            {
                _content = www.text;
                if (!string.IsNullOrEmpty(_content))
                {
                    IOUtil.WriteTextToFile(_localListFullPath, _content);
                }
            }
            else
            {
                //加载失败从本地加载
                _content = IOUtil.ReadTextFromFile(_localListFullPath);
                //Debug.Log("ServerListLoad--LoadLocalList--------_content:" + _content);
            }
            LoadServerComplete();
        }
        
        private void LoadServerComplete()
        {
            _isDone = true;

            // 埋点 成功获取服务器列表
            SDKHandler.Instance.LogEventOnlyName("GetServerListSuccess", (int)SDKType.One);
        }
        
        public bool isDone
        {
            get{return _isDone;}
        }

        public string content
        {
            get { return _content; }
        }

        public byte[] bytes
        {
            get { return System.Text.Encoding.UTF8.GetBytes(_content); }
        }
    }
}
