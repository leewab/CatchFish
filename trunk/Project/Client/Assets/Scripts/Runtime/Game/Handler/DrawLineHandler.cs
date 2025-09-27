using Framework.Core;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawLineHandler : MonoBehaviour
{
    [SerializeField] private AnimationCurve ac;
    [SerializeField] private LineRenderer lr;

    public void Init(int posCount, int numCapVertices)
    {
        if(lr == null) lr = GetComponent<LineRenderer>();
        lr.positionCount = posCount;
        lr.widthCurve = ac;
        lr.numCapVertices = numCapVertices;
        HideDraw();
    }

    public void ShowDraw()
    {
        lr.enabled = true;
    }

    public void HideDraw()
    {
        lr.enabled = false;
    }

    public void SetPosition(int index, Vector3 pos)
    {
        lr.SetPosition(index, pos);
    }

    public void SetPositions(Vector3[] poses)
    {
        lr.SetPositions(poses);
    }
}
