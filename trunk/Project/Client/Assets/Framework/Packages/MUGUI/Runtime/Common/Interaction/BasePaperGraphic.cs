using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    //PaperGraphic的基类，所以继承该类的类都能作为PaperGraphicTarget的目标，并可以设置FrondImage和BackImage
    //另外，该类封装一些子类可能会需要的通用方法（比如说：按照密度生成Mesh）
    //该类配合PaperGraphicTarget可以实现很多UI整体的效果
    public class BasePaperGraphic : MaskableGraphic
    {
        //正面图像
        [SerializeField]
        private Texture frontImage;

        //提供给程序设置和获取相关属性的接口，并且设置相关数据失效
        //SerializedField被动画系统直接修改时，内部会设置所有数据失效（通过基类中的OnDidApplyAnimationProperties方法）
        public Texture FrontImage { get { return frontImage; } set { if (SetPropertyUtility.SetClass<Texture>(ref frontImage, value)) SetMaterialDirty(); } }
        public override Texture mainTexture
        {
            get
            {
                return frontImage;
            }
        }

        //外部将该组件作RenderTexture的载体时，可以设置对应的摄像机
        public virtual void SetRenderCamera(Camera renderCamera)
        {

        }
        public virtual void OnEnterPaperState()
        {

        }
        public virtual void OnExitPaperState()
        {

        }

        //留给子类用的接口
        protected Material currentMaterial
        {
            get
            {
                Material mat = canvasRenderer.GetMaterial(0);
                if(mat == null)
                {
                    mat = material;
                }
                return mat;
            }
        }
        protected bool IsDefaultMaterial
        {
            get { return material == defaultMaterial; }
        }
        protected Mesh GenerateMesh(float meshDenity)
        {
            var rect = GetPixelAdjustedRect();
            var meshWidth = rect.width;
            var meshHeight = rect.height;
            if (meshWidth <= 0 || meshHeight <= 0)
            {
                return new Mesh();
            }
            int columnLineCount = Mathf.CeilToInt(meshWidth * meshDenity) + 1;
            float horizontalStep = 1 / meshDenity;
            int rowLineCount = Mathf.CeilToInt(meshHeight * meshDenity) + 1;
            float verticalStep = 1 / meshDenity;
            //最左上角的Mesh的模型坐标系下的X轴坐标值和Z轴坐标值（按照Unity的习惯，Y值将表示高度）
            float startX = rect.x;
            float startY = rect.y;
            //float startX = -meshWidth / 2;
            //float startY = -meshHeight / 2;
            //按照从下往上，从左往右的顺序确定每一个顶点的坐标
            //确定UV坐标，与Unity习惯保持一致，左下角顶点UV坐标为（0,0），右上角顶点UV坐标为（1,1）
            Vector3[] vertices = new Vector3[columnLineCount * rowLineCount];
            Vector2[] uvs = new Vector2[columnLineCount * rowLineCount];
            for (int i = 0; i < rowLineCount; i++)
            {
                for (int j = 0; j < columnLineCount; j++)
                {
                    float x = j == columnLineCount - 1 ? startX + meshWidth : startX + j * horizontalStep;
                    float y = i == rowLineCount - 1 ? startY + meshHeight : startY + i * verticalStep;
                    float z = 0;
                    float u = Mathf.Abs(x - startX) / meshWidth;
                    float v = Mathf.Abs(y - startY) / meshHeight;

                    //设置对应顶点的位置和UV坐标，行数从下到上增长，列数从左到右增长
                    vertices[i * columnLineCount + j] = new Vector3(x, y, z);
                    uvs[i * columnLineCount + j] = new Vector2(u, v);
                }
            }
            //生成索引数组,每个“小方格”都由两个三角形组成
            int[] indices = new int[(columnLineCount - 1) * (rowLineCount - 1) * 6];
            int currentIndex = 0;
            for (int i = 0; i < rowLineCount - 1; i++)
            {
                for (int j = 0; j < columnLineCount - 1; j++)
                {
                    //为第i行，第j列的小方格确定它对应的顶点序列
                    //第一个三角形，左下角的三角形顶点，按照顺时针方向排列（Unity似乎是以顺时针的三个顶点作为一个正面的三角形）
                    indices[currentIndex++] = i * columnLineCount + j;
                    indices[currentIndex++] = (i + 1) * columnLineCount + j;
                    indices[currentIndex++] = i * columnLineCount + j + 1;
                    //第二个三角形，右下角的三角形顶点，按照顺时针方向排列
                    indices[currentIndex++] = i * columnLineCount + j + 1;
                    indices[currentIndex++] = (i + 1) * columnLineCount + j;
                    indices[currentIndex++] = (i + 1) * columnLineCount + j + 1;
                }
            }
            //生成最终的Mesh
            workerMesh.Clear();
            workerMesh.vertices = vertices;
            workerMesh.uv = uvs;
            workerMesh.triangles = indices;
            workerMesh.RecalculateNormals();
            workerMesh.RecalculateBounds();
            return workerMesh;
        }
        private VertexHelper m_vertexHelper = new VertexHelper();
        protected Mesh GenerateSimpleMesh()
        {
            base.OnPopulateMesh(m_vertexHelper);
            m_vertexHelper.FillMesh(workerMesh);
            return workerMesh;
        }
    }

    //复制UnityEngine.UI 中的代码
    public static class SetPropertyUtility
    {
        public static bool SetColor(ref Color currentValue, Color newValue)
        {
            if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (EqualityComparer<T>.Default.Equals(currentValue, newValue))
                return false;

            currentValue = newValue;
            return true;
        }

        public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
        {
            if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
                return false;

            currentValue = newValue;
            return true;
        }
    }
}

