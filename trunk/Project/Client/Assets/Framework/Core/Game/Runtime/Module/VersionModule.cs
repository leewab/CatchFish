using Game.Core;
using MUEngine;
using UnityEngine;

namespace Game
{
    public class VersionModule : BaseModule
    {
        public override void Init()
        {
            base.Init();
#if UNITY_EDITOR
            GameConfig.IsVersionUpdate = false;
#else
    #if LHRG_VerionUpdate
            GameConfig.IsVersionUpdate = true;
    #else
            GameConfig.IsVersionUpdate = false;
    #endif
#endif
            if (GameConfig.IsVersionUpdate)
            {
                MUEngine.CoreDelegate.DynamicResource = true;
            }

            checkMultiLanguage();
            InitUpdateInfo();
        }
        
        private void InitUpdateInfo()
        {
            MUEngine.MUUpdateConfig.ChannelRes = SDKHandler.Instance.GetChannelDir();
            MUEngine.MUUpdateConfig.ResCachePath = Application.persistentDataPath;
            
            if (!GameConfig.IsVersionUpdate)
            {
#if LHRG_OB
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyd/client/";
                MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
#else
                MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
                MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
                MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
#endif
                MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
                return;
            }
            
#if LHRG_OB
            MUEngine.MUUpdateConfig.UpdateCDN_0 = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
            MUEngine.MUUpdateConfig.UpdateCDN_1 = "https://smdlyd-ws.wmupd.com/smdlyd/client/";
            MUEngine.MUUpdateConfig.UpdateUrl = "https://smdlyd-tx.wmupd.com/smdlyd/client/";
#else
            MUEngine.MUUpdateConfig.UpdateCDN_0 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
            MUEngine.MUUpdateConfig.UpdateCDN_1 = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
            MUEngine.MUUpdateConfig.UpdateUrl = "http://10.4.4.23:8081/Default_Update_DALAN/reset/";
#endif
            MUEngine.MUUpdateConfig.PatchType = EUpdateType.SmallPackage;
        }
        
        private void checkMultiLanguage()
        {
#if LHRG_SINGLE_LANGUAGE
			//单语言，屏蔽语言选择
            GameConfig.IsChangeLanguage = false;
            PlayerPrefs.SetString(MUUpdateConfig.LANGUAGE_KEY, LanguageType.English);
			MUEngine.MUUpdateConfig.SystemLanguageValue = LanguageType.English;
			Debug.LogFormat("Single Language:{0}", MUEngine.MUUpdateConfig.SystemLanguageValue);
#else
            string lan = LanguageHelper.Instance.GetSysLanguage();
            MUEngine.MUUpdateConfig.SystemLanguageValue = lan;
            Debug.LogFormat("System Language:{0}", lan);
#endif
            MUEngine.MUUpdateConfig.IsHighDevice = true;
        }

        
    }
}