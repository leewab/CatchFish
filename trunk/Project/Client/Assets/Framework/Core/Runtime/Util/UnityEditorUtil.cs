namespace Game.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    public static class UnityEditorUtil
    {
#if UNITY_EDITOR

        /// <summary>
        /// 获取目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <returns></returns>
        public static string GetScriptingDefineSymbols(TargetPlatformEnum _targetGroup)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup((BuildTargetGroup)_targetGroup);
        }

        /// <summary>
        /// 添加目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <param name="_defineSymbols"></param>
        public static void AddScriptingDefineSymbols(TargetPlatformEnum _targetGroup, string[] _defineSymbols)
        {
            //校验旧宏定义是否有空或者重复
            List<string> checkSymbolsArray = new List<string>();
            for (int i = 0; i < _defineSymbols.Length; i++)
            {
                if (string.IsNullOrEmpty(_defineSymbols[i])) continue;
                checkSymbolsArray.Add(_defineSymbols[i]);
                for (int j = i + 1; j < _defineSymbols.Length; j++)
                {
                    if (_defineSymbols[i].Equals(_defineSymbols[j])) _defineSymbols[j] = null;
                }
            }

            //新加宏定义与旧有宏定义融合
            StringBuilder newSymbols = new StringBuilder();
            for (int i = 0; i < checkSymbolsArray.Count; i++)
            {
                newSymbols.Append($"{checkSymbolsArray[i]};");
            }

            Debug.Log("添加宏定义—— " + newSymbols.ToString());
            PlayerSettings.SetScriptingDefineSymbolsForGroup((BuildTargetGroup)_targetGroup, newSymbols.ToString());
        }

        /// <summary>
        /// 移除目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <param name="_defineSymbols"></param>
        public static void RemoveScriptingDefineSymbols(TargetPlatformEnum _targetGroup, string[] _defineSymbols)
        {
            string lastSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup((BuildTargetGroup)_targetGroup);
            for (int i = 0; i < _defineSymbols.Length; i++)
            {
                lastSymbols = lastSymbols.Replace(_defineSymbols[i], "");
            }

            Debug.Log("移除宏定义—— " + lastSymbols);
            PlayerSettings.SetScriptingDefineSymbolsForGroup((BuildTargetGroup)_targetGroup, lastSymbols);
        }

        /// <summary>
        /// 添加SortingLayer层级
        /// </summary>
        /// <param name="layerName"></param>
        public static void AddSortingLayers(string layerName)
        {
            if (!HasSortingLayer(layerName))
            {
                SerializedObject tagsAndLayersManager =
                    new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty sortingLayersProp = tagsAndLayersManager.FindProperty("m_SortingLayers");
                sortingLayersProp.InsertArrayElementAtIndex(sortingLayersProp.arraySize);
                var newLayer = sortingLayersProp.GetArrayElementAtIndex(sortingLayersProp.arraySize - 1);
                newLayer.FindPropertyRelative("uniqueID").intValue = sortingLayersProp.arraySize - 1;
                newLayer.FindPropertyRelative("name").stringValue = layerName;
                tagsAndLayersManager.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("SortingLayer添加成功：" + layerName);
            }
        }

        /// <summary>
        /// SortingLayer中查找是否存在
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        private static bool HasSortingLayer(string layerName)
        {
            SerializedObject tagManager =
                new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty it = tagManager.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.name == "m_SortingLayers")
                {
                    for (int i = 0; i < it.arraySize; i++)
                    {
                        SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
                        while (dataPoint.NextVisible(true))
                        {
                            if (dataPoint.name == "name")
                            {
                                if (dataPoint.stringValue == layerName) return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 在Windows平台执行exe文件
        /// </summary>
        public static void RunExeProcess_Windows(string exeFilePath, string args, Action<bool> callBack)
        {
            Debug.Log("RunExeProcess_Windows => " + exeFilePath);
            System.Diagnostics.Process luaJitProcess = new System.Diagnostics.Process();
            luaJitProcess.StartInfo.UseShellExecute = false;
            luaJitProcess.StartInfo.FileName = exeFilePath;
            luaJitProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(exeFilePath);
            luaJitProcess.StartInfo.Arguments = args;
            RunGenLuaBytesProcess(luaJitProcess, callBack);
        }

        /// <summary>
        /// 在Windows平台执行bat文件
        /// </summary>
        public static void RunBatProcess_Windows(string batPath, Action<bool> callBack)
        {
            System.Diagnostics.Process luaJitProcess = new System.Diagnostics.Process();
            luaJitProcess.StartInfo.CreateNoWindow = true;
            luaJitProcess.StartInfo.FileName = batPath;
            RunGenLuaBytesProcess(luaJitProcess, callBack);
        }

        /// <summary>
        /// 在 OSX/UNIX 平台执行shell文件
        /// </summary>
        /// <param name="workingDir">工作目录</param>
        /// <param name="args">参数</param>
        public static void RunShellProcess_OSX_UNIX(string workingDir, string args, Action<bool> callBack)
        {
            System.Diagnostics.Process luaJitProcess = new System.Diagnostics.Process();
            luaJitProcess.StartInfo.UseShellExecute = false;
            luaJitProcess.StartInfo.FileName = "bash";
            luaJitProcess.StartInfo.WorkingDirectory = Application.dataPath + workingDir;
            luaJitProcess.StartInfo.Arguments = Application.dataPath + args;
            RunGenLuaBytesProcess(luaJitProcess, callBack);
        }

        /// <summary>
        /// 执行生成Lua二进制文件的进程
        /// </summary>
        /// <param name="process">待执行的进程</param>
        public static void RunGenLuaBytesProcess(System.Diagnostics.Process process, Action<bool> callBack)
        {
            using (process)
            {
                process.Start();
                process.WaitForExit();
                process.Close();
                callBack?.Invoke(true);
            }
        }

#endif
    }
}