using UnityEngine;
using Game.UI;
using UnityEngine.UI;

public class BoxBlurSprite : Image
{
    #region Static Data

    //如果编辑器 这里应设置为Game试图 宽高
    static int ScreenWidth = 0;
    static int ScreenHeight = 0;

    static Color32 ClearRTColor = new Color32(0,0,0,0);


    static RenderTexture _RenderTexture = null;
    static RenderTexture BlurRenderTexture
    {
        get {
            if (_RenderTexture == null)
            {
#if UNITY_EDITOR
                Vector2 screenWH = Editor_GetScreenPixelDimensions(UICamera);
                ScreenWidth = (int)screenWH.x;
                ScreenHeight = (int)screenWH.y;
#else
                ScreenWidth = Mathf.Max(Screen.width, Screen.height);
                ScreenHeight = Mathf.Min(Screen.width, Screen.height);
#endif
#if UNITY_EDITOR
                _RenderTexture = new RenderTexture(ScreenWidth/2, ScreenHeight/2, 1, RenderTextureFormat.ARGB32);
#else
            // 可以宽高除2
			_RenderTexture = new RenderTexture(ScreenWidth/2, ScreenHeight/2, 1, RenderTextureFormat.ARGB32);
#endif
                _RenderTexture.name = "BoxBlurSprite._RenderTexture";
                _RenderTexture.filterMode = FilterMode.Trilinear;
            }
            return _RenderTexture;
        }
    }

    static Texture2D spriteTexture2D = null;


    static Camera _camera = null;
    static Camera UICamera => UIRoot.UICamera;
    
    //static CameraFilterPack_Blur_Blurry _blurry = null;
    //static CameraFilterPack_Blur_Blurry Blurry
    //{
    //    get
    //    {
    //        if (_blurry == null)
    //        {
    //            _blurry = UICamera.gameObject.AddComponent<CameraFilterPack_Blur_Blurry>();
    //        }

    //        return _blurry;
    //    }
    //}

    #endregion

    #region Member Data
    private bool isInit = false;

    //应在UGUI生成AssetBundle时赋值
    public CanvasScaler canvasScaler = null;

    #endregion

    public Image waitImage = null;

    public void OnEnableDraw()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            return;
        }
#endif
        if (waitImage != null)
        {
            if (waitImage.sprite == null)
            {
                this.enabled = false;
                return;
            }
        }

        Camera gameCamera = Camera.main;

        CameraClearFlags gflags = gameCamera.clearFlags;
        gameCamera.clearFlags = CameraClearFlags.Nothing;


        gameCamera.targetTexture = BlurRenderTexture;
        //Real Clear
        gameCamera.clearFlags = CameraClearFlags.Color;
        gameCamera.backgroundColor = new Color32(0, 0, 0, 0);
        gameCamera.RenderWithShader(null, null);

        //Blurry.enabled = true;
        //gameCamera.RenderWithShader(null, null);
        //Blurry.enabled = false;
        gameCamera.targetTexture = null;
        gameCamera.clearFlags = gflags;

        Camera uiCamera = UICamera;

        CameraClearFlags flags = uiCamera.clearFlags;
        uiCamera.clearFlags = CameraClearFlags.Nothing;
        

        uiCamera.targetTexture = BlurRenderTexture;
        //Real Clear
        uiCamera.RenderWithShader(null, null);
        //Blurry.enabled = true;
        //uiCamera.RenderWithShader(null, null);
        //Blurry.enabled = false;
        uiCamera.targetTexture = null;
        uiCamera.clearFlags = flags;

        if (spriteTexture2D == null)
        {
            spriteTexture2D = new Texture2D(_RenderTexture.width, _RenderTexture.height);
        }

        RenderTexture.active = _RenderTexture;
        spriteTexture2D.ReadPixels(new Rect(0, 0, _RenderTexture.width, _RenderTexture.height), 0, 0);
        spriteTexture2D.Apply();
        RenderTexture.active = null;

        if (isInit)
            return;
        this.enabled = true;
        RectTransform temp = this.GetComponent<RectTransform>();

        Transform parentTransform = this.gameObject.transform;
        while (canvasScaler == null && parentTransform != null)
        {
            canvasScaler = parentTransform.GetComponent<CanvasScaler>();

            parentTransform = parentTransform.transform.parent;
        }

        temp.anchorMin = new Vector2(0.5f, 0.5f);
        temp.anchorMax = new Vector2(0.5f, 0.5f);
        temp.pivot = new Vector2(0.5f, 0.5f);
        if (this.isInit == false)
        {
            this.sprite = Sprite.Create(spriteTexture2D, new Rect(0, 0, ScreenWidth / 2, ScreenHeight / 2), new Vector2(0.5f, 0.5f));
        }


        if (canvasScaler != null)
        {
            if (canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                float percentage = canvasScaler.matchWidthOrHeight;

                Vector2 resolution = canvasScaler.referenceResolution;

                float widthRatio = resolution.x / (float)ScreenWidth;
                float heightRatio = resolution.y / (float)ScreenHeight;

                float resolutionReal = widthRatio * (1 - percentage) + heightRatio * percentage;

                float realWidth = resolutionReal * ScreenWidth;
                float readHeight = resolutionReal * ScreenHeight;

                temp.sizeDelta = new Vector2(realWidth, readHeight);
            }
            else {
                D.warn("BoxBlurSprite uiScaleMode Set Error!!!!!!!!!!!!!");
            }
        }


        isInit = true;
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();
        /*
        if (_RenderTexture != null) {
            GameObject.Destroy(this.sprite);
            _RenderTexture = null;
        }

        if (this.sprite != null) {
            GameObject.Destroy(this.sprite.texture);
            this.sprite = null;
        }
        this.material = null;
         * */
        if (this.sprite != null)
        {
            GameObject.Destroy(this.sprite);
            this.sprite = null;
        }
        isInit = false;

    }

    protected override void Awake()
    {
        base.Awake();
    }

    #region 获取编辑器分辨率


