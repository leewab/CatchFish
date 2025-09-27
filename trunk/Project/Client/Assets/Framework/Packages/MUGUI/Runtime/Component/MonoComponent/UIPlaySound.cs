namespace Game.UI
{
    using MUEngine;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UIPlaySound : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum Trigger
        {
            OnClick,
            OnPress,
            OnRelease,
            OnEnable,
            OnDisable,
            Custom,
        }

        public AudioClip audioClip;
        public Trigger trigger = Trigger.OnClick;
        [HideInInspector] public string audioName = string.Empty;
        public string CustomTriggerCondition = string.Empty;

        void OnEnable()
        {
            if (trigger == Trigger.OnEnable)
            {
                PlaySound();
            }
        }

        void OnDisable()
        {
            if (trigger == Trigger.OnDisable)
            {
                PlaySound();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            if (trigger == Trigger.OnClick)
            {
                PlaySound();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (trigger == Trigger.OnPress)
            {
                PlaySound();
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (trigger == Trigger.OnRelease)
            {
                PlaySound();
            }
        }

        public void OnCustomPlay(string condition)
        {
            if (trigger == Trigger.Custom && CustomTriggerCondition == condition)
            {
                PlaySound();
            }
        }

        static AudioListener mListener = null;

        private void PlaySound()
        {
            if (mListener == null)
            {
                AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];
                if (listeners != null)
                {
                    for (int i = 0; i < listeners.Length; ++i)
                    {
                        if (listeners[i].enabled && listeners[i].gameObject.activeInHierarchy)
                        {
                            mListener = listeners[i];
                            break;
                        }
                    }
                }
            }
#if UNITY_EDITOR
            //资源工程播放
            if (audioClip != null)
            {
                if (mListener == null)
                {
                    Camera cam = Camera.main;
                    if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
                    if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
                }

                if (mListener != null)
                {
                    AudioSource source = mListener.GetComponent<AudioSource>();
                    if (source == null)
                    {
                        source = mListener.gameObject.AddComponent<AudioSource>();
                    }

                    source.PlayOneShot(audioClip);
                }
            }
            else //Game工程播放
            {
                PlayInGame();
            }
#else
        PlayInGame();
#endif
        }

        private void PlayInGame()
        {
            if (audioName != "" && MURoot.MUAudioMgr != null && mListener != null)
            {
                MURoot.MUAudioMgr.AddSound(audioName, mListener.gameObject);
            }
        }
    }

}