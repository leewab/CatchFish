using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Game.Core
{
    public static class MD5Helper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.Default.GetBytes(value);
                byte[] nBytes = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nBytes.Length; i++)
                {
                    sb.Append(nBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Encrypt(string value, out int size)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = Encoding.Default.GetBytes(value);
                byte[] nBytes = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nBytes.Length; i++)
                {
                    sb.Append(nBytes[i].ToString("x2"));
                }

                size = bytes.Length / 1024;
                return sb.ToString();
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Encrypt(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string Encrypt(Stream stream, out int size)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes = md5.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }

                size = bytes.Length / 1024;
                return sb.ToString();
            }
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EncryptFile(string path)
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    string result = Encrypt(fs);
                    fs.Close();
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// 加密文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EncryptFile(string path, out float size)
        {
            LogHelper.LogBlue(path);
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    size = (float)fs.Length / 1024; //k
                    string result = Encrypt(fs);
                    fs.Close();
                    return result;
                }
            }

            size = 0;
            return null;
        }
    }
}