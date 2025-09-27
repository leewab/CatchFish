using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    public class FormatLua
    {
        /// <summary>
        /// 文本忽略字符 （用于忽略开头含有该字符的文本行）
        /// </summary>
        private const string TagIgnore = "--";
        private const string TagField = "-- @";
        private const string TagName = "Name";
        private const string TagSortId = "SortID";
        private const string TagDescription = "Description";
        private const string TagPrefixBracket = "{";
        private const string TagSuffixBracket = "}";
        private const string EventDefineFile = "/../../Lua/common/eventdefine.lua";
        
        [MenuItem("Game/格式化 eventdefine")]
        public static bool FormatLuaFile_EventDefine()
        {
            string luaFile = Application.dataPath + EventDefineFile;
            if (!File.Exists(luaFile))
            {
                Debug.LogError($"文件不存在，注意查看文件路径! {luaFile}");
                return false;
            }
            List<string> eventContents = new List<string>();
            Dictionary<string, string> eventNameMap = new Dictionary<string, string>();
            try
            {
                using (StreamReader sr = new StreamReader(luaFile))
                {
                    string line;
                    string eventKey = "";
                    bool flag = false;
                    List<string> eventList = null;
                    int sortStart = 0;
                    int sortEnd = 0;
                    int sortCur = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith(TagIgnore))
                        {
                            eventContents.Add(line);
                            if (line.StartsWith(TagField))
                            {
                                string postLine = line.Replace(TagField, "");
                                string[] arrLines = postLine.Split(':');
                                if (arrLines[0].Equals(TagSortId))
                                {
                                    eventKey = arrLines[1];
                                    if (!string.IsNullOrEmpty(eventKey))
                                    {
                                        string[] arrSorts = eventKey.Split('-');
                                        if (string.IsNullOrEmpty(arrSorts[0]) || string.IsNullOrEmpty(arrSorts[1]))
                                        {
                                            Debug.LogError($"SortID数据为空，或者格式有误  正确格式@SortID:XX-XX");
                                            return false;
                                        }
                                        if (!int.TryParse(arrSorts[0].Trim(), out sortStart))
                                        {
                                            Debug.LogError($"SortID数据为空，或者格式有误，无法转为int");
                                            return false;
                                        }
                                        if (!int.TryParse(arrSorts[1].Trim(), out sortEnd))
                                        {
                                            Debug.LogError($"SortID数据为空，或者格式有误，无法转为int");
                                            return false;
                                        }
                                    }
                                }
                                else if (arrLines[0].Equals(TagName))
                                {
                                    if (string.IsNullOrEmpty(eventKey)) break;
                                    if (!eventNameMap.ContainsKey(eventKey))
                                    {
                                        eventNameMap.Add(eventKey, arrLines[1]);
                                    }
                                    else
                                    {
                                        Debug.LogError($"SortID:{eventKey} 重复，重复内容为 => Name:{eventNameMap[eventKey]} Name:{arrLines[1]}");
                                        return false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            string eventLine = line;
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (line.Trim().EndsWith(TagPrefixBracket) || line.Trim().StartsWith(TagPrefixBracket))
                                {
                                    flag = true;
                                    sortCur = sortStart;
                                }
                                else if(line.Trim().EndsWith(TagSuffixBracket))
                                {
                                    flag = false;
                                }
                                else
                                {
                                    if (flag)
                                    {
                                        if (sortCur > sortEnd) Debug.LogError($"SortID超出范围！");
                                        string[] arrEventContents = line.Split('=');
                                        string[] arrEventValues = arrEventContents[1].Split(',');
                                        eventLine = $"    {arrEventContents[0]} = {sortCur},{arrEventValues[1]}";
                                        sortCur++;
                                    }
                                }
                            }
                            eventContents.Add(eventLine);
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

            try
            {
                using (FileStream fs = new FileStream(luaFile, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (var content in eventContents) 
                    {
                        sw.Write(content);
                        sw.Write("\n");
                    }
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            Debug.Log("eventdefine 格式化完毕");
            return true;
        }
    }
}