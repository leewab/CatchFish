using Game;
using Game.Core;
using MUEngine;
using MUEngine.Story;
using MUGame;
using UnityEngine;

namespace Game
{
    public class DeviceModule : BaseModule
    {
        public static DeviceModule Instance => ModuleMgr.GetModule<DeviceModule>();
        
        private static bool bDeviceUserSet = false;
        private static bool bFirstInitQuality = false;
        public static bool FirstInitQuality => bFirstInitQuality;

        // 用户设置画质
        private QualityType DeviceUserSetInfo;

        // 推荐画质
        public QualityType RecommendDeviceInfo;
        
        // 当前画质
        private QualityType mDeviceType;
        public QualityType DeviceType
        {
            get
            {
                return mDeviceType;
            }
            set
            {
                mDeviceType = value;
                this.NotifyEngineDevicePerformance();
            }
        }
        
        public override void Init()
        {
            base.Init();
            Application.targetFrameRate = 30;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            QualitySettings.skinWeights = SkinWeights.TwoBones;
            this.AdaptiveDeviceType();
            this.PreSetScreenWH();
        }

        public void Start()
        {
            SettingNotch();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            SetRealScreenWH();
            CheckEnableDof();
        }

        public void AdaptiveDeviceType()
        {
            string processorType = SystemInfo.graphicsDeviceName;
            LogHelper.Log("SystemInfo.graphicsDeviceName: " + processorType);
            
#if UNITY_IPHONE
            SystemMemory = SystemInfo.systemMemorySize;
            LogHelper.Log("SystemInfo.systemMemorySize: " + SystemMemory);
            //Iphone 8以上 7p
            if (SystemMemory > 2500) 
            {
                initAllocMemory(8 * 1024 * 1024);
                if (processorType.Contains("A10"))
                {
                    this.DeviceType = QualityType.Perfect;
                }
                else
                {
                    this.DeviceType = QualityType.AllPerfect;
                }
            }
            //Iphone 6s Iphone7 Iphone8
            else if (SystemMemory > 1500)
            {
                initAllocMemory(8 * 1024 * 1024);
                //iphone 6s
                if (processorType.Contains("A9"))
                {
                    this.DeviceType = QualityType.High;
                }
                else if (processorType.Contains("A10"))
                {
                    this.DeviceType = QualityType.Perfect;
                }
                else if (processorType.Contains("A11"))
                {
                    this.DeviceType = QualityType.AllPerfect;
                }
                else
                {
                    this.DeviceType = QualityType.Mid;
                }
            }
            //iphone 6 6sp
            else
            {
                if (processorType.Contains("A8"))//6s
                {
                    this.DeviceType = QualityType.Mid;
                }
                else
                {
                    this.DeviceType = QualityType.Low;
                }
            }

            if ((float)Screen.width/(float)Screen.height < 1.5f)
            {
                this.DeviceType = (QualityType)Math.Min((int)this.DeviceType, 2);
                if (processorType.Contains("A8"))
                {
                    this.DeviceType = QualityType.Mid;
                }
            }
#elif UNITY_ANDROID && !UNITY_EDITOR 
            //主要用去分辨率设置
            initAllocMemory(8 * 1024 * 1024);
            int graphicMemory = SystemInfo.graphicsMemorySize;
            int processorFrequency = SystemInfo.processorFrequency;
            SystemMemory = SystemInfo.systemMemorySize;
            int graphicLevel = 0;
            if (DefaultQualityChecking.Instance.checkGraphicDevice(out graphicLevel))
            {
                this.DeviceType = (QualityType)graphicLevel;
            }
            else
            {
                if (processorFrequency <= 1800 && SystemMemory <= 2048 || Screen.dpi < 300)
                {
                    this.DeviceType = QualityType.Low;
                }
                else if (processorFrequency > 2100 && graphicMemory > 1000 && Screen.dpi >= 400)
                {
                    this.DeviceType = QualityType.Perfect;
                }
                else
                {
                    this.DeviceType = QualityType.High;
                }
            }
            if (CommonUseSDK.IsSimulator())
            {
                this.DeviceType = QualityType.High;
            }
#elif UNITY_WEBGL
            this.DeviceType = QualityType.Mid;
#endif
        }
        
