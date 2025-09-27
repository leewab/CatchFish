// using Game;
// using Game.Core;
// using UnityEngine;
// using MUEngine;
// using UnityEngine.UI;
//
// namespace MUGame
// {
//     public class SceneRenderer2DHandler : BaseHandler
//     {
//         /// <summary>
//         /// 单例注册
//         /// </summary>
//         public static SceneRenderer2DHandler Instance => HandlerModule.SceneRenderer2DHandler;
//
//
//         public override void Update()
//         {
//             base.Update();
//             if (!IsStarted)
//                 return;
//             //if (MUQualityConfig.Scene2UIScale != _TextureScale * _DynamicResScale)
//             //{
//             //    MUQualityConfig.Scene2UIScale = _TextureScale * _DynamicResScale;
//             //}
//             if (MUQualityConfig.Scene2UIScale != _TextureScale)
//             {
//                 MUQualityConfig.Scene2UIScale = _TextureScale;
//             }
//             //相机被删除了
//             if (_CameraReady && _MainCamera == null)
//             {
//                 DisposeTexture();
//                 //RealDoExecuteDynamicRes(1);
//                 _CameraReady = false;
//             }
//
//             if (!_CameraReady)
//             {
//                 MUCamera muCamera = MURoot.MUCamera;
//                 if (muCamera != null)
//                 {
//                     Camera camera = muCamera.Camera;
//                     if (camera != null && camera.name != "default_camera")
//                     {
//                         _MainCamera = camera;
//                     }
//                 }
//
//                 _CameraReady = _MainCamera != null;
//                 if (_CameraReady)
//                 {
//                     _TextureDirty = true;
//                     //_DynamicResDirty = true;
//                 }
//             }
//
//             if (_CameraReady)
//             {
//                 //bool screenSizeChange = false;
//                 int screenWidth = Screen.width;
//                 int screenHeight = Screen.height;
//                 if (_ScreenSize.x != screenWidth || _ScreenSize.y != screenHeight)
//                 {
//                     _ScreenSize.x = screenWidth;
//                     _ScreenSize.y = screenHeight;
//                     //screenSizeChange = true;
//                     _TextureDirty = true;
//                     //_DynamicResDirty = true;
//                 }
//
//                 //if (screenSizeChange)
//                 //{
//                 //    _UIObj.localScale = new Vector3(_UICamera.aspect * _UICamera.orthographicSize * 2, _UICamera.orthographicSize * 2, 1);
//                 //}
//
//                 if (_TextureDirty)
//                 {
//                     DisposeTexture();
//                     CreateTexture();
//                     _TargetImage.texture = _RenderTex;
//                     _TextureDirty = false;
//                 }
//                 //if (_DynamicResDirty && AllowDynamicRes)
//                 //{
//                 //    RealDoExecuteDynamicRes(_DynamicResScale);
//                 //    _DynamicResDirty = false;
//                 //}
//                 //发现很多时候相机会变
//                 if (_MainCamera.targetTexture == null)
//                 {
//                     _MainCamera.targetTexture = _RenderTex;
//                 }
//             }
//         }
//
//         private bool _Started = false;
//         private RenderTexture _RenderTex = null;
//         private Vector2 _ScreenSize = Vector2.zero;
//         private float _TextureScale = 1f;
//         private bool _TextureDirty = false;
//
//         //private bool AllowDynamicRes = true;
//         //private float _DynamicResScale = 1f;
//         //private bool _DynamicResDirty = false;
//
//         private Camera _MainCamera = null;
//         private bool _CameraReady = false;
//         private RawImage _TargetImage = null;
//         private Material _UIMaterial = null;
//         private GameObject _UIObj = null;
//         //private Camera _UICamera = null;
//         private bool _Ready = false;
//
//         //public void StartWork(float scale, bool isDynamicRes = false, float DynamicResRate = 1f)
//         public void StartWork(float scale)
//         {
//             if (LightFaceEffect.GetQuality() <= TerrainXQualityLevel.High)
//             {
//                 Debug.LogError("移动端最高的两挡配置会自定义主相机target，这里设置会产生冲突和其他隐患，具体联系@jiangboyao");
//                 return;
//             }
//
//             if (IsStarted)
//             {
//                 //ResetTextureSize(scale, isDynamicRes, DynamicResRate);
//                 ResetTextureSize(scale);
//                 return;
//             }
//
//             GameObject uiRoot = GameObject.Find("UI Root");
//             Transform obj = uiRoot.transform.Find("Scene2DUI");
//             if (obj == null)
//             {
//                 return;
//             }
//             //_UICamera = obj.GetComponent<Camera>();
//             _Ready = true;
//             _UIObj = obj.gameObject;
//             _UIObj.SetActive(true);
//             //_UICamera.enabled = true;
//             _Started = true;
//             _CameraReady = false;
//             _TargetImage = obj.GetComponentInChildren<RawImage>();
//             _UIMaterial = Resources.Load<Material>("shader/_Material/Scene2DUI");
//             _TargetImage.material = _UIMaterial;
//
//             //if (!isDynamicRes)
//             //{
//                 _TextureScale = scale;
//                 _TextureDirty = true;
//             //}
//             //else
//             //{
//             //    _DynamicResScale = DynamicResRate;
//             //    _DynamicResDirty = true;
//             //}
//         }
//         public bool IsStarted
//         {
//             get
//             {
//                 return _Started;
//             }
//         }
//         public bool IsReady
//         {
//             get
//             {
//                 return _Ready;
//             }
//         }
//         //public void ResetTextureSize(float scale, bool isDynamicRes = false, float DynamicResRate = 1f)
//         public void ResetTextureSize(float scale)
//         {
//             //if (!isDynamicRes)
//             //{
//                 if (_TextureScale != scale)
//                 {
//                     _TextureScale = scale;
//                     _TextureDirty = true;
//                 }
//             //}
//             //else
//             //{
//             //    _DynamicResScale = DynamicResRate;
//             //    _DynamicResDirty = true;
//             //}
//         }
//
//         public void StopWork()
//         {
//             if (!IsStarted) return;
//             if (!_Ready) return;
//             _Started = false;
//             DisposeTexture();
//             _UIObj.SetActive(false);
//             //RealDoExecuteDynamicRes(1);
//             //_UICamera.enabled = false;
//             MUQualityConfig.Scene2UIScale = 1f;
//         }
//         public void RestartWork()
//         {
//             if (IsStarted) return;
//             if (!_Ready) return;
//             _Started = true;
//             _TextureDirty = true;
//             //_DynamicResDirty = true;
//             _CameraReady = false;
//             //_UICamera.enabled = true;
//             _UIObj.SetActive(true);
//         }
//
//         //public void SetDynamicResEnabled(bool enable)
//         //{
//         //    AllowDynamicRes = enable;
//         //    if (!AllowDynamicRes)
//         //    {
//         //        RealDoExecuteDynamicRes(1f);
//         //    }
//         //}
//
//         private void DisposeTexture()
//         {
//             if (_RenderTex != null)
//             {
//                 if (_MainCamera != null)
//                 {
//                     _MainCamera.targetTexture = null;
//                 }
//                 if(_TargetImage != null)
//                 {
//                     _TargetImage.texture = null;
//                 }
//                 _RenderTex.Release();
//                 UnityEngine.Object.Destroy(_RenderTex);
//                 _RenderTex = null;
//             }
//         }
//         private void CreateTexture()
//         {
//             if (_RenderTex == null)
//             {
//                 _RenderTex = new RenderTexture((int)(Screen.width * _TextureScale), (int)(Screen.height * _TextureScale), 24, RenderTextureFormat.ARGB32);
//                 _RenderTex.name = "SceneRenderer2DMode._RenderTex";
//                 //_RenderTex.antiAliasing = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
//                 _RenderTex.Create();
//             }
//         }
//
//         //private void RealDoExecuteDynamicRes(float rate)
//         //{
//         //    if (_MainCamera != null)
//         //    {
//         //        _MainCamera.rect = new Rect(Vector2.zero, Vector2.one * rate);
//         //    }
//         //    Shader.SetGlobalFloat("_DynamicResRate", rate);
//         //}
//     }
// }
