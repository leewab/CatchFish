using UnityEngine;

namespace Framework.Core
{
    public static class GameLog
    {
        public static bool isOpenLogInEditor = true;
        public static bool isOpenLogInRemote = false;
        
        public static void Log(string log)
        {
#if UNITY_EDITOR
            if (!isOpenLogInEditor) return;
#else
             if (!isOpenLogInRemote) return;
#endif
            Debug.Log(log);
        }

        public static void Error(string log)
        {
#if UNITY_EDITOR
            if (!isOpenLogInEditor) return;
#else
             if (!isOpenLogInRemote) return;
#endif
            Debug.LogError(log);
        }

        public static void Warning(string log)
        {
#if UNITY_EDITOR
            if (!isOpenLogInEditor) return;
#else
             if (!isOpenLogInRemote) return;
#endif
            Debug.LogWarning(log);
        }
    }
}