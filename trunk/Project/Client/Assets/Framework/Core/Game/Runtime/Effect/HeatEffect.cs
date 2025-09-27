using UnityEngine;
using System.Collections;

public class HeatEffect : MonoBehaviour
{
    private Shader shader = null;
	[Range(0.0f, 1.0f)]
    public float Refraction = 0.2f;
	[Range(-1.0f, 1.0f)]
	public float Test = 0.001f;
	[Range(1.0f, 30.0f)] 
	public float SpeedY = 25.6f;
	[Range(0.0f, 1.0f)] 
	public float CutOut = 0.3f;
	[Range(0.0f, 1.0f)]
	public float FrameAlpha;

    //private Material mat = null;
	public Material mat;
    void Start()
    {


    }
		
    void Update()
    {

    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mat == null)
            Graphics.Blit(source, destination);
        // mat.SetTexture("_RefractionTex", source);
        Refraction = Mathf.Clamp(Refraction, 0, 1.0f);
        mat.SetFloat("_Refraction", Refraction);
		mat.SetFloat ("_TestSlider", Test);
		mat.SetFloat ("_SpeedY", SpeedY);
		mat.SetFloat ("_CutOut", CutOut);
		mat.SetFloat ("_FrameAlpha", FrameAlpha);

        Graphics.Blit(source, destination , mat);
  //      cam.targetTexture = source;
    }
}
