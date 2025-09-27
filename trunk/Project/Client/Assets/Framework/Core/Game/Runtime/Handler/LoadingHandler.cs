using System;
using Game;
using Game.Core;
using Game.UI;
using MUEngine;
using UnityEngine;

namespace Game
{
    public enum LoadingType
    {
        LoginLoading, // 登录loading
        SceneLoading, // 场景loading
    }

    public class LoadingHandler : BaseHandler
    {
        /// <summary>
        /// 单例注册
        /// </summary>
        public static LoadingHandler Instance => HandlerModule.LoadingHandler;

        private Action mOnEndCallBack;
        private UILoadingView mCurUILoadingView;

        public void Start(LoadingType loadingType, Action callBack)
        {
            this.mOnEndCallBack = callBack;
            Debug.Log("LoadingType " + loadingType.ToString());
            if (loadingType == LoadingType.LoginLoading)
            {
                if (mCurUILoadingView != null)
                {
                    LogHelper.Error("当前正在Loading，请等待完毕！");
                    return;
                }
                this.mCurUILoadingView = UIManager.Instance.OpenUI<UILoadingView>(UILayerEnums.UILoadingLayer);
                //这里本来是等资源加载完毕自动关闭，目前小demo阶段 没有走 直接关
                End();
                // 监测资源加载
                // MURoot.ResMgr.InitProgress(progressHandler, assetHandler, OnLoadedProgress);
            }
        }
        
        public void End()
        {
            this.mOnEndCallBack?.Invoke();
            if (this.mCurUILoadingView != null) this.mCurUILoadingView.Close();
        }

        public int GetProgressValue()
        {
            return this.mProgressValue;
        }

        private EProgressType mCurProgressType;
        private int mProgressValue = 0;
        private float loadPercent;//mCurLoadPercent;
        private int totalSize = 0;//所有type需要加载的大小
        private float loadTime = 0f;
        private int lastLoadSize = 0;
        private string networkSpeed = string.Empty;
        private bool hasPatch = false;

        private void OnLoadedProgress()
        {
            LogHelper.Log("Loading 加载完毕！");
        }
        
        public void progressHandler(int type, float value)
        {
            if (type < 0) return;
            if (this.mCurUILoadingView == null || !this.mCurUILoadingView.ResLoaded) return;
            this.loadPercent = value;
            this.mCurProgressType = (EProgressType)type;
            this.mProgressValue = (int)(value * 100);
            float nowSize = totalSize * value / 1024f;
            if (value < 1f)
            {
                if (this.mCurProgressType == EProgressType.UpdateLanguageDownload 
                    || this.mCurProgressType == EProgressType.UpdateDeCompressUpdate 
                    || this.mCurProgressType == EProgressType.UpdateVersion 
                    || this.mCurProgressType == EProgressType.UpdatePatch)
                {
                    //计算下载速度
                    float diffTime = Time.realtimeSinceStartup - loadTime;
                    if (diffTime > 0.99f)
                    {
                        float diffValue = (totalSize * value - lastLoadSize);
                        if (diffValue > 0f)
                        {
                            float speed = diffValue / diffTime;
                            if (speed > 1024f)
                            {
                                networkSpeed = string.Format(CSDisplayText.GetMessage("NetworkSpeed"), (speed / 1024f).ToString("#0.00"), "MB/S");
                            }
                            else
                            {
                                networkSpeed = string.Format(CSDisplayText.GetMessage("NetworkSpeed"), speed.ToString("#0.00"), "KB/S");
                            }
                            lastLoadSize = (int)(totalSize * value);
                        }
                        else
                        {
                            if (value > 0f)
                            {
                                lastLoadSize = (int)(totalSize * value);
                            }
                            networkSpeed = string.Format(CSDisplayText.GetMessage("NetworkSpeed"), "0", "KB/S");
                        }
                        loadTime = Time.realtimeSinceStartup;
                    }
                }
            }

            string progressText = string.Format("{0}M / {1}M", nowSize.ToString("#0.00"), (totalSize / 1024f).ToString("#0.00"));
            switch (this.mCurProgressType)
            {
                case EProgressType.UpdateVersion:
                    string stepText = hasPatch ? "(2/2)" : "";
                    this.mCurUILoadingView.Refresh(string.Format("{0}{1}: {2}  {3}", CSDisplayText.GetMessage("UpdatingAssets"), stepText, progressText, networkSpeed), "", value);
                    break;
                case EProgressType.UpdatePatch:
                    this.mCurUILoadingView.Refresh(string.Format("{0}(1/2): {1}  {2}", CSDisplayText.GetMessage("UpdatingAssets"), progressText, networkSpeed), "", value);
                    break;
                case EProgressType.UpdateDeCompressUpdate:
                    this.mCurUILoadingView.Refresh(string.Format("{0}(1/2)：{1}  {2}", CSDisplayText.GetMessage("UpdatingAssets"), progressText, networkSpeed), "", value);
                    break;
                case EProgressType.UpdateDeCompress:
                    this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("DeCompressAssets"), value);
                    break;
                case EProgressType.UpdateBundleMap:
                    this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("BundleMapAssets"), value);
                    break;
                case EProgressType.UpdateLanguageDownload:
                    this.mCurUILoadingView.Refresh(string.Format("{0}：{1}  {2}", CSDisplayText.GetMessage("UpdatingLanguage"), progressText, networkSpeed), "", value);
                    break;
                case EProgressType.UpdateLanguageDeCompress:
                    this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("DeCompressAssets"), value);
                    break;
                case EProgressType.FirstInit:
                case EProgressType.LoadScene:
                default:
                    if (this.mProgressValue <= 50)
                        this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("Initializing"), value);
                    else if (this.mProgressValue <= 90)
                        this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("ArrangingAssets"), value);
                    else
                        this.mCurUILoadingView.Refresh($"{this.mProgressValue}%",CSDisplayText.GetMessage("EnteringGame"), value);
                    break;
            }
        }
        
        public void assetHandler(string name, float value)
        {
            if (this.mCurUILoadingView == null) return;
            string loadingTxt = string.Format(CSDisplayText.GetMessage("LoadingX"), name);
            this.mCurUILoadingView.Refresh(string.Format("{0} {1}%", loadingTxt, (int)(value * 100)), "", value);
        }
        
        public void SetLoadType(int _type, int _size = 0, int _totalSize = 0)
        {
            mCurProgressType = (EProgressType)_type;
            totalSize = _totalSize;
            lastLoadSize = _size;
            loadTime = Time.realtimeSinceStartup - 1f;
            networkSpeed = string.Empty;
            if (this.mCurProgressType == EProgressType.UpdatePatch || this.mCurProgressType == EProgressType.UpdateDeCompressUpdate)
            {
                hasPatch = true;
            }
        }
        
    }
}