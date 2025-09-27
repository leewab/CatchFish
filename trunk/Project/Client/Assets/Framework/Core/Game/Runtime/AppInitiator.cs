using Game.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    /// <summary>
    /// APP启动器
    /// </summary>
    public class AppInitiator
    {
        private const string InitSceneName = "AppInitiator";
        
        [RuntimeInitializeOnLoadMethod]
        public static void Initiator()
        {
            //初始场景
            //加载本地AppFile文件
            // AppManager.LoadAppFile();
            //加载场景
            if (!SceneManager.GetActiveScene().name.Equals(InitSceneName))
            {
                LogHelper.Log("加载初始化场景: " + InitSceneName);
                SceneManager.LoadScene(InitSceneName);
            }        
        }
    }
}