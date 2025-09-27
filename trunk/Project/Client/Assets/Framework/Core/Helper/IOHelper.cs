using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    public static class IOHepler
    {
        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="extension"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, string extension = "*")
        {
            Debug.LogError(sourceDirName);
            Debug.LogError(destDirName);
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            foreach (string folderPath in Directory.GetDirectories(sourceDirName, "*", SearchOption.AllDirectories))
            {
                if (!Directory.Exists(folderPath.Replace(sourceDirName, destDirName)))
                    Directory.CreateDirectory(folderPath.Replace(sourceDirName, destDirName));
            }

            foreach (string filePath in Directory.GetFiles(sourceDirName, $"*.{extension}", SearchOption.AllDirectories)
            )
            {
                var fileDirName = Path.GetDirectoryName(filePath)?.Replace("\\", "/");
                var fileName = Path.GetFileName(filePath);
                if (fileDirName != null)
                {
                    string newFilePath = Path.Combine(fileDirName.Replace(sourceDirName, destDirName), fileName);

                    File.Copy(filePath, newFilePath, true);
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="sourceDirPath"></param>
        /// <param name="destDirPath"></param>
        public static void MoveDirectory(string sourceDirPath, string destDirPath)
        {
            Debug.LogError(sourceDirPath);
            Debug.LogError(destDirPath);
            Directory.Move(sourceDirPath, destDirPath);
            AssetDatabase.Refresh();
        }

        public static void CreateDirectory(string _dirPath)
        {
            if (string.IsNullOrEmpty(_dirPath)) return;
            if (!DirExists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
        }

        public static bool DirExists(string _dirPath)
        {
            if (string.IsNullOrEmpty(_dirPath)) return false;
            return Directory.Exists(_dirPath);
        }

        public static void CreateFile(string _filePath, byte[] bytes)
        {
            if (string.IsNullOrEmpty(_filePath)) return;
            if (File.Exists(_filePath)) File.Delete(_filePath);
            using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length - 1);
                fs.Close();
            }
        }

        public static bool FileExists(string _filePath)
        {
            return File.Exists(_filePath);
        }


        public static void DeleteFile(string _filePath)
        {
            if (_filePath == null) return;
            Debug.Log("Delete //" + _filePath);
            if (File.Exists(_filePath)) File.Delete(_filePath);
            if (File.Exists(_filePath + ".meta")) File.Delete(_filePath + ".meta");
        }

        public static void DeleteDir(string _dirPath)
        {
            if (_dirPath == null) return;
            Debug.Log("DeleteDirectory//" + _dirPath);
            if (Directory.Exists(_dirPath))
            {
                Directory.Delete(_dirPath);
            }
        }

        public static byte[] GetFileByte(string _filePath)
        {
            FileStream fs = new FileStream(_filePath, FileMode.Open);
            int len = (int) fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            return data;
        }
    }
}