
namespace Game.UI
{

#if !GAME_EDITOR
    
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using MUEngine;
    using HedgehogTeam.EasyTouch;
    using MUGame;

    public class GameUIScene2D : GameUIModelBase
    {
        private static int Count = 0;
        private static Transform mSceneRoot = null;
        private static List<UIModelSceneCamera> mUseList = new List<UIModelSceneCamera>();
        private static List<UIModelSceneCamera> mUnUseList = new List<UIModelSceneCamera>();

        /// <summary>
        /// 最大列数
        /// </summary>
        protected const int SCENE_MAX_COL = 10;
        /// <summary>
        /// 间距
        /// </summary>
        protected const int SCENE_MAX_SIZE = 10;
        /// <summary>
        /// 起始位置
        /// </summary>
        protected static Vector3 SCENE_BASE_POS = new Vector3(-500.0f, -500.0f, -500.0f);
                
        // 基础坐标
        private Vector3 mBasePos = new Vector3(0, 0, 0);
        // 摄像机
        private UIModelSceneCamera mRenderCamera = null;
        // idx
        private int mIdx = 0;
        // 启用相机旋转
        private bool mEnableCameraRotate = false;
        // 相机旋转cache
        private bool mEnableCameraRotateCache = false;
        // 最小仰角
        private int mMinVerticalAngle = -10;
        // 最大仰角
        private int mMaxVerticalAngle = 30;
        // 相机旋转系数
        private const float mCameraRotateFactor = 0.1f;
        // 当前旋转角度
        private Vector3 mCameraRotate = new Vector3(0, 180, 0);
        // 相机位置偏移
        private Vector3 mCameraPos = new Vector3(0, 2, 2);
        // 

        // 相机动画
        // 位置动画
        private MUGUI.TweenPosition3D mTweenPos;
        // 旋转动画
        private MUGUI.TweenRotation mTweenRotation;
        // 动画时长
        private float mTweenTime = 0.5f;
        // 是否正在播放动画
        private bool mTweenPlaying = false;

        //是否是旋转相机的效果
        private bool IsRotateCamera = false;
        private float mCameraRotateTarget = 360;
        private float mCurCameraRotateValue = 0;
        private bool RotateTweening = false;

        private class PosTweenInfo
        {
            public Vector3 from = new Vector3();
            public Vector3 to = new Vector3();
        };
        private class ValTweenInfo
        {
            public float from = 0;
            public float to = 0;
        };
        
        public GameUIScene2D()
        {
            Count++;
            mIdx = Count;
            int row = mIdx / SCENE_MAX_COL;
            int col = mIdx % SCENE_MAX_COL;
            mBasePos = SCENE_BASE_POS + new Vector3(-row * SCENE_MAX_SIZE, -col * SCENE_MAX_SIZE, 0);
        }

        protected override void OnInit()
        {
            base.OnInit();
            
            AddDragCallBack(OnDrag);            
        }

        public override void RemoveMount()
        {
            base.RemoveMount();
        }
        
        //放置模型的根节点
        protected override Transform ModelRoot
        {
            get
            {
                if (mSceneRoot == null)
                {
                    string name = $"GameUIModelScene_Root_{mIdx}";
                    GameObject obj = new GameObject(name);
                    Object.DontDestroyOnLoad(obj);
                    mSceneRoot = obj.transform;
                    mSceneRoot.position = new Vector3(0,0,0);
                    mSceneRoot.rotation = Quaternion.identity;
                }
                return mSceneRoot;
            }
        }


        public override void Load(string res)
        {
            //相当于PreLoad了,多次加载的时候，不会多次创建
            if (mRenderCamera == null)
            {
                mRenderCamera = CreateRenderCamera();
            }
            base.Load(res);
        }

        public override void AutoSet()
        {
            base.AutoSet();
            InitPos();
            InitReflect();
        }

        protected void InitPos()
        {
            if(LogicEntity != null)
            {
                LogicEntity.Position = mBasePos;
            }
            if(BackSceneEntity != null)
            {
                BackSceneEntity.Position = mBasePos + mBackSceneOffset;
            }
            if(mRenderCamera != null)
            {
                mRenderCamera.SetPos(mBasePos + mCameraPos);
            }

        }


