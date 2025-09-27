using UnityEngine;
using MUGUI;

public class UIEffectCrossFadeAlpha : UIEffect
{
    public static float duration = 1f;
    public bool play = true;
    CanvasGroup canvasGroup;
    float mDuration = 0f;
    void Awake()
    {
        canvasGroup = this.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        canvasGroup.alpha = 0;
        mDuration = duration;
        CrossFadeAlpha();
        play = true;
    }

    void LateUpdate()
    {
        if (play)
            CrossFadeAlpha();
    }
    void CrossFadeAlpha()
    {
        float delta = RealTime.deltaTime;
        if (mDuration - RealTime.deltaTime < 0)
            delta = mDuration;
        mDuration -= delta;

        canvasGroup.alpha = EaseManager.EasingFromType(0, 1, 1 - mDuration / duration, EaseType.easeOutBack);
        if (mDuration <= 0)
        {
            canvasGroup.alpha = 1;
            play = false;
        }
    }
}
