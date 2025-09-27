using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

//通用sdk功能，用于存放一些简单的sdk接口
public class CommonUseSDK
{

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _BrightnessSetValue(float brightness);

    [DllImport("__Internal")]
    private static extern void _BrightnessRecover();


    [DllImport("__Internal")]
    private static extern void _InitAliYunByData(string accessKeyId, string secretKeyId, string endPoint, string bucket);

#endif

#if UNITY_ANDROID
    static AndroidJavaObject m_activity;
    private static void InitAndroidActivity()
    {
        using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            m_activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
    }
#endif
    //修改屏幕亮度
    public static void BrightnessSetValue(float brightness)
    {
        Debug.Log("CommonUseSDK.BrightnessSetValue：" + brightness.ToString());
#if UNITY_IOS && !UNITY_EDITOR
        _BrightnessSetValue(brightness);
#elif UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        m_activity.Call("BrightnessSetValue", brightness);
#endif
    }
    //还原屏幕亮度
    public static void BrightnessRecover()
    {
        Debug.Log("CommonUseSDK.BrightnessRecover");
#if UNITY_IOS && !UNITY_EDITOR
        _BrightnessRecover();
#elif UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        m_activity.Call("BrightnessRecover");
#endif
    }
    public static void SetDeviceDisplayMode(int fpsvalue)
    {
        Debug.Log("CommonUseSDK.SetDeviceDisplayMode");

#if UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        m_activity.Call("SetDeviceFPS",fpsvalue);
#endif
    }

    //获取设备支持的帧率列表
    public static void GetDeviceDisplayModels()
    {
        Debug.Log("CommonUseSDK.GetDeviceDisplayModels");

#if UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        m_activity.Call("GetDeviceDisplayModels");
#endif
    }
    public static string UrlEncodeTwice(string str)
    {
        //System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.UrlEncode(str));
        return WWW.EscapeURL(WWW.EscapeURL(str)); 
    }

    //获取设备名称
    public static string GetDeviceModel()
    {
        string deviceModel = SystemInfo.deviceModel;
        deviceModel = Regex.Replace(deviceModel, @"\s", "");
        return deviceModel;
    }

    public static void UpLoadFile(string keyName, string fullName)
    {
#if UNITY_IOS && !UNITY_EDITOR

#endif
    }

    public static void InitAliYunByData(string accessKeyId, string secretKeyId, string endPoint, string bucket)
    {
#if UNITY_IOS && !UNITY_EDITOR
        _InitAliYunByData(accessKeyId, secretKeyId, endPoint, bucket);
#elif UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        m_activity.Call("InitAliYunByData", accessKeyId, secretKeyId, endPoint, bucket);
#endif
    }

    public static bool IsSimulator()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return false;
#elif UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        return m_activity.Call<bool>("IsSimulator");
#else
        return false;     
#endif
    }

    public static int GetAndroidVersion()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return -1;
#elif UNITY_ANDROID && !UNITY_EDITOR
        if (m_activity == null)
        {
            InitAndroidActivity();
        }
        return m_activity.Call<int>("GetAndroidVersion");
#else
        return -1;
#endif
    }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    public enum DMDO
    {
        DEFAULT = 0,
        D90 = 1,
        D180 = 2,
        D270 = 3
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct DEVMODE
    {
        public const int DM_DISPLAYFREQUENCY = 0x400000;
        public const int DM_PELSWIDTH = 0x80000;
        public const int DM_PELSHEIGHT = 0x100000;
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public DMDO dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
        public int dmICMMethod;
        public int dmICMIntent;
        public int dmMediaType;
        public int dmDitherType;
        public int dmReserved1;
        public int dmReserved2;
        public int dmPanningWidth;
        public int dmPanningHeight;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int ChangeDisplaySettings([In] ref DEVMODE lpDevMode, int dwFlags);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern bool EnumDisplaySettings(string lpszDeviceName, System.Int32 iModeNum, ref DEVMODE lpDevMode);
#endif

    //获取显示器刷新帧率
    public static int GetWindowsDisplayFps()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        try
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));
            if (EnumDisplaySettings(null, -1, ref dm))
            {
                Debug.Log("CommonUseSDK.GetWindowsDisplayFps fps = " + dm.dmDisplayFrequency.ToString());
                return dm.dmDisplayFrequency;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
#endif
        return 0;
    }
}
