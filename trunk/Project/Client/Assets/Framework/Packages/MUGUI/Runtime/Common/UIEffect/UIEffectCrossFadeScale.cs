using UnityEngine;
using MUGUI;

public class UIEffectCrossFadeScale : UIEffect
{
    public float duration = 0.3f;
    float mDuration = 0f;
    Canvas canvas;
    float scaleFactor = 0;
    Vector3 scaleFactorV3 = Vector3.zero;
    bool play = false;
    public EaseType type = EaseType.easeOutBack;
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        if (canvas)
            scaleFactor = canvas.scaleFactor;
        else
            scaleFactorV3 = this.gameObject.transform.localScale;
        Play();
    }

    void LateUpdate()
    {
        if (!play)
            return;

        float delta = RealTime.deltaTime;
        if (mDuration - RealTime.deltaTime < 0)
            delta = mDuration;
        mDuration -= delta;

        if (canvas)
        {
            if (canvasDealy)   //跟其他脚本有冲突，需要延迟一帧执行
            {
                canvasDealy = false;
                canvas.scaleFactor = 0;
            }

            //             canvas.scaleFactor += delta * scaleFactor / duration;
            canvas.scaleFactor = EaseManager.EasingFromType(0, scaleFactor, 1 - mDuration / duration, type);
            if (mDuration <= 0)
            {
                canvas.scaleFactor = scaleFactor;
                play = false;
            }
        }
        else
        {
            Vector3 v3 = this.gameObject.transform.localScale;
            float val = EaseManager.EasingFromType(0, scaleFactorV3.x, 1 - mDuration / duration, type);
            v3 = Vector3.zero * (1f - val) + scaleFactorV3 * val;
            if (mDuration <= 0)
            {
                v3 = scaleFactorV3;
                play = false;
            }
            this.gameObject.transform.localScale = v3;
        }
    }

    void OnEnable()
    {
        Play();
    }

    void OnDisable()
    {
        play = false ;
    }

    bool canvasDealy = false;
    public void Play()
    {
        if (canvas)
        {
            canvasDealy = true;
        }
        else
        {
            this.gameObject.transform.localScale = Vector3.zero;
        }
        mDuration = duration;
        play = true;
    }
}
