using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MUGUI
{
    
    /// <summary>
    /// 序列帧组建
    /// </summary>
    [AddComponentMenu("UI/Animated Image", 11)]
    [ExecuteInEditMode]
    public class AnimatedImage : Image
    {
        [SerializeField]
        int fps = 25;
        [SerializeField]
        Sprite[] sprites;

        public enum AnimType
        {
            loop = 1,
            once = 2,
        }
        public string[] animationNames;
        float tpf;
        int idx;
        float accumulatedTime;
        bool isSet = false;
        public int FPS { get { return fps; } set { fps = value; } }
        public AnimType mType = AnimType.loop;

        public Sprite[] Sprites { get { return sprites; } set { sprites = value; } }

        protected override void OnEnable()
        {
            base.OnEnable();
            raycastTarget = false;
            tpf = 1f / fps;
            ResetData();
        }

        protected override void Start()
        {
            base.Start();
            if (sprites == null || sprites.Length <= 0)
            {
                return;
            }
            //第一次加载未完成，这块会出现白板
            if (sprites[0] != null)
            {
                sprite = sprites[0];
                //UIAtlasManager.GetInstance().SetAtlasMaterial(this, sprites[0].name);
            }
        }

        void Update()
        {
            if (mType == AnimType.loop)
            {
                accumulatedTime += Time.deltaTime;
                while (accumulatedTime >= tpf)
                {
                    idx++;
                    accumulatedTime -= tpf;
                    isSet = false;
                }

                if (sprites == null || sprites.Length <= 0)
                    return;
                idx = idx % sprites.Length;

                
            }else if (mType == AnimType.once)
            {
                if (sprites == null || sprites.Length <= 0)
                    return;
                if (idx >= sprites.Length - 1)
                {
                    return;
                }
                accumulatedTime += Time.deltaTime;
                while (accumulatedTime >= tpf)
                {
                    idx++;
                    accumulatedTime -= tpf;
                    isSet = false;
                }
                if (idx >= sprites.Length)
                    idx = sprites.Length - 1;
            }

            if (isSet == false)
            {
                if (idx < sprites.Length)
                {
                    //第一次加载未完成，这块会出现白板
                    if (sprites[idx] != null)
                    {
                        if (UnityStandardAssets.ImageEffects.DepthOfField.ISDOF)
                        {
                            if (gameObject.name == "UI_WriteDepth")
                            {
                                sprite = sprites[idx];
                            }
                        }
                        else
                        {
                            sprite = sprites[idx];
                        }
                        //UIAtlasManager.GetInstance().SetAtlasMaterial(this, sprites[idx].name);
                    }
                    isSet = true;
                }
            }
        }

        public void ResetData()
        {
            idx = 0;
            accumulatedTime = 0;
            if (sprites[0] != null)
            {
                sprite = sprites[0];
            }
        }

    }
}