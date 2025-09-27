using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIEffectSuperHexagon : MonoBehaviour
{
    #region Variables
    public Shader SCShader;
    [Range(0.0f, 1.0f)]
    public float _AlphaHexa = 1.0f;
    private float TimeX = 1.0f;
    private Vector4 ScreenResolution;
    Material SCMaterial;
    [Range(0.2f, 10.0f)]
    public float HexaSize = 2.5f;
    public float _BorderSize = 1.0f;
    public Color _BorderColor = new Color(0.75f, 0.75f, 1, 1);
    public Color _HexaColor = new Color(0, 0.5f, 1, 1);
    public float _SpotSize = 2.5f;
    [Range(-2f, 0f)]
    public float _Progress = 0f;

    public static float ChangeBorderSize = 1.0f;
    public static Color ChangeBorderColor;
    public static Color ChangeHexaColor;
    public static float ChangeSpotSize = 1.0f;
    public static float ChangeAlphaHexa = 1.0f;
    public static float ChangeValue;
    public Vector2 center = new Vector2(0.5f, 0.5f);
    [Range(-1f, 1f)]
    public float Radius = -0.1f;

    public static Vector2 Changecenter;
    public static float ChangeRadius;

    static Texture2D spriteTexture2D = null;
    #endregion

    #region Properties
    Material scmaterial
    {
        get
        {
            if (SCMaterial == null)
            {
                SCMaterial = new Material(SCShader);
                SCMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return SCMaterial;
        }
    }
    #endregion

    RenderTexture renderTexture = null;
    Image image = null;


    public enum EffectType{
        Progress,
        Radius
    }

    public EffectType effectType = EffectType.Progress;

    void Start()
    {
        Changecenter = center;
        ChangeRadius = Radius;
        ChangeValue = HexaSize;
        ChangeAlphaHexa = _AlphaHexa;

        ChangeBorderSize = _BorderSize;
        ChangeBorderColor = _BorderColor;
        ChangeHexaColor = _HexaColor;
        ChangeSpotSize = _SpotSize;

        SCShader = Shader.Find("CameraFilterPack/AAA_Super_Hexagon");

        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        GameObject obj = new GameObject("EffectImage");
        obj.transform.parent = this.gameObject.transform;
        image = obj.AddComponent<Image>();
        image.raycastTarget = true;

        image.material = scmaterial;
        renderTexture = new RenderTexture(256, 256, 1, RenderTextureFormat.ARGB32);
        renderTexture.name = "UIEffectSuperHexagon.renderTexture";
        if (image.material.HasProperty("_MainTex"))
            image.material.SetTexture("_MainTex", renderTexture);

        RectTransform rectTransform = image.rectTransform;
        rectTransform.SetAsLastSibling();
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rectTransform.position = new Vector3(0, 0, 0);
        rectTransform.localScale = new Vector3(1, 1, 1);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition3D = new Vector3( 0, 0, 0);
        rectTransform.sizeDelta = this.GetComponent<RectTransform>().sizeDelta;


//         spriteTexture2D = new Texture2D(renderTexture.width, renderTexture.height);
//         RenderTexture.active = renderTexture;
//         spriteTexture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
//         spriteTexture2D.Apply();
//         RenderTexture.active = null;
//         this.sprite = Sprite.Create(spriteTexture2D, new Rect(0, 0, spriteTexture2D.width, spriteTexture2D.height), new Vector2(0.5f, 0.5f));
    }

    void OnValidate()
    {
        ChangeValue = HexaSize;
        Changecenter = center;
        ChangeRadius = Radius;
        ChangeBorderSize = _BorderSize;
        ChangeBorderColor = _BorderColor;
        ChangeHexaColor = _HexaColor;
        ChangeSpotSize = _SpotSize;
        ChangeAlphaHexa = _AlphaHexa;
    }
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {
            if (image.enabled == false)
                return;
            switch (effectType)
            {
                case EffectType.Progress:
                    _Progress -= Time.deltaTime*4;
                    if (_Progress < -2)
                    {
                        _Progress = 0;
                        image.enabled = false;
                    }
                    break;
                case EffectType.Radius:
                    ChangeRadius += Time.deltaTime * 4;
                    if (ChangeRadius > 1)
                    {
                        ChangeRadius = -0.1f;
                        image.enabled = false;
                    }
                    break;
            }

            HexaSize = ChangeValue;
            center = Changecenter;
            Radius = ChangeRadius;
            _BorderSize = ChangeBorderSize;
            _BorderColor = ChangeBorderColor;
            _HexaColor = ChangeHexaColor;
            _SpotSize = ChangeSpotSize;
            _AlphaHexa = ChangeAlphaHexa;


            TimeX += Time.deltaTime;
            if (TimeX > 100)
            {
                TimeX = 0;
            }
            image.material.SetFloat("_TimeX", TimeX);
            image.material.SetFloat("_Value", HexaSize);
            image.material.SetFloat("_PositionX", center.x);
            image.material.SetFloat("_PositionY", center.y);
            image.material.SetFloat("_Radius", Radius);
            image.material.SetFloat("_BorderSize", _BorderSize);
            image.material.SetColor("_BorderColor", _BorderColor);
            image.material.SetColor("_HexaColor", _HexaColor);
            image.material.SetFloat("_AlphaHexa", _AlphaHexa);
            image.material.SetFloat("_SpotSize", _SpotSize);
            image.material.SetFloat("_Progress", _Progress);
            image.material.SetVector("_ScreenResolution", new Vector4(image.material.mainTexture.width, image.material.mainTexture.height, 0.0f, 0.0f));

        }
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            SCShader = Shader.Find("CameraFilterPack/AAA_Super_Hexagon");
        }
#endif

    }

    void OnDisable()
    {
        image.enabled = true;
        _Progress = 0;
        Radius = -1;
//         if (SCMaterial)
//         {
//             DestroyImmediate(SCMaterial);
//         }
    }
}
