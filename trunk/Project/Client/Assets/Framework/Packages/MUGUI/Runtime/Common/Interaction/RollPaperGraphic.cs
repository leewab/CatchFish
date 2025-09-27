using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    //用于模拟卷纸效果的图像 add by liujunjie in 2018/12/25
    [ExecuteInEditMode]
    public class RollPaperGraphic : AlphaFilterPaperGraphic
    {
        //反面图像
        [SerializeField]
        private Texture backImage;

        //当前曲率 (用曲率半径来代表曲率)
        [SerializeField]
        [Range(0.1f, 1000)]
        private float radius = 100;
        //卷曲部分的角度(角度制表示)
        [SerializeField]
        [Range(0, 7200)]
        private float rollAngle;
        //归一化的X坐标
        [SerializeField]
        [Range(0, 1)]
        private float normalX;
        //归一化的Y坐标
        [SerializeField]
        [Range(0, 1)]
        private float normalY;
        //开始卷曲的方向，绕Z轴正方向逆时针旋转的角度
        [SerializeField]
        [Range(0, 360)]
        private float directionAngle;

        [SerializeField]
        [Range(-10, 10)]
        private float circleOffset;

        //提供给程序设置和获取相关属性的接口，并且设置相关数据失效
        //SerializedField被动画系统直接修改时，内部会设置所有数据失效（通过基类中的OnDidApplyAnimationProperties方法）
        public Texture BackImage { get { return backImage; } set { if (SetPropertyUtility.SetClass<Texture>(ref backImage, value)) SetMaterialDirty(); } }
        public float CircleOffset { get { return circleOffset; } set { if (SetPropertyUtility.SetStruct<float>(ref circleOffset, value)) SetMaterialDirty(); } }

        //以下数值既会导致Material刷新，也可能导致IsSimpleMode发生变化，进而导致Mesh刷新，这里不做太多判断，直接声明所有数据需要刷新
        public float Radius { get { return radius; } set { if (SetPropertyUtility.SetStruct<float>(ref radius, value)) SetAllDirty(); } }
        public float RollAngle { get { return rollAngle; } set { if (SetPropertyUtility.SetStruct<float>(ref rollAngle, value)) SetAllDirty(); } }
        public float NormalX { get { return normalX; } set { if (SetPropertyUtility.SetStruct<float>(ref normalX, value)) SetAllDirty(); } }
        public float NormalY { get { return normalY; } set { if (SetPropertyUtility.SetStruct<float>(ref normalY, value)) SetAllDirty(); } }
        public float DirectionAngle { get { return directionAngle; } set { if (SetPropertyUtility.SetStruct<float>(ref directionAngle, value)) SetAllDirty(); } }


        protected override void OnEnable()
        {
            base.OnEnable();
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EnableSimpleMode = false;
            }
#endif
            //这个UI被隐掉之后，再显示出来的时候需要重新生成Mesh（可能被隐掉的时候，相关数据被“自动”清除了）
            SetDefaultValue();
        }

        protected override void UpdateMaterial()
        {
            CheckUpdateMaterial();
            base.UpdateMaterial();
        }

        //更新Mesh的代价较大，需要尽可能的减少更新频率
        protected override void UpdateGeometry()
        {
            //base.UpdateGeometry();
            CheckUpdateMesh();
        }

        private void Update()
        {
            //当PaperGraphic的位置发生变化时，也需要刷新一下卷曲起点的位置，但是只是位置发生变动，不会触发Rebuild，所以只能通过每帧进行检测来达到目的
            //if (SetPropertyUtility.SetStruct<Vector3>(ref lastPosition, rectTransform.position))
            //{
            //    UpdateRollStartPoint();
            //}
        }

        private const float meshDenity = 0.025f;
        //该字段仅用于预览
        [System.NonSerialized]
        public Mesh currentMesh = null;
        private void UpdateMeshData()
        {
            if (currentSimpleMode)
            {
                currentMesh = GenerateSimpleMesh();
            }
            else
            {
                currentMesh = GenerateMesh(meshDenity);
            }
#if UNITY_EDITOR
            Debug.Log("update paper mesh , current mesh mode is simple ? " + currentSimpleMode);
#endif
            canvasRenderer.SetMesh(currentMesh);
        }
        //做一些顶点偏移相关的整体操作，逐顶点的计算在Shader中进行
        private void UpdateRollData()
        {
            //角度值转弧度制
            float radians = Mathf.Deg2Rad * rollAngle;
            //卷曲部分的总长度（卷曲部分与卷曲起点的最大距离）
            float rollDistance = radians * radius;
            //卷曲终点线（这是一条与卷曲起点平行的线，但是可能在“纸”上并不能找到这条线）的高度
            float rollEndLineHeight = radius * (1 - Mathf.Cos(radians));

            //在shader中做逐顶点的计算，依次传入参数
            //卷曲相关数据
            material.SetFloat("_RollDistance", rollDistance);
            material.SetFloat("_RollRadians", radians);
            material.SetFloat("_RollRadius", radius);
            //卷曲终点的相关数据，最复杂的部分，x表示超过卷曲终点线后，距离每增加1，沿着起点normal方向增加的值，y表示超过卷曲终点线后，距离每增加1，z轴增加的值，
            //z表示终点线的的xy值相对于起点normal的比值（它们必定共线），w表示终点线的z值（终点线上的z值必定保持一致）
            float x = Mathf.Cos(radians);
            float y = -Mathf.Sin(radians);
            float z = radius * Mathf.Sin(radians);
            float w = -rollEndLineHeight;
            material.SetVector("_RollEndLine", new Vector4(x, y, z, w));
        }
        private void UpdateTextures()
        {
            //设置贴图
            material.SetTexture("_BackTex", BackImage);
        }
        private void UpdateRollStartPoint()
        {
            //unity canvas的内部似乎会修改传过去的Mesh顶点的顶点坐标（模型坐标系下），这会让代码生成Mesh时设置的顶点坐标与shader中拿到的顶点坐标并不一致
            //目前发现：被canvas render修改后的顶点局部坐标系，都会以对应canvas的位置作为原点，这里通过将卷曲起始点也做同样的变换，来保持卷纸的行为一致
            //缩放x和y目前也是可行的，但是缩放z看不出效果（如果之后需要看到效果，这里可能要将缩放相关数据纳入计算）
            //Vector2 point = Vector2.Scale(new Vector2(normalX - rectTransform.pivot.x, normalY - rectTransform.pivot.y), rectTransform.rect.size);
            //point = canvas.transform.InverseTransformPoint(rectTransform.TransformPoint(point.x, point.y, 0));
            //无法在shader中禁用 batch,所以该为传入归一化的坐标来达到目的

            //用一个点和一个法向量来表示一条线，表示卷曲的起点,该法向量指向需要卷起来的一边（这两个的坐标系都是模型坐标系,并且z都强制为0）
            //左手坐标系下，用左手定则确定AngleAxis的旋转方向
            Vector2 normal = Quaternion.AngleAxis(directionAngle, -Vector3.forward) * Vector2.right;

            //起点相关数据
            Vector4 rollStartLine = new Vector4(normalX, normalY, normal.x, normal.y);
            material.SetVector("_RollStartLine", rollStartLine);
        }
        private void UpdateCircleOffset()
        {
            material.SetFloat("_CircleOffset", circleOffset);
            //根据实际经验，当半径偏移小于1的时候，开启Z写入，并不能很好的看到正确的遮挡关系，开启深度写入反而效果更差
            material.SetInt("_ZWrite", Mathf.Abs(circleOffset) < 1 ? 0 : 1);
        }

        private void UpdateRectSize()
        {
            material.SetFloat("_RectWidth", rectTransform.rect.width);
            material.SetFloat("_RectHeight", rectTransform.rect.height);
        }

        private void UpdateColor()
        {
            //Unity UGUI是通过顶点颜色 * CanvasRender对应的颜色 * 贴图颜色得到最终的颜色
            //该类的Mesh顶点可能较多，而且目前所有顶点的颜色都是一样的，所以在Shader中额外增加一个MeshColor,代表顶点颜色
            material.SetColor("_MeshColor", color);
        }

        //当前是否处于正常状态（没有任何卷曲的状态），如果处于这个状态，那么就不要生成过于复杂的网格了
        //是否需要不断地计算当前纸面的状态，然后选择尽可能少的网格顶点数量？这能减少正常情况下的渲染压力，但是会增加CPU的计算量，并且可能会不会生成新的Mesh
        //当界面大部分时候处于“稳定”状态（非卷曲状态）时，更应该开启检测，当纸面不断地在两个状态间变化时，则更应该关闭检测
        [System.NonSerialized]
        private bool enableSimpleMode = true;
        public bool EnableSimpleMode { get { return enableSimpleMode; } set { enableSimpleMode = value; } }
        private bool IsSimpleMode()
        {
            if (!EnableSimpleMode)
            {
                return false;
            }
            bool isSimpleMode = Mathf.Approximately(rollAngle, 0) || Mathf.Approximately(radius, 0);
            if (!isSimpleMode)
            {
                Vector2 normal = Quaternion.AngleAxis(directionAngle, -Vector3.forward) * Vector2.right;
                float maxDistance = 1 / Mathf.Max(Mathf.Abs(Mathf.Cos(directionAngle * Mathf.Deg2Rad)), Mathf.Abs(Mathf.Sin(directionAngle * Mathf.Deg2Rad)));
                float currentDistance = Vector2.Dot(new Vector2(normalX - 0.5f, normalY - 0.5f), normal);
                if (maxDistance - currentDistance < 0.01f)
                {
                    isSimpleMode = true;
                }
            }
            return isSimpleMode;
        }

        [System.NonSerialized]
        private bool currentSimpleMode = true;
        //判断当前是否需要刷新Mesh和Material
        [System.NonSerialized]
        private Texture lastBackImage = null;
        [System.NonSerialized]
        private float lastRadius = -1;
        [System.NonSerialized]
        private float lastRollAngle = -1;
        [System.NonSerialized]
        private float lastNormalX = -1;
        [System.NonSerialized]
        private float lastNormalY = -1;
        [System.NonSerialized]
        private float lastDirectionAngle = -1;
        [System.NonSerialized]
        private Vector3 lastPosition = new Vector3(-1, -1, -1);
        [System.NonSerialized]
        private float lastCircleOffset = float.MinValue;

        private void CheckUpdateMaterial()
        {
            if (IsDefaultMaterial)
            {
                return;
            }
            //更新Material相关数值的消耗并不大，所以就算每次全部刷新也无所谓，不过这里暂时还是多做一些判断
            if (lastBackImage != BackImage)
            {
                UpdateTextures();
                lastBackImage = backImage;
            }
            bool needUpdatePoint = SetPropertyUtility.SetStruct<float>(ref lastNormalX, normalX);
            needUpdatePoint |= SetPropertyUtility.SetStruct<float>(ref lastNormalY, normalY);
            needUpdatePoint |= SetPropertyUtility.SetStruct<float>(ref lastDirectionAngle, directionAngle);

            if (needUpdatePoint)
            {
                UpdateRollStartPoint();
            }

            bool needUpdateRollData = SetPropertyUtility.SetStruct<float>(ref lastRadius, radius);
            needUpdateRollData |= SetPropertyUtility.SetStruct<float>(ref lastRollAngle, rollAngle);
            if (needUpdateRollData)
            {
                UpdateRollData();
            }

            if (SetPropertyUtility.SetStruct<float>(ref lastCircleOffset, circleOffset))
            {
                UpdateCircleOffset();
            }

            UpdateColor();
        }
        [System.NonSerialized]
        private Rect lastRect = new Rect(-1, -1, -1, -1);
        private void CheckUpdateMesh()
        {
            if (SetPropertyUtility.SetStruct<Rect>(ref lastRect, rectTransform.rect))
            {
                //currentSimpleMode此时也刷新一下，避免下一帧再刷新
                currentSimpleMode = IsSimpleMode();
                UpdateMeshData();
                UpdateRectSize();
            }
            else if (SetPropertyUtility.SetStruct<bool>(ref currentSimpleMode, IsSimpleMode()))
            {
                UpdateMeshData();
            }
        }

#if UNITY_EDITOR
        //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
        protected override void OnValidate()
        {
            base.OnValidate();
            //SetDefaultValue();;
            
        }

        //Reset is called when the user hits the Reset button in the Inspector's context menu or when adding the component the first time. 
        //This function is only called in editor mode. Reset is most commonly used to give good default values in the inspector.
        protected override void Reset()
        {
            base.Reset();
            SetDefaultValue();
        }

#endif
        private void SetDefaultValue()
        {
            currentMesh = null;
            currentSimpleMode = true;
            lastRect = new Rect(-1, -1, -1, -1);
            lastBackImage = null;
            lastRadius = -1;
            lastRollAngle = -1;
            lastNormalX = -1;
            lastNormalY = -1;
            lastDirectionAngle = -1;
            lastPosition = new Vector3(-1, -1, -1);
            lastCircleOffset = float.MinValue;
        }
    }
}