        public void InitReflect()
        {
            if(BackSceneEntity != null)
            {
                // LogicMirrorReflect reflect = ModelRoot.GetComponentInChildren<LogicMirrorReflect>();
                // if(reflect != null && mRenderCamera != null)
                // {
                //     reflect.Init(mRenderCamera.Camera);
                // }
            }
        }

        /// <summary>
        /// 设置模型旋转角度
        /// </summary>
        /// <param name="y"></param>
        public override void SetModelRotation(float y)
        {
            y = GetDefaultAngle(y);
            if (LogicEntity != null)
            {
                LogicEntity.Rotation = Quaternion.Euler(0, y, 0);
            }
        }

        /// <summary>
        /// 相机是否支持旋转
        /// </summary>
        /// <param name="enable"></param>
        public void SetCameraRotateEnable(bool enable)
        {
            mEnableCameraRotate = enable;
        }

        /// <summary>
        /// 相机旋转角度范围
        /// </summary>
        /// <param name="minAngle"></param>
        /// <param name="maxAngle"></param>
        public void SetCameraRotateRange(int minAngle, int maxAngle)
        {
            mMinVerticalAngle = minAngle;
            mMaxVerticalAngle = maxAngle;
        }

        public void SetIsRotateCamera(bool rotatecamera)
        {
            this.IsRotateCamera = rotatecamera;
        }

        public void InitCamera(Vector3 pos, Vector3 rotate, float fov)
        {
            mCameraPos = pos;
            mCameraRotate = rotate;
            mRenderCamera.SetPos(mBasePos + mCameraPos);
            mRenderCamera.Fov = fov;
            ReCalcCamInfo();
        }

        public void SetCameraFov(float fov)
        {
            if(mRenderCamera != null)
            {
                mRenderCamera.Fov = fov;
            }
        }

        // public void SetCameraBloom(float threshold, float intensity, float blurSize, int blurIterations, bool isUIBloomMask = true)
        // {
        //     if (LightFaceEffect.GetQuality() >= TerrainXQualityLevel.Mid) return;
        //
        //     if (mRenderCamera != null)
        //     {
        //         mRenderCamera.SetBloom(threshold,intensity,blurSize,blurIterations, isUIBloomMask);
        //     }
        // }

        private Light ui_light;

        // public void OpenShadow(bool open = true) {
        //
        //     return;
        //
        //     LightFaceEffect lfe = MonoBehaviour.FindObjectOfType<LightFaceEffect>();
        //     if (lfe != null) {
        //         if (lfe.MainLight != null) {
        //             lfe.MainLight.enabled = !open;
        //         }
        //
        //         if (ui_light == null) {
        //             GameObject ui_lightObj = new GameObject("UI_Light_Obj");
        //             ui_lightObj.transform.parent = mSceneRoot;
        //             ui_lightObj.transform.localPosition = Vector3.zero;
        //             ui_lightObj.transform.localRotation = Quaternion.identity;
        //             ui_lightObj.transform.localScale = Vector3.one;
        //
        //             ui_light = ui_lightObj.AddComponent<Light>();
        //             ui_light.type = LightType.Directional;
        //             ui_light.color = Color.white;
        //             //ui_light.lightmapBakeType = LightmapBakeType.Realtime;
        //             //ui_light.lightmappingMode = LightmappingMode.Realtime;
        //             ui_light.intensity = 1f;
        //             ui_light.shadows = LightShadows.Soft;
        //             ui_light.shadowStrength = 1f;
        //             ui_light.shadowResolution = UnityEngine.Rendering.LightShadowResolution.VeryHigh;
        //             ui_light.cullingMask = 1 << 28;
        //
        //             ui_lightObj.transform.Rotate(new Vector3(0, 135, 0));
        //         }
        //     }
        // }

