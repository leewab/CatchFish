using UnityEngine;  
using System.Collections;  
using System.Collections.Generic;
//粒子特效适配模型
public class ParticleScale : MonoBehaviour
{
    //体积信息
    private List<float> volumeData = null;
    //粒子列表
    private ParticleSystem[] particles = null;
    private ParticleSystem.ShapeModule[] shapes = null;
    private bool hasInit = false;

    void Awake()
    {
        // Save off all the initial scale values at start.  
        CheckInit();

        if(volumeData!= null && volumeData.Count==3)
        {
            SetModelVolume(volumeData[0], volumeData[1], volumeData[2]);
            volumeData.Clear();
            volumeData = null;
        }
    }

    private void CheckInit()
    {
        if (hasInit) {
            return;
        }
        particles = gameObject.GetComponentsInChildren<ParticleSystem>();
        if (particles != null)
        {
            shapes = new ParticleSystem.ShapeModule[particles.Length];
            for (int i = 0; i < particles.Length; i++)
            {
                shapes[i] = particles[i].shape;
            }
        }
        hasInit = true;
    }

    public void SetModelVolume(float x,float y,float z)
    {
        if (hasInit == false)
        {
            volumeData = new List<float>();
            volumeData.Add(x);
            volumeData.Add(y);
            volumeData.Add(z);
            return;
        }
        for(int i=0;i<particles.Length;i++)
        {
            shapes[i].enabled = true;
            //shapes[i].boxThickness = new Vector3(0, 0, 0);
            shapes[i].shapeType = ParticleSystemShapeType.BoxShell;
            shapes[i].scale = new Vector3(x, y, z);
            shapes[i].position = new Vector3(0, 0, z / 2);

        }
    }
}  