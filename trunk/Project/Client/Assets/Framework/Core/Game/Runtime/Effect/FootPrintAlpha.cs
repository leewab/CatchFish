using UnityEngine;

public class FootPrintAlpha : MonoBehaviour {

    private float mCurTime;
    public float mTarTime;
    private bool isTweenAlpha = false;

    private Material mat = null;

    private void setAdd(float Add, bool isDisable = false) {
        if (mat != null) {
            mat.SetFloat("_Add", Add);
        }
        if (isDisable) return;
        if (Add > 0.99f) {
            this.OnDisable();
        }
    }

    void Start() {
        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if (mr != null) {
            mat = mr.material;
        }
        setAdd(0);
        isTweenAlpha = true;
        mCurTime = 0f;
        mTarTime = 10f;
    }

    void Update() {
        if (isTweenAlpha) {
            mCurTime += Time.deltaTime;
            if (mCurTime > mTarTime) {
                setAdd(1);
                isTweenAlpha = false;
                return;
            }
            setAdd(mCurTime / mTarTime);
        }
    }

    void OnEnable() {
        setAdd(0);
        isTweenAlpha = true;
        mCurTime = 0f;
        mTarTime = 10f;
    }

    void OnDisable() {
        setAdd(1, true);
        isTweenAlpha = false;
    }
}
