using System;
using System.Collections.Generic;
using System.IO;
using Game.Core;
using MUEngine;
using UnityEngine;

namespace Game
{
    public class EditorProcess : IProcess
    {
        public string PreloadDesc => "EditorProcess";
        public Action OnFinishedEvent { get; set; }

        public void Start()
        {
#if UNITY_EDITOR
            GameConfig.LoadFromPrefab = Main.Instance.LoadFromPrefab;
            CustomGraphicRaycaster.isLoadFromPrefab = GameConfig.LoadFromPrefab;
            this.LinkResAsset();
#endif
            this.Finish();
        }

        private void Finish()
        {
            this.OnFinishedEvent?.Invoke();
            this.OnFinishedEvent = null;
        }
        
#if UNITY_EDITOR
        
        private void LinkResAsset()
        {
            if (GameConfig.LoadFromPrefab)
            {
                CoreDelegate.LoadAsset = this.LoadFromPrefab;
                if ( !System.IO.Directory.Exists("./Assets/ResAssets"))
                {
                    string file = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\Framework\\_mklink.bat";
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo( file );
                    psi.RedirectStandardOutput = true;
                    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    psi.UseShellExecute = false;
                    psi.WorkingDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Assets\\";
                    System.Diagnostics.Process listFiles;
                    listFiles = System.Diagnostics.Process.Start( psi );
                    System.IO.StreamReader myOutput = listFiles.StandardOutput;
                    listFiles.WaitForExit( );
                    string output = myOutput.ReadToEnd();
                    Debug.Log(output);
                    UnityEditor.EditorApplication.isPlaying = false;
                    UnityEditor.EditorApplication.isPlaying = true;
                }
            }
        }
        
        private static Dictionary<string, string> fileMap;
        public UnityEngine.Object LoadFromPrefab(string name)
        {
            if (fileMap == null)
            {
                fileMap = new Dictionary<string, string>();
                string[] paths = UnityEditor.AssetDatabase.GetAllAssetPaths();
                foreach ( string file in paths )
                {
                    // if ( file.Contains( "/generate/" ) || file.Contains("/UIPrefab/") )
                    //     continue;
                    if (file.StartsWith("Assets/ResAssets"))
                    {
                        Debug.Log(file);
                        Debug.Log(Path.GetFileName(file));
                        fileMap[Path.GetFileName(file)] = file;
                    }
                }
            }
            string path;
            if ( fileMap.TryGetValue( name, out path ) )
            {
                //Debug.Log( "LoadFromPrefab: " + name );
                //加载图片时，指定类型，这样在加载很多sprite时，editor模式和bundle模式可以有一样的结果。
                //由于假定了，加载.png都是加载sprite,所以可能会导致一些其它的问题
                //此处代码谨慎提交,如果有问题请回退此处 modify by iiujunjie in 2018/11/23
//				UnityEngine.Object obj = null
//				if (path.EndsWith (".png")) {
//					var allObjs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath (path);
//					obj = allObjs.Length > 0 ? allObjs [allObjs.Length - 1] : null;
//				} else {
//					obj = UnityEditor.AssetDatabase.LoadAssetAtPath( path, typeof(UnityEngine.Object));
//				}
                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath( path, typeof(UnityEngine.Object));
                if ( obj != null )
                {
                    if ( obj as GameObject )
                    {
                        ( obj as GameObject ).SetActive( false );
                    }
                    return obj;
                }
            }

            return null;
        }
#endif
        
    }
}