using UnityEngine;
using UnityEditor;

namespace MUGUI
{
    [CustomEditor(typeof(ImageFont), true)]
    public class ImageFontEditor : Editor
    {
        private ImageFont imageFont;
        private GUIStyle gs;
        private void displayone(int index, string title)
        {
            gs.fontSize = 25;
            gs.normal.textColor = new Color(0.3f, 0.9f, 0.8f);
            GUILayout.Label(title, gs);
            EditorGUILayout.ObjectField(imageFont._EmojiTexture[index], typeof(Texture), true);

            gs.fontSize = 15;
            gs.normal.textColor = new Color(0.8f, 0.9f, 0.7f);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", gs, GUILayout.Width(120));
            GUILayout.Label("id", gs, GUILayout.Width(60));
            GUILayout.Label("帧数", gs, GUILayout.Width(50));
            GUILayout.EndHorizontal();

            //一开始就聚合增加了显示的难度。
            EmojiInfo[] ei = imageFont.items[index]._emojiinfo;

            for (int i = 0; i < ei.Length; i++)
            {
                EmojiInfo info = ei[i];
                if (info == null||info.frameCount==0)
                    continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label(info.name, GUILayout.Width(120));
                GUILayout.Label(info.key.ToString(), GUILayout.Width(50));
                GUILayout.Label(info.frameCount.ToString(), GUILayout.Width(50));
                GUILayout.EndHorizontal();
            }
            gs.normal.textColor = new Color(0.8f, 0.2f, 0.8f);
            GUILayout.Label("_____________________________________________", gs);
        }
  		public override void OnInspectorGUI()
        {
            //给出一个比较合适的可视化方法。
            imageFont = target as ImageFont;
            gs = new GUIStyle();
            gs.normal.textColor = new Color(0.7f, 0.9f, 0.9f);
            gs.fontSize = 15;
            if (imageFont.items == null || imageFont.items.Length != imageFont.NUM_EMOJI_SET+1)
            {
                GUILayout.Label("输入图像必须是可读的且尺寸不要缩放成2的n次方", gs);
                GUILayout.Label("没有任何表情，请使用菜单EmojiText/BuildEmoji重新构建。", gs);
                GUILayout.Label("仍要确保ImageFont绑定在AdvancedText上", gs);
                GUILayout.Label("无需给AdvancedText绑定LazyImageLoader", gs);
                return;
            }
            

            //分别对每一个组进行展示，主要目标是查询id.
            displayone( 0, "公用表情");
            for (int i = 0; i < imageFont.NUM_EMOJI_SET; i++)
                displayone(i+1, "表情集" + i);
            //AdvancedTextData没有任何可以Inspector中修改的属性。
        }
    }
}