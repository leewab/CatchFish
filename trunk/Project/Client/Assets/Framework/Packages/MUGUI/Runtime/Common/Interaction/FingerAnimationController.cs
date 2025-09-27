using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MUEngine;

namespace Game.UI
{
    public enum FingerAnimationType
    {
        Click,
        Drag,
        CurvesDrag,
    }

    [ExecuteInEditMode]
    public class FingerAnimationController : UIBehaviour
    {
        //手指图片,该控件会直接更改对应image的位置，所以必须在UI中设置正确的父节点和anchor
        [SerializeField]
        private Image fingerImage;
        //箭头动画的根节点
        [SerializeField]
        private Transform arrowRoot;

        private bool _isInverse = false;
        public bool IsInverse
        {
            get { return _isInverse; }
            set
            {
                if (_isInverse == value) return;
                _isInverse = value;
                if(fingerImage != null)
                {
                    fingerImage.rectTransform.localRotation = _isInverse ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(0, 0, 0);
                }
            }
        }


        //UI配置项
        [SerializeField]
        private FingerClickAnimation clickAnimation;
        [SerializeField]
        private FingerDragAnimation dragAnimation;
        //曲线引导专用配置，通用性不是很大，目前仅用于少数特殊的引导（比如：引导转镜头）
        //目前，曲线引导不会有箭头，所以会直接忽略相关的处理
        [SerializeField]
        private FingerCurveDragAnimation curvesDragAnimation;    
        [SerializeField]
        private ArrowAnimation arrowAnimation;

        //当前动画类型
        [SerializeField]
        private FingerAnimationType animationType;
        [SerializeField]
        private float fingerScale = 1;
        [SerializeField]
        private RectTransform targetRect;
        [SerializeField]
        private Vector2 startOffset;
        [SerializeField]
        private Vector2 endOffset; //当动画类型不为drag时，该值会被忽略

        private Vector2 startPos;
        private Vector2 endPos;

        private float totalDistance;  //当动画类型不为drag时，该值会被忽略
        private float arrowTotalDistance; //箭头动画的总距离
        private Vector2 arrowNormalDirection;  //箭头动画的方向
        private Vector2 arrowStartPos;    //箭头动画的起点
        private Vector2 arrowEndPos;    //箭头动画的终点
        [System.NonSerialized]
        private List<ArrowImageData> imageDataList; //arrow动画的总数据

        //该值仅用于预览，运行时会忽略该值
        [SerializeField]
        private float timeScale = 1;

        //为了在Editor工程中能运行，直接使用Update来完成动画效果
        private bool isPlayingAnimation = false;
        private bool isPause = false;
        private float currentTime = 0;
        private void Update()
        {
            if(isPlayingAnimation && !isPause)
            {
#if UNITY_EDITOR
                //允许预览模式的时间缩放，运行模式忽略此参数
                currentTime += Application.isPlaying ? Time.deltaTime : Time.deltaTime * timeScale;
#else
                currentTime += Time.deltaTime;
#endif
                RecaculatePositionData();
                if (animationType == FingerAnimationType.Click)
                {
                    UpdateClickAnimation(currentTime);
                }else if(animationType == FingerAnimationType.Drag){
                    UpdateDragAnimation(currentTime);
                    if (dragAnimation.UseArrowAnimation)
                    {
                        UpdateArrowAnimation(currentTime);
                    }
                }else if(animationType == FingerAnimationType.CurvesDrag)
                {
                    UpdateCurvesDragAnimation(currentTime);
                }
            }
        }
        //目标RectTransform位置可能随时发生变化，所以每一帧重算相关数据
        private void RecaculatePositionData()
        {
            if(targetRect == null || (transform.parent as RectTransform) == null)
            {
                return;
            }
            Vector2 targetPos = ConvertCenterPos(targetRect, transform.parent as RectTransform);
            startPos = targetPos + startOffset;
            endPos = targetPos + endOffset;
            //UnityEngine.Debug.LogFormat("update position data ,current target pos is : {0},startPos is : {1},endPos is : {2}", targetPos, startPos, endPos);
            if (FingerAnimationType.Drag == animationType && dragAnimation.UseArrowAnimation)
            {
                var normalX = (endPos - startPos).normalized;
                var normalY = new Vector2(-normalX.y, normalX.x);
                arrowStartPos = startPos + normalX * arrowAnimation.arrowStartOffset.x + normalY * arrowAnimation.arrowStartOffset.y;
                arrowEndPos = endPos + normalX * arrowAnimation.arrowEndOffset.x + normalY * arrowAnimation.arrowEndOffset.y;
            }
        }
        //将目标RectTransfrom的中心位置转化为同一屏幕点上的另一个RectTransfrom的位置
        private Vector2 ConvertCenterPos(RectTransform target, RectTransform current)
        {
            //目标RectTransform中心点的局部坐标（相对轴点坐标）
            var targetCenterOffset = Vector2.Scale(new Vector2(0.5f, 0.5f) - target.pivot, target.rect.size);
            //目标RectTransform中心点，在当前RectTransform下的局部坐标（相对于轴点坐标）
            var currentPosOffset = (Vector2)current.InverseTransformPoint(target.TransformPoint(targetCenterOffset));
            //目标RectTransfrom中心点，相对当前RectTransform左下角的坐标
            return currentPosOffset + Vector2.Scale(current.rect.size, current.pivot - Vector2.zero);
        }

