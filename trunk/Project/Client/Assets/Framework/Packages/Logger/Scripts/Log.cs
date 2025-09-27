using UnityEngine;

namespace Framework.Case
{
    public static class Log
    {
        public static bool IsOpenLog = true;
        public static string LogFormatString = "{0}----{1}----{2}";
        public static string LogFormatStringWithColor = "{0}{1}----{2}----{3}{4}";
        
        public static void LogError(string _content)
        {
            if (IsOpenLog)
            {
                Debug.LogErrorFormat(LogFormatString, "Time", _content, "UserName");
            }
        }

        public static void LogWarning(string _content)
        {
            if (IsOpenLog)
            {
                Debug.LogWarningFormat(LogFormatString, "Time", _content, "UserName");
            }
        }

        public static void LogWrite(string _content)
        {
            if (IsOpenLog)
            {
                Debug.LogFormat(LogFormatString, "Time", _content, "UserName");
            }
        }
        
        public static void LogYellow(string _content)
        {
            if (IsOpenLog)
            {
                Debug.LogFormat(LogFormatStringWithColor, "<color = yellow>", "Time", _content, "UserName", "</color>");
            }
        }
        
        public static void LogRed(string _content)
        {
            if (IsOpenLog)
            {
                Debug.LogFormat(LogFormatStringWithColor, "<color = red>", "Time", _content, "UserName", "</color>");
            }
        }
    }
}