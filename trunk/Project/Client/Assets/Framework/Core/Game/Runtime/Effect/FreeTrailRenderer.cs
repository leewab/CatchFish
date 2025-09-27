using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FreeTrailRenderer : MonoBehaviour
{
	public struct Matrix_Time
	{
		public Matrix4x4 matrix;
		//public Quaternion rot;
		//public Vector3 tran;
		//public Vector3 scale;
		public float t;

		//public static bool operator ==(Matrix_Time lhs, Matrix_Time rhs)
		//{
		//	//return lhs.rot == rhs.rot && lhs.tran == rhs.tran && lhs.scale == rhs.scale;
		//	return lhs.matrix == rhs.matrix;
		//}

		//public static bool operator !=(Matrix_Time lhs, Matrix_Time rhs)
		//{
		//	return ! (lhs == rhs);
		//}
	}
	//记录一定时间或长度内的变换数据//
	Queue<Matrix_Time> mRec = new Queue<Matrix_Time>();
    public Queue<Matrix_Time> Rec
    {
        get { return mRec; }
    }
	public float Duration = 1f;//
	//public float LenLimit = 1f;
	public float BeginScale = 0.3f;
	public float EndScale = 0.1f;
	public float BeginColorFade = 1f;
	public float EndColorFade = 0f;
	public float BeginVShift = 0f;
	public float EndVShift = 0f;
	public float BeginVShiftSpeed = 0f;
	public float EndVShiftSpeed = 0f;
	public float BeginVShiftMin = 0f;
	public float EndVShiftMin = 0f;
	public float BeginVShiftMax = 0.5f;
    public float EndVShiftMax = 0.5f;

    public bool IntervalIsDist = true;
    public float RecInterval = 0.1f;
    //public bool RecLimit = false;
	public float RecTimeLimit = 0f;
	float RecBeginTime = 0f;
    float FadeTime = 0.5f;

	class MeshTemplate
	{
		public Vector3[] mVertexTemplate = null;
		public Vector2[] mUVTemplate = null;
		public Color[] mColorTemplate = null;
		public int[] mLineTemplate = null;
	};
	static MeshTemplate[] mTemplates = null;
	public enum TmplType
	{
		Cross = 0,
        Horizon,
        Vertical,
    }
    public TmplType TemplateType = TmplType.Horizon;

	Mesh mMesh = null;

	static void BuildCrossTemplate()
	{
		if (null != mTemplates)
			return;

		mTemplates = new MeshTemplate[3];

		MeshTemplate CrossTemplate = new MeshTemplate();
        MeshTemplate HorizonTemplate = new MeshTemplate();
        MeshTemplate VerticalTemplate = new MeshTemplate();
        mTemplates[0] = CrossTemplate;
        mTemplates[1] = HorizonTemplate;
        mTemplates[2] = VerticalTemplate;
        //十字模板//
		//Mesh mesh = new Mesh();
		Vector3[] verts = new Vector3[4];
		verts[0] = new Vector3(0, -1, 0);
		verts[1] = new Vector3(0, 1, 0);
		verts[2] = new Vector3(-1, 0, 0);
		verts[3] = new Vector3(1, 0, 0);
		CrossTemplate.mVertexTemplate = verts;
        HorizonTemplate.mVertexTemplate = verts;
        VerticalTemplate.mVertexTemplate = verts;

		Vector2[] uvs = new Vector2[4];
		uvs[0] = new Vector2(0, 0);
		uvs[1] = new Vector2(0, 1);
		uvs[2] = new Vector2(0, 0);
		uvs[3] = new Vector2(0, 1);
		CrossTemplate.mUVTemplate = uvs;
        HorizonTemplate.mUVTemplate = uvs;
        VerticalTemplate.mUVTemplate = uvs;

		Color[] colors = new Color[4];
		colors[0] = Color.white;
		colors[1] = Color.white;
		colors[2] = Color.white;
		colors[3] = Color.white;
		CrossTemplate.mColorTemplate = colors;
		HorizonTemplate.mColorTemplate = colors;
        VerticalTemplate.mColorTemplate = colors;

		{
			int[] lines = new int[4];
			lines[0] = 0;
			lines[1] = 1;
			lines[2] = 2;
			lines[3] = 3;
			CrossTemplate.mLineTemplate = lines;
		}
        {
            int[] lines = new int[2];
            lines[0] = 2;
            lines[1] = 3;
            HorizonTemplate.mLineTemplate = lines;
        }
        {
            int[] lines = new int[2];
            lines[0] = 0;
            lines[1] = 1;
            VerticalTemplate.mLineTemplate = lines;
        }
    }

	// Use this for initialization
	void Start () {
		mMesh = new Mesh();
		GetComponent<MeshFilter>().sharedMesh = mMesh;

		BuildCrossTemplate();
	}
	
	// Update is called once per frame
	List<Vector3> verts = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<Color> colors = new List<Color>();
	List<int> tris = new List<int>();
    float lastRecTime = 0f;
    Vector3 lastRecPos;
    Matrix_Time curMT;

    void AddMT(float lerpactor, float scale, float fade, ref MeshTemplate SelTmpl, ref Matrix4x4 w2l, Matrix_Time rts, float t)
    {
        int idxbase = verts.Count;
        for (int i = 0, jmax = SelTmpl.mVertexTemplate.Length; i < jmax; ++i)
        {
            verts.Add(w2l.MultiplyPoint(rts.matrix.MultiplyPoint(SelTmpl.mVertexTemplate[i] * scale)));

            Vector2 uvnew = SelTmpl.mUVTemplate[i];
            //uvnew.x = 0 == uvnew.x ? lerpactor : lerpactor2;
            uvnew.x = lerpactor;
            float vshiftbegin = Mathf.Clamp(BeginVShift + BeginVShiftSpeed * t, BeginVShiftMin, BeginVShiftMax);
            float vshiftend = Mathf.Clamp(EndVShift + EndVShiftSpeed * t, EndVShiftMin, EndVShiftMax);
            uvnew.y += Mathf.Lerp(vshiftend, vshiftbegin, lerpactor);
            uvs.Add(uvnew);

            colors.Add(SelTmpl.mColorTemplate[i] * fade);
        }
        if (idxbase > 0)
        {
            int idxbaseLast = idxbase - SelTmpl.mVertexTemplate.Length;
            for (int i = 0; i < SelTmpl.mLineTemplate.Length; i += 2)
            {
                tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i]);
                tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i + 1]);
                tris.Add(idxbase + SelTmpl.mLineTemplate[i]);
                tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i + 1]);
                tris.Add(idxbase + SelTmpl.mLineTemplate[i + 1]);
                tris.Add(idxbase + SelTmpl.mLineTemplate[i]);
            }
        }
    }
    void LateUpdate()
	{
		BuildCrossTemplate();

		float t = Time.time;
		Transform tran = transform;

        //判断记录间隔//
        bool recFrame = false;
        if (IntervalIsDist)
        {
            //距离模式//
            if (0f == RecInterval)
            {
                recFrame = true;
            }
            else
            {
                if (Vector3.Distance(lastRecPos, tran.position) > RecInterval)
                {
                    recFrame = true;
                }
            }
        }
        else
        {
            //时间模式//
            if (0f == RecInterval)
            {
                recFrame = true;
            }
            else
            {
                if (t - lastRecTime > RecInterval)
                {
                    recFrame = true;
                }
            }
        }
        if (recFrame)
        {
            lastRecTime = t;
            lastRecPos = tran.position;
        }

        //Rec
		curMT.matrix = tran.localToWorldMatrix;
		//cur.matrix = tran.worldToLocalMatrix;
		curMT.t = t;
        if (recFrame && (RecTimeLimit == 0 || RecTimeLimit > t - RecBeginTime))
		{
            //符合条件时添加//
			mRec.Enqueue(curMT);

			while (mRec.Count > 0 && mRec.Peek().t + Duration < t)
			{
				mRec.Dequeue();
			}

            //lastQ = tran.rotation;
        }

        //更新顶点//
		int TemplateIndex = (int)TemplateType;
		MeshTemplate SelTmpl = mTemplates[TemplateIndex];
        Matrix4x4 w2l = tran.worldToLocalMatrix;
		verts.Clear();
		uvs.Clear();
		colors.Clear();
		tris.Clear();
		float idx=0f, imax=mRec.Count;
        float timeFadeActor = (t - lastRecTime)/FadeTime;
		foreach (Matrix_Time rts in mRec)
		{
            float lerpactor = Mathf.Clamp01( idx / imax - timeFadeActor );
			//float lerpactor2 = (idx+1) / imax;
			float scale = Mathf.Lerp(EndScale, BeginScale, lerpactor);
			float fade = Mathf.Lerp(EndColorFade, BeginColorFade, lerpactor);
			++idx;
            AddMT(lerpactor, scale, fade, ref SelTmpl, ref w2l, rts, t);
            //int idxbase = verts.Count;
            //for (int i = 0, jmax = SelTmpl.mVertexTemplate.Length; i < jmax; ++i)
            //{
            //    verts.Add(w2l.MultiplyPoint(rts.matrix.MultiplyPoint(SelTmpl.mVertexTemplate[i] * scale)));

            //    Vector2 uvnew = SelTmpl.mUVTemplate[i];
            //    //uvnew.x = 0 == uvnew.x ? lerpactor : lerpactor2;
            //    uvnew.x = lerpactor;
            //    float vshiftbegin = Mathf.Clamp( BeginVShift + BeginVShiftSpeed * t, BeginVShiftMin, BeginVShiftMax);
            //    float vshiftend = Mathf.Clamp(EndVShift + EndVShiftSpeed * t, EndVShiftMin, EndVShiftMax);
            //    uvnew.y += Mathf.Lerp(vshiftend, vshiftbegin, lerpactor);
            //    uvs.Add(uvnew);

            //    colors.Add(SelTmpl.mColorTemplate[i] * fade);
            //}
            //if (idxbase > 0)
            //{
            //    int idxbaseLast = idxbase - SelTmpl.mVertexTemplate.Length;
            //    for (int i = 0; i < SelTmpl.mLineTemplate.Length; i += 2)
            //    {
            //        tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i]);
            //        tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i + 1]);
            //        tris.Add(idxbase + SelTmpl.mLineTemplate[i]);
            //        tris.Add(idxbaseLast + SelTmpl.mLineTemplate[i + 1]);
            //        tris.Add(idxbase + SelTmpl.mLineTemplate[i + 1]);
            //        tris.Add(idxbase + SelTmpl.mLineTemplate[i]);
            //    }
            //}
		}
        //最后一个点，头部//
        {
            float lerpactor = Mathf.Clamp01(1 - timeFadeActor);
            float scale = Mathf.Lerp(EndScale, BeginScale, lerpactor);
            float fade = Mathf.Lerp(EndColorFade, BeginColorFade, lerpactor);
            AddMT(lerpactor, scale, fade, ref SelTmpl, ref w2l, curMT, t);
        }
        mMesh = GetComponent<MeshFilter>().sharedMesh;
		mMesh.Clear(true);
		mMesh.vertices = verts.ToArray();
		mMesh.uv = uvs.ToArray();
		mMesh.colors = colors.ToArray();
		mMesh.triangles = tris.ToArray();
		GetComponent<MeshFilter>().sharedMesh = mMesh;
	}
}
