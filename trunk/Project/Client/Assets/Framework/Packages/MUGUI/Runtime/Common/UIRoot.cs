using Game;
using Game.Core;
using UnityEngine;

namespace Game.UI
{
    public class UIRoot : MonoSingleton<UIRoot>
    {
        public const string UI_ROOT_NAME = "UIRoot";
        public const string UI_CAMERA_NAME = "UICamera";
        public const string UI_EVENTSYSTEM_NAME = "EventSystem";
        
        private static UIPanelLayer[] LayerList;
        public static Canvas UIRootCanvas{ get; private set; }
        public static Camera UICamera{ get; private set; }
        public static UIPanelLayer UIBaseLayer => GetUIPanelLayer(UILayerEnums.UIBaseLayer);
        public static UIPanelLayer UIGameLayer => GetUIPanelLayer(UILayerEnums.UIGameLayer);
        public static UIPanelLayer UIDefaultLayer => GetUIPanelLayer(UILayerEnums.UIDefaultLayer);
        public static UIPanelLayer UIPopupLayer => GetUIPanelLayer(UILayerEnums.UIPopupLayer);
        public static UIPanelLayer UINoticeLayer => GetUIPanelLayer(UILayerEnums.UINoticeLayer);
        public static UIPanelLayer UILoadingLayer => GetUIPanelLayer(UILayerEnums.UILoadingLayer);

        private const int MAX_INDEX = (int)UILayerEnums.MAX_VALUE % 10;
        
        public static UIPanelLayer GetUIPanelLayer(UILayerEnums layerEnums)
        {
            if (LayerList == null || LayerList.Length == 0) return null;
            int index = (int)layerEnums % 10;
            return LayerList[index];
        }
        
        public void InitUIRoot()
        {
            this.SetUIPanelLayer(UILayerEnums.UIBaseLayer);
            this.SetUIPanelLayer(UILayerEnums.UIGameLayer);
            this.SetUIPanelLayer(UILayerEnums.UIDefaultLayer);
            this.SetUIPanelLayer(UILayerEnums.UIPopupLayer);
            this.SetUIPanelLayer(UILayerEnums.UINoticeLayer);
            this.SetUIPanelLayer(UILayerEnums.UILoadingLayer);
            this.SetCamera(UI_CAMERA_NAME);
            MUGUI.GOGUITools.UICamera = UIRoot.UICamera;
        }

        private void SetUIPanelLayer(UILayerEnums layerEnums)
        {
            GameObject obj = this.gameObject.GetGameObjectByID(layerEnums.ToString());
            UIPanelLayer canvas = obj == null ? UnityUtil.CreateUIObject<UIPanelLayer>(layerEnums.ToString(), this.transform) : obj.AddOneComponent<UIPanelLayer>();
            if (LayerList == null || LayerList.Length == 0) LayerList = new UIPanelLayer[MAX_INDEX];
            int index = (int)layerEnums % 10;
            LayerList[index] = canvas;
            this.InitUIPanelLayer(layerEnums, canvas);
#if UNITY_EDITOR
            if (_LayerList == null || _LayerList.Length == 0) _LayerList = new UIPanelLayer[MAX_INDEX];
            _LayerList[index] = canvas;
#endif
        }
        
        private void SetCamera(string name)
        {
            GameObject obj = this.gameObject.GetGameObjectByID(name);
            Camera camera = obj == null ? UnityUtil.CreateUIObject<Camera>(name, this.transform) : obj.AddOneComponent<Camera>();
            UIRootCanvas = this.gameObject.GetComponent<Canvas>();
            UIRootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            UIRootCanvas.worldCamera = camera;
            UICamera = camera;
            this.InitUICamera(camera);
#if UNITY_EDITOR
            _UICamera = camera;
#endif
        }
        
        private void InitUICamera(Camera uiCamera)
        {
            var rectTrans = this.gameObject.GetComponent<RectTransform>();
            rectTrans.SetAnchorsStretch();
            rectTrans.localPosition = new Vector3(0, 0, -500);
            uiCamera.clearFlags = CameraClearFlags.Depth;
            uiCamera.cullingMask = -1;                            //Everything
            uiCamera.orthographic = true;
            uiCamera.orthographicSize = 5;
            uiCamera.depth = 0;
            uiCamera.nearClipPlane = 0.3f;
            uiCamera.farClipPlane = 50;
        }

        private void InitUIPanelLayer(UILayerEnums layerEnums, UIPanelLayer canvas)
        {
            var rectTrans = this.gameObject.GetComponent<RectTransform>();
            rectTrans.SetAnchorsStretch();
            canvas.SortingOrder = (int)layerEnums;
            canvas.LayerName = layerEnums.ToString();
        }
        
#if UNITY_EDITOR
        
        [SerializeField] private Camera _UICamera;
        [SerializeField] private UIPanelLayer[] _LayerList;
        
#endif
        
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        
        public Canvas GetNearestCanvas(GameObject gameObject)
        {
            if (gameObject == null) { return null; }
            var currentTrans = gameObject.transform;
            Canvas lastCanvas = null;
            while (currentTrans != null && currentTrans != this.transform)
            {
                var canvas = currentTrans.GetComponent<Canvas>();
                if (canvas != null && canvas.enabled)
                {
                    lastCanvas = canvas;
                    //原来发现Unity有如下Bug，所以不使用RootCanvas相关的属性 （更换Unity版本之后不知道有没有了，不过先当做也有吧）
                    //如果是子界面动态添加到某个节点下面，虽然看上去子界面上的Canvas已经从RootCanvas转化成了子Canvas，
                    //但是它的isRootCanvas,rootCanvas这些属性并不会重置。 SB Unity
                    if (lastCanvas.overrideSorting)
                    {
                        break;
                    }
                }
                
                currentTrans = currentTrans.parent;
            }
            
            if(lastCanvas != null)
            {
                return lastCanvas;
            }

            return null;
        }
        
    }
}