        public void BeginAnimation(FingerAnimationType animationType,RectTransform targetRect,Vector2 startOffset,Vector2 endOffset,
            float fingerScale = 1,float moveDuration = -1,bool breakPause = true,bool forceVisible = true)
        {
            if (isPause && breakPause)
            {
                ContinueAnimation();
            }
            if (forceVisible)
            {
                SetVisible(true);
            }
            this.animationType = animationType;
            this.targetRect = targetRect;
            this.startOffset = startOffset;
            this.fingerScale = fingerScale;
            this.endOffset = endOffset;
            this.totalDistance = Vector2.Distance(startOffset, endOffset);

            //打个补丁，允许外部设置拖动速度。 由于是补丁代码，和原先的设计稍有偏差，不过暂时就这样了
            if(moveDuration != -1)
            {
                if(animationType == FingerAnimationType.Drag && dragAnimation != null)
                {
                    dragAnimation.SetDuration(moveDuration);
                }
                if(animationType == FingerAnimationType.CurvesDrag && curvesDragAnimation != null)
                {
                    curvesDragAnimation.SetDuration(moveDuration);
                }
            }

            RecaculatePositionData();
            SetBeginState();
            this.isPlayingAnimation = true;
        }

        //仅用于预览动画的方法，此时的相关参数都已经通过编辑器设置好了，这里只需要启动动画就好了
        [Conditional("UNITY_EDITOR")]
        public void BeginAnimationForPreview()
        {
            ContinueAnimation();
            SetVisible(true);
            //this.currentTime = 0;
            this.totalDistance = Vector2.Distance(startOffset, endOffset);
            //this.normalDirection = (endPos - startPos).normalized;
            RecaculatePositionData();
            SetBeginState();
            this.isPlayingAnimation = true;
        }

        public void EndAnimation(bool breakPause = true,bool forceInVisible = true)
        {
            if (!isPlayingAnimation)
            {
                return;
            }
            if (isPause && breakPause)
            {
                ContinueAnimation();
            }
            if(forceInVisible)
            {
                SetVisible(false);
            }
            if(clickEffect != null)
            {
                MURoot.Scene.DelEntity(clickEffect);
                clickEffect = null;
            }
            if(displayImages.Count > 0)
            {
                ReleaseAllImage();
            }
            isPlayingAnimation = false;
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

        public void SetVisible(bool visible)
        {
            if(fingerImage != null)
            {
                fingerImage.enabled = visible;
            }
            if(arrowRoot != null)
            {
                arrowRoot.gameObject.SetActive(visible);
            }
        }

        private void SetBeginState()
        {
            currentTime = 0;
            if (fingerImage != null)
            {
                fingerImage.SetNativeSize();
                fingerImage.rectTransform.anchoredPosition = startPos;
                fingerImage.rectTransform.localScale = new Vector3(fingerScale, fingerScale, fingerScale);
                fingerImage.color = Color.white;
            }
            if (clickEffect != null)
            {
                MURoot.Scene.DelEntity(clickEffect);
                clickEffect = null;
            }
            ReleaseAllImage();
            if(FingerAnimationType.Drag == animationType && dragAnimation.UseArrowAnimation)
            {
                //var normalX = (endPos - startPos).normalized;
                //var normalY = new Vector2(-normalX.y, normalX.x);
                //arrowStartPos = startPos + normalX * arrowAnimation.arrowStartOffset.x + normalY * arrowAnimation.arrowStartOffset.y;
                //arrowEndPos = endPos + normalX * arrowAnimation.arrowEndOffset.x + normalY * arrowAnimation.arrowEndOffset.y;
                arrowTotalDistance = (arrowEndPos - arrowStartPos).magnitude;
                arrowNormalDirection = (arrowEndPos - arrowStartPos).normalized;
                imageDataList = new List<ArrowImageData>();
                lastEmittedIndex = -1;
            }
        }

        private void UpdateDragAnimation(float time)
        {
            if (fingerImage == null || dragAnimation == null || animationType != FingerAnimationType.Drag)
            {
                return;
            }
            float factor = dragAnimation.GetCurrentPathFactor(time, totalDistance);
            Vector2 offset = dragAnimation.GetOffset(time, totalDistance);
            fingerImage.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, factor) + offset;
        }
        private void UpdateCurvesDragAnimation(float time)
        {
            if (fingerImage == null || curvesDragAnimation == null || animationType != FingerAnimationType.CurvesDrag)
            {
                return;
            }
            //计算位置
            Vector2 normlizedOffset = curvesDragAnimation.GetCurrentOffset(time);
            Vector2 xAxis = endPos - startPos;
            Vector2 yAxis = new Vector2(-xAxis.y, xAxis.x);
            Vector2 targetPos = startPos + xAxis * normlizedOffset.x + yAxis * normlizedOffset.y;
            fingerImage.rectTransform.anchoredPosition = targetPos;
            //计算Alpha
            float alpha = curvesDragAnimation.GetAlphaValue(time);
            fingerImage.CrossFadeAlpha(alpha, 0, true);
        }

