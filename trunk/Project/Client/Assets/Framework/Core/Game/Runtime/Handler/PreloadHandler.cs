#if !UNITY_EDITOR && (ANDROID_AR_TEST || IOS_AR_TEST)
    #define AR_TEST_UPDATE
#endif
using System.Collections.Generic;
using Game.Core;
using MUEngine;
using UnityEngine;
using Game.UI;

namespace Game
{
    /// <summary>
    /// 预加载资源
    /// </summary>
    public class PreloadHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static PreloadHandler Instance = HandlerModule.PreloadHandler;

        private bool mIsCalling = false;
        private Queue<PreloadProcess> mProcessQueue = new Queue<PreloadProcess>();
        private Dictionary<string, PreloadProcess> mPreloadProcessDic = new Dictionary<string, PreloadProcess>();


        public override void OnGameStateChanged(States newState)
        {
            if (InGameState)
            {
                // SetPersistent();  这里是设置资源是否为可持续使用资源的标记，修改为加载资源预加载资源结束后设置
                StartPreload();
                SetPreloadProgress();
            }
        }

        protected override bool EnableInCurrentState(States currentState)
        {
            return currentState == States.PRELOAD;
        }

        private void StartPreload()
        {
            Debug.Log("Start Preload");

            // MURoot.ResMgr.PreloadScene("Login");
            AddProcess(new PreloadProcessInit());
            AddProcess(new PreloadProcessShader());
            // AddProcess(new PreloadProcessFont());
            AddProcess(new PreloadProcessMsgPb());
            AddProcess(new PreloadProcessConf());
            // AddProcess(new PreloadProcessAct());
#if UNITY_TOLUA
            AddProcess(new PreloadProcessLua());
#endif
            AddProcess(new PreloadProcessFinish());
        }

        public void AddProcess(PreloadProcess process)
        {
            if (mProcessQueue == null) mProcessQueue = new Queue<PreloadProcess>();
            mProcessQueue.Enqueue(process);
            if (!mIsCalling) CallProcess();
        }

        private void CallProcess()
        {
            if (mProcessQueue == null || mProcessQueue.Count == 0)
            {
                mIsCalling = false;
                return;
            }
            
            mIsCalling = true;
            var process = mProcessQueue.Dequeue();
            if (process == null) return;
            process.OnFinishedEvent = CallProcess;
            process.Start();
        }

        private void SetPersistent()
        {
            Debug.Log("Start Persistent");

            // MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_ACT_BUNDLE_NAME);
            // Debug.Log("Start act");
            // if (DeviceModule.IsAndroid64bit())
            // {
            //     MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_LUA64_BUNDLE_NAME);
            // }
            // else
            // {
            //     MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_LUA_BUNDLE_NAME);
            // }
            // Debug.Log("Start lua");
            // MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_SHADER_BUNDLE_NAME);
            // Debug.Log("Start shader");
            // MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_UI_FONT_BUNDLE);
            // Debug.Log("Start font");
            // MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.GAME_UI_TRANSPARENT_BG);
            // Debug.Log("Start uitransparentbg");
            
            //MURoot.ResMgr.SetAssetBundlePersistent(GameConfig.UI_PREFAB_NAME);
        }

        private void SetPreloadProgress()
        {
            Debug.Log("SetPreloadProgress");
            
            string[] bundleNames;
            string[] assetNames = new string[]
            {

            };
            
            if (DeviceModule.IsAndroid64bit())
            {
                bundleNames = new string[]
                {
                    // GameConfig.GAME_ACT_BUNDLE_NAME, 
                    // GameConfig.GAME_UI_FONT_BUNDLE,
                    
                    GameConfig.GAME_SHADER_BUNDLE_NAME,
                    GameConfig.GAME_MESSAGE_BUNDLE_NAME,
                    GameConfig.GAME_CONF_BUNDLE_NAME,
                    GameConfig.GAME_LUA64_BUNDLE_NAME,
                };
            }
            else
            {
                bundleNames = new string[]
                {
                    // GameConfig.GAME_ACT_BUNDLE_NAME, 
                    // GameConfig.GAME_UI_FONT_BUNDLE, 
                    
                    GameConfig.GAME_SHADER_BUNDLE_NAME,
                    GameConfig.GAME_MESSAGE_BUNDLE_NAME,
                    GameConfig.GAME_CONF_BUNDLE_NAME,
                    GameConfig.GAME_LUA_BUNDLE_NAME,
                };
            }

            LoadingHandler.Instance.SetLoadType((int)EProgressType.FirstInit);
            MURoot.ResMgr.SetProgress(bundleNames, assetNames, (int)EProgressType.FirstInit);
        }

    }
}
