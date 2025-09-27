using MUEngine;
using MUGame;
using UnityEngine;

namespace Game
{
    public class PreloadProcessMsgPb : PreloadProcess
    {
        public string PreloadDesc => "协议文件";

        public override void Start()
        {
            base.Log(PreloadDesc);
#if AR_TEST_UPDATE
                VersionHelper.Instance.GetLuaPB(onGetAllMsgPb);
                return;
#endif
            MURoot.ResMgr.PreloadBundle(GameConfig.GAME_MESSAGE_BUNDLE_NAME, onGetAllMsgPb);
        }
        
        private void onGetAllMsgPb(string name, UnityEngine.AssetBundle ab)
        {
            if (ab == null || !name.Equals(GameConfig.GAME_MESSAGE_BUNDLE_NAME))
            {
                Debug.LogError("预加载失败！");
                return;
            }

            TextAsset msgAsset = ab.LoadAsset<TextAsset>("allMsgPb.bytes");
            if (msgAsset == null)
            {
                ConfigUtil.LogError("allMsgPb.bytes asset is null");
                return;
            }
#if UNITY_TOLUA
            LuaMsgUtils.mAllMsgPbBytes = msgAsset.bytes;
#endif
            Resources.UnloadAsset(msgAsset);
            MURoot.ResMgr.SetAssetBundlePersistent(name);
            this.Finish(PreloadDesc);
        }

    }
}