#if VIDEO_PLAYER

#if ANDROID_AR_TEST || UNITY_EDITOR_PROJECT //|| UNITY_ANDROID //临时在Android上打开Log，用于排查异常情况
    #define UI_VIDEO_TEST
#endif

//目前在PC使用插件
//#if !(UNITY_EDITOR || UNITY_STANDALONE_WIN)
//    #define USE_VIDEO_PLAYER
//#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using MUEngine;
using System.IO;

#if USE_VIDEO_PLAYER
using UnityEngine.Video;
#else
using RenderHeads.Media.AVProVideo;
#endif

namespace Game.UI
{
    //在RawImage上播放特定的视频，该类将尝试使用Unity自带的VideoPlayer进行视频播放
    //该类既允许通过在Editor中通过直接挂在并且指定视频配置来播放视频（此时必定是从Bundle中读取），
    //又允许通过代码来进行视频播放 （此时，这个视频可以来源于Bundle，PersistentDataPath，或者StreamingAssets）
    
    //视频播放结果
    public enum PlayVideoResult
    {
        Success = 0,    //成功播放
        GetResFailed = 1,   //没有找到对应的资源
        PlayFailed = 2,     //找到对应视频资源的情况下播放失败
        Abort = 3,     //被其它视频打断
    }
    
    [RequireComponent(typeof(DisplayUGUI))]
    public class UIVideoPlayer : MonoBehaviour
    {
        //视频路径的位置类型 , 记录最后一次调用PlayVideo相关接口时，使用视频的相对位置
        private enum VideoLocation
        {
            Bundle,
            StreamingAssets,
            PersistentDataPath,
            LocalFullPath,
        }

        #region 允许Editor设置的属性
        [SerializeField]
        private string videoSource = "";
        [SerializeField]
        private bool playOnAwake = false;   //这个默认值必须是false，否则代码添加脚本后，会在Awake中执行对应的代码
        [SerializeField]
        private bool loop = false;

        [SerializeField]
        private bool autoSet = false;

        private string readyVideoSource = string.Empty;
        private bool waitMediaPlayerInit = false;
        private bool waitClose = false;

        public bool AutoSet
        {
            get
            {
                return autoSet;
            }
            set
            {
                autoSet = value;
            }
        }

        [SerializeField]
        [Tooltip("显示视频的区域，宽度或者高度相对父节点Size的比例，只有AutoSet被勾选时，该值才会启用")]
        private float scaleFactor = 0.8f;

        public float ScaleFactor
        {
            get
            {
                return scaleFactor;
            }
            set
            {
                scaleFactor = value;
            }
        }

        #endregion

        #region 内部使用属性
#if !UNITY_EDITOR_PROJECT
        [SerializeField]   //在Editor环境中不希望它能够编辑，但是在Project工程，希望能看到这个值
#endif
        private VideoLocation _videoLocation = VideoLocation.Bundle;

        private DisplayUGUI _dUGUI;

        private DisplayUGUI DisplayUGUI
        {
            get
            {
                if (_dUGUI == null)
                {
                    _dUGUI = GetComponent<DisplayUGUI>();
                }
                return _dUGUI;
            }
        }


#if USE_VIDEO_PLAYER
        private VideoPlayer _videoPlayer;
        private VideoPlayer VideoPlayer
        {
            get
            {
                if (_videoPlayer == null)
                {
                    _videoPlayer = gameObject.GetOrAddComponent<VideoPlayer>();
                    _videoPlayer.playOnAwake = false;
                    _videoPlayer.waitForFirstFrame = false;
                    _videoPlayer.renderMode = VideoRenderMode.APIOnly;
                    _videoPlayer.isLooping = loop;
                    _videoPlayer.source = VideoSource.Url;
                    _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

                    //开始监听必要的事件
                    _videoPlayer.prepareCompleted += OnVideoPlayerPrepared;
                    _videoPlayer.loopPointReached += OnVideoFinish;
                }
                return _videoPlayer;
            }
        }
#else
        private MediaPlayer _mediaPlayer;
        private MediaPlayer MediaPlayer
        {
            get
            {
                if (_mediaPlayer == null)
                {
                    if (DisplayUGUI != null)
                    {
                        _mediaPlayer = DisplayUGUI._mediaPlayer;
                        if (_mediaPlayer == null)
                            _mediaPlayer = gameObject.GetOrAddComponent<MediaPlayer>();
                        if (_mediaPlayer != null)
                        {
                            _mediaPlayer.m_AutoOpen = false;
                            _mediaPlayer.m_AutoStart = false;
                            _mediaPlayer.m_Loop = loop;
                            _mediaPlayer.Events.AddListener(OnMediaEventHandler);
                        }
                    }
                }
                return _mediaPlayer;
            }
        }
#endif
        #endregion

