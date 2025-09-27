using UnityEngine;

namespace Framework.Core
{
    public class SoundController : MonoSingleton<SoundController>
    {
        void Start()
        {
            Debug.Log("Start------SoundController");
        }
        
        public void AddAudioListener(string _alName)
        {
            
        }

        public void RemoveAudioListener(AudioListener _audioListener)
        {
            if (_audioListener != null)
            {
                Destroy(_audioListener);
            }
        }

        public AudioSource AddAudioSource(string _asName, SoundType _soundType = SoundType.DefaultAudio)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            SetAudioSource(audioSource, _soundType);
            return audioSource;
        }

        public void RemoveAudioSource(AudioSource _audioSource)
        {
            if (_audioSource != null)
            {
                Destroy(_audioSource);
            }
        }

        public void SetAudioSource(AudioSource _audioSource, SoundType _soundType = SoundType.DefaultAudio)
        {
            switch (_soundType)
            {
                case SoundType.ClipAudio:
                    
                    break;
                case SoundType.MainAudio:
                    
                    break;
                case SoundType.DefaultAudio:
                    
                    break;
            }
        }

        public void TestSound()
        {
            Debug.Log("Test Sound");
        }
        
    }
}