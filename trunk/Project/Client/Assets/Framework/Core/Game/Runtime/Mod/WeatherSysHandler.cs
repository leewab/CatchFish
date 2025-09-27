// using MUEngine;
// using System;
// using System.Collections.Generic;
// using Game;
// using Game.Core;
// using UnityEngine;
//
//
// namespace MUGame
// {
//     public class WeatherSysHandler : BaseHandler
//     {
//         /// <summary>
//         /// 单例注册
//         /// </summary>
//         public static WeatherSysHandler Instance => HandlerModule.WeatherSysHandler;
//
//         /// <summary>
//         /// 进入场景
//         /// </summary>
//         /// <param name="time_ofday"></param>
//         /// <param name="init_weather"></param>
//         /// <param name="enter_time"></param>
//         public static void EnterStage( string prefab_name, float time_ofday, string init_weather, float enter_time)
//         {
//             WeatherSysHandler.Instance.OnEnterStage( prefab_name, time_ofday, init_weather, enter_time);
//         }
//         /// <summary>
//         /// 进入场景
//         /// </summary>
//         /// <param name="time_ofday"></param>
//         public static void EnterStage(string prefab_name, float time_ofday)
//         {
//             WeatherSysHandler.Instance.OnEnterStage( prefab_name, time_ofday);
//         }
//         public static void InitTimeOnly( float time_ofday )
//         {
//             WeatherSysHandler.Instance.OnInitTimeOnly(time_ofday);
//         }
//         /// <summary>
//         /// 改变天气
//         /// </summary>
//         /// <param name="weather"></param>
//         public static void ChangeWeather(string weather)
//         {
//             WeatherSysHandler.Instance.OnChangeWeather(weather);
//         }
//         public static void LeaveStage()
//         {
//             WeatherSysHandler.Instance.OnLeaveStage();
//         }
//         private void OnEnterStage( string prefab_name, float time_ofday, string init_weather, float enter_time)
//         {
//             InitWeatherObj( prefab_name, delegate {
//                 if (mWeather == null)
//                 {
//                     return;
//                 }
//                 mWeather.OnEnterStage(time_ofday, init_weather, enter_time);
//                 mInited = true;
//             } );
//             
//         }
//         private void OnEnterStage( string prefab_name, float time_ofday)
//         {
//             InitWeatherObj(prefab_name, delegate {
//                 if (mWeather == null)
//                 {
//                     return;
//                 }
//                 mWeather.OnEnterStage(time_ofday);
//                 mInited = true;
//             });
//             
//         }
//         private void OnInitTimeOnly( float time_ofday )
//         {
//             InitWeatherObj("", delegate {
//                 if (mWeather == null)
//                 {
//                     return;
//                 }
//                 mWeather.OnEnterStage(time_ofday);
//                 //禁用更新
//                 mInited = false;
//             });
//             
//         }
//         private void OnChangeWeather(string weather)
//         {            
//             mLoader.DoFunc(delegate
//             {
//                 if (mWeather == null || !mInited)
//                 {
//                     return;
//                 }
//                 mWeather.ChangeWeather(weather);
//             });            
//         }       
//         public override void Update()
//         {
//             base.Update();
//             if (!mInited)
//             {
//                 return;
//             }
//             if (mWeather != null)
//             {
//                 //mWeather.OnUpdate();
//             }
//             
//         }
//         private void OnLeaveStage()
//         {            
//             if (mWeather != null)
//             {
//                 mWeather.OnLeaveScene();
//                 mWeather = null;
//             }
//             mLoader.Release();
//             mInited = false;
//             
//         }
//     
//         private void InitWeatherObj( string prefab_name, Action callback )
//         {
//             if (prefab_name == "")
//             {
//                 mWeather = null;
//                 return;
//             }
//             mLoader.LoadAsset(prefab_name, delegate {
//                 GameObject weather_obj = mLoader.GetRes() as GameObject;
//                 mWeather = weather_obj.GetComponent<WeatherDynamic>();
//                 //WeatherDynamic.IsAutoUpdate = false;
//                 if (callback != null)
//                 {
//                     callback();
//                 }
//
//             });                   
//         }
//
//         private WeatherDynamic mWeather;
//         private bool mInited = false;
//         private BundleResLoader mLoader = new BundleResLoader();
//     }
//
//     class BundleResLoader
//     {
//         public BundleResLoader()
//         { }
//         public void Init()
//         { }
//
//         public void Release()
//         {
//             NextID();
//             if (mLoadedObj != null)
//             {
//                 MUEngine.MURoot.ResMgr.ReleaseAsset(mResName, mLoadedObj);
//                 mLoadedObj = null;
//             }
//             mResName = "";
//             mOpList.Clear();
//             mLoading = false;
//
//         }
//         public void LoadAsset(string res_name, Action callback)
//         {            
//             mResName = res_name;
//             mLoading = true;
//             int id = NextID();
//             MUEngine.MURoot.ResMgr.GetAsset(res_name, delegate (string name, UnityEngine.Object obj) {
//                 if (id != mID)
//                 {
//                     MUEngine.MURoot.ResMgr.ReleaseAsset(name, obj);
//                     return;
//                 }
//                 mLoadedObj = obj;
//                 mLoading = false;
//                 if (callback != null)
//                 {
//                     callback();
//                 }
//                 FlushOps();
//             }, LoadPriority.MostPrior);
//         }
//         
//         
//         public void DoFunc(Action func)
//         {
//             if (mLoading)
//             {
//                 mOpList.Add(func);
//             }
//             else
//             {
//                 func();
//             }
//         }
//         public UnityEngine.Object GetRes()
//         {
//             return mLoadedObj;
//         }
//         private void FlushOps()
//         {
//             foreach (Action func in mOpList)
//             {
//                 func();
//             }
//             mOpList.Clear();
//         }
//         private int NextID()
//         {
//             return ++mID;
//         }
//         private List<Action> mOpList = new List<Action>();
//         private int mID = 0;
//         private UnityEngine.Object mLoadedObj;
//         private string mResName = "";
//         private bool mLoading = false;
//     }
// }
