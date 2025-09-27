using UnityEngine;
using UnityEditor;

namespace MUGUI
{

    [CustomEditor(typeof(AdvancedText), true)]
    public class AdvancedTextEditor : UnityEditor.UI.GraphicEditor
    {
        AdvancedText adt;
        GUIStyle gs;
        SerializedProperty s_OriginText;
        SerializedProperty s_Text;
        SerializedProperty s_FontData;
        SerializedProperty s_setId;
        SerializedProperty s_auto;
        SerializedProperty s_copy;
        SerializedProperty s_input;
        GUIContent g1, g2, g3, g4, g5;
        protected override void OnEnable()
        {
            base.OnEnable();
            adt = target as AdvancedText;
            gs = new GUIStyle();
            gs.normal.textColor = new Color(0.8f, 0.3f, 0.7f);
            //struct 类型的返回值以值传递，需要接收后更改。
            gs.fontSize = 15;
            s_OriginText = serializedObject.FindProperty("s_OriginalText");
            s_Text = serializedObject.FindProperty("m_Text");
            s_FontData = serializedObject.FindProperty("m_FontData");
            s_setId = serializedObject.FindProperty("s_setId");
            s_auto = serializedObject.FindProperty("s_auto");
            s_copy = serializedObject.FindProperty("s_copy");
            s_input = serializedObject.FindProperty("s_input");
            g1 = new GUIContent("内容");
            g2 = new GUIContent("表情集编号");
            g3 = new GUIContent("自适应单图");
            g4 = new GUIContent("允许复制");
            g5 = new GUIContent("禁用color 表情 下划线");
        }
        public override void OnInspectorGUI()
        {

            serializedObject.Update();
            EditorGUILayout.PropertyField(s_OriginText, g1);
            EditorGUILayout.PropertyField(s_setId, g2);
            EditorGUILayout.PropertyField(s_auto, g3);
            EditorGUILayout.PropertyField(s_copy, g4);
            EditorGUILayout.PropertyField(s_input, g5);
            GUILayout.Label("______________________________________________________________________", gs);

            EditorGUILayout.PropertyField(s_Text);
            EditorGUILayout.PropertyField(s_FontData);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}