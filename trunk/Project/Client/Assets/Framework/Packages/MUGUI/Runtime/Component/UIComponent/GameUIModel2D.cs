using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MUEngine;

namespace Game.UI
{

#if !GAME_EDITOR

    public class GameUIModel2D : GameUIModelBase
    {
        private static int CameraCount = 0;
        private static Transform mModelRoot = null;
        private static List<UIModel2DCamera> mUseList = new List<UIModel2DCamera>();
        private static List<UIModel2DCamera> mUnUseList = new List<UIModel2DCamera>();
        private UIModel2DCamera mRenderCamera = null;
        private int mCameraIdx = 0;

        // 启用相机旋转
        private bool mEnableCameraRotate = false;
        // 最小仰角
        private int mMinVerticalAngle = -10;
        // 最大仰角
        private int mMaxVerticalAngle = 30;
        // 相机旋转系数
        private const float mCameraRotateFactor = 0.1f;
        // 当前旋转角度
        private float mCurCameraRotateAngle = 0.0f;
        // 相机默认朝向
        private Vector3 mCameraDefaultForward = new Vector3(0, 0, 1);
        public GameUIModel2D()
        {
            CameraCount++;
            mCameraIdx = CameraCount;
        }

        protected override void OnInit()
        {
            base.OnInit();
            AddDragCallBack(OnDrag);
        }

        public override void RemoveMount()
        {
            base.RemoveMount();
            //角色模型还原位置、朝向等
            Vector2 rPos = new Vector2(-PosXOffset, -BodySize / 2 + FootHeight + PosYOffset);
            SetCameraSize((BodySize + FootHeight) / 2);
            SetEntityRelativePos(rPos);
            SetModelRotation(InitialAngle);
        }
        //放置模型的根节点
        protected override Transform ModelRoot
        {
            get
            {
                if (mModelRoot == null)
                {
                    GameObject obj = new GameObject("RenderTexture");
                    GameObject.DontDestroyOnLoad(obj);
                    mModelRoot = obj.transform;
                    mModelRoot.position = Vector3.zero;
                    mModelRoot.rotation = Quaternion.identity;
                }
                return mModelRoot;
            }
        }

        private void SetCameraSize(float camSize)
        {
            if(mRenderCamera != null)
            {
                mRenderCamera.OrthographicSize = camSize;
            }
        }

        private void SetEntityRelativePos(Vector2 relativePos)
        {
            if (mRenderCamera != null)
            {
                if (LogicEntity != null)
                {
                    LogicEntity.Position = mRenderCamera.LookAtPos + new Vector3(relativePos.x, relativePos.y, 0f);
                }
                if (BackSceneEntity != null)
                {
                    BackSceneEntity.Position = mRenderCamera.LookAtPos + mBackSceneOffset + new Vector3(relativePos.x, relativePos.y, 0f);
                }
            }
        }

        public override void Load(string res)
        {
            //相当于PreLoad了,多次加载的时候，不会多次创建
            if(mRenderCamera == null)
            {
                mRenderCamera = CreateRenderCamera();
                CalcCameraRotateAngleAndPos();
            }
            
            base.Load(res);
        }

        public override void AutoSet()
        {
            base.AutoSet();
            MUActorEntity p = MountEntity;
            if (p != null)
            {
                float cSize = MountHeight + RidingHeight;
                Vector2 rPos = new Vector2(-MountPosXOffset, -cSize / 2);
                SetCameraSize(cSize / 2);
                SetEntityRelativePos(rPos);
                SetModelRotation(MountInitialAngle);
            }
            p = Entity;
            if (p != null)
            {
                // 在有坐骑的情况下，不应该设置位置等
                if (MountEntity == null)
                {
                    BodyShowType _showType = (BodyShowType)ShowType;
                    Vector2 rPos = Vector2.zero;
                    float cSize = 0f;

                    if (_showType == BodyShowType.Half)
                    {
                        cSize = HalfBodySize;
                        rPos = new Vector2(-PosXOffset, -(BodySizeNonWhole - cSize / 2));
                    }
                    else if (_showType == BodyShowType.Head)
                    {
                        cSize = HeadBodySize;
                        rPos = new Vector2(-PosXOffset, -(BodySizeNonWhole - cSize / 2));
                    }
                    else
                    {
                        cSize = BodySize;
                        rPos = new Vector2(-PosXOffset, -cSize / 2 + FootHeight + PosYOffset);
                    }

                    SetCameraSize((cSize + FootHeight) / 2);
                    SetEntityRelativePos(rPos);
                    SetModelRotation(InitialAngle);
                }
            }
        }

        public override void SetModelRotation(float y)
        {
            y = GetDefaultAngle(y);
            if (LogicEntity != null)
            {
                LogicEntity.Rotation = Quaternion.Euler(0, y, 0);
            }
        }

        public void SetCameraRotateEnable(bool enable)
        {
            mEnableCameraRotate = enable;
        }

        public void SetCameraDefaultForward(Vector3 forward)
        {
            mCameraDefaultForward = forward;
            if(mRenderCamera != null)
            {
                mRenderCamera.Forward = mCameraDefaultForward;
                CalcCameraRotateAngleAndPos();
            }
            
        }

