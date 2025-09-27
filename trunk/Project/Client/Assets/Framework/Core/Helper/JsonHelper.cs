using System;
using System.IO;
using Newtonsoft.Json;

namespace Framework.Core
{
    public static class JsonHelper
    {
        #region Json存储本地

        public static void Save(object obj, string path, Action<bool> callBack = null)
        {
            string jsonStr = JsonConvert.SerializeObject(obj, Formatting.Indented);
            WriteFile(path, jsonStr, callBack);
        }
        
        private static void WriteFile(string path, string value, Action<bool> callBack)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.Write(value);
                    sw.Close();
                    callBack?.Invoke(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                callBack?.Invoke(false);
                throw;
            }
            
           
        }

        #endregion

        #region Json本地加载

        public static T LoadSync<T>(string path) where T : new()
        {
            string jsonStr = ReadFileSync(path);
            if (string.IsNullOrEmpty(jsonStr)) return new T();
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }

        private static string ReadFileSync(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs);
                string content = sr.ReadToEnd();
                sr.Close();
                return content;
            }
        }

        public static void LoadAsync<T>(string path, Action<T> callBackResult) where T : new()
        {
            ReadFileAsync(path, jsonStr =>
            {
                if (string.IsNullOrEmpty(jsonStr)) callBackResult?.Invoke(new T());
                callBackResult?.Invoke(JsonConvert.DeserializeObject<T>(jsonStr));
            });
        }

        private static void ReadFileAsync(string path, Action<string> callBackResult)
        {
            if (string.IsNullOrEmpty(path)) callBackResult?.Invoke(null);
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                StreamReader sr = new StreamReader(fs);
                var task = sr.ReadToEndAsync();
                if (task.IsCompleted)
                {
                    sr.Close();
                    callBackResult?.Invoke(task.Result);
                }
            }
        }
        
        #endregion

        #region Convert 格式化

        public static T Convert<T>(string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string Convert<T>(this T t)
        {
            try
            {
                return JsonConvert.SerializeObject(t);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
    }
}