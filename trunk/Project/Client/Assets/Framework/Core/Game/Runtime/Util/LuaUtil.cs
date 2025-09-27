#if UNITY_TOLUA
using System;
using System.Collections.Generic;
using Game.Core;
using Game.UI;
using LuaInterface;
using MUEngine;
using MUGUI;
using UnityEngine;

namespace Game
{
    public static class LuaUtil
    {
        
        private static Dictionary<int, LuaFunction> mActorLuaFunctionMap = new Dictionary<int, LuaFunction>();
        public static void SetEntityLuaCallBack(Entity entity, LuaFunction func)
        {
            int reference = func.GetReference();
            entity.LuaCallBackReference = reference;
            mActorLuaFunctionMap[reference] = func;
        }

        public static void ClearEntityLuaCallBack(Entity entity)
        {
            if(entity == null)
            {
                return;
            }
            int reft = entity.LuaCallBackReference;
            mActorLuaFunctionMap.Remove(reft);
        }
        
        public static void OnEntityLuaCallBack(MUActorLuaCallParam param)
        {
            LuaFunction func;
            if (mActorLuaFunctionMap.TryGetValue(param.FuncReference, out func))
            {
                int funcType = param.FuncType;
                if (param.FuncType == (int)EActorLuaCallback.EffectMoveToCallback
                    || param.FuncType == (int)EActorLuaCallback.OnLoadResource
                    || param.FuncType == (int)EActorLuaCallback.MoveToFinishCallback
                    || param.FuncType == (int)EActorLuaCallback.OnUserStopMove
                    || param.FuncType == (int)EActorLuaCallback.OnUserStopFreeFly)
                    LuaClient.Instance.CallLuaFunction(func, (int)param.FuncType);
                else if (param.FuncType == (int)EActorLuaCallback.OnUserStopCurve)
                {
                    if (func != null)
                    {
                        func.BeginPCall();
                        func.Push(param.FuncType);
                        func.Push(param.Vec3Param1);
                        func.PCall();
                        func.EndPCall();
                    }
                }
                else if (param.FuncType == (int)EActorLuaCallback.OnCloseToGround
                         || param.FuncType == (int)EActorLuaCallback.OnUserFlySpeedChanged)
                {
                    if (func != null)
                    {
                        func.BeginPCall();
                        func.Push(param.FuncType);
                        func.Push(param.FParam);
                        func.PCall();
                        func.EndPCall();
                    }
                }
                else if (param.FuncType == (int)EActorLuaCallback.OnUserJumpTo
                         || param.FuncType == (int)EActorLuaCallback.OnUserMoveTo)
                {
                    if (func != null)
                    {
                        func.BeginPCall();
                        func.Push(param.FuncType);
                        func.Push(param.Vec3Param1);
                        func.Push(param.Vec3Param2);
                        func.PCall();
                        func.EndPCall();
                    }
                }
                else if (param.FuncType == (int)EActorLuaCallback.OnUserFreeFlyTo)
                {
                    if (func != null)
                    {
                        func.BeginPCall();
                        func.Push(param.FuncType);
                        func.Push(param.Vec3Param1);
                        func.Push(param.Vec3Param2);
                        func.Push(param.FParam);
                        func.PCall();
                        func.EndPCall();
                    }
                }
                else
                {
                    //Debug.LogError(" OnEntityLuaCallBack error functype  " + param.FuncType.ToString());
                }
            }
        }

        public static void OnRemoveLuaCallBack(int reference)
        {
            mActorLuaFunctionMap.Remove(reference);
        }
        