        public void SetCameraRotateRange(int minAngle, int maxAngle)
        {
            mMinVerticalAngle = minAngle;
            mMaxVerticalAngle = maxAngle;
        }

        protected void CalcCameraRotateAngleAndPos()
        {
            if (mRenderCamera != null)
            {
                Vector3 forward = mRenderCamera.Forward;
                forward.Normalize();
                mCurCameraRotateAngle = Mathf.Acos(Vector3.Dot(forward, new Vector3(0, 0, 1)));
                mCurCameraRotateAngle = Mathf.Rad2Deg * mCurCameraRotateAngle;
                mRenderCamera.SetPos(mRenderCamera.LookAtPos - forward * mRenderCamera.DestDistance);
            }
        }
        private void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            if( mEnableCameraRotate )
            {
                mCurCameraRotateAngle -= delta.y * mCameraRotateFactor;
                mCurCameraRotateAngle = Mathf.Max(mCurCameraRotateAngle, mMinVerticalAngle);
                mCurCameraRotateAngle = Mathf.Min(mCurCameraRotateAngle, mMaxVerticalAngle);

                float dy = Mathf.Sin(Mathf.Deg2Rad * mCurCameraRotateAngle) * mRenderCamera.DestDistance;
                float dz = - Mathf.Cos(Mathf.Deg2Rad * mCurCameraRotateAngle) * mRenderCamera.DestDistance;

                Vector3 offset = new Vector3(0, dy, dz);
                Vector3 lookAtPos = mRenderCamera.LookAtPos;
                mRenderCamera.SetPos(lookAtPos + offset);
                mRenderCamera.Forward = -offset;
            }
        }
        // public void SetMask()
        // {
        //     if (mUITex != null)
        //     {
        //         mUITex.material = ShaderModule.Instance.NpcMaskMaterial;
        //     }
        // }

        private UIModel2DCamera CreateRenderCamera()
        {
            UIModel2DCamera rCam = GetRenderCamera();
            rCam.Index = mCameraIdx;
            rCam.SetMSAA(true);
            rCam.SetHDR(false);
            rCam.SetOcc(false);
            rCam.SetRenderTextureSize(512, 512);
            Rect rect = mUITex.rectTransform.rect;
            rCam.SetCameraAspect(rect.width, rect.height);
            rCam.AddTexture(mUITex as RawImage);
            rCam.Enable();
            rCam.Forward = mCameraDefaultForward;
            
            return rCam;
        }

        private UIModel2DCamera GetRenderCamera()
        {
            UIModel2DCamera rCam = null;
            if (mUnUseList.Count > 0)
            {
                rCam = mUnUseList[0];
                mUnUseList.RemoveAt(0);
            }
            else
            {
                rCam = new UIModel2DCamera();
                rCam.Init(ModelRoot);
            }
            mUseList.Add(rCam);
            return rCam;
        }

        private void DisposeCamera()
        {
            if (mRenderCamera != null)
            {
                mRenderCamera.Destroy();
                mUnUseList.Add(mRenderCamera);
                mRenderCamera = null;
            }
        }

        //private int Dir
        //{

        //}

        public override void DisposeModel()
        {
            base.DisposeModel();
            DisposeCamera();
        }

