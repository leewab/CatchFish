using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using Object = Object;

    public static class UnityUtil
    {
        /// <summary>
        /// 添加组件 相同的仅一个
        /// </summary>
        public static T AddOneComponent<T>(this Transform go) where T : Component
        {
            return AddOneComponent<T>(go.gameObject);
        }

        /// <summary>
        /// 添加Component组件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static Component AddOneComponent(this GameObject go, Type componentType)
        {
            if (go != null)
            {
                Component ts = go.GetComponent(componentType);
                if (ts == null) ts = go.AddComponent(componentType);
                return ts;
            }

            return null;
        }

        /// <summary>
        /// 添加组件 相同的仅一个
        /// </summary>
        public static T AddOneComponent<T>(this GameObject go) where T : Component
        {
            if (go != null)
            {
                T[] ts = go.GetComponents<T>();
                if (ts.Length == 1)
                {
                    return ts[0];
                }
                else if (ts.Length > 1)
                {
                    for (int i = 1; i < ts.Length; i++)
                    {
                        if (ts[i] != null) Object.Destroy(ts[i]);
                    }

                    return ts[0];
                }
                else
                {
                    return go.gameObject.AddComponent<T>();
                }
            }

            return null;
        }

        /// <summary>
        /// 移除该所有组建
        /// </summary>
        /// <param name="go"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveComponent<T>(this GameObject go) where T : Component
        {
            if (go != null)
            {
                T[] ts = go.GetComponents<T>();
                if (ts.Length > 1)
                {
                    for (int i = 1; i < ts.Length; i++)
                    {
                        if (ts[i] != null) Object.Destroy(ts[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 是否含有组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="isIncludeSub">是否包含子物体</param>
        /// <returns></returns>
        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            if (go == null) return false;
            T[] ts = go.GetComponents<T>();
            return ts.Length > 0;
        }

        /// <summary>
        /// 创建GameObject对象
        /// </summary>
        /// <param name="name"> 创建对象的名称 </param>
        /// <param name="parent"> 创建对象所属父物体 </param>
        /// <param name="layer"> 创建对象所在层级 </param>
        /// <param name="components"> 创建对象上添加的Component组件 </param>
        /// <returns></returns>
        public static GameObject CreateObject(string name, Transform parent = null, int layer = 0,
            params Type[] components)
        {
            var go = new GameObject(name, components) { layer = layer };
            if (parent) go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }

        /// <summary>
        /// CreateUIComponent
        /// </summary>
        /// <param name="name"> 对象名称 </param>
        /// <param name="parent"> 对象所在父物体 </param>
        /// <param name="layer"> 对象所在层级 默认为UI层 5 </param>
        /// <param name="components"> 对象默认所带有的组件 </param>
        /// <typeparam name="T"> 返回添加的组件 </typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(string name, Transform parent = null, int layer = 0, params Type[] components)
            where T : Component
        {
            var go = CreateObject(name, parent, layer, components);
            return null == go ? null : go.AddOneComponent<T>();
        }

        /// <summary>
        /// 创建UI对象
        /// </summary>
        /// <param name="name"> 对象名称 </param>
        /// <param name="parent"> 对象所在父物体 </param>
        /// <param name="layer"> 对象所在层级 默认为UI层 5 </param>
        /// <param name="components"> 对象默认所带有的组件 </param>
        /// <returns></returns>
        public static RectTransform CreateUIObject(string name, Transform parent = null, int layer = 5,
            params Type[] components)
        {
            var go = CreateObject(name, parent, layer, components);
            return null == go ? null : go.AddComponent<RectTransform>();
        }

        /// <summary>
        /// CreateUIComponent
        /// </summary>
        /// <param name="name"> 对象名称 </param>
        /// <param name="parent"> 对象所在父物体 </param>
        /// <param name="layer"> 对象所在层级 默认为UI层 5 </param>
        /// <param name="components"> 对象默认所带有的组件 </param>
        /// <typeparam name="T"> 返回添加的组件 </typeparam>
        /// <returns></returns>
        public static T CreateUIObject<T>(string name, Transform parent = null, int layer = 5, params Type[] components)
            where T : Component
        {
            var go = CreateObject(name, parent, layer, components);
            if (null == go) return null;
            go.AddOneComponent<RectTransform>();
            return go.AddOneComponent<T>();
        }

        /// <summary>
        /// 设置锚点为中心点
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SetAnchorsPos<T>(this T t) where T : Transform
        {
            if (t is RectTransform)
            {
                RectTransform rectTrans = t as RectTransform;
                rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
            }
            else
            {
                throw new InvalidOperationException("SetAnchorsCenter 只有对RectTransform 组件有用！");
            }
        }

        /// <summary>
        /// 设置锚点为平铺填满
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        public static void SetAnchorsStretch<T>(this T t) where T : Transform
        {
            if (t is RectTransform)
            {
                RectTransform rectTrans = t as RectTransform;
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.sizeDelta = Vector2.zero;
            }
            else
            {
                throw new InvalidOperationException("SetAnchorsStretch 只有对RectTransform 组件有用！");
            }
        }

        // TODO: 1.这里试想扩展一个新思路，之前有人写过使用缓存池记录查找，但是对于UI的查找，只需要在OnInit也就是界面首次加载的时候才需要查找并绑定
        // TODO: 2.而且如果程序选择使用自动释放UI对象，在重新加载UI的情况下，部分UI重复加载的次数概率很小，如果这里做了缓存，只会增加对象池的内存常驻问题，
        // TODO: 3.所以这里扩展设计，可以使用枚举记录的方式，记录释放之后需要多次重复加载的UI，对于这些UI采用缓存记录的方式
        // TODO: 4.缓存记录，可以在测试阶段开启加载记录收集，然后统一整理该记录，并同步更新枚举记录文件
        public static GameObject GetGameObjectByID(this GameObject root, string id)
        {
            if (string.IsNullOrEmpty(id) || root == null || root.name == id)
            {
                return root;
            }

            //不再允许任何情况下的模糊查找，也不允许使用:进行查找，所以这种方式进行查找都将会直接抛出异常
            Transform realTrans = root.transform.Find(id);
            return realTrans == null ? null : realTrans.gameObject;
        }

        private static string GetTransformPath(Transform root, Transform transform)
        {
            if (transform == null)
            {
                return "";
            }

            StringBuilder sb = new StringBuilder();
            while (transform != null && transform != root)
            {
                sb.Insert(0, "/");
                sb.Insert(0, transform.name);
                transform = transform.parent;
            }

            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        private static bool CheckPathMatch(Transform t, string path)
        {
            List<string> idList = new List<string>(path.Split(':'));
            idList.Reverse();
            int len = idList.Count;
            Transform tmpTran = t;
            for (int i = 0; i < len; i++)
            {
                if (tmpTran.gameObject.name != idList[i])
                    return false;
                tmpTran = tmpTran.parent;
            }

            return true;
        }

        /// <summary>
        /// 查找子物体
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"> 子物体名称 </param>
        /// <returns></returns>
        public static Transform FindChild(this GameObject go, string name)
        {
            return null == go ? null : FindChild(go.transform, name);
        }

        /// <summary>
        /// 查找子物体
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="name"> 子物体名称 </param>
        /// <returns></returns>
        public static Transform FindChild(this Transform tf, string name)
        {
            if (null == tf) return null;
            var ret = tf.Find(name);
            if (null != ret)
            {
                return ret;
            }
            else
            {
                for (int i = 0; i < tf.childCount; i++)
                {
                    ret = FindChild(tf.GetChild(i), name);
                    if (null != ret)
                    {
                        return ret;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 查找父物体
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"> 父物体名称 </param>
        /// <returns></returns>
        public static Transform FindParent(this GameObject go, string name)
        {
            return null == go ? null : FindParent(go.transform, name);
        }

        /// <summary>
        /// 查找父物体
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="name"> 父物体名称 </param>
        /// <returns></returns>
        public static Transform FindParent(this Transform tf, string name)
        {
            if (null == tf) return null;
            var ret = tf.parent;
            if (null == ret) return null;
            return ret.name.Equals(name) ? ret : FindParent(ret, name);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(this GameObject go, string name)
        {
            return null == go ? null : Peer(go.transform, name);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(this Transform go, string name)
        {
            Transform tran = go.parent.Find(name);
            return tran == null ? null : tran.gameObject;
        }

        /// <summary>
        /// 清除所有子对象
        /// </summary>
        public static void DestroyChild(this Transform tf)
        {
            if (tf == null) return;
            for (int i = tf.childCount - 1; i >= 0; i--)
            {
                var childObj = tf.GetChild(i);
                if (null != childObj)
                {
                    Object.Destroy(childObj.gameObject);
                }
            }
        }

        /// <summary>
        /// 删除某一个子对象
        /// </summary>
        /// <param name="go"></param>
        /// <param name="name"></param>
        public static void DestroyChild(this Transform tf, string name)
        {
            if (tf == null || string.IsNullOrEmpty(name)) return;
            var childObj = FindChild(tf, name);
            if (null != childObj) Object.Destroy(childObj.gameObject);
        }


        /// <summary>
        /// 高效的设置显隐
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isShow"></param>
        public static void SetActiveSpare(this GameObject obj, bool isShow)
        {
            if (null == obj) return;
            if (obj.activeSelf != isShow)
            {
                obj.SetActive(isShow);
            }
        }


        /// <summary>
        /// 获取UI的屏幕坐标【0,1】
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static Vector2 GetUIScreenPosition(Graphic ui)
        {
            //获取到UI所处的canvas
            Canvas canvas = ui.GetComponentInParent<Canvas>();

            //Overlay模式 或者 ScreenSpaceCamera模式没有关联UI相机的情况
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            {
                float x = ui.transform.position.x / Screen.width;
                float y = ui.transform.position.y / Screen.height;
                return new Vector2(x, y);
            }
            //ScreenSpaceCamera 和 WorldSpace模式  注意WorldSpace没有关联UI相机获取到的就会有问题
            else
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, ui.transform.position);
                float x = screenPos.x / Screen.width;
                float y = screenPos.y / Screen.height;
                return new Vector2(x, y);
            }
        }
        
        /// <summary>
        /// 获取UI的屏幕坐标【0,1】
        /// </summary>
        /// <param name="ui"></param>
        /// <returns></returns>
        public static Vector2 GetUIScreenPosition(RectTransform ui)
        {
            //获取到UI所处的canvas
            Canvas canvas = ui.GetComponentInParent<Canvas>();
            
            //Overlay模式 或者 ScreenSpaceCamera模式没有关联UI相机的情况
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay ||
                canvas.renderMode == RenderMode.ScreenSpaceCamera && canvas.worldCamera == null)
            {
                float x = ui.transform.position.x / Screen.width;
                float y = ui.transform.position.y / Screen.height;
                return new Vector2(x, y);
            }
            //ScreenSpaceCamera 和 WorldSpace模式  注意WorldSpace没有关联UI相机获取到的就会有问题
            else
            {
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, ui.transform.position);
                float x = screenPos.x / Screen.width;
                float y = screenPos.y / Screen.height;
                return new Vector2(x, y);
            }
        }

        public static void ScreenSizeChange()
        {
#if UNITY_TOLUA
            LuaModule.Instance.ScreenSizeChange();
#else
            //TODO: 这里需要通过C#实现Screen Size Change
#endif
        }

    }
}