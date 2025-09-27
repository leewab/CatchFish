#if UNITY_TOLUA

using System.Collections.Generic;
using System.IO;
using System.Text;
using Game.Core;
using LuaInterface;
using MUEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class LuaModule : BaseModule
    {
        public static LuaModule Instance => Main.Instance.LuaModule;

        private LuaInterface.LuaFunction _levelLoaded = null;
        private LuaInterface.LuaFunction _screenSizeChange = null;
        private LuaInterface.LuaFunction _luaUpdate = null;
        private LuaInterface.LuaFunction _luaOnDestroy = null;
        private LuaInterface.LuaFunction _luaApplicationQuit = null;
        private LuaInterface.LuaFunction _luaApplicationFocus = null;
        private LuaInterface.LuaFunction _luaAddUIScreenEffect = null;
        private LuaInterface.LuaFunction _luaDelUIScreenEffect = null;
        private LuaInterface.LuaFunction _luaApplicationPause = null;
        private LuaInterface.LuaFunction _luaRemoteGMFunc = null;
        private LuaInterface.LuaFunction _luaEnterGameFunc = null;

        public void InitLua()
        {
#if UNITY_TOLUA
            LuaClient.Instance.Init();
    #if UNITY_EDITOR
            BuildLuaFileMap();
    #endif
#endif
        }
        
        public void StartLua()
        {
#if UNITY_TOLUA
            LuaClient.Instance.StartLua();
#endif
        }

        public void DoStepGC()
        {
#if UNITY_TOLUA
            LuaClient.Instance.DoStepGC();
#endif
        }

        public override void Update()
        {
            
        }

        public override void LogicUpdate()
        {
            //UnityEngine.Profiling.Profiler.BeginSample("UpdateLua");
            UpdateLua();
            UpdateGMMessage();
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        public override void RegisterEvent()
        {
#if UNITY_TOLUA
            EngineDelegate.OnEntityLuaCallBack += OnEntityLuaCallBack;
            EngineDelegate.OnRemoveLuaCallBack += OnRemoveLuaCallBack;
            EngineDelegate.LogWithLuaStack += OnLogWithLuaStack;
            
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif   
            
#endif
        }

        public override void UnRegisterEvent()
        {
#if UNITY_TOLUA
            EngineDelegate.OnEntityLuaCallBack -= OnEntityLuaCallBack;
            EngineDelegate.OnRemoveLuaCallBack -= OnRemoveLuaCallBack;
            EngineDelegate.LogWithLuaStack -= OnLogWithLuaStack;
            
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif      
            
#endif
        }
        
        public override void Dispose()
        {
            base.Dispose();
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
#if UNITY_TOLUA
            // OnDestroy
            if (LuaClient.Instance.bPreload)
            {
                if (_luaOnDestroy == null)
                {
                    _luaOnDestroy = LuaClient.Instance.GetLuaFunction("OnDestroy");
                }
                LuaClient.Instance.CallFunction(_luaOnDestroy);
            }
            
            // LuaFunction 释放
            if (_levelLoaded != null)
            {
                _levelLoaded.Dispose();
                _levelLoaded = null;
            }

            if (_screenSizeChange != null)
            {
                _screenSizeChange.Dispose();
                _screenSizeChange = null;
            }            
            
            if (_luaUpdate != null)
            {
                _luaUpdate.Dispose();
                _luaUpdate = null;
            }
                        
            if (_luaOnDestroy != null)
            {
                _luaOnDestroy.Dispose();
                _luaOnDestroy = null;
            }
            
            if (_luaApplicationQuit != null)
            {
                _luaApplicationQuit.Dispose();
                _luaApplicationQuit = null;
            }
            
            if (_luaApplicationFocus != null)
            {
                _luaApplicationFocus.Dispose();
                _luaApplicationFocus = null;
            }
            
            if (_luaAddUIScreenEffect != null)
            {
                _luaAddUIScreenEffect.Dispose();
                _luaAddUIScreenEffect = null;
            }
            
            if (_luaDelUIScreenEffect != null)
            {
                _luaDelUIScreenEffect.Dispose();
                _luaDelUIScreenEffect = null;
            }
            
            if (_luaApplicationPause != null)
            {
                _luaApplicationPause.Dispose();
                _luaApplicationPause = null;
            }
            
            // LuaClient 释放
            LuaClient.Instance.Destroy();
#endif
            
#if UNITY_EDITOR
            mLuaFileMap.Clear();
#endif
        }
        
#if UNITY_5_4_OR_NEWER
        protected void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            OnLevelLoaded(scene.buildIndex);
        }
#else
        protected void OnLevelWasLoaded(int level)
        {
            OnLevelLoaded(level);
        }
#endif
        
        public void OnApplicationPause(bool isPause)
        {
#if UNITY_TOLUA
            if (isPause)
            {
                DoStepGC();
            }
            
            if(LuaClient.Instance.bPreload)
            {
                if(_luaApplicationPause == null)
                {
                    _luaApplicationPause = LuaClient.Instance.GetLuaFunction("OnApplicationPause");
                }
                if(_luaApplicationPause != null)
                {
                    _luaApplicationPause.BeginPCall();
                    _luaApplicationPause.Push(isPause);
                    _luaApplicationPause.PCall();
                    _luaApplicationPause.EndPCall();
                }
            }
#endif
        }
        
        public void OnApplicationFocus(bool hasFocus)
        {
            if (LuaClient.Instance.bPreload)
            {
                if (_luaApplicationFocus == null)
                {
                    _luaApplicationFocus = LuaClient.Instance.GetLuaFunction("OnApplicationFocus");
                }
                if(_luaApplicationFocus != null)
                {
                    _luaApplicationFocus.BeginPCall();
                    _luaApplicationFocus.Push(hasFocus);
                    _luaApplicationFocus.PCall();
                    _luaApplicationFocus.EndPCall();
                }
            }
        }

        public void OnApplicationQuit()
        {
            if (LuaClient.Instance.bPreload)
            {
                if (_luaApplicationQuit == null)
                {
                    _luaApplicationQuit = LuaClient.Instance.GetLuaFunction("OnApplicationQuit");
                }
                LuaClient.Instance.CallFunction(_luaApplicationQuit);
                LuaClient.Instance.Destroy();
            }
            
            RemoteGMServer.StopServer();
            
        }
        
        void OnLevelLoaded(int level)
        {
            if (_levelLoaded != null)
            {
                _levelLoaded.BeginPCall();
                _levelLoaded.Push(level);
                _levelLoaded.PCall();
                _levelLoaded.EndPCall();
            }

            var luaState = LuaClient.Instance.GetLuaState();
            if (luaState != null)
            {            
                luaState.RefreshDelegateMap();
            }
        }

        #region Event Function
        
#if UNITY_TOLUA
        private void OnEntityLuaCallBack(MUActorLuaCallParam param)
        {
            LuaUtil.OnEntityLuaCallBack(param);
        }
        
        private void OnRemoveLuaCallBack(int reference)
        {
            LuaUtil.OnRemoveLuaCallBack(reference);
        }
        
        private void OnLogWithLuaStack(string msg,int errorLevel)
        {
            LuaUtil.LogWithLuaStack(msg, errorLevel);
        }
#endif
        #endregion

        public void EnterGame(string roleInfo)
        {
#if UNITY_TOLUA
            if (!LuaClient.Instance.bPreload) return;
            if (_luaEnterGameFunc == null) _luaEnterGameFunc = LuaClient.Instance.GetLuaFunction("EnterGame");
            if (_luaEnterGameFunc != null)
            {
                _luaEnterGameFunc.BeginPCall();
                _luaEnterGameFunc.Push(roleInfo);
                _luaEnterGameFunc.PCall();
                _luaEnterGameFunc.EndPCall();
            }
#endif
        }
        
        public void UpdateLua()
        {
#if UNITY_TOLUA
            if (!LuaClient.Instance.bPreload)
            {
                return;
            }

            if (_luaUpdate == null)
            {
                _luaUpdate = LuaClient.Instance.GetLuaFunction("Update");
            }
            LuaClient.Instance.CallFunction(_luaUpdate);
#endif
        }
        
        public void AddUIScreenEffect(string param)
        {
#if UNITY_TOLUA
            if (_luaAddUIScreenEffect == null)
            {
                _luaAddUIScreenEffect = LuaClient.Instance.GetLuaFunction("AddUIScreenEffect");
            }
            LuaClient.Instance.CallFunction(_luaAddUIScreenEffect, param);
#endif
        }        
        
        public void DelUIScreenEffect()
        {
#if UNITY_TOLUA
            if (_luaDelUIScreenEffect == null)
            {
                _luaDelUIScreenEffect = LuaClient.Instance.GetLuaFunction("DelUIScreenEffect");
            }
            LuaClient.Instance.CallFunction(_luaDelUIScreenEffect);
#endif
        }
        
        public void UpdateGMMessage()
        {
#if UNITY_TOLUA
            if (string.IsNullOrEmpty(GameModule.Instance.RemoteGMCmd)) return;

            if (_luaRemoteGMFunc == null)
            {
                _luaRemoteGMFunc = LuaClient.Instance.GetLuaFunction("OnRemoteGMMessage");
            }
            
            if (_luaRemoteGMFunc != null)
            {
                _luaRemoteGMFunc.BeginPCall();
                _luaRemoteGMFunc.Push(GameModule.Instance.RemoteGMCmd);
                _luaRemoteGMFunc.PCall();
                _luaRemoteGMFunc.EndPCall();
            }
            
            GameModule.Instance.RemoteGMCmd = string.Empty;
#endif
        }
        
        public void ScreenSizeChange()
        {
#if UNITY_TOLUA
            if (LuaClient.Instance.bPreload)
            {
                if (_screenSizeChange == null)
                {
                    _screenSizeChange = LuaClient.Instance.GetLuaFunction("ScreenSizeChange");
                }
                LuaClient.Instance.CallFunction(_screenSizeChange);
            }
#endif
        }

        public void DB_ConfDisConnect()
        {
#if UNITY_TOLUA
            if (LuaClient.Instance.bPreload)
            {
                LuaClient.Instance.CallFunction("DB.ConfDisConnect");
                LuaClient.Instance.CallFunction("DB.CacheDisConnect");
                LuaClient.Instance.Destroy();
            }
#endif
        }
        
        /// <summary>
        /// 动态预加载Lua脚本内容 (不运行)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="luaText"></param>
        public void PreLoadLua(string fileName, string luaText)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(luaText)) return;
            byte[] luaBytes = System.Text.Encoding.UTF8.GetBytes(luaText);
            PreLoadLua(fileName, luaBytes);
        }
        
        /// <summary>
        /// 动态预加载Lua脚本内容 (不运行)
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="luaBytes"></param>
        public void PreLoadLua(string fileName, byte[] luaBytes)
        {
            if (luaBytes != null && luaBytes.Length > 0)
            {
                LuaFileUtils.Instance.AddBytes(fileName, luaBytes);
            }
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// lua文件名map，文件名-路径
        /// </summary>
        private Dictionary<string, string> mLuaFileMap = new Dictionary<string, string>();
#endif
        
#if UNITY_EDITOR
        private void BuildLuaFileMap()
        {
            StringBuilder sb = new StringBuilder();
            WalkLuaFiles(LuaConst.luaDir, delegate(string filename, string fullname) {
                //删除filename的.lua后缀
                filename = filename.Substring(0, filename.Length - ".lua".Length);
                fullname = fullname.Replace("\\", "/");
                if (mLuaFileMap.ContainsKey(filename))
                {
                    //已经记录过这个文件名了，说明文件名有重名，报出来
                    string orig_filepath = mLuaFileMap[filename];
                    sb.Clear();
                    sb.Append("lua文件名重名：\n");
                    sb.Append(orig_filepath);
                    sb.Append(fullname);
                    Debug.LogError(sb.ToString());
                }
                else
                {
                    mLuaFileMap[filename] = fullname;
                }
            });
        }

        private void WalkLuaFiles( string parent_dir, System.Action<string, string> callback )
        {
            DirectoryInfo dir = new DirectoryInfo(parent_dir);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo finfo in files)
            {
                if (finfo.Name.EndsWith(".lua"))
                {
                    //是lua文件
                    if (callback != null)
                    {
                        callback(finfo.Name, finfo.FullName);
                    }                    
                }
            }
            DirectoryInfo[] sub_dirs = dir.GetDirectories();
            foreach (DirectoryInfo sub_dir in sub_dirs)
            {
                WalkLuaFiles(sub_dir.FullName, callback);
            }
        }
#endif

       
    }

}

#endif