using System.IO;

namespace Game.Core
{
    public class IOUtil
    {
        public static void WriteTextToFile(string filePath, string content)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            //byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
            //FileStream fileStream = new FileStream(filePath, FileMode.Create);
            //fileStream.Write(bytes, 0, bytes.Length);
            //fileStream.Close();
            File.WriteAllText(filePath, content);
        }

        public static string ReadTextFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
                //FileStream fs = new FileStream(filePath, FileMode.Open);//初始化文件流
                //byte[] array = new byte[fs.Length];//初始化字节数组
                //fs.Read(array, 0, array.Length);//读取流中数据到字节数组中
                //fs.Close();//关闭流
                //string str = System.Text.Encoding.Default.GetString(array);//将字节数组转化为字符串
                //return str;
            }

            return "";
        }

    }
}