using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MUGame;
using System.IO;
using System.Text;
using Game.UI;

public class SQLiteLoad
{
    //public static bool resLoaded = false;
    private static string loadName = "ConfData.bytes";
    private const string WRITENAME = "ConfData";
    private static string m_DBName = "";
    private static string m_LocalDB = "LocalData.db";
    private const char CONF_BREAK_SYMBOL = '|';
    public SQLiteLoad()
    {

    }

    public static string GetConfDBPath()
    {
        return GetDBPath(m_DBName);
    }

    public static bool GetCacheBundle()
    {
        try
        {
            //read DB Log
            FileStream fileStreamCode = new FileStream(GetDBLog(), FileMode.Open);
            StreamReader read = new StreamReader(fileStreamCode);
            string info = read.ReadLine();
            read.Close();
            fileStreamCode.Close();
            if (string.IsNullOrEmpty(info))
            {
                return false;
            }

            //get confdata.db
            string dbpath = GetDBname();
            if (!File.Exists(dbpath))
            {
                return false;
            }

            // get confData.db md5
            FileStream db = new FileStream(dbpath, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(db);
            db.Close();
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(dbpath);
#endif
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            string bundlemd5 = sb.ToString();

            //bytes md5
            //bundle md5
            string[] infoArr = info.Split(CONF_BREAK_SYMBOL);
            if (infoArr.Length > 2
                && infoArr[0] == bundlemd5)
            {
                return infoArr[1] == MUEngine.MURoot.ResMgr.GetBundleRealName(GameConfig.GAME_CONF_BUNDLE_NAME);
            }
            return false;
        }catch
        {
            return false;
        }
    }

    public static string GetDBPath(string connectionStr)
    {
        string cStr = string.Empty;//数据库位于工程的根目录
        if (Application.platform == RuntimePlatform.Android)
        {
            cStr = Application.persistentDataPath + "/" + connectionStr;
            //cStr = "URI=file:" + cStr;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            cStr = Application.persistentDataPath + "/" + connectionStr;
            //cStr = "data source=" + cStr;
        }
        else
        {
            cStr = Application.persistentDataPath + "/" + connectionStr;
            //cStr = "data source=" + cStr;
        }
        //Debug.Log(cStr);
        return cStr;
    }

    internal static void OnLoadFile(string name, UnityEngine.Object obj )
    {
        if (name != loadName)
        {
            ConfigUtil.LogWarning("invalid file: " + name + ", need: " + "ConfData.db");
            return;
        }
        TextAsset ta = obj as TextAsset;
        if (ta == null)
        {
            ConfigUtil.LogError("text asset is null");
            return;
        }

        OnLoadFile( name, ta.bytes );        

        ConfigUtil.ReleaseAsset(name, obj);
        //resLoaded = true;
        //D.log("sqlite date load  success");
        //ConfFact.Register();
    }

    internal static void OnLoadFile( string name, byte[] bytes )
    {
        if ( name != loadName )
        {
            ConfigUtil.LogWarning( "invalid file: " + name + ", need: " + "ConfData.db" );
            return;
        }
        
        //save confData.db
        string dbPath = GetDBname();
        FileStream fileStream = new FileStream(dbPath, FileMode.Create);
        fileStream.Write( bytes, 0, bytes.Length );
        fileStream.Close();

        //try
        //{
        //    //  "confData md5|confBundle md5|"
        //    // ConfData.Log
        //    string md5 = MUEngine.SysUtil.GetMD5Str(bytes);
        //    string bundleName = MUEngine.MURoot.ResMgr.GetBundleRealName(name);
        //    StringBuilder strBuilder = new StringBuilder();
        //    strBuilder.Append(md5);
        //    strBuilder.Append(CONF_BREAK_SYMBOL);
        //    strBuilder.Append(bundleName);
        //    strBuilder.Append(CONF_BREAK_SYMBOL);
        //    string str = strBuilder.ToString();

        //    FileStream fileStreamCode = new FileStream(GetDBLog(), FileMode.Create);
        //    StreamWriter sw = new StreamWriter(fileStreamCode);
        //    sw.Write(str);
        //    sw.Flush();
        //    sw.Close();
        //    fileStreamCode.Close();
        //}catch
        //{
        //    D.log(" write confData.Log is failed");
        //}
        
        
        // D.log( "sqlite date load  success" );
       
    }

    private static string getDBPath()
    {
        string path;//数据库位于工程的根目录

        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath + "/";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            path = Application.persistentDataPath + "/";
        }
        else
        {
            path = Application.persistentDataPath + "/";
        }
        return path;
    }

    private static int dbIndex = 0;
    public static string GetDBname()
    {
        string path = getDBPath();
        m_DBName = WRITENAME + ".db";
        string cStr = path + m_DBName;

#if UNITY_STANDALONE_WIN
        if (dbIndex > 0)
        {
            cStr = path + WRITENAME + dbIndex + ".db";
        }
        else
        {
            while (CheckDBUsed(cStr))
            {
                m_DBName = WRITENAME + dbIndex + ".db";
                cStr = path + m_DBName;
                dbIndex++;
            }
        }
#endif
        return cStr;
    }

    public static string GetDBLog()
    {
        string path = getDBPath();
        return path + WRITENAME + ".log";
    }

    public static bool CheckDBUsed(string file)
    {
        bool result;
        if (!File.Exists(file))
        {
            result = false;
        }
        else
        {
            try
            {
                File.Delete(file);
                result = false;
            }
            catch (IOException ioEx)
            {
                result = true;
            }
            catch (Exception ex)
            {
                result = true;
            }
        }
        return result;
    }
    //public  static void LoadSQLite()
    //{
    //    //if (resLoaded) return;
    //    ConfigUtil.LoadRes(loadName, OnLoadFile);
    //}
}