        public void AutoRotate()
        {
            if (IsRotateCamera&&mRenderCamera.CameraObj && !RotateTweening)
            {
                this.mCurCameraRotateValue = 0;
                RotateTweening = true;
                TimerHandler.SetTimeInterval(updateCameraRotate, 0.005f);
            }
        }
        private void OnRotateTweenFinished()
        {
            TimerHandler.RemoveTimeaction(updateCameraRotate);
            RotateTweening = false;
            this.mCurCameraRotateValue = 0;
        }
        private void updateCameraRotate()
        {
            if (IsRotateCamera && RotateTweening)
            {
                this.mCurCameraRotateValue += 10f* mCameraRotateFactor;
                if (mCurCameraRotateValue > mCameraRotateTarget)
                {
                    OnRotateTweenFinished();
                    return;
                }
                Vector3 v3 = new Vector3(0, 0, 0);
                // 绕x轴旋转，即上下旋转
                //v3.x += delta.y * mCameraRotateFactor;
                // 绕y轴旋转，左右旋转
                v3.y += 10f * mCameraRotateFactor;

                Vector3 axis = new Vector3();
                float angle = 0;
                Quaternion.Euler(v3).ToAngleAxis(out angle, out axis);
                mRenderCamera.Camera.transform.RotateAround(this.mBasePos, axis, angle);
            }
        }

        // 从当前位置平滑移动到目标位置
        public void MoveTo(Vector3 cameraPos, Vector3 rotate, float fov)
        {
            mTweenPos = MUGUI.TweenPosition3D.BeginFrom(mRenderCamera.CameraObj, mTweenTime, mRenderCamera.CameraObj.transform.localPosition, mBasePos + cameraPos);
            mTweenRotation = MUGUI.TweenRotation.Begin(mRenderCamera.CameraObj, mTweenTime, mRenderCamera.CameraObj.transform.localRotation);
            mTweenRotation.to = Quaternion.Euler(rotate).eulerAngles;

            mTweenPos.PlayForward();
            mTweenRotation.PlayForward();
            mTweenPos.SetOnFinishedOneShot(this.OnTweenFinished);

            mCameraPos = cameraPos;
            mCameraRotate = rotate;
            mRenderCamera.Fov = fov;

            if(!mTweenPlaying)
            {
                mEnableCameraRotateCache = mEnableCameraRotate;
                mEnableCameraRotate = false;
            }
            mTweenPlaying = true;
        }

        public void OnTweenFinished()
        {
            mTweenPlaying = false;
            mEnableCameraRotate = mEnableCameraRotateCache;
            ReCalcCamInfo();
        }
        
        public void OnUpdate()
        {
            
        }

        protected override void OnRotaModel(GameObject go, Vector2 delta, Vector2 pos)
        {
            if (!this.IsRotateCamera)
            {
                int count = EasyTouch.GetTouchCount();
                if (count >= 2)
                {
                    return;
                }
                delta = delta * 0.33f;
                base.OnRotaModel(go, delta, pos);
            }
        }

        private void OnDrag(GameObject go, Vector2 delta, Vector2 pos)
        {
            if(mRenderCamera == null || mRenderCamera.Camera == null)
            {
                return;
            }
            int count = EasyTouch.GetTouchCount();
            if(count >= 2)
            {
                return;
            }
            if (!this.IsRotateCamera)
            {
                if (mEnableCameraRotate)
                {
                    // 绕x轴旋转，即上下旋转
                    mCameraRotate.x -= delta.y * mCameraRotateFactor;
                    mCameraRotate.x = Mathf.Max(mCameraRotate.x, mMinVerticalAngle);
                    mCameraRotate.x = Mathf.Min(mCameraRotate.x, mMaxVerticalAngle);

                    // 初始朝向
                    Vector3 forward = new Vector3(0, 0, 1).normalized;
                    // 旋转后朝向
                    forward = Quaternion.Euler(mCameraRotate) * forward;

                    Vector3 lookAtPos = mRenderCamera.TargetPos;
                    //mCameraPos = lookAtPos - forward * mRenderCamera.Distance;
                    mRenderCamera.SetPos(lookAtPos - forward * mRenderCamera.Distance);
                    mRenderCamera.Forward = forward;
                }
            }
            else
            {
                if(RotateTweening)
                {
                    this.OnRotateTweenFinished();
                }
                Vector3 v3 = new Vector3(0, 0, 0);
                // 绕x轴旋转，即上下旋转
                //v3.x += delta.y * mCameraRotateFactor;
                // 绕y轴旋转，左右旋转
                v3.y += delta.x * mCameraRotateFactor;

                Vector3 axis = new Vector3();
                float angle = 0;
                Quaternion.Euler(v3).ToAngleAxis(out angle, out axis);
                mRenderCamera.Camera.transform.RotateAround(this.mBasePos, axis, angle);
            }
        }