        #region 检查视频是否加载完成
        //这里面的代码用于：UI上已经挂载了UIVideoPlayer，代码中去检查这个UIVideoPlayer是否已经开始播放视频了
        //允许多个地方同时等待同一个视频加载完成 , 如果代码中，在前面视频没有加载完成时，又播放了新的视频，
        //那么这里会等待最后一个视频加载完毕后调用
        private bool isVideoLoaded = false;
        public bool IsVideoLoaded { get { return IsVideoLoaded; } }
        private List<Action> loadCompleteCallback = new List<Action>();
        public void WaitForVideoLoadComplete(Action callback)
        {
            if (callback == null)
            {
                return;
            }
            if (isVideoLoaded)
            {
                callback();
                return;
            }
            loadCompleteCallback.Add(callback);
#if USE_VIDEO_PLAYER
            if (VideoPlayer != null)
            {
                CallbackVideoFromBundle_Internal();
            }
#else
            if (_mediaPlayer != null)
            {
                CallbackVideoFromBundle_Internal();
            }
            else
            {
                waitMediaPlayerInit = true;
            }
#endif
        }

        private void BrocastLoadedEvent()
        {
            if (loadCompleteCallback.Count > 0)
            {
                for (int i = 0; i < loadCompleteCallback.Count; i++)
                {
                    var action = loadCompleteCallback[i];
                    if (action != null)
                    {
                        action();
                    }
                }
                loadCompleteCallback.Clear();
            }
        }
        private void NotifyLoaded()
        {
            if (isVideoLoaded)
            {
                return;
            }
            isVideoLoaded = true;
            BrocastLoadedEvent();
        }
        #endregion

        #region 视频播放结果回调相关
        //这里的代码用于，代码中手动调用相关的接口播放视频，可以选择传入一个回调函数来接收视频播放结果的事件
        //同时最多有一个对应的回调被保存，如果后一个回调“顶掉”了之前的回调，那么会先告知前一个回调
        private Action<PlayVideoResult> _playVideoCallback = null;
        private void ResetPlayVideoCallback(Action<PlayVideoResult> callback)
        {
            //通知前一个回调，它被 “顶掉了”
            if (_playVideoCallback != null)
            {
                _playVideoCallback(PlayVideoResult.Abort);
            }
            _playVideoCallback = callback;
        }
        private void NotifyPlayVideoResult(PlayVideoResult result)
        {
            //Loaded相关的需求是：播放失败，或者播放成功（只要不是在准备中），都通知外面
            //如果是因为播放其它视频被顶掉的话，那么就等待最后一个视频播放成功（或者失败）
            if (result != PlayVideoResult.Abort)
            {
                NotifyLoaded();
            }

            if (_playVideoCallback == null)
            {
                return;
            }
            _playVideoCallback(result);
            _playVideoCallback = null;
        }
        #endregion

        #region 视频播放完成回调
        //最后一次手动调用播放视频的接口传入的播放完成回调，目前通过在Editor中直接配置的方式还拿不到播放完成的回调
        //注意，由于实际上是使用 loopEnd 事件，如果视频时循环播放，那么这个回调会被多次调用
        //另外，与播放结果回调不同，这个回调可能永远不会被调用到（比如说，播放视频失败）
        private Action _videoFinishCallback = null;
        private void ResetVideoFinishCallback(Action callback)
        {
            _videoFinishCallback = callback;
        }
        private void NotifyVideoFinish()
        {
            if (_videoFinishCallback != null)
            {
                _videoFinishCallback();
            }
        }
        #endregion

        #region mono callback
        private void Awake()
        {
            if (!playOnAwake || DisplayUGUI == null)
            {
                return;
            }
#if USE_VIDEO_PLAYER
            if (VideoPlayer != null)
            {
                PlayVideoFromBundle_Internal();
            }
#else
            if (MediaPlayer != null)
            {
                PlayVideoFromBundle_Internal();
            }
#endif
        }

