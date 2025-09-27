using UnityEditor;
using UnityEngine;

namespace Framework.Core
{
    public static class UnityHelper
    {
        public static T AddCom<T>(this GameObject go) where T : Component
        {
            var com = go.GetComponent<T>();
            if (com == null)
            {
                com = go.AddComponent<T>();
            }

            return com;
        }

        public static bool HasCom<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        /// <summary>
        /// 获取目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <returns></returns>
        public static string GetScriptingDefineSymbols(BuildTargetGroup _targetGroup)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(_targetGroup);
        }

        /// <summary>
        /// 添加目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <param name="_defineSymbols"></param>
        public static void AddScriptingDefineSymbols(BuildTargetGroup _targetGroup, string _defineSymbols)
        {
            string lastSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_targetGroup);
            string[] lastSymbolsArray = lastSymbols.Split(';');
            for (int i = 0; i < lastSymbolsArray.Length; i++)
            {
                if (lastSymbolsArray[i].Equals(_defineSymbols)) return;
            }
            string newSymbols = $"{lastSymbols};{_defineSymbols}";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_targetGroup, newSymbols);
        }

        /// <summary>
        /// 移除目标平台下的宏定义
        /// </summary>
        /// <param name="_targetGroup"></param>
        /// <param name="_defineSymbols"></param>
        public static void RemoveScriptingDefineSymbols(BuildTargetGroup _targetGroup, string _defineSymbols)
        {
            string lastSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_targetGroup);
            string newSymbols = lastSymbols.Replace(_defineSymbols, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(_targetGroup, newSymbols);
           
        }
    }
}