        private void ReCalcCamInfo()
        {
            // 相机默认朝向
            Vector3 forward = new Vector3(0, 0, 1).normalized;
            // 旋转后朝向
            forward = Quaternion.Euler(mCameraRotate) * forward;

            // 人物所在位置，面向摄像机的平面法向量(摄像机朝向，去除y旋转就是了）
            Vector3 n = forward;
            n.y = 0;

            // 摄像机朝向与平面的交点，即lookat位置
            Vector3 lookAt = GetIntersectWithLineAndPlane(mRenderCamera.CameraObj.transform.position, forward, n, mBasePos);
            Debug.DrawRay(lookAt, forward);
            mRenderCamera.TargetPos = lookAt;
            mRenderCamera.Distance = (mRenderCamera.CameraObj.transform.position - lookAt).magnitude;
            mRenderCamera.Forward = forward;
        }

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        private Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }
        
        // public void SetMask()
        // {
        //     if (mUITex != null)
        //     {
        //         mUITex.material = ShaderModule.Instance.NpcMaskMaterial;
        //     }
        // }

        private UIModelSceneCamera CreateRenderCamera()
        {
            UIModelSceneCamera rCam = GetRenderCamera();
            //rCam.SetMSAA(true);
            rCam.SetHDR(false);
            rCam.SetOcc(false);
            rCam.SetParent(ModelRoot);
            rCam.SetRenderTextureSize(Screen.width, Screen.height);
            Rect rect = mUITex.rectTransform.rect;
            rCam.SetCameraAspect(rect.width, rect.height);
            rCam.AddTexture(mUITex as RawImage);
            rCam.Distance = SCENE_MAX_SIZE / 2;
            rCam.Orthographic = false;
            rCam.Enable();
            
            return rCam;
        }

        private UIModelSceneCamera GetRenderCamera()
        {
            UIModelSceneCamera rCam = null;
            if (mUnUseList.Count > 0)
            {
                rCam = mUnUseList[0];
                mUnUseList.RemoveAt(0);
            }
            else
            {
                rCam = new UIModelSceneCamera();
                rCam.Init(ModelRoot);
            }
            mUseList.Add(rCam);
            return rCam;
        }

        private void DisposeCamera()
        {
            if (mRenderCamera != null)
            {
                if (mTweenPos != null) GameObject.Destroy(mTweenPos);
                if (mTweenRotation != null) GameObject.Destroy(mTweenRotation);
                mRenderCamera.Destroy();
                mUnUseList.Add(mRenderCamera);
                mRenderCamera = null;
            }
        }

        //private int Dir
        //{

        //}
        public override bool Is3DModel()
        {
            return false;
        }

