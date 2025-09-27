using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public class TwoFingerPinchListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private int currentFirstFinger = -1;
        private int currentSecondFinger = -1;
        private int kountFingersDown = 0;
        private bool pinching = false;

        private Vector2 positionFirst = Vector2.zero;
        private Vector2 positionSecond = Vector2.zero;
        private float previousDistance = 0f;
        private float delta = 0f;
        float distanceStart;
        float zoomRate;

        GameObject go;
        public event EventTriggerListener.VectorDelegate onPinchStart;
        public event EventTriggerListener.VectorFloatDelegate onPinchZoom;
        public event EventTriggerListener.VoidDelegate onPinchEnd;

        static public TwoFingerPinchListener Get(GameObject go)
        {
            TwoFingerPinchListener listener = go.GetComponent<TwoFingerPinchListener>();
            if (listener == null) listener = go.AddComponent<TwoFingerPinchListener>();
            listener.go = go;
            return listener;
        }

        void Awake()
        {

        }

        public void OnPointerDown(PointerEventData data)
        {
            kountFingersDown = kountFingersDown + 1;

            if (currentFirstFinger == -1 && kountFingersDown == 1)
            {
                // first finger must be a pure first finger and that's that

                currentFirstFinger = data.pointerId;
                positionFirst = data.position;

                return;
            }

            if (currentFirstFinger != -1 && currentSecondFinger == -1 && kountFingersDown == 2)
            {
                // second finger must be a pure second finger and that's that

                currentSecondFinger = data.pointerId;
                positionSecond = data.position;

                distanceStart = Vector2.Distance(positionFirst, positionSecond);

                pinching = true;
                if (onPinchStart != null)
                {
                    onPinchStart(gameObject, (positionFirst+positionSecond)/2);
                }
                return;
            }

        }

        public void OnPointerUp(PointerEventData data)
        {
            kountFingersDown = kountFingersDown - 1;

            if (currentFirstFinger == data.pointerId)
            {
                currentFirstFinger = -1;

                if (pinching)
                {
                    pinching = false;
                    if (onPinchEnd != null)
                    {
                        onPinchEnd(gameObject);
                    }
                }
            }

            if (currentSecondFinger == data.pointerId)
            {
                currentSecondFinger = -1;

                if (pinching)
                {
                    pinching = false;
                    if (onPinchEnd != null)
                    {
                        onPinchEnd(gameObject);
                    }
                }
            }

        }

        public void OnDrag(PointerEventData data)
        {
            Debug.Log("pointer " + data.pointerId);
            if (currentFirstFinger == data.pointerId)
            {
                positionFirst = data.position;
            }

            if (currentSecondFinger == data.pointerId)
            {
                positionSecond = data.position;
            }

            if (pinching)
            {
                if (data.pointerId == currentFirstFinger || data.pointerId == currentSecondFinger)
                {
                    if (kountFingersDown == 2)
                    {
                        if (onPinchStart != null)
                        {
                            float dist = Vector2.Distance(positionFirst, positionSecond);
                            onPinchZoom(gameObject, (positionFirst + positionSecond) / 2,dist/distanceStart);
                        }
                    }
                    return;
                }
            }
        }

    }
}