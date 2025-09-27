using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Core
{
    public class AssemblyHelper
    {
        /// <summary>
        /// 编辑器默认的程序集Assembly-CSharp.dll
        /// </summary>
        private static Assembly defaultCSharpAssembly;

        /// <summary>
        /// 程序集缓存
        /// </summary>
        private static Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();

        /// <summary>
        /// 获取编辑器默认的程序集Assembly-CSharp.dll
        /// </summary>
        public static Assembly DefaultCSharpAssembly
        {
            get
            {
                //如果已经找到，直接返回
                if (defaultCSharpAssembly != null)
                    return defaultCSharpAssembly;

                //从当前加载的程序包中寻找，如果找到，则直接记录并返回
                var assems = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assem in assems)
                {
                    //所有本地代码都编译到Assembly-CSharp中
                    if (assem.GetName().Name == "Assembly-CSharp")
                    {
                        //保存到列表并返回
                        defaultCSharpAssembly = assem;
                        break;
                    }
                }
                return defaultCSharpAssembly;
            }
        }

        /// <summary>
        /// 获取Assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string name)
        {
#if UNITY_ANDROID
            if (!assemblyCache.ContainsKey(name))
                return null;

            return assemblyCache[name];
#else
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                
                if (assembly.GetName().Name == name)
                {
                    return assembly;
                }
            }
            return null;
#endif
        }


        /// <summary>
        /// 根据名称获取程序集中的类型
        /// </summary>
        /// <param name="assembly">程序集名称，例如：SSJJ，Start</param>
        /// <param name="typeName">类型名称，必须包含完整的命名空间，例如：SSJJ.Render.BulletRail</param>
        /// <returns>类型</returns>
        public static Type GetAssemblyType(string assembly, string typeName)
        {
            Type t;

            if (Application.platform == RuntimePlatform.Android || Application.isEditor)
                t = AssemblyHelper.GetAssembly(assembly).GetType(typeName);
            //其他平台使用默认程序集中的类型
            else
                t = AssemblyHelper.DefaultCSharpAssembly.GetType(typeName);

            return t;
        }

        /// <summary>
        /// 通过类的完整名称来获得
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <returns></returns>
        public static Type GetAssemblyType(string typeFullName)
        {
            var pointPos = typeFullName.LastIndexOf(".", StringComparison.Ordinal);
            string assemblyName = null;
            string typeName = null;
            if (pointPos > 0)
            {
                assemblyName = typeFullName.Substring(0, pointPos);
                typeName = typeFullName.Substring(pointPos);
            }
            else
            {
                typeName = typeFullName;
            }

            var orgType = assemblyName == null
                ? GetDefaultAssemblyType(typeName)
                : GetAssemblyType(assemblyName, typeName);

            return orgType;
        }


        /// <summary>
        /// 获取默认的程序集
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetDefaultAssemblyType(string typeName)
        {
            var t = AssemblyHelper.DefaultCSharpAssembly.GetType(typeName);
            return t;
        }


        public static Type[] GetTypeList(string assemblyName)
        {
            return GetAssembly(assemblyName).GetTypes();
        }
        
        /// <summary>
        /// 通过Interface获取其所有的派生类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Type[] GetDerivedClassesWithInterface<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(T))))
                    .Where(t => !t.IsAbstract && t.IsClass)              //获取非抽象类 排除接口继承
                    .ToArray();
        }
        
        public static IEnumerable<T> GetDerivedClassInstancesWithInterface<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes()
                    .Where(t => t.GetInterfaces().Contains(typeof(T))))
                    .Where(t => !t.IsAbstract && t.IsClass)              //获取非抽象类 排除接口继承
                    .Select(t => (T) Activator.CreateInstance(t))
                    .ToArray();
        }

    }
    
    public static class ReflectionUtil
    {
        public static Assembly GetAssemblyCSharp()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
            return null;
        }
        
        public static Assembly GetAssemblyCSharpEditor()
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var a in assemblies)
            {
                if (a.FullName.StartsWith("Assembly-CSharp-Editor,"))
                {
                    return a;
                }
            }

