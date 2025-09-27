using UnityEngine;

namespace MUGUI
{
    public class AnimatedSprite : MonoBehaviour
    {
        [SerializeField]
        int fps = 25;
        [SerializeField]
        Sprite[] sprites;
        float tpf;
        int idx;
        float accumulatedTime;
        public int FPS { get { return fps; } set { fps = value; } }

        public Sprite[] Sprites { get { return sprites; } set { sprites = value; } }

        private SpriteRenderer render;
        void OnEnable()
        {
            render = this.GetComponent<SpriteRenderer>();
            tpf = 1f / fps;
            ResetData();
        }

        void Update()
        {
            accumulatedTime += Time.deltaTime;
            while (accumulatedTime >= tpf)
            {
                idx++;
                accumulatedTime -= tpf;
            }

            if (sprites == null || sprites.Length <= 0)
                return;
            idx = idx % sprites.Length;

            render.sprite = sprites[idx];
        }

        public void ResetData()
        {
            idx = 0;
            accumulatedTime = 0;
        }
    }
}
