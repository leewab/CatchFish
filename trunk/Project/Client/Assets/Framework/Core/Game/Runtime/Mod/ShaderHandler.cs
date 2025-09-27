// using Game;
// using Game.Core;
// using UnityEngine;
// using MUEngine;
//
// namespace MUGame
// {
//     public class ShaderHandler : BaseHandler
//     {
//         /// <summary>
//         /// 单例注册
//         /// </summary>
//         public static ShaderHandler Instance => HandlerModule.ShaderHandler;
//
//
//         private const string Material_Path = "shader/_Material/";
//         private LightFaceEffect _lightEffect = null;
//         /// <summary>
//         /// 低血量shader
//         /// </summary>
//         //private Material _lowHpMaterial = null;
//         //private bool _bLowHpPlay = false;
//         //private Texture2D _maskTexture = null;
//         //public void StartLowHpShader()
//         //{
//         //    if (_bLowHpPlay)
//         //    {
//         //        return;
//         //    }
//         //    if (_lowHpMaterial == null)
//         //    {
//         //        Shader shader = Shader.Find("GOE/ImageEffect/LowHP");
//         //        _lowHpMaterial = new Material(shader);
//         //    }
//         //    //if (_maskTexture == null)
//         //    //{
//         //    //    _maskTexture = Resources.Load("low_hp_mask") as Texture2D;
//         //    //}
//
//         //    getLigtEffect().AddImageEffect(lowHpEffect);
//         //    _bLowHpPlay = true;
//         //}
//
//         //private void lowHpEffect(RenderTexture source, RenderTexture destination)
//         //{
//         //    //_lowHpMaterial.SetTexture("_Mask", _maskTexture);
//         //    Graphics.Blit(source, source, _lowHpMaterial);
//         //}
//
//         //public void StopLowHpShader()
//         //{
//         //    if (!_bLowHpPlay)
//         //    {
//         //        return;
//         //    }
//         //    getLigtEffect().RemoveImageEffect(lowHpEffect);
//         //    _bLowHpPlay = false;
//         //}
//
//         //private bool _bGray = false;
//         //private Material _GrayMaterial = null;
//         //public void ShowGrayShader()
//         //{
//         //    if (_bGray)
//         //    {
//         //        return;
//         //    }
//
//         //    _bGray = true;
//
//         //    if (_GrayMaterial == null)
//         //    {
//         //        Shader shader = Shader.Find("GOE/Gray");
//         //        _GrayMaterial = new Material(shader);
//         //    }
//         //    getLigtEffect().AddImageEffect();
//         //}
//
//         //public void HideGrayShader()
//         //{
//         //    if (!_bGray)
//         //    {
//         //        return;
//         //    }
//         //}
//
//
//         /// <summary>
//         /// 高级动态模糊效果
//         /// </summary>
//         /// 
//         private Material _highMotionBlurMaterial = null;
//         private bool _bHighMotionBlur = false;
//         private Vector3 oldActorPos;
//         //private Vector3 oldCamPos;
//         private bool _IsFirstFrame = true;
//
//         public void StartHighMotionBlurShader() {
//             if (_bHighMotionBlur) {
//                 return;
//             }
//             if (!_lightEffect) {
//                 _lightEffect = MURoot.Scene.LightFaceEffect;
//             }
//             if (!_highMotionBlurMaterial) {
//                 _highMotionBlurMaterial = Resources.Load<Material>(Material_Path + "HighMotionBlur");
//             }
//             _lightEffect.AddImageEffect(HighmotionBlurEffect);
//             _bHighMotionBlur = true;
//             _IsFirstFrame = true;
//         }
//
//         private void HighmotionBlurEffect(RenderTexture source, RenderTexture destination) {
//             if (_lightEffect == null)
//                 return;
//             RenderTexture rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
//             rt.name = "ShaderMode.HighmotionBlurEffect.rt";
//             Graphics.Blit(source, rt);
//             if (_IsFirstFrame) {
//                 _highMotionBlurMaterial.SetVector("_OffsetCam", Vector4.zero);
//             } else {
//                 Camera cam = MURoot.MUCamera.Camera;
//                 Vector3 p1 = cam.WorldToScreenPoint(oldActorPos);
//                 Vector3 p2 = cam.WorldToScreenPoint(MURoot.MUCamera.LookAtTarget.GameObject.transform.position);
//                 Vector3 v = p2 - p1;
//                 _highMotionBlurMaterial.SetFloat("_Value", v.magnitude * 0.001f);
//                 _highMotionBlurMaterial.SetVector("_OffsetCam", v.normalized);
//             }
//             _highMotionBlurMaterial.SetTexture("_MainTex", rt);
//             Graphics.Blit(rt, destination, _highMotionBlurMaterial);
//             RenderTexture.ReleaseTemporary(rt);
//
//             //MURoot.MUCamera.LookAtTarget.GameObject.transform;
//             if (_IsFirstFrame) _IsFirstFrame = false;
//             oldActorPos = MURoot.MUCamera.LookAtTarget.GameObject.transform.position;
//             //oldCamPos = MURoot.MUCamera.Camera.transform.position;
//         }
//
//         public void StopHighMotionBlurShader() {
//             if (_bHighMotionBlur) {
//                 _lightEffect.RemoveImageEffect(HighmotionBlurEffect);
//                 _bHighMotionBlur = false;
//                 _IsFirstFrame = true;
//             }
//         }
//
//         /// <summary>
//         /// 动态模糊效果
//         /// </summary>
//         ///        
//         private Material _motionBlurMaterial = null;
//         private bool _bMotionBlur = false;
//         private RenderTexture _lastRenderTexture = null;
//
//         public void StartMotionBlurShader()
//         {
//             if (_bMotionBlur)
//             {
//                 return;
//             }
//             if (!_lightEffect)
//             {
//                 _lightEffect = MURoot.Scene.LightFaceEffect;
//             }
//
//             if (!_motionBlurMaterial)
//             {
//                 _motionBlurMaterial = Resources.Load<Material>(Material_Path + "MotionBlur");
//             }
//
//             _lightEffect.AddImageEffect(motionBlurEffect);
//             _bMotionBlur = true;
//         }
//
//         /// <summary>
//         /// 参数值域[0, 1]
//         /// </summary>
//         /// <param name="strength"></param>
//         public void MotionBlurStrength(float strength) {
//             if (!_motionBlurMaterial) return;
//             _motionBlurMaterial.SetFloat("_Strength", strength * 2 - 1);
//         }
//         
//         private void motionBlurEffect(RenderTexture source, RenderTexture destination)
//         {
//             if (_lightEffect == null)
//                 return;
//
//             if (!_lastRenderTexture) {
//                 RenderTexture.DestroyImmediate(_lastRenderTexture);
//                 _lastRenderTexture = new RenderTexture(source.width, source.height, 0);
//                 _lastRenderTexture.name = "ShaderMode._lastRenderTexture";
//                 _lastRenderTexture.hideFlags = HideFlags.HideAndDontSave;
//                 Graphics.Blit(source, _lastRenderTexture);
//             }
//             
//             _motionBlurMaterial.SetTexture("_MainTex", _lastRenderTexture);
//             _lastRenderTexture.MarkRestoreExpected();
//
//             Graphics.Blit(source, _lastRenderTexture, _motionBlurMaterial);
//             Graphics.Blit(_lastRenderTexture, destination);
//         }
//
//         public void StopMotionBlurShader() {
//             if (_bMotionBlur) {
//                 _lightEffect.RemoveImageEffect(motionBlurEffect);
//                 _bMotionBlur = false;
//             }
//             if (_lastRenderTexture) {
//                 RenderTexture.DestroyImmediate(_lastRenderTexture);
//             }
//         }
//
//         /// <summary>
//         /// UI图按照PS的柔光方式来叠加的效果
//         /// 柔光公式：gl_FragColor = 2.0 * baseColor * blendColor + baseColor*baseColor -2.0*baseColor*baseColor*blendColor;
//         /// type = 0: 柔光
//         /// type = 1: 正片叠底
//         /// type = 2: 叠加
//         /// </summary>
//         ///        
//         private Material _softLightMaterial = null;
//         private bool _bsoftLight = false;
//
//         public void StartSoftLight(int type)
//         {
//             if (_bsoftLight)
//             {
//                 return;
//             }
//             if (!_softLightMaterial)
//             {
//                 _softLightMaterial = Resources.Load<Material>(Material_Path + "SoftLight");
//             }
//
//             if (_softLightMaterial != null)
//             {
//                 _softLightMaterial.DisableKeyword("_Softlight");
//                 _softLightMaterial.DisableKeyword("_Multiply");
//                 _softLightMaterial.DisableKeyword("_Add");
//                 if (type == 0)
//                 {
//                     _softLightMaterial.EnableKeyword("_Softlight");
//                 }
//                 else if (type == 1)
//                 {
//                     _softLightMaterial.EnableKeyword("_Multiply");
//                 }
//                 else
//                 {
//                     _softLightMaterial.EnableKeyword("_Add");
//                 }
//             }
//
//             if (!_lightEffect)
//             {
//                 _lightEffect = MURoot.Scene.LightFaceEffect;
//             }
//             _lightEffect.AddImageEffect(softLightEffect);
//             _bsoftLight = true;
//         }
//
//         /// <summary>
//         /// 参数值域[0, 1]
//         /// </summary>
//         /// <param name="strength"></param>
//         public void SoftLightStrength(float strength)
//         {
//             if (!_softLightMaterial) return;
//             _softLightMaterial.SetFloat("_Strength", strength * 2 - 1);
//         }
//
//         private void softLightEffect(RenderTexture source, RenderTexture destination)
//         {
//             if (source == destination)
//             {
//                 RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height, source.depth, source.format);
//                 temp.name = "ShaderMode.softLightEffect.temp";
//                 Graphics.Blit(source, temp, _softLightMaterial);
//                 Graphics.Blit(temp, destination);
//                 RenderTexture.ReleaseTemporary(temp);
//             }
//             else
//             {
//                 Graphics.Blit(source, destination, _softLightMaterial);
//             }
//         }
//
//         public void StopSoftLight()
//         {
//             if (_bsoftLight)
//             {
//                 _bsoftLight = false;
//                 _lightEffect.RemoveImageEffect(softLightEffect);
//             }
//         }
//
//         /// <summary>
//         /// 毛玻璃效果
//         /// </summary>
//         ///        
//         private Material _blendrMaterial = null;
//         private bool _bblend = false;
//         //private Texture2D _blendMaskTexture = null;
//
//         public void StartBlendShader()
//         {
//             if (DefaultQualityChecking.Instance.IsMaliGPU())
//                 return;
//             if (_bblend)
//             {
//                 return;
//             }
//             //if (!_blendMaskTexture)
//             //{
//             //    _blendMaskTexture = Resources.Load("maoboli") as Texture2D;
//             //}
//             if (!_blendrMaterial)
//             {
//                 //Shader shader = Shader.Find("Hidden/Blend");
//                 //_blendrMaterial = new Material(shader);
//                 _blendrMaterial = Resources.Load<Material>(Material_Path + "Maoboli");
//             }
//             if (!_lightEffect)
//             {
//                 _lightEffect = MURoot.Scene.LightFaceEffect;
//             }
//             _lightEffect.AddImageEffect(blendEffect);
//             _bblend = true;
//         }
//
//         private void blendEffect(RenderTexture source, RenderTexture destination)
//         {
//             //if (_lightEffect == null)
//             //    return;
//
//             if (source == destination) {
//                 RenderTexture temp = RenderTexture.GetTemporary(source.width / 4, source.height / 4, source.depth, source.format);
//                 temp.name = "ShaderMode.BlendEffect.temp";
//                 Graphics.Blit(source, temp, _blendrMaterial);
//                 Graphics.Blit(temp, destination);
//                 RenderTexture.ReleaseTemporary(temp);
//             } else {
//                 Graphics.Blit(source, destination, _blendrMaterial);
//             }
//         }
//
//         public void StopBlendShader()
//         {
//             if (DefaultQualityChecking.Instance.IsMaliGPU())
//                 return;
//             if (_bblend)
//             {
//                 _bblend = false;
//                 _lightEffect.RemoveImageEffect(blendEffect);
//             }
//         }
//
//
//         /// <summary>
//         /// 黑白效果
//         /// </summary>
//         ///        
//         private Material _grayMaterial = null;
//
//         public void StartGrayShader()
//         {
//             if (!_grayMaterial)
//             {
//                 //Shader shader = Shader.Find("GOE/Gray");
//                 //_grayMaterial = new Material(shader);
//                 _grayMaterial = Resources.Load<Material>(Material_Path + "Gray");
//             }
//             if (!_lightEffect)
//             {
//                 _lightEffect = MURoot.Scene.LightFaceEffect;
//             }
//             _lightEffect.AddImageEffect(GrayEffect);
//             //_bblend = true;
//         }
//
//         private void GrayEffect(RenderTexture source, RenderTexture destination)
//         {
//             //if (_lightEffect == null)
//             //    return;
//             if (source == destination) {
//                 RenderTexture temp = RenderTexture.GetTemporary(source.width / 4, source.height / 4, source.depth, source.format);
//                 temp.name = "ShaderMode.GrayEffect.temp";
//                 Graphics.Blit(source, temp, _grayMaterial);
//                 Graphics.Blit(temp, destination);
//                 RenderTexture.ReleaseTemporary(temp);
//             }
//             else {
//                 Graphics.Blit(source, destination, _grayMaterial);
//             }
//         }
//
//         public void StopGrayShader()
//         {
//             _lightEffect.RemoveImageEffect(GrayEffect);
//
//         }
//
//         /// <summary>
//         /// 屏幕涟漪效果
//         /// </summary>
//         /// 
//         private Material _waterWaveMat = null;
//         private float _lastTime = 0;
//         private float _timeLength = 0;
//
//         public void StartWaterWaveShader() {
//             if (_waterWaveMat == null) {
//                 Shader shader = Shader.Find("GOE/ImageEffect/WaterWaveEffect");
//                 _waterWaveMat = new Material(shader);
//             }
//
//             if (_lightEffect == null) {
//                 _lightEffect = GameObject.FindObjectOfType<LightFaceEffect>();
//                 if (_lightEffect == null)
//                     return;
//             }
//
//             float time = Time.timeSinceLevelLoad;
//             _waterWaveMat.SetFloat("_CurTime", time);
//             _lastTime = time;
//             _waterWaveMat.SetTexture("_FadeInTex", _lightEffect.ScreenShot);
//
//             _lightEffect.AddImageEffect(WaterWaveEffect);
//         }
//
//         public void SetWaterWaveProperty(float waveTime, float fadeOutTime) {
//             if (_waterWaveMat == null) {
//                 Shader shader = Shader.Find("GOE/ImageEffect/WaterWaveEffect");
//                 _waterWaveMat = new Material(shader);
//             }
//
//             _waterWaveMat.SetFloat("_WaveTime", waveTime);
//             _waterWaveMat.SetFloat("_FadeOutTime", fadeOutTime);
//             _timeLength = waveTime + fadeOutTime;
//         }
//
//         public void SetWaterWaveFadeOutTexture(Texture tex) {
//             if (_waterWaveMat == null) {
//                 Shader shader = Shader.Find("GOE/ImageEffect/WaterWaveEffect");
//                 _waterWaveMat = new Material(shader);
//             }
//
//             //_waterWaveMat.SetTexture("_FadeOutTex", tex);
//         }
//
//         private void WaterWaveEffect(RenderTexture source, RenderTexture destination) {
//             if (Time.timeSinceLevelLoad - _lastTime >= _timeLength)
//                 StopWaterWaveEffect();
//
//             _waterWaveMat.SetTexture("_MainTex", source);
//             Graphics.Blit(source, destination, _waterWaveMat);
//
//         }
//
//         public void StopWaterWaveEffect() {
//             _lightEffect.RemoveImageEffect(WaterWaveEffect);
//         }
//
//         /// <summary>
//         /// NPC Mask ?
//         /// </summary>
//         /// 
//         Material _npcMask = null;
//         public Material NpcMaskMaterial
//         {
//             get {
//                 if (_npcMask == null)
//                 {
//                     _npcMask = Resources.Load(Material_Path + "UIMask") as Material;
//                 }
//                 return _npcMask;
//             }
//         }
//
//         public override void Dispose() {
//             if (_lastRenderTexture) {
//                 RenderTexture.DestroyImmediate(_lastRenderTexture);
//             }
//         }
//     }
//
// }
