using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.IO.Compression;
using MUEngine;
using System.Reflection;
using Game.Core;
using Game.UI;

namespace Game
{
    public static class SysUtil
    {
        public static bool isDebug = true;

        static DateTime BaseDate = new DateTime(1970, 1, 1);

        public static string CreateGUID()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static void DebugBreak()
        {
            if(isDebug)
                Debug.Break();
        }

        public static void Assert(bool val, string msg)
        {
            if(isDebug && !val)
            {
                Debug.Log("assert failed:\n" + msg);
                Debug.Break();
            }
        }

        public static int MilliSecToFps(long time)
        {
            return 1000 / (int)time;
        }

        public static long FpsToMilliSec(int fps)
        {
            return 1000 / fps;
        }

        public static long TickToMilliSec(long tick)
        {
            return tick / (10 * 1000);
        }

        public static long MilliSecToTick(long time)
        {
            return time * 10 * 1000;
        }

        public static float MilliSecToSec(long ms)
        {
            return ((float)ms) / 1000;
        }

        public static long SecToMilliSec(float s)
        {
            return (long)(s * 1000);
        }

        public static string GetAddr(string ip, int port)
        {
            return ip + ":" + port;
        }

        public static long GetTotalMilliSecFromBase(int year,int month,int day,int hour,int min,int sec)
        {
            DateTime dt = new DateTime(year, month, day, hour, min, sec);
            TimeSpan ts = dt.Subtract(BaseDate);
            long totalMilliSec = (long)ts.TotalMilliseconds;
            return totalMilliSec;
        }

        public static int GetCurUtcHour()
        {
            return DateTime.UtcNow.Hour;
        }

