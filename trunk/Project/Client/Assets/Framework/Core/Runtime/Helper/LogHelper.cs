using UnityEngine;

namespace Game.Core
{
    public static class LogHelper
    {
        public static bool IsOpenLog
        {
            get
            {
                var isLocalLog  = AppManager.AppFileData == null || AppManager.AppFileData.IsLocalLog;
                var isRemoteLog = AppManager.AppFileData == null || AppManager.AppFileData.IsRemoteLog;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                if (!isLocalLog) return false;
#else
                if (!isRemoteLog) return false;
#endif
                return true;
            }
        }
        
        public static void Log(string log)
        {
            if (!IsOpenLog) return;
            Debug.Log(log);
        }
        
        public static void Log(string log1, string log2)
        {
            if (!IsOpenLog) return;
            Debug.Log($"{log1} {log2}");
        }

        public static void LogYellow(string log)
        {
            if (!IsOpenLog) return;
            Debug.Log("<color=#FF8200>"+log+"</color>");
        }
        
        public static void LogGreen(string log)
        {
            if (!IsOpenLog) return;
            Debug.Log("<color=#27CD07>"+log+"</color>");
        }
        
        public static void LogBlue(string log)
        {
            if (!IsOpenLog) return;
            Debug.Log("<color=#1A08CC>"+log+"</color>");
        }

        public static void Error(string log)
        {
            if (!IsOpenLog) return;
            Debug.LogError(log);
        }

        public static void Warning(string log)
        {
            if (!IsOpenLog) return;
            Debug.LogWarning(log);
        }
    }
}