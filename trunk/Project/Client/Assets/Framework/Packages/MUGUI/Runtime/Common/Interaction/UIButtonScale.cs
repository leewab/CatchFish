using UnityEngine;
using UnityEngine.EventSystems;

namespace MUGUI
{
    /// <summary>
    /// 按钮事件大小缩放
    /// </summary>

    [AddComponentMenu("MUGUI/Interaction/Button Scale")]
    public class UIButtonScale : MonoBehaviour, SUGUIEventSystemsInterface
    {
        public Transform tweenTarget;
        UnityEngine.UI.Button button;
        public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);
        public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);
        public Vector3 normal = new Vector3(1f, 1f, 1f);
        public float duration = 0.2f;

        Vector3 mScale;

        bool mStarted = false;

        void Start()
        {
            if (!mStarted)
            {
                mStarted = true;
                if (tweenTarget == null)
                {
                    tweenTarget = transform;
                    button = gameObject.GetComponent<UnityEngine.UI.Button>();
                }
                mScale = tweenTarget.localScale;
            }
        }

        void OnDisable()
        {
            if (mStarted && tweenTarget != null)
            {
                TweenScale tc = tweenTarget.GetComponent<TweenScale>();

                if (tc != null)
                {
                    tc.value = mScale;
                    tc.enabled = false;
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Scale(hover);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Scale(mScale);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Scale(pressed);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Scale(mScale);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
        }

        void Scale(Vector3 to)
        {
            if (button && !button.interactable)
            {
                to = normal;
            }
            TweenScale.Begin(tweenTarget.gameObject, duration, to).easeType = EaseType.linear;
        }
    }
}