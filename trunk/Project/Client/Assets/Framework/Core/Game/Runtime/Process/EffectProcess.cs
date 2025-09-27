// using System;
// using UnityEngine;
//
// namespace Game
// {
//     public class EffectProcess : IProcess
//     {
//         public string PreloadDesc { get; }
//         public Action OnFinishedEvent { get; set; }
//
//         public void Start()
//         {
//             LightFaceEffect.isReleaseDebug = false;
// #if UNITY_IPHONE && !UNITY_EDITOR
//             TerrainXUtils.IsRelease = false;
//             Shader.SetGlobalFloat("IS_RELEASE", 3f);
// #elif UNITY_ANDROID && !UNITY_EDITOR
//             TerrainXUtils.IsRelease = false;
//             Shader.SetGlobalFloat("IS_RELEASE", 3f);
// #else
//             TerrainXUtils.IsRelease = true;
//             Shader.SetGlobalFloat("IS_RELEASE", 0.2f);
// #endif
//             TerrainXUtils.IsSimulator = CommonUseSDK.IsSimulator();
//             MUUtility.isSimulator = CommonUseSDK.IsSimulator();
//
//             Shader.globalMaximumLOD = 350;
//
//             Shader.DisableKeyword("_HERO_CUSTOM_REFLECTION");
//             Shader.SetGlobalFloat("_ScreenZRate", 5);
//             Shader.DisableKeyword("SNOW_WEATHER");
//             Shader.SetGlobalFloat("_RainSmoothAdd", 1);
//             Shader.SetGlobalFloat("_GSwing", 1);
//             Shader.SetGlobalFloat("_GWindSpeed", 1);
//             Shader.SetGlobalFloat(GlobalShaderIDs.HDRParamsID, 1);
//             Shader.SetGlobalFloat("_FlagIsOnUI", 0);
//
//             Shader.SetGlobalFloat("_Mobile_FogCube_HDR", 1);
//
//             Shader.SetGlobalFloat("_SceneBrightness", 1f);
//             Shader.SetGlobalFloat("_SceneSpecScale", 1f);
//             Shader.SetGlobalFloat("_ReflectMin", 1f);
//             Shader.SetGlobalFloat("_ReflectMax", 1f);
//             Shader.SetGlobalFloat("_TerrainReflectFresnelFix", 1f);
//             Shader.SetGlobalFloat("_RealTimeDiffuse", 1f);
//             Shader.SetGlobalColor("_RealTimeDiffuseColor", Color.white);
//             Shader.SetGlobalFloat("_RealTimeSun", 1f);
//             Shader.SetGlobalColor("_RealTimeSunColor", Color.white);
//             Shader.SetGlobalFloat("_UEShadowBias", 1f);
//             Shader.SetGlobalFloat("_UEBakeDiffuse", 1f);
//             Shader.SetGlobalColor("_UEBakeDiffuseColor", Color.white);
//             Shader.SetGlobalFloat("_UEBakeSun", 1f);
//             Shader.SetGlobalColor("_UEBakeSunColor", Color.white);
//             Shader.SetGlobalFloat("_UEBakeSun_LOWMULTI", 1f);
//             Shader.SetGlobalFloat("_CharactorDiffuse", 1f);
//             Shader.SetGlobalColor("_CharactorDiffuseColor", Color.white);
//             Shader.SetGlobalFloat("_CharactorSun", 1f);
//             Shader.SetGlobalColor("_CharactorSunColor", Color.white);
//             Shader.SetGlobalFloat("_CharactorReflectionFix", 1f);
//             Shader.SetGlobalColor("_CharactorReflectionFixColor", Color.white);
//             Shader.SetGlobalFloat("_SkinFix", 1f);
//
//             Shader.SetGlobalFloat("_GrassStingLightmapFix", 0f);
//             Shader.SetGlobalFloat("_CustomReflectionFix", 1f);
//
//             Shader.SetGlobalFloat("_FogAddThreshold", 0f);
//             Shader.SetGlobalColor("_FogAddColor", Color.white);
//             Shader.SetGlobalFloat("_FogAddIntensity", 1f);
//             Shader.SetGlobalFloat("_blurIntensityAdd", 1f);
//
//             Shader.SetGlobalFloat(ParticleColorMaskWritter.shaderPropName, ParticleColorMaskWritter.DefaultColorMaskValue);
//
//             Shader.SetGlobalFloat("_singleSceneGlobalDOFIntensity", 0f);
//             Shader.SetGlobalFloat("_singleSceneGlobalPixelFogIntensity", 0f);
//             Shader.SetGlobalFloat("_singleSceneGlobalDepthFogIntensity", 0f);
//
//             //login point light
//             Shader.SetGlobalVector("P1_Color", Vector4.zero);
//             Shader.SetGlobalVector("P1_Pos", Vector4.one * 10000);
//
//             Shader.SetGlobalVector("P2_Color", Vector4.zero);
//             Shader.SetGlobalVector("P2_Pos", Vector4.one * 10000);
//
//             Shader.SetGlobalFloat("_LoginPointLightPowWeight", 1.5f);
//             Shader.SetGlobalFloat("_LoginPointLightIntensity", 0.2f);
//
//             TerrainXUtils.IsEditorProject = false;
//             
//             this.Finish();
//         }
//
//         private void Finish()
//         {
//             this.OnFinishedEvent?.Invoke();
//             this.OnFinishedEvent = null;
//         }
//     }
// }