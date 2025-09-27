using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 语言类型
    /// </summary>
    public class LanguageType
    {
        /// <summary>
        /// 简体中文
        /// </summary>
        public const string SimpChinese = "SC";
        /// <summary>
        /// 繁体中文
        /// </summary>
        public const string TradChinese = "TC";
        /// <summary>
        /// 英语
        /// </summary>
        public const string English = "EN";
        /// <summary>
        /// 印尼语
        /// </summary>
        public const string Indonesian = "ID";
        /// <summary>
        /// 法语
        /// </summary>
        public const string French = "FR";
        /// <summary>
        /// 德语
        /// </summary>
        public const string German = "DE";
        /// <summary>
        /// 韩语
        /// </summary>
        public const string Korean = "KO";
        /// <summary>
        /// 俄语
        /// </summary>
        public const string Russian = "RU";
        /// <summary>
        /// 泰语
        /// </summary>
        public const string Thailand = "TH";
        /// <summary>
        /// 西班牙语
        /// </summary>
        public const string Spanish = "ES";
        /// <summary>
        /// 葡萄牙语
        /// </summary>
        public const string Portuguese = "PT";
    }


    public class LanguageHelper
    {
        private static LanguageHelper _instance;

        public static LanguageHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageHelper();
                    _instance.Init();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 默认语言
        /// </summary>
        public const string NormalLanguage = LanguageType.SimpChinese;
        
        /// <summary>
        /// 当前选中的语言
        /// </summary>
        public string SelectLanguage
        {
            get
            {
                string localLanguage = PlayerPrefs.GetString(LANGUAGE_KEY);
                if (string.IsNullOrEmpty(localLanguage))
                {
                    localLanguage = GetSysLanguage();
                    if (string.IsNullOrEmpty(localLanguage)) localLanguage = NormalLanguage;
                    if (!mLanguageNameDic.ContainsKey(localLanguage))
                    {
                        localLanguage = NormalLanguage;
                        Debug.LogError($"当前语言列表中没有对应的语言 {localLanguage} ，默认切换为 {NormalLanguage}");
                    }
                    PlayerPrefs.SetString(LANGUAGE_KEY, localLanguage);
                }
                
                return localLanguage;
            }
        }

        /// <summary>
        /// 语言存储key
        /// </summary>
        private const string LANGUAGE_KEY = "LanguegeType_Local_Key";
        
        /// <summary>
        /// 语言字典
        /// </summary>
        private Dictionary<string, string> mLanguageNameDic = new Dictionary<string, string>();

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            mLanguageNameDic.Add(LanguageType.SimpChinese, "简体中文");
            mLanguageNameDic.Add(LanguageType.TradChinese, "繁体中文");
            mLanguageNameDic.Add(LanguageType.English, "English");
            mLanguageNameDic.Add(LanguageType.Indonesian, "Indonesia");
            mLanguageNameDic.Add(LanguageType.French, "French");
            mLanguageNameDic.Add(LanguageType.German, "German");
//            mLanguageNameDic.Add(LanguageType.Korean, "Korean");
//            mLanguageNameDic.Add(LanguageType.Russian, "Russian");
//            mLanguageNameDic.Add(LanguageType.Korean, "Spanish");
//            mLanguageNameDic.Add(LanguageType.Russian, "Portuguese");
        }

        public string GetDisplayName(string key)
        {
            if(mLanguageNameDic.ContainsKey(key))
            {
                return mLanguageNameDic[key];
            }
            return string.Empty;
        }

        public string GetLanguageType(string name)
        {
            foreach (string key in mLanguageNameDic.Keys)
            {
                if (mLanguageNameDic[key] == name)
                {
                    return key;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取系统语言
        /// </summary>
        /// <returns></returns>
        public string GetSysLanguage()
        {
            var lan = Application.systemLanguage;
            switch (lan)
            {
                case SystemLanguage.ChineseSimplified:
                    return LanguageType.SimpChinese;
                case SystemLanguage.ChineseTraditional:
                    return LanguageType.TradChinese;
                case SystemLanguage.English:
                    return LanguageType.English;
                case SystemLanguage.Indonesian:
                    return LanguageType.Indonesian;
                case SystemLanguage.French:
                    return LanguageType.French;
                case SystemLanguage.German:
                    return LanguageType.German;
                case SystemLanguage.Korean:
                    return LanguageType.Korean;
                case SystemLanguage.Russian:
                    return LanguageType.Russian;
            }

            return null;
        }
    }
}