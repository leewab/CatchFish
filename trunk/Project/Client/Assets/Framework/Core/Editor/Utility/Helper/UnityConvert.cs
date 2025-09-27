using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public static class UnityConvert
    {
        /// <summary>
        /// Object 2 Byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Object2Byte(UnityEngine.Object obj)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            Debug.Log(path);
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                Debug.Log(fs.Length);
                byte[] buff = new byte[fs.Length];
                fs.Read(buff, 0, (int)buff.Length);
                return buff;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    //关闭资源  
                    fs.Close();
                }
            }
        }       
        
        /// <summary>
        /// object 2 Byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Object2Byte(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                Debug.Log(fs.Length);
                byte[] buff = new byte[fs.Length];
                fs.Read(buff, 0, (int)buff.Length);
                return buff;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    //关闭资源  
                    fs.Close();
                }
            }
        }
    }
}