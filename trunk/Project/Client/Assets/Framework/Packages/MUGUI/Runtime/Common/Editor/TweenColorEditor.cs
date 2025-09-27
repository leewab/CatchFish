using UnityEngine;
using UnityEditor;

namespace MUGUI
{
    [CustomEditor(typeof(TweenColor))]
    public class TweenColorEditor : UITweenerEditor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            SUGUIEditorTools.SetLabelWidth(120f);

            TweenColor tw = target as TweenColor;
            GUI.changed = false;

            Color from = EditorGUILayout.ColorField("From", tw.from);
            Color to = EditorGUILayout.ColorField("To", tw.to);

            if (GUI.changed)
            {
                SUGUIEditorTools.RegisterUndo("Tween Change", tw);
                tw.from = from;
                tw.to = to;
                GOGUITools.SetDirty(tw);
            }

            DrawCommonProperties();
        }
    }
}