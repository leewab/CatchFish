// using System.Collections.Generic;
// using UnityEngine.UI;
// using UnityEngine;
// using MUEngine;
// /// <summary>
// /// 渲染对象基类 模型 特效
// /// </summary>
// public class Render2DResource
// {
//     public enum MaterialIdx
//     {
//         DefaultMaterial = 0,
//         UIMaterial = 1,
//         UIMaterialcj2 = 2,
//     }
//     public int mIdxUIMaterial = 1;
//
//     public System.Action onLoadComplete = null;
//     protected static int UID = 0;
//     protected Entity mEntity;
//     protected MUEngine.MUActorEntity mHumanRidingEntity;
//     protected int mUID = 0;
//     protected bool mLoadComplete;
//     protected string mResName = string.Empty;
//     protected bool mOne2Many = true;//资源与摄像机是否是一对多关系
//
//     private bool bSetEntity = false;
//
//     protected List<Render2DCamera> mCameraList = new List<Render2DCamera>();
//     public Render2DResource()
//     {
//         UID++;
//         mUID = UID;
//     }
//     public bool One2Many
//     {
//         get
//         {
//             return mOne2Many;
//         }
//
//         set
//         {
//             mOne2Many = value;
//         }
//     }
//     /// <summary>
//     /// 是否在渲染中
//     /// </summary>
//     public bool IsRendering
//     {
//         get
//         {
//             return mCameraList.Count > 0;
//         }
//     }
//
//     public string ResName
//     {
//         get
//         {
//             return mResName;
//         }
//     }
//     public Entity Entity
//     {
//         get
//         {
//             return mEntity;
//         }
//     }
//     public MUEngine.MUActorEntity HumanRidingEntity
//     {
//         get
//         {
//             return mHumanRidingEntity;
//         }
//     }
//     public int GUID
//     {
//         get
//         {
//             if (mEntity != null)
//             {
//                 return mEntity.ID;
//             }
//             return 0;
//         }
//     }
//     protected virtual void OnLoadRes()
//     {
//         mLoadComplete = true;
//         mEntity.OnLoadResource -= OnLoadRes;
//     }
//
//     public virtual void SetRes(string name)
//     {
//         mResName = name;
//     }
//
//     public virtual void SetEntity(Entity e)
//     {
//         bSetEntity = true;
//         mEntity = e;
//         mResName = e.Name;
//         mEntity.OnLoadResource += OnLoadRes;
//     }
//     public virtual void SetHumanRidingEntity(MUEngine.MUActorEntity e, string bp_name)
//     {
//
//     }
//     //---------------特效可能会把size传过来-------------------
//     public virtual void AddRender(RawImage tex, float camSize, bool forceadd = false)
//     {
//         Rect rect = tex.rectTransform.rect;
//         Render2DCamera rCam = CreateRenderCamera(rect.width, rect.height, camSize, forceadd);
//         rCam.AddTexture(tex);
//     }
//
//     public virtual void AddRender(RawImage tex)
//     {
//         AddRender(tex, 1);
//     }
//
//     public virtual void AddRender(RawImage tex, float w, float h, float camSize)
//     {
//         Render2DCamera rCam = CreateRenderCamera(w, h, camSize);
//         rCam.AddTexture(tex);
//     }
//
//     public GameObject GameObject
//     {
//         get
//         {
//             if(mEntity != null)
//             {
//                 return mEntity.GameObject;
//             }
//             return null;
//         }
//     }
//     /// <summary>
//     /// 移除渲染项
//     /// </summary>
//     public void RemoveRender(RawImage tex)
//     {
//         Render2DCamera rCam = GetRenderCamera(tex);
//         if (rCam != null)
//         {
//             rCam.RemoveTexture(tex);
//
//             if (!rCam.HasOutTexture)
//             {
//                 mCameraList.Remove(rCam);
//                 Render2DCameraMgr.Instance.Release(rCam);
//             }
//
//             if (!IsRendering)
//             {
//                 //Render2DResourceMgr.Instance.Release(this);
//                 Destroy();
//             }
//         }
//     }
//     /// <summary>
//     /// 获取渲染摄像机
//     /// </summary>
//     private Render2DCamera GetRenderCamera(RawImage tex)
//     {
//         for (int i = 0; i < mCameraList.Count; ++i)
//         {
//             Render2DCamera rCam = mCameraList[i];
//             if (rCam.HasTexture(tex))
//             {
//                 return rCam;
//             }
//         }
//         return null;
//     }
//
//     public virtual void Load()
//     {
//         mLoadComplete = false;
//     }
//
//     public bool IsEmpty
//     {
//         get
//         {
//             return mEntity == null;
//         }
//     }
//     private bool mSetComplete = false;
//     public bool SetComplete
//     {
//         set { mSetComplete = value; }
//         get { return mSetComplete; }
//     }
//
//     protected virtual void OnDestory()
//     {
//
//     }
//
//     public void Destroy()
//     {
//         OnDestory();
//         for (int i = 0; i < mCameraList.Count; ++i)
//         {
//             Render2DCameraMgr.Instance.Release(mCameraList[i]);
//         }
//         mCameraList.Clear();
//         mResName = string.Empty;
//         mLoadComplete = false;
//         onLoadComplete = null;
//         mSetComplete = false;
//         if (mEntity != null && !bSetEntity)
//         {
//             // mEntity.OriginalShader();
//             mEntity.OnLoadResource = null;
//             MURoot.Scene.DelEntity(mEntity);
//         }
//         mEntity = null;
//         bSetEntity = false;
//     }
//
//     /// <summary>
//     /// 创建渲染摄像机
//     /// </summary>
//     private Render2DCamera CreateRenderCamera(float uW, float uH, float camSize, bool forceadd = false)
//     {
//         Render2DCamera rCam = GetRenderCamera(uW, uH, camSize);
//         if (rCam == null || forceadd)
//         {
//             rCam = Render2DCameraMgr.Instance.GetRenderCamera();
//             rCam.Index = mUID;
//             rCam.SetCameraAspect(uW, uH);
//             rCam.Enable();
//             mCameraList.Add(rCam);
//         }
//
//         if (rCam != null)
//         {
//             rCam.SetRenderTextureSize((int)uW, (int)uH);
//         }
//
//         return rCam;
//     }
//
//     public bool LoadComplete
//     {
//         get
//         {
//             //已经加载过，不会发通知OnLoadResource
//             if (mLoadComplete == false && mEntity != null && mEntity.GameObject != null)
//             {
//                 //Debug.Log("--------------- 重复加载资源");
//                 mLoadComplete = true;
//                 mEntity.OnLoadResource -= OnLoadRes;
//             }
//             return mLoadComplete;
//         }
//     }
//
//     /// <summary>
//     /// 设置相机size
//     /// </summary>
//     public void SetCameraOrthographicSize(float size, RawImage tex)
//     {
//         for (int i = 0; i < mCameraList.Count; ++i)
//         {
//             if (mCameraList[i].HasTexture(tex))
//             {
//                 mCameraList[i].OrthographicSize = size;
//             }
//         }
//     }
//
//     /// <summary>
//     /// 设位置偏移
//     /// </summary>
//     public void SetEntityPosition(Vector2 pos, RawImage tex)
//     {
//         if (mEntity != null && tex != null)
//         {
//             for (int i = 0; i < mCameraList.Count; ++i)
//             {
//                 if (mCameraList[i].HasTexture(tex))
//                 {
//                     mEntity.Position = mCameraList[i].LookAtPos + new Vector3(pos.x, pos.y, 0f);
//                     break;
//                 }
//             }
//         }
//     }
//
//     /// <summary>
//     /// 获取渲染摄像机
//     /// </summary>
//     private Render2DCamera GetRenderCamera(float uW, float uH, float camSize)
//     {
//         if (!One2Many)
//         {
//             return null;
//         }
//
//         float aspect = uW / uH;
//
//         for (int i = 0; i < mCameraList.Count; ++i)
//         {
//             Render2DCamera rCam = mCameraList[i];
//             if (Mathf.Approximately(rCam.Aspect, aspect))// && Mathf.Approximately(camSize, rCam.OrthographicSize)
//             {
//                 return rCam;
//             }
//         }
//
//         return null;
//     }
// }
//
// public class Render2DModel : Render2DResource
// {
//     AnimatorCullingMode mOriginCullingMode = AnimatorCullingMode.CullUpdateTransforms;
//     
//     protected string bd_name = "";
//
//     public Render2DModel()
//     {
//         mOne2Many = false;
//     }
//
//     public override void SetRes(string name)
//     {
//         base.SetRes(name);
//         mEntity = MURoot.Scene.AddActor();
//         mEntity.Name = name;
//         mEntity.OnLoadResource += OnLoadRes;
//         (mEntity as MUActorEntity).OnLoadedAllElements -= OnLoadedAllElements;
//         (mEntity as MUActorEntity).OnLoadedAllElements += OnLoadedAllElements;
//     }
//     public override void SetEntity(Entity e)
//     {
//         base.SetEntity(e);
//         if (mEntity is MUActorEntity)
//         {
//             (mEntity as MUActorEntity).OnLoadedAllElements -= OnLoadedAllElements;
//             (mEntity as MUActorEntity).OnLoadedAllElements += OnLoadedAllElements;
//         }
//     }
//     public override void SetHumanRidingEntity(MUEngine.MUActorEntity e, string bp_name)
//     {
//         bd_name = bp_name;
//         mHumanRidingEntity = e;
//         mHumanRidingEntity.OnLoadResource -= OnLoadRidingRes;
//         mHumanRidingEntity.OnLoadResource += OnLoadRidingRes;
//         mHumanRidingEntity.OnLoadedAllElements -= OnLoadedAllElements;
//         mHumanRidingEntity.OnLoadedAllElements += OnLoadedAllElements;
//     }
//     public override void Load()
//     {
//         base.Load();
//         if (mEntity != null)
//             mEntity.Load();
//     }
//     protected void OnLoadRidingRes()
//     {
//         resetAnimator(mHumanRidingEntity.GameObject);
//
//         mHumanRidingEntity.Layer = RenderLayerDef.RL_ACTOR;
//         GameObjectUtil.SetLayer(mHumanRidingEntity.GameObject, RenderLayerDef.RL_ACTOR, true);
//         //mHumanRidingEntity.Parent = Render2DCameraMgr.Instance.Root;
//         Animator t = mHumanRidingEntity.GameObject.GetComponent<Animator>();
//         if (t != null)
//         {
//             mOriginCullingMode = t.cullingMode;
//             t.cullingMode = AnimatorCullingMode.AlwaysAnimate;
//         }
//
//         mHumanRidingEntity.CSEntity.RecordOriginalShader();
//         replaceUIMaterial(mHumanRidingEntity.GameObject);
//
//         //(mEntity as MUActorEntity).MountAttachPlayer((mHumanRidingEntity.CSEntity as MUActorEntity), bd_name);
//     }
//
//     protected override void OnLoadRes()
//     {
//         base.OnLoadRes();
//
//         resetAnimator( mEntity.GameObject );
//
//         mEntity.Layer = RenderLayerDef.RL_ACTOR;
//         GameObjectUtil.SetLayer(mEntity.GameObject, RenderLayerDef.RL_ACTOR, false);
//         mEntity.Parent = Render2DCameraMgr.Instance.Root;
//         Animator t = mEntity.GameObject.GetComponent<Animator>();
//         if (t != null)
//         {
//             mOriginCullingMode = t.cullingMode;
//             t.cullingMode = AnimatorCullingMode.AlwaysAnimate;
//         }
//         if (onLoadComplete != null)
//         {
//             onLoadComplete();
//         }
//         mEntity.RecordOriginalShader();
//         replaceUIMaterial(mEntity.GameObject);
//     }
//     private void OnLoadedAllElements()
//     {
//         //Debug.Log("-OnLoadedAllElements-");
//         ////mEntity.RecordOriginalShader();
//         replaceUIMaterial(mEntity.GameObject);
//
//     }
//     private void replaceUIMaterial(GameObject obj)
//     {
//         (mEntity as MUActorEntity).SetMaterialByIdx(this.mIdxUIMaterial);
//         if (mHumanRidingEntity != null)
//         {
//             mHumanRidingEntity.SetMaterialByIdx(this.mIdxUIMaterial);
//         }
//     }
//
//     private void resetAnimator(GameObject obj)
//     {
//         var animator = obj.GetComponent<Animator>();
//         var hashCode = 0;
//         if (null != animator)
//         {
//             hashCode = Animator.StringToHash( "live" );
//             if ( ! animator.GetBool( hashCode ) )
//                 animator.SetBool( hashCode, true );
//
//             hashCode = Animator.StringToHash( "battle" );
//             if ( animator.GetBool( hashCode ) )
//                 animator.SetBool( hashCode, false );
//
//             hashCode = Animator.StringToHash( "movestate" );
//             if ( 0 != animator.GetInteger( hashCode ) )
//                 animator.SetInteger( hashCode, 0 );
//         }
//     }
//
//     private Material getUIMaterial (string name, Material[] arr)
//     {
//         for (int i = 0; i < arr.Length; i++)
//         {
//             if (arr[i].name == name)
//             {
//                 return arr[i];
//             }
//         }
//         return null;
//     }
//
//     public void SetModelRotation(float y)
//     {
//         if (mEntity != null)
//         {
//             mEntity.Rotation = Quaternion.Euler(0, y, 0);
//         }
//     }
//
//     public void SetModelRotation( float x, float y)
//     {
//         if (mEntity != null)
//         {
//             mEntity.Rotation = Quaternion.Euler( x, y, 0 );
//         }
//     }
//
//     public void RotateModel(Vector3 axis, float v)
//     {
//         if (mEntity != null)
//         {
//             mEntity.GameObject.transform.Rotate(axis, v);
//         }
//     }
//
//     public MUGame.LuaEntity AddAttach(string name, string bd)
//     {
//         if (mEntity != null)
//         {
//             Entity en = mEntity.AddAttach(name, bd);
//             if (en != null)
//             {
//                 en.OnLoadResource = () =>
//                 {
//                     en.RecordOriginalShader();
//                     replaceUIMaterial(en.GameObject);
//                 };
//                 en.Load();
//             }
//             return MUGame.LuaEntity.Get(en);
//         }
//         return null;
//     }
//
//     public void RemoveAttach(MUGame.LuaEntity attach)
//     {
//         if (null != attach)
//         {
//             var authAttach = mEntity.GetAttchById(attach.ID);
//             if (null != authAttach)
//             {
//                 mEntity.RemoveAttach(attach.CSEntity);
//             }
//         }
//     }
//
//     protected override void OnDestory()
//     {
//         base.OnDestory();
//         if (mEntity != null)
//         {
//             Animator t = null;
//             if (mEntity.GameObject != null)
//             {
//                 t = mEntity.GameObject.GetComponent<Animator>();
//                 (mEntity as MUActorEntity).SetMaterialByIdx(0);
//             }
//             if (t != null)
//             {
//                 t.cullingMode = mOriginCullingMode;
//             }
//             (mEntity as MUActorEntity).StopAct();
//             mEntity.OriginalShader();
//         }
//         if(mHumanRidingEntity != null)
//         {
//             mHumanRidingEntity.CSEntity.OriginalShader();
//             if (mEntity != null)
//             {
//                 (mEntity as MUActorEntity).MountRemovePlayer((mHumanRidingEntity.CSEntity as MUActorEntity));
//             }
//         }
//     }
// }
//
// public class Render2DEffect : Render2DResource
// {
//     protected UIEffectScale mScaleComponent;
//     public override void SetRes(string name)
//     {
//         base.SetRes(name);
//         mEntity = MURoot.Scene.AddEffect(mResName, Vector3.zero, Quaternion.identity);
//         mEntity.Name = name;
//         mEntity.OnLoadResource += OnLoadRes;
//     }
//
//     protected override void OnLoadRes()
//     {
//         base.OnLoadRes();
//         mEntity.Layer = RenderLayerDef.RL_EFFECT;
//         GameObjectUtil.SetLayer(mEntity.GameObject, RenderLayerDef.RL_EFFECT, true);
//         mEntity.Parent = Render2DCameraMgr.Instance.Root;
//         mEntity.Scale = Vector3.one;
//         mScaleComponent = mEntity.GameObject.GetComponent<UIEffectScale>();
//         if (mScaleComponent == null)
//         {
//             mScaleComponent = mEntity.GameObject.AddComponent<UIEffectScale>();
//         }
//         mScaleComponent.ResetLifeTime();
//         mScaleComponent.AutoHideCallback = AutoHide;
//         if (onLoadComplete != null)
//         {
//             onLoadComplete();
//         }
//     }
//
//     private void AutoHide()
//     {
//         this.Destroy();
//     }
//
//     public void SetScale(float scale)
//     {
//         if(mScaleComponent != null)
//         {
//             mScaleComponent.Scale = scale;
//         }
//     }
//
//     public void SetAutoHide(bool value)
//     {
//         if (mScaleComponent != null)
//         {
//             mScaleComponent.mAutoHide = value;
//         }
//     }
//
//     public void SetPlayCount(int count)
//     {
//         if(mScaleComponent != null)
//         {
//             mScaleComponent.mPlayCount = count;
//         }
//     }
//
//     public void SetDirty()
//     {
//         if(mScaleComponent != null)
//         {
//             mScaleComponent.Dirty = true;
//         }
//     }
//
//     public override void Load()
//     {
//         base.Load();
//         if (!MURoot.MUQualityMgr.CanCacheObject)
//             return;
//         if (mEntity != null)
//             mEntity.Load();
//     }
//
//     protected override void OnDestory()
//     {
//         base.OnDestory();
//         MURoot.ResMgr.RemoveAsset(mResName);
//     }
// }
//
