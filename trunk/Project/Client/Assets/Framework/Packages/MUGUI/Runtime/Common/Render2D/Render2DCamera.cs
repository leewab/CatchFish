// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using MUEngine;
// public class Render2DCamera
// {
//     /// <summary>
//     /// 最大列数
//     /// </summary>
//     protected const int CAMERA_MAX_COL = 10;
//     /// <summary>
//     /// 间距
//     /// </summary>
//     protected const int CAMERA_MAX_SIZE = 500;
//     /// <summary>
//     /// 起始位置
//     /// </summary>
//     protected static Vector3 CAMERA_BASE_POS = new Vector3(-500.0f, -2500.0f, -5000.0f);
//
//     protected List<RawImage> mTextureList = new List<RawImage>();
//     protected GameObject mCamObj = null;
//     protected Camera mCamera = null;
//     protected RenderTexture mRenderTex = null;
//     protected UIPostEffect mPostEffect = null;
//
//     protected int mIndex = 0;
//     public int Index
//     {
//         get
//         {
//             return mIndex;
//         }
//         set
//         {
//             mIndex = value;
//             if (mCamObj != null)
//             {
//                 string camName = string.Format("Res-{0}-Camera", mIndex);
//                 mCamObj.name = camName;
//                 mCamObj.transform.position = AutoSetPos();
//             }
//         }
//     }
//
//     /// <summary>
//     /// 初始化摄像机
//     /// </summary>
//     /// <param name="root">摄像机父节点</param>
//     public void Init(Transform root)
//     {
//         mCamObj = new GameObject();
//         mCamera = mCamObj.AddComponent<Camera>();
//         mCamera.clearFlags = CameraClearFlags.SolidColor;
//         mCamera.backgroundColor = new Color(0, 0, 0, 0);
//         mCamera.cullingMask = 1 << RenderLayerDef.RL_ACTOR;
//         mCamera.cullingMask |= 1 << RenderLayerDef.RL_EFFECT;
//         mCamera.cullingMask |= 1;//武器是default层
//         mCamera.orthographic = true;
//         mCamera.nearClipPlane = 150f;
//         mCamera.farClipPlane = 300f;
//         mCamera.allowHDR = false;
//         mCamObj.transform.parent = root;
//         mCamObj.transform.rotation = Quaternion.Euler(0, 180, 0);
//         mCamObj.SetActive(false);
//         mPostEffect = mCamera.gameObject.AddComponent<UIPostEffect>();
//         if (MURoot.MUQualityMgr.CurQualityType() != QualityType.Height)
//         {
//             SetPostEffectActive(false);
//         }
//     }
//
//     public void SetPostEffectActive(bool bActive)
//     {
//         if (mPostEffect != null)
//         {
//             mPostEffect.enabled = bActive;
//         }
//     }
//
//     /// <summary>
//     /// 自动分配摄像机位置
//     /// </summary>
//     /// <returns>The position.</returns>
//     protected Vector3 AutoSetPos()
//     {
//         int row = mIndex / CAMERA_MAX_COL;
//         int col = mIndex % CAMERA_MAX_COL;
//         Vector3 offset = Vector3.right * CAMERA_MAX_SIZE * col + Vector3.down * CAMERA_MAX_SIZE * row;
//         return CAMERA_BASE_POS + offset;
//     }
//     /// <summary>
//     /// 设置新大小的RenderTexture
//     /// </summary>
//     public void SetRenderTextureSize(int w, int h)
//     {
//         if (mRenderTex != null)
//         {
//             if (mRenderTex.width != w || mRenderTex.height != h)
//             {
//                 RenderTexture.ReleaseTemporary(mRenderTex);
//                 mRenderTex = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32);
//                 mRenderTex.antiAliasing = 1;//QualitySettings.antiAliasing;
//                 mRenderTex.Create();
//                 mCamera.targetTexture = mRenderTex;
//             }
//         }
//         else
//         {
//             mRenderTex = RenderTexture.GetTemporary(w, h, 16, RenderTextureFormat.ARGB32);
//             mRenderTex.antiAliasing = 1;// QualitySettings.antiAliasing;
//             mRenderTex.Create();
//             mCamera.targetTexture = mRenderTex;
//         }
//     }
//     public void AddTexture(RawImage tex)
//     {
//         if (HasTexture(tex))
//             return;
//
//         mTextureList.Add(tex);
//         tex.texture = mRenderTex;
//     }
//
//     public void RemoveTexture(RawImage tex)
//     {
//         if (HasTexture(tex))
//         {
//             mTextureList.Remove(tex);
//             tex.texture = null;
//         }
//     }
//
//     public bool HasTexture(RawImage tex)
//     {
//         return mTextureList.Contains(tex);
//     }
//
//     public bool HasOutTexture
//     {
//         get
//         {
//             return mTextureList.Count > 0;
//         }
//     }
//     /// <summary>
//     /// 看向的物体位置
//     /// </summary>
//     public Vector3 LookAtPos
//     {
//         get
//         {
//             return mCamObj.transform.position + new Vector3(0, 0, -CAMERA_MAX_SIZE * 0.5f);
//         }
//     }
//     /// <summary>
//     /// 启用
//     /// </summary>
//     public void Enable()
//     {
//         if (mCamObj != null)
//         {
//             mCamObj.SetActive(true);
//         }
//     }
//
//     /// <summary>
//     /// 禁用
//     /// </summary>
//     public void Disable()
//     {
//         if (mCamObj != null)
//         {
//             mCamObj.SetActive(false);
//         }
//     }
//
//     /// <summary>
//     /// 设置摄像机大小
//     /// </summary>
//     public float OrthographicSize
//     {
//         get
//         {
//             if (mCamera != null)
//             {
//                 return mCamera.orthographicSize;
//             }
//             return 0f;
//         }
//         set
//         {
//             if (mCamera != null)
//             {
//                 mCamera.orthographicSize = value;
//             }
//         }
//     }
//
//     public float Aspect
//     {
//         get
//         {
//             return mCamera.aspect;
//         }
//     }
//
//     /// <summary>
//     /// 设置摄像机宽高比
//     /// </summary>
//     public void SetCameraAspect(float uiWidth, float uiHeight)
//     {
//         if (mCamera != null)
//         {
//             mCamera.aspect = uiWidth / uiHeight;
//         }
//     }
//
//     public void SetCameraPixelRect(float uiWidth, float uiHeight)
//     {
//         if (mCamera != null)
//         {
//             mCamera.pixelRect = new Rect(uiWidth, 0, uiWidth, uiHeight);
//         }
//     }
//
//     public void Destroy()
//     {
//         for (int i = 0; i < mTextureList.Count; ++i)
//         {
//             mTextureList[i].texture = null;
//         }
//         mTextureList.Clear();
//         if(mRenderTex != null)
//         {
//             mRenderTex.Release();
//             Object.Destroy(mRenderTex);
//             mRenderTex = null;
//         }
//         mCamera.targetTexture = null;
//         Disable();
//     }
// }
//
