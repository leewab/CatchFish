using MUEngine;
using UnityEngine;

namespace Game
{
    public class PreloadProcessAct : PreloadProcess
    {
        public string PreloadDesc => "ACT文件";
        
        public override void Start()
        {
            base.Log(PreloadDesc);
            MURoot.ResMgr.PreloadBundle(GameConfig.GAME_ACT_BUNDLE_NAME, onLoadBound);
        }

        private void onLoadBound(string name, AssetBundle db)
        {
            if (db == null || !name.Equals(GameConfig.GAME_ACT_BUNDLE_NAME))
            {
                Debug.LogError("预加载失败！");
                return;
            }
            
            //SQLiteLoad.OnLoadFile(name,db)
            this.Finish(PreloadDesc);
        }
        
    }
}