        public static string GetMD5Str(string str, int bitCount)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            if (bitCount == 32)
            {
                byte[] encryptedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < encryptedBytes.Length; i++)
                {
                    sb.AppendFormat("{0:X2}", encryptedBytes[i]);
                }
                return sb.ToString();
            }
            else
            {
                string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);
                t2 = t2.Replace("-", "");
                return t2;
            }
        }
		

        public static string GetMD5Str(Stream stream)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encryptedBytes = md5.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:X2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }

        public static void DrawDebugDimension(Vector3 dim, MathUtil.BoundOriginType origin,
                                            Vector3 pos, Color col)
        {
            if(isDebug)
            {
                Bounds bd = MathUtil.DimensionToBound(dim, origin);
                Matrix4x4 mtx = Matrix4x4.identity;
                mtx.SetTRS(pos, Quaternion.identity, Vector3.one);
                DrawDebugBounds(bd, mtx, col);
            }
        }

        public static void DrawDebugBounds(Bounds bound, Matrix4x4 mtx, Color col)
        {
            if(isDebug)
            {
                Vector3 src = Vector3.zero;
                Vector3 dst = Vector3.zero;

                //bottom
                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                //top
                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.max.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.max.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                //side
                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.min.y;
                src.z = bound.min.z;
                dst.x = bound.max.x;
                dst.y = bound.max.y;
                dst.z = bound.min.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.max.x;
                src.y = bound.max.y;
                src.z = bound.max.z;
                dst.x = bound.max.x;
                dst.y = bound.min.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);

                src.x = bound.min.x;
                src.y = bound.min.y;
                src.z = bound.max.z;
                dst.x = bound.min.x;
                dst.y = bound.max.y;
                dst.z = bound.max.z;
                src = mtx.MultiplyPoint(src);
                dst = mtx.MultiplyPoint(dst);
                UnityEngine.Debug.DrawLine(src, dst, col);
            }
        }

        public static KeyCode TransKeyToKeyCode(int key)
        {
            return (KeyCode)key;
        }

        public static int TransKeyCodeToKey(KeyCode code)
        {
            return (int)code;
        }

        //lower str, then rm all space
        public static string GetPureStr(string txt)
        {
            string pt = txt.ToLower();
            pt = pt.Trim();
            pt = pt.Replace('\t', '_');
            pt = pt.Replace(' ', '_');
            pt = pt.Replace('-', '_');
            return pt;
        }

        public static bool StrPureEqual(string txt1, string txt2)
        {
            string pt1 = GetPureStr(txt1);
            string pt2 = GetPureStr(txt2);
            return pt1 == pt2;
        }

        public static bool DetectEnterKeyInStr(string str)
        {
            if(str == null || str == string.Empty) return false;

            return str[str.Length - 1] == '\n';
        }

        public static string RmEnterKeyInStr(string str)
        {
            if(str == null || str == string.Empty) return str;

            string nStr = str;
            while(nStr.Length > 0)
            {
                if(nStr[nStr.Length - 1] == '\n' || nStr[nStr.Length - 1] == '\r')
                    nStr = nStr.Remove(nStr.Length - 1);
                else
                    break;
            }

            return nStr;
        }
		
		    /*
        public static void ClearGuiContent(GUIContent content)
        {
            content.image = null;
            content.text = string.Empty;
            content.tooltip = string.Empty;
        }
        */

        public static bool IsEqual(float f1,float f2)
        {
            return System.Math.Abs(f1 - f2) < 0.00001f;
        }
		
	    public static bool IsEqualXZ ( Vector3 v1, Vector3 v2 )
		    {
			    return IsEqual(v1.x, v2.x) && IsEqual(v1.z, v2.z);
		    }

        public static bool IsEqual(Vector3 v1,Vector3 v2)
        {
            return IsEqual(v1.x, v2.x) && IsEqual(v1.y, v2.y) && IsEqual(v1.z, v2.z);
        }

        public static GameObject CreateEmptyGameObj(string name)
        {
            GameObject go = new GameObject();
            go.name = name;
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go;
        }

        public static bool RaycastPlane(Vector3 from,Vector3 dir,Plane plane,out Vector3 pos)
        {
            Ray r = new Ray(from, dir);
            return RaycastPlane(r, plane, out pos);
        }

        public static bool RaycastPlane(Ray ray,Plane plane,out Vector3 pos)
        {
            float dist;
            pos = Vector3.zero;
            bool result = plane.Raycast(ray, out dist);
            if (result)
            {
                pos = ray.origin + ray.direction * dist;
            }
            return result;
        }

        public static Vector3 MakeOrientation( Vector3 target, Vector3 from )
        {
            Vector3 diff = target - from;
            diff.y = 0;
            return diff.normalized;
        }

        static string[] emptyStrArray = { };
        public static string[] SplitString(string str, string flag)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(flag))
            {
                return emptyStrArray;
            }

            string[] temp = new string[]{flag};
            string[] array = str.Split(temp, StringSplitOptions.RemoveEmptyEntries);

            return array;
        }

		
        /// <summary>
        /// 获得ui元素的实际宽高（lua中可用）
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="axis">0 宽 1 高</param>
        /// <returns></returns>
        public static float GetUISize(RectTransform rect,int axis)
        {
            return UnityEngine.UI.LayoutUtility.GetPreferredSize(rect, axis);
        }

        public static bool UIContainsScreenPoint(RectTransform rect, Vector2 screenPoint)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint, UIRoot.UICamera);
        }

        public static void ForceRebuildLayoutImmediate(RectTransform rect)
        {
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        public static float GetCanvasSize(Canvas canvas, int axis)
        {
            if (axis == 0)
                return (canvas.transform as RectTransform).rect.width;
            else
                return (canvas.transform as RectTransform).rect.height;
        }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="root">根节点</param>
        // /// <param name="containsRoot">是否包含根节点</param>
        // /// <param name="ignoreInactive">是否忽略隐藏节点</param>
        // /// <param name="ignoreChildPrefix">忽略节点前缀</param>
        // /// <param name="extraChild">额外需要添加的</param>
        // /// <returns></returns>
        // public static List<RectTransform> LuaGetUIChild(RectTransform root, bool containsRoot, bool ignoreInactive,
        //     LuaInterface.LuaTable ignoreChildPrefix, LuaInterface.LuaTable extraChild)
        // {
        //     if (root == null) return new List<RectTransform>();
        //     List<RectTransform> lst = GetUIChild(root, containsRoot, ignoreInactive,
        //         ignoreChildPrefix == null ? null : ignoreChildPrefix.ToArray());
        //     if (extraChild != null)
        //     {
        //         object[] elst = extraChild.ToArray();
        //         for (int i = 0, imax = elst.Length; i < imax; i++)
        //         {
        //             lst.Add((RectTransform)elst[i]);
        //         }
        //     }
        //     return lst;
        // }
        
        /// <summary>
        /// 获得子节点
        /// </summary>
        /// <param name="root">根节点</param>
        /// <param name="includeInactive">是否包括隐藏节点</param>
        /// <param name="ignoreChildPrefix">忽略的名字前缀数组</param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public static List<RectTransform> GetUIChild(RectTransform root, bool containsRoot = true,
            bool ignoreInactive = true, object[] ignoreChildPrefix = null, List<RectTransform> lst = null)
        {
            if (lst == null) lst = new List<RectTransform>();
            if (root == null) return lst;
            if (containsRoot) lst.Add(root);
            int ccount = root.childCount;
            RectTransform child = null;
            for (int i = 0; i < ccount; i++)
            {
                child = root.GetChild(i) as RectTransform;
                if (child != null)
                {
                    if (ignoreInactive && !child.gameObject.activeInHierarchy) continue;
                    if (ignoreChildPrefix != null)
                    {
                        bool find = false;
                        for (int j = 0; j < ignoreChildPrefix.Length; j++)
                        {
                            string prefix = (string)ignoreChildPrefix[j];
                            if (!string.IsNullOrEmpty(prefix) &&
                                child.name.StartsWith(prefix))
                            {
                                find = true;
                                break;
                            }
                        }
                        if (find) continue;
                    }

                    lst.Add(child);
                    GetUIChild(child, false, ignoreInactive, ignoreChildPrefix, lst);
                }
            }
            return lst;
        }
        public static void SetUIMinAnchor(RectTransform rect,Vector2 anchor)
        {
            rect.anchorMin = anchor;
        }
        public static void SetUIMaxAnchor(RectTransform rect, Vector2 anchor)
        {
            rect.anchorMax = anchor;
        }

        public static bool IsOverUI(RectTransform rect)
        {
            EventSystem uiEventSystem = EventSystem.current;
            if (uiEventSystem != null)
            {
                PointerEventData uiPointerEventData = new PointerEventData(uiEventSystem);
                uiPointerEventData.position = Input.mousePosition;
                //List<RaycastResult> uiRaycastResultCache = new List<RaycastResult>();
                //uiEventSystem.RaycastAll(uiPointerEventData, uiRaycastResultCache);
                List<RaycastResult> uiRaycastResultCache = uiEventSystem.CachedRaycastAll(uiPointerEventData);

                if (uiRaycastResultCache.Count > 0)
                {
                    Transform t = uiRaycastResultCache[0].gameObject.transform;
                    if (t == rect) return true;
                    Transform temp = t;
                    while(temp.parent != null)
                    {
                        temp = temp.parent;
                        if (temp == rect) return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 获得一个UI的包围盒大小
        /// 两个节点的情况
        /// </summary>
        /// <param name="root"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static Bounds GetUIBounds(RectTransform root, RectTransform child)
        {
            return GetUIBounds(root, new List<RectTransform> { root, child });
        }
        /// <summary>
        /// 获得一个UI的包围盒大小
        /// (修改的这个方法RectTransformUtility.CalculateRelativeRectTransformBounds)
        /// 因为UI中可能包含ScrollRect这种，不能全部孩子节点全部都参与计算
        /// 所有分开两个函数处理Bounds(一个是获取子节点，一个就是获取bounds)
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static Bounds GetUIBounds(RectTransform root, List<RectTransform> lst = null)
        {
            Vector3[] s_Corners = new Vector3[4];
            if (lst == null)
            {
                lst = new List<RectTransform>(root.GetComponentsInChildren<RectTransform>(false));
            }
            if (lst.Count > 0)
            {
                Vector3 vector = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
                Vector3 vector2 = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
                Matrix4x4 worldToLocalMatrix = root.worldToLocalMatrix;
                int i = 0;
                int num = lst.Count;
                while (i < num)
                {
                    lst[i].GetWorldCorners(s_Corners);
                    for (int j = 0; j < 4; j++)
                    {
                        Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(s_Corners[j]);
                        vector = Vector3.Min(lhs, vector);
                        vector2 = Vector3.Max(lhs, vector2);
                    }
                    i++;
                }
                Bounds result = new Bounds(vector, Vector3.zero);
                result.Encapsulate(vector2);
                return result;
            }
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        public static float Angle_360( Vector3 from, Vector3 to)
        {
            float angle = Vector3.Angle( from, to );;
            Vector3 v3 = Vector3.Cross( from, to );
            if ( v3.y > 0 )
            {
               
            }                
            else
            {
                angle =  360 - angle;
            }                       

            return angle / 360 * 2 * Mathf.PI;

        }

        public static Vector3 Angle2Axis( Vector3 from, Vector3 to)
        {
            Quaternion qua = Quaternion.FromToRotation( from, to );

            Vector3 axix = qua * new Vector3( 0, 0, 0.5f );

            return axix;
        }

        public static bool CheckTouchDown()
        {
            return Input.GetMouseButtonDown(0)|| Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        }
        public static Vector2 ConverScreenPosition(Vector2 source)
        {
            //source.x -= GameConfig.ScreenLeftUpPoint.x;
            //source.y -= GameConfig.ScreenRightDownPoint.y;
            return source;
        }
        public static Vector2 InputPosition
        {
            get
            {
                if (Input.touchCount > 0)
                {
                    Touch t;
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        t = Input.GetTouch(i);
                        if (t.phase == TouchPhase.Began)
                        {
                            return ConverScreenPosition(t.position);
                        }
                    }
                    return ConverScreenPosition(Input.GetTouch(0).position);
                }
                return ConverScreenPosition(Input.mousePosition);
            }
        }

        static public string Encrypt( string stringToEncrypt, string sKey )
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.GetEncoding( "UTF-8" ).GetBytes( stringToEncrypt );
                byte[] keyBytes = ASCIIEncoding.UTF8.GetBytes( sKey );
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream( ms, des.CreateEncryptor( keyBytes, keyBytes ), CryptoStreamMode.Write );
                cs.Write( inputByteArray, 0, inputByteArray.Length );
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach ( byte b in ms.ToArray() )
                {
                    ret.AppendFormat( "{0:X2}", b );
                }
                ret.ToString();
                return ret.ToString();
            }
            catch( System.Exception se )
            {
                return stringToEncrypt;
            }
        }
        static public string Decrypt( string stringToDecrypt, string sKey )
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[stringToDecrypt.Length / 2];
                for ( int x = 0; x < stringToDecrypt.Length / 2; x++ )
                {
                    int i = ( Convert.ToInt32( stringToDecrypt.Substring( x * 2, 2 ), 16 ) );
                    inputByteArray[x] = (byte)i;
                }
                des.Key = ASCIIEncoding.UTF8.GetBytes( sKey );
                des.IV = ASCIIEncoding.UTF8.GetBytes( sKey );
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream( ms, des.CreateDecryptor(), CryptoStreamMode.Write );
                cs.Write( inputByteArray, 0, inputByteArray.Length );
                cs.FlushFinalBlock();
                return System.Text.Encoding.Default.GetString( ms.ToArray() );
            }
            catch( System.Exception se )
            {
                return stringToDecrypt;
            }
        }

        /// <summary>
        /// 日期字符串转unix时间戳
        /// 格式：2017-05-06 18:50:00
        /// </summary>
        /// <returns></returns>
        public static int StrToUnixTimestamp(string timeStr)
        {
            if (string.IsNullOrEmpty(timeStr))
                return 0;

            DateTime date;
            if(DateTime.TryParse(timeStr,out date))
            {
                DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
                return Convert.ToInt32((date - DateStart).TotalSeconds);
            }
            return 0;
        }

        public static Vector2 GetUIScreenPoint(RectTransform rect)
        {
            return GetScreenPoint(rect.position);
        }

        public static Vector2 GetScreenPoint(Vector3 pos)
        {
            //Vector3 vp = UIRoot.UICamera.WorldToViewportPoint(pos);
            Vector2 sp = RectTransformUtility.WorldToScreenPoint(UIRoot.UICamera, pos);
            return ConverScreenPosition(sp);
        }

        public static Vector2 GetLocalPointFromScreenPoint(RectTransform rect, Vector2 screenPoint)
        {
            Vector2 localPoint = new Vector2(0, 0);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, UIRoot.UICamera, out localPoint);
            return localPoint;
        }

        public static Vector2 GetTargetLocalPointFromScreenPoint(Vector3 targetPos, RectTransform rect)
        {
            Vector2 lp = RectTransformUtility.WorldToScreenPoint(UIRoot.UICamera, targetPos);
            Vector2 op;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, lp, UIRoot.UICamera, out op);
            return op;
        }

        public static Vector2 Get3DWorldPointToLocalPointInRectangle(Vector3 wroldPos, RectTransform rect)
        {
            Vector2 lp = RectTransformUtility.WorldToScreenPoint(MURoot.Scene.Camera.Camera, wroldPos);
            lp = lp / MUQualityConfig.Scene2UIScale;
            Vector2 retPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, lp, UIRoot.UICamera, out retPos);
            return retPos;
        }

        public static Vector3 GetPosAfterFlatMoveWithXZ(Transform tf,float dx,float dz)
        {
            Vector3 ret = tf.right * dx + (Quaternion.Euler(0, 90, 0) * tf.right)*dz;
            ret.y = 0;
            ret = tf.position + ret;
            return ret;
        }


        public static string RandomReplaceChineseWord(string str, string rep)
        {
            string retValue = string.Empty;
            var strsStrings = str.ToCharArray();
            int count = 0;
            for (int index = 0; index < strsStrings.Length; index++)
            {
                if (strsStrings[index] >= 0x4e00 && strsStrings[index] <= 0x9fa5)
                {
                    count++;
                }
            }
            if(count == 0)
            {
                return str;
            }
            int p = UnityEngine.Random.Range(1, count+1);
            count = 0;
            for (int index = 0; index < strsStrings.Length; index++)
            {
                if (strsStrings[index] >= 0x4e00 && strsStrings[index] <= 0x9fa5)
                {
                    count++;
                    if(count == p)
                    {
                        retValue += rep;
                    }
                    else
                    {
                        retValue += strsStrings[index];
                    }
                }
                else
                {
                    retValue += strsStrings[index];
                }
                
            }
            return retValue;
        }

        public static string StringInsertSeparator(string str, string sep)
        {
            string txt = "";
            string flag = "";
            int len = str.Length - 1;
            bool last = false;
            for (int i = 0; i < len; i++)
            {
                if (str[i] == '<')
                {
                    flag = "" + str[i];
                    for (int j = i + 1; j <= len; j++)
                    {
                        flag += str[j];
                        if (str[j] == '>')
                        {
                            i = j;
                            txt = txt + flag;
                            if (j == len)
                                last = true;
                            break;
                        }
                    }
                    continue;
                }
                txt = txt + str[i] + sep;
            }
            if (len >= 0 && !last)
            {
                txt = txt + str[len];
            }
            return txt;
        }

        public static int GetStringLenWithoutColor(string str)
        {
            string[] substr = str.Split(new string[] { "</color>" }, StringSplitOptions.RemoveEmptyEntries);
            string retstr = "";
            for (int i = 0; i < substr.Length; i ++)
            {
                int idx = substr[i].IndexOf("<color");
                if (idx != -1)
                {
                    int idx2 = substr[i].IndexOf(">", idx);
                    if (idx2 != -1)
                    {
                        string beg = substr[i].Substring(0, idx);
                        string end = substr[i].Substring(idx2 + 1);
                        retstr += beg + end;
                    }
                }
                else
                {
                    retstr += substr[i];
                }
            }
            return retstr.Length;
        }
        
        static bool IsChinese(string ch)
        {
            Regex regChina = new Regex("^[^\x00-\xFF]");
            if (regChina.IsMatch(ch))
                return true;
            return false;
        }

        static int _chCodeFrom = Convert.ToInt32("4e00", 16);
        static int _chCodeEnd = Convert.ToInt32("9fff", 16);
        static bool IsChinese(string chinese, int index)
        {
            int charcode = char.ConvertToUtf32(chinese, index);
            if (charcode >= _chCodeFrom && charcode <= _chCodeEnd)
            {
                return true;
            }
            return false;
        }

        static bool IsEnglish(string str, int index)
        {
            int charcode = char.ConvertToUtf32(str, index);
            if (charcode >= 0 && charcode <= 255)
            {
                return true;
            }
            return false;
        }

        // .Net库使用两个utf16来表示一个utf32，称为surrogate pair，在char.ConvertToUtf32之前先判断是否是surrogate pair，来防止报错
        // 可以使用 char.ConvertFromUtf32(Int32.Parse("2A601", System.Globalization.NumberStyles.HexNumber)) 来验证
        // 某些国家的稀奇字符都按中文宽度来处理
        public static int GetChineseStringLenWithoutColor(string str)
        {
            int ccnt = 0;
            int ecnt = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsLowSurrogate(str, i))
                {
                    ++ccnt;
                }
                else if (Char.IsHighSurrogate(str, i))
                {
                }
                else
                {
                    if (IsEnglish(str, i))
                        ++ecnt;
                    else
                        ++ccnt;
                }
            }
            return (int)Math.Floor(ccnt + ecnt * 0.5f);
        }

        public static float GetChineseStringWidth(string str, float fsize)
        {
            int ccnt = 0;
            int ecnt = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsLowSurrogate(str, i))
                {
                    ++ccnt;
                }
                else if (Char.IsHighSurrogate(str, i))
                {
                }
                else
                {
                    if (IsEnglish(str, i))
                        ++ecnt;
                    else
                        ++ccnt;
                }
            }
            //Debug.Log("--------------------------GetChineseStringWidth " + str + "," + ccnt + "," + ecnt + "," + fsize + "," + ((ccnt + ecnt * 0.5f) * fsize));
            return ((ccnt + ecnt * 0.5f) * fsize);
        }

        static double Multiply(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return ((p1.x - p3.x) * (p2.z - p3.z) - (p2.x - p3.x) * (p1.z - p3.z));
        }

        public static bool InRectangle(Vector3 pos, Vector3 centerPos, float width, float height, float angle)
        {
            Vector3 localpos = pos - centerPos;
            double theta = angle / 180.0 * Math.PI;
            float cos = (float)Math.Cos(theta);
            float sin = (float)Math.Sin(theta);
            float newx = localpos.x * cos - localpos.z*sin;
            float newy = localpos.x * sin + localpos.z * cos;
            return 2*Math.Abs(newx) <= width && 2*Math.Abs(newy) <= height;
        }

        private static Dictionary<int, bool> laststatus;
        public static bool InRectangleAndPaint(Vector3 pos, Vector3 centerPos, float width, float height, float angle, int sn)
        {
            if(laststatus==null)
            {
                laststatus = new Dictionary<int, bool>();
            }
            if(sn==0)
            {
                foreach (var element in laststatus)
                {
                    GameObject target = GameObject.Find("InRectangleAndPaint" + element.Value);
                    if (target != null)
                        GameObject.Destroy(target);
                }
                laststatus.Clear();
				return false;
            }
			//总是约定逆时针旋转
            angle = -angle;
            //这是一个测试方法，总是将此矩形在场景中绘制出来。
            //并且如果未命中为白色 命中了为红色。
            string name = "InRectangleAndPaint" + sn;
            GameObject g =  GameObject.Find(name);
            if (g != null)
                centerPos = g.transform.position;
            bool ret = InRectangle(pos, centerPos, width, height, angle);
            if (g==null)
            {
                g= GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.name = name;
                g.transform.position = centerPos;
                g.transform.localScale = new Vector3(width, 0.1f, height);
                g.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
                g.GetComponent<MeshRenderer>().material.color = ret ? Color.red : Color.white;
                laststatus[sn] = ret;
            }
            else
            {
                bool oldret;
                if(!laststatus.TryGetValue(sn, out oldret)||oldret !=ret)
                {
                    laststatus[sn] = ret;
                    g.GetComponent<MeshRenderer>().material.color = ret ? Color.red : Color.white;
                }
            }
           
            return ret;
        }

        public static void SetEasyTouchAvoidAllocUpdate( bool avoid)
        {
            ETCBase.AvoidAllocUpdate = avoid;
        }

        public static void Vibrate()
        {
#if (!UNITY_STANDALONE_WIN)
            Handheld.Vibrate();
#endif
        }

        public static Vector3 GetMoveDir(Camera cam, float degree, float shift = 0)
        {
            Matrix4x4 matDir = new Matrix4x4();
            matDir.SetTRS(Vector3.zero, Quaternion.Euler(0, degree + shift, 0), Vector3.one);
            Vector3 moveDir = matDir.MultiplyVector(cam.transform.forward);
            moveDir.y = 0;
            moveDir.Normalize();
            moveDir = moveDir * 2.5f;

            return moveDir;
        }

        public static void RotateAngle(GameObject obj, Camera cam, float degree)
        {
            Matrix4x4 matDir = new Matrix4x4();
            matDir.SetTRS(Vector3.zero, Quaternion.Euler(0, degree, 0), Vector3.one);
            Vector3 rockDir = matDir.MultiplyVector(cam.transform.forward);
            rockDir.y = 0;
            rockDir.Normalize();
            if (rockDir.magnitude < 0.001)
                return;
            obj.transform.rotation = Quaternion.LookRotation(rockDir);
        }

        // 获取cpu架构
        public static string GetSystemType()
        {
            string sys = System.Environment.OSVersion.Platform.ToString();
            if (sys == "Win32NT")
            {
                return "x86";
            }
            else
            {
                return "x64";
            }
        }

        // 获取网络状态 
        public static string GetInternetReachability()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
                return "当前网络：不可用";
            else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                return "4G";
            else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
                return "wifi";
            else
                return "当前网络：不可用";
        }


        // 获取物理mac地址
        public static string GetMacAddressByNetworkInformation()
        {
            string key = "SYSTEM\\CurrentControlSet\\Control\\Network\\{4D36E972-E325-11CE-BFC1-08002BE10318}\\";
            string macAddress = string.Empty;
            try
            {
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in nics)
                {
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                        && adapter.GetPhysicalAddress().ToString().Length != 0)
                    {
                        macAddress = adapter.GetPhysicalAddress().ToString();

                        //2019.05.30 刘北辰
                        //在mscorelib的4.0.0.0版本中，RegistryKey相关接口已经不存在，以下代码废止

                        //string fRegistryKey = key + adapter.Id + "\\Connection";
                        //RegistryKey rk = Registry.LocalMachine.OpenSubKey(fRegistryKey, false);
                        //if (rk != null)
                        //{
                        //    string fPnpInstanceID = rk.GetValue("PnpInstanceID", "").ToString();
                        //    int fMediaSubType = Convert.ToInt32(rk.GetValue("MediaSubType", 0));
                        //    if (fPnpInstanceID.Length > 3 &&
                        //        fPnpInstanceID.Substring(0, 3) == "PCI")
                        //    {
                        //        macAddress = adapter.GetPhysicalAddress().ToString();
                        //        for (int i = 1; i < 6; i++)
                        //        {
                        //            macAddress = macAddress.Insert(3 * i - 1, ":");
                        //        }
                        //        break;
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                //这里写异常的处理  
            }
            return macAddress;
        }

        //调用指定GameObject上的GraphicColorChanger的SetColorByIndex,用于改变指定Graphic的Color值
        //add by liujunjie in 2018/11/2
        public static void ChangeTargetColor(GameObject gameObject,int index)
        {
            var colorChanger = gameObject.GetComponent<GraphicColorChanger>();
            if(colorChanger != null)
            {
                colorChanger.SetColorByIndex(index);
            }
        }

        //一次设置某个GameObject及所有子节点的MaskableText的相关设置
        public static void SetTextMaskPosAndSize(GameObject gameObject,float centerY,float sizeY)
        {
            foreach(var maskableText in gameObject.GetComponentsInChildren<MaskableText>(true))
            {
                maskableText.SetNormalCenterY(centerY);
                maskableText.SetNormalSizeY(sizeY);
            }
        }

        //让某个GameObject上的Graphic忽略某些特定的UI raycast 事件,如果目标GameObject上没有挂载Graphic（接收 UI事件的基类)，本次调用会失败
        public static bool SetRayCastIgnoreRectTransform(GameObject gameObject,RectTransform ignoreTransform)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Graphic>() == null) return false;
            gameObject.GetOrAddComponent<ConditionalRayCastReciver>().SetIngoreRectTransform(ignoreTransform);
            return true;
        }
        public static bool AddRayCastIgnoreGraphic(GameObject gameObject,RectTransform ignoreGraphicRoot)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Graphic>() == null) return false;
            gameObject.GetOrAddComponent<ConditionalRayCastReciver>().AddIgnoreGraphicRoot(ignoreGraphicRoot);
            return true;
        }
        public static void RemoveCastIgnoreGraphic(GameObject gameObject,RectTransform ignoreGraphicRoot)
        {
            var reciver = gameObject.GetComponent<ConditionalRayCastReciver>();
            if (reciver != null) reciver.RemoveIgnoreGraphicRoot(ignoreGraphicRoot);
        }
        public static void ClearRayCastIgnoreGraphics(GameObject gameObject)
        {
            var reciver = gameObject.GetComponent<ConditionalRayCastReciver>();
            if (reciver != null) reciver.ClearIngoreGrphic();
        }
        public static void ClearAllIgnoreCondition(GameObject gameObject)
        {
            var reciver = gameObject.GetComponent<ConditionalRayCastReciver>();
            if (reciver != null) UnityEngine.Object.DestroyImmediate(reciver);
        }
        //给某个Graphic挂上UI事件代理，它会把收到的UI事件传递给目标UI
        public static void SetUIEventDelegate(GameObject gameObject,RectTransform target)
        {
            if (gameObject.GetComponent<UnityEngine.UI.Graphic>() == null) return;
            gameObject.GetOrAddComponent<UIEventDelegate>().SetDelegateTarget(target);
        }
        //设置所有sortingOrder在xxx - xxx之间(左右都不包含)的Canvas的RayCaster是否启用，用于新手引导
        private static Dictionary<BaseRaycaster, bool> originRaycasterStateDic = new Dictionary<BaseRaycaster, bool>();
        public static void SetRayCasterEnable(Transform startOrderObj, Transform endOrderObj,bool enable)
        {
            //Debug.LogError("try to set enable canvas raycaster");
            if (startOrderObj == null || endOrderObj == null || startOrderObj.GetComponentInParent<Canvas>() == null 
                || endOrderObj.GetComponentInParent<Canvas>() == null ) return;
            var startRootCanvas = startOrderObj.GetComponentInParent<Canvas>().rootCanvas;
            var endRootCanvas = endOrderObj.GetComponentInParent<Canvas>().rootCanvas;
            var startOrder = startRootCanvas.sortingOrder;
            var endOrder = endRootCanvas.sortingOrder;
            //Debug.LogError("startOrder :" + startOrder + " endOrder :" + endOrder);
            //只针对UIRoot之下的所有Canvas
            foreach(var canvas in UIRoot.Instance.GetComponentsInChildren<Canvas>())
            {
                if(canvas.transform.IsChildOf(startRootCanvas.transform) || canvas.transform.IsChildOf(endRootCanvas.transform))
                {
                    //出于某种目的 ， 有的界面会有多个不同sortingOrder的Canvas Raycaster (到底是什么目的我也不是很明白，但是就是有这样的)
                    //如果把子Canvas 的 Raycaster 给干掉，那么就会出现 界面点击无反应的情况 （目前出现这种情况的是 时装界面 UI_Fashion）
                    //UIChildMaker 也会要求在子界面上添加Raycaster (可能是为了有些时候屏蔽某些子界面的点击事件 ？)
                    continue;
                }
                //Debug.LogError("finding canvas ,its name is :" + canvas.name + " its sortingOrder is :" + canvas.sortingOrder);
                if(canvas.sortingOrder > startOrder && canvas.sortingOrder < endOrder)
                {
                    foreach(var raycaster in canvas.GetComponents<BaseRaycaster>())
                    {
                        //Debug.LogError("set enable success in :" + raycaster.name);
                        if(raycaster.enabled != enable)
                        {
                            //记录原来状态，用于下次复原(如果多次调用，之前保留的更改数据可能会被覆盖)
                            originRaycasterStateDic[raycaster] = raycaster.enabled;
                            raycaster.enabled = enable;
                        }
                    }
                }
            }
        }
        //回退最后一次对BaseRayCaster的更改
        public static void ResetRayCasterEnable()
        {
            foreach(var pair in originRaycasterStateDic)
            {
                if(pair.Key != null)
                {
                    pair.Key.enabled = pair.Value;
                }
            }
            originRaycasterStateDic.Clear();
        }

        public static string CompressString(string str)
        {
            try
            {
                byte[] strByte = Encoding.GetEncoding("us-ascii").GetBytes(str);
                MemoryStream ms = new MemoryStream();
                GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(strByte, 0, strByte.Length);
                zip.Close();
                byte[] buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                string cstr = Convert.ToBase64String(buffer);
                return cstr;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static string DecompressString(string str)
        {
            try
            {
                byte[] cstrByte = Convert.FromBase64String(str);
                MemoryStream ms = new MemoryStream(cstrByte);
                GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
                MemoryStream msreader = new MemoryStream();
                byte[] buffer = new byte[2048];
                while (true)
                {
                    int reader = zip.Read(buffer, 0, buffer.Length);
                    if (reader <= 0)
                    {
                        break;
                    }
                    msreader.Write(buffer, 0, reader);
                }
                msreader.Position = 0;
                buffer = msreader.ToArray();
                msreader.Close();
                zip.Close();
                ms.Close();
                string dstr = Encoding.GetEncoding("us-ascii").GetString(buffer);
                return dstr;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static void SetGOText(GameObject gameObject, string str)
        {
            UnityEngine.UI.Text o = gameObject.GetComponent<UnityEngine.UI.Text>();
            if (o != null)
                o.text = str;
        }

        public static void SetGOLocalPosX(GameObject gameObject, float x)
        {
            var lp = gameObject.transform.localPosition;
            lp.x = x;
            gameObject.transform.localPosition = lp;
        }

        public static void SetScrollRectEnable(GameObject gameObject, bool enable)
        {
            UnityEngine.UI.ScrollRect sr = gameObject.GetComponent<UnityEngine.UI.ScrollRect>();
            if (sr != null)
                sr.enabled = enable;
        }
        //让Unity Editor暂停 ， 仅在Editor模式下有用 ， 用于方便调试
        public static void PauseUnityEditor()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#endif
        }

        public static string GetCurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        public static void PlayCG(string fileName, Action<string, string> callback)
        {
            GameObject obj = GameObject.Find("CG");
            if (obj == null)
            {
                callback?.Invoke(fileName, "fail");
                return;
            }
            // CGManager mgr = obj.GetComponent<CGManager>();
            // if (mgr == null)
            // {
            //     callback?.Invoke(fileName, "fail");
            //     return;
            // }
            // mgr.PlayCG(fileName, callback);
        }

        // public static bool IsCGPlaying()
        // {
        //     GameObject obj = GameObject.Find("CG");
        //     if (obj == null)
        //     {
        //         return false;
        //     }
        //     CGManager mgr = obj.GetComponent<CGManager>();
        //     if (mgr == null)
        //     {
        //         return false;
        //     }
        //     return mgr.IsCGPlaying();
        // }

        public static bool GetSystemInfoBoolValue(string name)
        {
            Type t = typeof(SystemInfo);
            PropertyInfo info = t.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            if(info == null)
            {
                return false;
            }
            object val = info.GetValue(null, null);
            if (val == null)
            {
                return false;
            }
            return (bool)val;
        }
        public static int GetSystemInfoIntValue(string name)
        {
            Type t = typeof(SystemInfo);
            PropertyInfo info = t.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            if (info == null)
            {
                return -1;
            }
            object val = info.GetValue(null, null);
            if(val == null)
            {
                return -1;
            }
            return (int)val;
        }
        public static string GetSystemInfoStringValue(string name)
        {
            Type t = typeof(SystemInfo);
            PropertyInfo info = t.GetProperty(name, BindingFlags.Public | BindingFlags.Static);
            if (info == null)
            {
                return "";
            }
            object val = info.GetValue(null, null);
            if (val == null)
            {
                return "";
            }
            return val.ToString();
        }
    }
}
