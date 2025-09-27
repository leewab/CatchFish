using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public static class IOHepler
    {
        /// <summary>
        /// 文件夹及其子文件夹整体拷贝
        /// </summary>
        /// <param name="sourceDirName"></param>
        /// <param name="destDirName"></param>
        /// <param name="extension"></param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, string extension = "*")
        {
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

            foreach (string filePath in Directory.GetFiles(sourceDirName, $"*.{extension}", SearchOption.AllDirectories))
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
        public static void DirectoryMove(string sourceDirPath, string destDirPath)
        {
            Debug.LogError(sourceDirPath);
            Debug.LogError(destDirPath);
            Directory.Move(sourceDirPath, destDirPath);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 文件夹创建
        /// </summary>
        /// <param name="_dirPath"></param>
        public static void DirectoryCreate(string _dirPath)
        {
            if (string.IsNullOrEmpty(_dirPath)) return;
            if (!DirectoryExists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
        }
        
        /// <summary>
        /// 文件夹删除
        /// </summary>
        /// <param name="_dirPath"></param>
        public static void DirectoryDelete(string _dirPath)
        {
            if (_dirPath == null) return;
            if (Directory.Exists(_dirPath))
            {
                Directory.Delete(_dirPath);
            }
        }

        /// <summary>
        /// 文件夹判定
        /// </summary>
        /// <param name="_dirPath"></param>
        /// <returns></returns>
        public static bool DirectoryExists(string _dirPath)
        {
            if (string.IsNullOrEmpty(_dirPath)) return false;
            return Directory.Exists(_dirPath);
        }

        /// <summary>
        /// 文件拷贝
        /// </summary>
        /// <param name="resourceFile">源文件</param>
        /// <param name="destDirPath">拷贝路径</param>
        /// <returns></returns>
        public static void FileCopy(string resourceFile, string destDirPath)
        {
            if (File.Exists(resourceFile))
            {
                File.Copy(resourceFile, destDirPath, true);
            }
        }

        /// <summary>
        /// 文件创建
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="bytes"></param>
        public static void FileCreate(string _filePath, byte[] bytes)
        {
            if (string.IsNullOrEmpty(_filePath)) return;
            if (File.Exists(_filePath)) File.Delete(_filePath);
            using (var fs = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length - 1);
                fs.Close();
            }
        }

        /// <summary>
        /// 文件判定
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        public static bool FileExists(string _filePath)
        {
            return File.Exists(_filePath);
        }

        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="_filePath"></param>
        public static void FileDelete(string _filePath)
        {
            if (_filePath == null) return;
            if (File.Exists(_filePath)) File.Delete(_filePath);
            if (File.Exists(_filePath + ".meta")) File.Delete(_filePath + ".meta");
        }

    }
}