#define DEBUG_LEVEL_ERROR
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
using System.Diagnostics;
#endif
using System.Text;
using Debug = UnityEngine.Debug;

// setting the conditional to the platform of choice will only compile the method for that platform
// alternatively, use the #defines at the top of this file
public static class D
{
    
    private static bool bLog = true;
    //是否开启写入日志
    public static bool bLogToFile
    {
        set
        {
            bLog = value;
        }
        get
        {
            return bLog;
        }
    }
    static D ()
    {
        
    }

    public static void InitLogCallback()
    {
        DateTime now = DateTime.Now;
        lodFileName =
            $"{Application.persistentDataPath}/log_{now:yyyy_MM_dd}.log";
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        var process = Process.GetCurrentProcess();
        lodFileName = $"{Application.persistentDataPath}/log_{now:yyyy_MM_dd}_{process.Id}.log";
#endif
        sw = new System.IO.StreamWriter(lodFileName, true, System.Text.Encoding.UTF8);
        Application.logMessageReceived += logCallback;
        //Application.RegisterLogCallback(logCallback);
    }

    public static void Dispose()
    {
        Application.logMessageReceived -= logCallback;
        sw?.Dispose();
    }
    
    public static void logCallback (string log, string stackTrace, LogType type)
    {
        string logType = "";
        switch (type)
        {
            case LogType.Error:
                logType = "Error";
                break;
            case LogType.Assert:
                logType = "Assert";
                break;
            case LogType.Warning:
                logType = "Warning";
                break;
            case LogType.Log:
                logType = "Log";
                break;
            case LogType.Exception:
                logType = "Exception";
                break;
        }
#if DEVELOPMENT_BUILD || UNITY_EDITOR || UNITY_STANDALONE || DEBUG_D_LOG_TO_FILE
        LogToFile(logType, log, stackTrace);
#else
        if (type == LogType.Error || type == LogType.Exception)
        {
            LogToFile(logType, log, stackTrace);
        }
#endif
    }
    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void log (object format, params object[] paramList)
    {
        string formatStr = format as string;
        if (formatStr != null)
            Debug.Log (string.Format (formatStr, paramList));
        else
            Debug.Log (format);         
    }
    
    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void log(string log)
    {
        Debug.Log(log);         
    }

    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void warn (object format, params object[] paramList)
    {
        string formatStr = format as string;
        if (formatStr != null)
            Debug.LogWarning (string.Format (formatStr, paramList));
        else
            Debug.LogWarning (format);
    }

    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional("DEBUG_LEVEL_ERROR")]
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void error(object format, params object[] paramList)
    {
        string formatStr = format as string;
        if (formatStr != null)
            Debug.LogError (string.Format (formatStr, paramList));
        else
            Debug.LogError (format);
    }

    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional( "UNITY_EDITOR" )]
    public static void assert (bool condition)
    {
        assert (condition, string.Empty, true);
    }

    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional( "UNITY_EDITOR" )]
    public static void assert (bool condition, string assertString)
    {
        assert (condition, assertString, false);
    }

    [System.Diagnostics.Conditional("UNITY_STANDALONE_WIN")]
    [System.Diagnostics.Conditional( "UNITY_EDITOR" )]
    public static void assert (bool condition, string assertString, bool pauseOnFail)
    {
        if (!condition) {
            Debug.LogError ("assert failed! " + assertString);
            
            if (pauseOnFail)
                Debug.Break ();
        }
    }

    static string lodFileName = string.Empty;
    static StringBuilder sb = new StringBuilder();
    static System.IO.StreamWriter sw = null;
    static void LogToFile(string prefix, string content, string callstack)
    {
        if (false == bLog)
            return;
        sb.Clear();
        sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        sb.Append(" [");
        sb.Append(prefix);
        sb.Append(" ]");
        sb.AppendLine(content);
        sb.AppendLine(callstack);
        if (sw != null)
        {
            sw.Write(sb.ToString());
            sw.Flush();
        }
    }
    
    /// <summary>
    /// 打开日志文件
    /// </summary>
    public static void OpenLogFile()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            System.Diagnostics.Process.Start(lodFileName);   
        }
    }
}
