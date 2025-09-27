// using System.Collections.Generic;
// using UnityEngine;
//
// namespace Game
// {
//     public class SoundManager : Singleton<SoundManager>
//     {
//         /// <summary>
//         /// 资源加载管理器
//         /// </summary>
//         private BaseResCtrl mAssetBundleCtrl = ResLoader.Allocate();
//         /// <summary>
//         /// AudioSource缓存池
//         /// </summary>
//         private Dictionary<string, AudioSource> mAudioSourceDic = new Dictionary<string, AudioSource>();
//         
//         private SoundController _mSoundController;
//         public SoundController SoundController
//         {
//             get
//             {
//                 if (_mSoundController == null)
//                 {
//                     _mSoundController = GameController.Instance.AddController<SoundController>();
//                 }
//
//                 return _mSoundController;
//             }
//         }
//         
//         private AudioSource mMainAudioSource;
//         public AudioSource MainAudioSource
//         {
//             get
//             {
//                 if (mMainAudioSource == null)
//                 {
//                     mMainAudioSource = GetAudioSource("mainAudioSource");
//                 }
//
//                 return mMainAudioSource;
//             }
//         }
//         
//         private AudioSource mClipAudioSource;
//         public AudioSource ClipAudioSource
//         {
//             get
//             {
//                 if (mClipAudioSource == null)
//                 {
//                     mClipAudioSource = GetAudioSource("clipAudioSource");
//                 }
//
//                 return mClipAudioSource;
//             }
//         }
//
//         public override void OnSingleInit()
//         {
//             base.OnSingleInit();
//             if (_mSoundController == null)
//             {
//                 _mSoundController = GameController.Instance.AddController<SoundController>();
//             }
//         }
//
//         public override void OnSingleDispose()
//         {
//             base.OnSingleDispose();
//             if (_mSoundController != null)
//             {
//                 _mSoundController = null;
//                 mAssetBundleCtrl.Dispose();
//                 mAssetBundleCtrl = null;
//             }
//         }
//
//         private AudioSource GetAudioSource(string _asName)
//         {
//             if (mAudioSourceDic.ContainsKey(_asName))
//             {
//                 return mAudioSourceDic[_asName];
//             }
//             else
//             {
//                 return SoundController.AddAudioSource(_asName);
//             }
//         }
//
//         #region Common Play Sound
//
//         public void PlayMainSound(string _audioName, bool _isLoop = true)
//         {
//             MainAudioSource.clip = GetSound(_audioName);
//             MainAudioSource.loop = _isLoop;
//         }
//
//         public void PlayClipSound(string _audioName, AudioSource _audioSource)
//         {
//             if (_audioSource != null)
//             {
//                 _audioSource.PlayOneShot(GetSound(_audioName));
//             }
//         }
//
//         public AudioClip GetSound(string _audioName)
//         {
//             return mAssetBundleCtrl.Load<AudioClip>(_audioName, _audioName);;
//         }
//
//         #endregion
//         
//     }
//
//     public enum SoundType
//     {
//         MainAudio,
//         ClipAudio,
//         DefaultAudio
//     }
// }