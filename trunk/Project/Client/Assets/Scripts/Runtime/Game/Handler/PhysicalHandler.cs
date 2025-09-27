using UnityEngine;

namespace SSC.Game.Handler
{
    public class PhysicalHandler
    {
        /// <summary>
        /// 计算半斤为radius的圆外给一个点 对应到圆上的一个点
        /// </summary>
        /// <param name="circleCenterPos"></param>
        /// <param name="curVector"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vector3 CalculateVectorWithRatio(Vector3 circleCenterPos, Vector3 curVector, float radius)
        {
            var distance = Vector2.Distance(new Vector2(circleCenterPos.x, circleCenterPos.z), new Vector2(curVector.x, curVector.z));
            if (radius >= distance) return curVector;
            float x = radius * curVector.x / distance;
            float y = curVector.y;
            float z = radius * curVector.z / distance;
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float CalculateVectorMiddlePos(Vector3 A, Vector3 B, Vector3 C, float b2)
        {
            float ab = Vector3.Distance(A, B);
            float bc = Vector3.Distance(B, C);
            float cos_b2 = Mathf.Cos(b2);
            float sin_b2 = Mathf.Sin(b2);
            return bc * ab / (bc * cos_b2 + ab * sin_b2);
        }

    }
}