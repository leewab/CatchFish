
using UnityEngine;

public class Bezier
{
    private Vector3 mP0;
    private Vector3 mP1;
    private Vector3 mP2;
    private float mTime;

    public Bezier()
    {

    }

    public Bezier(Vector3 p0, Vector3 p1, float time)
    {
        this.mP0 = p0;
        this.mP1 = p1;
        this.mTime = time;
    }

    public Bezier(Vector3 p0, Vector3 p1, Vector3 p2)
    {
        this.mP0 = p0;
        this.mP1 = p1;
        this.mP2 = p2;
    }

    /// <summary>
    /// 更新线性Bezier曲线数据
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="time"></param>
    public void UpdateLinearBezier(Vector3 p0, Vector3 p1, float time)
    {
        this.mP0 = p0;
        this.mP1 = p1;
        this.mTime = time;
    }

    /// <summary>
    /// B(t) = P0+(P1-P0)t = (1-t)P0 + tP1, t∈[0,1]
    /// 计算线性Bezier曲线
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public Vector3 CalculateLinearBezierPos(float curTime)
    {
        if (this.mTime == 0)
        {
            return this.mP0;
        }
        else
        {
            var t = curTime / this.mTime;
            return (1 - t) * this.mP0 + t * this.mP1;
        }

    }

    /// <summary>
    /// 更新儿子发Bezier曲线的数据
    /// </summary>
    /// <param name="p0"></param>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    public void UpdateQuadraticBezier(Vector3 p0, Vector3 p1, Vector3 p2, float time)
    {
        this.mP0 = p0;
        this.mP1 = p1;
        this.mP2 = p2;
        this.mTime = time;
    }

    /// <summary>
    /// B(t) = (1-t)^2*P0 + 2t(1-t)*P1 + t^2*P2, t∈[0,1]
    /// 计算二次方Bezier曲线
    /// </summary>
    /// <param name="curTime"></param>
    /// <returns></returns>
    public Vector3 CalculateQuadraticBezierPos(float curTime)
    {
        if (this.mTime == 0)
        {
            return this.mP0;
        }
        else
        {
            var t = curTime / this.mTime;
            return (1-t)*(1-t)*this.mP0 + 2*t*(1-t)*this.mP1 + t*t*this.mP2;
        }
    }
}
