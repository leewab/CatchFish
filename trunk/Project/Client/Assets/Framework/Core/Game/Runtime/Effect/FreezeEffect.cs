using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FreezeEffect : MonoBehaviour {

	public Material mat;
	float myTime = 0.0f;
	[Range(0.0f, 1.0f)]
	public float FrameAlpha;
	[Range(0.0f, 1.0f)]
	public float CutOut;

	void Start () 
	{

	}


	void Update () 
	{

	}

	void OnRenderImage (RenderTexture src, RenderTexture dest)
	{
		//mat.SetTexture("_MainTex", src);
		//mat.SetTexture("_ScreenWaterDropTex", ScreenWaterDropTex);  
		myTime += Time.deltaTime;

		mat.SetFloat ("_Level", 1 - myTime * 0.2f);
		mat.SetFloat ("_FrameAlpha", FrameAlpha);
		mat.SetFloat ("_CutOut", CutOut);
		Debug.Log (myTime);
		Graphics.Blit (src, dest, mat);
	}



}
	