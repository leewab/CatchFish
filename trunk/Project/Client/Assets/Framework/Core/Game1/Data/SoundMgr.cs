using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core
{
    public class SoundMgr : Singleton<SoundMgr>
    {
        private Dictionary<string, AudioSource> mAudioSourceDic = new Dictionary<string, AudioSource>();
        
        private SoundController mSoundController;

        public SoundController SoundController
        {
            get
            {
                if (mSoundController == null)
                {
                    // mSoundController = GameController.Instance.AddComCtrl<SoundController>();
                }

                return mSoundController;
            }
        }
        
        private AudioSource mMainAudioSource;

        public AudioSource MainAudioSource
        {
            get
            {
                if (mMainAudioSource == null)
                {
                    mMainAudioSource = GetAudioSource("mainAudioSource");
                }

                return mMainAudioSource;
            }
        }
        
        private AudioSource mClipAudioSource;

        public AudioSource ClipAudioSource
        {
            get
            {
                if (mClipAudioSource == null)
                {
                    mClipAudioSource = GetAudioSource("clipAudioSource");
                }

                return mClipAudioSource;
            }
        }

        public override void OnSingleInit()
        {
            base.OnSingleInit();
            if (mSoundController == null)
            {
                // mSoundController = GameController.Instance.AddComCtrl<SoundController>();
            }
        }

        public override void OnSingleDispose()
        {
            base.OnSingleDispose();
            if (mSoundController != null)
            {
                mSoundController = null;
            }
        }

        private AudioSource GetAudioSource(string _asName)
        {
            if (mAudioSourceDic.ContainsKey(_asName))
            {
                return mAudioSourceDic[_asName];
            }
            else
            {
                return SoundController.AddAudioSource(_asName);
            }
        }

        #region Common Play Sound

        public void PlayMainSound(string _audioName, bool _isLoop = true)
        {
            // MainAudioSource.clip = GetSound(_audioName);
            MainAudioSource.loop = _isLoop;
        }

        public void PlayClipSound(string _audioName, AudioSource _audioSource)
        {
            // if (_audioSource != null)
            // {
            //     _audioSource.PlayOneShot(GetSound(_audioName));
            // }
        }

        // public AudioClip GetSound(string _audioName)
        // {
        //     return ResLoader.Instance.LoadAudioClip(_audioName, _audioName);;
        // }

        #endregion
        
        public string GetTest()
        {
            return "SoundManager";
        }
        
    }

    public enum SoundType
    {
        MainAudio,
        ClipAudio,
        DefaultAudio
    }
}