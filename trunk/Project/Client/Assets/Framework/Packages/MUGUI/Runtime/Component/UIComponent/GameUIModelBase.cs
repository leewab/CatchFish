using System;
using System.Collections.Generic;
using MUEngine;
using MUGame;
using MUGUI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// ui中模型基类
    /// 1.3渲2的显示方式
    /// 2.3d模型直接挂载的方式
    /// </summary>
    public class GameUIModelBase : GameUIComponent
    {
        public enum MaterialIdx
        {
            DefaultMaterial = 0,
            UIMaterial = 1,
        }

        public enum EntityType
        {
            Player,
            Mount,
            BackScene,
        }

        public Action onLoadComplete = null;
        public Action onZoomIn = null;
        public Action onZoomOut = null;
        public Action<float> onRotation = null;

        protected MUActorEntity mPlayerEntity;
        protected MUActorEntity mMountEntity;
        protected bool mPlayerLoadComplete = false;
        protected bool mMountLoadComplete = false;
        protected Graphic mUITex = null;
        protected string mPlayerResName = "";
        protected string mMountResName = "";
        protected bool mLogicVisible = true;
        protected bool mRemovingMount = false;

        private Entity mEffect = null;
        private int effectId = MUEngine.Entity.INVALID_ID; 
        protected string mEffectBindPoint = "";


        private AnimatorCullingMode mPlayerOriginCullingMode = AnimatorCullingMode.CullUpdateTransforms;
        private AnimatorCullingMode mMountOriginCullingMode = AnimatorCullingMode.CullUpdateTransforms;
        //--------------放大缩小操作-----------------
        private TweenPosition3D mTweenPos;
        private TweenScale mTweenScale;
        private bool mZoomIn = false;
        private bool mZoomOut = true;//默认是拉远的状态
        private float mZoomInFactor = 0.99f;
        private const float ZoomTimer = 0.5f;

        //---------------旋转操作---------------------
        private TweenRotation mTweenRotation;
        private TweenRotation mTweenRotation2;
        private TweenRotation mTweenRotation3;
        private bool mRotationBack = false; //默认未旋转的背后
        private const float mRotationTime = 0.3f;
        private int rotatingID = -1;
        private float rotatingDelta = 0f;

        // 背景
        protected Entity mBackSceneEntity;
        protected string mBackSceneResName = "";
        protected bool mBackSceneLoadComplete = false;
        protected Vector3 mBackSceneOffset = new Vector3();
        private TweenPosition3D mBackSceneTweenPos;
        private TweenScale mBackSceneTweenScale;
        public enum BodyShowType
        {
            Whole, Half, Head
        }

        protected override void OnInit()
        {
            base.OnInit();
            mUITex = gameObject.GetComponent<Graphic>();
            UIEventListener.AddDragCallBack(OnRotaModel);
        }

        //显示类型，全身、半身、头部
        public int ShowType
        {
            set; get;
        }
        /// <summary>
        /// 禁止拖拽ui改变模型y的旋转
        /// </summary>
        public bool ForbidRotate
        {
            set; get;
        }
        //全身高度（脚到脑袋顶）
        public float BodySize
        {
            set; get;
        }
        //用于渲染非全身时所用的高度
        public float BodySizeNonWhole
        {
            set; get;
        }
        //半身高度
        public float HalfBodySize
        {
            set; get;
        }
        //头部高度
        public float HeadBodySize
        {
            set; get;
        }
        //初始角度(Y)
        public float InitialAngle
        {
            set; get;
        }
        //默认x偏移
        public float PosXOffset
        {
            set; get;
        }
        //默认Y偏移
        public float PosYOffset
        {
            set; get;
        }
        //UI挂载模型默认x偏移比例
        public float PosXFactor
        {
            set; get;
        }
        //UI挂载模型默认Y偏移比例
        public float PosYFactor
        {
            set; get;
        }
        //脚离地高度
        public float FootHeight
        {
            set; get;
        }
        //坐骑默认x偏移
        public float MountPosXOffset { set; get; }
        //坐骑初始角度(Y)
        public float MountInitialAngle { set; get; }
        //坐骑高度
        public float MountHeight { set; get; }
        //角色骑乘高度
        public float RidingHeight { set; get; }
        private bool _OpenCache = false;
        public bool OpenCache
        {
            set { _OpenCache = value; }
            get { return _OpenCache; }
        }
        protected Dictionary<string, MUActorEntity> _Cache = null;
        public virtual void Load(string res)
        {
            mPlayerLoadComplete = false;
            if (mPlayerEntity != null)
            {
                if (mPlayerResName.Equals(res))
                {
                    if (mPlayerLoadComplete)
                    {
                        AddNextFrameExecute();
                    }
                    return;
                }
                else
                {
                    if (_OpenCache)
                    {
                        if (_Cache == null)
                            _Cache = new Dictionary<string, MUActorEntity>();

                        if (_Cache.ContainsKey(mPlayerResName) == false)
                        {
                            _Cache[mPlayerResName] = mPlayerEntity;
                        }
                        _Cache[mPlayerResName].Visible = false;
                        mPlayerResName = res;
                        if (_Cache.ContainsKey(res))
                        {
                            mPlayerEntity = _Cache[res];
                            mPlayerEntity.Visible = true;
                            mPlayerLoadComplete = mPlayerEntity.GameObject != null;
                            if (mPlayerLoadComplete)
                            {
                                OnPlayerLoadDone();
                            }
                            else mPlayerEntity.OnLoadResource += OnPlayerLoadDone;
                            return;
                        }
                    }
                    else RemovePlayer();
                }
            }
            mPlayerResName = res;

            mPlayerEntity = MURoot.Scene.AddActor();
            mPlayerEntity.Name = res;
            mPlayerEntity.OnLoadResource += OnPlayerLoadDone;
            mPlayerEntity.Load();
            mPlayerEntity.SetMaterialByIdx((int)MaterialIdx.UIMaterial);

            if (_OpenCache)
            {
                if (_Cache == null)
                    _Cache = new Dictionary<string, MUActorEntity>();

                if (_Cache.ContainsKey(mPlayerResName) == false)
                {
                    _Cache[mPlayerResName] = mPlayerEntity;
                }
            }
        }

        public virtual void AddMount(string res, int type, string bd_name)
        {
            if (mMountEntity != null)
            {
                if (mMountResName.Equals(res))
                {
                    if (mMountLoadComplete)
                    {
                        MountAttachPlayer(type, bd_name);
                        AddNextFrameExecute();
                        return;
                    }
                }
                else RemoveMount();
            }
            mMountResName = res;
            if (mPlayerEntity != null)
            {
                //角色已经加载后再加载坐骑，scale应该还原
                mPlayerEntity.Scale = Vector3.one;
                mPlayerEntity.CancelAction(0.0f);
                mMountEntity = MURoot.Scene.AddActor();
                mMountEntity.Name = res;
                mPlayerEntity.Pause();
                mMountEntity.OnLoadResource += OnMountLoadDone;
                mMountLoadComplete = false;
                mMountEntity.Load();
                mMountEntity.SetMaterialByIdx((int)MaterialIdx.UIMaterial);
                MountAttachPlayer(type, bd_name);
            }
        }
        public virtual void RemoveMount()
        {
            RealRemoveMount(false);

        }

        // 添加一个小场景
        public virtual void AddBackScene(string res, Vector3 pos)
        {
            if(mBackSceneEntity != null)
            {
                if(mBackSceneResName.Equals(res))
                {
                    if(mBackSceneLoadComplete)
                    {
                        AddNextFrameExecute();
                    }
                    return;
                }
                else
                {
                    RemoveBackScene();
                }
            }

            mBackSceneResName = res;
            mBackSceneEntity = MURoot.Scene.AddEntity();
            mBackSceneEntity.Name = res;
            mBackSceneEntity.OnLoadResource += OnBackSceneLoadDone;
            mBackSceneEntity.Load();
            mBackSceneLoadComplete = false;
            mBackSceneOffset = pos;

        }

        public virtual void RemoveBackScene()
        {
            if(mBackSceneEntity == null)
            {
                return;
            }
            mBackSceneEntity.OnLoadResource -= OnBackSceneLoadDone;
            MURoot.Scene.DelEntity(mBackSceneEntity);

            mBackSceneLoadComplete = false;
            mBackSceneResName = "";
            mBackSceneEntity = null;
        }

        public virtual void OnBackSceneLoadDone()
        {
            mBackSceneLoadComplete = true;
            mBackSceneEntity.OnLoadResource -= OnBackSceneLoadDone;
            mBackSceneEntity.SetLayer(RenderLayer);
            mBackSceneEntity.Parent = ModelRoot;
            ChangeLayer(mBackSceneEntity.GameObject.transform, RenderLayer);
            AddNextFrameExecute();
        }

        public virtual void SetBackSceneScale(Vector3 scale)
        {
            if(mBackSceneEntity != null)
            {
                mBackSceneEntity.Scale = scale;
            }
        }

        public virtual void SetBackSceneRotation(Vector3 rotate)
        {
            if(mBackSceneEntity != null)
            {
                mBackSceneEntity.Rotation = Quaternion.Euler(rotate.x, rotate.y, rotate.z);
            }
        }

        protected virtual void ChangeLayer(Transform trans, int layer)
        {
            trans.gameObject.layer = layer;
            int childCount = trans.childCount;
            for(int i=0; i<childCount; i++)
            {
                Transform child = trans.GetChild(i);
                ChangeLayer(child, layer);
            }
        }

        //获取加载完成
        public virtual bool LoadComplete
        {
            get
            {
                if(mPlayerEntity != null)
                {
                    if (mPlayerLoadComplete == false && mPlayerEntity.GameObject != null)
                    {
                        mPlayerLoadComplete = true;
                        mPlayerEntity.OnLoadResource -= OnPlayerLoadDone;
                    }
                }

                if (mMountEntity != null)
                {
                    if (mMountLoadComplete == false && mMountEntity.GameObject != null)
                    {
                        mMountLoadComplete = true;
                        mMountEntity.OnLoadResource -= OnMountLoadDone;
                    }
                }
                else
                {
                    mMountLoadComplete = true;
                }

                if (mBackSceneEntity != null)
                {
                    if (mBackSceneLoadComplete == false && mBackSceneEntity.GameObject != null)
                    {
                        mBackSceneLoadComplete = true;
                        mBackSceneEntity.OnLoadResource -= OnBackSceneLoadDone;
                    }
                }
                else
                {
                    mBackSceneLoadComplete = true;
                }
                return mPlayerLoadComplete && mMountLoadComplete && mBackSceneLoadComplete;
            }
        }

        public virtual MUActorEntity LogicEntity
        {
            get
            {
                if (mMountEntity != null)
                {
                    return mMountEntity;
                }
                return mPlayerEntity;
            }
        }

        public virtual MUActorEntity Entity
        {
            get
            {
                return mPlayerEntity;
            }
        }

        public virtual MUActorEntity MountEntity
        {
            get
            {
                return mMountEntity;
            }
        }

        public virtual Entity BackSceneEntity
        {
            get
            {
                return mBackSceneEntity;
            }
        }
        //设置旋转
        public virtual void SetModelRotation(float y)
        {
            if (LogicEntity != null)
            {
                LogicEntity.Rotation = Quaternion.Euler(0, y, 0);
            }
        }
        //设置模型在ui的材质
        public void SetMaterialByIdx(int idx)
        {
            if (Entity != null)
            {
                Entity.SetMaterialByIdx(idx);
            }
            if (MountEntity != null)
            {
                MountEntity.SetMaterialByIdx(idx);
            }
        }
        //时装添加部件
        public void AddPlayerComponent(int type, string resName)
        {
            if (this.Entity != null)
            {
                this.Entity.AddPlayerComponent(type, resName);
            }
        }
        /// <summary>
        /// 添加部位特效
        /// </summary>
        /// <param name="part"></param>
        /// <param name="res_name"></param>
        /// <param name="bd_pt"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <param name="scale"></param>
        public void AddComponentEffect(EPlayerElement part, string res_name, string bd_pt, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            if (this.Entity != null)
            {
                this.Entity.AddComponentEffect(part, res_name, bd_pt, pos, rot, scale);
            }
        }
        /// <summary>
        /// 设置部位特效可见性
        /// </summary>
        /// <param name="ele_type"></param>
        /// <param name="vis"></param>
        public void SetComponentEffectVisible(EPlayerElement ele_type, bool vis)
        {
            if (this.Entity != null)
            {
                this.Entity.SetComponentEffectVisible(ele_type, vis);
            }
        }

        public void AddFaceDriver(string prefab, int maxPart)
        {
            if (this.Entity != null)
            {
                Entity.AddFaceDriver(prefab, maxPart);
            }
        }
        public void ApplyMakeFace(int index, int faceType, int subType, int attr)
        {
            if (this.Entity != null)
            {
                Entity.ApplyMakeFace(index, faceType, subType, attr);
            }
        }
        public void SetMakeFaceTexQualityDiv(int div)
        {
            if (this.Entity != null)
            {
                Entity.SetMakeFaceTexQualityDiv(div);
            }
        }
        public void ApplyMakeFaceAll(string faceInfo)
        {
            if (this.Entity != null)
            {
                Entity.ApplyMakeFaceAll(faceInfo);
            }
        }
        public void ChangeFace(string face_types, string strs, int oper_type)
        {
            if (this.Entity != null)
            {
                Entity.ChangeFace(face_types, strs, oper_type);
            }
        }
        public void ChangeFaceTex(string face_types, string strs, string colors)
        {
            if (this.Entity != null)
            {
                Entity.ChangeFaceTex(face_types, strs, colors);
            }
        }
        public void ChangeSkinColor(float intensity, Color clr)
        {
            if (this.Entity != null)
            {
                Entity.ChangeSkinColor(intensity, clr);
            }
        }
        //时装移除部件
        public void RemovePlayerComponent(EPlayerElement type)
        {
            if (this.Entity != null)
            {
                this.Entity.RemovePlayerComponent(type);
            }
        }

        public void AddComponentEffect(EPlayerElement type, string res_name, string bd_pt = "")
        {
            if (this.Entity != null)
            {
                this.Entity.AddComponentEffect(type, res_name, bd_pt);
            }
        }

        public void ClearComponentEffect(EPlayerElement type)
        {
            if (this.Entity != null)
            {
                this.Entity.ClearComponentEffect(type);
            }
        }
        public void ClearComponentEffectByBindPointName(EPlayerElement type, string bd_pt)
        {
            if (this.Entity != null)
            {
                this.Entity.ClearComponentEffectByBindPointName(type, bd_pt);
            }
        }
        //更改时装部件颜色
        public void ChangeComponentColorStr(int type, string hexStr, int idx)
        {
            if (this.Entity != null)
            {
                this.Entity.ChangeComponentColorStr(type, hexStr, idx);
            }
        }

        public void AddEntityOnBindPoint(MUFakeEntity model, string bd_point)
        {
            if (this.Entity != null)
            {
                this.Entity.AddEntityOnBindPoint(model, bd_point);
            }
        }

        public void DelEntityOnBindPoint(string bd_point)
        {
            if (this.Entity != null)
            {
                this.Entity.DelEntityOnBindPoint(bd_point);
            }
        }

        public void ChangeHairColor(string clr1, string clr2, string clr3, string clr4)
        {
            if (this.Entity != null)
            {
                this.Entity.ChangeHairColor(clr1, clr2, clr3, clr4);
            }
        }

        public void ChangeBeardColor(string clr1, string clr2, string clr3, string clr4)
        {
            if (this.Entity != null)
            {
                this.Entity.ChangeBeardColor(clr1, clr2, clr3, clr4);
            }
        }
        public void ChangeSelfHoldMeshColor(string clr1, string clr2, string clr3, string clr4)
        {
            if (this.Entity != null)
            {

                this.Entity.ChangeSelfHoldMeshColor(clr1, clr2, clr3, clr4);
            }
        }

        //播放act
        public IActCtrl PlayAction(string name, ActType mask = ActType.None, float speed = 1)
        {
            //if (this.Entity != null)
            //{
            //    return this.Entity.PlayAct(name, (int)mask, speed);
            //}
            return null;
        }
        //播放trigger
        public void PlayAnimByAnimator(int trigger_hash)
        {
            if (this.Entity != null)
            {
                this.Entity.PlayAnimByAnimator(trigger_hash);
            }
        }
        //终止动作
        public void CancelAction(float time = 0.1f)
        {
            if (this.Entity != null)
            {
                this.Entity.ResetAnimator();
            }
        }

        public void SetAnimatorInt(int trigger_hash, int val)
        {
            if (this.Entity != null)
            {
                this.Entity.SetAnimatorInt(trigger_hash,val);
            }
        }

        public void StartAnimationClipControl(string name, bool aotoUpdate)
        {
            if (this.Entity != null)
            {
                this.Entity.StartAnimationClipControl(name, aotoUpdate);
            }

        }
        public void StopAnimationClipControl()
        {
            this.Entity.StopAnimationClipControl();
        }
        public void SetTransformVisible(string name, bool visible)
        {
            if (this.Entity != null)
            {
                this.Entity.SetTransformVisible(name, visible);
            }
        }

        public void SetComponentVisible(MUEngine.EPlayerElement ele_type, bool visible)
        {
            if (this.Entity != null)
            {
                this.Entity.SetComponentVisible(ele_type, visible);
            }
        }

        public Transform FindBone(string bone_name)
        {
            if (this.Entity != null)
            {
                return this.Entity.FindBone(bone_name);
            }
            return null;
        }

        //设置是否显示
        public void SetModelVisible(bool v)
        {
            mLogicVisible = v;
            if (Entity != null)
            {
                Entity.RenderVisible = v;
            }
            if (MountEntity != null)
            {
                MountEntity.RenderVisible = v;
            }
        }
        public float ZoomInFactor
        {
            set
            {
                mZoomInFactor = value;
            }
        }

        public bool LogicVisible
        {
            get
            {
                if (Entity!= null)
                    return Entity.LogicVisible;
                return false;
            }
        }

        public void ChangePosAndScale(float offsetY, float scale)
        {
            if (this.LoadComplete)
            {
                GameObject go = this.Entity.GameObject;
                go.transform.localPosition = new Vector3(go.transform.localPosition.x, go.transform.localPosition.y + offsetY, go.transform.localPosition.z);
                go.transform.localScale = new Vector3(scale, scale, scale);
            }
        }
        //放大
        public void ZoomIn()
        {
            if (this.LoadComplete)
            {
                if (mZoomIn == false)
                {
                    GameObject go = this.Entity.GameObject;
                    if (mTweenPos == null)
                    {
                        Vector3 frompos = go.transform.localPosition;
                        Vector3 topos = new Vector3(frompos.x, frompos.y - ((BodySize) / mZoomInFactor * LogicEntity.Scale.x), frompos.z);
                        mTweenPos = TweenPosition3D.BeginFrom(go, ZoomTimer, frompos, topos);

                        if (mBackSceneEntity != null)
                        {
                            if (mBackSceneTweenPos == null)
                            {
                                Vector3 sceneFromPos = mBackSceneEntity.GameObject.transform.localPosition;
                                Vector3 sceneToPos = topos + mBackSceneOffset;
                                mBackSceneTweenPos = TweenPosition3D.BeginFrom(mBackSceneEntity.GameObject, ZoomTimer, sceneFromPos, sceneToPos);
                            }
                        }
                    }

                    if (mTweenScale == null)
                    {
                        Vector3 fromscale = go.transform.localScale;
                        Vector3 toscale = fromscale * 2;
                        mTweenScale = TweenScale.Begin(go, ZoomTimer, toscale);
                        mTweenScale.from = fromscale;
                    }

                    if (mBackSceneEntity != null)
                    {
                        Vector3 fromscale = mBackSceneEntity.GameObject.transform.localScale;
                        Vector3 toscale = fromscale * 2;
                        mBackSceneTweenScale = TweenScale.Begin(mBackSceneEntity.GameObject, ZoomTimer, toscale);
                        mBackSceneTweenScale.from = fromscale;
                    }

                    mTweenPos.PlayForward();
                    mTweenScale.PlayForward();
                    if(mBackSceneTweenPos)
                    {
                        mBackSceneTweenPos.PlayForward();
                    }
                    if(mBackSceneTweenScale)
                    {
                        mBackSceneTweenScale.PlayForward();
                    }

                    mZoomIn = true;
                    mZoomOut = false;

                    if (onZoomIn != null) onZoomIn();
                }
            }
        }
        //缩小
        public void ZoomOut()
        {
            if (this.LoadComplete)
            {
                if (mZoomOut == false)
                {
                    mZoomOut = true;
                    mZoomIn = false;

                    if (mTweenPos != null)
                    {
                        mTweenPos.PlayReverse();
                    }
                    if (mTweenScale != null)
                    {
                        mTweenScale.PlayReverse();
                    }
                    if (mBackSceneTweenPos != null)
                    {
                        mBackSceneTweenPos.PlayReverse();
                    }
                    if (mBackSceneTweenScale != null)
                    {
                        mBackSceneTweenScale.PlayReverse();
                    }
                    if (onZoomOut != null) onZoomOut();
                }
            }
        }

        public void RotationBack()
        {
            if (this.LoadComplete)
            {
                if (mRotationBack == false)
                {
                    mTweenRotation = TweenRotation.Begin(this.Entity.GameObject, mRotationTime, Quaternion.Euler(0, 0, 0));
                    mTweenRotation.to = Quaternion.Euler(0, 0, 0).eulerAngles;
                    mTweenRotation.PlayForward();

                    mRotationBack = true;

                }
            }
        }
        //缩小
        public void RotationFront()
        {
            if (this.LoadComplete)
            {
                if (mRotationBack == true)
                {
                    mTweenRotation2 = TweenRotation.Begin(this.Entity.GameObject, mRotationTime, Quaternion.Euler(0, 180, 0));
                    mTweenRotation2.to = Quaternion.Euler(0, 180, 0).eulerAngles;
                    mTweenRotation2.PlayForward();
                    mRotationBack = false;
                }
            }
        }

        //旋转到某个角度
        public void RotationTo(float angle)
        {
            if (this.LoadComplete)
            {
                float MAX_ANGLE = 360;
                angle %= MAX_ANGLE;
                // 计算小角，决定旋转方向
                Vector3 eulerAngles = this.Entity.GameObject.transform.rotation.eulerAngles;
                float from = eulerAngles.y;
                if(angle - from > MAX_ANGLE/2)
                {
                    angle -= MAX_ANGLE;
                }
                else if(angle - from < -MAX_ANGLE/2)
                {
                    angle += MAX_ANGLE;
                }
                mTweenRotation3 = TweenRotation.Begin(this.Entity.GameObject, mRotationTime, Quaternion.Euler(0, angle, 0));
                mTweenRotation3.to = new Vector3(0, angle, 0);
                mTweenRotation3.PlayForward();
                mRotationBack = false;
            }
        }

        protected override void OnDispose()
        {
            base.Dispose();
            onLoadComplete = null;
            onZoomIn = null;
            onZoomOut = null;
            onRotation = null;
            if (rotatingID > -1)
                TimerHandler.RemoveTimeactionByID(rotatingID);
            rotatingID = -1;
            rotatingDelta = 0f;
        }
        //销毁模型
        public virtual void DisposeModel()
        {
            onLoadComplete = null;
            mPlayerLoadComplete = false;
            mPlayerResName = string.Empty;
            mMountLoadComplete = false;
            mMountResName = string.Empty;
            mZoomOut = true;
            mZoomIn = false;
            if (mTweenPos != null) GameObject.Destroy(mTweenPos);
            if (mTweenScale != null) GameObject.Destroy(mTweenScale);
            if (mBackSceneTweenPos != null) GameObject.Destroy(mBackSceneTweenPos);
            if (mBackSceneTweenScale != null) GameObject.Destroy(mBackSceneTweenScale);
            mTweenScale = null;
            mTweenPos = null;
            mBackSceneTweenPos = null;
            mBackSceneTweenScale = null;
            if (rotatingID > -1)
                TimerHandler.RemoveTimeactionByID(rotatingID);
            rotatingID = -1;
            rotatingDelta = 0f;

            mRotationBack = false;
            if (mTweenRotation != null) {
                GameObject.Destroy(mTweenRotation);
                mTweenRotation = null;
            }
            if (mTweenRotation2 != null)
            {
                GameObject.Destroy(mTweenRotation2);
                mTweenRotation2 = null;
            }
            if (mTweenRotation3 != null)
            {
                GameObject.Destroy(mTweenRotation3);
                mTweenRotation3 = null;
            }
            if (mLogicVisible == false)
            {
                mLogicVisible = true;
                if (Entity != null)
                {
                    Entity.RenderVisible = true;
                    Entity.ForceLOD(-1);
                }
                if (MountEntity != null)
                {
                    MountEntity.RenderVisible = true;
                    MountEntity.ForceLOD(-1);
                }
            }
            if (Entity != null)
            {
                Entity.RemoveFaceDriver();
 //               Entity.SetMaterialByIdx((int)MaterialIdx.DefaultMaterial);
            }
            
            if (MountEntity != null)
            {
                MountEntity.SetMaterialByIdx((int)MaterialIdx.DefaultMaterial);
            }
            RealRemoveMount(true);
            
            if (_OpenCache && _Cache != null)
            {
                foreach(var item in _Cache)
                {
                    RemoveEntityTempInfo(item.Value, false);
                    item.Value.OnLoadResource = null;
                    MURoot.Scene.DelEntity(item.Value);
                }
                _Cache = null;
            }
            else
            {
                RemovePlayer();
            }
            //多次设置，反正残留
            mPlayerEntity = null;

            RemoveBackScene();
            RemoveEffect();
        }
        //角色加载完成
        protected virtual void OnPlayerLoadDone()
        {
            mPlayerLoadComplete = true;
            mPlayerEntity.OnLoadResource -= OnPlayerLoadDone;
            SetEntityTempInfo(mPlayerEntity, false);
            AddNextFrameExecute();
        }
        //坐骑加载完成
        protected virtual void OnMountLoadDone()
        {
            mMountLoadComplete = true;
            mMountEntity.OnLoadResource -= OnMountLoadDone;
            SetEntityTempInfo(mMountEntity, true);
            AddNextFrameExecute();
            mPlayerEntity.RestartAttachEffects();
        }
        //获得模型根节点
        protected virtual Transform ModelRoot
        {
            get
            {
                return mUITex.transform;
            }
        }
        protected float GetDefaultAngle(float a)
        {
            if (a == 0)
            {
                return 180;
            }
            else
            {
                return 180 + a;
            }
        }
        protected virtual int RenderLayer
        {
            get
            {
                return RenderLayerDef.RL_ACTOR;
            }
        }
        //加载完成自动设置数据
        public virtual void AutoSet()
        {
            //Debug.Log("---------------------AutoSet " + Time.frameCount);
        }
        /*
         * 如果不是下一帧检查
         * 加载角色同步加载完成
         * 坐骑对象还没有
         * 所以LoadComplete就算是完成
         * 会先执行一次onLoadComplete处理
         * 等坐骑加载完成会再一次的执行onLoadComplete处理
         * 这样一次加载会有两次的回调处理（非常不好）
         */
        private void NextFrameCheck()
        {
            if (LoadComplete)
            {
                AutoSet();
                if (mPlayerEntity != null)
                {
                    mPlayerEntity.ForceLOD(0);
                }
                if (mMountEntity != null)
                {
                    mMountEntity.ForceLOD(0);
                }
                if (onLoadComplete != null)
                {
                    onLoadComplete();
					//add by LiaoPengfei 20170731恢复动作
                    if (mZoomIn) {
                        mZoomIn = false;
                        ZoomIn();
                    }
                    if (mRotationBack) {
                        mRotationBack = false;
                        RotationBack();
                    }
                }
            }
        }
        //加载完成设置实体的一些临时参数
        private void SetEntityTempInfo(MUActorEntity e, bool isMount)
        {
            int layer = RenderLayer;
            e.SetLayer(layer);
            //e.Layer = RenderLayer;
            //GameObjectUtil.SetLayer(e.GameObject, RenderLayer, true);
            e.Parent = ModelRoot;
            Animator t = null;
            if (e.GameObject != null)
            {
                t = e.GameObject.GetComponent<Animator>();
            }
            if (t != null)
            {
                if (isMount)
                    mMountOriginCullingMode = t.cullingMode;
                else
                    mPlayerOriginCullingMode = t.cullingMode;
                t.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }
            //e.RecordOriginalShader();
        }
        //还原参数
        private void RemoveEntityTempInfo(MUActorEntity e, bool isMount)
        {
            if (e == null) return;
            Animator a = null;
            if (e.GameObject != null)
            {
                a = e.GameObject.GetComponent<Animator>();
            }
            if (a != null)
            {
                if (isMount)
                {
                    a.cullingMode = mMountOriginCullingMode;
                }
                else
                {
                    a.cullingMode = mPlayerOriginCullingMode;
                }

            }
            if (e != null)
            {
                e.StopAct();
            }
            int layer = RevertLayer;
            e.SetLayer(layer);
            //e.Layer = RevertLayer;
            //GameObjectUtil.SetLayer(e.GameObject, RevertLayer, true);
            e.Scale = Vector3.one;
            //e.OriginalShader();
        }
        //拖拽旋转实体
        private Vector3 mCenterPos;
        public Vector3 CenterPos { set { mCenterPos = value; } }
        private bool mChangeCenterPos = false;
        public bool ChangeCenterPos { set { mChangeCenterPos = value; } }
        protected virtual void OnRotaModel(GameObject go, Vector2 delta, Vector2 pos)
        {
            StopRotation();
            if (!ForbidRotate && LoadComplete)
            {
                var maintrans = LogicEntity.GameObject.transform;
                if (mChangeCenterPos)
                {
                    var center = maintrans.TransformPoint(mCenterPos);
                    maintrans.RotateAround(center, Vector3.up, -delta.x);
                    if (onRotation != null)
                        onRotation(-delta.x);
                }
                else
                {
                    maintrans.Rotate(Vector3.up, -delta.x);
                    if (onRotation != null)
                        onRotation(-delta.x);
                }
            }
        }

        //把人骑在模型上
        private void MountAttachPlayer(float type, string bd_name)
        {
            if (mRemovingMount)
            {
                Entity.RenderVisible = true;
                TimerHandler.RemoveTimeaction(OnTimer);
                mRemovingMount = false;
            }

            Entity.Position = Vector3.zero;
            Entity.Rotation = Quaternion.identity;
            int movestate_enum = AnimatorHashTool.GetNameHash("movestate");
            Entity.SetAnimatorInt(movestate_enum, 15);

            int ridetype_enum = AnimatorHashTool.GetNameHash("ridetype");
            Entity.SetAnimatorFloat(ridetype_enum, type);

            int movespeed_enum = AnimatorHashTool.GetNameHash("movespeed");
            Entity.SetAnimatorFloat(movespeed_enum, 0);

            mMountEntity.MountAttachPlayer(Entity, bd_name);
        }

        private void RemovePlayer()
        {
            if (mPlayerEntity != null)
            {
                RemoveEntityTempInfo(mPlayerEntity, false);
                mPlayerEntity.OnLoadResource = null;
                MURoot.Scene.DelEntity(mPlayerEntity);
                mPlayerEntity = null;
                RemoveTween();
            }
        }
        private void RemoveTween()
        {
            //add by LiaoPengfei 20170731清理动作
            if (mTweenPos != null) GameObject.Destroy(mTweenPos);
            if (mTweenScale != null) GameObject.Destroy(mTweenScale);
            if (mBackSceneTweenPos != null) GameObject.Destroy(mBackSceneTweenPos);
            if (mBackSceneTweenScale != null) GameObject.Destroy(mBackSceneTweenScale);
            
            mTweenScale = null;
            mTweenPos = null;
            mBackSceneTweenPos = null;
            mBackSceneTweenScale = null;

            if (mTweenRotation != null) GameObject.Destroy(mTweenRotation);
            if (mTweenRotation2 != null) GameObject.Destroy(mTweenRotation2);
            if (mTweenRotation3 != null) GameObject.Destroy(mTweenRotation3);
            mRotationBack = false;
        }

        private void AddNextFrameExecute()
        {
            /*
            * 去掉上一个加载完成的回调函数（特别是在同一帧中两个都加载完成的情况）
            * 所以检查完成的操作就变成了一个（结果完成的回调函数只是执行一次）
            * 在多帧加载完成的情况下
            * 第一个的LoadComplete肯定是false不会执行回调函数
            * （结果完成的回调函数只是执行一次）
            */
            Game.Core.Common.NextFrameExecute -= NextFrameCheck;
            Game.Core.Common.NextFrameExecute += NextFrameCheck;
        }
        private int RevertLayer
        {
            get
            {
                return RenderLayerDef.RL_ACTOR;
            }
        }
        private void RealRemoveMount(bool dispose)
        {
            //3->2的需要重新计算视口，位置等
            if (mMountEntity != null)
            {
                if (Entity != null)
                {
                    int movestate_enum = AnimatorHashTool.GetNameHash("movestate");
                    Entity.SetAnimatorInt(movestate_enum, 0);
                    mMountEntity.MountRemovePlayer(Entity);

                    if (!dispose)
                    {
                        Entity.Parent = ModelRoot;
                        Entity.RenderVisible = false;
                        TimerHandler.RemoveTimeaction(OnTimer);
                        TimerHandler.SetTimeout(OnTimer, 0.2f);
                        mRemovingMount = true;
                    }
                    else
                    {
                        if (mRemovingMount)
                        {
                            Entity.RenderVisible = true;
                            TimerHandler.RemoveTimeaction(OnTimer);
                            mRemovingMount = false;
                        }
                    }
                }
                RemoveEntityTempInfo(mMountEntity, true);
                mMountEntity.OnLoadResource -= OnMountLoadDone;
                MURoot.Scene.DelEntity(mMountEntity);
                mMountEntity = null;
            }
            mMountResName = "";
        }

        private void OnTimer()
        {
            mRemovingMount = false;
            if (Entity != null)
            {
                Entity.RenderVisible = true;
            }
        }

        public void EnableLodGroup( bool enable )
        {
            if (Entity != null)
            {
                Entity.SetLodGroupEnable(enable);
            }
        }

        public void StartRotation(float delta)
        {
            StopRotation();
            rotatingID = TimerHandler.SetTimeInterval(UpdateRotate, 0.01f);
            rotatingDelta = delta;
        }

        public void StopRotation()
        {
            if (rotatingID > -1)
                TimerHandler.RemoveTimeactionByID(rotatingID);
            rotatingID = -1;
            rotatingDelta = 0f;
        }

        private void UpdateRotate()
        {
            if (!ForbidRotate && LoadComplete)
            {
                var maintrans = LogicEntity.GameObject.transform;
                maintrans.Rotate(Vector3.up, rotatingDelta);
                if (onRotation != null)
                    onRotation(rotatingDelta);
            }
        }

        public void AddEffect(string eff_name, string bd_name, Vector3 pos, Vector3 rot, Vector3 scale)
        {
            RemoveEffect();
            mEffect = MURoot.Scene.AddEffect(eff_name, Vector3.zero, Quaternion.identity);

            if (mEffect != null)
            {
                effectId = mEffect.ID;

                mEffectBindPoint = bd_name;
                //mEffect.EntityType = EntityType.UIEffectEntity;
                mEffect.OnLoadResource += OnEffectLoadDone;
                mEffect.Position = pos;
                mEffect.Scale = scale;
                mEffect.Rotation = Quaternion.Euler(rot);
                mEffect.Load();
            }
        }

        public void RemoveEffect()
        {
            if (IsValidEffect())
            {
                if(mEffect.IsValid())
                    mEffect.OnLoadResource -= OnEffectLoadDone;
                MURoot.Scene.DelEntity(effectId);
            }
            mEffectBindPoint = "";
        }

        private void OnEffectLoadDone()
        {
            if (!IsValidEffect())
                return;
            if (mEffect.GameObject == null)
                return;
            mEffect.OnLoadResource -= OnEffectLoadDone;
            mEffect.Restart();
            if (mPlayerEntity != null)
                mEffect.Parent = mPlayerEntity.GetBindPoint(mEffectBindPoint);

            //统一设置SortingOrder 注意此处应该放在上面一行代码的后面，应为必须先设置对应GameObject的父节点，才能正确设置SortingOrder
            UIUtil.AutoSetUIEntitySortingOrder(mEffect);

            mEffect.SetAniCullingType(UnityEngine.AnimationCullingType.AlwaysAnimate);
        }

        private bool IsValidEffect()
        {
            return mEffect != null && effectId == mEffect.ID;
        }

        private void ClearEffectInfo()
        {
            mEffect = null;
            effectId = MUEngine.Entity.INVALID_ID;
        }

        public virtual bool Is3DModel()
        {
            return true;
        }
        public bool IsValid()
        {
            if (mPlayerEntity == null)
                return false;
            if (mPlayerEntity.IsValid() == false)
                return false;
            return true;
        }

        public GameUIModelBase(string name, Transform father = null) : base(name, father)
        {
        }

        public GameUIModelBase(GameObject obj) : base(obj)
        {
        }
        
        public GameUIModelBase() : base()
        {
        }
    }
}
