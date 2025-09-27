using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

#if UNITY_TOLUA
using LuaFramework;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework.Core
{
    public class Util
    {
        private static List<string> luaPaths = new List<string>();

        public static float AspectRatioScale()
        {
            return ((float)Screen.height / Screen.width) / (1920f / 1080f);
        }

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static string Uid(string uid)
        {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static string NoHTML(string Htmlstring)
        {
            //删除HTML
            string StrNohtml = System.Text.RegularExpressions.Regex.Replace(Htmlstring, "<[^>]+>", "");
            StrNohtml = System.Text.RegularExpressions.Regex.Replace(StrNohtml, "&[^;]+;", "");
            return StrNohtml;
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T Get<T>(GameObject go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.transform.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Transform版
        /// </summary>
        public static T Get<T>(Transform go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Component版
        /// </summary>
        public static T Get<T>(Component go, string subnode) where T : Component
        {
            return go.transform.Find(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component
        {
            if (go != null)
            {
                T[] ts = go.GetComponents<T>();
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i] != null) GameObject.Destroy(ts[i]);
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(Transform go) where T : Component
        {
            return Add<T>(go.gameObject);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(GameObject go, string subnode)
        {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(Transform go, string subnode)
        {
            Transform tran = go.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        public static Transform FindChild(GameObject go, string name)
        {
            return FindChild(go.transform,name);
        }

        public static Transform FindChild(Transform tf,string name)
        {
            if(tf)
            {
                var ret = tf.Find(name);
                if(ret)
                {
                    return ret;
                }else
                {
                    for(int i=0;i<tf.childCount;i++)
                    {
                        ret = FindChild(tf.GetChild(i),name);
                        if(ret)
                        {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode)
        {
            Transform tran = go.parent.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        public static string md5Bytes(byte[] buffer)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(buffer,0,buffer.Length);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5bytes fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go)
        {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 清理内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect(); 
            Resources.UnloadUnusedAssets();
#if UNITY_TOLUA
            LuaController mgr = GameController.Instance.LuaCtrl;
            if (mgr != null) mgr.LuaGC();
#endif
        }

        // public static string GetRelativePath()
        // {
        //     if (Application.isEditor)
        //         // return System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + AppConst.AssetDir + "/";
        //         return ResConst.GetResRelativePath(ResType.NativeFile);
        //     else if (Application.isMobilePlatform || Application.isConsolePlatform)
        //         return ResConst.GetResRelativePath(ResType.NativeFile);
        //     else // For standalone player.
        //         return Application.streamingAssetsPath + "/";
        // }

        /// <summary>
        /// 取得行文本
        /// </summary>
        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        /// <summary>
        /// 应用程序内容路径
        /// </summary>
        public static string AppContentPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.streamingAssetsPath;
                    break;
            }
            return path;
        }

        public static void Log(string str)
        {
            Debug.Log(str);
        }

        public static void LogWarning(string str)
        {
            Debug.LogWarning(str);
        }

        public static void LogError(string str)
        {
            Debug.LogError(str);
        }

        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        /// <summary>
        /// 防止初学者不按步骤来操作
        /// </summary>
        /// <returns></returns>
        public static int CheckRuntimeFile()
        {
            if (!Application.isEditor) return 0;
            string streamDir = Application.dataPath + "/StreamingAssets/";
            if (!Directory.Exists(streamDir))
            {
                return -1;
            }
            else
            {
                string[] files = Directory.GetFiles(streamDir);
                if (files.Length == 0) return -1;

                // if (!File.Exists(streamDir + AppMgr.CurAppAsset.Version_Name))
                {
                    return -1;
                }
            }
            string sourceDir = Application.dataPath + "/ToLua/Source/Generate/";
            if (!Directory.Exists(sourceDir))
            {
                return -2;
            }
            else
            {
                string[] files = Directory.GetFiles(sourceDir);
                if (files.Length == 0) return -2;
            }
            return 0;
        }

        //获取格式化的字节大小字符串
        public static string GetBinarySizeString(long b)
        {
            if (b < 1024 * 1024)
            {
                return string.Format("{0:f1}", b / 1024f) + "KB";
            }
            else
            {
                return string.Format("{0:f1}", b / 1024f / 1024f) + "MB";
            }
        }

        public static string GetArchitecture()
        {
            if(System.IntPtr.Size == 8)
            {
                return "64_bit";
            }else
            {
                return "32_bit";
            }
        }

        public static void SetParticleSystemSpeed(ParticleSystem ps, float speed)
        {
            ps.playbackSpeed = speed;
        }

        public static void ChangeLayer(Transform tf, int layer)
        {
            tf.gameObject.layer = layer;
            if (tf.childCount > 0)
            {
                for (int i = 0; i < tf.childCount; i++)
                {
                    ChangeLayer(tf.GetChild(i), layer);
                }
            }
        }

        //Hex颜色转Color
        public static Color HexToColorFunc(string hex)
        {
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);  //暂时不考虑透明度 默认为1 有需求的话沟通
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            //float a = cc / 255f;
            return new Color(r, g, b, 1);
        }

        /// <summary>
        /// 检查运行环境
        /// </summary>
        public static bool CheckEnvironment()
        {
#if UNITY_EDITOR
            int resultId = Util.CheckRuntimeFile();
            if (resultId == -1)
            {
                Debug.LogError("没有找到框架所需要的资源，单击Game菜单下Build xxx Resource生成！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            else if (resultId == -2)
            {
                Debug.LogError("没有找到Wrap脚本缓存，单击Lua菜单下Gen Lua Wrap Files生成脚本！！");
                EditorApplication.isPlaying = false;
                return false;
            }
            // if (Application.loadedLevelName == "Test" && !AppConst.DebugMode)
            // {
            //     Debug.LogError("测试场景，必须打开调试模式，AppConst.DebugMode = true！！");
            //     EditorApplication.isPlaying = false;
            //     return false;
            // }
#endif
            return true;
        }
    }
}