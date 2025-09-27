using UnityEngine;

namespace Game.UI
{
    public class GameCommonListPreview : MonoBehaviour
    {
        [SerializeField]
        public int typeid;
        [SerializeField]
        public int count = 1;
        [SerializeField]
        public int lenth;
        [SerializeField]
        public int borderX;
        [SerializeField]
        public int borderY;
        [SerializeField]
        public bool IsVertical = true;

        private GameCommonList gcl;
        private float timestamp = 0;
        void refresh()
        {
            //根据配置的不同重建表格。
            if (Application.isPlaying)
            {
                //要注意，在重建之前必须将它们遗留的GameObject删除。
                var child = GetComponentsInChildren<Transform>();
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    var cur = transform.GetChild(i);
                    if (!cur.name.Equals(GameCommonList.TEMPLATE))
                        Destroy(cur.gameObject);
                }
                gcl = new GameCommonList(gameObject);
                // gcl.Init(gameObject);
                gcl.SetOneRowOrColCount(count);
                gcl.SetBorderSizeX(borderX);
                gcl.SetBorderSizeY(borderY);
                gcl.SetIsVertical(IsVertical);
                for (int i = 0; i < lenth; i++)
                    gcl.AddItem(typeid);
            }
        }
        public void OnValidate()
        {
            if (typeid < 0) typeid = 0;
            if (count < 1) count = 1;
            if (borderX < 0) borderX = 0;
            if (borderY < 0) borderY = 0;
            if (lenth < 0) lenth = 0;
            refresh();
        }
        void Update()
        {
            if (gcl != null)
            {
                float delta = Time.realtimeSinceStartup - timestamp;
                if (delta > 0.04)
                {
                    timestamp = Time.realtimeSinceStartup;
                    gcl.Update();
                }
            }
        }
        
    }
}
