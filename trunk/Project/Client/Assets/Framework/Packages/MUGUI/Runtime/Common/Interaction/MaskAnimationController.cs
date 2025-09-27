using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    //该类用于控制遮罩动画，并且提供配置各种动画参数的接口
    [ExecuteInEditMode]
    public class MaskAnimationController : UIBehaviour
    {
        //AreaMaskImage的RectTransform必须与该RectTransform保持一致,否则难以保证效果与预期一致
        [SerializeField]
        private AreaMaskImage areaMaskImage;
        //RectImage的轴点必须为0.5,0.5,锚点必须为左下角
        [SerializeField]
        private Image rectImage;

        //UI配置项
        [SerializeField]
        private MaskAnimation maskAnimation = new MaskAnimation();
        [SerializeField]
        private RectAnimation rectAnimation = new RectAnimation();
        [SerializeField]
        private Vector2 rectOutLineWidth = new Vector2(3, 3);
        [SerializeField]
        private Vector2 maskOutLineWidth = new Vector2(0, 0);
        [SerializeField]
        private float endEdgeWidth = 10;
        [SerializeField]
        private float startSizeFactor = 3;
        [SerializeField]
        private float startEdgeFactor = 1;

        //该数据用于预览，运行时程序会覆盖该值
        [SerializeField]
        private bool showMask = false;
        [SerializeField]
        private bool showRect = false;
        [SerializeField]
        private RectTransform targetTransform = null;
        [SerializeField]
        private bool tracingTarget = true;


        //该值仅用于预览，运行时会忽略该值
        [SerializeField]
        private float timeScale = 1;

        //程序内部的计算值
        private Vector2 startSize;
        private Vector2 startPos;
        private float startEdgeWidth;
        private Vector2 endSize;
        private Vector2 endPos;

        public RectTransform rectTransform
        {
            get
            {
                return transform as RectTransform;
            }
        }

        //为了在Editor工程也能正常执行动画，这里不使用TimerHandler，直接用原生的Update去做
        private Action callback = null;
        private float currentTime = 0;
        private bool isPlayingAnimation = false;
        private bool isPause = false;
        private bool isTracingTarget = false;
        void Update()
        {
            if (isPlayingAnimation && !isPause)
            {
                //允许预览模式的时间缩放，运行模式忽略此参数
                currentTime += Application.isPlaying ? Time.deltaTime : Time.deltaTime * timeScale;
                //currentTime += 0.01f;
                UpdateAnimation();
            }else if(!isPlayingAnimation && isTracingTarget)
            {
                UpdateTracingTarget();
            }
        }

        //根据currentTime来更新一次动画状态
        private void UpdateAnimation()
        {
            //当targetTransform == rectImage.rectTransform 时，播放动画，Unity会直接Crash。。。。。
            if(targetTransform == null || (rectImage.rectTransform != null && rectImage.rectTransform == targetTransform))
            {
                return;
            }
            //每帧重算位置，避免在播放动画的过程中，目标产生了变化，导致问题，目前这样，至少可以保证，在播放动画期间，即使目标发生变化，也会有追踪效果
            endPos = ConvertCenterPos(targetTransform, rectTransform);
            endSize = ConvertTotalSize(targetTransform, rectTransform);
            startSize = endSize * startSizeFactor;
            startEdgeWidth = (startSize.x + startSize.y) * startEdgeFactor;
            startPos = KeepInViewport(endPos, startSize, rectTransform.rect.size);
            if (showMask)
            {
                UpdateAreaMask(Mathf.Clamp01(maskAnimation.Duration > 0 ? currentTime / maskAnimation.Duration : 1));
            }
            if (showRect)
            {
                UpdateRectImage(Mathf.Clamp01(rectAnimation.Duration > 0 ? currentTime / rectAnimation.Duration : 1));
            }
            //停止动画
            if (currentTime > rectAnimation.Duration && currentTime > maskAnimation.Duration)
            {
                isPlayingAnimation = false;
                currentTime = 0;
                CheckCallBack();
            }
        }
        //动画停止后，可以继续保持Rect和Mask追踪目标
        private void UpdateTracingTarget()
        {
            if (targetTransform == null || (rectImage.rectTransform != null && rectImage.rectTransform == targetTransform))
            {
                return;
            }
            endPos = ConvertCenterPos(targetTransform, rectTransform);
            endSize = ConvertTotalSize(targetTransform, rectTransform);
            //startSize = endSize * startSizeFactor;
            //startEdgeWidth = (startSize.x + startSize.y) * startEdgeFactor;
            //startPos = KeepInViewport(endPos, startSize, rectTransform.rect.size);
            if (showMask)
            {
                UpdateAreaMask(1);
            }
            if (showRect)
            {
                UpdateRectImage(1);
            }
        }


        public void BeginAnimation(GuideAnimationData animationData, Action callback = null, bool breakPause = true)
        {
            //如果之前有未完成的回调，则此时强制触发
            CheckCallBack();

            if (isPause && breakPause)
            {
                ContinueAnimation();
            }
            showMask = animationData.showMask;
            showRect = animationData.showRect;
            targetTransform = animationData.targetRect;
            tracingTarget = animationData.tracingTarget;

            currentTime = 0;
            SetBeginState();
            isPlayingAnimation = true;

            if (tracingTarget)
            {
                BeginTracingTarget();
            }

            //动画正常启动后，再添加完成的callback
            this.callback = callback;
        }

        public void EndAnimation(bool breakPause = true)
        {
            //立刻停止追踪状态
            EndTracingTarget();

            if (!isPlayingAnimation)
            {
                return;
            }
            if (isPause && breakPause)
            {
                ContinueAnimation();
            }
            isPlayingAnimation = false;

            CheckCallBack();
        }

        //允许外部手动开启和关闭追踪目标的状态，但是目标必须是通过BeginAnimation设置的
        //内部开关动画时，也会调用这两个函数
        public void BeginTracingTarget()
        {
            isTracingTarget = true;
        }
        public void EndTracingTarget()
        {
            isTracingTarget = false;
        }

        public void PauseAnimation()
        {
            isPause = true;
        }

        public void ContinueAnimation()
        {
            isPause = false;
        }

        public bool IsPlaying()
        {
            return isPlayingAnimation;
        }

        public bool IsPause()
        {
            return isPause;
        }

        //仅用于预览动画的方法，此时的相关参数都已经通过编辑器设置好了，这里只需要启动动画就好了
        [Conditional("UNITY_EDITOR")]
        public void BeginAnimationForPreview()
        {
            ContinueAnimation();
            this.currentTime = 0;
            SetBeginState();
            this.isPlayingAnimation = true;
            this.isTracingTarget = tracingTarget;
        }

        //将归一化的坐标转化为实际坐标
        private Vector2 ConvertNormalizedVector(Vector2 vector2)
        {
            vector2.x *= rectTransform.rect.width;
            vector2.y *= rectTransform.rect.height;
            return vector2;
        }

        //设置初始状态，主要设置图片显隐,然后通过UpdateAnimation来设置位置Size为初始状态
        private void SetBeginState()
        {
            if (areaMaskImage != null)
            {
                areaMaskImage.enabled = showMask;
            }
            if (rectImage != null)
            {
                rectImage.enabled = showRect;
            }
            //此时,currentTime应该保证为0
            UpdateAnimation();
        }

        //检查是否有完成的callback,如果有，就触发它，然后将回调置为空
        private void CheckCallBack()
        {
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }

        //更新遮罩图片的相关数据
        private void UpdateAreaMask(float normalizeTime)
        {
            if (areaMaskImage == null || maskAnimation == null)
            {
                return;
            }
            var data = maskAnimation.GetMaskAnimationData(normalizeTime);
            var currentPos = Vector2.Lerp(startPos, endPos, data.path);
            var currentSize = Vector2.Lerp(startSize, endSize + maskOutLineWidth, data.size);
            var currentEdgeWidth = Mathf.Lerp(startEdgeWidth, endEdgeWidth, data.EdgeSizeFactor);
            areaMaskImage.SetCenter(currentPos);
            areaMaskImage.SetSize(currentSize/2);
            areaMaskImage.SetBackgroundColor(data.backgroundColor);
            areaMaskImage.SetEdgeX(currentEdgeWidth);
            areaMaskImage.SetForegroundColor(data.foregroundColor);
            areaMaskImage.SetCircleRadiusFactor(data.circleRadiusFactor);
        }

        //更改矩形图片的相关数据
        private void UpdateRectImage(float normalizedTime)
        {
            if (rectImage == null || rectAnimation == null)
            {
                return;
            }
            var pathFactor = rectAnimation.GetRectPathFactor(normalizedTime);
            var sizeFactor = rectAnimation.GetRectSizeFactor(normalizedTime);
            var currentPos = Vector2.Lerp(startPos, endPos, pathFactor);
            var currentSize = Vector2.Lerp(startSize, endSize + rectOutLineWidth, sizeFactor);
            rectImage.rectTransform.anchoredPosition = currentPos;
            //下面这句话，可以让Unity Crash，Unity 5.6.2f1
            //currentSize = new Vector2(62425180000.0f, 42014650000.0f);
            rectImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize.x);
            rectImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentSize.y);
        }
        //将目标RectTransfrom的中心位置转化为同一屏幕点上的另一个RectTransfrom的位置
        public Vector2 ConvertCenterPos(RectTransform target, RectTransform current)
        {
            //目标RectTransform中心点的局部坐标（相对轴点坐标）
            var targetCenterOffset = Vector2.Scale(new Vector2(0.5f, 0.5f) - target.pivot, target.rect.size);
            //目标RectTransform中心点，在当前RectTransform下的局部坐标（相对于轴点坐标）
            var currentPosOffset = (Vector2)current.InverseTransformPoint(target.TransformPoint(targetCenterOffset));
            //目标RectTransfrom中心点，相对当前RectTransform左下角的坐标
            return currentPosOffset + Vector2.Scale(current.rect.size, current.pivot - Vector2.zero);
        }
        //把目标RectTransform的大小转化为目标RectTransform下的大小
        public Vector2 ConvertTotalSize(RectTransform target, RectTransform current)
        {
            return current.InverseTransformVector(target.TransformVector(target.rect.size));
        }
        private Vector2 KeepInViewport(Vector2 endPos, Vector2 startSize,Vector2 totalSize)
        {
            //该函数，负责“尽量”让初始时的整个Rect都在视野之内
            var startHalfWidth = startSize.x / 2;
            var startHalfHeight = startSize.y / 2;
            var startX = endPos.x;
            var startY = endPos.y;

            //确定X
            if (startHalfWidth > totalSize.x)
            {
                startX = totalSize.x / 2;
            }
            else if (startX - startHalfWidth < 0)
            {
                startX = startHalfWidth;
            }
            else if (startX + startHalfWidth > totalSize.x)
            {
                startX = totalSize.x - startHalfWidth;
            }

            //确定Y
            if (startHalfHeight > totalSize.y)
            {
                startY = totalSize.y / 2;
            }
            else if (startY - startHalfHeight < 0)
            {
                startY = startHalfHeight;
            }
            else if (startY + startHalfHeight > totalSize.y)
            {
                startY = totalSize.y - startHalfHeight;
            }

            return new Vector2(startX, startY);
        }

        public class GuideAnimationData
        {
            public RectTransform targetRect;
            public bool showMask = false;
            public bool showRect = false;
            public bool tracingTarget = true;
        }

        //代表一个遮罩动画的固定配置（除了Size和Center外的其它配置）
        [System.Serializable]
        private class MaskAnimation
        {
            public class MaskAnimationData
            {
                //当前的路径系数
                public float path;
                //当前Size系数
                public float size;
                //背景颜色
                public Color backgroundColor;
                //前景颜色
                public Color foregroundColor;
                //边大小系数
                public float EdgeSizeFactor;
                //圆角半径系数
                public float circleRadiusFactor;
            }
            //位置变化曲线，它的key的时间应该是从0-1，代表从开始位置变化到目标位置的路径变化（0为初始位置，1为目标位置）
            [SerializeField]
            private AnimationCurve _pathCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //size变化曲线，它的key的时间应该是从0-1，代表从开始位置变化到目标位置的Size变化（0为初始位置，1为目标位置）
            [SerializeField]
            private AnimationCurve _sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //初始背景颜色
            [SerializeField]
            private Color startBackgroundColor = new Color(0, 0, 0, 0.1f);
            //最终背景颜色
            [SerializeField]
            private Color endBackgroundColor = new Color(0, 0, 0, 0.5f);
            //背景颜色变化曲线，key的时间0-1，值0-1,0代表初始颜色，1代表目标颜色
            [SerializeField]
            private AnimationCurve _backgroundColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //初始前景色
            [SerializeField]
            private Color startForegroundColor = new Color(1, 1, 1, 0);
            //最终前景色
            [SerializeField]
            private Color endForegroundColor = new Color(1, 1, 1, 0);
            //前景颜色变化曲线，key的时间0-1，值0-1,0代表初始颜色，1代表目标颜色
            [SerializeField]
            private AnimationCurve _foregroundColorCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //边大小变化曲线，key的时间0-1
            [SerializeField]
            private AnimationCurve _EdgeSizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //圆角半径系数变化曲线
            [SerializeField]
            private AnimationCurve _circleRadiusFactorCurve = AnimationCurve.Linear(0, -0.5f, 1, 0.3f);

            //传入归一化动画时间，得到当前动画数据
            public MaskAnimationData GetMaskAnimationData(float normalizedTime)
            {
                MaskAnimationData maskAnimationData = new MaskAnimationData();
                maskAnimationData.path = _pathCurve.Evaluate(normalizedTime);
                maskAnimationData.size = _sizeCurve.Evaluate(normalizedTime);
                maskAnimationData.backgroundColor = Color.Lerp(startBackgroundColor, endBackgroundColor, _backgroundColorCurve.Evaluate(normalizedTime));
                maskAnimationData.foregroundColor = Color.Lerp(startForegroundColor, endForegroundColor, _foregroundColorCurve.Evaluate(normalizedTime));
                maskAnimationData.EdgeSizeFactor = _EdgeSizeCurve.Evaluate(normalizedTime);
                maskAnimationData.circleRadiusFactor = _circleRadiusFactorCurve.Evaluate(normalizedTime);
                return maskAnimationData;
            }

            [SerializeField]
            private float maskAnimationDuration = 0.5f;

            public float Duration { get { return maskAnimationDuration; } }
        }

        //代表框框UI动画的相关配置
        [System.Serializable]
        private class RectAnimation
        {
            //size变化曲线，它的key的时间应该是从0-1，代表从开始位置变化到目标位置的Size变化（0为初始位置，1为目标位置）
            [SerializeField]
            private AnimationCurve _sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //位置变化曲线，它的key的时间应该是从0-1，代表从开始位置变化到目标位置的路径变化（0为初始位置，1为目标位置）
            [SerializeField]
            private AnimationCurve _pathCurve = AnimationCurve.Linear(0, 0, 1, 1);

            public float GetRectSizeFactor(float normalizedTime)
            {
                return _sizeCurve.Evaluate(normalizedTime);
            }
            public float GetRectPathFactor(float normalizedTime)
            {
                return _pathCurve.Evaluate(normalizedTime);
            }

            [SerializeField]
            private float rectAnimationDuration = 0.6f;

            public float Duration { get { return rectAnimationDuration; } }
        }
    }

}
