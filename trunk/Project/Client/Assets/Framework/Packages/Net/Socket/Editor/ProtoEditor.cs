using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Framework.Core;
using protocol;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework.Case.Net.Socket.Editor
{
    public class ProtoEditor
    {
#if UNITY_EDITOR

        private static string strTargetProtoDir = "/Scripts/Proto/";

        [MenuItem("Game/Tools/Proto")]
        public static void OpenProtoWindow()
        {
            var protoWin = EditorWindow.CreateInstance<ProtoWindow>();
            protoWin.Show();
        }

#endif
    }

    public class ProtoWindow : EditorWindow
    {
        private static string protoDir = @"Tools\Protobuf\protoToCs";
        private static string protobufSourceDir = "Tools/Protobuf/protoToCs/generate";
        private static string protobufTargetDir = "/Assets/Framework/Case/Net/Socket/Scripts/Proto";

        private static string protobufTxt = "Assets/Framework/Case/Net/Socket/Editor/ProtobufHandler.txt";
        private static string protobufCS = "/Framework/Case/Net/Socket/Scripts/Handler/ProtobufHandler.cs";

#if UNITY_EDITOR

        private void OnGUI()
        {
            EditorHelper.DrawTitle("Proto Importer");
            EditorHelper.DrawButton("导入Proto2Cs", new Vector2(120, 30), () =>
            {
#if UNITY_EDITOR_WIN
                GenerateProtoFile_Win();
#endif
            });
        }

        /// <summary>
        /// Window下生成Proto协议文件
        /// </summary>
        private void GenerateProtoFile_Win()
        {
            //根据ProtoDefine创建ProtobufHandler
            GenerateProtobufHandle();
            string toolProtoDir = EditorHelper.CurrentDirectoryWin.Replace(@"Client", protoDir);
            ExcuteProto2Cs( toolProtoDir, "protoToCs.bat", () =>
            {
                string sourceDir = EditorHelper.CurrentDirectoryUnity.Replace("Client", protobufSourceDir);
                string targetDir = EditorHelper.CurrentDirectoryUnity + protobufTargetDir;
                IOHepler.DirectoryCopy(sourceDir, targetDir, "cs");
            });
        }

        /// <summary>
        /// 执行bat文件
        ///  注意bat文件由于使用的是Win下 Process执行 所以本质上所指的路径必须是 \ 而非 / 所以所对应的bat文件需要以
        ///  @"E:\IPro\L-Framework\Tools\Protobuf\protoToCs" 这种方式指向
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="batFile"></param>
        private static void ExcuteProto2Cs(string targetDir, string batFile, Action callBack)
        {
            Debug.LogError(targetDir);
            if (!batFile.EndsWith(".bat"))
            {
                Debug.LogError("非bat文件");
                return;
            }

            Process process = new Process();
            process.StartInfo.WorkingDirectory = targetDir;
            process.StartInfo.FileName = batFile;
            process.StartInfo.Arguments = "10";
            process.StartInfo.CreateNoWindow = true;                //是否不显示进度面板
            process.Start();
            process.WaitForExit();
            if (process.HasExited)
            {
                callBack?.Invoke();
            }
        }

        private static void GenerateProtobufHandle()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int indexA = 0, indexB = 0;
            string[] contents = File.ReadAllLines(protobufTxt);
            for (int i = 0; i < contents.Length; i++)
            {
                stringBuilder.AppendLine(contents[i]);
                if (contents[i].Contains("//dynamic"))
                {
                    indexA = i + 1;
                    indexB = i + 2;
                    string strMoudle = contents[indexA] + "\n" + contents[indexB];
                    foreach (var value in Enum.GetValues(typeof(ProtoDefine)))
                    {
                        string strName = Enum.GetName(typeof(ProtoDefine), value);
                        stringBuilder.AppendLine(String.Format(strMoudle, strName, strName));
                    }

                    i += 2;
                }
            }
            Debug.Log(stringBuilder.ToString());
            protobufCS = Application.dataPath + protobufCS;
            Debug.Log(protobufCS);
            if (File.Exists(protobufCS)) { File.Delete(protobufCS); }
            using (FileStream fs = new FileStream(protobufCS, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(stringBuilder.ToString());
                sw.Close();
                fs.Dispose();
                AssetDatabase.Refresh();
                Debug.LogError("ProtobufHandler.cs 创建成功！");
            }
        }

        private void RefreshRecordFile()
        {
            
        }

#endif
    }
}