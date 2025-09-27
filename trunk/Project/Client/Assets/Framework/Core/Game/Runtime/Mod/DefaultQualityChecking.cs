using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MUEngine;
using System.IO;
using Game;
using Game.UI;

namespace MUGame
{
    public class DefaultQualityChecking
    {

        private static DefaultQualityChecking _instance = null;
        public static DefaultQualityChecking Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DefaultQualityChecking();
                }

                return _instance;
            }
        }

        private float cpuStartTime = 0f;
        private float cpuLevel = 0;
        private float TotalLevel = 0;
        public void StartCpuTest()
        {
            cpuStartTime = Time.realtimeSinceStartup;
        }

        public void StopCpuTest()
        {
            float deltaTime = Time.realtimeSinceStartup - cpuStartTime;
            Debug.Log("first cpu Time " + cpuLevel);
            Debug.Log("first cpu Time " + deltaTime);
            if (deltaTime < 1.0f)//高配模板
            {
                cpuLevel = 4;
            }
            else if (deltaTime < 0.15f)//中配模板 2.58  1.47
            {
                cpuLevel = 3;
            }
            else if (deltaTime < 0.3f)//低配模板
            {
                cpuLevel = 2;
            }
            else
            {
                cpuLevel = 1;
            }

            Debug.Log("first cpu Level " + cpuLevel);
        }

        /// <summary>
        /// cpu 标量比
        /// </summary>
        /// <returns></returns>
        private int checkCpuDevice()
        {
            int deviceLevel = 3;
            int graphicMemory = SystemInfo.graphicsMemorySize;
            int processorFrequency = SystemInfo.processorFrequency;
            int processorCount = SystemInfo.processorCount;

            int systemMemory = SystemInfo.systemMemorySize;


#if UNITY_ANDROID
            if (processorFrequency <= 1800
                || systemMemory <= 2048)
            {
                deviceLevel = 0;
            }
            else if (processorFrequency > 2100
                    && graphicMemory > 1000)
            {
                deviceLevel = 3;
            }
            else
            {
                deviceLevel = 2;
            }
#endif
            return deviceLevel;
        }


        private float cpuIOStartTime = 0f;
        private float cpuIOLevel = 0;
        public void StartCpuIOTest()
        {
            cpuIOStartTime = Time.realtimeSinceStartup;
        }

        public void StopCpuIOTest()
        {
            cpuIOStartTime = Time.realtimeSinceStartup - cpuIOStartTime;
            Debug.Log("{QualtiyTest}cpuIOStartTime" + cpuIOStartTime);
#if UNITY_ANDROID

            float cpuIOlevel = 0;
            if (cpuIOStartTime < 0.37f) //1.27432f mate10
            {
                cpuIOlevel = 4;
            }else if (cpuIOStartTime < 0.75f) // 1.528959f 红米note7
            {
                cpuIOlevel = 3;
            }
            else if (cpuIOStartTime < 1.35f)//6.08f 红米6A
            {
                cpuIOlevel = 2;
            }
            else if (cpuIOStartTime < 1.5f)
            {
                cpuIOlevel = 1;
            }else
            {
                cpuIOlevel = 0;
            }

            cpuLevel = cpuIOlevel;

            int level = 0;
            if (checkGraphicDevice(out level))
            {
                if (isOppoVivo())
                {
                    level = Math.Min(3, level);
                }
                Debug.Log("{QualtiyTest}getGraphic " + level);
                TotalLevel = level;
            }else
            {
                TotalLevel = cpuIOlevel == 4? cpuIOlevel : Math.Min(2, cpuLevel); 
            }

            RecheckDPI();
            if (CommonUseSDK.IsSimulator())
            {
                TotalLevel = 4;
            }
#elif UNITY_IPHONE
            TotalLevel = (float)GameConfig.DeviceType;
#else
            //其他平台暂定全部最高
            TotalLevel = 4;
#endif
            Debug.Log("{QualtiyTest}最终推荐级别： TestDeviceInfo " + TotalLevel);
            DeviceModule.Instance.RecommendDeviceInfo = (QualityType)Math.Floor(TotalLevel + 0.1f);

            if (DeviceModule.FirstInitQuality && (DeviceModule.Instance.DeviceType != DeviceModule.Instance.RecommendDeviceInfo))
            {
                //MUEngine.MUQualityConfig.ResetScreenRatio((MUEngine.QualityType)GameConfig.TestDeviceInfo);
            }

            PrintDevice();
        }

        private void PrintDevice()
        {
            //云测打印机型LOG
            StringWriter TestInfo = new StringWriter();
            TestInfo.WriteLine("SMDL INFO:");
            TestInfo.WriteLine("deviceName " + SystemInfo.deviceName);
            TestInfo.WriteLine("deviceModel " + SystemInfo.deviceModel);
            TestInfo.WriteLine("systemMemorySize " + SystemInfo.systemMemorySize);
            //Cpu
            TestInfo.WriteLine("processorType " + SystemInfo.processorType);
            TestInfo.WriteLine("processorCount " + SystemInfo.processorCount);
            TestInfo.WriteLine("processorFrequency " + SystemInfo.processorFrequency);
            //Gpu
            TestInfo.WriteLine("graphicsDeviceName " + SystemInfo.graphicsDeviceName);
            TestInfo.WriteLine("graphicsDeviceID " + SystemInfo.graphicsDeviceID);
            TestInfo.WriteLine("graphicsDeviceType " + SystemInfo.graphicsDeviceType);
            TestInfo.WriteLine("graphicsDeviceVendor " + SystemInfo.graphicsDeviceVendor);
            TestInfo.WriteLine("graphicsDeviceVendorID " + SystemInfo.graphicsDeviceVendorID);
            TestInfo.WriteLine("graphicsDeviceVersion " + SystemInfo.graphicsDeviceVersion);
            TestInfo.WriteLine("graphicsMemorySize " + SystemInfo.graphicsMemorySize);
            TestInfo.WriteLine("graphicsMultiThreaded " + SystemInfo.graphicsMultiThreaded);
            TestInfo.WriteLine("graphicsShaderLevel " + SystemInfo.graphicsShaderLevel);
            TestInfo.WriteLine("ASTC TEST: " + SystemInfo.SupportsTextureFormat(TextureFormat.ASTC_6x6));
            Debug.Log(TestInfo.ToString());
        }

        private void RecheckDPI()
        {
            if (Screen.dpi < 300)//低配
            {
                TotalLevel = Math.Min(1, TotalLevel); ;
                Debug.Log("{QualtiyTest}DPI: < 300");
            }
            else if (Screen.dpi < 400 && GraphicLevel < 4)//最高中配
            {
                TotalLevel = Math.Min(2, TotalLevel);
                Debug.Log("{QualtiyTest}DPI: < 400");
            }

            if (SystemInfo.systemMemorySize < 2100 && GraphicLevel < 4)//内存小于2g 低配
            {
                TotalLevel = Math.Min(1, TotalLevel);
            }

            Debug.Log("{QualtiyTest}DPI:" + Screen.dpi);
        }

        static int GraphicLevel = -1;
        /// <summary>
        /// 检查gpu型号
        /// </summary>
        /// 4:
        /// Adreno (TM) 
        ///     630
        /// Mali
        ///     G76
        /// 3:
        /// Adreno (TM) 
        ///     616系列 530以上 （430）
        /// Mali
        ///     G71 G72（暂定） Mali-T880
        /// <returns></returns>
        public bool checkGraphicDevice(out int level)
        {
            //直接返回上次的检测结果
            if (GraphicLevel > -1)
            {
                level = GraphicLevel;
                return level >= 0;
            }

            level = 0;
            GraphicLevel = 0;
            int graphicMemory = SystemInfo.graphicsMemorySize;
            string graphicsDeviceName = SystemInfo.graphicsDeviceName;
            int graphicsDeviceID = SystemInfo.graphicsDeviceID;
            if (graphicsDeviceName.Contains("Mali"))
            {
                isMali = true;
            }

            List<string[]> GpuList = new List<string[]>();
            GpuList.Add(Gpu0);
            GpuList.Add(Gpu1);
            GpuList.Add(Gpu2);
            GpuList.Add(Gpu3);
            GpuList.Add(Gpu4);
            for (int lvli = 0; lvli < GpuList.Count; lvli++)
            {
                for (int i = 0; i < GpuList[lvli].Length; i++)
                {
                    if (GpuList[lvli][i].Contains(graphicsDeviceName))
                    {
                        level = lvli;
                        GraphicLevel = level;
                        return true;
                    }
                }

            }

            isGpuMatch = false;
            Debug.Log("{QualtiyTest}not find GPU Name:" + graphicsDeviceName);
            return false;
        }

        bool isMali = false;
        public bool IsMaliGPU()
        {
            return isMali;
        }

        private static bool isOppoVivo()
        {
            return SystemInfo.deviceModel.Contains("vivo")
            || SystemInfo.deviceModel.Contains("OPPO");
        }

        private static bool isGpuMatch = true;
        public  bool IsGpuMatch()
        {
            return isGpuMatch;
        }


        string[] Gpu4 = {
            "Mali-G78", "Mali-G77","Mali-G76", "Adreno (TM) 650", "Adreno (TM) 640", "Adreno (TM) 630"};
        //608
        string[] Gpu3 = {
            "Adreno (TM) 540", "Adreno (TM) 620", "Adreno (TM) 618", "Adreno (TM) 430", "Adreno (TM) 530", "Adreno (TM) 616",
            "Adreno (TM) 615", "Adreno (TM) 612","PowerVR GXA6850",
            "Mali-G72 MP3","Mali-G72","Mali-G52","Mali-G57","Mali-G52 MC2",
            "PowerVR GT7600" };
        //826
        string[] Gpu2 = {
            "Mali-G71", "Mali-G51", "Mali-T880","Mali-T760",
            "Adreno (TM) 418", "Adreno (TM) 420","Adreno (TM) 330",
            "Adreno (TM) 506","Adreno (TM) 509","Adreno (TM) 508","Adreno (TM) 512",
            "Adreno (TM) 610", 
            "PowerVR Rogue GE8340","PowerVR Rogue GE8430","	PowerVR G6430","PowerVR GX6450"
        };
        //850
        string[] Gpu1 = {
            "PowerVR Rogue GE8322", "PowerVR Rogue GE8325",
            "PowerVR SGX554MP4","PowerVR G6400","PowerVR GX6250",
            "Adreno (TM) 505","Adreno (TM) 504","Adreno (TM) 405","Adreno (TM) 320",
            "Mali-T860","Mali-T604","Mali-T628","PowerVR Rogue GE8320","PowerVR Rogue GE8300",
            "Mali-T830"
        };

        string[] Gpu0 = {
            "PowerVR Rogue GE8100","PowerVR Rogue GE8200",
            "PowerVR SGX543MP4","PowerVR G6200",
            "Adreno (TM) 200", "Adreno (TM) 203", "Adreno (TM) 225", "Adreno (TM) 305", "Adreno (TM) 304","Adreno (TM) 308",
            "Mali-T624","Mali-T720"
        };

    }
}