#if UNITY_EDITOR
    public static Vector2 Editor_GetScreenPixelDimensions(Camera ScreenCamera)
    {
        Vector2 dimensions = new Vector2(ScreenCamera.pixelWidth, ScreenCamera.pixelHeight);


        // 获取编辑器 GameView 的分辨率
        float gameViewPixelWidth = 0, gameViewPixelHeight = 0;
        float gameViewAspect = 0;

        if (Editor__GetGameViewSize(out gameViewPixelWidth, out gameViewPixelHeight, out gameViewAspect))
        {
            if (gameViewPixelWidth != 0 && gameViewPixelHeight != 0)
            {
                dimensions.x = gameViewPixelWidth;
                dimensions.y = gameViewPixelHeight;
            }
        }


        return dimensions;
    }
#endif

    #if UNITY_EDITOR
    static bool Editor__getGameViewSizeError = false;
    public static bool Editor__gameViewReflectionError = false;

    // 尝试获取 GameView 的分辨率
    // 当正确获取到 GameView 的分辨率时，返回 true
    public static bool Editor__GetGameViewSize(out float width, out float height, out float aspect)
    {
        try
        {
            Editor__gameViewReflectionError = false;

            System.Type gameViewType = System.Type.GetType("UnityEditor.GameView,UnityEditor");
            System.Reflection.MethodInfo GetMainGameView = gameViewType.GetMethod("GetMainGameView", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            object mainGameViewInst = GetMainGameView.Invoke(null, null);
            if (mainGameViewInst == null)
            {
                width = height = aspect = 0;
                return false;
            }
            System.Reflection.FieldInfo s_viewModeResolutions = gameViewType.GetField("s_viewModeResolutions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (s_viewModeResolutions == null)
            {
                System.Reflection.PropertyInfo currentGameViewSize = gameViewType.GetProperty("currentGameViewSize", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                object gameViewSize = currentGameViewSize.GetValue(mainGameViewInst, null);
                System.Type gameViewSizeType = gameViewSize.GetType();
                int gvWidth = (int)gameViewSizeType.GetProperty("width").GetValue(gameViewSize, null);
                int gvHeight = (int)gameViewSizeType.GetProperty("height").GetValue(gameViewSize, null);
                int gvSizeType = (int)gameViewSizeType.GetProperty("sizeType").GetValue(gameViewSize, null);
                if (gvWidth == 0 || gvHeight == 0)
                {
                    width = height = aspect = 0;
                    return false;
                }
                else if (gvSizeType == 0)
                {
                    width = height = 0;
                    aspect = (float)gvWidth / (float)gvHeight;
                    return true;
                }
                else
                {
                    width = gvWidth; height = gvHeight;
                    aspect = (float)gvWidth / (float)gvHeight;
                    return true;
                }
            }
            else
            {
                Vector2[] viewModeResolutions = (Vector2[])s_viewModeResolutions.GetValue(null);
                float[] viewModeAspects = (float[])gameViewType.GetField("s_viewModeAspects", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                string[] viewModeStrings = (string[])gameViewType.GetField("s_viewModeAspectStrings", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                if (mainGameViewInst != null
                    && viewModeStrings != null
                    && viewModeResolutions != null && viewModeAspects != null)
                {
                    int aspectRatio = (int)gameViewType.GetField("m_AspectRatio", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).GetValue(mainGameViewInst);
                    string thisViewModeString = viewModeStrings[aspectRatio];
                    if (thisViewModeString.Contains("Standalone"))
                    {
                        width = UnityEditor.PlayerSettings.defaultScreenWidth; height = UnityEditor.PlayerSettings.defaultScreenHeight;
                        aspect = width / height;
                    }
                    else if (thisViewModeString.Contains("Web"))
                    {
                        width = UnityEditor.PlayerSettings.defaultWebScreenWidth; height = UnityEditor.PlayerSettings.defaultWebScreenHeight;
                        aspect = width / height;
                    }
                    else
                    {
                        width = viewModeResolutions[aspectRatio].x; height = viewModeResolutions[aspectRatio].y;
                        aspect = viewModeAspects[aspectRatio];
                        // this is an error state
                        if (width == 0 && height == 0 && aspect == 0)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        catch (System.Exception e)
        {
            if (Editor__getGameViewSizeError == false)
            {
                Debug.LogError("GameCamera.GetGameViewSize - has a Unity update broken this?\nThis is not a fatal error !\n" + e.ToString());
                Editor__getGameViewSizeError = true;
            }
            Editor__gameViewReflectionError = true;
        }
        width = height = aspect = 0;
        return false;
    }
    #endif

    #endregion
}