        private void OnDestroy()
        {
#if USE_VIDEO_PLAYER
            if(_videoPlayer != null)
            {
                Destroy(_videoPlayer);
            }
#else
            if(_mediaPlayer != null)
            {
                _mediaPlayer.CloseVideo();
                Destroy(_mediaPlayer);
            }
#endif
        }
        #endregion

        #region 播放视频的相关设置与处理

        //代码播放视频时，会设置这个数据 （一方面用于编辑器中能看到对应的值，另一方面，需要让自己 “知道”最后一次播放视频的配置）
        private void SetVideoData(VideoLocation videoLocation,string videoSource,bool loop)
        {
            this._videoLocation = videoLocation;
            this.videoSource = videoSource;

            //loop数据这里存下来，还可以在第一次创建VideoPlayer时，保证loop与之前指定的一致
            //从bundle读取数据时，通过这种方式可以尽量延后VideoPlayer的创建
            this.loop = loop;
            
        }

        //根据之前设置的相关数据，得到当前视频的具体路径
        private string GetCurrentVideoPath()
        {
            if (_videoLocation == VideoLocation.Bundle || _videoLocation == VideoLocation.PersistentDataPath)
            {
                return Path.Combine(Application.persistentDataPath, videoSource);
            }
            else if (_videoLocation == VideoLocation.LocalFullPath)
            {
                return videoSource;
            }
            else
            {
                return null;
            }
        }