//            Log.E(">>>>>>>Error: Can\'t find Assembly-CSharp-Editor.dll");
            return null;
        }
        
        
        /// <summary>
        /// 通过反射方式调用函数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object InvokeByReflect(this object obj, string methodName, params object[] args)
        {
            var methodInfo = obj.GetType().GetMethod(methodName);
            if (methodInfo == null) return null;
            return methodInfo.Invoke(obj, args);
        }

        /// <summary>
        /// 通过反射方式获取域值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">域名</param>
        /// <returns></returns>
        public static object GetFieldByReflect(this object obj, string fieldName)
        {
            var fieldInfo = obj.GetType().GetField(fieldName);
            if (fieldInfo == null) return null;
            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 通过反射方式获取属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(obj, index);
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit)
        {
            return prop.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
        {
            return field.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
        {
            return type.GetCustomAttributes(attributeType, inherit).Length > 0;
        }

        /// <summary>
        /// 拥有特性
        /// </summary>
        /// <returns></returns>
        public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit)
        {
            return method.GetCustomAttributes(attributeType, inherit).Length > 0;
        }


        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T: Attribute
        {
            var attrs = (T[])method.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
        {
            var attrs = (T[])field.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
        {
            var attrs = (T[])prop.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }

        /// <summary>
        /// 获取第一个特性
        /// </summary>
        public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var attrs = (T[])type.GetCustomAttributes(typeof(T), inherit);
            if (attrs != null && attrs.Length > 0)
                return attrs[0];
            return null;
        }
        
        /// <summary>
        /// 获取字段
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FieldInfo[] GetFields<T>()
        {
            return typeof(T).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// 获取字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PropertyInfo[] GetProperties<T>()
        {
            return typeof(T).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
        }
        
        /// <summary>
        /// 获取类中的所有字段
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isIgnoreEmptyValue"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValue> GetFieldsDeepness<T>(this T t)
        {
            if (t == null) return null;
            System.Reflection.FieldInfo[] fieldInfos = t.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fieldInfos.Length <= 0) return null;
            List<KeyValue> fieldsList = new List<KeyValue>();
            foreach (System.Reflection.FieldInfo fieldInfo in fieldInfos)
            {
                string name = fieldInfo.Name;           //名称
                object value = fieldInfo.GetValue(t);   //值
                if (fieldInfo.FieldType.IsPrimitive || fieldInfo.FieldType.IsValueType || fieldInfo.FieldType == typeof(string))
                {
                    fieldsList.Add(new KeyValue(name, value));
                }
                else if (fieldInfo.FieldType.IsClass && !typeof(IEnumerable).IsAssignableFrom(fieldInfo.FieldType))
                {
                    fieldsList.AddRange(GetFieldsDeepness(value));
                }
                else
                {
                    var enumerableObj = value as IEnumerable;
                    if (enumerableObj == null) continue;
                    var objList = enumerableObj.GetEnumerator();
                    while (objList.MoveNext())
                    {
                        objList.MoveNext();
                        fieldsList.AddRange(GetFieldsDeepness(objList.Current));
                    }
                }
            }

            return fieldsList;
        }

        /// <summary>
        /// 深度获取类中的属性键值
        /// </summary>
        /// <param name="t"></param>
        /// <param name="isIgnoreEmptyValue">是否忽略空值</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValue> GetPropertiesDeepness<T>(this T t)
        {
            if (t == null) return null;
            System.Reflection.PropertyInfo[] propertyInfos = t.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfos.Length <= 0) return null;
            List<KeyValue> propertiesList = new List<KeyValue>();
            foreach (var propertyInfo in propertyInfos)
            {
                string name = propertyInfo.Name;           //名称
                object value = propertyInfo.GetValue(t);   //值
                if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    propertiesList.Add(new KeyValue(name, value));
                }
                else if (propertyInfo.PropertyType.IsClass || !typeof(IEnumerator).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    propertiesList.AddRange(GetPropertiesDeepness(value));
                }
                else
                {
                    var enumerableObj = value as IEnumerable;
                    if (enumerableObj == null) continue;
                    var objList = enumerableObj.GetEnumerator();
                    while (objList.MoveNext())
                    {
                        objList.MoveNext();
                        propertiesList.AddRange(GetPropertiesDeepness(objList.Current));
                    }
                }
            }

            return propertiesList;
        }
    }
}