        public override bool Is3DModel()
        {
            return false;
        }
    }

    public class UIModel2DCamera
    {
        /// <summary>
        /// 最大列数
        /// </summary>
        protected const int CAMERA_MAX_COL = 10;
        /// <summary>
        /// 间距
        /// </summary>
        protected const int CAMERA_MAX_SIZE = 10;
        /// <summary>
        /// 起始位置
        /// </summary>
        protected static Vector3 CAMERA_BASE_POS = new Vector3(-500.0f, -2500.0f, -5000.0f);

        protected List<RawImage> mTextureList = new List<RawImage>();
        protected RawImage mTexture = null;
        protected GameObject mCamObj = null;
        protected Camera mCamera = null;
        protected RenderTexture mRenderTex = null;
        protected int mIndex = 0;

        // 与目标点的距离
        public float DestDistance
        {
            get
            {
                return CAMERA_MAX_SIZE * 0.5f;
            }
        }
        public int Index
        {
            get
            {
                return mIndex;
            }
            set
            {
                mIndex = value;
                if (mCamObj != null)
                {
                    string camName = string.Format("Res-{0}-Camera", mIndex);
                    mCamObj.name = camName;
                    mCamObj.transform.position = CalcInitPos();
                }
            }
        }

        public Vector3 Forward
        {
            get
            {
                if (mCamObj != null)
                {
                    return mCamObj.transform.forward;
                }
                return new Vector3(0, 0, 1);
            }
            set
            {
                if (mCamObj != null)
                {
                    mCamObj.transform.forward = value;
                }
            }
        }

        /// <summary>
        /// set msaa
        /// </summary>
        /// <param name="flag"></param>
        public void SetMSAA(bool flag) {
            if (mCamera != null) {
                mCamera.allowMSAA = flag;
            }
        }

        /// <summary>
        /// set occlusion
        /// </summary>
        /// <param name="flag"></param>
        public void SetOcc(bool flag) {
            if (mCamera != null) {
                mCamera.useOcclusionCulling = flag;
            }
        }

        /// <summary>
        /// set HDR
        /// </summary>
        /// <param name="flag"></param>
        public void SetHDR(bool flag) {
            if (mCamera != null) {
                mCamera.allowHDR = flag;
            }
        }

        /// <summary>
        /// 初始化摄像机
        /// </summary>
        /// <param name="root">摄像机父节点</param>
        public void Init(Transform root)
        {
            mCamObj = new GameObject();
            mCamera = mCamObj.AddComponent<Camera>();
            mCamera.clearFlags = CameraClearFlags.SolidColor;
            mCamera.backgroundColor = new Color(0, 0, 0, 0);
            mCamera.cullingMask = 1 << RenderLayerDef.RL_ACTOR;
            mCamera.cullingMask |= 1 << RenderLayerDef.RL_EFFECT;
            mCamera.cullingMask |= 1;//武器是default层
            mCamera.orthographic = true;
            mCamera.nearClipPlane = -4.0f;
            mCamera.farClipPlane = CAMERA_MAX_SIZE;
            mCamObj.transform.parent = root;
            mCamObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            mCamObj.SetActive(false);
            mCamObj.GetOrAddComponent<UICameraFixParams>();

            //在使用这种方式渲染的时候，由于没有背景，因此是要RT的ColorBuffer的A没有值，就会在最终的UI上Blend
            //这一点对于特效尤其致命，因此在此类型的渲染中，单把特效的ColorMask改成RGBA
            //TODO: ParticleColorMaskWritter 丢失暂时注释
            // mCamObj.GetOrAddComponent<ParticleColorMaskWritter>()._EffectColorMaskValue = 15;
            ////
        }
        public int Dir
        {
            get
            {
                return mCamObj.transform.eulerAngles.y == 180 ? 1 : -1;
            }
        }
        /// <summary>
        /// 计算初始化分配摄像机位置
        /// </summary>
        /// <returns>The position.</returns>
        protected Vector3 CalcInitPos()
        {
            int row = mIndex / CAMERA_MAX_COL;
            int col = mIndex % CAMERA_MAX_COL;
            Vector3 offset = Vector3.right * CAMERA_MAX_SIZE * col + Vector3.down * CAMERA_MAX_SIZE * row;
            return CAMERA_BASE_POS + offset;
        }
        /// <summary>
        /// 设置新大小的RenderTexture
        /// </summary>
        public void SetRenderTextureSize(int w, int h)
        {
            if (mRenderTex != null)
            {
                if (mRenderTex.width != w || mRenderTex.height != h)
                {
                    mRenderTex.Release();
                    UnityEngine.Object.Destroy(mRenderTex);

                    mRenderTex = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
                    mRenderTex.name = "GameUIModel2D.mRenderTex";
                    //mRenderTex.antiAliasing = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
                    mRenderTex.Create();
                    mCamera.targetTexture = mRenderTex;
                }
            }
            else
            {
                mRenderTex = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
                mRenderTex.name = "GameUIModel2D.mRenderTex";
                //mRenderTex.antiAliasing = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
                mRenderTex.Create();
                mCamera.targetTexture = mRenderTex;
            }
        }
        public void AddTexture(RawImage tex)
        {
            mTexture = tex;
            mTexture.texture = mRenderTex;
        }

        /// <summary>
        /// 看向的物体位置
        /// </summary>
        public Vector3 LookAtPos
        {
            get
            {
                return CalcInitPos() + new Vector3(0, 0, DestDistance);
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        public void Enable()
        {
            if (mCamObj != null)
            {
                mCamObj.SetActive(true);
            }
        }

        /// <summary>
        /// 禁用
        /// </summary>
        public void Disable()
        {
            if (mCamObj != null)
            {
                mCamObj.SetActive(false);
            }
        }
        /// <summary>
        /// 设置摄像机大小
        /// </summary>
        public float OrthographicSize
        {
            get
            {
                if (mCamera != null)
                {
                    return mCamera.orthographicSize;
                }
                return 0f;
            }
            set
            {
                if (mCamera != null)
                {
                    mCamera.orthographicSize = value;
                }
            }
        }
        public float Aspect
        {
            get
            {
                return mCamera.aspect;
            }
        }
        /// <summary>
        /// 设置摄像机宽高比
        /// </summary>
        public void SetCameraAspect(float uiWidth, float uiHeight)
        {
            if (mCamera != null)
            {
                mCamera.aspect = uiWidth / uiHeight;
            }
        }

        public void Destroy()
        {
            mTexture.texture = null;
            if (mRenderTex != null)
            {
                mRenderTex.Release();
                UnityEngine.Object.Destroy(mRenderTex);
                mRenderTex = null;
            }
            mCamera.targetTexture = null;
            Disable();
        }

        public void SetPos(Vector3 pos)
        {
            mCamObj.transform.position = pos;
        }

    }

#else
    
    public class GameUIModel2D : GameUIModelBase
    {

    }

#endif

}
