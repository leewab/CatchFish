// using System;
// using System.Collections.Generic;
// using MUEngine;
// public class Render2DResourceMgr
// {
//
//     private static Render2DResourceMgr _instance;
//     public static Render2DResourceMgr Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 _instance = new Render2DResourceMgr();
//                 _instance.OnInit();
//             }
//             return _instance;
//         }
//     }
//
//     public Render2DResourceMgr()
//     {
//
//     }
//
//     private List<Render2DResource> mResourceList = null;
//
//     private List<Render2DResource> mUnUseList = null;
//
//     private void OnInit()
//     {
//         mResourceList = new List<Render2DResource>();
//         mUnUseList = new List<Render2DResource>();
//     }
//     //todo 模型不应该去找相同的 特效应该
//     public T CreateResource<T>(string res) where T : Render2DResource,new()
//     {
//         T t = GetExistResource<T>(res);
//         if (t == null)
//         {
//             t = GetUnuseResource<T>();
//             if (t == null)
//             {
//                 t = new T();
//             }
//             else
//             {
//                 mUnUseList.Remove(t);
//             }
//             mResourceList.Add(t);
//         }
//         return t;
//     }
//
//     public T GetExistResource<T>(string res) where T : Render2DResource, new()
//     {
//         for (int i = 0; i < mResourceList.Count; i++)
//         {
//             T t = mResourceList[i] as T;
//             if(t != null && t.One2Many && t.ResName.Equals(res))
//             {
//                 return t;
//             }
//         }
//         return null;
//     }
//
//     public T GetUnuseResource<T>() where T : Render2DResource, new()
//     {
//         for (int i = 0; i < mUnUseList.Count; i++)
//         {
//             T t = mUnUseList[i] as T;
//             if(t != null)
//             {
//                 return t;
//             }
//         }
//         return null;
//     }
//
//     public void Release(Render2DResource res)
//     {
//         if (mResourceList.Contains(res))
//         {
//             mResourceList.Remove(res);
//         }
//         res.Destroy();
//         mUnUseList.Add(res);
//     }
// }
//
