// using System;
// using Game.UI;
// using UnityEngine.UI;
// using UnityEngine;
// using MUEngine;
//
// namespace MUGame
// {
//     public class GameRender2DComponent : GameUIComponent
//     {
//         public Action onLoadComplete = null;
//         protected Render2DResource mRes = null;
//         protected string mResName = string.Empty;
//         protected RawImage mUITex = null;
//         protected RawImage mUITexShadow = null;
//
//         protected override void OnInit()
//         {
//             base.OnInit();
//             mUITex = gameObject.GetComponent<RawImage>();
//             MUGUI.UIActorShadow uiShadow = gameObject.GetComponent<MUGUI.UIActorShadow>();
//             if (uiShadow != null)
//             {
//                 mUITexShadow = uiShadow.ShadowRawImage;
//             }
//         }
//
//         public string PrefabName
//         {
//             get { return mResName; }
//         }
//
//         public float TexAlpha
//         {
//             get { if (mUITex != null) return mUITex.color.a; else return 0; }
//             set { if (mUITex != null) mUITex.color = new Color(mUITex.color.r, mUITex.color.g, mUITex.color.b, value); }
//         }
//         public int IdxUIMaterial
//         {
//             get;
//             set;
//         }
//         public virtual void Load(string pname)
//         {
//
//         }
//
//         public virtual void LoadEntity(Entity e)
//         {
//
//         }
//         public void SetHumanRidingEntity(MUEngine.MUActorEntity ridingEntity, string bd_name)
//         {
//             if (mRes != null)
//             {
//                 mRes.SetHumanRidingEntity(ridingEntity, bd_name);
//             }  
//         }
//         protected virtual void Load<T>(string pname, Entity e = null) where T : Render2DResource, new()
//         {
//             if (e != null) pname = e.Name;
//             if (mRes == null)
//             {
//                 mRes = new T();
//             }
//             mRes.mIdxUIMaterial = Math.Max(1, this.IdxUIMaterial);
//             if (mResName.Equals(pname))
//             {
//                 if (mRes.LoadComplete)
//                 {
//                     if (onLoadComplete != null) onLoadComplete();
//                 }
//                 return;
//             }
//             mRes.RemoveRender(mUITex);
//             if (mUITexShadow != null)
//             {
//                 mRes.RemoveRender(mUITexShadow);
//             }
//
//             mResName = pname;
//             if (mRes.IsEmpty)
//             {
//                 if (e != null) mRes.SetEntity(e);
//                 else mRes.SetRes(mResName);
//             }
//             mRes.AddRender(mUITex, GetDefaultCameraSize());
//             if (mUITexShadow != null)
//             {
//                 mRes.One2Many = true;
//                 mRes.AddRender(mUITexShadow, GetDefaultCameraSize(), true);
//             }
//             mRes.Load();
//
//             if (mRes.LoadComplete)
//             {
//                 OnLoadComplete();
//             }
//             else
//             {
//                 mRes.onLoadComplete = OnLoadComplete;
//             }
//         }
//         public void SetLoadedEntity(Entity e)
//         {
//             if (e == null) return;
//             if (mRes == null)
//             {
//                 mRes = new Render2DModel();
//             }
//             mRes.mIdxUIMaterial = Math.Max(1, this.IdxUIMaterial);
//             string pname = e.Name;
//             if (mResName.Equals(pname))
//             {
//                 if (mRes.LoadComplete)
//                 {
//                     if (onLoadComplete != null) onLoadComplete();
//                 }
//                 return;
//             }
//             mRes.RemoveRender(mUITex);
//             if (mUITexShadow != null)
//             {
//                 mRes.One2Many = true;
//                 mRes.RemoveRender(mUITexShadow);
//             }
//
//             mResName = pname;
//             if (mRes.IsEmpty)
//             {
//                 mRes.SetEntity(e);
//             }
//             mRes.AddRender(mUITex, GetDefaultCameraSize());
//             if (mUITexShadow != null)
//             {
//                 mRes.AddRender(mUITexShadow, GetDefaultCameraSize());
//             }
//             if (mRes.LoadComplete)
//             {
//                 OnLoadComplete();
//             }
//             else
//             {
//                 mRes.onLoadComplete = OnLoadComplete;
//             }
//         }
//         protected virtual float GetDefaultCameraSize()
//         {
//             return 1;
//         }
//
//         protected virtual void OnLoadComplete()
//         {
//             AutoSet();
//             if (onLoadComplete != null)
//             {
//                 onLoadComplete();
//             }
//         }
//
//         public bool LoadComplete
//         {
//             get
//             {
//                 if(mRes != null)
//                 {
//                     return mRes.LoadComplete;
//                 }
//                 return false;
//             }
//         }
//
//         public virtual void AutoSet()
//         {
//
//         }
//
//         public void SetCameraSize(float camSize)
//         {
//             if (mRes != null)
//             {
//                 mRes.SetCameraOrthographicSize(camSize, mUITex);
//                 mRes.SetCameraOrthographicSize(camSize, mUITexShadow);
//             }
//         }
//
//         public void SetEntityRelativePos(Vector2 relativePos)
//         {
//             if (mRes != null)
//             {
//                 mRes.SetEntityPosition(relativePos, mUITex);
//             }
//         }
//
//         public virtual void DisposeModel()
//         {
//             if (mUITex == null)
//                 return;
//             if (mRes != null)
//             {
//                 if (mUITexShadow != null)
//                 {
//                     mRes.RemoveRender(mUITexShadow);
//                 }
//                 mRes.onLoadComplete = null;
//                 mRes.RemoveRender(mUITex);
//             }
//             if (onLoadComplete != null)
//             {
//                 onLoadComplete = null;
//             }
//
//             mResName = string.Empty;
//         }
//
//         public override void Dispose()
//         {
//             base.Dispose();
//             DisposeModel();
//             mRes = null;
//         }
//     }
// }
//