        //启动时分配连续的内存，以减少gc的触发
        private void initAllocMemory(int size)
        {
            byte[] buff = new byte[size];
            buff = null;
        }
        
        //是否开启景深
        public bool ShouldEnableDof()
        {
            //在LuaUtil设置时 QualityType.High == TerrainXQualityLevel.Mid 在DepthOfField中，关闭景深的条件是 LightFaceEffect.GetQuality() >= (int)TerrainXQualityLevel.Mid 
            //所以此处的开启景深的条件应该是 DeviceType > QualityType.High（两个枚举值的的顺序是相反的，卧槽）
            //否则，会导致来回切换景深导致界面抖动的情况 
            //modify by liujunjie in 2019/4/26 
            //3152】【场景】当贴近人物，人物自动摆头后，远处的景物人物的衣服出现抖动情况
            return ((int)DeviceType > (int)QualityType.Perfect);
        }

        public void PreSetScreenWH()
        {
            if (PlayerPrefs.HasKey("PlayerPrefsKey_1") && PlayerPrefs.GetFloat("PlayerPrefsKey_1") > 0f)
            {
                DeviceModule.bDeviceUserSet = true;
                DeviceUserSetInfo = QualityType.Low;
                MUEngine.MUQualityConfig.ResetScreenRatio(MUEngine.QualityType.Low);
                return;
            }
            else if (PlayerPrefs.HasKey("PlayerPrefsKey_2") && PlayerPrefs.GetFloat("PlayerPrefsKey_2") > 0f)
            {
                DeviceModule.bDeviceUserSet = true;
                DeviceUserSetInfo = QualityType.Mid;
                MUEngine.MUQualityConfig.ResetScreenRatio(MUEngine.QualityType.Mid);
                return;
            }
            else if (PlayerPrefs.HasKey("PlayerPrefsKey_3") && PlayerPrefs.GetFloat("PlayerPrefsKey_3") > 0f)
            {
                DeviceModule.bDeviceUserSet = true;
                DeviceUserSetInfo = QualityType.High;
                MUEngine.MUQualityConfig.ResetScreenRatio(MUEngine.QualityType.High);
                return;
            }
            else if (PlayerPrefs.HasKey("PlayerPrefsKey_42") && PlayerPrefs.GetFloat("PlayerPrefsKey_42") > 0f)
            {
                DeviceModule.bDeviceUserSet = true;
                DeviceUserSetInfo = QualityType.Perfect;
                MUEngine.MUQualityConfig.ResetScreenRatio(MUEngine.QualityType.Perfect);
                return;
            }
            else if (PlayerPrefs.HasKey("PlayerPrefsKey_45") && PlayerPrefs.GetFloat("PlayerPrefsKey_45") > 0f)
            {
                DeviceModule.bDeviceUserSet = true;
                DeviceUserSetInfo = QualityType.AllPerfect;
                MUEngine.MUQualityConfig.ResetScreenRatio(MUEngine.QualityType.AllPerfect);
                return;
            }
            else
            {
                bFirstInitQuality = true;
            }

            MUEngine.MUQualityConfig.ResetScreenRatio((MUEngine.QualityType)DeviceType);
        }

        public void NotifyEngineDevicePerformance()
        {
            QualityType deviceType = mDeviceType;
            if (bDeviceUserSet)
            {
                deviceType = DeviceUserSetInfo;
                bDeviceUserSet = false;
            }

            if (GameModule.Instance != null && GameModule.Instance.EngineReady)
            {
                switch (deviceType)
                {
                    case QualityType.Low:
                        MUEngine.MURoot.MUQualityMgr.LowQuality();
                        // TODO:LightFace丢失 暂时注释
                        // LightFaceEffect.SetQuality(TerrainXQualityLevel.Safe);
                        break;
                    case QualityType.Mid:
                        MUEngine.MURoot.MUQualityMgr.MidQuality();
                        // TODO:LightFace丢失 暂时注释
                        // LightFaceEffect.SetQuality(TerrainXQualityLevel.Low);
                        break;
                    case QualityType.High:
                        MUEngine.MURoot.MUQualityMgr.HeightQuality();
                        // TODO:LightFace丢失 暂时注释
                        // LightFaceEffect.SetQuality(TerrainXQualityLevel.Mid);
                        break;
                    case QualityType.Perfect:
                        MUEngine.MURoot.MUQualityMgr.PerfectQuality();
                        // TODO:LightFace丢失 暂时注释
                        // LightFaceEffect.SetQuality(TerrainXQualityLevel.High);
                        break;
                    case QualityType.AllPerfect:
                        MUEngine.MURoot.MUQualityMgr.PerfectQuality();
                        // TODO:LightFace丢失 暂时注释
                        // LightFaceEffect.SetQuality(TerrainXQualityLevel.Wonderful);
                        break;
                }
            }
        }

