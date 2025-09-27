// using System.Collections.Generic;
// using UnityEngine;
// public class Render2DCameraMgr
// {
//     private static Render2DCameraMgr _instance = null;
//     public static Render2DCameraMgr Instance
//     {
//         get
//         {
//             if(_instance == null)
//             {
//                 _instance = new Render2DCameraMgr();
//                 _instance.OnInit();
//             }
//             return _instance;
//         }
//     }
//
//     private List<Render2DCamera> mUnUseList = null;
//     private List<Render2DCamera> mAllList = null;
//     private Transform mRoot = null;
//     public void OnInit()
//     {
//         GameObject obj = new GameObject("RenderTexture");
//         GameObject.DontDestroyOnLoad(obj);
//         mRoot = obj.transform;
//         mRoot.position = Vector3.zero;
//         mRoot.rotation = Quaternion.identity;
//         mUnUseList = new List<Render2DCamera>();
//         mAllList = new List<Render2DCamera>();
//     }
//
//     public Transform Root
//     {
//         get
//         {
//             return mRoot;
//         }
//     }
//
//     public Render2DCamera GetRenderCamera()
//     {
//         Render2DCamera rCam = null;
//         if (mUnUseList.Count > 0)
//         {
//             rCam = mUnUseList[0];
//             mUnUseList.RemoveAt(0);
//         }
//         else
//         {
//             rCam = new Render2DCamera();
//             rCam.Init(mRoot);
//             mAllList.Add(rCam);
//         }
//         return rCam;
//     }
//
//     public void Release(Render2DCamera rCam)
//     {
//         rCam.Destroy();
//         mUnUseList.Add(rCam);
//     }
//
//     public void SetAllCameraPostEffectActive(bool bActive)
//     {
//         for( int i = 0; i < mAllList.Count; i++)
//         {
//             mAllList[i].SetPostEffectActive(bActive);
//         }
//     }
// }
//
