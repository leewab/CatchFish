using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace MUGUI
{
    public class TweenGroup
    {
        List<UITweener> tweens = new List<UITweener>();
        public List<UITweener> Tweens { get { return tweens; } }

        public void ResetAndPlayForward()
        {
            foreach (var i in tweens)
            {
                i.ResetToBeginning();
                i.PlayForward();
            }
        }

        public void Stop()
        {
            foreach (var i in tweens)
            {
                i.enabled = false;
            }
        }
    }
    public static class GOGUITools
    {
        public static System.Action<string, System.Action<string, Object>, MUEngine.LoadPriority, MUEngine.ECacheType> GetAssetAction;
        public static System.Action<string, Object> ReleaseAssetAction;
        public static Camera UICamera;

        public static Dictionary<int, TweenGroup> GetTweenGroup(this GameObject go, System.Action<int, UITweener> onProcessTweener = null)
        {
            Dictionary<int, TweenGroup> res = new Dictionary<int, TweenGroup>();
            UITweener[] t = go.GetComponents<UITweener>();
            foreach (var i in t)
            {
                int group = i.tweenGroup;
                TweenGroup grp;
                if (!res.TryGetValue(group, out grp))
                {
                    grp = new TweenGroup();
                    res[group] = grp;
                }
                if (onProcessTweener != null)
                    onProcessTweener(group, i);
                grp.Tweens.Add(i);
            }

            return res;
        }
        
        public static GameObject AddChild(GameObject parent, GameObject template)
        {
            GameObject go = GameObject.Instantiate(template);
            Vector3 oldScale = go.transform.localScale;
            go.transform.SetParent(parent.transform);
            go.transform.localScale = oldScale;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }
        /// <summary>
        ///  Class + Function 转换为 Class.Function.
        /// </summary>
        static public string GetFuncName(object obj, string method)
        {
            if (obj == null) return "<null>";
            string type = obj.GetType().ToString();
            int period = type.LastIndexOf('/');
            if (period > 0) type = type.Substring(period + 1);
            return string.IsNullOrEmpty(method) ? type : type + "/" + method;
        }

        /// <summary>
        /// 添加缺少的组件
        /// </summary>
        static public T AddMissingComponent<T>(this GameObject go) where T : Component
        {
#if UNITY_FLASH
        object comp = go.GetComponent<T>();
#else
            T comp = go.GetComponent<T>();
#endif
            if (comp == null)
            {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                RegisterUndo(go, "Add " + typeof(T));
#endif
                comp = go.AddComponent<T>();
            }
#if UNITY_FLASH
        return (T)comp;
#else
            return comp;
#endif
        }

        static public void RegisterUndo(UnityEngine.Object obj, string name)
        {
#if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(obj, name);
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

        static public void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
        }

#if UNITY_EDITOR
		//Debug用代码，打印fileMap
		private static void PrintFileMap(Dictionary<string,string> fileMap){
			string fileName = "fileMap.txt";
			string result = "";
			foreach (var pair in fileMap) {
				result += pair.Key + " " + pair.Value + "\r\n";
			}
			FileStream fs = null;
			try{
				fs = new FileStream(Path.Combine(Application.dataPath,fileName),FileMode.Create);
				byte[] data = System.Text.Encoding.UTF8.GetBytes(result);
				fs.Write(data,0,data.Length);
				fs.Close();
				Debug.Log("<color=#00FF00> print fileMap success ! see fileMap.txt for detail </color>");
			}catch(System.Exception e){
				Debug.LogError (e);
				if (fs != null)
					fs.Close ();
			}
		}
#endif

    }
}