using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]
[ExecuteInEditMode]
public class UVAnimation : MonoBehaviour
{
    public bool allowCurve = false;
    public AnimationCurve curveX = null;
    public AnimationCurve curveY = null;
    public Vector2 Direction = Vector2.zero;
    public float Speed = 0;

    private float mTime = 0;
    private Vector2 oldOffset;
    private Vector2 localOffset = Vector2.zero;

    private Material TheMat
    {
        get
        {
            if (cachedMat == null)
            {
                InitializeMaterial();
            }
            return cachedMat;
        }
    }
    private Material cachedMat;

    private bool hasMatInitialized = false;

    private void SetValue()
    {
        if (localOffset != null && TheMat != null)
        {
            if (TheMat.HasProperty("_MainTex"))
            {
                TheMat.SetTextureOffset("_MainTex", localOffset);
            }
        }
    }

    private void InitializeMaterial()
    {
        if (hasMatInitialized)
        {
            return;
        }

        Renderer ren = this.gameObject.GetComponent<Renderer>();
        if (ren != null)
        {
            cachedMat = ren.sharedMaterial;
        }

        if (cachedMat != null && cachedMat.HasProperty("_MainTex"))
        {
            localOffset = oldOffset = cachedMat.GetTextureOffset("_MainTex");
        }
        else
        {
            localOffset = oldOffset = Vector2.zero;
        }

        hasMatInitialized = true;
    }

    private void Start()
    {
        mTime = 0;
    }

    private void OnEnable()
    {
        mTime = 0;
    }

    private void Update()
    {
        if (allowCurve && (curveX != null) && (curveY != null))
        {
            mTime += Time.deltaTime;
            localOffset.x = curveX.Evaluate(mTime);
            localOffset.y = curveY.Evaluate(mTime);
        }
        else
        {
            localOffset += Time.deltaTime * Direction * Speed;
        }
        SetValue();
    }

    private void OnDisable()
    {
        localOffset = oldOffset;
        SetValue();

        cachedMat = null;
        hasMatInitialized = false;
    }

    private void OnDestroy()
    {
        localOffset = oldOffset;
        SetValue();

        cachedMat = null;
        hasMatInitialized = false;
    }

#if UNITY_EDITOR

    private bool isPaused;

    public float CurrentTime
    {
        get { return mTime; }
        set { mTime = value; }
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Resue()
    {
        isPaused = false;
    }

    /// <summary>
    /// 将UV滚动的JS脚本升级为该 UVAnimation 脚本
    /// </summary>
    /// <param name="node">待处理的节点</param>
    /// <returns>是否实际对节点进行了修改</returns>
    public static bool UpgradeUVScrollJavaScript(GameObject node)
    {
        if (node == null)
        {
            return false;
        }

        float? speedLR = null;
        float? speedUD = null;

        Component lrCom = node.GetComponent("uv_l-r");
        if (lrCom != null)
        {
            speedLR = (float)(lrCom.GetType().GetField("scrollSpeed").GetValue(lrCom));
            GameObject.DestroyImmediate(lrCom);
        }

        Component udCom = node.GetComponent("uv_u-d");
        if (udCom != null)
        {
            speedUD = (float)(udCom.GetType().GetField("scrollSpeed").GetValue(udCom));
            GameObject.DestroyImmediate(udCom);
        }

        if (speedLR == null && speedUD == null)
        {
            return false;
        }
        else
        {
            UVAnimation uvAnim = node.GetOrAddComponent<UVAnimation>();
            uvAnim.allowCurve = false;
            uvAnim.Speed = -1;
            uvAnim.Direction = new Vector2(speedLR.HasValue ? speedLR.Value : 0, speedUD.HasValue ? speedUD.Value : 0);

            return true;
        }
    }

#endif

}
