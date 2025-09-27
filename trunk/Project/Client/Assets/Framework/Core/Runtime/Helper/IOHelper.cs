using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Core
{
    public static class IOHelper
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
        }

        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="sourceDirPath"></param>
        /// <param name="destDirPath"></param>
        public static void MoveDirectory(string sourceDirPath, string destDirPath)
        {
            LogHelper.Log(sourceDirPath);
            LogHelper.Log(destDirPath);
            Directory.Move(sourceDirPath, destDirPath);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="_dirPath"></param>
        public static void CreateDirectory(string _dirPath)
        {
            if (string.IsNullOrEmpty(_dirPath)) return;
            if (!DirExists(_dirPath))
            {
                Directory.CreateDirectory(_dirPath);
            }
            else
            {
                LogHelper.Log("该文件夹目录已存在！无需创建");
            }
        }

        /// <summary>
        /// 判断文件夹是否存在
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="isLogTips">是否log打印提示</param>
        /// <returns></returns>
        public static bool DirExists(string dirPath, bool isLogTips = true)
        {
            if (string.IsNullOrEmpty(dirPath))
            {
                LogHelper.Log("文件夹路径为空！");
                return false;
            }
            bool isHas = Directory.Exists(dirPath);
            if (!isHas && isLogTips) LogHelper.Error("IOHelper 文件夹目录不存在。" + dirPath);
            return isHas;
        }
        
        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        public static void DeleteDir(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath)) return;
            Debug.Log("DeleteDirectory//" + dirPath);
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath);
        }

        /// <summary>
        /// 根据string内容创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        public static void CreateFile(string filePath, string content)
        {
            CreateFile(filePath, System.Text.Encoding.Default.GetBytes(content));
        }

        /// <summary>
        /// 根据byte字节创建文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        public static void CreateFile(string filePath, byte[] bytes)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            if (File.Exists(filePath)) File.Delete(filePath);
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                fs.Write(bytes, 0, bytes.Length - 1);
                fs.Close();
            }
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="isTips"></param>
        /// <returns></returns>
        public static bool FileExists(string filePath, bool isTips = true)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                LogHelper.Log("文件路径为空！");
                return false;
            }
            bool isHas = File.Exists(filePath);
            if (!isHas && isTips) Debug.LogError("IOHelper 文件不存在。" + filePath);
            return isHas;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;
            Debug.Log("Delete //" + filePath);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string LoadFileString(string filePath)
        {
            byte[] bytes = LoadFileByte(filePath);
            if (bytes == null || bytes.Length <= 0) return null;
            return System.Text.Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// 加载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] LoadFileByte(string filePath)
        {
            if (!FileExists(filePath)) return null;
            byte[] data = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                int len = (int) fs.Length;
                data = new byte[len];
                fs.Read(data, 0, len);
                fs.Close(); 
            }
            return data;
        }

        /// <summary>
        /// 文本忽略字符 （用于忽略开头含有该字符的文本行）
        /// </summary>
        private static char[] IgnoreChara = { '*','#','/','\\' };

        /// <summary>
        /// 通过行数读取文本
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[] ReadFileLineByte(string filePath)
        {
            List<string> data = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        foreach (var t in IgnoreChara)
                        {
                            if (!line.StartsWith(t.ToString()))
                            {
                                data.Add(line);
                            }
                        }
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("This file is not find or can`t read");
                Console.WriteLine(e.Message);
            }

            return data.ToArray();
        }
        
        
    }
}