        [System.NonSerialized]
        private Entity clickEffect = null;
        [System.NonSerialized]
        private float logicStartTime = 0;
        private void UpdateClickAnimation(float time)
        {
            if(fingerImage == null || clickAnimation == null || animationType != FingerAnimationType.Click)
            {
                return;
            }
            Vector2 originSize = new Vector2(fingerImage.preferredWidth, fingerImage.preferredHeight);
            Vector2 currentSize = originSize * clickAnimation.GetCurrentSizeFactor(time);
            float currentAlpha = clickAnimation.GetCurrentAlpha(time);
            fingerImage.rectTransform.anchoredPosition = startPos;
            fingerImage.CrossFadeAlpha(currentAlpha, 0, true);

            fingerImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, currentSize.x);
            fingerImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentSize.y);

            //根据时间确定特效
            if(clickEffect == null && time > clickAnimation.GetEffectDelayTime())
            {
                clickEffect = MURoot.Scene.AddEffect(clickAnimation.GetEffectName(), Vector3.zero, Quaternion.identity,clickAnimation.GetEffectScale());
                if(clickEffect == null)
                {
                    return;
                }
                logicStartTime = Time.time - (time - clickAnimation.GetEffectDelayTime());
                clickEffect.Parent = transform;
                clickEffect.OnLoadResource += () =>
                {
                    GameObjectUtil.SetLayer(clickEffect.GameObject, 5, true);
                    clickEffect.GameObject.transform.localPosition = clickAnimation.GetEffectOffset();
                    var currentSortingOrder = GetComponentInParent<Canvas>().sortingOrder;
                    foreach(var renderer in clickEffect.GameObject.GetComponentsInChildren<Renderer>())
                    {
                        //特效出现在手指下面，其它UI上面，所以给手指单独加一个Canvas，并且让它的sortingOrder正好比引导UI整体的sortingOrder大2
                        //Unity渲染次序规则 camera depth > sortingLayer > sortingOrder + renderQueue (居然是相加，卧槽) > 其它（比如UGUI可能根据层级排序，不透明物品可能从前到后排序，可能按距离排序。。。）
                        renderer.sortingOrder = currentSortingOrder - 1;
                        if(renderer.material != null)
                        {
                            //UGUI材质的renderQueue都是3000
                            renderer.material.renderQueue = 3000;
                        }
                    }
                    foreach (var particle in clickEffect.GameObject.GetComponentsInChildren<ParticleSystem>())
                    {
                        var m = particle.main;
                        m.simulationSpeed = clickAnimation.GetEffectSpeed();
                        particle.time = (Time.time - logicStartTime) * clickAnimation.GetEffectSpeed();
                    }
                };
                clickEffect.Load();
            }
        }


        [System.NonSerialized]
        private int lastEmittedIndex = -1;
        private void UpdateArrowAnimation(float time)
        {
#if UNITY_EDITOR
            //非运行模式下，很难准确回收自己生成的额外资源，所以直接不播放箭头动画
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            //由于箭头需要与fingerImage对齐，所以必须拥有fingerImage的引用，才能计算正确位置
            if(arrowRoot == null || fingerImage == null)
            {
                return;
            }
            time -= dragAnimation.DelayArrowDisplay;
            if(time < 0)
            {
                return;
            }
            var currentIndex = Mathf.FloorToInt(time / arrowAnimation.emitInterval);
            if (currentIndex > lastEmittedIndex)
            {
                //发射一个新的Image
                var imageData = new ArrowImageData();
                imageData.image = GetImage();
                imageData.index = currentIndex;
                imageDataList.Add(imageData);
                lastEmittedIndex = currentIndex;
            }
            //更新每一个Image的位置，朝向，大小和透明度
            for (int i = imageDataList.Count - 1; i >= 0; i--)
            {
                var data = imageDataList[i];
                Vector2 offset = arrowNormalDirection * (time - data.index * arrowAnimation.emitInterval) * arrowAnimation.speed;
                float normalLength = offset.magnitude / arrowTotalDistance;
                if (normalLength > 1)
                {
                    //超过了最大距离，回收它
                    ReleaseImage(data.image);
                    imageDataList.RemoveAt(i);
                    continue;
                }
                var fingerParent = fingerImage.rectTransform.parent as RectTransform;
                //之前为了方便，startPos会直接赋值给fingerImage，而且fingerImage的锚点在父节点的左下角，所以这里需要做一些其它操作,来进行“对齐”
                Vector3 worldPos = fingerParent.TransformPoint(arrowStartPos + offset + Vector2.Scale(fingerParent.rect.size, -fingerParent.pivot));
                data.image.rectTransform.localPosition = arrowRoot.InverseTransformPoint(worldPos);
                //更改其朝向
                data.image.rectTransform.right = fingerParent.TransformVector(arrowNormalDirection);
                //根据距离值，更改其大小与alpha
                float size = arrowAnimation.sizeCurve.Evaluate(normalLength);
                data.image.rectTransform.localScale = new Vector3(size, size, size);
                float alpha = arrowAnimation.alphaCurve.Evaluate(normalLength);
                data.image.color = new Color(1, 1, 1, alpha);
            }
        }

        [System.NonSerialized]
        private HashSet<Image> displayImages = new HashSet<Image>();
        [System.NonSerialized]
        private Stack<Image> unusedImages = new Stack<Image>();
        private Image GetImage()
        {
            Image image = null;
            if (unusedImages.Count != 0)
            {
                image = unusedImages.Pop();
            }
            if (image == null)
            {
                GameObject obj = new GameObject("temp-arrow-image");
                obj.transform.parent = arrowRoot;
                image = obj.AddComponent<Image>();
                image.sprite = arrowAnimation.sprite;
                image.SetNativeSize();
                image.raycastTarget = false;
                image.rectTransform.anchoredPosition = Vector2.zero;
            }
            image.enabled = true;
            displayImages.Add(image);
            return image;
        }
        private void ReleaseImage(Image image)
        {
            if (!displayImages.Contains(image))
            {
                return;
            }
            image.enabled = false;
            displayImages.Remove(image);
            unusedImages.Push(image);
        }
        private void ReleaseAllImage()
        {
            foreach (var image in displayImages)
            {
                image.enabled = false;
                unusedImages.Push(image);
            }
            displayImages.Clear();
        }

        //代表手指点击UI动画的相关配置
        [System.Serializable]
        private class FingerClickAnimation
        {
            //size变化曲线，它的key的时间应该是从0-1,代表一个周期中Size变化的规律
            [SerializeField]
            private AnimationCurve _sizeCurve = AnimationCurve.Linear(0, 0, 1, 1);
            [SerializeField]
            private AnimationCurve _alphaCurve = AnimationCurve.Linear(0, 0, 1, 1);
            [SerializeField]
            private float duration = 0.3f;
            [SerializeField]
            private bool isSizeLoop = true;
            [SerializeField]
            private bool isAlphaLoop = false;

            //点击特效配置
            [SerializeField]
            private string clickEffectName = null;
            [SerializeField]
            private float effectDelayTime = 0.3f;
            [SerializeField]
            private Vector2 effectOffset = Vector2.zero;
            [SerializeField]
            private float effectScale = 1;
            [SerializeField]
            private float speedScale = 1;

            //传入一个已经过的时间（不需要归一化），传出一个Size比例
            public float GetCurrentSizeFactor(float time)
            {
                if (duration <= 0)
                {
                    return _sizeCurve.Evaluate(0);
                }
                float times = Mathf.Floor(time / duration);
                float offset = time - times * duration;
                if (isSizeLoop || times < 1)
                {
                    return _sizeCurve.Evaluate(offset / duration);
                }
                return _sizeCurve.Evaluate(1);
            }

            public float GetCurrentAlpha(float time)
            {
                if (duration <= 0)
                {
                    return _alphaCurve.Evaluate(0);
                }
                float times = Mathf.Floor(time / duration);
                float offset = time - times * duration;
                if (isAlphaLoop || times < 1)
                {
                    return _alphaCurve.Evaluate(offset / duration);
                }
                return _alphaCurve.Evaluate(1);
            }

            public string GetEffectName()
            {
                return clickEffectName;
            }

            public float GetEffectDelayTime()
            {
                return effectDelayTime;
            }

            public float GetEffectScale()
            {
                return effectScale;
            }

            public float GetEffectSpeed()
            {
                return speedScale;
            }

            public Vector2 GetEffectOffset()
            {
                return effectOffset;
            }
        }

        //代表手指拖动UI动画的相关配置
        [System.Serializable]
        private class FingerDragAnimation
        {
            //位移变化曲线，它的key的时间应该是0-1，值为0-1，代表从起点到终点的变化方式
            [SerializeField]
            private AnimationCurve _pathCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //平均数据，目前采用一个固定值
            [SerializeField]
            private float duration = 1;
            //一个附加位移
            [SerializeField]
            private Vector2 extraOffset;
            //附加偏移的变化曲线,它的key的时间为0-1，代表从开始到最后的附加位移偏移值的变化
            [SerializeField]
            private AnimationCurve _offsetCurve = AnimationCurve.Linear(0, 0, 1, 0);
            [SerializeField]
            private bool useArrowAnimation = true;
            [SerializeField]
            private float delayArrowDisplay = 0.3f;

            public void SetDuration(float duration)
            {
                this.duration = duration;
            }

            public float GetCurrentPathFactor(float time,float totalDistance)
            {
                if (duration == 0)
                {
                    return _pathCurve.Evaluate(0);
                }
                var offset = time - Mathf.Floor(time / duration) * duration;
                return _pathCurve.Evaluate(offset/ duration);
            }
            public Vector2 GetOffset(float time, float totalDistance)
            {
                if (duration == 0)
                {
                    return extraOffset * _pathCurve.Evaluate(0);
                }
                var offset = time - Mathf.Floor(time / duration) * duration;
                return extraOffset * _offsetCurve.Evaluate(offset / duration);
            }

            public bool UseArrowAnimation { get { return useArrowAnimation; } }
            public float DelayArrowDisplay { get { return delayArrowDisplay; } }
        }

        //箭头动画相关，为了更好的统一管理，将箭头动画直接内嵌到这里
        private class ArrowImageData
        {
            //一个逐渐递增的内部index，用于简化计算
            public int index;
            public Image image;
        }
        //拖动箭头动画的相关配置
        [System.Serializable]
        private class ArrowAnimation
        {
            public Sprite sprite;
            public float emitInterval = 0.5f;
            public float speed = 100;
            public AnimationCurve alphaCurve = AnimationCurve.Linear(0, 1, 1, 1);
            public AnimationCurve sizeCurve = AnimationCurve.Linear(0, 1, 1, 1);
            public Vector2 arrowStartOffset;
            public Vector2 arrowEndOffset;
        }

        [System.Serializable]
        private class FingerCurveDragAnimation
        {
            //位移变化曲线，它的key的时间应该是0-1，值为0-1，代表从起点到终点的变化方式
            [SerializeField]
            private AnimationCurve _pathCurve = AnimationCurve.Linear(0, 0, 1, 1);
            //总时间
            [SerializeField]
            private float duration = 2;
            //描述曲线 形状的曲线，X轴代表当前沿着X轴的位移，Y轴代表沿着Y轴的位移
            [SerializeField]
            private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 0);
            //Alpha变化曲线(横轴依然是归一化时间)
            [SerializeField]
            private AnimationCurve _alphaCurve = AnimationCurve.Linear(0, 1, 1, 1);
            //Alpha是否循环变化
            [SerializeField]
            private bool _isAlphaLoop = false;

            public void SetDuration(float duration)
            {
                this.duration = duration;
            }

            public Vector2 GetCurrentOffset(float time)
            {
                if(Mathf.Approximately(duration,0))
                {
                    return Vector2.zero;
                }
                time = (time % duration) / duration;
                float x = _pathCurve.Evaluate(time);
                float y = _curve.Evaluate(x);
                return new Vector2(x, y);
            }

            public float GetAlphaValue(float time)
            {
                if (Mathf.Approximately(duration, 0))
                {
                    return 1;
                }
                if(time > duration && !_isAlphaLoop)
                {
                    time = 1;
                }
                else
                {
                    time = (time % duration) / duration;
                }
                return _alphaCurve.Evaluate(time);
            }
        }
    }
}

