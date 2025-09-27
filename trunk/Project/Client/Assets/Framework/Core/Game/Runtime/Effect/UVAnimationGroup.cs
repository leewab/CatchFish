using UnityEngine;

[System.Reflection.Obfuscation(Exclude = true)]

public class UVAnimationGroup : MonoBehaviour {

    public int MaterialID = -1;

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
            if (MaterialID >= 0 && MaterialID < ren.sharedMaterials.Length)
            {
                cachedMat = ren.sharedMaterials[MaterialID];
            }
            else
            {
                cachedMat = null;
            }
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

    private void Update() {
        if (allowCurve && (curveX != null) && (curveY != null)) {
            mTime += Time.deltaTime;
            localOffset.x = curveX.Evaluate(mTime);
            localOffset.y = curveY.Evaluate(mTime);
        }
        else {
            localOffset += Time.deltaTime * Direction * Speed;
        }
        SetValue();
    }

    public void EditorStart() {
        mTime = 0;
    }

    public void EditorUpdate(float delta) {
        if (allowCurve && (curveX != null) && (curveY != null)) {
            mTime += delta;
            localOffset.x = curveX.Evaluate(mTime);
            localOffset.y = curveY.Evaluate(mTime);
        }
        else {
            localOffset += delta * Direction * Speed;
        }
        SetValue();
    }

    public void EditorStop() {
        localOffset = oldOffset;
        SetValue();
    }

    private void Start() {
        mTime = 0;
    }

    private void OnEnable() {
        mTime = 0;
    }

    private void OnDisable() {
        localOffset = oldOffset;
        SetValue();

        cachedMat = null;
        hasMatInitialized = false;
    }

    private void OnDestroy() {
        localOffset = oldOffset;
        SetValue();

        cachedMat = null;
        hasMatInitialized = false;
    }
}