        public override void DisposeModel()
        {
            base.DisposeModel();
            OnRotateTweenFinished();
            DisposeCamera();
        }
    }


    /// <summary>
    /// camera 里不涉及具体逻辑，只有一些数据
    /// </summary>
    public class UIModelSceneCamera
    {
        protected List<RawImage> mTextureList = new List<RawImage>();
        protected RawImage mTexture = null;
        protected GameObject mCamObj = null;
        protected Camera mCamera = null;
        protected RenderTexture mRenderTex = null;

        // 相机与目标点的距离
        protected float mDistance = 0;
        // 目标点
        protected Vector3 mTargetPos = new Vector3(0, 0, 0);
        /// <summary>
        /// 与目标点的距离
        /// </summary>
        public float Distance
        {
            get
            {
                return mDistance;
            }
            set
            {
                mDistance = value;
            }
        }

        /// <summary>
        /// 相机朝向
        /// </summary>
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

        public Camera Camera
        {
            get
            {
                return mCamera;
            }
        }

        /// <summary>
        /// 相机看着的点
        /// </summary>
        public Vector3 TargetPos
        {
            set
            {
                mTargetPos = value;
            }
            get
            {
                return mTargetPos;
            }
        }


        /// <summary>
        /// 正交、透视相机
        /// </summary>
        public bool Orthographic
        {
            set
            {
                if(mCamera != null)
                {
                    mCamera.orthographic = value;
                }
            }
            get
            {
                if(mCamera != null)
                {
                    return mCamera.orthographic;
                }
                return false;
            }
        }

        /// <summary>
        /// fov
        /// </summary>
        public float Fov
        {
            get
            {
                if(mCamera != null)
                {
                    return mCamera.fieldOfView;
                }
                return 0f;
            }
            set
            {
                if(mCamera != null)
                {
                    mCamera.fieldOfView = value;
                }
            }
        }


        public GameObject CameraObj
        {
            get
            {
                return mCamObj;
            }
        }

        /// <summary>
        /// 初始化摄像机
        /// </summary>
        /// <param name="root">摄像机父节点</param>
        public void Init(Transform root)
        {
            mCamObj = new GameObject();
            mCamObj.name = "Camera";
            mCamera = mCamObj.AddComponent<Camera>();
            mCamera.clearFlags = CameraClearFlags.SolidColor;
            mCamera.backgroundColor = new Color(0, 0, 0, 0);
            mCamera.cullingMask = 1 << RenderLayerDef.RL_ACTOR;
            mCamera.cullingMask |= 1 << RenderLayerDef.RL_EFFECT;
            mCamera.cullingMask |= 1 << 5;
            mCamera.cullingMask |= 1;//武器是default层
            // 默认透视投影
            mCamera.orthographic = false;
            mCamera.nearClipPlane = 0.1f;
            mCamera.farClipPlane = 60f;
            mCamObj.transform.parent = root;
            mCamObj.transform.rotation = Quaternion.Euler(0, 0, 0);
            mCamObj.SetActive(false);
            mCamObj.GetOrAddComponent<UICameraFixParams>();
        }

        public void SetCullingMask(int mask)
        {
            mCamera.cullingMask = mask;
        }

        /// <summary>
        /// set msaa
        /// </summary>
        /// <param name="flag"></param>
        public void SetMSAA(bool flag)
        {
            if (mCamera != null)
            {
                mCamera.allowMSAA = flag;
            }
        }

        /// <summary>
        /// set occlusion
        /// </summary>
        /// <param name="flag"></param>
        public void SetOcc(bool flag)
        {
            if (mCamera != null)
            {
                mCamera.useOcclusionCulling = flag;
            }
        }

        /// <summary>
        /// set HDR
        /// </summary>
        /// <param name="flag"></param>
        public void SetHDR(bool flag)
        {
            if (mCamera != null)
            {
                mCamera.allowHDR = flag;
            }
        }

        /// <summary>
        /// set FarClip
        /// </summary>
        /// <param name="dis"></param>
        public void SetFarClipPlane(float far)
        {
            if (mCamera != null)
            {
                mCamera.farClipPlane = far;
            }
        }

        /// <summary>
        /// set position
        /// </summary>
        /// <param name="pos"></param>
        public void SetPos(Vector3 pos)
        {
            mCamObj.transform.localPosition = pos;
        }

        public void SetParent(Transform parent)
        {
            mCamObj.transform.parent = parent;
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
                    mRenderTex.name = "GameUIScene2D.mRenderTex";
                    //mRenderTex.antiAliasing = QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing;
                    mRenderTex.Create();
                    mCamera.targetTexture = mRenderTex;
                }
            }
            else
            {
                mRenderTex = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
                mRenderTex.name = "GameUIScene2D.mRenderTex";
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

        /// <summary>
        /// 设置bloom
        /// </summary>
        public void SetBloom(float threshold, float intensity, float blurSize, int blurIterations, bool isUIBloomMask = true)
        {
            UnityStandardAssets.ImageEffects.BloomOptimized bloom = mCamObj.GetOrAddComponent<UnityStandardAssets.ImageEffects.BloomOptimized>();
            if(bloom != null)
            {
                bloom.fastBloomShader = Shader.Find("Hidden/FastBloom");
                bloom.threshold = threshold;
                bloom.intensity = intensity;
                bloom.blurSize = blurSize;
                bloom.blurIterations = blurIterations;
                bloom.isUIBloomMask = isUIBloomMask;

                //打开HDR效果会有错误
                if (mCamera != null)
                {
                    mCamera.allowHDR = false;
                }
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
    }

#else

    public class GameUIScene2D : GameUIModelBase
    {

    }

#endif

}
