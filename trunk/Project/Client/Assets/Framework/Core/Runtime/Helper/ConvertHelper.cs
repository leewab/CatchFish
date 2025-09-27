using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace Game.Core
{
    public class ConvertHelper
    {
        /// <summary>
        /// Byte to object
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object Byte2Object(byte[] bytes)
        {
            object obj;
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                IFormatter iformatter = new BinaryFormatter();
                obj = iformatter.Deserialize(memoryStream);
            }

            return obj;
        }

        /// <summary>
        /// object to byte
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Object2Byte(object obj)
        {
            byte[] bytes;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                bytes = memoryStream.GetBuffer();
            }

            return bytes;
        }

        public static Int32 String2Int(string _value)
        {
            try
            {
                return Int32.Parse(_value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static float String2Float(string _value)
        {
            try
            {
                return Int64.Parse(_value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static byte[] String2Byte(string _value)
        {
            return Encoding.Default.GetBytes(_value);
        }

        public static string Byte2String(byte[] _value)
        {
            return Encoding.Default.GetString(_value);
        }

        public static byte[] String2ASCIIByte(string _value)
        {
            return Encoding.ASCII.GetBytes(_value);
        }

        public static string ASSCIIByte2String(byte[] _value)
        {
            return Encoding.ASCII.GetString(_value);
        }

        public static string GetFileMD5(string filePath)
        {
            byte[] datas = IOHelper.LoadFileByte(filePath);
            return GetByteMD5(new MD5CryptoServiceProvider().ComputeHash(datas));
        }

        public static string GetStrMD5(string content)
        {
            return GetByteMD5(String2Byte(content));
        }

        public static string GetByteMD5(byte[] datas)
        {
            return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(datas));
        }
    }
}