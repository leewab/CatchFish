using MUEngine;
using UnityEngine;

namespace Game
{
    public class PreloadProcessShader : PreloadProcess
    {
        public string PreloadDesc => "Shader文件";
        public override void Start()
        {
            base.Log(PreloadDesc);
            MURoot.ResMgr.PreloadBundle(GameConfig.GAME_SHADER_BUNDLE_NAME, onLoadedShader);
        }
        
        private void onLoadedShader(string name, AssetBundle ab) {
            if (ab == null || !name.Equals(GameConfig.GAME_SHADER_BUNDLE_NAME))
            {
                Debug.LogError("预加载失败！");
                return;
            }

            UnityEngine.Object obj = ab.LoadAsset("Materials.prefab");
            if (obj != null)
            {
                GameObject gameobject = (GameObject) GameObject.Instantiate(obj);
                GameObject.DontDestroyOnLoad(gameobject);
                gameobject.transform.position = new Vector3(10000, -10000, 10000); //new Vector3(0, 0, 5);
                gameobject.layer = 5;
            }

            UnityEngine.Object extraobj = ab.LoadAsset("ExtraMaterials.prefab");
            if (extraobj != null)
            {
                GameObject gameobject = (GameObject)GameObject.Instantiate(extraobj);
                GameObject.DontDestroyOnLoad(gameobject);
                gameobject.transform.position = new Vector3(10000, -10000, 10000); //new Vector3(0, 0, 5);
                gameobject.layer = 5;
            }
            
            MURoot.ResMgr.SetAssetBundlePersistent(name);
            this.Finish(PreloadDesc);
        }
        
    }
}