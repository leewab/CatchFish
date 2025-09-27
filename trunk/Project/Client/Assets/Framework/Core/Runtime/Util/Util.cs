using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Game.Core
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    namespace Core
    {
        public static class Util
        {

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
                string StrNohtml = Regex.Replace(Htmlstring, "<[^>]+>", "");
                StrNohtml = Regex.Replace(StrNohtml, "&[^;]+;", "");
                return StrNohtml;
            }

            /// <summary>
            /// 计算字符串的MD5值
            /// </summary>
            public static string md5(string source)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] data = Encoding.UTF8.GetBytes(source);
                byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
                md5.Clear();

                string destString = "";
                for (int i = 0; i < md5Data.Length; i++)
                {
                    destString += Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
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
                    MD5 md5 = new MD5CryptoServiceProvider();
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
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(buffer, 0, buffer.Length);

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
                get { return Application.internetReachability != NetworkReachability.NotReachable; }
            }

            /// <summary>
            /// 是否是无线
            /// </summary>
            public static bool IsWifi
            {
                get { return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork; }
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

            public static bool IsInt(string value)
            {
                return Regex.IsMatch(value, @"^[+-]?\d*$");
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
                if (IntPtr.Size == 8)
                {
                    return "64_bit";
                }
                else
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
                byte br = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                byte bg = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                byte bb = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                byte cc = byte.Parse(hex.Substring(6, 2),
                    NumberStyles.HexNumber); //暂时不考虑透明度 默认为1 有需求的话沟通
                float r = br / 255f;
                float g = bg / 255f;
                float b = bb / 255f;
                float a = cc / 255f;
                return new Color(r, g, b, a);
            }

            public static string FormatTime(float seconds)
            {
                TimeSpan time = TimeSpan.FromSeconds(seconds);
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
            
            /// <summary>
            /// 集合做减法
            /// </summary>
            public static void CollectionMinus<T>(List<T> list1, List<T> list2, ref List<T> o )
            {
                for (int k = 0; k < list1.Count; ++k)
                {
                    T cur_item = list1 [k];
                    bool found = false;
                    for (int j = 0; j < list2.Count; ++j)
                    {
                        T item2 = list2 [j];
                        if (cur_item.Equals(item2))
                        {
                            //在列表2中找到了列表1 的元素，跳过
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        //没有在列表2中找到列表1的元素，加入到输出集
                        o.Add(cur_item);
                    }
                }
            }
        }
    }
}