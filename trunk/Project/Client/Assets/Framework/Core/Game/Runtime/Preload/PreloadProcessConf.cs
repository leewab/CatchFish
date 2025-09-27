using MUEngine;
using MUGame;
using UnityEngine;

namespace Game
{
    public class PreloadProcessConf : PreloadProcess
    {
        public string PreloadDesc => "配置文件";
        
        public override void Start()
        {
            base.Log(PreloadDesc);
            if (string.IsNullOrEmpty(VersionHelper.Instance.ConfBundleName))
            {
#if UNITY_EDITOR
                MURoot.ResMgr.GetBytes(GameConfig.GAME_CONF_BUNDLE_NAME, onLoadConf);
#elif UNITY_STANDALONE_WIN
                if(GameConfig.IsOnlinePC)
                {
                    MURoot.ResMgr.GetAsset(GameConfig.GAME_CONF_BUNDLE_NAME, onLoadConf, LoadPriority.MostPrior);
                }
                else
                {
                    MURoot.ResMgr.GetBytes(GameConfig.GAME_CONF_BUNDLE_NAME, onLoadConf);
                }
#else
                MURoot.ResMgr.GetAsset(GameConfig.GAME_CONF_BUNDLE_NAME, onLoadConf, LoadPriority.MostPrior);
#endif
            }
            else
            {
                VersionHelper.Instance.GetConf(onLoadConf);
            }
        }

        private void onLoadConf(string name, byte[] db)
        {
            if (db == null || !name.Equals(GameConfig.GAME_CONF_BUNDLE_NAME))
            {
                Debug.LogError("预加载失败！");
                return;
            }
            
            SQLiteLoad.OnLoadFile(name, db);
            this.Finish(PreloadDesc);
        }
        
        private void onLoadConf(string name, UnityEngine.Object ab)
        {
            if (ab == null || !name.Equals(GameConfig.GAME_CONF_BUNDLE_NAME))
            {
                Debug.LogError("预加载失败！");
                return;
            }
            
            SQLiteLoad.OnLoadFile(name, ab);
            this.Finish(PreloadDesc);
        }
        
    }
}