        /// <summary>
        /// 设置刘海屏参数
        /// </summary>
        public void SettingNotch()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (OneSDK.GetInstance().IsNotch() == true)
            {
                GameConfig.BaseUIProportion = (Screen.width * 0.92f) / Screen.height;
            }
            else
            {
                try
                {
                    Rect safeRect = Screen.safeArea;
                    if (safeRect.width < Screen.width)
                    {
                        GameConfig.BaseUIProportion = (Screen.width * 0.92f) / Screen.height;
                    }
                    else
                    {
                        GameConfig.BaseUIProportion = Screen.width * 1.0f / Screen.height;
                    }
                }
                catch(Exception e)
                {
                    GameConfig.BaseUIProportion = Screen.width * 1.0f / Screen.height;
                }
            }
#endif
        }
        
        
        private int _screenWidth = -1;
        private int _screenHeight = -1;
        private void SetRealScreenWH()
        {
            if (_screenWidth != Screen.width || _screenHeight != Screen.height || GameConfig.ForceLayoutUI)
            {
                _screenWidth = Screen.width;
                _screenHeight = Screen.height;
                UnityUtil.ScreenSizeChange();
                GameConfig.ForceLayoutUI = false;
#if UNITY_EDITOR || UNITY_STANDALONE
                GameConfig.ScreenLeftUpPoint.x = 0;
                GameConfig.ScreenLeftUpPoint.y = Screen.height;
                GameConfig.ScreenRightDownPoint.x = Screen.width;
                GameConfig.ScreenRightDownPoint.y = 0;
                GameUIAdapter.NeedAdapter = false;
#else
                float wh_r = Screen.width * 1.0f / Screen.height;
                if (wh_r > GameConfig.BaseUIProportion)
                {
                    float tempw = GameConfig.BaseUIProportion * Screen.height;
                    float halfw = (Screen.width - tempw) / 2;
                    GameConfig.ScreenLeftUpPoint.x = halfw;
                    GameConfig.ScreenLeftUpPoint.y = Screen.height;
                    GameConfig.ScreenRightDownPoint.x = Screen.width - halfw;
                    GameConfig.ScreenRightDownPoint.y = 0;
                    GameUIAdapter.NeedAdapter = true;
                }
                else
                {
                    GameConfig.ScreenLeftUpPoint.x = 0;
                    GameConfig.ScreenLeftUpPoint.y = Screen.height;
                    GameConfig.ScreenRightDownPoint.x = Screen.width;
                    GameConfig.ScreenRightDownPoint.y = 0;
                    GameUIAdapter.NeedAdapter = false;
                }
#endif
            }
        }

        /// <summary>
        /// 检测开启景深
        /// </summary>
        public void CheckEnableDof()
        {
            // if (!StoryModule.IsPlaying && !StoryModule.IsEdittingStory && GameModule.Instance.EngineReady)
            // {
            //     if (GameConfig.EnableDof)
            //     {
            //         GameMod.Instance.EnableDof(ShouldEnableDof() && MURoot.MUCamera.Radius != 0 && MURoot.MUCamera.Radius < 2.8f);
            //     }
            //     else
            //     {
            //         GameMod.Instance.EnableDof(false);
            //     }
            // }
        }
        
        //检查是否是安卓64位
        public static bool IsAndroid64bit()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (IntPtr.Size == 8)
            {
                return true;
            }
            return false;
#else
            return false;
#endif
        }

    }
}