        //根据当前设置的相关数据，得到当前视频的URL
        private string GetCurrentVideoURL()
        {
            if (_videoLocation == VideoLocation.Bundle || _videoLocation == VideoLocation.PersistentDataPath)
            {
                return VideoVersionHelper.GetVideoURLFromPersistentDataPath(videoSource);
            }
            else if (_videoLocation == VideoLocation.LocalFullPath)
            {
                return VideoVersionHelper.GetVideoURLFromLocalFullPath(videoSource);
            }
            else if (_videoLocation == VideoLocation.StreamingAssets)
            {
                return VideoVersionHelper.GetVideoURLFromStreamingAssets(videoSource);
            }
            return null;
        }

#if USE_VIDEO_PLAYER
        private void StartPlayVideoUseVideoPlayer(string url,bool loop)
        {
            var videoInfo = MediaHelper.GetVideoInfo(GetCurrentVideoPath());
#if UI_VIDEO_TEST
            Debug.LogErrorFormat("videoInfo , height : {0} , width : {1} , rotation : {2}", videoInfo.height, videoInfo.width, videoInfo.rotation);
#endif
            VideoPlayer.source = VideoSource.Url;
            VideoPlayer.url = url;
            VideoPlayer.isLooping = loop;

            VideoPlayer.Prepare();
        }
#else
        private MediaPlayer.FileLocation GetFileLocation()
        {
            if (_videoLocation == VideoLocation.Bundle || _videoLocation == VideoLocation.PersistentDataPath)
            {
                return MediaPlayer.FileLocation.RelativeToPeristentDataFolder;
            }
            else if (_videoLocation == VideoLocation.LocalFullPath)
            {
                return MediaPlayer.FileLocation.AbsolutePathOrURL;
            }
            else if (_videoLocation == VideoLocation.StreamingAssets)
            {
                return MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder;
            }
            return MediaPlayer.FileLocation.AbsolutePathOrURL;
        }
        private void StartPlayVideoUseMediaPlayer(MediaPlayer.FileLocation localtion, string relativePath, bool loop)
        {
            try
            {
                if (_mediaPlayer != null)
                {
                    _mediaPlayer.m_Loop = loop;
                    _mediaPlayer.OpenVideoFromFile(localtion, relativePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
#endif
        private void StartPlayVideo()
        {
            isVideoLoaded = false;

            try
            {

#if USE_VIDEO_PLAYER
                StartPlayVideoUseVideoPlayer(GetCurrentVideoURL(), loop);
#else
                StartPlayVideoUseMediaPlayer(GetFileLocation(), videoSource, loop);
#endif
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                NotifyPlayVideoResult(PlayVideoResult.PlayFailed);
            }
        }

#if USE_VIDEO_PLAYER
        private void OnVideoPlayerPrepared(VideoPlayer videoPlayer)
        {
            if(gameObject == null || DisplayUGUI == null)
            {
                //如果回调完成的时候，GameObject已经被外部销毁，需要直接返回
                return;
            }
            //如果本身有ImageLoader，则干掉它，避免异步加载对应的图片后覆盖掉设置的值
            GOGUI.RawImageLoader imageLoader = GetComponent<GOGUI.RawImageLoader>();
            if (imageLoader != null)
            {
#if UI_VIDEO_TEST
                Debug.LogError("video player remove raw image loader");
#endif
                DestroyImmediate(imageLoader);
            }

            try
            {
                videoPlayer.Play();
            }
			catch(Exception e)
            {
                Debug.LogError(e);
                NotifyPlayVideoResult(PlayVideoResult.PlayFailed);
                return;
            }
#if UI_VIDEO_TEST
            Debug.LogError("video player set raw image texture success !");
#endif
            NotifyPlayVideoResult(PlayVideoResult.Success);
        }

        private void OnVideoFinish(VideoPlayer videoPlayer)
        {
            NotifyVideoFinish();
        }
#else
        private void OnMediaEventHandler(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
        {
            if (mp != _mediaPlayer)
            {
                return;
            }
#if UI_VIDEO_TEST
#if (UNITY_ANDROID)
                Debug.LogError("Debug log, MediaPlayer handler: " + et);
#endif
#endif
            switch (et)
            {
                    // 需要处理那个，就从这一堆中摘出来
                case MediaPlayerEvent.EventType.MetaDataReady:      // Triggered when meta data(width, duration etc) is available
                case MediaPlayerEvent.EventType.ReadyToPlay:        // Triggered when the video is loaded and ready to play
                case MediaPlayerEvent.EventType.Started:            // Triggered when the playback starts
                case MediaPlayerEvent.EventType.Closing:            // Triggered when the media is closed
                case MediaPlayerEvent.EventType.SubtitleChange:     // Triggered when the subtitles change
                case MediaPlayerEvent.EventType.ResolutionChanged:  // Triggered when the resolution of the video has changed (including the load) Useful for adaptive streams
                case MediaPlayerEvent.EventType.StartedSeeking:     // Triggered whhen seeking begins
                case MediaPlayerEvent.EventType.FinishedSeeking:    // Triggered when seeking has finished
                case MediaPlayerEvent.EventType.StartedBuffering:   // Triggered when buffering begins
                case MediaPlayerEvent.EventType.FinishedBuffering:  // Triggered when buffering has finished
                case MediaPlayerEvent.EventType.PropertiesChanged:	// Triggered when any properties (eg stereo packing are changed) - this has to be triggered manually
			    case MediaPlayerEvent.EventType.PlaylistItemChanged:// Triggered when the new item is played in the playlist
                case MediaPlayerEvent.EventType.PlaylistFinished:	// Triggered when the playlist reaches the end
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:    // Triggered when the first frame has been rendered
//#if UNITY_EDITOR || UNITY_STANDALONE_WIN
//                    gameObject.transform.eulerAngles = (_mediaPlayer != null && _mediaPlayer.TextureProducer.RequiresVerticalFlip()) ? Vector3.zero : new Vector3(180, 0, 0);
//#endif
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:    // Triggered when a non-looping video has finished playing
                    NotifyVideoFinish();
                    break;
                case MediaPlayerEvent.EventType.Stalled:            // Triggered when media is stalled (eg. when lost connection to media stream) - Currently only suported on Windows platforms
                case MediaPlayerEvent.EventType.Unstalled:          // Triggered when media is resumed form a stalled state (eg. when lost connection is re-established)
                                                                    // 根据avpro帮助，这个只支持windows platforms，所以在手机平台上，要做处理，否则显示就异常了
#if UI_VIDEO_TEST
#if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                    NotifyVideoFinish();
                    ClearVideo();
#endif
#endif
                    break;
                case MediaPlayerEvent.EventType.Error:
                    Debug.LogError("MediaPlayer Play ErrorCode:" + errorCode);
                    break;
            }
        }
#endif
#endregion

                    #region 外部接口
                    //如果是 自己再Awake的时候播放视频的话，直接调用这个方法就好了
                    private void PlayVideoFromBundle_Internal()
        {
            //拿到url,然后设置videoPlayer
            string currentVideoSource = videoSource;
            VideoVersionHelper.GetVideoURLFromBundle(videoSource, url =>
            {
                if (videoSource != currentVideoSource)
                {
                    //如果回调回来的时候，当前的videoSource 已经与开始加载的时候不一样，说明已经播放其它视频了，所以这届返回，不做任何处理
                    return;
                }
                if (string.IsNullOrEmpty(url))
                {
                    Debug.LogError("无法找到合适的资源url : " + videoSource);
                    NotifyPlayVideoResult(PlayVideoResult.GetResFailed);
                    return;
                }
#if UI_VIDEO_TEST
                Debug.LogError("the ui video url is : " + url);
#endif
                StartPlayVideo();
            });
        }

        private void CallbackVideoFromBundle_Internal()
        {
            //拿到url,然后设置videoPlayer
            string currentVideoSource = videoSource;
            VideoVersionHelper.GetVideoURLFromBundle(videoSource, url =>
            {
                if (videoSource != currentVideoSource)
                {
                    //如果回调回来的时候，当前的videoSource 已经与开始加载的时候不一样，说明已经播放其它视频了，所以这届返回，不做任何处理
                    return;
                }
                if (string.IsNullOrEmpty(url))
                {
                    Debug.LogError("无法找到合适的资源url : " + videoSource);
                    NotifyPlayVideoResult(PlayVideoResult.GetResFailed);
                    return;
                }
#if UI_VIDEO_TEST
                Debug.LogError("the ui video url is : " + url);
#endif
                NotifyPlayVideoResult(PlayVideoResult.Success);
                readyVideoSource = videoSource;
            });
        }

        public void PlayVideoFromBundle(string videoName, bool isLoop, Action<PlayVideoResult> playResultCallback = null, Action videoFinishCallback = null)
        {
            SetVideoData(VideoLocation.Bundle, videoName, isLoop);
            ResetPlayVideoCallback(playResultCallback);
            ResetVideoFinishCallback(videoFinishCallback);

            PlayVideoFromBundle_Internal();
        }
        public void PlayVideoFromStreamingAssets(string videoName, bool isLoop, Action<PlayVideoResult> playResultCallback = null, Action videoFinishCallback = null)
        {
            SetVideoData(VideoLocation.StreamingAssets, videoName, isLoop);
            ResetPlayVideoCallback(playResultCallback);
            ResetVideoFinishCallback(videoFinishCallback);

            StartPlayVideo();
        }
        public void PlayVideoFromPersistentDataPath(string videoRelativePath, bool isLoop, Action<PlayVideoResult> playResultCallback = null, Action videoFinishCallback = null)
        {
            SetVideoData(VideoLocation.PersistentDataPath, videoRelativePath, isLoop);
            ResetPlayVideoCallback(playResultCallback);
            ResetVideoFinishCallback(videoFinishCallback);

            StartPlayVideo();
        }
        public void PlayVideoFromLoaclFullPath(string videoPath, bool isLoop, Action<PlayVideoResult> playResultCallback = null, Action videoFinishCallback = null)
        {
            SetVideoData(VideoLocation.LocalFullPath, videoPath, isLoop);
            ResetPlayVideoCallback(playResultCallback);
            ResetVideoFinishCallback(videoFinishCallback);

            StartPlayVideo();
        }

        //暂停与重放
        public void PauseVideo()
        {
#if USE_VIDEO_PLAYER
            if(_videoPlayer != null)
            {
                _videoPlayer.Pause();
            }
#else
            if(_mediaPlayer != null)
            {
                _mediaPlayer.Pause();
            }
#endif
        }
        public void RewindVideo()
        {
#if USE_VIDEO_PLAYER
            if(_videoPlayer != null)
            {
                _videoPlayer.Play();
            }
#else
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Rewind(false);
            }
#endif
        }
        public void StartVideo()
        {
#if USE_VIDEO_PLAYER
            if(_videoPlayer != null)
            {
                _videoPlayer.Play();
            }
#else
            if(_mediaPlayer != null)
            {
                _mediaPlayer.Play();
            }
#endif
        }
        public void StopVideo()
        {
#if USE_VIDEO_PLAYER
            if(_videoPlayer != null)
            {
                _videoPlayer.Stop();
            }
#else
            if(_mediaPlayer != null)
            {
                _mediaPlayer.Stop();
            }
#endif
        }

        public void PlayVideo()
        {
#if USE_VIDEO_PLAYER
            if (VideoPlayer != null)
            {
                PlayVideoFromBundle_Internal();
            }
#else
            if (_mediaPlayer != null)
            {
                PlayVideoFromBundle_Internal();
            }
#endif
        }

        public void ClearVideo()
        {
#if USE_VIDEO_PLAYER
            //直接销毁，简单粗暴
            if (GetComponent<VideoPlayer>() != null)
                DestroyImmediate(GetComponent<VideoPlayer>());
#else
            waitClose = true;
#endif
        }

        #endregion

        private void Update()
        {
            if (MediaPlayer != null && waitMediaPlayerInit)
            {
                CallbackVideoFromBundle_Internal();
                waitMediaPlayerInit = false;
            }
            if (isVideoLoaded && !string.IsNullOrEmpty(readyVideoSource))
            {
                StartPlayVideo();
                readyVideoSource = string.Empty;
            }
            if (waitClose && MediaPlayer)
            {
                MediaPlayer.CloseVideo();
                waitClose = false;
            }
        }
    }

    //Video版本管理
    public static class VideoVersionHelper
    {
        //TODO : 既然已经升级到了2018，那么可能可以直接从Bundle中读取视频片段然后播放了
        //有时间的话，可以优化此处 （目前只有一个视频是从Bundle中读取并且播放的，所以暂时不着急改）



        //根据资源名，获取一个对应的URL，这个URL将指向对应的视频文件位置
        //如果直接将视频资源打包成AssetBundle，那么读取出来的VideoClip将无法用于VideoPlayer播放（这是Unity自身的Bug，直到unity 2018这个问题才被解决）
        //如果将视频资源直接放在StreamingAssets 下（或者Resources？ 感觉应该不行），通过URL来指定资源路径，那么是可以播放资源的，但是这就不能走热更新流程了
        //这里尝试采取这么一种方式：将视频资源直接作为二进制数据整体打包到AssetBundle中，在需要的时候，从AssetBundle中拿到这些二进制数据，然后写到对应的文件中
        //之后，将文件的URL传给VideoPlayer。这种方式比较浪费存储空间，在视频资源量较少的情况下应该问题不大。
        //如果之后需要优化这里，可以考虑视频资源直接从网络上下载，然后存到本地。
        //不管用哪种方式，它们都会使用相同的接口 ： 传入资源名，获取对应的URL
        public static void GetVideoURLFromBundle(string videoSource, Action<string> onResult)
        {
            if (onResult == null)
            {
                return;
            }
            CheckUpdateVideo(videoSource, result =>
            {
                if (result)
                {
                    onResult(GetVideoURLFromPersistentDataPath(videoSource));
                }
                else
                {
                    onResult(null);
                }
            });
        }

        //获取相对PersistentDataPath的路径
        public static void GetVideoRelativePath(string videoSource, Action<string> onResult)
        {
            if (onResult == null)
            {
                return;
            }
            CheckUpdateVideo(videoSource, result =>
            {
                if (result)
                {
                    onResult(videoSource);
                }
                else
                {
                    onResult(null);
                }
            });
        }

        //检查资源是否已经是最新，如果不是最新，则尝试更新对应的资源,回调通过一个bool值指定是否成功将最新资源更新到目标位置
        private static void CheckUpdateVideo(string videoSource, Action<bool> onResult)
        {
            if (onResult == null)
            {
                return;
            }
            var currentVersion = PlayerPrefs.GetInt(videoSource, 0);
#if UI_VIDEO_TEST
            Debug.LogError("current video source is : " + videoSource + " , local version is : " + currentVersion);
#endif
            GetCurrentVideoVersion(videoSource, version =>
            {
#if UI_VIDEO_TEST
                Debug.LogError("current video source is : " + videoSource + " , current version is : " + version);
#endif
                //如果本地有资源，并且版本号相等（或者无法拿到版本信息，版本号返回了默认的0），那么直接用本地的资源
                var path = GetVideoPath(videoSource);
                if (version == currentVersion && File.Exists(path))
                {
                    onResult(true);
                    return;
                }
                //尝试从Bundle中拿到对应的二进制数据，然后写入到对应的路径
                GetVideoDataFromAssetBundle(videoSource, data =>
                {
                    if (data != null && data.Length > 0)
                    {
                        try
                        {
                            File.Create(path).Dispose();
                            //目前直接使用同步文件读写，之后此处可以换成异步文件读写
                            File.WriteAllBytes(path, data);
                            PlayerPrefs.SetInt(videoSource, version);
#if UI_VIDEO_TEST
                            Debug.LogError("write to file success ! the videosource is : " + videoSource + " , the path is : " + path);
#endif
                            onResult(true);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                            onResult(false);
                        }
                    }
                    else
                    {
                        onResult(false);
                    }
                });
            });
        }

        //检查资源是否已经复制到了本地，目前通过文件名和一个videomap来进行判断
        static Dictionary<string, int> videoVersionMap;
        private const string VIDEO_MAP_NAME = "videomap.bytes";
        static void CheckInitialVideoMap(Action onResult)
        {
            if (onResult == null)
            {
                return;
            }
            if (videoVersionMap != null)
            {
                onResult();
                return;
            }
            MURoot.ResMgr.GetAsset(VIDEO_MAP_NAME, (name, obj) =>
            {
#if UI_VIDEO_TEST
                Debug.LogError("get video map success !");
#endif
                TextAsset texts = obj as TextAsset;
                if (texts != null && !string.IsNullOrEmpty(texts.text))
                {
#if UI_VIDEO_TEST
                    Debug.LogError("the video map content is : " + texts.text);
#endif
                    string[] versionInfos = texts.text.Split('\n');
                    videoVersionMap = new Dictionary<string, int>();
                    for (int i = 0; i < versionInfos.Length; i++)
                    {
                        string versionInfo = versionInfos[i].TrimEnd('\r');
                        string[] versionPair = versionInfo.Split(' ');
                        if (versionPair.Length != 2)
                        {
                            continue;
                        }
                        int versionNum = 0;
                        int.TryParse(versionPair[1], out versionNum);
                        videoVersionMap[versionPair[0]] = versionNum;
                    }
                }
#if !UNITY_EDITOR_PROJECT
                //在Editor工程中，这个Asset是本地资源，不应该删除
                MURoot.ResMgr.ReleaseAsset(name, obj);
#endif
                onResult();
            });
        }
        static void GetCurrentVideoVersion(string videoSource, Action<int> onResult)
        {
            if (onResult == null)
            {
                return;
            }
            var path = GetVideoPath(videoSource);
            CheckInitialVideoMap(() => {
                if (videoVersionMap == null || !videoVersionMap.ContainsKey(videoSource))
                {
                    onResult(0);
                    return;
                }
                onResult(videoVersionMap[videoSource]);
            });
        }

        //从AssetBundle中获取对应的二进制数据
        static void GetVideoDataFromAssetBundle(string videoSource, Action<byte[]> onResult)
        {
            //作为二进制存储，需要使用.bytes的后缀名
            string assetName = videoSource + ".bytes";
            MURoot.ResMgr.GetAsset(assetName, (name, obj) =>
            {
                var textAsset = obj as TextAsset;
                byte[] data = textAsset == null ? null : textAsset.bytes;
                if (onResult != null)
                {
                    onResult(data);
                    //立刻释放资源,目前应该不会有问题
                }
#if !UNITY_EDITOR_PROJECT
                //在Editor工程中，这个Asset是本地资源，不应该删除
                MURoot.ResMgr.ReleaseAsset(name, obj);
#endif
            }, LoadPriority.HighPrior);
        }
        //从网络上获取对应的二进制数据，暂未实现
        static void GetVideoDataFromNetwork(string videoSource, Action<byte[]> onResult)
        {
            onResult(null);
        }

        //生成StreamingAssets下的资源URL
        public static string GetVideoURLFromStreamingAssets(string videoSource)
        {
            string url = "file://" + Application.streamingAssetsPath + "/" + videoSource;
#if !UNITY_EDITOR && UNITY_ANDROID
            url = Application.streamingAssetsPath + "/" + videoSource;
#endif
            return url;
        }

        //生成本地任意路径下的资源URL
        public static string GetVideoURLFromLocalFullPath(string fullPath)
        {
            //也许是这样？
            return "file://" + fullPath;
        }

        //生成PersistentDataPath下的资源URL
        public static string GetVideoURLFromPersistentDataPath(string videoSource)
        {
            string url = "file://" + Application.persistentDataPath + "/" + videoSource;
            return url;
        }

        //生成PersistentDataPath下的资源文件路径
        static string GetVideoPath(string videoSource)
        {
            return Application.persistentDataPath + "/" + videoSource;
        }
    }
}

#endif