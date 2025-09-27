using UnityEngine;

public static class VectorUtil
{
    /// <summary>
    /// 向量方向枚举，Unity坐标系，以z轴为正方向，y轴上方向
    /// </summary>
    public enum VectorDirEnum
    {
        ZX,      // 当前向量在ZX平面上
        ZY,      // 当前向量在ZY平面上
        XY,      // 当前向量向XY平面上
    }

    /// <summary>
    /// 求当前向量在某个平面上的投影向量
    /// </summary>
    /// <param name="vectorValue"></param>
    /// <param name="plane"></param>
    public static Vector3 GetVerticalVector(Vector3 vectorValue, VectorDirEnum vectorDir)
    {
        switch (vectorDir)
        {
            case VectorDirEnum.ZY:
                return Vector3.Cross(vectorValue, Vector3.ProjectOnPlane(vectorValue, Vector3.right));
            case VectorDirEnum.XY:
                return Vector3.Cross(vectorValue, Vector3.ProjectOnPlane(vectorValue, Vector3.up));
            case VectorDirEnum.ZX:
                return Vector3.Cross(vectorValue, Vector3.ProjectOnPlane(vectorValue, Vector3.up));
        }

        return Vector3.zero;
    }
}
