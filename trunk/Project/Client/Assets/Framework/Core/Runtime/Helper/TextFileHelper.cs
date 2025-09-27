using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.Core
{
    public static class TextFileHelper
    {
        /// <summary>
        /// 以键值的方式获取对象中的属性字段
        /// </summary>
        /// <param name="t"></param>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        public static void CreateTextFile<T>(T t, string path)
        {
            List<KeyValue> infos = new List<KeyValue>();
            var fieldInfos = t.GetFieldsDeepness();
            var propertiesInfos = t.GetPropertiesDeepness();
            if (fieldInfos != null) infos.AddRange(fieldInfos);
            if (propertiesInfos != null) infos.AddRange(propertiesInfos);
            StringBuilder fileContent = new StringBuilder($"******{typeof(T).Name}******");
            foreach (var info in infos)
            {
                fileContent.Append("\n");
                fileContent.Append($"{info.Key}={info.Value}");
            }
            Debug.Log(fileContent);
            IOHelper.CreateFile(path, fileContent.ToString());
        }

        /// <summary>
        /// 加载全文本
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string LoadTextFile(string path)
        {
            return IOHelper.LoadFileString(path);
        }
    }
}