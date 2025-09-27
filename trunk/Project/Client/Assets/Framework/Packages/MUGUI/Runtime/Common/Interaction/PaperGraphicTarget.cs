using System.Collections.Generic;
using Game;
using Game.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    //该类用于扩展PaperGraphic，可以指定某些界面作为被“卷纸”的目标
    [RequireComponent(typeof(BasePaperGraphic))]
    [DisallowMultipleComponent]
    public class PaperGraphicTarget : MonoBehaviour
    {
        //暂时只支持单个节点，该节点与所有子节点都会被当做目标
        [SerializeField]
        private RectTransform targetRoot;

        //目前仅用于目标节点是PaperGraphic的直接父节点，并且拥有一个Canvas的情况（需要拍摄整个UI）
        //如果该值为true,那么在这种情况下，会“伪造”一个父节点，并且在动画期间，将PaperGraphic的父节点设为伪造的父节点
        //这样，动画中直接控制PaperGraphic的局部坐标也能生效
        //注意：由于更改了节点路径，如果在播放动画的过程中，Animator 重新进行了绑定，可能会导致动画失效
        [SerializeField]
        private bool allowdUseFakeParent = true;

        private BasePaperGraphic _paperGraphic;
        public BasePaperGraphic paperGraphic
        {
            get
            {
                if (_paperGraphic == null)
                {
                    _paperGraphic = GetComponent<BasePaperGraphic>();
                }
                return _paperGraphic;
            }
        }

        private void OnDestroy()
        {
            if (currentPaperCamera != null)
            {
                ExitPaperState();
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (currentPaperCamera != null)
            {
                ExitPaperState();
            }
        }
#endif
    
        //开启或者关闭“Paper”效果
        private bool isPaperState = false;
        public void SetPaperState(bool paperState)
        {
            if (targetRoot == null || isPaperState == paperState)
            {
                return;
            }
            bool result = false;
            if (paperState)
            {
                result = EnterPaperState();
            }
            else
            {
                result = ExitPaperState();
            }
            if (result)
            {
                isPaperState = paperState;
            }
        }

        //PaperGraphic与UI被更改前的相关数据
        private Vector3 originUIPosiiton;
        private bool needResetPaperPos = false;
        private Vector3 originPaperLocalPosition;
        //private Transform originPaperParent = null;
        private RectTransform fakeGraphicParent = null;
        private int orginSiblingIndex = 0;

        private Texture originFrontImage;
        private Camera currentPaperCamera;
        private RenderMode originRenderMode;

        //这个变量表示当前是否在做一些奇怪的操作，操作未完成前不能再次进入Paper状态
        private bool CanEnterPaperState = true;

        //进入Paper状态，更改目标UI的位置，分配一个摄像机去拍摄它，并且将摄像机的渲染结果作为PaperGraphic的正面图来源
        private bool EnterPaperState()
        {
            if (!CanEnterPaperState)
            {
                return false;
            }
            //并不是任何UI都可以渲染到“纸”上,目前，要求目标节点的根Canvas必须是ScreenWorldCamera模式(否则只有某一个摄像机可以拍摄UI)
            var canvas = targetRoot.GetComponentInParent<Canvas>();
            canvas = canvas != null ? canvas.rootCanvas : null;
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay || canvas.worldCamera == null)
            {
                return false;
            }
            currentPaperCamera = GetPaperCamera(canvas.worldCamera);
            if (currentPaperCamera == null)
            {
                return false;
            }
            originFrontImage = paperGraphic.FrontImage;
            originRenderMode = canvas.renderMode;
            canvas.renderMode = RenderMode.WorldSpace;
            var rect = paperGraphic.rectTransform.rect;

            originUIPosiiton = targetRoot.localPosition;
            //将摄像机的照射范围与PaperGraphic的范围一致，超出部分将不会被显示（也找不到合适的地方进行显示）
            currentPaperCamera.orthographicSize = paperGraphic.rectTransform.TransformVector(0, rect.height * 0.5f, 0).y;
            currentPaperCamera.aspect = rect.width / rect.height;
            //为了兼容PaperGraphic属于TargetRoot的子节点的情况
            needResetPaperPos = paperGraphic.transform.IsChildOf(targetRoot);
            if (needResetPaperPos)
            {
                if(allowdUseFakeParent && targetRoot == paperGraphic.transform.parent && targetRoot.GetComponent<Canvas>() != null)
                {
                    //使用“假”的父节点，由于目前只考虑父节点是界面根节点的情况,所以只需要复制必要的数据
                    RectTransform root = fakeRootPool.Get();
                    root.gameObject.layer = targetRoot.gameObject.layer;
                    root.parent = targetRoot.parent;
                    root.pivot = targetRoot.pivot;
                    root.sizeDelta = targetRoot.sizeDelta;
                    root.localPosition = targetRoot.localPosition;
                    root.localScale = targetRoot.localScale;
                    Canvas targetCanvas = targetRoot.GetComponent<Canvas>();
                    Canvas newCanvas = root.GetComponent<Canvas>();
                    newCanvas.worldCamera = targetCanvas.worldCamera;
                    newCanvas.sortingLayerID = targetCanvas.sortingLayerID;
                    newCanvas.sortingOrder = targetCanvas.sortingOrder;

                    //Canvas Scaler的影响暂时忽略

                    fakeGraphicParent = root;
                    orginSiblingIndex = paperGraphic.transform.GetSiblingIndex();
                    paperGraphic.transform.SetParent(fakeGraphicParent, true);

                    targetRoot.position = new Vector3(currentPaperCamera.transform.position.x, currentPaperCamera.transform.position.y, targetRoot.position.z);
                }
                else
                {
                    fakeGraphicParent = null;
                    //不使用“假”的父节点，直接更改PaperGraphic的位置，注意这种清苦下，其它地方（主要是动画）不要更改PaperGraphic的位置
                    originPaperLocalPosition = paperGraphic.transform.localPosition;
                    var originPaperPosition = paperGraphic.transform.position;
                    //目前，假定目标RectTransform的pivot为（0.5,0.5）
                    targetRoot.position = new Vector3(currentPaperCamera.transform.position.x, currentPaperCamera.transform.position.y, targetRoot.position.z);
                    paperGraphic.transform.position = originPaperPosition;
                }
            }
            else
            {
                targetRoot.position = new Vector3(currentPaperCamera.transform.position.x, currentPaperCamera.transform.position.y, targetRoot.position.z);
            }

            //currentPaperCamera.targetTexture = new RenderTexture((int)rect.width, (int)rect.height, 24);
            paperGraphic.FrontImage = currentPaperCamera.targetTexture;
            paperGraphic.SetRenderCamera(currentPaperCamera);

            return true;
        }
        //退出Paper状态，将摄像机释放，将UI还原，将PaperGraphic的正面图来源重置回去
        private bool ExitPaperState()
        {
            if(paperGraphic == null || targetRoot == null)
            {
                //目前只可能在编辑器模式下，运行过程中修改了代码，导致信息缺失，才会出现这种情况
                //目前无法正确处理这种情况，直接返回
                return false;
            }
            if(targetRoot.GetComponentInParent<Canvas>() == null)
            {
                return false;
            }

            paperGraphic.FrontImage = originFrontImage;
            targetRoot.localPosition = originUIPosiiton;
            if (needResetPaperPos)
            {
                if(fakeGraphicParent == null)
                {
                    paperGraphic.transform.localPosition = originPaperLocalPosition;
                }
                else
                {
                    paperGraphic.transform.SetParent(targetRoot);
                    paperGraphic.transform.SetSiblingIndex(orginSiblingIndex);
                    fakeRootPool.Release(fakeGraphicParent);
                    fakeGraphicParent = null;
                }
                needResetPaperPos = false;
            }
            ReleaseCamera(currentPaperCamera);
            currentPaperCamera = null;

            //Canvas.ForceUpdateCanvases();
            //canvas.worldCamera.Render();
            //只更改UI位置，不会发生问题，只更改canvas的RenderMode也不会有问题，但是如果两者在同一帧中发生，就会有很奇怪的显示效果发生
            //这可能是Unity内部的Bug，这里不做深究（没有源代码怎么深究？），暂时的解决办法是：这一帧更改位置，等到下一帧再更改RenderMode
            var canvas = targetRoot.GetComponentInParent<Canvas>().rootCanvas;
            if (originRenderMode != RenderMode.WorldSpace)
            {

#if UNITY_EDITOR_PROJECT
                //Editor没有Common,直接立即设置
                canvas.renderMode = originRenderMode;
#else
                 CanEnterPaperState = false;
                 Common.NextFrameExecute += () =>
                 {
                     canvas.renderMode = originRenderMode;
                     //骚操作完成，可以再次进入Paper状态了
                     CanEnterPaperState = true;
                 };
#endif
            }
            return true;
        }

        //共同摄像机对象池
        private static ObjectPool<Camera> paperCameraPool = new ObjectPool<Camera>(OnCreateCamera, OnGetCamera, OnReleaseCamera);
        private static ObjectPool<RectTransform> fakeRootPool = new ObjectPool<RectTransform>(OnCreateFakeRoot, OnGetRoot, OnReleaseRoot);
        //摄像机的范围相关
        //private static Vector2 StartPosition = new Vector2(1000, 0);
        private static Vector3 OffsetPosition = new Vector3(1500, 0, 0);
        private const int MaxCameraCount = 10;
        private static Dictionary<int, Camera> UsedIndexDic = new Dictionary<int, Camera>();
        //获取当前可用的一个用来拍摄目标UI的摄像机
        private static Camera GetPaperCamera(Camera originCamera)
        {
            //给Camera分配一个尽量不会发生重复的位置
            if (UsedIndexDic.Count == 0)
            {
                for (int i = 0; i < MaxCameraCount; i++)
                {
                    UsedIndexDic.Add(i, null);
                }
            }
            int useableIndex = -1;
            foreach (var pair in UsedIndexDic)
            {
                if (pair.Value == null)
                {
                    useableIndex = pair.Key;
                    break;
                }
            }
            if (useableIndex == -1)
            {
                Debug.LogError("Too Many Paper Camera !! Please Ensure Relase them if not needed");
                return null;
            }

            var camera = paperCameraPool.Get();
            UsedIndexDic[useableIndex] = camera;

            camera.orthographic = originCamera.orthographic;
            Vector3 position = originCamera.transform.position + OffsetPosition * (useableIndex + 1);
            camera.transform.position = new Vector3(position.x, position.y, originCamera.transform.position.z);
            camera.transform.rotation = originCamera.transform.rotation;

            return camera;
        }
        //释放指定的摄像机
        private static void ReleaseCamera(Camera camera)
        {
            int usedIndex = -1;
            foreach (var pair in UsedIndexDic)
            {
                if (pair.Value == camera)
                {
                    usedIndex = pair.Key;
                    break;
                }
            }
            if (usedIndex == -1)
            {
                Debug.LogError("Release a not allocated paper camera");
                return;
            }
            UsedIndexDic[usedIndex] = null;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                DestroyImmediate(camera);
                return;
            }
#endif
            paperCameraPool.Release(camera);
        }

        private const int UILayer = 5;

        private static int renderTextureCount = 0;

        private static void OnCreateCamera(Camera camera)
        {
            DontDestroyOnLoad(camera.gameObject);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0, 0, 0, 0);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 10000f;
            camera.cullingMask = 1 << UILayer;
            camera.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            //一个Camera固定分配一个RenderTexture，该Texture的尺寸应该在保证效果可以接收的情况下，尽可能的小
            camera.targetTexture = RenderTexture.GetTemporary(800,450);// new RenderTexture(1280, 720, 24);

            renderTextureCount++;
            if(renderTextureCount > 1)
            {
                Debug.LogError("PaperGraphic Target 创建了超过一个RenderTexture !! , 这不符合预期情况，请确认此处是否能够进行优化 ， 当前创建的RenderTexture数量为 : " + renderTextureCount);
            }
        }
        private static void OnGetCamera(Camera camera)
        {
            camera.enabled = true;
        }
        private static void OnReleaseCamera(Camera camera)
        {
            camera.enabled = false;
        }

        private static void OnCreateFakeRoot(RectTransform root)
        {
            DontDestroyOnLoad(root.gameObject);
            var canvas = root.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            //obj.transform.parent = null;
            //obj.SetActive(false);
        }
        private static void OnGetRoot(RectTransform root)
        {
            root.gameObject.SetActive(true);
        }
        private static void OnReleaseRoot(RectTransform root)
        {
            root.parent = null;
            root.gameObject.SetActive(false);
        }
    }

    //复制UGUI的代码
    public class ObjectPool<T> where T : Component
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly UnityAction<T> m_actionOnCreate;
        private readonly UnityAction<T> m_ActionOnGet;
        private readonly UnityAction<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(UnityAction<T> actionOnCreate, UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease)
        {
            m_actionOnCreate = actionOnCreate;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                GameObject obj = new GameObject(typeof(T).ToString());
                element = obj.GetOrAddComponent<T>();
                if (m_actionOnCreate != null)
                {
                    m_actionOnCreate(element);
                }
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }

    }
}

