using UnityEngine;
using UnityEditor;

namespace MUGUI
{
    [CustomEditor(typeof(UITweener), true)]
    public class UITweenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Space(6f);
            SUGUIEditorTools.SetLabelWidth(110f);
            base.OnInspectorGUI();
            DrawCommonProperties();
        }

        protected void DrawCommonProperties()
        {
            UITweener tw = target as UITweener;

            if (SUGUIEditorTools.DrawHeader("Tweener"))
            {
                SUGUIEditorTools.BeginContents();
                SUGUIEditorTools.SetLabelWidth(110f);

                GUI.changed = false;

                UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
                EaseType easeType = (EaseType)EditorGUILayout.EnumPopup("Ease Type", tw.easeType);
                AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve, GUILayout.Width(170f), GUILayout.Height(62f));
                GUILayout.BeginHorizontal();
                float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
                GUILayout.Label("seconds");
                GUILayout.EndHorizontal();

                int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
                bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);

                if (GUI.changed)
                {
                    SUGUIEditorTools.RegisterUndo("Tween Change", tw);
                    tw.easeType = easeType;
                    tw.style = style;
                    tw.ignoreTimeScale = ts;
                    tw.tweenGroup = tg;
                    tw.duration = dur;
                    tw.delay = del;
                    GOGUITools.SetDirty(tw);
                }
                SUGUIEditorTools.EndContents();
            }

            SUGUIEditorTools.SetLabelWidth(80f);
            SUGUIEditorTools.DrawEvents("On Finished", tw, tw.onFinished);
        }
    }
}