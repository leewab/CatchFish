#if UNITY_TOLUA
using LuaInterface;
using MUEngine;
using MUGame;
using UnityEngine;

namespace Game
{
    public class PreloadProcessLua : PreloadProcess
    {
        public string PreloadDesc => "Lua文件";
        
        public override void Start()
        {
            base.Log(PreloadDesc);
            LuaModule.Instance.InitLua();
            if(string.IsNullOrEmpty(VersionHelper.Instance.LuaBundleName))
            {
                if (DeviceModule.IsAndroid64bit())
                {
                    MURoot.ResMgr.PreloadBundle(GameConfig.GAME_LUA64_BUNDLE_NAME, onGetLua);
                }
                else
                {
                    MURoot.ResMgr.PreloadBundle(GameConfig.GAME_LUA_BUNDLE_NAME, onGetLua);
                }
            }
            else
            {
                VersionHelper.Instance.GetLua(onGetLua);
            }
        }
        
        private void onGetLua(string name, UnityEngine.AssetBundle ab)
        {
            if (ab == null)
            {
                Debug.LogError("预加载失败！");
                return;
            }
            
            if (name == GameConfig.GAME_LUA64_BUNDLE_NAME)
            {
                LuaFileUtils.Instance.AddSearchBundle("lua", ab);
            }
            else
            {
                LuaFileUtils.Instance.AddSearchBundle(name.Split('.')[0], ab);
            }
           
            MURoot.ResMgr.SetAssetBundlePersistent(name);
            this.Finish(PreloadDesc);
        }
        
    }
}
#endif