        public static Vector2 GetUIPiexls(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return Vector2.zero;
            }
            var canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return Vector2.zero;
            }
            canvas = canvas.rootCanvas;
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.worldCamera == null)
            {
                var scale = rectTransform.lossyScale;
                return new Vector2(rectTransform.rect.width * scale.x, rectTransform.rect.height * scale.y);
            }
            else
            {

                var camera = canvas.worldCamera;

                var rect = rectTransform.rect;
                Vector3 worldLeftButtom = rectTransform.TransformPoint(rect.min);
                Vector3 worldRightUp = rectTransform.TransformPoint(rect.max);
                return camera.WorldToScreenPoint(worldRightUp) - camera.WorldToScreenPoint(worldLeftButtom);
            }
        }

        

        // //左侧摇杆，模拟手指抬起
        // public static void SimulateJoyStickPointerUp(GameObject gameObject)
        // {
        //     if(gameObject == null)
        //     {
        //         return;
        //     }
        //     ETCJoystick joystick = gameObject.GetComponent<ETCJoystick>();
        //     if (joystick == null)
        //     {
        //         return;
        //     }
        //     UnityEngine.EventSystems.PointerEventData fakeData = new UnityEngine.EventSystems.PointerEventData(null);
        //     fakeData.pointerId = joystick.pointId;
        //     joystick.OnPointerUp (fakeData);
        // }
        // public static void StartGameUIAdapterTest()
        // {
        //     GameUIAdapter.TestUIAdap();
        // }

        public static void EnableLogicControlShadow(bool bEnable)
        {
            if (bEnable)
            {
                LightFaceEffect.LogicControlShadow(true, true, false, false, ShadowProjection.CloseFit, 0, 0.0f, Vector3.zero, 0.0f, ShadowResolution.Low);
            }
            else
            {
                LightFaceEffect.LogicControlShadow(false, false, false, false, ShadowProjection.CloseFit, 0, 0.0f, Vector3.zero, 0.0f, ShadowResolution.Low);
            }
        }

        public static void InitAdvancedTextRegex(string tagRegex, string tagRegex2, string hrefRegex)
        {
            AdvancedText.InitRegex(tagRegex, tagRegex2, hrefRegex);
        }
        
        // public static void SetChaosCameraEnabled(bool enable)
        // {
        //     if (Main.Instance != null && Main.Instance.ChaosCamera != null)
        //         Main.Instance.ChaosCamera.enabled = enable;
        // }
        
        public static void OnGameCmd(string cmd)
        {
            try {
                if (cmd.Contains("Egn_")) {
                    if (cmd.Contains("Egn_Debug")) {
                        if (cmd.Contains("Egn_Debug_On")) {
                            EngineDebugTool.Init();
                        }
                        else if (cmd.Contains("Egn_Debug_Off")) {
                            EngineDebugTool.Exit();
                        }
                    }
                    else if (cmd.Contains("Egn_Frame"))
                    {
                        string[] ss = cmd.Split('_');
                        string s = ss[ss.Length - 1];
                        try
                        {
                            int ans = int.Parse(s);
                            Application.targetFrameRate = ans;
                        }
                        catch { }
                    }
                    else if (cmd.Contains("Egn_Shaderlod"))
                    {
                        string[] ss = cmd.Split('_');
                        string s = ss[ss.Length - 1];
                        try
                        {
                            int ans = int.Parse(s);
                            Shader.globalMaximumLOD = ans;
                        }
                        catch { }
                    }
                    //else if (cmd.Contains("Egn_DynamicRes"))
                    //{
                    //    string[] ss = cmd.Split('_');
                    //    string s = ss[ss.Length - 1];
                    //    try
                    //    {
                    //        int ans = int.Parse(s);
                    //        SceneRenderer2DMod.Instance.StartWork(1f, true, ans * 0.01f);
                    //    }
                    //    catch { }
                    //}
                    else if (cmd.Contains("Egn_AllowWorldUICamera"))
                    {
                        string[] ss = cmd.Split('_');
                        string s = ss[ss.Length - 1];
                        try
                        {
                            int ans = int.Parse(s);
                            if (MURoot.MUCamera != null)
                            {
                                MURoot.MUCamera.ForceWorldUICameraEnable(ans != 0);
                            }
                        }
                        catch { }
                    }
                    else if (cmd.Contains("Egn_AntiAliasing"))
                    {
                        string[] ss = cmd.Split('_');
                        string s = ss[ss.Length - 1];
                        try
                        {
                            int ans = int.Parse(s);
                            QualitySettings.antiAliasing = ans;
                        }
                        catch { }
                    }
                    else if (cmd.Contains("Egn_TestA"))
                    {
                        Shader.EnableKeyword("_A");

                    }
                    else if (cmd.Contains("Egn_TestB"))
                    {
                        Shader.EnableKeyword("_B");

                    }
                    else if (cmd.Contains("Egn_TestC"))
                    {
                        Shader.EnableKeyword("_C");

                    }
                    else if (cmd.Contains("Egn_Revert"))
                    {
                        Shader.DisableKeyword("_B");
                        Shader.DisableKeyword("_A");
                        Shader.DisableKeyword("_C");
                    }
                    else if (cmd.Contains("Egn_Lala"))
                    {
                        if (LightFaceEffect.GetLFE() != null)
                        {
                            if (LightFaceEffect.GetLFE().mConst != null)
                            {
                                LightFaceEffect.GetLFE().mConst.AllowWaterGrab = false;

                            }
                        }
                    }
                    else if(cmd.Contains("Egn_lightmapa"))
                    {
                        Shader.EnableKeyword("LIGHTMAPCHANGE");
                    }else if (cmd.Contains("Egn_lightmapb"))
                    {
                        Shader.DisableKeyword("LIGHTMAPCHANGE");
                    }
                    else if (cmd.Contains("Egn_TestGray"))
                    {
                        MUEngine.MUActorEntity actor = (MUEngine.MURoot.Scene.Hero as MUEngine.MUActorEntity);
                        if (actor != null)
                        {
                            actor.SwitchDiscoloration(1f);
                        }
                    }
                    else
                    {

                    }

                }
            }
            catch {
                return;
            }
            switch(cmd)
            {
                case "gm":
                    break;
            }
        }

        public static float GetRealTimeSinceStart()
        {
            return MURoot.RealTime.RealTime;
        }
        
         public static bool SetGraphicRaycasterEnabled(UnityEngine.GameObject obj, bool enabled)
         {
             if (obj == null)
                 return false;
             UnityEngine.UI.GraphicRaycaster caster = obj.GetComponent<UnityEngine.UI.GraphicRaycaster>();
             if (caster != null)
             {
                 caster.enabled = enabled;
                 return true;
             }
             CustomGraphicRaycaster caster2 = obj.GetComponent<CustomGraphicRaycaster>();
             if (caster2 != null)
             {
                 caster2.enabled = enabled;
                 return true;
             }
             return false;
         }
         
         /// <summary>
         /// 清理内存
         /// </summary>
         public static void ClearMemory()
         {
             GC.Collect();
             Resources.UnloadUnusedAssets();
             LuaModule.Instance.DoStepGC();
         }
        
        public static GameObject GetGameObjectByPath(GameObject root,string path)
        {
            return UIUtil.GetGameObjectByPath(root, path);
        }
        
        public static T ToGameUIComponent<T>(this GameObject go) where T : GameUIComponent
        {
            return UIUtil.ToGameUIComponent<T>(go);
        }

        public static GameUIComponent ToGameUIComponent(this GameObject go)
        {
            return UIUtil.ToGameUIComponent(go);
        }

        public static void LogWithLuaStack(string msg, int errorLevel = 2)
        {
            LuaDLL.LogWithLuaStack(msg, errorLevel);
        }
        
        /// <summary>
        /// 获取推荐画质
        /// </summary>
        /// <returns></returns>
         public static int GetRecommendDeviceInfo()
         {
             return (int)DeviceModule.Instance.RecommendDeviceInfo;
         }
        
         /// <summary>
         /// 设置推荐画质
         /// </summary>
         /// <returns></returns>
         public static void SetRecommendDeviceInfo(int deviceType)
         {
             DeviceModule.Instance.RecommendDeviceInfo = (QualityType)deviceType;
         }

         /// <summary>
         /// 获取当前画质
         /// </summary>
         /// <returns></returns>
         public static int GetDeviceType()
         {
             return (int)DeviceModule.Instance.DeviceType;
         }

         /// <summary>
         /// 设置当前画质
         /// </summary>
         /// <param name="deviceType"></param>
         public static void SetDeviceInfo(int deviceType)
         {
             DeviceModule.Instance.DeviceType = (QualityType)deviceType;
         }
         
         public static void SetTextureLevel(int lvl)
         {
             MURoot.MUQualityMgr.SetTextureLevel(lvl);
         }
        
         public static void LightFaceQuality(int level)
         {
             LightFaceEffect.SetQuality((TerrainXQualityLevel)level);
         }
         
         public static void SystemSetting(int sn)
         {
             if (sn == 1)
             {
                 //--画质流畅
                 SetTextureLevel(1);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Low);
                 DeviceModule.Instance.DeviceType = QualityType.Low;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Low);
             }
             else if (sn == 2)
             {
                 //--画质正常
                 SetTextureLevel(1);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Mid);
                 DeviceModule.Instance.DeviceType = QualityType.Mid;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Low);
             }
             else if (sn == 3)
             {
                 //--画质高清
                 SetTextureLevel(0);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Mid);
                 DeviceModule.Instance.DeviceType = QualityType.High;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Mid);
             }
             else if (sn == 42)
             {
                 //-画质完美
                 SetTextureLevel(0);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.High);
                 DeviceModule.Instance.DeviceType = QualityType.Perfect;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.High);
             }
             else if (sn == 45)
             {
                 //--画质完美 对应在设置界面中的显示应该是 “极致”
                 SetTextureLevel(0);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Wonderful);
                 DeviceModule.Instance.DeviceType = QualityType.AllPerfect;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Wonderful);
             }
             else if (sn == 46)
             {
                 //--画质，最低，保险模式
                 SetTextureLevel(1);
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Safe);
                 DeviceModule.Instance.DeviceType = QualityType.Low;
                 LightFaceEffect.SetQuality(TerrainXQualityLevel.Safe);
             }
         }
         
         public static void InitStepSoundTable_SetRowCnt(int row_cnt)
         {
             StepSoundTable.Instance.SetRowCnt(row_cnt);
         }
         
         public static void InitStepSoundTable_SetData(int idx, int flag, string hero_soundnames, string mount_soundnames)
         {
             StepSoundTable.Instance.SetRowData(idx, flag, hero_soundnames, mount_soundnames);
         }
         
         public static int GetTriggerNameHash(string name)
         {
             return AnimatorHashTool.GetNameHash(name);
         }
         
         public static bool FileExists(string filePath)
         {
             return System.IO.File.Exists(filePath);
         }
         
         public static void WriteTextToFile(string filePath,string content)
         {
             string dirPath = System.IO.Path.GetDirectoryName(filePath);
             if(!System.IO.Directory.Exists(dirPath))
             {
                 System.IO.Directory.CreateDirectory(dirPath);
             }
             //byte[] bytes = System.Text.Encoding.Default.GetBytes(content);
             //FileStream fileStream = new FileStream(filePath, FileMode.Create);
             //fileStream.Write(bytes, 0, bytes.Length);
             //fileStream.Close();
             System.IO.File.WriteAllText(filePath, content);
         }

         
    }
}
#endif